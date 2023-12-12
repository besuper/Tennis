using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tennis.Objects
{
    public class MatchSummary
    {

        private int position;
        private List<Set> sets;
        private int TieBreakScore;
        private int CurrentPoint;


        public MatchSummary(int position, List<Set> sets)
        {
            this.position = position;
            this.sets = sets;
        }

        public string this[int set]
        {
            get 
            {
                if (set >= sets.Count) return "";

                if (this.position == 0)
                {
                    CurrentPoint = sets[set].Match.ActualSet.ActualGame.CurrentScoreOp1;
                    return ""+sets[set].GameScorePlayerA();
                }
                CurrentPoint = sets[set].Match.ActualSet.ActualGame.CurrentScoreOp2;
                return "" +sets[set].GameScorePlayerB();
            }
        }

        public int this[double a]
        {
            get { return 0; }
        }
    }
}
