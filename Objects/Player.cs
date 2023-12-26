using System;
using System.Collections.Generic;
using Tennis.DAO;
using Tennis.Factory;

namespace Tennis.Objects
{
    public class Player : Person
    {
        private readonly int id;
        private readonly int rank;
        private readonly GenderType gender;

        public Player(int id, string firstname, string lastname, string nationality, int rank, int gender) : base(firstname, lastname, nationality)
        {
            this.id = id;
            this.rank = rank;

            Array genders = Enum.GetValues(typeof(GenderType));
            object? _gender = genders.GetValue(gender);

            if (_gender == null)
            {
                throw new Exception("Can't find gender");
            }

            this.gender = (GenderType)_gender;
        }

        public int Id { get { return this.id; } }

        public static List<Player> GetAllPlayers()
        {
            PlayerDAO playerDAO = (PlayerDAO)AbstractDAOFactory.GetFactory().GetPlayerDAO();

            return playerDAO.FindAll();
        }

        public static List<Player> GetPlayersFromOpponent(Opponent opponent)
        {
            PlayerDAO playerDAO = (PlayerDAO)AbstractDAOFactory.GetFactory().GetPlayerDAO();

            return playerDAO.GetPlayersFromOpponent(opponent);
        }
    }
}