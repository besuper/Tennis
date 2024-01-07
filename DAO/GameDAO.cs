using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Tennis.Objects;

namespace Tennis.DAO
{
    public class GameDAO : DAO<Game>
    {
        public override bool Create(Game obj)
        {
            using (SqlCommand cmd = new SqlCommand("INSERT INTO Games(id_opponent, id_set, score_a, score_b, isTieBreak) output INSERTED.id_game VALUES(@id_opponent, @id_set, @score_a, @score_b, @isTieBreak)", DatabaseManager.GetConnection()))
            {

                cmd.Parameters.AddWithValue("@id_opponent", obj.Winner!.Id);
                cmd.Parameters.AddWithValue("@id_set", obj.Set.Id);
                cmd.Parameters.AddWithValue("@score_a", obj.CurrentScoreOp1);
                cmd.Parameters.AddWithValue("@score_b", obj.CurrentScoreOp2);
                cmd.Parameters.AddWithValue("@isTieBreak", obj is TieBreak);

                int modified = (int)cmd.ExecuteScalar();

                obj.Id = modified;

                return true;
            }
        }

        public bool CreateGames(List<Game> objs)
        {
            StringBuilder sb = new StringBuilder("INSERT INTO Games(id_opponent, id_set, score_a, score_b, isTieBreak) VALUES");

            foreach (Game game in objs)
            {
                int position = objs.IndexOf(game);
                sb.Append($"(@id_opponent{position}, @id_set, @score_a{position}, @score_b{position}, @isTieBreak{position}),");
            }

            string query = sb.ToString().TrimEnd(',');

            using (SqlCommand cmd = new SqlCommand(query, DatabaseManager.GetConnection()))
            {
                foreach (Game game in objs)
                {
                    int position = objs.IndexOf(game);

                    cmd.Parameters.AddWithValue($"@id_opponent{position}", game.Winner!.Id);
                    cmd.Parameters.AddWithValue($"@score_a{position}", $"{game.CurrentScoreOp1}");
                    cmd.Parameters.AddWithValue($"@score_b{position}", $"{game.CurrentScoreOp2}");
                    cmd.Parameters.AddWithValue($"@isTieBreak{position}", game is TieBreak);
                }

                cmd.Parameters.AddWithValue($"@id_set", objs[0].Set.Id);

                int modified = cmd.ExecuteNonQuery();

                return modified != objs.Count;
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

        public List<Game> GetGamesFromSet(Set set)
        {
            List<Game> games = new List<Game>();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Games WHERE id_set = @id", DatabaseManager.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@id", set.Id);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (Convert.ToBoolean(reader.GetBoolean("isTieBreak")))
                        {
                            games.Add(new TieBreak(
                                reader.GetInt32("id_game"),
                                reader.GetString("score_a"),
                                reader.GetString("score_b"),
                                reader.GetInt32("id_opponent"), //Winner of the set
                                set
                            ));
                        }
                        else
                        {
                            games.Add(new Game(
                                reader.GetInt32("id_game"),
                                reader.GetString("score_a"),
                                reader.GetString("score_b"),
                                reader.GetInt32("id_opponent"), //Winner of the set
                                set
                            ));
                        }

                    }
                }
            }
            return games;

        }
    }
}
