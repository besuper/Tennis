using System;
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

        private string currentPoint = "";
        private string totalSet = "";

        public Opponent Opponent { get { return match.Opponents[position]; } }
        public Match Match { get { return match; } }
        public string CurrentPoint { get { return match.IsFinished ? "" : currentPoint; } }
        public string TotalSet { get { return totalSet; } set { this.totalSet = value; } }

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
        public Tuple<string, string, string> this[int set]
        {
            get
            {
                if (set >= sets.Count)
                {
                    return Tuple.Create("", "", "Normal");
                }

                Set currentSet = sets[set];
                Game? actualGame = currentSet.ActualGame;

                if (actualGame == null)
                {
                    return Tuple.Create("", "", "Normal");
                }

                int scoreA = currentSet.GameScorePlayerA();
                int scoreB = currentSet.GameScorePlayerB();
                string tieBreakScore = "";

                if (this.position == 0)
                {
                    if (actualGame is TieBreak)
                    {
                        tieBreakScore = $"{actualGame.CurrentScoreOp1}";
                    }

                    return Tuple.Create($"{scoreA}", tieBreakScore, scoreB < scoreA ? "Bold" : "Normal");
                }

                if (actualGame is TieBreak)
                {
                    tieBreakScore = $"{actualGame.CurrentScoreOp2}";
                }

                return Tuple.Create($"{scoreB}", tieBreakScore, scoreB > scoreA ? "Bold" : "Normal");
            }
        }

        public void Update()
        {
            Game? actualGame = match.ActualSet.ActualGame;

            if (actualGame == null) { return; }

            if (this.position == 0)
            {
                currentPoint = $"{actualGame.CurrentScoreOp1}";
                totalSet = $"{match.ScoreOpponentA()}";
            }
            else
            {
                currentPoint = $"{actualGame.CurrentScoreOp2}";
                totalSet = $"{match.ScoreOpponentB()}";
            }
        }
    }
}
