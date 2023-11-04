namespace Tennis.Objects
{
    public class Player : Person
    {
        private int rank;

        private GenderType gender;

        public Player(string firstname, string lastname, string nationality, int rank, string gender) : base(firstname, lastname, nationality)
        {
            this.rank = rank;
            this.gender = gender == "H" ? GenderType.Man : GenderType.Woman;
        }
    }
}