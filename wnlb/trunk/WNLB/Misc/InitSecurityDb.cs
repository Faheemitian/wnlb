using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Security;
using WebMatrix.WebData;
using WNLB.Models;

namespace WNLB.Misc
{
    public class InitSecurityDb : DropCreateDatabaseIfModelChanges<AppDbContext>
    {
        protected override void Seed(AppDbContext context)
        {

            if (!WebSecurity.Initialized)
            {
                WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);
            }
            
            var membership = (SimpleMembershipProvider)Membership.Provider;

            if (membership.GetUser("admin", false) == null)
            {
                membership.CreateUserAndAccount("admin", "admin");
            }

            //context.Servers.Add(new Server() { ServerName = "LocalConfigSrv", ServerHost = "localhost", Port = 5555 });
            //context.Applications.Add(new Application() { AppName = "Config Console", Path = "/_config" });

            //base.Seed(context);
        }
    }
}