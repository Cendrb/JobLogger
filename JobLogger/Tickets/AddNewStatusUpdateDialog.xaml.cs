using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Windows.Shapes;

namespace JobLogger.Tickets
{
    /// <summary>
    /// Interaction logic for AddNewStatusUpdateDialog.xaml
    /// </summary>
    public partial class AddNewStatusUpdateDialog : Window
    {
        public Ticket Ticket { get; private set; }
        public bool Saved { get; private set; }

        public AddNewStatusUpdateDialog(Ticket ticket)
        {
            InitializeComponent();

            this.Ticket = ticket;

            this.Title = $"Add status update for {ticket.TracTicket.ID}";
            this.statusUpdatesTextBlock.Text = ticket.GetStatusUpdatesString();
            this.newStatusUpdateAuthorTextBox.Text = ConfigurationManager.AppSettings["AuthorAbbreviation"];
            this.newStatusUpdateDateDatePicker.SelectedDate = DateTime.Today;
        }

        private void SaveAndClose()
        {
            if (string.IsNullOrWhiteSpace(this.newStatusUpdateTextBox.Text))
            {
                MessageBox.Show("Status update text cannot be empty", "Invalid values", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (!this.newStatusUpdateDateDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Status update date needs to be selected", "Invalid values", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (string.IsNullOrWhiteSpace(this.newStatusUpdateAuthorTextBox.Text))
            {
                MessageBox.Show("Status update author cannot be empty", "Invalid values", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (config.AppSettings.Settings.AllKeys.Contains("AuthorAbbreviation"))
                {
                    config.AppSettings.Settings["AuthorAbbreviation"].Value = this.newStatusUpdateAuthorTextBox.Text;
                }
                else
                {
                    config.AppSettings.Settings.Add("AuthorAbbreviation", this.newStatusUpdateAuthorTextBox.Text);
                }

                config.Save();

                ConfigurationManager.RefreshSection("appSettings");
                this.Ticket.AddStatusUpdate(this.newStatusUpdateAuthorTextBox.Text, this.newStatusUpdateDateDatePicker.SelectedDate.Value, this.newStatusUpdateTextBox.Text);
                this.Saved = true;
                this.Close();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }

            if (e.Key == Key.Enter)
            {
                this.SaveAndClose();
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            this.SaveAndClose();
        }
    }
}
