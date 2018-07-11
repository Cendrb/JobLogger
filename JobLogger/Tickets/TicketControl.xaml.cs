﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace JobLogger.Tickets
{
    /// <summary>
    /// Interaction logic for TicketControl.xaml
    /// </summary>
    partial class TicketControl : UserControl
    {
        public event Action<Ticket> TicketChanged;

        private Ticket ticket;

        public TicketControl(Ticket ticket)
        {
            InitializeComponent();

            this.TicketChanged += innerTicket => this.ReloadUI();

            this.ticket = ticket;

            this.statesComboBox.ItemsSource = this.ticket.StateQueue.Select(state => state.Code);

            this.statesComboBox.SelectedIndex = this.ticket.StateQueue.IndexOf(this.ticket.CurrentState);
        }

        private void ReloadUI()
        {
            this.primaryTitleLabel.Content = this.ticket.GetPrimaryString();
            this.statusUpdatesTextBox.Text = this.ticket.GetStatusUpdatesString();

            this.propertiesStackPanel.Children.Clear();
            foreach (TicketPropertyValuePair pair in this.ticket.GetPropertyValuePairs())
            {
                Grid grid = new Grid();
                grid.VerticalAlignment = VerticalAlignment.Top;
                Label leftLabel = new Label();
                leftLabel.VerticalAlignment = VerticalAlignment.Top;
                leftLabel.HorizontalAlignment = HorizontalAlignment.Left;
                leftLabel.Content = pair.Name + ":";
                leftLabel.Padding = new Thickness(0);
                grid.Children.Add(leftLabel);
                Label rightLabel = new Label();
                rightLabel.VerticalAlignment = VerticalAlignment.Top;
                rightLabel.HorizontalAlignment = HorizontalAlignment.Right;
                rightLabel.Content = pair.Value;
                rightLabel.Padding = new Thickness(0);
                grid.Children.Add(rightLabel);
                this.propertiesStackPanel.Children.Add(grid);
            }

            this.warningsStackPanel.Children.Clear();
            foreach (TicketStateValidationMessage message in this.ticket.ValidateTicket().OrderByDescending(message => message.Severity))
            {
                Brush foregroundBrush;
                if (message.Severity == TicketStateValidationMessageSeverity.Info)
                {
                    foregroundBrush = new SolidColorBrush(Colors.Blue);
                }
                else if (message.Severity == TicketStateValidationMessageSeverity.Warning)
                {
                    foregroundBrush = new SolidColorBrush(Colors.Orange);
                }
                else
                {
                    foregroundBrush = new SolidColorBrush(Colors.DarkRed);
                }

                Grid grid = new Grid();
                grid.VerticalAlignment = VerticalAlignment.Top;
                Label leftLabel = new Label();
                leftLabel.VerticalAlignment = VerticalAlignment.Top;
                leftLabel.HorizontalAlignment = HorizontalAlignment.Left;
                leftLabel.Content = message.AffectedField;
                leftLabel.Foreground = foregroundBrush;
                leftLabel.Padding = new Thickness(0, 0, 5, 0);
                leftLabel.FontWeight = FontWeights.Bold;
                leftLabel.ToolTip = message.Message;
                grid.Children.Add(leftLabel);
                StackPanel rightActionStackPanel = new StackPanel();
                rightActionStackPanel.Orientation = Orientation.Horizontal;
                rightActionStackPanel.VerticalAlignment = VerticalAlignment.Top;
                rightActionStackPanel.HorizontalAlignment = HorizontalAlignment.Right;
                foreach (TicketStateValidationMessageAction action in message.Actions)
                {
                    TextBlock actionLabel = new TextBlock();
                    actionLabel.Text = action.Title;
                    actionLabel.Foreground = new SolidColorBrush(Colors.Black);
                    actionLabel.TextDecorations.Add(TextDecorations.Underline);
                    actionLabel.Cursor = Cursors.Hand;
                    actionLabel.Margin = new Thickness(5, 0, 0, 0);
                    actionLabel.MouseDown += (s, e) =>
                    {
                        action.Execute(this.ticket);
                        this.TicketChanged?.Invoke(this.ticket);
                    };

                    rightActionStackPanel.Children.Add(actionLabel);
                }

                grid.Children.Add(rightActionStackPanel);
                this.warningsStackPanel.Children.Add(grid);
            }
        }

        private void statesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ticket.CurrentState = this.ticket.StateQueue[this.statesComboBox.SelectedIndex];
            this.TicketChanged?.Invoke(this.ticket);
        }

        private void primaryTitleLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("http://10.71.23.133:8088/Malo/ticket/" + this.ticket.TracTicket.ID);
        }
    }
}
