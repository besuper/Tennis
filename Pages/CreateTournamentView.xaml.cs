using System;
using System.Windows;

namespace Tennis.Pages
{
    /// <summary>
    /// Interaction logic for CreateTournamentView.xaml
    /// </summary>
    public partial class CreateTournamentView : Window
    {
        public CreateTournamentView()
        {
            InitializeComponent();
        }

        private void CreateTournament(object sender, RoutedEventArgs e)
        {
            string tournamentName = this.textbox.Text.Trim();

            if (tournamentName.Length == 0)
            {
                MessageBox.Show("Nom du tournoi invalide", "Erreur de création", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

#if DEBUG
            ScheduleView scheduleView = new ScheduleView(tournamentName);
            scheduleView.Show();
#else
            try
            {
                ScheduleView scheduleView = new ScheduleView(tournamentName);
                scheduleView.Show();

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
#endif


            this.Close();
            this.Owner.Close(); // Close the main window
        }
    }
}
