using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JobLogger
{
    /// <summary>
    /// Interaction logic for TeaTimer.xaml
    /// </summary>
    public partial class TeaTimer : UserControl
    {
        private List<TimeSpan> selectableTimeSpans;
        private TimeSpan selectedTimeSpan;
        private Timer uiUpdateTimer;
        private int elapsedSeconds;
        private bool running;
        private SoundPlayer soundPlayer;

        public TeaTimer(IEnumerable<TimeSpan> selectableTimeSpans)
        {
            InitializeComponent();

            this.soundPlayer = new SoundPlayer(Properties.Resources.bell);

            this.selectableTimeSpans = selectableTimeSpans.ToList();

            for (int i = 0; i < this.selectableTimeSpans.Count; i++)
            {
                TimeSpan timeSpan = this.selectableTimeSpans[i];

                RadioButton radioButton = new RadioButton();
                radioButton.Content = timeSpan.ToMinutesSeconds();
                radioButton.Checked += (object sender, RoutedEventArgs e) =>
                {
                    int timeSpanIndex = this.timeSelectStackPanel.Children.IndexOf((RadioButton)sender);
                    this.SelectTime(this.selectableTimeSpans[timeSpanIndex]);
                };

                radioButton.FontSize = 14;

                this.timeSelectStackPanel.Children.Add(radioButton);

                if (i == 0)
                {
                    radioButton.IsChecked = true;
                }
            }

            this.uiUpdateTimer = new Timer(new TimerCallback(this.TimerElapsed), null, Timeout.Infinite, 1000);

            this.Render();
        }

        private void TimerElapsed(object state)
        {
            this.elapsedSeconds++;

            if (this.elapsedSeconds >= this.selectedTimeSpan.TotalSeconds)
            {
                this.soundPlayer.Play();
                if (this.elapsedSeconds - this.selectedTimeSpan.TotalSeconds < 1)
                {
                    MessageBox.Show("Tea is ready!");
                    this.SetRunning(false);
                }
            }

            this.Render();
        }

        private void SelectTime(TimeSpan timeSpan)
        {
            this.selectedTimeSpan = timeSpan;
            this.Render();
        }

        private void startStopButton_Click(object sender, RoutedEventArgs e)
        {
            this.SetRunning(!this.running);
        }

        private void SetRunning(bool running)
        {
            this.elapsedSeconds = 0;

            this.running = running;
            if (this.running)
            {
                this.uiUpdateTimer.Change(0, 1000);
            }
            else
            {
                this.uiUpdateTimer.Change(Timeout.Infinite, 1000);
            }

            this.Render();
        }

        private void Render()
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                this.timeRemainingLabel.Content = this.selectedTimeSpan.Subtract(TimeSpan.FromSeconds(this.elapsedSeconds)).ToMinutesSeconds();
                this.timeSelectStackPanel.IsEnabled = !this.running;
            });
        }
    }
}
