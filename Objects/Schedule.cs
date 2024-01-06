
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

        private DateTime currentDate;
        private int currentMatchHours = 0;

        private DateTime matchDay;

        /// <summary>
        /// Constructor
        /// </summary>
        public Schedule(Tournament tournament, ScheduleType type, List<Player> players)
        {
            this.tournament = tournament;
            this.type = type;
            this.players = players;

            this.winners = MakeGroups();

            //currentDate = Match.GetLastDateFromLastTournament();

            //TEST
            DateTime lastMatchDay = Match.GetLastDateFromLastTournament().AddDays(1);
            matchDay = new DateTime(lastMatchDay.Year, lastMatchDay.Month, lastMatchDay.Day, 10, 00, 00);
            //TEST

            //SkipNewDay();
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

            winners.Clear();
            
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

            // Referee begins to referee (means he is not available)
            match.Referee.Match = match;
            match.Court.Match = match;

            match.NotifyPropertyChanged("Referee");
            match.NotifyPropertyChanged("Court");

            // Create match into database
            Match.CreateMatch(match);

            // Insert Oppopnents and Players in database
            foreach (Opponent o in match.Opponents)
            {
                Opponent.CreateOpponent(o);
            }

            foreach (var opponent in match.Opponents)
            {
                Match.AddOpponent(match, opponent);
            }

            match.Play();

            if (!match.IsFinished)
            {
                throw new Exception("Match not finished after play()");
            }

            // Make sure winners is not in concurrent modification (Multiple Tasks adding a winner at the same time)
            lock(winners)
            {
                winners.Add(match.GetWinner()!);
            }
        }

        private List<Match> CreateMatches(List<Opponent> opponents)
        {
            Random random = new Random();

            int indexSelected;

            List<Opponent> tempGroupBattle;
            Match tempMatch;
            List<Match> matches = new List<Match>();

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
                tempMatch.Date = matchDay;

                UpdateMatchDay();
                //UpdateCurrentDate();

                // Try to get an available referee and Court
                Referee? foundReferee = null;
                Court? foundCourt = null;

                do
                {

                    if(foundReferee == null)
                    {
                        foundReferee = tournament.GetAvailableReferee(tempMatch);
                    }

                    if (foundCourt == null)
                    {
                        foundCourt = tournament.GetAvailableCourt(tempMatch);
                    }

                    // If there is no referee available or no court available for this date, report the match
                    if(foundReferee == null || foundCourt == null)
                    {
                        //Ici il est possible que il n'y ait ni arbitre ni court disponible pour cette date donc skip des ann�es
                        tempMatch.Date = matchDay;
                        UpdateMatchDay();
                        //UpdateCurrentDate();
                    }
                    
                } while (foundReferee == null || foundCourt == null);


                // The referee and court are planned
                tempMatch.Referee = foundReferee;
                tempMatch.Court = foundCourt;

                matches.Add(tempMatch);
            }

            return matches;
        }

        private List<Opponent> MakeGroups()
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

        /*
        private void SkipNewDay()
        {
            DateTime tempDate = currentDate;
            tempDate = tempDate.AddDays(1);

            currentDate = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, 10, 00, 00);
        }
        */

        public void UpdateMatchDay()
        {
            matchDay = matchDay.AddHours(2);
            //The first match was at 10 am, next match is at 12 pm next 14, next 16 and after that we skip a day

            if (matchDay.Hour > 16)
            {
                DateTime previousMatchDay = matchDay.AddDays(1);
                matchDay = new DateTime(previousMatchDay.Year, previousMatchDay.Month, previousMatchDay.Day, 10, 00, 00);
            }

        }

        /*
        public void UpdateCurrentDate()
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
        */

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

        /// <summary>
        /// DAO Methods
        /// </summary>

        public static void CreateSchedule(Schedule schedule)
        {
            DAO<Schedule> scheduleDAO = AbstractDAOFactory.Factory.GetScheduleDAO();

            scheduleDAO.Create(schedule);
        }

        public static List<Schedule> GetAllScheduleFromTournament(Tournament tournament)
        {
            ScheduleDAO scheduleDAO = (ScheduleDAO)AbstractDAOFactory.Factory.GetScheduleDAO();

            return scheduleDAO.GetAllScheduleFromTournamen(tournament);
        }
    }
}