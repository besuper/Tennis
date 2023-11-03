
using System;
using System.Collections.Generic;
using Tennis;

public class Set
{
    /// <summary>
    /// Attributes
    /// </summary>
    protected int scoreOp1;
    protected int scoreOp2;

    private Match match;
    protected Opponent? winner;
    private List<Game> games;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="match"></param>
    public Set(Match match)
    {
        this.match = match;
        this.games = new List<Game>();
    }

    /// <summary>
    /// Getters and Setters
    /// </summary>
    public Match Match { get { return match; } }
    public Opponent? Winner { get { return winner; } }

    /// <summary>
    /// WPF Getters
    /// </summary>
    public Game ActualGame { get { return games[games.Count - 1]; } }
    public int ScoreOp1 { get { return GameScorePlayerA(); } }
    public int ScoreOp2 { get { return GameScorePlayerB(); } }

    /// <summary>
    /// Methods
    /// </summary>
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

                if (match.IsWinningSet())
                {
                    // super tie-break
                    Debugger.log("Super tie-break non pris en charge");

                    SuperTieBreak stb = new SuperTieBreak(match);
                    stb.Play(10);
                    match.AddSet(stb);
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