using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
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

            ReloadUI();
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
        }

        private string GetPathForDate(DateTime date)
        {
            string todayFileName = date.ToString("dd. MM. yyyy") + ".txt";

            DirectoryInfo saveLocation = new DirectoryInfo(ConfigurationManager.AppSettings["MainFolder"]);
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
    }
}
