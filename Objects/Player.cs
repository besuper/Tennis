using System;
using System.Diagnostics;

namespace Tennis.Objects
{
    public class Player : Person
    {
        private int id;
        private int rank;
        private GenderType gender;

        public Player(int id, string firstname, string lastname, string nationality, int rank, int gender) : base(firstname, lastname, nationality)
        {
            this.id = id;
            this.rank = rank;

            Array genders = Enum.GetValues(typeof(ScheduleType));

            this.gender = (GenderType)genders.GetValue(gender);
        }

        public int Id { get { return this.id; } }
    }
}