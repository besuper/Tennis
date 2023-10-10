using DemoClasses.Enum;
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
                str.Append(player.GetName()).Append(" ").Append(player.GetFirstName());
            }
            str.Append("]");

            return str.ToString();
        }

        public override string? ToString()
        {
            return $"{id}; {players[0]};";
        }
    }
}
