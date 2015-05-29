using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NLBLib.Misc
{
    public class HttpErrorResponse
    {
        public static void Send(HttpContext context, int code, string message) {
            context.Response.StatusCode = code;
            context.Response.Output.WriteLine(message);
            context.Response.End();
        }
    }
}
