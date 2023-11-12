
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Tennis.DAO;
using Tennis.Factory;

namespace Tennis.Objects
{
    public class Schedule : INotifyPropertyChanged
    {

        /// <summary>
        /// Attributes
        /// </summary>
        private readonly ScheduleType type;

        private int id;
        private int actualRound = 0;
        private readonly Tournament tournament;
        private List<Match> matches = new List<Match>();
        private List<Player> players;
        private List<Opponent> winners = new List<Opponent>();

        private Opponent? scheduleWinner;

        private bool CancellationPending = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public Schedule(Tournament tournament, ScheduleType type, List<Player> players)
        {
            this.tournament = tournament;
            this.type = type;
            this.players = players;

            this.winners = MakeGroups();
        }

        /// <summary>
        /// Getters and Setters
        /// </summary>
        public List<Match> Matches { get { return matches; } }
        public ScheduleType Type { get { return type; } }
        public int Id { get { return id; } set { this.id = value; } }
        public Tournament Tournament { get { return tournament; } }

        /// <summary>
        /// WPF
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        public virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Methods
        /// </summary>
        public int NbWinningSets()
        {
            int nb = 0;
            switch (type)
            {
                case ScheduleType.LadiesDouble:
                case ScheduleType.LadiesSingle:
                case ScheduleType.MixedDouble:
                    nb = 3;
                    break;
                case ScheduleType.GentlemenSingle:
                case ScheduleType.GentlemenDouble:
                    nb = 5;
                    break;
            }

            return nb;
        }

        public void PlayNextRound()
        {
            actualRound++;

            Debug.WriteLine("New round " + type + " " + actualRound);
            Debug.WriteLine(winners.Count);

            matches = CreateMatches(winners);

            // Notify the window to update its listview
            NotifyPropertyChanged("NewMatches");

            winners = new List<Opponent>();

            // Play everything matches of the some date at the same time
            DateTime dateRef = matches[0].Date;
            int matchIndex = 0;

            while (matchIndex < matches.Count)
            {
                List<Task> tasks = new List<Task>();
                dateRef = matches[matchIndex].Date;

                // Get matches of date
                for (int i = matchIndex; i < matches.Count; i++)
                {
                    Match match = matches[i];

                    if (match.Date != dateRef)
                    {
                        continue;
                    }

                    Task temp = new Task(() => PlayMatch(match));
                    temp.Start();
                    tasks.Add(temp);
                }

                Task.WaitAll(tasks.ToArray());

                matchIndex += tasks.Count;
            }

            // TODO: Remove this debug
            /*Debug.WriteLine($"Matches finished {matches.Count}");

            foreach (Match match in matches)
            {
                if(!match.IsFinished || !match.IsPlayed)
                {
                    throw new Exception("Match not finished or not started!!!");
                }
            }*/

            if (winners.Count == 1)
            {
                scheduleWinner = winners[0];
            }
        }

        public void PlayMatch(Match match)
        {
            // Check if tournament is cancelled
            if (CancellationPending)
            {
                return;
            }

            while (match.Referee == null)
            {
                match.Referee = tournament.GetAvailableReferee(match);
                Thread.Sleep(20);
            }

            while (match.Court == null)
            {
                match.Court = tournament.GetAvailableCourt(match);
                Thread.Sleep(20);
            }

            AbstractDAOFactory factory = AbstractDAOFactory.GetFactory(DAOFactoryType.MS_SQL_FACTORY);
            DAO<Match> matchDAO = factory.GetMatchDAO();

            matchDAO.Create(match);

            match.Play();

            NotifyPropertyChanged("NewMatch");

            winners.Add(match.GetWinner());
        }

        public List<Match> CreateMatches(List<Opponent> opponents)
        {
            List<Match> matches = new List<Match>();
            Random random = new Random();

            int indexSelected;

            List<Opponent> tempGroupBattle;
            Match tempMatch;

            while (opponents.Count != 0)
            {
                tempGroupBattle = new List<Opponent>();

                for (int i = 0; i < 2; i++)
                {
                    indexSelected = random.Next(0, opponents.Count);
                    tempGroupBattle.Add(opponents[indexSelected]);
                    opponents.Remove(opponents[indexSelected]);
                }

                tempMatch = new Match(this, tempGroupBattle);
                tempMatch.Round = actualRound;
                tempMatch.Date = tournament.CurrentDate;

                tournament.AddNewMatch();

                matches.Add(tempMatch);
            }


            return matches;
        }

        public List<Opponent> MakeGroups()
        {
            List<Opponent> opponents = new List<Opponent>();
            int countGroups = 64;
            int countPlayerPerGroup = 2;

            switch (type)
            {
                case ScheduleType.GentlemenSingle:
                case ScheduleType.LadiesSingle:
                    countGroups = 128;
                    countPlayerPerGroup = 1;
                    break;

                case ScheduleType.GentlemenDouble:
                case ScheduleType.LadiesDouble:
                    countGroups = 64;
                    countPlayerPerGroup = 2;
                    break;
                case ScheduleType.MixedDouble:
                    countGroups = 32;
                    break;
            }

            switch (type)
            {
                case ScheduleType.GentlemenSingle:
                case ScheduleType.GentlemenDouble:
                    players = players.GetRange(0, 128);
                    break;
                case ScheduleType.LadiesSingle:
                case ScheduleType.LadiesDouble:
                    players = players.GetRange(127, 128);
                    break;
            }

            if (players.Count < (countGroups * countPlayerPerGroup))
            {
                throw new Exception("Not enough players");
            }

            Opponent temp;
            Random rand = new Random();
            Player currentPlayer;
            int currentIndex;

            if (type == ScheduleType.MixedDouble)
            {
                List<Player> mens = players.GetRange(0, 128);
                List<Player> womens = players.GetRange(127, 128);

                Console.WriteLine(countGroups);

                for (int i = 0; i < countGroups; i++)
                {
                    temp = new Opponent();

                    //Select a man
                    currentIndex = rand.Next(mens.Count);
                    currentPlayer = mens[currentIndex];

                    temp.AddPlayer(currentPlayer);
                    mens.Remove(currentPlayer);

                    //Select a woman
                    currentIndex = rand.Next(womens.Count);
                    currentPlayer = womens[currentIndex];

                    temp.AddPlayer(currentPlayer);
                    womens.Remove(currentPlayer);

                    opponents.Add(temp);
                }

                return opponents;
            }

            Debug.WriteLine(type);
            Debug.WriteLine(players.Count);

            for (int i = 0; i < countGroups; i++)
            {
                temp = new Opponent();

                for (int j = 0; j < countPlayerPerGroup; j++)
                {
                    currentIndex = rand.Next(players.Count);
                    currentPlayer = players[currentIndex];

                    temp.AddPlayer(currentPlayer);
                    players.Remove(currentPlayer);
                }

                opponents.Add(temp);
            }

            Debug.WriteLine(opponents.Count);

            return opponents;
        }

        public Opponent? GetWinner()
        {
            return scheduleWinner;
        }

        public bool HasNextRound()
        {
            return winners.Count > 1;
        }

        public void StopSchedule()
        {
            CancellationPending = true;
        }

    }
}