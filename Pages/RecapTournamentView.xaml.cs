using System.Diagnostics;
using System.Windows;
using Tennis.Objects;

namespace Tennis.Pages
{
    /// <summary>
    /// Interaction logic for RecapTournamentView.xaml
    /// </summary>
    public partial class RecapTournamentView : Window
    {
        public bool IsSystemClosing = false;

        public RecapTournamentView(Tournament tournament)
        {
            InitializeComponent();

            Debug.WriteLine("RecapTournamentView");
            Debug.WriteLine(tournament);

            this.DataContext = tournament;
            this.controls.ItemsSource = tournament.ScheduleList;
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsSystemClosing)
            {
                return;
            }

            e.Cancel = true;
        }
    }
}
