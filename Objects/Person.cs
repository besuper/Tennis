namespace Tennis.Objects
{

    public class Person
    {

        private readonly string firstname;
        private readonly string lastname;
        private readonly string nationality;

        public string Firstname { get { return firstname; } }
        public string Lastname { get { return lastname; } }

        public Person(string firstname, string lastname, string nationality)
        {
            this.firstname = firstname;
            this.lastname = lastname;
            this.nationality = nationality;
        }

        public override string? ToString()
        {
            return $"{firstname} {lastname}";
        }
    }
}