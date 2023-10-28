
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Referee : Person {

    private List<Tournament> tournaments;
    private List<Match> matches;

    public Referee(string firstname, string lastname, string nationality) : base(firstname, lastname, nationality)
    {
    }

    public void Available() {
        // TODO implement here
    }

    public void Release() {
        // TODO implement here
    }

}