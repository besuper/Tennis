using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tennis.Objects;

namespace Tennis.DAO
{
    public class ScheduleDAO : DAO<Schedule>
    {
        public override bool Create(Schedule obj)
        {
            using (SqlCommand cmd = new SqlCommand("INSERT INTO Schedules(schedule_type, id_tournament) output INSERTED.ID_SCHEDULE VALUES(@schedule_type, @id_tournament)", DatabaseManager.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@schedule_type", (int)obj.Type);
                cmd.Parameters.AddWithValue("@id_tournament", (int)obj.Tournament.Id);

                int modified = (int)cmd.ExecuteScalar();

                obj.Id = modified;

                return true;
            }
        }

        public override bool Delete(Schedule obj)
        {
            throw new NotImplementedException();
        }

        public override Schedule Find(int id)
        {
            throw new NotImplementedException();
        }

        public override bool Update(Schedule obj)
        {
            throw new NotImplementedException();
        }
    }
}
