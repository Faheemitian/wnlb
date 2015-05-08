using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WNLB.Modules.LoadBalancer
{
    public class ApplicationRegister
    {
        private SortedSet<Application> _applications = new SortedSet<Application>(new ApplicationPathComparer());

        public void AddAppliction(Application app)
        {
            _applications.Add(app);
        }

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

    class ApplicationPathComparer : IComparer<Application>
    {
        public int Compare(Application x, Application y)
        {
            return x.AppPath.Length - y.AppPath.Length;
        }
    }
}