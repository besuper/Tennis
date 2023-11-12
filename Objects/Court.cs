namespace Tennis.Objects
{
    public class Court
    {

        private int id;
        private string name;
        private int nbSpectators;
        private bool covered;

        private Match? match;

        public Court(int id, string name, int nbSpectators, bool covered)
        {
            this.id = id;
            this.name = name;
            this.nbSpectators = nbSpectators;
            this.covered = covered;
        }

        public Match? Match { get { return match; } }
        public int Id { get { return id; } }

        public bool Available(Match checkMatch)
        {
            bool isAvailable = match == null;

            if (isAvailable)
            {
                match = checkMatch;
            }

            return isAvailable;
        }

        public void Release()
        {
            match = null;
        }

        public override string? ToString()
        {
            return name;
        }
    }
}