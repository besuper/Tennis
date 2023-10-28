
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Court {

    private int nbSpectators;

    private bool covered;

    private Tournament tournament;
    private Match? match;

    public Court(int nbSpectators, bool covered)
    {
        this.nbSpectators = nbSpectators;
        this.covered = covered;
    }

    public bool Available() {
        // TODO implement here
        return covered;
    }

    public void Release() {
        match = null;
    }

}