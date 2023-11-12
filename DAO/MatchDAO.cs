using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tennis.Objects;

namespace Tennis.DAO
{
    public class MatchDAO : DAO<Match>
    {
        public override bool Create(Match obj)
        {
            using (SqlCommand cmd = new SqlCommand("INSERT INTO Matches(match_date, duration, round, id_referee, id_court, id_schedule) output INSERTED.ID_SCHEDULE VALUES(@match_date, @duration, @round, @id_referee, @id_court, @id_schedule)", DatabaseManager.GetConnection()))
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
    }
}
