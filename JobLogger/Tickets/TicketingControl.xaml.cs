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
using System.Windows.Navigation;
using System.Windows.Shapes;
using JobLogger.Tickets.States;

namespace JobLogger.Tickets
{
    /// <summary>
    /// Interaction logic for TicketingControl.xaml
    /// </summary>
    public partial class TicketingControl : UserControl
    {
        private List<Ticket> tickets = new List<Ticket>();
        private TicketLoader ticketLoader;

        private StateQueues stateQueues = new StateQueues()
        {
            new StateQueue("Development", TicketStateRegistry.Instance.Get<ProgrammingTicketState>()),
            new StateQueue("Estimating", TicketStateRegistry.Instance.Get<EstimatingMeetingTicketState>())
        };

        public TicketingControl()
        {
            InitializeComponent();

            this.ticketLoader = new TicketLoader(System.IO.Path.Combine(ConfigurationManager.AppSettings["MainFolder"], "tickets.txt"), ConfigurationManager.AppSettings["TracUsername"], ConfigurationManager.AppSettings["TracPassword"], this.stateQueues);

            this.queueSelectComboBox.ItemsSource = this.stateQueues.Select(queue => queue.Name);
            this.queueSelectComboBox.SelectedIndex = 0;

            this.ReloadTickets();
        }

        private async void ReloadTickets()
        {
            this.loadButton.IsEnabled = false;
            this.includeDoneCheckBox.IsEnabled = false;
            this.loadButton.Content = "Loading...";

            this.tickets = new List<Ticket>();

            this.ReloadUI();

            bool includeDoneTickets = this.includeDoneCheckBox.IsChecked.GetValueOrDefault();

            await Task.Factory.StartNew(() =>
            {
                foreach (Ticket ticket in this.ticketLoader.Load(includeDoneTickets))
                {
                    this.tickets.Add(ticket);
                    this.Dispatcher.Invoke(() => this.AddTicketToUI(ticket));
                }
            });

            this.loadButton.IsEnabled = true;
            this.includeDoneCheckBox.IsEnabled = true;
            this.loadButton.Content = "Reload";
        }

        private void ReloadUI()
        {
            this.ticketsStackPanel.Children.Clear();
            foreach (Ticket ticket in this.tickets)
            {
                this.AddTicketToUI(ticket);
            }
        }

        private void AddTicketToUI(Ticket ticket)
        {
            TicketControl ticketControl = new TicketControl(ticket);
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem = new MenuItem() { Header = "Remove" };
            menuItem.Click += (s, e) =>
            {
                this.tickets.Remove(ticket);
                this.ticketLoader.Save(this.tickets, !this.includeDoneCheckBox.IsChecked.GetValueOrDefault());
                this.ReloadUI();
            };
            contextMenu.Items.Add(menuItem);
            ticketControl.ContextMenu = contextMenu;
            this.ticketsStackPanel.Children.Add(ticketControl);
            ticketControl.TicketChanged += t => this.ticketLoader.Save(this.tickets, !this.includeDoneCheckBox.IsChecked.GetValueOrDefault());
        }

        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            this.ReloadTickets();
        }

        private void ticketNumberTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                int ticketID;
                if (int.TryParse(this.ticketNumberTextBox.Text, out ticketID) && this.queueSelectComboBox.SelectedItem != null)
                {
                    if (!this.tickets.Any() || this.tickets.Any(ticket => ticket.TracTicket.ID != ticketID))
                    {
                        StateQueue queue = this.stateQueues[this.queueSelectComboBox.SelectedIndex];
                        this.tickets.Add(this.ticketLoader.CreateNew(ticketID, queue.InitialState));
                        this.ticketLoader.Save(this.tickets, !this.includeDoneCheckBox.IsChecked.GetValueOrDefault());
                        this.ReloadUI();
                    }
                }

                this.ticketNumberTextBox.Text = string.Empty;
            }
        }

        private void includeDoneCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.ReloadTickets();
        }
    }
}
