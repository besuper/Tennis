
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tennis.DAO;
using Tennis.Factory;

namespace Tennis.Objects
{
    public class Opponent
    {
        private int id;
        private List<Player> players = new List<Player>();

        public List<Player> Players { get { return players; } }

        public int Id { get { return id; } set { this.id = value; } }
        // public object Date { get; internal set; } ????

        public Opponent()
        { }

        public Opponent(int id)
        { 
            this.id = id;

            //Load les players
            this.players = Player.GetPlayersFromOpponent(this);
            Console.WriteLine("Opponent : " + this.ToString());
        }

        public void AddPlayer(Player player)
        {
            players.Add(player);
        }

        public override string? ToString()
        {
            return string.Join(", ", players);
        }

        public static List<Opponent> GetOpponnentFromMatch(Match match)
        {
            OpponentDAO opponentDAO = (OpponentDAO) AbstractDAOFactory.GetFactory().GetOpponentDAO();

            return opponentDAO.GetOpponnentFromMatch(match);
        }
    }
}