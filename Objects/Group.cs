using DemoClasses.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoClasses.Objects
{
    internal class Group
    {
        private int id;
        private List<Player> players = new List<Player>();
        private List<Match> matches;

        public List<Player> Players {  get { return players; } }


        public Group()
        {
        }

        public void AddPlayer(Player player)
        {
            players.Add(player);
        }

        public string GetGroupName()
        {
            StringBuilder str = new StringBuilder();
            str.Append("[");
            foreach (Player player in players)
            {
                str.Append(player.GetName()).Append(" ").Append(player.GetFirstName()).Append(", ");
            }
            str.Append("]");

            return str.ToString();
        }

        public override string? ToString()
        {

            StringBuilder str = new StringBuilder();
            str.Append("[");
            foreach (Player player in players)
            {
                str.Append(player.GetName()).Append(" ").Append(player.GetFirstName()).Append(", ");
            }
            str.Append("]");

            return str.ToString();

        }
    }
}
