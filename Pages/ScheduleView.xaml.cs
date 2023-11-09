using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tennis.Objects;

namespace Tennis.Pages
{
    /// <summary>
    /// Interaction logic for ScheduleView.xaml
    /// </summary>
    public partial class ScheduleView : Window
    {
        public Dictionary<ScheduleType, ObservableCollection<Match>> schedulers = new Dictionary<ScheduleType, ObservableCollection<Match>>();
        public List<ListView> listViews = new List<ListView>();
        public List<int> openedMatches = new List<int>();
        public BackgroundWorker bgWorker;

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
                    listViews.Add(temp as ListView);
                }
            }

            // Create tournament
            Tournament tour = new Tournament(tournamentName);

            // Register event listener
            foreach (var item in tour.ScheduleList)
            {
                item.PropertyChanged += OnMatchesPropertyChanged;
            }

            this.Title = tournamentName;

            RunBackground(tour);
        }

        private void RunBackground(Tournament tour)
        {
            bgWorker = new BackgroundWorker();
            bgWorker.WorkerSupportsCancellation = true;

            bgWorker.DoWork += (sender, args) =>
            {
                tour.Play();

                // Since Play is not blocking we need to block the thread with a while
                while(true)
                {
                    if (bgWorker.CancellationPending)
                    {
                        // If the worker is cancelled => stop the tournament
                        tour.StopTournament();
                        break;
                    }
                }
            };

            bgWorker.RunWorkerCompleted += (sender, args) =>
            {
                if (args.Error != null)
                {
                    MessageBox.Show(args.Error.ToString());
                }
            };

            bgWorker.RunWorkerAsync();
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

            if(App.Current == null)
            {
                return;
            } 

            // Always refresh lists when update received from schedule
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                foreach (ListView listView in listViews)
                {
                    listView.Items.Refresh();
                }
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

        private void OnClosed(object sender, EventArgs e)
        {
            // Send stop notification to the background worker
            bgWorker.CancelAsync();
            bgWorker.Dispose();
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            // Open main view when schedule view is closed
            MainWindow mainApp = new MainWindow();
            mainApp.Show();
        }
    }
}
