using Tennis.DAO;
using Tennis.Factory;

namespace Tennis.Objects
{

    public class Referee : Person
    {
        // Uniquement utile en bdd?
        //private List<Tournament> tournaments;
        private readonly int id;
        private Match? match;

        public Match? Match { get { return match; } }
        public int Id { get { return id; } }

        public Referee(int id, string firstname, string lastname, string nationality) : base(firstname, lastname, nationality)
        {
            this.id = id;
        }

        public bool IsAvailable(Match checkMatch)
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

        public static Referee GetById(int id)
        {
            DAO<Referee> refereeDAO = AbstractDAOFactory.GetFactory().GetRefereeDAO();

            return refereeDAO.Find(id);
        }   

    }
}