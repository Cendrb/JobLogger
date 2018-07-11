using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaTracInterface
{
    public class TicketTestPlanAttachment : TicketAttachment
    {
        public int TestPlanTicketID { get; private set; }
        public string TestPlanAuthor { get; private set; }
        public string TestPlanVersion { get; private set; }
        public bool IsTestPlanFinal
        {
            get
            {
                return this.TestPlanVersion.Contains("2");
            }
        }

        public TicketTestPlanAttachment()
        {
        }

        public static TicketTestPlanAttachment TryParse(TicketAttachment ticketAttachment)
        {
            try
            {
                string fileName = Path.GetFileNameWithoutExtension(ticketAttachment.FileName);
                int firstNumberIndex = 0;
                do
                {
                    firstNumberIndex++;

                    if (firstNumberIndex >= fileName.Length)
                    {
                        // no number in the filename
                        return null;
                    }
                }
                while (!char.IsNumber(fileName[firstNumberIndex]));

                int numberLength = 1;
                while (char.IsNumber(fileName[firstNumberIndex + numberLength]))
                {
                    numberLength++;
                }

                int ticketID = int.Parse(fileName.Substring(firstNumberIndex, numberLength));
                string restOfString = fileName.Substring(firstNumberIndex + numberLength);
                string[] parts = restOfString.Split('_');

                return new TicketTestPlanAttachment()
                {
                    FileName = ticketAttachment.FileName,
                    Author = ticketAttachment.Author,
                    Created = ticketAttachment.Created,
                    Description = ticketAttachment.Description,
                    Size = ticketAttachment.Size,
                    TestPlanTicketID = ticketID,
                    TestPlanAuthor = parts[0],
                    TestPlanVersion = parts[1]
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
