﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Tennis.Objects;

namespace Tennis.DAO
{
    public class MatchDAO : DAO<Match>
    {
        public override bool Create(Match obj)
        {
            using (SqlCommand cmd = new SqlCommand("INSERT INTO Matches(match_date, duration, round, id_referee, id_court, id_schedule) output INSERTED.ID_MATCH VALUES(@match_date, @duration, @round, @id_referee, @id_court, @id_schedule)", DatabaseManager.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@match_date", obj.Date);
                cmd.Parameters.AddWithValue("@duration", obj.Duration);
                cmd.Parameters.AddWithValue("@round", obj.Round);
                cmd.Parameters.AddWithValue("@id_referee", obj.Referee!.Id);
                cmd.Parameters.AddWithValue("@id_court", obj.Court!.Id);
                cmd.Parameters.AddWithValue("@id_schedule", obj.Schedule.Id);

                int modified = (int)cmd.ExecuteScalar();

                obj.Id = modified;

                return true;
            }
        }

        public bool AddOpponent(Match match, Opponent opponent)
        {
            using (SqlCommand cmd = new SqlCommand("INSERT INTO MatchOpponent(id_match, id_opponent) VALUES(@id_match, @id_opponent)", DatabaseManager.GetConnection()))
            {
                Debugger.log(match.Id + " " + opponent.Id);
                cmd.Parameters.AddWithValue("@id_match", match.Id);
                cmd.Parameters.AddWithValue("@id_opponent", opponent.Id);

                cmd.ExecuteNonQuery();
                return true;
            }
        }

        public override bool Delete(Match obj)
        {
            throw new NotImplementedException();
        }

        public override Match Find(int id)
        {
            throw new NotImplementedException();
        }

        public override bool Update(Match obj)
        {
            throw new NotImplementedException();
        }

        internal List<Match> GetAllMatchesFromSchedule(Schedule schedule)
        {
            List<Match> matches = new List<Match>();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Matches WHERE id_schedule = @id", DatabaseManager.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@id", schedule.Id);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Match match= new Match(
                        reader.GetInt32("id_match"), 
                        reader.GetDateTime("match_date"), 
                        reader.GetTimeSpan(2), // duration, ne fonctionne pas en str
                        reader.GetInt32("round"), 
                        reader.GetInt32("id_referee"), 
                        reader.GetInt32("id_court"),
                        schedule
                        );
                    matches.Add(match);
                }
            }

            return matches;
        }
    }
}
