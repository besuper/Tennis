using System.Collections.Generic;
using System.ComponentModel;
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
        private Match match;

        public MatchView(ref Match match)
        {
            this.match = match;
            InitializeComponent();
            this.DataContext = match;
            //When update in match, update in view
            //SetsRecap.ItemsSource = match.Sets;

            IsMainClosing = false;
            Score.ItemsSource = match.summary;
        }

        public bool IsMainClosing { get; set; }

        public int MatchId()
        {
            return match.Id;
        }
    }
}
