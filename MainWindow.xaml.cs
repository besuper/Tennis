using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Tennis.Objects;
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
            TournamentList.ItemsSource = Tournament.GetTournaments();
        }

        private void OpenCreateTournament(object sender, RoutedEventArgs e)
        {
            CreateTournamentView tournamentView = new CreateTournamentView();
            tournamentView.Owner = this;
            tournamentView.ShowDialog();
        }

        private void OpenTournament(object sender, RoutedEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;

            if (item != null)
            {
                Tournament tournament = (Tournament)item;
                ScheduleView scheduleView = new ScheduleView(tournament);
                scheduleView.Owner = this;
                scheduleView.Show();
            }
        }
    }
}
