
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tennis;
using Tennis.Objects;

public class Set {
    private int scoreOp1;

    private int scoreOp2;

    private Match match;
    private Opponent winner;
    private List<Game> games;

    public int ScoreOp1 { get { return scoreOp1;} }
    public int ScoreOp2 { get { return scoreOp2; } }
    public Match Match { get { return match; } }
    public Opponent Winner { get { return winner; } }

    public Set(Match match)
    {
        this.match = match;
        this.games = new List<Game>();
    }

    public int GameScorePlayerA()
    {
        int GameScoreA = 0;

        foreach (Game game in games)
        {
            if (game.Winner != null && game.Winner == match.Oppnents[0])
            {
                GameScoreA++;
            }
        }

        return GameScoreA;
    }

    public int GameScorePlayerB()
    {
        int GameScoreB = 0;

        foreach (Game game in games)
        {
            if (game.Winner != null && game.Winner == match.Oppnents[1])
            {
                GameScoreB++;
            }
        }

        return GameScoreB;
    }

    public void Play()
    {
        Debugger.log($"\n===========[Nouveau set {match.ScoreOpponentA()} - {match.ScoreOpponentB()}]===========");

        Game temp;

        for (int actualGameState = 1; actualGameState <= 12; actualGameState++)
        {
            temp = new Game(this);

            games.Add(temp);
            temp.Play();

            int GamePointA = GameScorePlayerA();
            int GamePointB = GameScorePlayerB();

            if (GamePointA >= 6 && (GamePointA - GamePointB) >= 2)
            {
                // Le joueur A a forcement gagné le SET ici
                winner = match.Oppnents[0];
                break;
            }

            if (GamePointB >= 6 && (GamePointB - GamePointA) >= 2)
            {
                // Le joueur B a forcement gagné le SET ici
                winner = Match.Oppnents[1];
                break;
            }

            if (GamePointA == 6 && GamePointB == 6)
            {
                // Les 12 jeux sont passé MAIS les joueurs sont a égalité, comment donner le point du set?
                // Faire jouer un tie-break
                // Si le set est le dernier du match (3 ou 5 sets) alors c'est un super tie-break
                // Get the last set and replace it by a SuperTieBreak

                Random rand = new Random();
                int rnd = rand.Next(0, 2);

                // dans un match 3 sets
                // si le score est 1 - 1 et que le jeu est 6 - 6 alors super tie break
                // si le score est 0 - 1 pas de super

                if (/* check 1 - 1 or 2 - 2*/match.ScoreOpponentA() == match.ScoreOpponentB() && /*check if 1 - 1 for 3 sets or 2 - 2 for 5 sets*/ match.IsWinningSet())
                {
                    // super tie-break
                    Debugger.log("Super tie-break non pris en charge");

                    // On fais gagner le Set aléatoiremenet temporraielent
                    if (rnd == 0)
                    {
                        winner = match.Oppnents[0];
                    }
                    else
                    {
                        winner = match.Oppnents[1];
                    }

                    break;
                }
                else
                {
                    // tie-break
                    Debugger.log("Tie-break non pris en charge");

                    // On fais gagner le Set aléatoiremenet temporraielent
                    if (rnd == 0)
                    {
                        winner = match.Oppnents[0];
                    }
                    else
                    {
                        winner = match.Oppnents[1];
                    }

                    break;
                }
            }
        }
    }

}