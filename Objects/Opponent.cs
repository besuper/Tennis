
using System.Collections.Generic;
using System.Linq;
using System.Text;

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