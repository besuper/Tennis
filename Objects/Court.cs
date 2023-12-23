namespace Tennis.Objects
{
    public class Court
    {
        private readonly int id;
        private readonly string name;
        private readonly int nbSpectators;
        private readonly bool covered;

        private Match? match;

        public Match? Match { get { return match; } }
        public int Id { get { return id; } }

        public Court(int id, string name, int nbSpectators, bool covered)
        {
            this.id = id;
            this.name = name;
            this.nbSpectators = nbSpectators;
            this.covered = covered;
        }

        public bool IsAvailable(Match checkMatch)
        {
            bool isAvailable = this.match == null;

            if (isAvailable)
            {
                this.match = checkMatch;
            }

            return isAvailable;
        }

        public void Release()
        {
            this.match = null;
        }

        public override string? ToString()
        {
            return this.name;
        }
    }
}