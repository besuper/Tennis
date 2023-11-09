
using System.Collections.Generic;
using System.Linq;
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
            return string.Join(", ", players);
        }
    }
}