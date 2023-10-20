using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DemoClasses.Objects
{
    internal class GameSet
    {
        private int id;

        private int winner = -1;

        public int Winner { get { return winner; } }

        public Match match { get; set; }

        private List<Game> games = new List<Game>();

        public List<Game> Games { get { return games; } }

        public GameSet(Match match)
        {
            this.match = match;
        }

        public void StartSet()
        {
            Console.WriteLine($"\n===========[Nouveau set {match.ScorePlayerA()} - {match.ScorePlayerB()}]===========");

            Game temp;

            for (int actualGameState = 1; actualGameState <= 12; actualGameState++)
            {
                temp = new Game(this);

                games.Add(temp);
                temp.StartGame();

                int GamePointA = GameScorePlayerA();
                int GamePointB = GameScorePlayerB();

                if (GamePointA >= 6 && (GamePointA - GamePointB) >= 2)
                {
                    // Le joueur A a forcement gagné le SET ici
                    winner = 0;
                    break;
                }

                if (GamePointB >= 6 && (GamePointB - GamePointA) >= 2)
                {
                    // Le joueur B a forcement gagné le SET ici
                    winner = 1;
                    break;
                }

                if (GamePointA == 6 && GamePointB == 6)
                {
                    // Les 12 jeux sont passé MAIS les joueurs sont a égalité, comment donner le point du set?
                    // Faire jouer un tie-break
                    // Si le set est le dernier du match (3 ou 5 sets) alors c'est un super tie-break

                    Random rand = new Random();
                    int rnd = rand.Next(0, 2);

                    if (match.MatchSets == match.CurrentSet)
                    {
                        // super tie-break
                        Console.WriteLine("Super tie-break non pris en charge");

                        // On fais gagner le Set aléatoiremenet temporraielent
                        if (rnd == 0)
                        {
                            winner = 0;
                        }
                        else
                        {
                            winner = 1;
                        }

                        break;
                    }
                    else
                    {
                        // tie-break
                        Console.WriteLine("Tie-break non pris en charge");

                        // On fais gagner le Set aléatoiremenet temporraielent
                        if (rnd == 0)
                        {   
                            winner = 0;
                        }
                        else
                        {
                            winner = 1;
                        }

                        break;
                    }
                }
            }
        }

        public int GameScorePlayerA()
        {
            int GameScoreA = 0;

            foreach (Game game in games)
            {
                if (game.Winner == 0)
                {
                    GameScoreA++;
                }
            }

            return GameScoreA;
        }

        public int GameScorePlayerB()
        {
            int GameScoreB = 0;

            foreach (Game game in games)
            {
                if (game.Winner == 1)
                {
                    GameScoreB++;
                }
            }

            return GameScoreB;
        }
    }
}
