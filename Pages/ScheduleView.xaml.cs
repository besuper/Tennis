using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tennis.DAO;
using Tennis.Factory;
using Tennis.Objects;

namespace Tennis.Pages
{
    /// <summary>
    /// Interaction logic for ScheduleView.xaml
    /// </summary>
    public partial class ScheduleView : Window
    {
        public Dictionary<ScheduleType, ObservableCollection<Match>> schedulers = new Dictionary<ScheduleType, ObservableCollection<Match>>();
        public Dictionary<ScheduleType, ListView> listViews = new Dictionary<ScheduleType, ListView>();
        public List<int> openedMatches = new List<int>();

        private bool started = false;
        private Tournament tournament;

        public ScheduleView(string tournamentName)
        {
            InitializeComponent();

            // Create schedulers from Enum
            Array types = Enum.GetValues(typeof(ScheduleType));

            foreach (ScheduleType type in types)
            {
                object temp = this.tabControl.FindName(type + "ListView");

                if (temp != null && temp is ListView)
                {
                    schedulers[type] = new ObservableCollection<Match>();

                    (temp as ListView).ItemsSource = schedulers[type];
                    listViews[type] = temp as ListView;
                }
            }

            // Create tournament
            tournament = new Tournament(tournamentName);

            // Add the tournament into database
            AbstractDAOFactory factory = AbstractDAOFactory.GetFactory(DAOFactoryType.MS_SQL_FACTORY);
            DAO<Tournament> tournamentDAO = factory.GetTournamentDAO();

            tournamentDAO.Create(tournament);
        }

        // Listen updates from schedulers
        private void OnMatchesPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Schedule? schedule = sender as Schedule;

            if (schedule == null)
            {
                return;
            }

            if (e.PropertyName == "NewMatches")
            {
                // Add new matches in the correct listview
                foreach (Match match in schedule.Matches)
                {
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        schedulers[schedule.Type].Add(match);
                    });
                }
            }

            if (App.Current == null)
            {
                return;
            }

            // Always refresh list when update received from schedule
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                listViews[schedule.Type].Items.Refresh();
            });
        }

        private void OpenMatch(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;

            if (item != null)
            {
                Match match = (Match)item;

                // Check if match is playing and if the match view is not already opened
                if (!match.IsPlayed || openedMatches.Contains(match.GetHashCode()))
                {
                    return;
                }

                openedMatches.Add(match.GetHashCode());

                MatchView mv = new MatchView(ref match);
                mv.Show();

                // Listen for closed event in MatchView window
                mv.Closed += (sender, e) =>
                {
                    openedMatches.Remove(match.GetHashCode());
                };
            }
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            // Stop the current tournament
            tournament.StopTournament();

            // Open main view when schedule view is closed
            MainWindow mainApp = new MainWindow();
            mainApp.Show();
        }

        private async void OnActivated(object sender, EventArgs e)
        {
            if (!started)
            {
                started = true;

                await Task.Run(() =>
                {
                    tournament.Create();
                });

                this.pageGrid.Children.Remove(this.loading);

                // Register event listener
                foreach (var item in tournament.ScheduleList)
                {
                    item.PropertyChanged += OnMatchesPropertyChanged;
                }

                this.Title = tournament.Name;

                tournament.Play();
            }
        }
    }
}
