using System.Threading.Tasks;
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

        private async void OpenTournament(object sender, RoutedEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;

            if (item != null)
            {
                Tournament tournament = (Tournament)item;

                ScheduleView scheduleView = new ScheduleView(tournament);
                scheduleView.Show();

                // Start database loading in a new Task
                await Task.Run(() =>
                {
#if DEBUG
                    scheduleView.LoadFromDatabase();

#else
                    try
                    {
                        scheduleView.LoadFromDatabase();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, "Erreur", button: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                        App.Current.Dispatcher.Invoke((Action)delegate
                        {
                            this.Close();
                        });
                    }
#endif

                });

                this.Close();
            }
        }
    }
}
