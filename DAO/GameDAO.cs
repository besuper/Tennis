using System;
using System.Data.SqlClient;
using Tennis.Objects;

namespace Tennis.DAO
{
    public class GameDAO : DAO<Game>
    {
        public override bool Create(Game obj)
        {
            using (SqlCommand cmd = new SqlCommand("INSERT INTO Games(id_opponent, id_set, score_a, score_b) output INSERTED.id_game VALUES(@id_opponent, @id_set, @score_a, @score_b)", DatabaseManager.GetConnection()))
            {

                cmd.Parameters.AddWithValue("@id_opponent", obj.Winner!.Id);
                cmd.Parameters.AddWithValue("@id_set", obj.Set.Id);
                cmd.Parameters.AddWithValue("@score_a", obj.CurrentScoreOp1);
                cmd.Parameters.AddWithValue("@score_b", obj.CurrentScoreOp2);


                int modified = (int)cmd.ExecuteScalar();

                obj.Id = modified;

                return true;
            }
        }

        public bool CreateWithoutWinner(Set obj)
        {
            using (SqlCommand cmd = new SqlCommand("INSERT INTO Sets(id_match) output INSERTED.id_set VALUES(@id_match)", DatabaseManager.GetConnection()))
            {

                cmd.Parameters.AddWithValue("@id_match", obj.Match.Id);

                int modified = (int)cmd.ExecuteScalar();

                obj.Id = modified;

                return true;
            }
        }

        public override bool Delete(Game obj)
        {
            throw new NotImplementedException();
        }

        public override Game Find(int id)
        {
            throw new NotImplementedException();
        }

        public override bool Update(Game obj)
        {
            throw new NotImplementedException();
        }
    }
}
