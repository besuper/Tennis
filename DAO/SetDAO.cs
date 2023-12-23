using System;
using System.Data.SqlClient;
using Tennis.Objects;

namespace Tennis.DAO
{
    public class SetDAO : DAO<Set>
    {
        public override bool Create(Set obj)
        {
            using (SqlCommand cmd = new SqlCommand("INSERT INTO Sets(id_match) output INSERTED.id_set VALUES(@id_match)", DatabaseManager.GetConnection()))
            {

                cmd.Parameters.AddWithValue("@id_match", obj.Match.Id);

                int modified = (int)cmd.ExecuteScalar();

                obj.Id = modified;

                return true;
            }
        }

        public override bool Delete(Set obj)
        {
            throw new NotImplementedException();
        }

        public override Set Find(int id)
        {
            throw new NotImplementedException();
        }

        public override bool Update(Set obj)
        {
            using (SqlCommand cmd = new SqlCommand("UPDATE Sets SET id_opponent = @id_opponent WHERE id_set = @id_set", DatabaseManager.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@id_set", obj.Id);
                cmd.Parameters.AddWithValue("@id_opponent", obj.Winner!.Id);

                cmd.ExecuteNonQuery();

                return true;
            }
        }
    }
}
