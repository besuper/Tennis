using System;
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

        public Match? Match { get { return match; } }
        public int Id { get { return id; } }

        /// <summary>
        /// Methods
        /// </summary>

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