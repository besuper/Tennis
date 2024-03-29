﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
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
        public Dictionary<ScheduleType, ListView> listViews = new Dictionary<ScheduleType, ListView>();
        public List<MatchView> openedMatches = new List<MatchView>();

        private bool started = false;
        private Tournament tournament;

        private RecapTournamentView? recapView = null;

        public ScheduleView(string tournamentName)
        {
            InitializeComponent();

            // Create schedulers from Enum
            Array types = Enum.GetValues(typeof(ScheduleType));

            foreach (ScheduleType type in types)
            {
                SetupListView(type);
            }

            // Create tournament
            tournament = new Tournament(tournamentName);
            tournament.TournamentFinished = OnTournamentFinished;
        }

        // Create tournament from database
        public ScheduleView(Tournament tournament)
        {
            InitializeComponent();

            this.tournament = tournament;
            this.Title = tournament.Name;
            this.started = true;

            this.pageGrid.Children.Remove(this.loading);
        }

        // Load the tournament from database
        public void LoadFromDatabase()
        {
            List<Schedule> schedules = Schedule.GetAllScheduleFromTournament(tournament);
            List<Task> tasks = new List<Task>();

            foreach (Schedule schedule in schedules)
            {
                this.tournament.ScheduleList.Add(schedule);

                Task temp = new Task(() =>
                {
                    schedule.LoadMatches();

                    App.Current.Dispatcher.Invoke(() =>
                    {
                        SetupListView(schedule.Type, schedule.Matches);
                    });
                });
                temp.Start();
                tasks.Add(temp);
            }

            Task.WaitAll(tasks.ToArray());

            App.Current.Dispatcher.Invoke(() =>
            {
                recapView = new RecapTournamentView(tournament);
                recapView.Show();
            });
        }

        // Listen updates from schedulers
        private void OnMatchesPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Schedule? schedule = sender as Schedule;

            if (schedule == null || App.Current == null)
            {
                return;
            }

            App.Current.Dispatcher.Invoke(() =>
            {
                if (e.PropertyName == "NewMatches")
                {
                    // Add new matches in the correct listview
                    foreach (Match match in schedule.Matches)
                    {
                        schedulers[schedule.Type].Add(match);
                    }
                }
            });
        }

        // On click listener to view match details
        private void OpenMatch(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;

            if (item != null)
            {
                Match match = (Match)item;

                // Check if match is playing
                if (!match.IsPlayed)
                {
                    return;
                }

                bool opened = false;

                // Check if the match view is already opened
                foreach (MatchView view in openedMatches)
                {
                    if (view.MatchId() == match.Id)
                    {
                        opened = true;
                    }
                }

                if (opened)
                {
                    return;
                }

                MatchView mv = new MatchView(ref match);
                mv.Show();

                openedMatches.Add(mv);

                // Listen for closed event in MatchView window
                mv.Closed += (sender, e) =>
                {
                    // If close event come from user input
                    if (!mv.IsMainClosing)
                    {
                        openedMatches.Remove(mv);
                    }
                };
            }
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            // Close all match windows
            foreach (MatchView view in openedMatches)
            {
                // Set MainClosing true to avoid collection modified in openedMatches
                view.IsMainClosing = true;
                view.Close();
            }

            // Stop the current tournament
            tournament.StopTournament();

            // Close recap view if opened
            if (recapView != null)
            {
                recapView.IsSystemClosing = true;
                recapView.Close();
            }

            // Open main view when schedule view is closed
            MainWindow mainApp = new MainWindow();
            mainApp.Show();
        }

        // Fired when tournament is finished
        private void OnTournamentFinished()
        {

            // Check if tournament is finished to avoid showing unnecessary RecapTournamentView
            if (!tournament.IsFinished())
            {
                return;
            }

            App.Current.Dispatcher.Invoke(() =>
            {
                recapView = new RecapTournamentView(tournament);
                recapView.Show();

                // Refresh all listviews when the tournament is finished
                foreach (var item in listViews)
                {
                    item.Value.Items.Refresh();
                }
            });
        }

        // When the window is activated
        private async void OnActivated(object sender, EventArgs e)
        {
            // Only run this code once
            if (!started)
            {
                started = true;

                // Create tournament in a separate thread
                await Task.Run(() =>
                {

#if DEBUG
                    tournament.Create();

#else
                    try
                    {
                        tournament.Create();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, "Erreur", button: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            this.Close();
                        });
                    }
#endif

                });

                // Remove waiting text
                this.pageGrid.Children.Remove(this.loading);

                // Register event listener
                foreach (var item in tournament.ScheduleList)
                {
                    // .subscribe
                    item.PropertyChanged += OnMatchesPropertyChanged;
                }

                this.Title = tournament.Name;

                tournament.Play();
            }
        }

        public void SetupListView(ScheduleType type, List<Match>? defaultContent = null)
        {
            object temp = this.tabControl.FindName(type + "ListView");

            if (temp != null && temp is ListView)
            {
                ListView tempView = (ListView)temp;

                schedulers[type] = new ObservableCollection<Match>(defaultContent == null ? new List<Match>() : defaultContent);
                listViews[type] = tempView;

                tempView.ItemsSource = schedulers[type];

                // Delete loading schedule label
                object loadingLabel = this.tabControl.FindName($"Loading{type}");

                if (loadingLabel != null)
                {
                    Label l = (Label)loadingLabel;
                    Grid g = (Grid)l.Parent;

                    g.Children.Remove(l);
                }
            }
        }
    }
}
