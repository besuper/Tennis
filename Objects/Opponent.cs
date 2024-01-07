
using System.Collections.Generic;
using Tennis.DAO;
using Tennis.Factory;

namespace Tennis.Objects
{
    public class Opponent
    {
        private int id;
        private List<Player> players = new List<Player>();
        
        public Opponent()
        { }

        public Opponent(int id)
        {
            this.id = id;

            //Load les players
            this.players = Player.GetPlayersFromOpponent(this);
        }

        public List<Player> Players { get { return players; } }
        public int Id { get { return id; } set { this.id = value; } }

        public void AddPlayer(Player player)
        {
            players.Add(player);
        }

        public override string? ToString()
        {
            return string.Join(", ", players);
        }

        /// <summary>
        /// DAO Methods
        /// </summary>

        public static void CreateOpponent(Opponent opponent)
        {
            OpponentDAO opponentDAO = (OpponentDAO)AbstractDAOFactory.Factory.GetOpponentDAO();

            opponentDAO.Create(opponent);
            opponentDAO.AddPlayer(opponent);
        }

        public static List<Opponent> GetOpponnentFromMatch(Match match)
        {
            OpponentDAO opponentDAO = (OpponentDAO)AbstractDAOFactory.GetFactory().GetOpponentDAO();

            return opponentDAO.GetOpponnentFromMatch(match);
        }
    }
}