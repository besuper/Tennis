using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tennis.Objects;

namespace Tennis.DAO
{
    public class PlayerDAO : DAO<Player>
    {
        public override bool Create(Player obj)
        {
            throw new NotImplementedException();
        }

        public override bool Delete(Player obj)
        {
            throw new NotImplementedException();
        }

        public override Player Find(int id)
        {
            throw new NotImplementedException();
        }

        public override bool Update(Player obj)
        {
            throw new NotImplementedException();
        }

        public List<Player> FindAll()
        {
            List<Player> list = new List<Player>();

            using (SqlCommand command = new SqlCommand("SELECT * FROM Players", DatabaseManager.GetConnection()))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //list.Add(new Player(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetInt32(4), reader.GetInt32(5)));
                        list.Add(new Player(
                            (int)reader["id_player"],
                            (string)reader["firstname"],
                            (string)reader["lastname"],
                            (string)reader["nationality"],
                            (int)reader["player_rank"],
                            (int)reader["gender"]
                        ));
                    }
                }
            }

            return list;
        }
    }
}
