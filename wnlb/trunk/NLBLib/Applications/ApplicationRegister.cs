using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NLBLib.Applications
{
    public class ApplicationRegister
    {
        private ISet<Application> _applications = new SortedSet<Application>(new ApplicationPathComparer());

        /// <summary>
        /// Registers the application
        /// </summary>
        /// <exception cref="ArgumentException">for duplicates</exception>
        /// <param name="newApp">The app object to register</param>
        public void AddAppliction(Application newApp)
        {
            foreach (var app in _applications)
            {
                if (String.Equals(app.AppName, newApp.AppName, StringComparison.CurrentCultureIgnoreCase)
                    || String.Equals(app.AppPath, newApp.AppPath, StringComparison.CurrentCultureIgnoreCase))
                {
                    throw new ArgumentException("Another app registered with given name or path");
                }
            }

            _applications.Add(newApp);
        }

        /// <summary>
        /// Un-registers the app with given name
        /// </summary>
        /// <param name="name">Name of the app. Returns quietly if app not found</param>
        public void RemoveApplication(string name)
        {
            Application appWithGivenName = null;
            foreach (var app in _applications)
            {
                if (String.Equals(app.AppName, name, StringComparison.CurrentCultureIgnoreCase))
                {
                    appWithGivenName = app;
                    break;
                }
            }

            if (appWithGivenName != null)
            {
                _applications.Remove(appWithGivenName);
            }
        }

        /// <summary>
        /// Gets registered application by checking path against registered apps. 
        /// </summary>
        /// <param name="path">Long path, may even contain query string.</param>
        /// <returns>Registered app object or null</returns>
        public Application GetApplicationForPath(String path)
        {
            foreach(var value in _applications) 
            {
                if (path.StartsWith(value.AppPath))
                {
                    return value;
                }
            }

            return null;
        }
    }


    /// <summary>
    /// Custom comparer for SortedSet of Applications
    /// </summary>
    class ApplicationPathComparer : IComparer<Application>
    {
        /// <summary>
        /// Compares the list based on length of their paths
        /// </summary>
        public int Compare(Application x, Application y)
        {
            return  y.AppPath.Length - x.AppPath.Length;
        }
    }
}