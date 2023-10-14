using DemoClasses.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoClasses.Objects
{
    internal class Player
    {
        private int id;
        private string name;
        private string firstname;
        private Gender gender;
        private List<Group> groups;

        public Gender Gender { get { return gender; } set { this.gender = value; } }


        public Player(int id, string name, string firstname, string gender)
        {
            this.id = id;
            this.name = name;
            this.firstname = firstname;
            this.gender = gender == "H" ? Gender.Man : Gender.Woman;
        }

        public override string? ToString()
        {
            return $"{id}; {name}; {firstname}; {gender}";
        }

        public string GetName()
        {
            return name;
        }

        public string GetFirstName()
        {
            return firstname;
        }
    }
}
