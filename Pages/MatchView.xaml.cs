using System.Windows;
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
        }
    }
}
