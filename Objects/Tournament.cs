
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private bool CancellationPending = false;

        public static Dictionary<DateTime, List<Referee>> UnavailableReferees = new Dictionary<DateTime, List<Referee>>();

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

            foreach (ScheduleType type in types)
            {
                Schedule temp = new Schedule(this, type, new List<Player>(players));

                scheduleList.Add(temp);

                Schedule.CreateSchedule(temp);
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
                            //Was set in the ScheduleView
                            TournamentFinished();
                        }

                        if (!IsFinished())
                        {
                            Tournament.Delete(this);
                        }
                    }

                    Debug.WriteLine($"End of thread {this.Name}_{type.Type}");
                }));

                t.Name = $"{this.Name}_{type}";
                t.Start();
            }
        }

        public Referee? GetAvailableReferee(Match match)
        {
            Referee? referee = null;
            Referee? temp = null;

            lock (UnavailableReferees)
            {
                if (!UnavailableReferees.ContainsKey(match.Date))
                {
                    UnavailableReferees[match.Date] = new List<Referee>();
                }

                refereeList.Reverse();

                for (int i = 0; i < refereeList.Count; i++)
                {
                    temp = refereeList[i];

                    if (temp.IsAvailable() && !UnavailableReferees[match.Date].Contains(temp))
                    {
                        referee = temp;
                        UnavailableReferees[match.Date].Add(temp);
                        break;
                    }
                }

                return referee;
            }
        }

        public Court? GetAvailableCourt()
        {
            Court? court = null;
            Court? temp = null;

            Random random = new Random();
            int i = random.Next(courtList.Count);

            for (; i < courtList.Count; i++)
            {
                temp = courtList[i];

                if (temp.IsAvailable())
                {
                    court = temp;
                    break;
                }
            }


            return court;
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