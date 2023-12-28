
using System.Collections.Generic;
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
        /// Constructor from database loading
        /// </summary>
        /// <param name="id"></param>
        /// <param name="match"></param>
        public Set(int id, Match match)
        {
            this.id = id;
            this.match = match;

            //Load each game
            games = Game.GetAllGamesFromSet(this);

            if (GameScorePlayerA() > GameScorePlayerB())
            {
                winner = match.Opponents[0];
            }
            else
            {
                winner = match.Opponents[1];
            }

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
        public int ScoreOp1 { get { return scoreOp1; } set { } }
        public int ScoreOp2 { get { return scoreOp2; } set { } }

        /// <summary>
        /// Methods
        /// </summary>
        public int GameScorePlayerA()
        {
            int GameScoreA = 0;

            foreach (Game game in games)
            {
                if (game.Winner != null && game.Winner == match.Opponents[0])
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
                if (game.Winner != null && game.Winner == match.Opponents[1])
                {
                    GameScoreB++;
                }
            }

            return GameScoreB;
        }

        public void Play()
        {
            Game temp;

            for (int actualGameState = 1; actualGameState <= 12; actualGameState++)
            {
                temp = new Game(this);

                games.Add(temp);

                temp.Play();

                //Game.CreateGame(temp);

                int GamePointA = GameScorePlayerA();
                int GamePointB = GameScorePlayerB();

                if (GamePointA >= 6 && (GamePointA - GamePointB) >= 2)
                {
                    // Le joueur A a forcement gagné le SET ici
                    winner = match.Opponents[0];
                    break;
                }

                if (GamePointB >= 6 && (GamePointB - GamePointA) >= 2)
                {
                    // Le joueur B a forcement gagné le SET ici
                    winner = Match.Opponents[1];
                    break;
                }

                if (GamePointA == 6 && GamePointB == 6)
                {
                    // Les 12 jeux sont passé MAIS les joueurs sont a égalité, comment donner le point du set?
                    // Faire jouer un tie-break
                    // Si le set est le dernier du match (3 ou 5 sets) alors c'est un super tie-break
                    // Get the last set and replace it by a SuperTieBreak

                    if (match.IsWinningSet())
                    {
                        // super tie-break
                        SuperTieBreak stb = new SuperTieBreak(match);
                        stb.Play(10);

                        Game tempgame = new Game(stb);
                        this.games.Add(tempgame);

                        winner = stb.Winner;
                        break;
                    }
                    else
                    {
                        // tie-break

                        // FIXME: !
                        SuperTieBreak stb = new SuperTieBreak(match);
                        stb.Play(7);

                        Game tempgame = new Game(stb);
                        this.games.Add(tempgame);

                        winner = stb.Winner;

                        break;
                    }
                }
            }
            Game.CreateGames(games);
        }

        /// <summary>
        /// DAO Methods
        /// </summary>

        public static List<Set> GetAllSetsFromMatch(Match match)
        {
            SetDAO setDAO = (SetDAO)AbstractDAOFactory.Factory.GetSetDAO();
            return setDAO.GetSetsFromMatch(match);
        }
    }
}