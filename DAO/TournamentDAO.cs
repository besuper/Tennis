using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Documents;
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

        public List<Tournament> FindAll()
        {
            List<Tournament> tournaments = new List<Tournament>();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Tournaments", DatabaseManager.GetConnection()))
            {
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Tournament tournament = new Tournament(reader.GetInt32(0), reader.GetString(1));
                    tournaments.Add(tournament);
                }
            }

            return tournaments;
        }
    }
}
