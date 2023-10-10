// See https://aka.ms/new-console-template for more information
using DemoClasses;
using DemoClasses.Objects;
using Microsoft.Data.SqlClient;
using System;

namespace DemoClasses
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Group> list = new List<Group>();

            DatabaseManager dm = new DatabaseManager();

            SqlDataReader request = dm.Get("SELECT * FROM Joueur");

            while (request.Read())
            {
                Group temp = new Group();
                temp.AddPlayer(new Player(request.GetInt32(0), request.GetString(1), request.GetString(2), request.GetString(3)));
                list.Add(temp);


            }
            list = list.GetRange(0, 128);


            list.ForEach(e => { Console.WriteLine(e); });


            Console.WriteLine("Hello, World!");

            Match match = new Match(5, list[14], list[44]);

            match.StartMatch();

            // Juste un petit résumé pour voir si tout est bon niveau objet
            Console.WriteLine("\n\n");

            Console.WriteLine("Résumé de cet incroyable match !");

            Console.WriteLine("Scores des jeux :");

            foreach (GameSet set in match.GameSets)
            {
                Console.WriteLine("Score du jeu " + set.GameScorePlayerA() + " - " + set.GameScorePlayerB());
            }

            Console.WriteLine("Final score des sets : ");

            Console.WriteLine(match.ScorePlayerA() + " - " + match.ScorePlayerB());
        }

        public void makeGroups(List<Player> list)
        {
            //couper la liste a la moitier pour n'avoir que les hommes, la deuxieme partie sera uniquement les femmes
            list = list.GetRange(0, 128);

            //Groupage par deux
            
        }
    }
}