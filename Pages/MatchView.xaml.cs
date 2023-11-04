using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
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

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
