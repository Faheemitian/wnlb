using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace WNLB.Misc
{
    public class TimeFormatter
    {
        internal static string Format(TimeSpan ts)
        {
            var sb = new StringBuilder();

            if ((int)ts.TotalDays > 0)
            {
                sb.Append((int)ts.TotalDays);
                sb.Append("d ");
            }

            if ((int)ts.Hours > 0 || (int)ts.TotalDays > 0)
            {
                sb.Append(ts.Hours.ToString("00"));
                sb.Append(":");
            }

            sb.Append(ts.Minutes.ToString("00"));
            sb.Append(":");
            sb.Append(ts.Seconds.ToString("00"));

            return sb.ToString();
        }
    }
}