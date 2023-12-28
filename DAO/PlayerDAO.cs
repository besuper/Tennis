using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Documents;
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
                        list.Add(new Player(
                            reader.GetInt32("id_player"),
                            reader.GetString("firstname"),
                            reader.GetString("lastname"),
                            reader.GetString("nationality"),
                            reader.GetInt32("player_rank"),
                            reader.GetInt32("gender")
                        ));
                    }
                }
            }

            return list;
        }

        public List<Player> GetPlayersFromOpponent(Opponent opponent)
        {
            List<Player> players = new List<Player>();

            using (SqlCommand command = new SqlCommand("SELECT * FROM Players INNER JOIN PlayersOpponent ON PlayersOpponent.id_player = Players.id_player WHERE id_opponent = @id", DatabaseManager.GetConnection()))
            {
                command.Parameters.AddWithValue("@id", opponent.Id);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        players.Add(new Player(
                            reader.GetInt32("id_player"),
                            reader.GetString("firstname"),
                            reader.GetString("lastname"),
                            reader.GetString("nationality"),
                            reader.GetInt32("player_rank"),
                            reader.GetInt32("gender")
                        ));
                    }
                }
            }

            return players;
        }
    }
}
