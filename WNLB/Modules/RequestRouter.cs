using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace WNLB.Modules
{
    public interface RequestRouter
    {
        public void RouteRequest(HttpRequest request);
    }
}
