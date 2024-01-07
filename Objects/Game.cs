using System;
using System.Collections.Generic;
using Tennis.DAO;
using Tennis.Factory;

namespace Tennis.Objects
{
    public class Game
    {
        /// <summary>
        /// Attributes
        /// </summary>
        private int id;
        protected int currentScoreOp1;
        protected int currentScoreOp2;

        private readonly Random rand = new Random();

        protected Opponent? winner;
        private readonly Set set;

        /// <summary>
        /// Getters and Setters
        /// </summary>
        public int Id { get { return id; } set { id = value; } }
        public Set Set { get { return set; } }

        public dynamic CurrentScoreOp1 { get { return currentScoreOp1 > 40 ? "AD" : this.currentScoreOp1; } set { } }
        public dynamic CurrentScoreOp2 { get { return currentScoreOp2 > 40 ? "AD" : this.currentScoreOp2; } set { } }

        public Opponent? Winner { get { return winner; } }

        /// <summary>
        /// Constructor from a classic set
        /// </summary>
        /// <param name="set"></param>
        public Game(Set set)
        {
            this.set = set;
        }

        /// <summary>
        /// Constructor from database loading
        /// </summary>
        public Game(int id, String scoreA, String scoreB, int winner, Set set)
        {
            this.id = id;
            this.currentScoreOp1 = scoreA == "AD" ? 41 : int.Parse(scoreA);
            this.currentScoreOp2 = scoreB == "AD" ? 41 : int.Parse(scoreB);

            //Get the winner by id from match winner id
            this.winner = set.Match.Opponents.Find(x => x.Id == winner);
            this.set = set;
        }

        /// <summary>
        /// Methods
        /// </summary>
        public virtual void Play()
        {
            winner = null;

            // Debug.WriteLine($"\n===========[Nouveau jeu {set.GameScorePlayerA()} - {set.GameScorePlayerB()}]===========");

            // Jouer une partie (un jeu)
            while (winner == null)
            {
                set.Match.UpdateSumary();

                SimulateAGame();

                set.Match.UpdateSumary();

                //Thread.Sleep(10);
            }
        }

        // Logique du dérouelement d'un jeu (points)
        public void SimulateAGame()
        {
            int gagnantTour = Dice();

            // Egalité
            if (currentScoreOp1 == 40 && currentScoreOp2 == 40)
            {
                if (gagnantTour == 0)
                {
                    currentScoreOp1++;

                    // Debug.WriteLine(set.Match.Opponents[0] + " prend l'avantage");
                }
                else
                {
                    currentScoreOp2++;

                    // Debug.WriteLine(set.Match.Opponents[1] + " prend l'avantage");
                }
            }

            // Ajout des points pour chaque gagnant
            // 1er point => 15
            // 2eme => 15
            // 3eme => 10
            if (gagnantTour == 0)
            {
                // Le joueur A était en avantage, il a marqué le second point, il gagne
                // Ou alors il a gagné avec 40 points (la vérification de l'égalité ce fais plus haut!)
                if (currentScoreOp1 == 41 || currentScoreOp1 == 40)
                {
                    // Debug.WriteLine(set.Match.Opponents[0] + " remporte la partie");

                    winner = set.Match.Opponents[0];

                    return;
                }

                // Le joueur B était en avantage, mais il la perdu
                if (currentScoreOp2 == 41)
                {
                    currentScoreOp2--;

                    // Debug.WriteLine(set.Match.Opponents[1] + " perd sont avantage, retour a l'égalité!");

                    return;
                }

                currentScoreOp1 += currentScoreOp1 == 30 ? 10 : 15;

                // Debug.WriteLine(set.Match.Opponents[0] + " : " + currentScoreOp1);
            }
            else
            {
                // Le joueur B était en avantage, il a marqué le second point, il gagne
                // Ou alors il a gagné avec 40 points (la vérification de l'égalité ce fais plus haut!)
                if (currentScoreOp2 == 41 || currentScoreOp2 == 40)
                {
                    // Debug.WriteLine(set.Match.Opponents[1] + " remporte la partie");

                    winner = set.Match.Opponents[1];

                    return;
                }

                // Le joueur A était en avantage, mais il la perdu
                if (currentScoreOp1 == 41)
                {
                    currentScoreOp1--;

                    // Debug.WriteLine(set.Match.Opponents[0] + " perd sont avantage, retour a l'égalité!");

                    return;
                }

                currentScoreOp2 += currentScoreOp2 == 30 ? 10 : 15;

                // Debug.WriteLine(set.Match.Opponents[1] + " : " + currentScoreOp2);
            }
        }

        // Return 0 or 1
        // Determines the winner of an exchange
        protected int Dice()
        {
            return rand.Next(0, 2);
        }

        /// <summary>
        /// Methods
        /// </summary>

        public static void CreateGames(List<Game> games)
        {
            GameDAO gameDAO = (GameDAO)AbstractDAOFactory.Factory.GetGameDAO();

            gameDAO.CreateGames(games);
        }

        public static List<Game> GetAllGamesFromSet(Set set)
        {
            GameDAO gameDAO = (GameDAO)AbstractDAOFactory.Factory.GetGameDAO();
            return gameDAO.GetGamesFromSet(set);
        }

    }
}

