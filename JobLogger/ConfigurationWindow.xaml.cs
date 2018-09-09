using JobLogger.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace JobLogger
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        public bool ConfigurationSaved { get; private set; }

        public ConfigurationWindow()
        {
            InitializeComponent();

            this.tracUsernameTextBox.Text = Settings.Default.TracUsername;
            this.tracPasswordTextBox.Text = Settings.Default.TracPassword;
            this.tracAuthorAbbreviationTextBox.Text = Settings.Default.AuthorAbbreviation;
            this.mainFolderTextBox.Text = Settings.Default.MainFolder;
        }

        private void MainFolderBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.SelectedPath = this.mainFolderTextBox.Text;
                folderBrowserDialog.Description = "Choose main folder directory";
                DialogResult dialogResult = folderBrowserDialog.ShowDialog();
                if (dialogResult == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                {
                    this.mainFolderTextBox.Text = folderBrowserDialog.SelectedPath;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.TracUsername = this.tracUsernameTextBox.Text;
            Settings.Default.TracPassword = this.tracPasswordTextBox.Text;
            Settings.Default.AuthorAbbreviation = this.tracAuthorAbbreviationTextBox.Text;
            Settings.Default.MainFolder = this.mainFolderTextBox.Text;

            if (string.IsNullOrEmpty(this.tracUsernameTextBox.Text)
                || string.IsNullOrEmpty(this.tracPasswordTextBox.Text)
                || string.IsNullOrEmpty(this.mainFolderTextBox.Text))
            {
                System.Windows.MessageBox.Show("You cannot save an invalid configuration. Fill in the missing fields.", "Incomplete configuration");
            }
            else
            {
                Settings.Default.Save();
                this.ConfigurationSaved = true;
                this.Close();
            }
        }
    }
}
