using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLogger
{
    // Properties and fields
    partial class WorkDay
    {
        public DateTime Date { get; private set; }
        private List<JobRecord> jobRecords;
    }

    // Business Methods
    partial class WorkDay
    {
        public int GetTotalMinutes()
        {
            int total = 0;
            foreach (JobRecord record in this.jobRecords)
            {
                total += (int) record.GetTotalMinutes();
            }

            return total;
        }
    }

    // Constructors
    partial class WorkDay
    {
        public WorkDay(DateTime date, string serializedString)
            : this(date)
        {
            ParseSerializedString(serializedString);
        }

        public WorkDay(DateTime date)
            : this(date, new List<JobRecord>())
        {
        }

        public WorkDay(DateTime date, List<JobRecord> records)
        {
            this.Date = date;
            this.jobRecords = new List<JobRecord>(records);
        }
    }

    // Record changes
    partial class WorkDay
    {
        public void AddRecord(JobRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException();
            }

            if (this.jobRecords.Contains(record))
            {
                throw new ArgumentException("One JobRecord cannot be done twice within one day");
            }

            this.jobRecords.Add(record);
        }

        public bool RemoveRecord(JobRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException();
            }

            return this.jobRecords.Remove(record);
        }

        public ReadOnlyCollection<JobRecord> GetRecords()
        {
            return new ReadOnlyCollection<JobRecord>(this.jobRecords);
        }
    }

    // Parsing and saving
    partial class WorkDay
    {
        private void ParseSerializedString(string serialized)
        {
            serialized = serialized.Replace("\r", string.Empty);
            foreach (string record in serialized.Split(';'))
            {
                if (!string.IsNullOrWhiteSpace(record.Trim('\n')))
                    this.jobRecords.Add(new JobRecord(record.Trim('\n')));
            }
        }

        public string GetSerializedString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (JobRecord record in this.jobRecords)
            {
                builder.Append(record.GetSerializedString());
                builder.Append(';');
                if (record != this.jobRecords.Last())
                {
                    builder.AppendLine();
                    builder.AppendLine();
                }
            }
            return builder.ToString();
        }
    }
}
