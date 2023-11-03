
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Referee : Person
{

    // Uniquement utile en bdd?
    //private List<Tournament> tournaments;
    private Match match;

    public Match Match { get { return match; } }

    public Referee(string firstname, string lastname, string nationality) : base(firstname, lastname, nationality)
    {
    }

    // FIXME: Probl�matique puisqu'on cr�er tout les matchs d'un coup (avant de les jouer) et donc les arbitres n'ont pas le temps de se lib�rer
    // pas possible de check nn plus avec un date
    // peut etre faire l'ajout et le check avant le play du match au lieu a sa cr�ation
    public bool Available(Match checkMatch)
    {
        bool isAvailable = match == null;

        if (isAvailable)
        {
            match = checkMatch;
        }

        return isAvailable;
    }

    public void Release()
    {
        match = null;
    }

}