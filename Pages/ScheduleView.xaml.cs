using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp1.Pages
{
    /// <summary>
    /// Interaction logic for ScheduleView.xaml
    /// </summary>
    public partial class ScheduleView : Window
    {
        public ObservableCollection<Match> Matches;

        public ScheduleView()
        {
            this.Matches = new ObservableCollection<Match>();
            InitializeComponent();
            this.DataContext = Matches;

            Tournament tour = new Tournament("test");

            List<Player> players = tour.GetPlayers();

            Schedule sc = new Schedule(tour, ScheduleType.GentlemenSingle, players);
            List<Opponent> opponents = sc.MakeGroups();
            List<Match> cc = sc.CreateMatches(opponents);
            foreach (Match match in cc)
            {
                this.Matches.Add(match);
            }

            RunBackground(cc[0]);
        }

        private void RunBackground(Match tour)
        {
            BackgroundWorker bw = new BackgroundWorker();

            bw.DoWork += (sender, args) =>
            {
                tour.Play();
            };

            bw.RunWorkerCompleted += (sender, args) =>
            {
                if (args.Error != null)
                {
                    MessageBox.Show(args.Error.ToString());
                }
            };

            bw.RunWorkerAsync();
        }

        private void OpenMatch(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;
            if (item != null)
            {
                Match match = (Match)item;
                if (!match.IsPlayed()) return;

                MatchView mv = new MatchView(ref match);

                mv.Show();
            }

        }
    }
}
