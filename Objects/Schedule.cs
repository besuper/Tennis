
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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

        public Schedule(int id, ScheduleType type, Tournament tournament)
        {
            this.id = id;
            this.type = type;
            this.tournament = tournament;
            // matches = Match.GetAllMatchesFromSchedule(this);

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

        public Opponent? Winner { get { return this.GetWinner(); } }
        public ObservableCollection<MatchSummary>? LastMatchSummary { get { return this.matches.Last().MatchSummary; } }

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

            // Play everything matches of the same date at the same time
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

        private void PlayMatch(Match match)
        {
            // Check if tournament is cancelled
            if (CancellationPending)
            {
                return;
            }

            // Find a referee
            Referee? foundReferee = null;

            while (foundReferee == null)
            {
                foundReferee = tournament.GetAvailableReferee(match);
                Thread.Sleep(20);
            }

            match.Referee = foundReferee;

            // Find a court
            Court? foundCourt = null;

            while (foundCourt == null)
            {
                foundCourt = tournament.GetAvailableCourt(match);
                Thread.Sleep(20);
            }

            match.Court = foundCourt;

            // Create match into database
            MatchDAO matchDAO = (MatchDAO)AbstractDAOFactory.Factory.GetMatchDAO();
            matchDAO.Create(match);

            // Insert Oppopnents and Players in database
            OpponentDAO opponentDAO = (OpponentDAO)AbstractDAOFactory.Factory.GetOpponentDAO();

            foreach(Opponent o in match.Opponents)
            {
                opponentDAO.Create(o);
                opponentDAO.AddPlayer(o);

            }

            foreach (var opponent in match.Opponents)
            {
                matchDAO.AddOpponent(match, opponent);
            }

            match.Play();

            NotifyPropertyChanged("NewMatch");

            if(!match.IsFinished)
            {
                throw new Exception("Match not finished after play()");
            }

            winners.Add(match.GetWinner()!);
        }

        private List<Match> CreateMatches(List<Opponent> opponents)
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

        private List<Opponent> MakeGroups()
        {
            // TODO: Remove this DAO to remove waiting tournament creation
            // FIXME: Correctly load opponents not created when closed the tournament before end
            //OpponentDAO opponentDAO = (OpponentDAO)AbstractDAOFactory.Factory.GetOpponentDAO();

            //DAO create opponent
            //Loop, add player to opponent

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
                    players = players.FindAll(m => m.Gender == GenderType.Man);
                    break;
                case ScheduleType.LadiesSingle:
                case ScheduleType.LadiesDouble:
                    players = players.FindAll(w => w.Gender == GenderType.Woman);
                    break;
            }

            if (players.Count < (countGroups * countPlayerPerGroup))
            {
                throw new Exception("Not enough players for " + type);
            }

            Opponent temp;
            Random rand = new Random();
            Player currentPlayer;
            int currentIndex;

            if (type == ScheduleType.MixedDouble)
            {
                List<Player> mens = players.FindAll(m => m.Gender == GenderType.Man);
                List<Player> womens = players.FindAll(w => w.Gender == GenderType.Woman);

                Console.WriteLine(countGroups);

                for (int i = 0; i < countGroups; i++)
                {
                    temp = new Opponent();
                    //opponentDAO.Create(temp);

                    //Select a man
                    currentIndex = rand.Next(mens.Count);
                    currentPlayer = mens[currentIndex];
                    //opponentDAO.AddPlayer(temp, currentPlayer);

                    temp.AddPlayer(currentPlayer);
                    mens.Remove(currentPlayer);

                    //Select a woman
                    currentIndex = rand.Next(womens.Count);
                    currentPlayer = womens[currentIndex];
                    //opponentDAO.AddPlayer(temp, currentPlayer);

                    temp.AddPlayer(currentPlayer);
                    womens.Remove(currentPlayer);

                    opponents.Add(temp);
                }

                return opponents;
            }

            // Debug.WriteLine(type);
            // Debug.WriteLine(players.Count);

            for (int i = 0; i < countGroups; i++)
            {
                temp = new Opponent();
                //opponentDAO.Create(temp);


                for (int j = 0; j < countPlayerPerGroup; j++)
                {
                    currentIndex = rand.Next(players.Count);
                    currentPlayer = players[currentIndex];
                    //opponentDAO.AddPlayer(temp, currentPlayer);

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

        // Load Matches from database
        public void LoadMatches()
        {
            this.matches = Match.GetAllMatchesFromSchedule(this);

            scheduleWinner = this.matches.Last().GetWinner();
        }

        public static List<Schedule> GetAllScheduleFromTournament(Tournament tournament)
        {
            ScheduleDAO scheduleDAO = (ScheduleDAO)AbstractDAOFactory.Factory.GetScheduleDAO();

            return scheduleDAO.GetAllScheduleFromTournamen(tournament);
        }
    }
}