using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLogger
{
    static class Formatting
    {
        public static string ToHoursMinutes(this TimeSpan span)
        {
            return span.ToString("hh\\:mm");
        }
    }
}
