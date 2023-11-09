using System.Windows;
using Tennis.Pages;

namespace Tennis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenCreateTournament(object sender, RoutedEventArgs e)
        {
            CreateTournamentView tournamentView = new CreateTournamentView();
            tournamentView.Owner = this;
            tournamentView.ShowDialog();
        }
    }
}
