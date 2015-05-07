using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WNLB.Modules
{
    public class LoadBalancerModule : IHttpModule
    {
        public LoadBalancerModule()
        {
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(this.beginRequestHandler);
            context.EndRequest += new EventHandler(this.endRequestHandler);
        }

        private void endRequestHandler(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpContext context = application.Context;
            HttpResponse response = context.Response;

            response.Headers.Add("WNLB Handler", "Request Ended");
        }

        private void beginRequestHandler(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpContext context = application.Context;
            HttpResponse response = context.Response;

            response.Headers.Add("WNL Handler", "Request Started");
        }

        public void Dispose()
        {
        }
    }
}