using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Tennis.Objects
{
    public class MatchSummary : INotifyPropertyChanged
    {
        private readonly int position;
        private readonly List<Set> sets;
        private readonly Match match;

        private string tieBreakScore = "";
        private string currentPoint = "";
        private string totalSet = "";

        public Opponent Opponent { get { return match.Oppnents[position]; } }
        public Match Match { get { return match; } }
        public string CurrentPoint { get { return currentPoint; } set { this.currentPoint = value; } }

        public string TotalSet { get { return totalSet; } set { this.totalSet = value; } }

        public string TieBreakScore { get { return tieBreakScore; } set { this.tieBreakScore = value; } }

        public event PropertyChangedEventHandler? PropertyChanged;

        public virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MatchSummary(int position, Match match)
        {
            this.position = position;
            this.match = match;
            this.sets = match.Sets;
        }

        [System.Runtime.CompilerServices.IndexerName("Item")]
        public string[] this[int set]
        {
            get
            {
                TieBreakScore = "";

                if (set >= sets.Count) return new string[] { "", "" };

                if (this.position == 0)
                {
                    if (sets[set].ActualGame.IsTieBreak)
                    {
                        TieBreakScore = $"{sets[set].ActualGame.CurrentScoreOp1}";
                    }

                    return new string[2] { $"{sets[set].GameScorePlayerA()}", TieBreakScore };
                }

                if (sets[set].ActualGame.IsTieBreak)
                {
                    TieBreakScore = $"{sets[set].ActualGame.CurrentScoreOp2}";
                }

                return new string[2] { $"{sets[set].GameScorePlayerB()}", TieBreakScore };
            }
        }

        public void Update()
        {
            if (this.position == 0)
            {
                currentPoint = $"{match.ActualSet.ActualGame.CurrentScoreOp1}";
                totalSet = $"{match.ScoreOpponentA()}";
            }
            else
            {
                currentPoint = $"{match.ActualSet.ActualGame.CurrentScoreOp2}";
                totalSet = $"{match.ScoreOpponentB()}";
            }
        }
    }
}
