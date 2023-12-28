using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Tennis.Objects;

namespace Tennis.DAO
{
    public class RefereeDAO : DAO<Referee>
    {
        public override bool Create(Referee obj)
        {
            throw new NotImplementedException();
        }

        public override bool Delete(Referee obj)
        {
            throw new NotImplementedException();
        }

        public override Referee? Find(int id)
        {
            Referee referee = null;

            using (SqlCommand command = new SqlCommand("SELECT * FROM Referees WHERE id_referee = @id", DatabaseManager.GetConnection()))
            {
                command.Parameters.AddWithValue("@id", id);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        referee = new Referee(
                            id,
                            reader.GetString("firstname"),
                            reader.GetString("lastname"),
                            reader.GetString("nationality")
                        );
                    }
                }
            }

            return referee;
        }

        public override bool Update(Referee obj)
        {
            throw new NotImplementedException();
        }

        public List<Referee> FindAll()
        {
            List<Referee> list = new List<Referee>();

            using (SqlCommand command = new SqlCommand("SELECT * FROM Referees", DatabaseManager.GetConnection()))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Referee(
                            reader.GetInt32("id_referee"),
                            reader.GetString("firstname"),
                            reader.GetString("lastname"),
                            reader.GetString("nationality")
                        ));
                    }
                }
            }

            return list;
        }
    }
}
