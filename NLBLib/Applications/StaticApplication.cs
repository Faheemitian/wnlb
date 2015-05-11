using NLBLib.Routers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NLBLib.Applications
{
    public class StaticApplication : Application
    {
        private String _appName;
        private String _appPath;
        private RequestRouter _requestRouter;

        public StaticApplication(String appName, String appPath, RequestRouter requestRouter)
        {
            _appName = appName;
            _appPath = appPath;
            _requestRouter = requestRouter;
        }

        public RequestRouter RequestRouter
        {
            get { return _requestRouter; }
        }

        public String AppPath
        {
            get { return _appPath; }
        }

        public String AppName
        {
            get { return _appName; }
        }
    }
}