
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Person {

    private string firstname;

    private string lastname;

    private string nationality;

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