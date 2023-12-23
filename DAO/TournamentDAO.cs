using System;
using System.Data.SqlClient;
using Tennis.Objects;

namespace Tennis.DAO
{
    public class TournamentDAO : DAO<Tournament>
    {
        public override bool Create(Tournament obj)
        {
            using (SqlCommand cmd = new SqlCommand("INSERT INTO Tournaments(name) output INSERTED.ID_TOURNAMENT VALUES(@name)", DatabaseManager.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@name", obj.Name);

                int modified = (int)cmd.ExecuteScalar();

                obj.Id = modified;

                return true;
            }
        }

        public override bool Delete(Tournament obj)
        {
            throw new NotImplementedException();
        }

        public override Tournament Find(int id)
        {
            throw new NotImplementedException();
        }

        public override bool Update(Tournament obj)
        {
            throw new NotImplementedException();
        }
    }
}
