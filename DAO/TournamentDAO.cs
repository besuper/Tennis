using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
            using (SqlCommand cmd = new SqlCommand("DELETE FROM Tournaments WHERE id_tournament = @id", DatabaseManager.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@id", obj.Id);

                int modified = cmd.ExecuteNonQuery();

                return modified == 1;
            }
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

            using (SqlCommand cmd = new SqlCommand("SELECT t.id_tournament, t.name FROM Tournaments t JOIN Schedules s ON s.id_tournament = t.id_tournament JOIN Matches m ON m.id_schedule = s.id_schedule WHERE s.schedule_type = 0 GROUP BY t.id_tournament, t.name HAVING COUNT(m.id_match) = 127", DatabaseManager.GetConnection()))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Tournament tournament = new Tournament(
                            reader.GetInt32("id_tournament"),
                            reader.GetString("name")
                        );

                        tournaments.Add(tournament);
                    }
                }
            }

            return tournaments;
        }
    }
}
