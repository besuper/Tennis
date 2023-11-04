
using System.Collections.Generic;
using System.Text;

namespace Tennis.Objects
{
    public class Opponent
    {
        private List<Match> matches = new List<Match>();
        private List<Player> players = new List<Player>();
        private List<Set> setWinList = new List<Set>();

        private List<Player> Players { get { return players; } set { players = value; } }

        public Opponent()
        { }

        public void AddPlayer(Player player)
        {
            players.Add(player);
        }

        public override string? ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append("[");
            foreach (Player player in players)
            {
                str.Append(player.Lastname).Append(" ").Append(player.Firstname).Append(", ");
            }
            str.Append("]");

            return str.ToString();
        }
    }
}