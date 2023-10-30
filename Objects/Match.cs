
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

public class Match {

    private DateTime date;
    private TimeSpan duration;

    private int round;
    private int currentSet = 0;
    private int matchSets = 0;

    private Court? court;
    private Schedule schedule;
    private Referee referee;
    private List<Opponent> opponents;
    private List<Set> sets;

    public List<Opponent> Oppnents { get { return opponents; } set { opponents = value; } }
    public Schedule Schedule { get { return schedule; } }
    public Referee Referee { get { return referee; } set { referee = value; } }
    public int CurrentSet { get { return currentSet; } }

    public Match(Schedule schedule, List<Opponent> opponents)
    {
        this.schedule = schedule;
        this.opponents = opponents;
        this.sets = new List<Set>();
    }

    public void Play()
    {
        Set temp;

        matchSets = schedule.NbWinningSets();

        Console.WriteLine("Match arbitré par " + referee);

        for (currentSet = 1; currentSet <= matchSets; currentSet++)
        {
            temp = new Set(this);
            sets.Add(temp);
            temp.Play();

            int SetsPlayerA = ScoreOpponentA();
            int SetsPlayerB = ScoreOpponentB();

            if (matchSets == 3)
            {
                // Si le joueur A a au moins gagné 2 set, il gagne
                if (SetsPlayerA >= 2)
                {
                    Console.WriteLine(opponents[0] + " a remporté le matche!!");

                    break;
                }

                // Si le joueur B a au moins gagné 2 set, il gagne
                if (SetsPlayerB >= 2)
                {
                    Console.WriteLine(opponents[1] + " a remporté le matche!!");

                    break;
                }
            }
            else if (matchSets == 5)
            {
                // Si le joueur A a au moins gagné 3 set, il gagne
                if (SetsPlayerA >= 3)
                {
                    Console.WriteLine(opponents[0] + " a remporté le matche!!");

                    break;
                }

                // Si le joueur B a au moins gagné 3 set, il gagne
                if (SetsPlayerB >= 3)
                {
                    Console.WriteLine(opponents[1] + " a remporté le matche!!");

                    break;
                }
            }
        }

        // TODO: Remove null verification (match always have a referee)
        if(referee != null)
        {
            referee.Release();
        }

        int scorePlayerA = ScoreOpponentA();
        int scorePlayerB = ScoreOpponentB();

        Console.WriteLine("Fin du match");
        Console.WriteLine("Scores : " + scorePlayerA + " - " + scorePlayerB);

    }

    public Opponent GetWinner() {
        return ScoreOpponentA() > ScoreOpponentB() ? opponents[0] : opponents[1] ;
    }

    public int ScoreOpponentA()
    {
        int SetsOpponentA = 0;

        foreach (Set gameSet in sets)
        {
            if (gameSet.Winner != null && gameSet.Winner == opponents[0])
            {
                SetsOpponentA++;
            }
        }

        return SetsOpponentA;
    }

    public int ScoreOpponentB()
    {
        int SetsOpponentB = 0;

        foreach (Set gameSet in sets)
        {
            if (gameSet.Winner != null && gameSet.Winner == opponents[1])
            {
                SetsOpponentB++;
            }
        }

        return SetsOpponentB;
    }

    // Check score equality before using this
    public bool IsWinningSet()
    {
        return (matchSets == 5 && ScoreOpponentA() == 2) || (matchSets == 3 && ScoreOpponentA() == 1);
    }

}