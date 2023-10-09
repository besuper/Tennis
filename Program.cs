// See https://aka.ms/new-console-template for more information
using DemoClasses.Objects;

Console.WriteLine("Hello, World!");

Match match = new Match(5);

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