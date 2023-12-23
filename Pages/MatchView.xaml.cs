using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Tennis.Objects;

namespace Tennis.Pages
{
    /// <summary>
    /// Interaction logic for MatchView.xaml
    /// </summary>
    public partial class MatchView : Window
    {
        private readonly Match match;

        public MatchView(ref Match match)
        {
            InitializeComponent();

            this.match = match;
            this.DataContext = this.match;

            IsMainClosing = false;
            Score.ItemsSource = this.match.summary;
        }

        public bool IsMainClosing { get; set; }

        public int MatchId()
        {
            return this.match.Id;
        }
    }
}
