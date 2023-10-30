
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

public class Match {

    private DateTime date;

    private TimeSpan duration;

    private int round;

    private Court? court;
    private Schedule schedule;
    private Referee referee;
    private List<Opponent> opponents;
    private List<Set> sets;

    public List<Opponent> Oppnents { get { return opponents; } set { opponents = value; } }
    public Schedule Schedule { get { return schedule; } }


    private int currentSet = 0;
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


        int MatchSets = schedule.NbWinningSets();

        for (currentSet = 1; currentSet <= MatchSets; currentSet++)
        {
            temp = new Set(this);
            sets.Add(temp);
            temp.Play();

            int SetsPlayerA = ScoreOpponentA();
            int SetsPlayerB = ScoreOpponentB();

            if (MatchSets == 3)
            {
                // Si le joueur A a au moins gagn� 2 set, il gagne
                if (SetsPlayerA >= 2)
                {
                    //Console.WriteLine("Le joueur A a remport� le matche!!");
                    Console.WriteLine(opponents[0] + " a remport� le matche!!");

                    break;
                }

                // Si le joueur B a au moins gagn� 2 set, il gagne
                if (SetsPlayerB >= 2)
                {
                    //Console.WriteLine("Le joueur B a remport� le matche!!");
                    Console.WriteLine(opponents[1] + " a remport� le matche!!");

                    break;
                }
            }
            else if (MatchSets == 5)
            {
                // Si le joueur A a au moins gagn� 3 set, il gagne
                if (SetsPlayerA >= 3)
                {
                    //Console.WriteLine("Le joueur A a remport� le matche!!");
                    Console.WriteLine(opponents[0] + " a remport� le matche!!");

                    break;
                }

                // Si le joueur B a au moins gagn� 3 set, il gagne
                if (SetsPlayerB >= 3)
                {
                    //Console.WriteLine("Le joueur B a remport� le matche!!");
                    Console.WriteLine(opponents[1] + " a remport� le matche!!");

                    break;
                }
            }
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

}