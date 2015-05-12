using NLBLib.Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NLBLib.Servers
{
    public class ServerMintoringThread
    {
        private static readonly object _singletonLock = new object();
        private static readonly object _monitorLock = new object();
        private static ServerMintoringThread _instance = null;
        private ServerRegister _serverRegister;
        private Thread _serverMintoringThread;
        private bool _monitoring;

        private ServerMintoringThread() { }
        public static ServerMintoringThread Instance
        {
            get
            {
                lock (_singletonLock)
                {
                    if (_instance == null)
                    {
                        _instance = new ServerMintoringThread();
                    }

                    return _instance;
                }
            }
        }

        public void StartMonitoring(ServerRegister serverRegister)
        {
            if (checkAndSetMonitoring())
            {
                _serverRegister = serverRegister;
                _serverMintoringThread = new Thread(new ThreadStart(ServerMonitor));
                _serverMintoringThread.Name = "Backend Server Monitor";
                _serverMintoringThread.Start();                
            }
        }

        public void StopMonitoring()
        {
            _serverMintoringThread.Interrupt();
        }

        private void ServerMonitor()
        {
            try
            {
                do
                {
                    foreach (var server in _serverRegister.Servers)
                    {
                        if (isDown(server))
                        {
                            server.Status = ServerStatus.DOWN;
                        }
                        else
                        {
                            server.Status = ServerStatus.AVAILABLE;
                        }
                    }

                    Thread.Sleep(TimeSpan.FromMinutes(1));

                } while (true);

            }
            catch 
            {
                Debug.Write("Server mintoring thread interrupted");
            }
        }

        private bool isDown(AppServer server)
        {
            Uri serverUri = new UriBuilder("http", server.Host, server.Port).Uri;
            HttpRequestProcessor processor = new HttpRequestProcessor();

            return !processor.IsHttpAccessible(serverUri);
        }

        private bool checkAndSetMonitoring()
        {
            lock (_monitorLock)
            {
                return !_monitoring ? (_monitoring = true) : false;
            }
        }
    }
}
