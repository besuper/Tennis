using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DemoClasses.Objects
{
    internal class Game
    {
        private int id;
        //public int gamePointA { get; set; } // devient inutile => winner
        //public int gamePointB { get; set; } // devient inutile => winner

        public int currentGamePointA { get; set; } // ici utile pour enregistrer les parties 15-30 dans la bdd (optionel)
        public int currentGamePointB { get; set; }

        private GameSet gameSet;
        private Random rand = new Random();

        private int winner = -1;

        public int Winner { get { return winner; } }

        public Game(GameSet gameSet)
        {
            this.gameSet = gameSet;
        }

        public void StartGame()
        {

            Console.WriteLine($"\n===========[Nouveau jeu {gameSet.GameScorePlayerA()} - {gameSet.GameScorePlayerB()}]===========");

            // Jouer une partie (un jeu)
            while (winner == -1)
            {
                SimulateAGame();
                //await Task.Delay(1000);
                //Thread.Sleep(1000);
            }

            // Afficher le gagnant
            if (winner == 0)
            {
                Console.WriteLine("Joueur A a remporté le jeu");
            }
            else
            {
                Console.WriteLine("Joueur B a remporté le jeu");
            }
        }

        // Logique du dérouelement d'un jeu (points)
        public void SimulateAGame()
        {
            int gagnantTour = Dice();

            // Egalité
            if (currentGamePointA == 40 && currentGamePointB == 40)
            {
                if (gagnantTour == 0)
                {
                    currentGamePointA++;
                    Console.WriteLine("JoueurA prend l'avantage");
                }
                else
                {
                    currentGamePointB++;
                    Console.WriteLine("JoueurB prend l'avantage");
                }
            }

            // Ajout des points pour chaque gagnant
            // 1er point => 15
            // 2eme => 15
            // 3eme => 10
            if (gagnantTour == 0)
            {
                // Le joueur A était en avantage, il a marqué le second point, il gagne
                // Ou alors il a gagné avec 40 points (la vérification de l'égalité ce fais plus haut!)
                if (currentGamePointA == 41 || currentGamePointA == 40)
                {

                    Console.WriteLine("Joueur A remporte la partie");
                    winner = 0;

                    return;
                }

                // Le joueur B était en avantage, mais il la perdu
                if (currentGamePointB == 41)
                {
                    currentGamePointB--;
                    Console.WriteLine("Joueur B perd sont avantage, retour a l'égalité!");
                    return;
                }

                currentGamePointA += currentGamePointA == 30 ? 10 : 15;
                Console.WriteLine("Points Joueur A " + currentGamePointA);
            }
            else
            {
                // Le joueur B était en avantage, il a marqué le second point, il gagne
                // Ou alors il a gagné avec 40 points (la vérification de l'égalité ce fais plus haut!)
                if (currentGamePointB == 41 || currentGamePointB == 40)
                {
                    Console.WriteLine("Joueur B remporte la partie");
                    winner = 1;

                    return;
                }

                // Le joueur A était en avantage, mais il la perdu
                if (currentGamePointA == 41)
                {
                    currentGamePointA--;
                    Console.WriteLine("Joueur A perd sont avantage, retour a l'égalité!");
                    return;
                }

                currentGamePointB += currentGamePointB == 30 ? 10 : 15;
                Console.WriteLine("Points Joueur B " + currentGamePointB);
            }
        }

        // Retourne 0 ou 1
        // Permet de définir un gagant pour une balle dans une partie
        int Dice()
        {
            return rand.Next(0, 2);
        }
    }
}
