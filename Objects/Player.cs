using System;

namespace Tennis.Objects
{
    public class Player : Person
    {
        private readonly int id;
        private readonly int rank;
        private readonly GenderType gender;

        public Player(int id, string firstname, string lastname, string nationality, int rank, int gender) : base(firstname, lastname, nationality)
        {
            this.id = id;
            this.rank = rank;

            Array genders = Enum.GetValues(typeof(ScheduleType));
            object? _gender = genders.GetValue(gender);

            if (_gender == null)
            {
                throw new Exception("Can't find gender");
            }

            this.gender = (GenderType)_gender;
        }

        public int Id { get { return this.id; } }
    }
}