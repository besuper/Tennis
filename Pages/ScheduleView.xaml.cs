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
        public ObservableCollection<Match> gentlemenSingleMatches = new ObservableCollection<Match>();
        public ObservableCollection<Match> ladieSingleMatches = new ObservableCollection<Match>();
        public ObservableCollection<Match> gentlemensDoubleMatches = new ObservableCollection<Match>();
        public ObservableCollection<Match> ladiesDoubleMatches = new ObservableCollection<Match>();
        public ObservableCollection<Match> mixedDoubleMatches = new ObservableCollection<Match>();


        public ScheduleView()
        {
            InitializeComponent();
            
            this.gentlemenSingleListView.ItemsSource = this.gentlemenSingleMatches;
            this.ladieSingleListView.ItemsSource= this.ladieSingleMatches;
            this.gentlemensDoubleListView.ItemsSource = this.gentlemensDoubleMatches;
            this.ladiesDoubleListView.ItemsSource = this.ladiesDoubleMatches;
            this.mixedDoubleListView.ItemsSource = this.mixedDoubleMatches;


            //this.DataContext = Matches;

            Tournament tour = new Tournament("test");

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

        private void OnMatchesPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Schedule? obj = sender as Schedule;

            
            foreach (Match match in obj.Matches)
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    switch (obj.Type)
                    {
                        case ScheduleType.GentlemenSingle:
                            this.gentlemenSingleMatches.Add(match);
                            break;
                        case ScheduleType.LadiesSingle:
                            this.ladieSingleMatches.Add(match);
                            break;
                        case ScheduleType.GentlemenDouble:
                            this.gentlemensDoubleMatches.Add(match);
                            break;
                        case ScheduleType.LadiesDouble:
                            this.ladiesDoubleMatches.Add(match);
                            break;
                        case ScheduleType.MixedDouble:
                            this.mixedDoubleMatches.Add(match);
                            break;
                        default:
                            break;
                    }
                });
            }

            /*
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                this.gentlemenSingleListView.Items.Refresh();
            });
            */


        }

        private void OpenMatch(object sender, MouseButtonEventArgs e)
        {

            var item = (sender as ListView).SelectedItem;
            if (item != null)
            {
                Match match = (Match)item;
                if (!match.IsPlayed) return;

                MatchView mv = new MatchView(ref match);

                mv.Show();
            }

        }
    }
}
