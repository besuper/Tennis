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
        public abstract DAO<Opponent> GetOpponentDAO();
        public abstract DAO<Set> GetSetDAO();
        public abstract DAO<Game> GetGameDAO();

        public static AbstractDAOFactory Factory = GetFactory();

        public static AbstractDAOFactory GetFactory(DAOFactoryType type = DAOFactoryType.MS_SQL_FACTORY)
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
