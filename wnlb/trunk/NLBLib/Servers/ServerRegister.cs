using NLBLib.Misc;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLBLib.Servers
{
    /// <summary>
    /// Server registry keeper
    /// </summary>
    public sealed class ServerRegister
    {
        private readonly IDictionary<string, AppServer> _servers = new ConcurrentDictionary<string, AppServer>(new CaseInsensitiveStringComparer());

        /// <summary>
        /// Registers server in the system.
        /// </summary>
        /// <exception cref="ArgumentException">on duplicates</exception>
        /// <param name="server">Server object</param>
        public void AddServer(AppServer server)
        {
            if (_servers.ContainsKey(server.Name))
            {
                throw new ArgumentException("Another server already registered with name " + server.Name);
            }

            _servers.Add(server.Name, server);
        }

        /// <summary>
        /// Un-registers the server from pool
        /// </summary>
        /// <param name="name">Name of the server to remove</param>
        public void RemoveServer(string name)
        {
            if (_servers.ContainsKey(name))
            {
                _servers.Remove(name);
            }
        }


        /// <summary>
        /// Returns the server for given name
        /// </summary>
        /// <param name="name">Name for server lookup</param>
        /// <returns>Returns the server or null.</returns>
        public AppServer GetServerWithName(string name)
        {
            AppServer server = null;
            _servers.TryGetValue(name, out server);
            return server;
        }

        /// <summary>
        /// Gets readonly collection of servers
        /// </summary>
        public ICollection<AppServer> Servers
        {
            get
            {
                return new ReadOnlyDictionary<string, AppServer>(_servers).Values;
            }
        }
    }
}
