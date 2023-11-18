using System;
using System.Threading;

namespace Tennis.Objects
{
    public class Game
    {
        private int id;
        private Random rand = new Random();
        private int currentScoreOp1;
        private int currentScoreOp2;

        private Opponent? winner;
        private Set set;

        public int Id { get { return id; } set { id = value; } }
        public Set Set { get { return set; } }

        public dynamic CurrentScoreOp1 { get { return currentScoreOp1 > 40 ? "AD" : this.currentScoreOp1; } set { } }
        public dynamic CurrentScoreOp2 { get { return currentScoreOp2 > 40 ? "AD" : this.currentScoreOp2; } set { } }


        public Opponent Winner { get { return winner; } }

        public Game(Set set)
        {
            this.set = set;
        }

        public void Play()
        {
            winner = null;

            Debugger.log($"\n===========[Nouveau jeu {set.GameScorePlayerA()} - {set.GameScorePlayerB()}]===========");

            // Jouer une partie (un jeu)
            while (winner == null)
            {
                set.Match.NotifyPropertyChanged("ActualSet");
                set.Match.NotifyPropertyChanged("SetsOpponentA");
                set.Match.NotifyPropertyChanged("SetsOpponentB");
                SimulateAGame();
                set.Match.NotifyPropertyChanged("ActualSet");
                set.Match.NotifyPropertyChanged("SetsOpponentA");
                set.Match.NotifyPropertyChanged("SetsOpponentB");
                //await Task.Delay(1000);
                Thread.Sleep(5);
            }

            // Afficher le gagnant
            if (winner == set.Match.Oppnents[0])
            {
                Debugger.log(set.Match.Oppnents[0] + " a remporté le jeu");

            }
            else
            {
                Debugger.log(set.Match.Oppnents[1] + " a remporté le jeu");

            }
        }

        // Logique du dérouelement d'un jeu (points)
        public void SimulateAGame()
        {
            int gagnantTour = Dice();

            // Egalité
            if (currentScoreOp1 == 40 && currentScoreOp2 == 40)
            {
                if (gagnantTour == 0)
                {
                    currentScoreOp1++;
                    Debugger.log(set.Match.Oppnents[0] + " prend l'avantage");

                }
                else
                {
                    currentScoreOp2++;
                    Debugger.log(set.Match.Oppnents[1] + " prend l'avantage");

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
                if (currentScoreOp1 == 41 || currentScoreOp1 == 40)
                {

                    Debugger.log(set.Match.Oppnents[0] + " remporte la partie");

                    winner = set.Match.Oppnents[0];

                    return;
                }

                // Le joueur B était en avantage, mais il la perdu
                if (currentScoreOp2 == 41)
                {
                    currentScoreOp2--;
                    Debugger.log(set.Match.Oppnents[1] + " perd sont avantage, retour a l'égalité!");

                    return;
                }

                currentScoreOp1 += currentScoreOp1 == 30 ? 10 : 15;

                Debugger.log(set.Match.Oppnents[0] + " : " + currentScoreOp1);

            }
            else
            {
                // Le joueur B était en avantage, il a marqué le second point, il gagne
                // Ou alors il a gagné avec 40 points (la vérification de l'égalité ce fais plus haut!)
                if (currentScoreOp2 == 41 || currentScoreOp2 == 40)
                {
                    Debugger.log(set.Match.Oppnents[1] + " remporte la partie");

                    winner = set.Match.Oppnents[1];

                    return;
                }

                // Le joueur A était en avantage, mais il la perdu
                if (currentScoreOp1 == 41)
                {
                    currentScoreOp1--;
                    Debugger.log(set.Match.Oppnents[0] + " perd sont avantage, retour a l'égalité!");

                    return;
                }

                currentScoreOp2 += currentScoreOp2 == 30 ? 10 : 15;

                Debugger.log(set.Match.Oppnents[1] + " : " + currentScoreOp2);
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

