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
            ScheduleView scheduleView = new ScheduleView(this.textbox.Text);
            scheduleView.Show();

            this.Close();
            this.Owner.Close(); // Close the main window
        }
    }
}
