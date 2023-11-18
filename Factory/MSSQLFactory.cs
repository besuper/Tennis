using Tennis.DAO;
using Tennis.Objects;

namespace Tennis.Factory
{
    public class MSSQLFactory : AbstractDAOFactory
    {
        public override DAO<Player> GetPlayerDAO()
        {
            return new PlayerDAO();
        }

        public override DAO<Schedule> GetScheduleDAO()
        {
            return new ScheduleDAO();
        }

        public override DAO<Tournament> GetTournamentDAO()
        {
            return new TournamentDAO();
        }

        public override DAO<Match> GetMatchDAO()
        {
            return new MatchDAO();
        }

        public override DAO<Referee> GetRefereeDAO()
        {
            return new RefereeDAO();
        }

        public override DAO<Court> GetCourtDAO()
        {
            return new CourtDAO();
        }

        public override DAO<Opponent> GetOpponentDAO()
        {
            return new OpponentDAO();
        }

        public override DAO<Set> GetSetDAO()
        {
            return new SetDAO();
        }

        public override DAO<Game> GetGameDAO()
        {
            return new GameDAO();
        }
    }
}
