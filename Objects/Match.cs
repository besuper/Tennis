
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Tennis.DAO;
using Tennis.Factory;

namespace Tennis.Objects
{
    public class Match : INotifyPropertyChanged
    {
        /// <summary>
        /// Attributes
        /// </summary>
        private int id;
        private DateTime date;
        private TimeSpan duration = new TimeSpan(0, 0, 0);

        private int round;
        private int matchSets = 0;

        private Court court;
        private Referee referee;
        private List<Set> sets = new List<Set>();
        private readonly Schedule schedule;
        private readonly List<Opponent> opponents;

        public ObservableCollection<MatchSummary> summary = new ObservableCollection<MatchSummary>();

        private bool isFinished = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="schedule"></param>
        /// <param name="opponents"></param>
        public Match(Schedule schedule, List<Opponent> opponents)
        {
            this.schedule = schedule;
            this.opponents = opponents;

            for (int i = 0; i < opponents.Count; i++)
            {
                summary.Add(new MatchSummary(i, this));
            }
        }

        /// <summary>
        /// Constructor from database loading
        /// </summary>
        public Match(int id, DateTime date, TimeSpan duration, int round, int idReferee, int idCourt, Schedule schedule)
        {
            this.id = id;
            this.date = date;
            this.duration = duration;
            this.round = round;

            this.referee = Referee.GetById(idReferee);
            this.court = Court.GetById(idCourt);

            this.schedule = schedule;

            this.opponents = Opponent.GetOpponnentFromMatch(this);

            this.sets = Set.GetAllSetsFromMatch(this);

            //Create a MatchSummary
            for (int i = 0; i < opponents.Count; i++)
            {
                summary.Add(new MatchSummary(i, this));
                summary[i].TotalSet = i == 0 ? $"{ScoreOpponentA()}" : $"{ScoreOpponentB()}";
            }
            //Loaded match so already played
            isFinished = true;
        }

        /// <summary>
        /// Getters and Setters
        /// </summary>
        public int Id { get { return id; } set { this.id = value; } }
        public List<Opponent> Opponents { get { return opponents; } }
        public Schedule Schedule { get { return schedule; } }
        public Referee Referee { get { return referee; } set { referee = value; } }
        public Court Court { get { return court; } set { court = value; } }
        public DateTime Date { get { return date; } set { date = value; } }
        public bool IsPlayed { get { return IsMatchPlayed(); } }
        public int Round { get { return round; } set { this.round = value; } }
        public TimeSpan Duration { get { return duration; } }
        public ObservableCollection<MatchSummary> MatchSummary { get { return summary; } }

        /// <summary>
        /// WPF Getters
        /// </summary>
        /// 
        public Set ActualSet { get { return sets[sets.Count - 1]; } set { } }
        public bool IsFinished { get { return isFinished; } }
        public List<Set> Sets { get { return sets; } }
        public Opponent? Winner { get { return GetWinner(); } }

        public event PropertyChangedEventHandler? PropertyChanged;

        public virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Methods
        /// </summary>
        public void Play()
        {
            Set temp;

            matchSets = schedule.NbWinningSets();

            for (int currentSet = 1; currentSet <= matchSets; currentSet++)
            {
                temp = new Set(this);
                this.sets.Add(temp);

                this.AddDuration(minutes: 15);

                //Create a set for the game
                Set.CreateSet(temp);

                temp.Play();

                //Add a winner
                Set.UpdateSet(temp);

                int SetsPlayerA = ScoreOpponentA();
                int SetsPlayerB = ScoreOpponentB();

                if (matchSets == 3)
                {
                    // Si le joueur A ou B à au moins gagné 2 set, il gagne
                    if (SetsPlayerA >= 2 || SetsPlayerB >= 2)
                    {
                        break;
                    }
                }
                else if (matchSets == 5)
                {
                    // Si le joueur A ou B à au moins gagné 3 set, il gagne
                    if (SetsPlayerA >= 3 || SetsPlayerB >= 3)
                    {
                        break;
                    }
                }
            }

            referee.Release();
            court.Release();

            //Remove unnecessary data from concurrent dictionary, we don't care about out value but it's required
            Tournament.UnavailableReferees.Remove(Date, out List<Referee>? tempRef);
            Tournament.UnavailableCourts.Remove(Date, out List<Court>? tempCourt);

            //Update le match pour la duration, pas opti
            MatchDAO matchDAO = (MatchDAO)AbstractDAOFactory.Factory.GetMatchDAO();
            matchDAO.Update(this);

            isFinished = true;

            UpdateSumary();

            NotifyPropertyChanged("Winner");
            NotifyPropertyChanged("IsFinished");
        }

        public Opponent? GetWinner()
        {
            if (isFinished)
            {
                return ScoreOpponentA() > ScoreOpponentB() ? opponents[0] : opponents[1];
            }

            return null;
        }

        public int ScoreOpponentA()
        {
            int SetsOpponentA = 0;

            foreach (Set gameSet in sets)
            {
                if (gameSet.Winner != null && gameSet.Winner == opponents[0])
                {
                    SetsOpponentA++;
                }
            }

            return SetsOpponentA;
        }

        public int ScoreOpponentB()
        {
            int SetsOpponentB = 0;

            foreach (Set gameSet in sets)
            {
                if (gameSet.Winner != null && gameSet.Winner == opponents[1])
                {
                    SetsOpponentB++;
                }
            }

            return SetsOpponentB;
        }

        public bool IsWinningSet()
        {
            return ((matchSets == 5 && ScoreOpponentA() == 2) || (matchSets == 3 && ScoreOpponentA() == 1)) && ScoreOpponentA() == ScoreOpponentB();
        }

        private bool IsMatchPlayed()
        {
            return sets.Count > 0;
        }

        public void UpdateSumary()
        {
            foreach (MatchSummary summ in this.summary)
            {
                summ.Update();

                summ.NotifyPropertyChanged("Item");
                summ.NotifyPropertyChanged("CurrentPoint");
                summ.NotifyPropertyChanged("TotalSet");
            }
        }

        private void AddDuration(int minutes)
        {
            this.duration = this.duration.Add(new TimeSpan(0, minutes, 0));
            NotifyPropertyChanged("Duration");
        }

        /// <summary>
        /// DAO Methods
        /// </summary>

        public static void CreateMatch(Match match)
        {
            MatchDAO matchDAO = (MatchDAO)AbstractDAOFactory.Factory.GetMatchDAO();

            matchDAO.Create(match);
        }

        public static void AddOpponent(Match match, Opponent opponent)
        {
            MatchDAO matchDAO = (MatchDAO)AbstractDAOFactory.Factory.GetMatchDAO();

            matchDAO.AddOpponent(match, opponent);
        }

        public static List<Match> GetAllMatchesFromSchedule(Schedule schedule)
        {
            MatchDAO matchDAO = (MatchDAO)AbstractDAOFactory.Factory.GetMatchDAO();

            return matchDAO.GetAllMatchesFromSchedule(schedule);
        }

        internal static DateTime? GetLastDateFromLastTournament()
        {
            MatchDAO matchDAO = (MatchDAO)AbstractDAOFactory.Factory.GetMatchDAO();

            return matchDAO.GetLastDateFromLastTournament();
        }
    }
}