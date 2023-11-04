
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;


namespace Tennis.Objects
{
    public class Schedule : INotifyPropertyChanged
    {

        /// <summary>
        /// Attributes
        /// </summary>
        private readonly ScheduleType type;

        private int actualRound = 0;
        private readonly Tournament tournament;
        private List<Match> matches = new List<Match>();
        private List<Player> players;

        private Opponent? scheduleWinner;

        /// <summary>
        /// Constructor
        /// </summary>
        public Schedule(Tournament tournament, ScheduleType type, List<Player> players)
        {
            this.tournament = tournament;
            this.type = type;
            this.players = players;
        }

        /// <summary>
        /// Getters and Setters
        /// </summary>
        public List<Match> Matches { get { return matches; } }
        public ScheduleType Type { get { return type; } }

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
            List<Opponent> winners = MakeGroups();

            do
            {
                actualRound++;

                Console.WriteLine("/!\\ /!\\ /!\\ /!\\ /!\\ /!\\ /!\\ /!\\ /!\\ /!\\ Playing round " + actualRound + " /!\\ /!\\ /!\\ /!\\ /!\\ /!\\ /!\\ /!\\ /!\\ /!\\");
                Debug.WriteLine("New round " + type + " " + actualRound);
                Debug.WriteLine(winners.Count);

                matches = CreateMatches(winners);
                NotifyPropertyChanged("Matches");

                winners = new List<Opponent>();

                /*if (matches.Count() <= 2)
                {
                    PlayMatches(matches, winners);
                }
                else
                {
                    IEnumerable<Match[]> chunks = matches.Chunk(matches.Count() / 4);
                    List<Thread> threads = new List<Thread>();

                    foreach (Match[] chunks2 in chunks)
                    {
                        threads.Add(new Thread(() => PlayMatches(chunks2, winners)));
                    }

                    foreach (Thread thread in threads)
                    {
                        thread.Start();
                    }

                    foreach (Thread thread in threads)
                    {
                        thread.Join();
                    }
                }*/
                PlayMatches(matches, winners);
            } while (winners.Count != 1);

            scheduleWinner = winners[0];
        }

        public void PlayMatches(dynamic matches, List<Opponent> winners)
        {
            foreach (Match match in matches)
            {
                match.Referee = tournament.GetAvailableReferee(match);
                match.Play();
                Console.WriteLine("Le winner est " + match.GetWinner());
                winners.Add(match.GetWinner());
            }
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
                // TODO: If referee is null ?
                //tempMatch.Referee = tournament.GetAvailableReferee(tempMatch);
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

        public Opponent GetWinner()
        {
            return scheduleWinner;
        }

    }
}