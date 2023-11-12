
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.DirectoryServices;
using System.Threading;
using System.Windows.Documents;
using Tennis.DAO;
using Tennis.Factory;

namespace Tennis.Objects
{
    public class Tournament
    {
        private int id;
        private string name;

        private List<Court> courtList = new List<Court>();
        private List<Referee> refereeList = new List<Referee>();
        private List<Schedule> scheduleList = new List<Schedule>();

        private DateTime currentDate = DateTime.Now;
        private int currentMatchHours = 0;

        private bool CancellationPending = false;

        public Tournament(string name)
        {
            this.name = name;

            AbstractDAOFactory factory = AbstractDAOFactory.GetFactory(DAOFactoryType.MS_SQL_FACTORY);

            RefereeDAO refereeDAO = (RefereeDAO)factory.GetRefereeDAO();
            CourtDAO courtDAO = (CourtDAO)factory.GetCourtDAO();

            this.refereeList = refereeDAO.FindAll();
            this.courtList = courtDAO.FindAll();

            SkipNewDay();
            //Create();
        }

        public DateTime CurrentDate { get { return currentDate; } }
        public List<Schedule> ScheduleList { get { return scheduleList; } }
        public string Name { get { return name; } }
        public int Id { get { return id; } set { this.id = value; } }

        public void Create()
        {
            Array types = Enum.GetValues(typeof(ScheduleType));

            AbstractDAOFactory factory = AbstractDAOFactory.GetFactory(DAOFactoryType.MS_SQL_FACTORY);
            PlayerDAO playerDAO = (PlayerDAO)factory.GetPlayerDAO();

            List<Player> players = playerDAO.FindAll();

            if (players.Count != 256)
            {
                throw new Exception("Can't load players " + players.Count);
            }

            DAO<Schedule> scheduleDAO = factory.GetScheduleDAO();

            foreach (ScheduleType type in types)
            {
                Schedule temp = new Schedule(this, type, new List<Player>(players));

                scheduleList.Add(temp);
                scheduleDAO.Create(temp);
            }
        }

        public void Play()
        {
            // Start every schedules in a separate thread
            foreach (Schedule type in scheduleList)
            {
                Thread t = new Thread(new ThreadStart(() =>
                {
                    while (type.HasNextRound() && !CancellationPending)
                    {
                        type.PlayNextRound();
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
                // Randomly sort the refreeList
                refereeList.Sort((a, b) =>
                {
                    return new Random().Next(-1, 2);
                });

                foreach (Referee item in refereeList)
                {
                    // Same here lock the referee to be sure there is not other modification
                    lock (item)
                    {
                        if (item.Match != null && item.Match.Date == match.Date)
                        {
                            continue;
                        }

                        if (item.Available(match))
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
                // Randomly sort the refreeList
                courtList.Sort((a, b) =>
                {
                    return new Random().Next(-1, 2);
                });

                foreach (Court item in courtList)
                {
                    // Same here lock the court to be sure there is not other modification
                    lock (item)
                    {
                        if (item.Match != null && item.Match.Date == match.Date)
                        {
                            continue;
                        }

                        if (item.Available(match))
                        {
                            court = item;
                            break;
                        }
                    }
                }
            }

            return court;
        }

        public void SkipNewDay()
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
    }
}