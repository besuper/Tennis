
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Tennis.DAO;
using Tennis.Factory;

namespace Tennis.Objects
{
    public class Match
    {
        /// <summary>
        /// Attributes
        /// </summary>
        private int id;
        private DateTime date;
        private TimeSpan duration;

        private int round;
        private int currentSet = 0;
        private int matchSets = 0;

        private Court? court;
        private readonly Schedule schedule;
        private Referee? referee;
        private readonly List<Opponent> opponents;
        private List<Set> sets = new List<Set>();
        private Opponent? winner;

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
        /// Getters and Setters
        /// </summary>
        public int Id { get { return id; } set { this.id = value; } }
        public List<Opponent> Oppnents { get { return opponents; } }
        public Schedule Schedule { get { return schedule; } }
        public Referee? Referee { get { return referee; } set { referee = value; } }
        public Court? Court { get { return court; } set { court = value; } }
        public DateTime Date { get { return date; } set { date = value; } }
        public Opponent? Winner { get { return GetWinner(); } }
        public bool IsPlayed { get { return IsMatchPlayed(); } }
        public int Round { get { return round; } set { this.round = value; } }
        public TimeSpan Duration { get { return duration; } }

        /// <summary>
        /// WPF Getters
        /// </summary>
        /// 
        public Set ActualSet { get { return sets[sets.Count - 1]; } set { } }
        public int SetsOpponentA { get { return ScoreOpponentA(); } }
        public int SetsOpponentB { get { return ScoreOpponentB(); } }
        public bool IsFinished { get { return isFinished; } }
        public List<Set> Sets { get{ return sets; } }


        /// <summary>
        /// Methods
        /// </summary>
        public void AddSet(Set set)
        {
            this.sets.Add(set);
        }

        public void Play()
        {
            DAO<Set> setDAO = AbstractDAOFactory.Factory.GetSetDAO();

            Set temp;

            matchSets = schedule.NbWinningSets();

            for (currentSet = 1; currentSet <= matchSets; currentSet++)
            {
                temp = new Set(this);
                AddSet(temp);
                //Create a set for the game
                setDAO.Create(temp);
                temp.Play();
                //Add a winner
                if (temp.Winner != null)
                {
                    setDAO.Update(temp);
                }

                int SetsPlayerA = ScoreOpponentA();
                int SetsPlayerB = ScoreOpponentB();

                if (matchSets == 3)
                {
                    // Si le joueur A a au moins gagné 2 set, il gagne
                    if (SetsPlayerA >= 2)
                    {
                        Console.WriteLine(opponents[0] + " a remporté le matche!!");

                        break;
                    }

                    // Si le joueur B a au moins gagné 2 set, il gagne
                    if (SetsPlayerB >= 2)
                    {
                        Console.WriteLine(opponents[1] + " a remporté le matche!!");

                        break;
                    }
                }
                else if (matchSets == 5)
                {
                    // Si le joueur A a au moins gagné 3 set, il gagne
                    if (SetsPlayerA >= 3)
                    {
                        Console.WriteLine(opponents[0] + " a remporté le matche!!");

                        break;
                    }

                    // Si le joueur B a au moins gagné 3 set, il gagne
                    if (SetsPlayerB >= 3)
                    {
                        Console.WriteLine(opponents[1] + " a remporté le matche!!");

                        break;
                    }
                }
            }

            // TODO: Remove null verification (match always have a referee)
            if (referee != null)
            {
                referee.Release();
            }

            if (court != null)
            {
                court.Release();
            }

            int scorePlayerA = ScoreOpponentA();
            int scorePlayerB = ScoreOpponentB();

            isFinished = true;
            Console.WriteLine("Fin du match");
            Console.WriteLine("Scores : " + scorePlayerA + " - " + scorePlayerB);

            UpdateSumary();
        }

        public Opponent GetWinner()
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

        public bool IsMatchPlayed()
        {
            return sets.Count > 0;
        }

        public void UpdateSumary()
        {
            foreach(MatchSummary summ in this.summary)
            {
                summ.Update();

                summ.NotifyPropertyChanged("Item");
                summ.NotifyPropertyChanged("CurrentPoint");
                summ.NotifyPropertyChanged("TotalSet");
                summ.NotifyPropertyChanged("TieBreakScore");
            }
        }

    }
}