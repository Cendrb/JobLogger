using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Deployment.Application;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using CookComputing.XmlRpc;
using JobLogger.Properties;
using JobLogger.Tickets;
using MetaTracInterface;

namespace JobLogger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DateTime currentDate;
        public MainWindow()
        {
            InitializeComponent();

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                this.Title = $"THIS - Trac Hummus Integration Software ${ApplicationDeployment.CurrentDeployment.CurrentVersion}";
            }
            else
            {
                this.Title = "THIS - Trac Hummus Integration Software (development version)";
            }

            if (Settings.Default.WindowSize.Height != 0)
            {
                this.Height = Settings.Default.WindowSize.Height;
            }

            if (Settings.Default.WindowSize.Width != 0)
            {
                this.Width = Settings.Default.WindowSize.Width;
            }

            if (string.IsNullOrEmpty(Settings.Default.TracPassword)
                || string.IsNullOrEmpty(Settings.Default.TracUsername)
                || string.IsNullOrEmpty(Settings.Default.MainFolder))
            {
                ConfigurationWindow configurationWindow = new ConfigurationWindow();
                configurationWindow.ShowDialog();
                if (!configurationWindow.ConfigurationSaved)
                {
                    Environment.Exit(0);
                }
            }

            this.currentDate = DateTime.Today;

            this.teaTimerGrid.Children.Add(new TeaTimer(new List<TimeSpan>()
            {
                new TimeSpan(0, 0, 15),
                new TimeSpan(0, 0, 30),
                new TimeSpan(0, 1, 0),
                new TimeSpan(0, 1, 30),
                new TimeSpan(0, 2, 0),
                new TimeSpan(0, 3, 0),
                new TimeSpan(0, 5, 0),
            }));

            TracComm tracComm = new TracComm(
                Settings.Default.TracUsername,
                Settings.Default.TracPassword);

            Directory.CreateDirectory(Settings.Default.MainFolder);
            TicketLoader ticketLoader = new TicketLoader(Path.Combine(Settings.Default.MainFolder, "tickets.txt"));

            TicketingControl ticketingControl = new TicketingControl(ticketLoader, tracComm);
            ticketingControl.Margin = new Thickness(10, 23, 0, 10);

            this.mainGrid.Children.Add(ticketingControl);

            ReloadUI();

            AppDomain.CurrentDomain.UnhandledException += this.CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Clipboard.SetText(e.ExceptionObject.ToString());
            if (MessageBox.Show("Do you want to edit the configuration?\n\nException copied to cliboard\n\n" + e.ExceptionObject.ToString(), "Unhandled exception", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
            {
                ConfigurationWindow configurationWindow = new ConfigurationWindow();
                configurationWindow.ShowDialog();
            }
        }

        private void ReloadUI()
        {
            string currentPath = GetPathForDate(this.currentDate);
            FileInfo currentFileInfo = new FileInfo(currentPath);
            if (!currentFileInfo.Exists)
            {
                currentFileInfo.Create().Close();
            }

            this.todayLabel.Content = Path.GetFileName(currentPath);

            using (StreamReader readingStream = currentFileInfo.OpenText())
            {
                try
                {
                    WorkDay currentWorkDay = new WorkDay(DateTime.Today, readingStream.ReadToEnd());
                    this.recordsList.Items.Clear();
                    foreach (JobRecord jobRecord in currentWorkDay.GetRecords())
                    {
                        this.recordsList.Items.Add(jobRecord.GetDisplayString());
                    }

                    int totalMinutes = currentWorkDay.GetTotalMinutes();
                    this.totalTimeLabel.Content = string.Format("{0:00}:{1:00}", totalMinutes / 60, totalMinutes % 60);
                }
                catch (JobRecordParseException e)
                {
                    MessageBox.Show(e.Message, string.Format("Unable to parse {0}, invalid format", Path.GetFileName(currentPath)), MessageBoxButton.OK, MessageBoxImage.Error);
                    this.OpenInNotepad(currentPath);
                }
            }

            this.UpdateUIVisibilities(
                Settings.Default.ShowTicketing,
                false,
                Settings.Default.ShowTeaTimer);
        }

        private void UpdateUIVisibilities(bool showTicketing, bool showJobLogger, bool showTeaTimer)
        {
            if (showTicketing)
            {
                this.ticketingControlColumn.Width = new GridLength(1, GridUnitType.Star);
            }
            else
            {
                this.ticketingControlColumn.Width = new GridLength(0);
            }

            if (showJobLogger)
            {
                this.jobLoggerColumn.Width = new GridLength(350);
            }
            else
            {
                this.jobLoggerColumn.Width = new GridLength(0);
            }

            if (showTeaTimer)
            {
                this.teaTimerColumn.Width = new GridLength(150);
            }
            else
            {
                this.teaTimerColumn.Width = new GridLength(0);
            }
        }

        private string GetPathForDate(DateTime date)
        {
            string todayFileName = date.ToString("dd. MM. yyyy") + ".txt";

            DirectoryInfo saveLocation = new DirectoryInfo(Settings.Default.MainFolder);
            return Path.Combine(saveLocation.ToString(), todayFileName);
        }

        private void OpenInNotepad(string path)
        {
            Process.Start("notepad.exe", path);
        }

        private void TodayLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenInNotepad(this.GetPathForDate(this.currentDate));
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            ReloadUI();
        }

        private void CurrentDateDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            this.currentDate = (DateTime)e.AddedItems[0];
            this.ReloadUI();
        }

        private void ConfigurationMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow();
            configurationWindow.ShowDialog();
            if (configurationWindow.ConfigurationSaved)
            {
                System.Windows.Forms.Application.Restart();
                Application.Current.Shutdown();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.Default.WindowSize = new System.Drawing.Size((int)this.Width, (int)this.Height);
            Settings.Default.Save();
        }
    }
}
