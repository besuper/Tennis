using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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

        public List<Schedule> GetAllScheduleFromTournamen(Tournament tournament)
        {
            Array genders = Enum.GetValues(typeof(ScheduleType));

            List<Schedule> schedules = new List<Schedule>();

            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Schedules WHERE id_tournament = @id", DatabaseManager.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@id", tournament.Id);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        object? _scheduleType = genders.GetValue(reader.GetInt32("schedule_type"));

                        if (_scheduleType == null)
                        {
                            throw new Exception("Can't find schedule type");
                        }

                        Schedule schedule = new Schedule(reader.GetInt32("id_schedule"), (ScheduleType)_scheduleType, tournament);
                        schedules.Add(schedule);
                    }
                }
            }

            return schedules;
        }
    }
}
