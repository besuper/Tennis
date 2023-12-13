
using System;


namespace Tennis.Objects
{
    public class SuperTieBreak : Set
    {
        public SuperTieBreak(Match match) : base(match)
        {
        }



        //ScoreOP1 /// ScoreOP2 +1 

        public void Play(int max)
        {
            Random rand = new Random();
            int rnd;

            winner = null;
            do
            {
                rnd = rand.Next(0, 2);

                if (rnd == 0)
                {
                    scoreOp1 += 1;
                }
                else
                {
                    scoreOp2 += 1;
                }

                if (scoreOp1 >= max || scoreOp2 >= max)
                {
                    if (scoreOp1 > scoreOp2)
                    {
                        if (scoreOp1 - scoreOp2 >= 2)
                        {
                            winner = Match.Oppnents[0];
                        }
                    }
                    else
                    {
                        if (scoreOp2 - scoreOp1 >= 2)
                        {
                            winner = Match.Oppnents[1];
                        }
                    }
                }

            } while (winner == null);

            if (winner == Match.Oppnents[0])
            {
                Debugger.log($"{winner} à gagné le superTieBreak {scoreOp1} - {scoreOp2}");
            }
            else
            {
                Debugger.log($"{winner} à gagné le superTieBreak {scoreOp2} - {scoreOp1}");
            }
        }

    }
}