using NLBLib.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Management;

namespace NLBLib.HealthEvents
{
    public class ErrorEvent : System.Web.Management.WebErrorEvent
    {
        private string[] _details;

        public ErrorEvent(string msg, string[] details, object eventSource, int eventCode, Exception ex) : base(msg, eventSource, eventCode, ex)
        {
            _details = details;
        }

        public ErrorEvent(string msg, string[] details, object eventSource, int eventCode, int eventDetailCode, Exception ex)
            : base(msg, eventSource, eventCode, eventDetailCode, ex)
        {
            _details = details;

        }

        public override void Raise()
        {
            base.Raise();
        }

        public override void FormatCustomEventDetails(WebEventFormatter formatter)
        {
            formatter.AppendLine("");

            formatter.IndentationLevel += 1;

            foreach (string detail in _details)
            {
                formatter.AppendLine(detail);
            }

            formatter.IndentationLevel -= 1;
        }
    }
}

