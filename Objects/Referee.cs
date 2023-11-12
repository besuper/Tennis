namespace Tennis.Objects
{

    public class Referee : Person
    {

        // Uniquement utile en bdd?
        //private List<Tournament> tournaments;
        private int id;
        private Match match;

        public Match Match { get { return match; } }

        public Referee(int id, string firstname, string lastname, string nationality) : base(firstname, lastname, nationality)
        {
            this.id = id;
        }

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

    }
}