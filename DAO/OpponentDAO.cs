using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tennis.Objects;

namespace Tennis.DAO
{
    public class OpponentDAO : DAO<Opponent>
    {
        public override bool Create(Opponent obj)
        {
            using (SqlCommand cmd = new SqlCommand("INSERT INTO Opponents output INSERTED.ID_OPPONENT DEFAULT VALUES", DatabaseManager.GetConnection()))
            {

                int modified = (int)cmd.ExecuteScalar();

                obj.Id = modified;

                return true;
            }
        }

        public bool AddPlayer(Opponent opponent)
        {
            String query = "INSERT INTO PlayersOpponent(id_opponent, id_player) VALUES(@id_opponent, @id_player1)";
            if (opponent.Players.Count > 1)
            {
                query = "INSERT INTO PlayersOpponent(id_opponent, id_player) VALUES(@id_opponent, @id_player1), (@id_opponent, @id_player2)";
            }

            using (SqlCommand cmd = new SqlCommand(query, DatabaseManager.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@id_opponent", opponent.Id);
                cmd.Parameters.AddWithValue("@id_player1", opponent.Players[0].Id);
                if (opponent.Players.Count > 1)
                {
                    cmd.Parameters.AddWithValue("@id_player2", opponent.Players[1].Id);
                }

                cmd.ExecuteNonQuery();

                return true;
            }
        }

        public override bool Delete(Opponent obj)
        {
            throw new NotImplementedException();
        }

        public override Opponent Find(int id)
        {
            throw new NotImplementedException();
        }

        public override bool Update(Opponent obj)
        {
            throw new NotImplementedException();
        }

        internal List<Opponent> GetOpponnentFromMatch(Match match)
        {
            List<Opponent> opponents = new List<Opponent>();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM MatchOpponent INNER JOIN Opponents ON Opponents.id_opponent = MatchOpponent.id_opponent WHERE id_match = @id", DatabaseManager.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@id", match.Id);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Opponent opponent = new Opponent(reader.GetInt32(1));
                    opponents.Add(opponent);
                }
            }

            return opponents;
        }
    }
}
