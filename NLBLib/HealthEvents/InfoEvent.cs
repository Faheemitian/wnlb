using NLBLib.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Management;

namespace NLBLib.HealthEvents
{
    public class InfoEvent : System.Web.Management.WebRequestEvent
    {
        private string[] _details;

        public InfoEvent(string msg, string[] details, object eventSource, int eventCode)
            : base(msg, eventSource, eventCode)
        {
            _details = details;
        }

        public InfoEvent(string msg, string[] details, object eventSource, int eventCode, int eventDetailCode)
            : base(msg, eventSource, eventCode, eventDetailCode)
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

