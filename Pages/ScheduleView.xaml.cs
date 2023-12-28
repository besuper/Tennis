﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
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
                Task temp = new Task(() =>
                {
                    schedule.LoadMatches();

                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        SetupListView(schedule.Type, schedule.Matches);
                    });
                });
                temp.Start();
                tasks.Add(temp);
            }

            Task.WaitAll(tasks.ToArray());
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

            // Open main view when schedule view is closed
            MainWindow mainApp = new MainWindow();
            mainApp.Show();
        }

        private void OnClosed(object sender, EventArgs e)
        {
            
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
                    tournament.Create();
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

                if(loadingLabel != null)
                {
                    Label l = (Label)loadingLabel;
                    Grid g = (Grid)l.Parent;

                    g.Children.Remove(l);
                }
            }
        }
    }
}
