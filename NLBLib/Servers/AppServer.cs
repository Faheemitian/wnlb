using NLBLib.Misc;
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

        private DateTime _startTime;
        private ServerStatus _status;
        private readonly object _statusLock = new object();
        private HitCounter _hitCounter = new HitCounter();

        public AppServer(string name, string host, int port)
        {
            _name = name;
            _host = host;
            _port = port;
            _status = ServerStatus.UNKNOWN;
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

        public DateTime AvailableSince
        {
            get
            {
                return _startTime;
            }
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
                        _startTime = DateTime.MinValue;
                    }
                    else if (_status != value && value == ServerStatus.AVAILABLE)
                    {
                        HealthEvents.EventManager.RaiseServerUpEvent(this, null);
                        _startTime = DateTime.Now;
                    }

                    _status = value;
                }
            }
        }

        public HitCounter HitCounter {
            get
            {
                return _hitCounter;
            }
        }

        public override int GetHashCode()
        {
            return (_name + _host + _port).GetHashCode();
        }
    }
}
