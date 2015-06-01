using NLBLib.Routers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLBLib.Applications
{
    /// <summary>
    /// Application meta-data interface for <see cref="BasicApplication"/> and <see cref="DynamicApplication"/> classes.
    /// </summary>
    public interface Application
    {
        /// <summary>
        /// Application name
        /// </summary>
        String AppName { get; }

        /// <summary>
        /// Gets application starting path. / is a valid path for root application.
        /// </summary>
        String AppPath { get; } 

        /// <summary>
        /// Gets the router of application.
        /// </summary>
        RequestRouter RequestRouter { get; }
    }
}
