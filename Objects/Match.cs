
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Tennis.Objects
{
    public class Match : INotifyPropertyChanged
    {
        /// <summary>
        /// Attributes
        /// </summary>
        private DateTime date;
        private TimeSpan duration;

        private int round;
        private int currentSet = 0;
        private int matchSets = 0;

        private Court? court;
        private Schedule schedule;
        private Referee referee;
        private List<Opponent> opponents;
        private List<Set> sets;
        private Opponent? winner;

        private bool isFinished = false;

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="schedule"></param>
        /// <param name="opponents"></param>
        public Match(Schedule schedule, List<Opponent> opponents)
        {
            this.schedule = schedule;
            this.opponents = opponents;
            this.sets = new List<Set>();

        }

        /// <summary>
        /// Getters and Setters
        /// </summary>
        public List<Opponent> Oppnents { get { return opponents; } set { opponents = value; } }
        public Schedule Schedule { get { return schedule; } }
        public Referee Referee { get { return referee; } set { referee = value; } }
        public DateTime Date { get { return date; } set { date = value; } }
        public int CurrentSet { get { return currentSet; } }
        public Opponent? Winner { get { return GetWinner(); } }

        public bool IsPlayed { get { return IsMatchPlayed(); } }


        public int Round { get { return round; } set { this.round = value; } }

        /// <summary>
        /// WPF Getters
        /// </summary>
        /// 
        public Set ActualSet { get { return sets[sets.Count - 1]; } set {  } }
        public int SetsOpponentA { get { return ScoreOpponentA(); }}
        public int SetsOpponentB { get { return ScoreOpponentB(); }}

        /// <summary>
        /// Methods
        /// </summary>
        public void AddSet(Set set)
        {
            this.sets.Add(set);
        }

        public virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            //this.CheckForPropertyErrors();

            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Play()
        {
            Set temp;

            matchSets = schedule.NbWinningSets();

            Console.WriteLine("*************** Match arbitré par " + referee + " ***************");
            Console.WriteLine("prévu le  " + date);

            for (currentSet = 1; currentSet <= matchSets; currentSet++)
            {
                temp = new Set(this);
                AddSet(temp);
                temp.Play();

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

            int scorePlayerA = ScoreOpponentA();
            int scorePlayerB = ScoreOpponentB();

            isFinished = true;
            Console.WriteLine("Fin du match");
            Console.WriteLine("Scores : " + scorePlayerA + " - " + scorePlayerB);
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

    }
}