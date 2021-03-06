﻿using System;
using System.Runtime.Serialization;
using System.Text;

namespace JobLogger
{
    public class JobRecord
    {
        private const char STARTED_CHAR = 'S';
        private const char TIME_CHAR = '@';
        private const char PROJECT_CHAR = '#';
        private const char TASK_CHAR = '^';
        private const char ENDED_CHAR = 'E';
        private const char END_OF_STRING_CHAR = '\r'; // can be used cause it's replaced by "" before

        private char[] STOP_CHARS = { TIME_CHAR, PROJECT_CHAR, TASK_CHAR, END_OF_STRING_CHAR };

        public JobRecord(string serializedString)
        {
            ParseSerializedString(serializedString);
        }

        public JobRecord(TimeSpan started, string projectName, string taskName, TimeSpan? timeFinished)
        {
            this.TimeStarted = started;
            this.ProjectName = projectName;
            this.TaskName = taskName;
            this.TimeFinished = timeFinished;
        }

        public TimeSpan TimeStarted { get; private set; }
        public TimeSpan? TimeFinished { get; set; }
        public string ProjectName { get; private set; }
        public string TaskName { get; private set; }
        public bool Done
        {
            get
            {
                return this.TimeFinished.HasValue;
            }
        }

        private void ParseSerializedString(string serializedString)
        {
            string[] parts = serializedString.Split('\n');
            if (parts.Length != 2 && parts.Length != 1)
            {
                throw new JobRecordParseException("Every job log needs to have 1 or 2 lines");
            }

            string firstLine = parts[0] + END_OF_STRING_CHAR; // to mark the end of string to be recognized by STOP_CHARS
            if (firstLine.Substring(0, 1) == STARTED_CHAR.ToString())
            {
                string startTimeString = GetValueStringOf(firstLine, TIME_CHAR);
                try
                {
                    this.TimeStarted = TimeSpan.Parse(startTimeString);
                }
                catch (FormatException e)
                {
                    throw new JobRecordParseException("Start time doesn't have a valid format");
                }

                this.ProjectName = GetValueStringOf(firstLine, PROJECT_CHAR);
                this.TaskName = GetValueStringOf(firstLine, TASK_CHAR);

                if (parts.Length > 1)
                {
                    string secondLine = parts[1] + END_OF_STRING_CHAR; // to mark the end of string to be recognized by STOP_CHARS
                    if (secondLine.Substring(0, 1) == ENDED_CHAR.ToString())
                    {
                        string endTimeString = GetValueStringOf(secondLine, TIME_CHAR);
                        try
                        {
                            this.TimeFinished = TimeSpan.Parse(endTimeString);
                        }
                        catch (FormatException e)
                        {
                            throw new JobRecordParseException("End time doesn't have a valid format");
                        }
                    }
                    else
                    {
                        throw new JobRecordParseException("Second line needs to start with: " + ENDED_CHAR);
                    }
                }
                else
                {
                    this.TimeFinished = null;
                }
            }
            else
            {
                throw new JobRecordParseException("First line needs to start with: " + STARTED_CHAR);
            }
        }

        private string GetValueStringOf(string sourceString, char startIndentifier)
        {
            if (sourceString.IndexOf(startIndentifier) == -1)
            {
                return null;
            }

            string withoutBeginningTillIdentifierExcluded = sourceString.Substring(sourceString.IndexOf(startIndentifier) + 1);
            int firstStopCharIndex = withoutBeginningTillIdentifierExcluded.IndexOfAny(STOP_CHARS) + (sourceString.Length - withoutBeginningTillIdentifierExcluded.Length) - 1;
            int identifierCharIndex = sourceString.IndexOf(startIndentifier);
            return sourceString.Substring(sourceString.IndexOf(startIndentifier) + 1, firstStopCharIndex - identifierCharIndex);
        }

        public string GetSerializedString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(STARTED_CHAR);

            builder.Append(TIME_CHAR);
            builder.Append(this.TimeStarted.ToHoursMinutes());

            builder.Append(PROJECT_CHAR);
            builder.Append(this.ProjectName);

            if (!string.IsNullOrWhiteSpace(this.TaskName))
            {
                builder.Append(TASK_CHAR);
                builder.Append(this.TaskName);
            }

            if (this.Done)
            {
                builder.AppendLine();

                builder.Append(ENDED_CHAR);

                builder.Append(TIME_CHAR);
                builder.Append(this.TimeFinished.Value.ToHoursMinutes());
            }

            return builder.ToString();
        }

        public string GetDisplayString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(this.TimeStarted.ToHoursMinutes());

            builder.Append(PROJECT_CHAR);
            builder.Append(this.ProjectName);

            if (!string.IsNullOrWhiteSpace(this.TaskName))
            {
                builder.Append(TASK_CHAR);
                builder.Append(this.TaskName);
            }

            if (Done)
            {
                builder.Append("$");
                builder.Append(GetTotalMinutes());
            }
            return builder.ToString();
        }

        public double GetTotalMinutes()
        {
            if (this.Done)
            {
                return (this.TimeFinished.Value - this.TimeStarted).TotalMinutes;
            }
            else
            {
                return 0;
            }
        }
    }

    [Serializable]
    internal class JobRecordParseException : Exception
    {
        public JobRecordParseException(string reason) : base("Unable to parse that: " + reason)
        {
        }
    }
}