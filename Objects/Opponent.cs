
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

public class Opponent {
    private List<Match> matches;
    private List<Player> players;
    private List<Set> setWinList;

    private List<Player> Players { get { return players; } set { players = value; } }

    public Opponent()
    {
    }

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