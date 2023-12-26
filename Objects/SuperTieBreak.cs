
using System;

namespace Tennis.Objects
{
    public class SuperTieBreak : Set
    {
        public SuperTieBreak(Match match) : base(match)
        {
        }

        public void Play(int max)
        {
            Random rand = new Random();
            int rnd;

            winner = null;

            do
            {
                rnd = rand.Next(0, 2);

                // Randomize who got the point
                if (rnd == 0)
                {
                    scoreOp1 += 1;
                }
                else
                {
                    scoreOp2 += 1;
                }

                // Check if is there a winner
                // The winner must have more score than the max
                if (scoreOp1 >= max || scoreOp2 >= max)
                {
                    // Then have at least 2 more points than the opponent
                    if (scoreOp1 - scoreOp2 >= 2)
                    {
                        winner = Match.Oppnents[0];
                    }

                    if (scoreOp2 - scoreOp1 >= 2)
                    {
                        winner = Match.Oppnents[1];
                    }
                }

            } while (winner == null); // Continue while there is no winner
        }
    }
}