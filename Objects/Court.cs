using System.Collections.Generic;
using Tennis.DAO;
using Tennis.Factory;

namespace Tennis.Objects
{
    public class Court
    {
        /// <summary>
        /// Attributes
        /// </summary>

        private readonly int id;
        private readonly string name;
        private readonly int nbSpectators;
        private readonly bool covered;

        private Match? match;

        /// <summary>
        /// Constructor
        /// </summary>
        public Court(int id, string name, int nbSpectators, bool covered)
        {
            this.id = id;
            this.name = name;
            this.nbSpectators = nbSpectators;
            this.covered = covered;
        }

        /// <summary>
        /// Getters and Setters
        /// </summary>

        public Match? Match { get { return match; } set { this.match = value; } }
        public int Id { get { return id; } }
        public string Information { get { return $"{nbSpectators} spectateurs et " + (covered ? "est" : "n'est pas") + " couvert"; } }

        /// <summary>
        /// Methods
        /// </summary>

        public bool IsAvailable(Match match)
        {
            if (this.match != null)
            {
                return this.match.Date != match.Date;
            }

            return this.match == null;
        }

        public void Release()
        {
            this.match = null;
        }

        public override string? ToString()
        {
            return this.name;
        }

        public override bool Equals(object? obj)
        {
            return obj is Court court &&
                   name == court.name;
        }

        /// <summary>
        /// DAO Methods
        /// </summary>

        public static Court GetById(int idCourt)
        {
            DAO<Court> courtDAO = AbstractDAOFactory.GetFactory().GetCourtDAO();

            return courtDAO.Find(idCourt);
        }

        public static List<Court> GetAll()
        {
            CourtDAO courtDAO = (CourtDAO)AbstractDAOFactory.GetFactory().GetCourtDAO();

            return courtDAO.FindAll();
        }
    }
}