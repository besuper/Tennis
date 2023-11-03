public class Court
{

    private int nbSpectators;
    private bool covered;

    private Tournament tournament;
    private Match? match;

    public Court(int nbSpectators, bool covered)
    {
        this.nbSpectators = nbSpectators;
        this.covered = covered;
    }

    public bool Available()
    {
        return match == null;
    }

    public void Release()
    {
        match = null;
    }

}