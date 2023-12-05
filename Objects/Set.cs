
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Tennis.DAO;
using Tennis.Factory;

namespace Tennis.Objects
{
    public class Set
    {
        /// <summary>
        /// Attributes
        /// </summary>
        protected int id;
        protected int scoreOp1;
        protected int scoreOp2;

        private readonly Match match;
        protected Opponent? winner;
        private List<Game> games = new List<Game>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="match"></param>
        public Set(Match match)
        {
            this.match = match;
        }

        /// <summary>
        /// Getters and Setters
        /// </summary>
        public Match Match { get { return match; } }
        public Opponent? Winner { get { return winner; } }

        public int Id { get { return id; } set { id = value; } }

        /// <summary>
        /// WPF Getters
        /// </summary>
        public Game ActualGame { get { return games[games.Count - 1]; } }
        public int ScoreOp1 { get { return GameScorePlayerA(); } set { } }
        public int ScoreOp2 { get { return GameScorePlayerB(); } set { } }
        public int WinnerScore { get { return ScoreOp1 > ScoreOp2 ? ScoreOp1 : ScoreOp2; } }

        /// <summary>
        /// Methods
        /// </summary>
        public int GameScorePlayerA()
        {
            int GameScoreA = 0;

            foreach (Game game in games)
            {
                if (game.Winner != null && game.Winner == match.Oppnents[0])
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
                if (game.Winner != null && game.Winner == match.Oppnents[1])
                {
                    GameScoreB++;
                }
            }

            return GameScoreB;
        }

        public void Play()
        {
            DAO<Game> gameDAO= AbstractDAOFactory.Factory.GetGameDAO();

            Debugger.log($"\n===========[Nouveau set {match.ScoreOpponentA()} - {match.ScoreOpponentB()}]===========");

            Game temp;

            for (int actualGameState = 1; actualGameState <= 12; actualGameState++)
            {
                temp = new Game(this);

                games.Add(temp);
                temp.Play();
                gameDAO.Create(temp);

                int GamePointA = GameScorePlayerA();
                int GamePointB = GameScorePlayerB();

                if (GamePointA >= 6 && (GamePointA - GamePointB) >= 2)
                {
                    // Le joueur A a forcement gagn� le SET ici
                    winner = match.Oppnents[0];
                    break;
                }

                if (GamePointB >= 6 && (GamePointB - GamePointA) >= 2)
                {
                    // Le joueur B a forcement gagn� le SET ici
                    winner = Match.Oppnents[1];
                    break;
                }

                if (GamePointA == 6 && GamePointB == 6)
                {
                    // Les 12 jeux sont pass� MAIS les joueurs sont a �galit�, comment donner le point du set?
                    // Faire jouer un tie-break
                    // Si le set est le dernier du match (3 ou 5 sets) alors c'est un super tie-break
                    // Get the last set and replace it by a SuperTieBreak

                    Random rand = new Random();
                    int rnd = rand.Next(0, 2);

                    if (match.IsWinningSet())
                    {
                        // super tie-break
                        SuperTieBreak stb = new SuperTieBreak(match);
                        stb.Play(10);
                        match.AddSet(stb);
                        break;
                    }
                    else
                    {
                        // tie-break
                        Debugger.log("Tie-break non pris en charge");

                        // On fais gagner le Set al�atoiremenet temporraielent
                        if (rnd == 0)
                        {
                            winner = match.Oppnents[0];
                        }
                        else
                        {
                            winner = match.Oppnents[1];
                        }

                        break;
                    }
                }
            }
        }

    }
}