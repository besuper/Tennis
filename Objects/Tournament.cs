
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.DirectoryServices;

namespace Tennis.Objects
{
    public class Tournament
    {

        private string name;

        private List<Court> courtList = new List<Court>();
        private List<Referee> refereeList = new List<Referee>();
        private List<Schedule> scheduleList = new List<Schedule>();

        private DateTime currentDate = DateTime.Now;
        private int currentMatchHours = 0;

        public Tournament(string name)
        {
            this.name = name;

            this.refereeList.Add(new Referee("Carlos", "Bernardes", "Bresil"));
            this.refereeList.Add(new Referee("Jaume", "Campistol", "Espagne"));
            this.refereeList.Add(new Referee("Pierre", "Bacchi", "France"));
            this.refereeList.Add(new Referee("Ricardo", "Ortiz", "Argentine"));
            this.refereeList.Add(new Referee("John", "Blom", "Australie"));

            SkipNewDay();
            Create();
        }

        public DateTime CurrentDate { get { return currentDate; } }
        public List<Schedule> ScheduleList { get { return scheduleList; } }

        public List<Player> GetPlayers()
        {
            //Put this function in DAO
            List<Player> list = new List<Player>();

            DatabaseManager dm = new DatabaseManager();

            SqlDataReader request = dm.Get("SELECT * FROM Joueur");
            while (request.Read())
            {
                list.Add(new Player(request.GetString(1), request.GetString(2), "", 0, request.GetString(3)));
            }

            request.Close();

            return list;
        }

        public void Create()
        {
            Array types = Enum.GetValues(typeof(ScheduleType));

            List<Player> players = GetPlayers();

            if (players.Count != 255)
            {
                throw new Exception("Can't load players " + players.Count);
            }

            foreach (ScheduleType type in types)
            {
                scheduleList.Add(new Schedule(this, type, new List<Player>(players)));
            }
        }

        public void Play()
        {
            List<Opponent> winner = new List<Opponent>();

            foreach (Schedule type in scheduleList)
            {
                type.PlayNextRound();
                winner.Add(type.GetWinner());
            }

            Console.WriteLine("Voici les winners");

            foreach (var item in winner)
            {
                Console.WriteLine(item);
            }

        }

        public Referee? GetAvailableReferee(Match match)
        {
            Referee? referee = null;

            foreach (Referee item in refereeList)
            {
                if (item.Available(match))
                {
                    referee = item;
                    break;
                }
            }

            return referee;
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

            if (currentMatchHours >= 1)
            {
                currentDate = currentDate.AddHours(2);

                currentMatchHours = 0;
            }
            else
            {
                currentMatchHours++;
            }
        }

    }
}