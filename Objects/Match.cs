using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoClasses.Objects
{
    internal class Match
    {
        private int id;
        private DateTime date;
        private Competition competition;
        private Court court;
        private Referee referee;
        private List<GameSet> gameSets = new List<GameSet>();
        private List<Group> groups = new List<Group>();
        private Group winner;

        public List<GameSet> GameSets { get { return gameSets; } }
        public List<Group> Groups { get { return groups; } }

        public DateTime Date { get { return date; } set { date = value; } }
        public Court Court { get { return court; } set { court = value; } }


        public int CurrentSet { get; set; }

        public int MatchSets { get; set; }

        public Match(int matchSets, params Group[] groups)
        {
            this.MatchSets = matchSets;
            this.groups.Add(groups[0]);
            this.groups.Add(groups[1]);
        }

        public void StartMatch()
        {
            GameSet temp;

            for (CurrentSet = 1; CurrentSet <= MatchSets; CurrentSet++)
            {
                temp = new GameSet(this);
                gameSets.Add(temp);
                temp.StartSet();

                int SetsPlayerA = ScorePlayerA();
                int SetsPlayerB = ScorePlayerB();

                if (MatchSets == 3)
                {
                    // Si le joueur A a au moins gagné 2 set, il gagne
                    if (SetsPlayerA >= 2)
                    {
                        //Console.WriteLine("Le joueur A a remporté le matche!!");
                        Console.WriteLine(groups[0].GetGroupName() + " a remporté le matche!!");

                        break;
                    }

                    // Si le joueur B a au moins gagné 2 set, il gagne
                    if (SetsPlayerB >= 2)
                    {
                        //Console.WriteLine("Le joueur B a remporté le matche!!");
                        Console.WriteLine(groups[1].GetGroupName() + " a remporté le matche!!");

                        break;
                    }
                }
                else if (MatchSets == 5)
                {
                    // Si le joueur A a au moins gagné 3 set, il gagne
                    if (SetsPlayerA >= 3)
                    {
                        //Console.WriteLine("Le joueur A a remporté le matche!!");
                        Console.WriteLine(groups[0].GetGroupName() + " a remporté le matche!!");

                        break;
                    }

                    // Si le joueur B a au moins gagné 3 set, il gagne
                    if (SetsPlayerB >= 3)
                    {
                        //Console.WriteLine("Le joueur B a remporté le matche!!");
                        Console.WriteLine(groups[1].GetGroupName() + " a remporté le matche!!");

                        break;
                    }
                }
            }

            Console.WriteLine("Fin du match");
            Console.WriteLine("Scores : " + ScorePlayerA() + " - " + ScorePlayerB());
        }

        public int ScorePlayerA()
        {
            int SetsPlayerA = 0;

            foreach (GameSet gameSet in gameSets)
            {
                SetsPlayerA += gameSet.SetPlayerA;
            }

            return SetsPlayerA;
        }

        public int ScorePlayerB()
        {
            int SetsPlayerB = 0;

            foreach (GameSet gameSet in gameSets)
            {
                SetsPlayerB += gameSet.SetPlayerB;
            }

            return SetsPlayerB;
        }
    }
}
