
using System;
using System.Collections.Generic;
using System.Threading;
using Tennis.DAO;
using Tennis.Factory;

namespace Tennis.Objects
{
    public class Tournament
    {
        /// <summary>
        /// Attributes
        /// </summary>
        /// 
        private int id;
        private readonly string name;

        private List<Court> courtList = new List<Court>();
        private List<Referee> refereeList = new List<Referee>();
        private List<Schedule> scheduleList = new List<Schedule>();

        private DateTime currentDate = DateTime.Now;
        private int currentMatchHours = 0;

        private bool CancellationPending = false;

        /// <summary>
        /// Constructor from create new tournament
        /// </summary>
        /// <param name="name"></param>
        public Tournament(string name)
        {
            this.name = name;

            TournamentDAO tournamentDAO = (TournamentDAO)AbstractDAOFactory.Factory.GetTournamentDAO();

            tournamentDAO.Create(this);

            this.refereeList = Referee.GetAll();
            this.courtList = Court.GetAll();

            if (courtList.Count == 0)
            {
                throw new Exception("Can't load courts");
            }

            if (refereeList.Count == 0)
            {
                throw new Exception("Can't load referees");
            }

            SkipNewDay();
        }

        /// <summary>
        /// Constructor from database
        /// </summary>
        /// <param name="id"></param>
        /// <param name="match"></param>
        public Tournament(int id, string name)
        {
            this.id = id;
            this.name = name;
        }

        /// <summary>
        /// Getters and Setters
        /// </summary>
        /// 
        public DateTime CurrentDate { get { return currentDate; } }
        public List<Schedule> ScheduleList { get { return scheduleList; } }
        public string Name { get { return name; } }
        public int Id { get { return id; } set { this.id = value; } }

        public delegate void TournamentFinishedDelegate();
        public TournamentFinishedDelegate? TournamentFinished = null;

        /// <summary>
        /// Methods
        /// </summary>

        public void Create()
        {
            Array types = Enum.GetValues(typeof(ScheduleType));

            List<Player> players = Player.GetAllPlayers();

            // Check: Players count
            //if (players.Count != 256)
            //{
            //    throw new Exception("Can't load players " + players.Count + " instead of 256");
            //}

            DAO<Schedule> scheduleDAO = AbstractDAOFactory.Factory.GetScheduleDAO();

            foreach (ScheduleType type in types)
            {
                Schedule temp = new Schedule(this, type, new List<Player>(players));

                scheduleList.Add(temp);
                scheduleDAO.Create(temp);
            }
        }

        public void Play()
        {
            int count = 0;
            // Start every schedules in a separate thread
            foreach (Schedule type in scheduleList)
            {
                Thread t = new Thread(new ThreadStart(() =>
                {
                    while (type.HasNextRound() && !CancellationPending)
                    {
                        type.PlayNextRound();
                    }

                    count++;

                    if (count == scheduleList.Count)
                    {
                        if (TournamentFinished != null)
                        {
                            TournamentFinished();
                        }

                        if (!IsFinished())
                        {
                            Tournament.Delete(this);
                        }
                    }
                }));

                t.Start();
            }
        }

        public Referee? GetAvailableReferee(Match match)
        {
            Referee? referee = null;

            // Lock the referee List to avoid simultaneously modifications by threads
            lock (refereeList)
            {
                foreach (Referee item in refereeList)
                {
                    // Same here lock the referee to be sure there is not other modification
                    lock (item)
                    {
                        if (item.Match != null && item.Match.Date == match.Date)
                        {
                            continue;
                        }

                        if (item.IsAvailable(match))
                        {
                            referee = item;
                            break;
                        }
                    }
                }
            }

            return referee;
        }

        public Court? GetAvailableCourt(Match match)
        {
            Court? court = null;

            // Lock the court List to avoid simultaneously modifications by threads
            lock (courtList)
            {
                foreach (Court item in courtList)
                {
                    // Same here lock the court to be sure there is not other modification
                    lock (item)
                    {
                        if (item.Match != null && item.Match.Date == match.Date)
                        {
                            continue;
                        }

                        if (item.IsAvailable(match))
                        {
                            court = item;
                            break;
                        }
                    }
                }
            }

            return court;
        }

        private void SkipNewDay()
        {
            DateTime tempDate = currentDate;
            tempDate = tempDate.AddDays(1);

            currentDate = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, 10, 00, 00);
        }

        public void AddNewMatch()
        {
            if (currentDate.Hour >= 16)
            {
                SkipNewDay();
                return;
            }

            if (currentMatchHours >= 2)
            {
                currentDate = currentDate.AddHours(2);

                currentMatchHours = 0;
            }
            else
            {
                currentMatchHours++;
            }
        }

        public void StopTournament()
        {
            CancellationPending = true;

            // Stop schedules
            foreach (Schedule schedule in scheduleList)
            {
                schedule.StopSchedule();
            }
        }

        public bool IsFinished()
        {
            foreach (Schedule schedule in scheduleList)
            {
                if (schedule.HasNextRound())
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// DAO Methods
        /// </summary>

        public static List<Tournament> GetTournaments()
        {
            TournamentDAO tournamentDAO = (TournamentDAO)AbstractDAOFactory.Factory.GetTournamentDAO();

            return tournamentDAO.FindAll();
        }

        public static bool Delete(Tournament tournament)
        {
            TournamentDAO tournamentDAO = (TournamentDAO)AbstractDAOFactory.Factory.GetTournamentDAO();

            return tournamentDAO.Delete(tournament);
        }
    }
}