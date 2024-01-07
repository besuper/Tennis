
using System;

namespace Tennis.Objects
{
    public class TieBreak : Game
    {
        private int max;

        public TieBreak(Set set, int max) : base(set)
        {
            this.max = max;
        }

        public TieBreak(int id, String scoreA, String scoreB, int winner, Set set) : base(id, scoreA, scoreB, winner, set)
        {

        }

        public override void Play()
        {
            int rnd;

            winner = null;

            do
            {
                rnd = Dice();

                // Randomize who got the point
                if (rnd == 0)
                {
                    currentScoreOp1 += 1;
                }
                else
                {
                    currentScoreOp2 += 1;
                }

                // Check if is there a winner
                // The winner must have more score than the max
                if (currentScoreOp1 >= max || currentScoreOp2 >= max)
                {
                    // Then have at least 2 more points than the opponent
                    if (currentScoreOp1 - currentScoreOp2 >= 2)
                    {
                        winner = Set.Match.Opponents[0];
                    }

                    if (currentScoreOp2 - currentScoreOp1 >= 2)
                    {
                        winner = Set.Match.Opponents[1];
                    }
                }

            } while (winner == null); // Continue while there is no winner
        }
    }
}