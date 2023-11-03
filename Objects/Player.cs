
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

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