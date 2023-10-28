
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Opponent {
    private List<Match> matches;
    private List<Player> players;
    private List<Set> setWinList;

    public Opponent(List<Player> players)
    {
        this.players = players;
    }

}