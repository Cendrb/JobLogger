using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

namespace JobLogger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string todayFileName;
        private string todayPath;
        private WorkDay today;
        private FileInfo todayLog;
        public MainWindow()
        {
            InitializeComponent();

            todayFileName = DateTime.Today.Date.ToString("dd. MM. yyyy") + ".txt";

            DirectoryInfo saveLocation = new DirectoryInfo(@"c:\Users\Cendrb\OneDrive\Dokumenty\joblog");
            todayPath = Path.Combine(saveLocation.ToString(), todayFileName);
            todayLog = new FileInfo(todayPath);
            if(!todayLog.Exists)
                todayLog.Create().Close();

            reloadToday();
        }

        private void reloadToday()
        {
            if (todayLog.Exists)
            {
                using (StreamReader readingStream = todayLog.OpenText())
                {
                    today = new WorkDay(DateTime.Today, readingStream.ReadToEnd());
                }
            }
            else
            {
                today = new WorkDay(DateTime.Today);
            }

            recordsList.Items.Clear();
            foreach (JobRecord jobRecord in today.GetRecords())
            {
                recordsList.Items.Add(jobRecord.GetDisplayString());
            }

            int totalMinutes = today.GetTotalMinutes();

            totalMinutesLabel.Content = totalMinutes + " m";
            todayLabel.Content = todayFileName;
        }

        public void CreateSamples()
        {
            // create new file everytime
            using (StreamWriter writer = todayLog.CreateText())
            {
                today.AddRecord(new JobRecord(TimeSpan.Parse("18:27"), "VitMed", "Fix data bindings", TimeSpan.Parse("18:50")));
                today.AddRecord(new JobRecord(TimeSpan.Parse("19:30"), "Laktát", "Reorganize partial classes in business objects", TimeSpan.Parse("20:30")));
                writer.Write(today.GetSerializedString());
            }
        }

        private void todayLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("notepad.exe", todayPath);
        }

        private void reloadButton_Click(object sender, RoutedEventArgs e)
        {
            reloadToday();
        }
    }
}
