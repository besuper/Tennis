using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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

        public ScheduleView()
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
            Tournament tour = new Tournament("test");

            // Register event listener
            foreach (var item in tour.ScheduleList)
            {
                item.PropertyChanged += OnMatchesPropertyChanged;
            }

            RunBackground(tour);
        }

        private void RunBackground(Tournament tour)
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

                if (!match.IsPlayed)
                {
                    return;
                }

                MatchView mv = new MatchView(ref match);
                mv.Show();
            }
        }
    }
}
