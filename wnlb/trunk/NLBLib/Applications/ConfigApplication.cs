using NLBLib.Routers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NLBLib.Applications
{
    public class ConfigApplication : Application
    {
        private String _appName;
        private String _appPath;
        private RequestRouter _requestRouter;

        public ConfigApplication(String appName, String appPath)
        {
            _appName = appName;
            _appPath = appPath;
            _requestRouter = new ConfigAppRequestRouter();
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