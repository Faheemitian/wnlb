using NLBLib.Servers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Management;

namespace NLBLib.HealthEvents
{
    public class EventManager
    {
        public const int SERVER_DOWN_EVENT_CODE = WebEventCodes.WebExtendedBase + 30;
        public const int SERVER_UP_EVENT_CODE = WebEventCodes.WebExtendedBase + 31;

        public static void RaiseServerDownEvent(AppServer server, object eventSource)
        {
            string message = String.Format("{0} SERVER IS DOWN", server.Name);
            string[] details =  new string[] { String.Format("Backend App Server {0} is DOWN", server.Name),
                                  String.Format("Hostname: {0}", server.Hostname),
                                  String.Format("IP: {0}", server.IPAddress),
                                  String.Format("PORT: {0}", server.Port) };

            try
            {
                new HealthEvents.ErrorEvent(message, details, eventSource, SERVER_DOWN_EVENT_CODE, null).Raise();
            }
            catch (Exception ex)
            {
                Trace.Write(String.Format("Failed to generate server down event with message {0}", ex.Message));
            }

        }

        public static void RaiseServerUpEvent(AppServer server, object eventSource)
        {
            string message = String.Format("{0} SERVER IS BACK UP", server.Name);
            string[] details = new string[] { String.Format("Backend App Server {0} is back up", server.Name),
                                  String.Format("Hostname: {0}", server.Hostname),
                                  String.Format("IP: {0}", server.IPAddress),
                                  String.Format("PORT: {0}", server.Port) };

            try
            {
                new HealthEvents.InfoEvent(message, details, eventSource, SERVER_UP_EVENT_CODE).Raise();
            }
            catch (Exception ex)
            {
                Trace.Write(String.Format("Failed to generate server up event with message {0}", ex.Message));
            }
        }
    }
}
