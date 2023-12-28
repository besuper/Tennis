using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Tennis.Objects;

namespace Tennis.DAO
{
    public class CourtDAO : DAO<Court>
    {
        public override bool Create(Court obj)
        {
            throw new NotImplementedException();
        }

        public override bool Delete(Court obj)
        {
            throw new NotImplementedException();
        }

        public override Court? Find(int id)
        {
            Court? referee = null;

            using (SqlCommand command = new SqlCommand("SELECT * FROM Courts WHERE id_court = @id", DatabaseManager.GetConnection()))
            {
                command.Parameters.AddWithValue("@id", id);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        referee = new Court(id,
                            reader.GetString("nom"),
                            reader.GetInt32("nb_spectator"),
                            reader.GetBoolean("covered")
                        );
                    }
                }
            }

            return referee;
        }

        public override bool Update(Court obj)
        {
            throw new NotImplementedException();
        }

        public List<Court> FindAll()
        {
            List<Court> list = new List<Court>();

            using (SqlCommand command = new SqlCommand("SELECT * FROM Courts", DatabaseManager.GetConnection()))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Court(
                            reader.GetInt32("id_court"),
                            reader.GetString("nom"),
                            reader.GetInt32("nb_spectator"),
                            reader.GetBoolean("covered")
                        ));
                    }
                }
            }

            return list;
        }
    }
}
