using Tennis.DAO;
using Tennis.Objects;

namespace Tennis.Factory
{
    public enum DAOFactoryType
    {
        MS_SQL_FACTORY,
        XML_FACTORY
    }
    public abstract class AbstractDAOFactory
    {
        public abstract DAO<Player> GetPlayerDAO();
        public abstract DAO<Schedule> GetScheduleDAO();
        public abstract DAO<Tournament> GetTournamentDAO();
        public abstract DAO<Match> GetMatchDAO();
        public abstract DAO<Referee> GetRefereeDAO();
        public abstract DAO<Court> GetCourtDAO();
        public static AbstractDAOFactory GetFactory(DAOFactoryType type)
        {
            switch (type)
            {
                case DAOFactoryType.MS_SQL_FACTORY:
                    return new MSSQLFactory();
                case DAOFactoryType.XML_FACTORY:
                    return null;
                default:
                    return null;
            }
        }
    }
}
