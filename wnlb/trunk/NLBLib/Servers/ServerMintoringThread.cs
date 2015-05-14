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
    /// <summary>
    /// Provides server monitoring capability
    /// </summary>
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
                    //
                    // @ThreadSafe Singleton
                    //
                    if (_instance == null)
                    {
                        _instance = new ServerMintoringThread();
                    }

                    return _instance;
                }
            }
        }

        /// <summary>
        /// Stores a reference to register and starts monitoring list of registered servers
        /// on a separate thread.
        /// </summary>
        /// <param name="serverRegister"></param>
        public void StartMonitoring(ServerRegister serverRegister)
        {
            //
            // Make sure we don't start this thread twice
            //
            if (checkAndSetMonitoring())
            {
                _serverRegister = serverRegister;
                _serverMintoringThread = new Thread(new ThreadStart(ServerMonitor));
                _serverMintoringThread.Name = "Backend Server Monitor";
                _serverMintoringThread.Start();                
            }
        }

        /// <summary>
        /// Stops server monitoring thread
        /// </summary>
        public void StopMonitoring()
        {
            //
            // Our worker looks for interrupt signal to stop
            //
            _serverMintoringThread.Interrupt();
        }

        // Actual worker method for the thread. Worker iterates through server
        // list every few mins and marks server down or available.
        private void ServerMonitor()
        {
            try
            {
                AppServer[] serverList = _serverRegister.Servers.ToArray<AppServer>();

                do
                {
                    foreach (var server in serverList)
                    {
                        if (!(server is LocalAppServer) && isDown(server))
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

        //
        // checks if server host is accessible
        //
        private bool isDown(AppServer server)
        {
            Uri serverUri = new UriBuilder("http", server.Host, server.Port).Uri;
            HttpRequestProcessor processor = new HttpRequestProcessor();

            return !processor.IsHttpAccessible(serverUri);
        }

        //
        // Sets and returns monitoring flag if it's not already set. @ThreadSafe
        //
        private bool checkAndSetMonitoring()
        {
            lock (_monitorLock)
            {
                return !_monitoring ? (_monitoring = true) : false;
            }
        }
    }
}
