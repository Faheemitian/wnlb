using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLBLib.Servers
{
    public abstract class AppServer
    {
        private readonly string _name;
        private readonly string _host;
        private readonly int _port;

        private DateTime _since;
        private ServerStatus _status;
        private readonly object _statusLock = new object();

        public AppServer(string name, string host, int port)
        {
            _name = name;
            _host = host;
            _port = port;
            _since = DateTime.Now;
            _status = ServerStatus.UKNOWN;
        }

        public int Port
        {
            get { return _port; }
        }

        public String Host
        {
            get { return _host; }
        }

        public string Name
        {
            get { return _name; }
        }

        public bool IsDown
        {
            get { return Status == ServerStatus.DOWN; }
        }

        public bool IsAvailable
        {
            get { return Status == ServerStatus.AVAILABLE; }
        }

        public bool IsSwamped
        {
            get { return Status == ServerStatus.SWAMPED; }
        }

        public int StatusSince
        {
            get { return DateTime.Now.Millisecond - _since.Millisecond; }
        }

        public ServerStatus Status
        {
            get
            {
                lock (_statusLock)
                {
                    return _status;
                }
            }
            set
            {
                lock (_statusLock)
                {
                    if (_status != value && value == ServerStatus.DOWN)
                    {
                        HealthEvents.EventManager.RaiseServerDownEvent(this, null);
                        _since = DateTime.Now;
                    }
                    else if (_status != value && value == ServerStatus.AVAILABLE)
                    {
                        HealthEvents.EventManager.RaiseServerUpEvent(this, null);
                        _since = DateTime.Now;
                    }

                    _status = value;
                }
            }
        }
    }
}
