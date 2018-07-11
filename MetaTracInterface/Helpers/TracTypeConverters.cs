using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaTracInterface.Helpers
{
    static class TracTypeConverters
    {
        public static readonly TracTypeConverter<string, TicketPriority> TicketPriorityConverter = new TracTypeConverter<string, TicketPriority>(
            source =>
            {
                switch (source)
                {
                    case "critical":
                        return TicketPriority.Critical;
                    case "high":
                        return TicketPriority.High;
                    case "medium":
                        return TicketPriority.Medium;
                    case "low":
                        return TicketPriority.Low;
                    default:
                        return TicketPriority.Unspecified;
                }
            },
            target =>
            {
                switch (target)
                {
                    case TicketPriority.Critical:
                        return "critical";
                    case TicketPriority.High:
                        return "high";
                    case TicketPriority.Medium:
                        return "medium";
                    case TicketPriority.Low:
                        return "low";
                    case TicketPriority.Unspecified:
                        return string.Empty;
                    default:
                        return string.Empty;
                }
            });

        public static readonly TracTypeConverter<string, TicketStatus> TicketStatusConverter = new TracTypeConverter<string, TicketStatus>(
            source =>
            {
                switch (source)
                {
                    case "new":
                        return TicketStatus.New;
                    case "assigned":
                        return TicketStatus.Assigned;
                    case "accepted":
                        return TicketStatus.Accepted;
                    case "code_review":
                        return TicketStatus.CodeReview;
                    case "code_review_passed":
                        return TicketStatus.CodeReviewPassed;
                    case "code_review_failed":
                        return TicketStatus.CodeReviewFailed;
                    case "testing":
                        return TicketStatus.Testing;
                    case "reopened":
                        return TicketStatus.Reopened;
                    case "documenting":
                        return TicketStatus.Documenting;
                    case "closed":
                        return TicketStatus.Closed;
                    default:
                        return TicketStatus.Unknown;
                }
            },
            target =>
            {
                switch (target)
                {
                    case TicketStatus.New:
                        return "new";
                    case TicketStatus.Assigned:
                        return "assigned";
                    case TicketStatus.Accepted:
                        return "accepted";
                    case TicketStatus.CodeReview:
                        return "code_review";
                    case TicketStatus.CodeReviewPassed:
                        return "code_review_passed";
                    case TicketStatus.CodeReviewFailed:
                        return "code_review_failed";
                    case TicketStatus.Testing:
                        return "testing";
                    case TicketStatus.Reopened:
                        return "reopened";
                    case TicketStatus.Documenting:
                        return "documenting";
                    case TicketStatus.Closed:
                        return "closed";
                    case TicketStatus.Unknown:
                        return string.Empty;
                    default:
                        return string.Empty;
                }
            });

        public static readonly TracTypeConverter<string, List<TicketStatusUpdate>> TicketStatusUpdatesConverter = new TracTypeConverter<string, List<TicketStatusUpdate>>(
            source =>
            {
                string[] statusUpdatesStrings = source.Split(new string[] { "[[BR]]" }, StringSplitOptions.RemoveEmptyEntries);
                List<TicketStatusUpdate> ticketStatusUpdates = new List<TicketStatusUpdate>();
                foreach (string statusUpdateString in statusUpdatesStrings)
                {
                    int openingBracketIndex = statusUpdateString.IndexOf("(");
                    int closingBracketIndex = statusUpdateString.IndexOf(")");

                    DateTime? dateTime;
                    if (openingBracketIndex > -1)
                    {
                        try
                        {
                            string dateTimeString = statusUpdateString.Substring(0, openingBracketIndex).Trim();
                            dateTime = DateTime.ParseExact(dateTimeString, "MM/dd/yy", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces);
                        }
                        catch (Exception)
                        {
                            dateTime = null;
                        }
                    }
                    else
                    {
                        dateTime = null;
                    }

                    string authorString;
                    if (openingBracketIndex > -1 && closingBracketIndex > -1)
                    {
                        try
                        {
                            authorString = statusUpdateString.Substring(openingBracketIndex + 1, closingBracketIndex - openingBracketIndex - 1).Trim();
                        }
                        catch (Exception)
                        {
                            authorString = null;
                        }
                    }
                    else
                    {
                        authorString = null;
                    }

                    string text;
                    if (openingBracketIndex > -1 && closingBracketIndex > -1)
                    {
                        text = statusUpdateString.Substring(closingBracketIndex + 1).Trim();
                    }
                    else
                    {
                        text = statusUpdateString.Trim();
                    }

                    ticketStatusUpdates.Add(new TicketStatusUpdate()
                    {
                        DateTime = dateTime,
                        AuthorAbbreviation = authorString,
                        Text = text
                    });

                }

                return ticketStatusUpdates;
            },
            target =>
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (TicketStatusUpdate ticketStatusUpdate in target)
                {
                    if (ticketStatusUpdate.DateTime.HasValue && ticketStatusUpdate.AuthorAbbreviation != null)
                    {
                        stringBuilder.AppendFormat("{0:MM/dd/yy} ({1}) {2}", ticketStatusUpdate.DateTime.Value, ticketStatusUpdate.AuthorAbbreviation, ticketStatusUpdate.Text);
                    }
                    else
                    {
                        stringBuilder.Append(ticketStatusUpdate.Text);
                    }

                    stringBuilder.Append("[[BR]]");
                }

                return stringBuilder.ToString();
            });

        public static readonly TracTypeConverter<string, bool> BooleanTracConverter = new TracTypeConverter<string, bool>(
            source =>
            {
                if (source == "1")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            },
            target =>
            {
                if (target)
                {
                    return "1";
                }
                else
                {
                    return "0";
                }
            });
    }
}
