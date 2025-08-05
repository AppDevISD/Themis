using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Routing;
using System.Web.UI;

namespace WebUI
{
    public class Global : System.Web.HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);            
        }
        //protected void Application_Error(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        Exception exception = Server.GetLastError();
        //        HttpException httpException = exception as HttpException;
        //        StackTrace trace = new StackTrace(httpException.GetBaseException(), true);
        //        Session["Error"] = httpException;
        //        StackFrame frame = trace.GetFrame(0);
        //        int httpCode = httpException?.GetHttpCode() ?? 500;

        //        Response.Redirect(httpCode != 403 ? $"./Error?err={httpCode}" : $"./AccessDenied?err={httpCode}");

        //    }
        //    catch (Exception ex)
        //    {
        //        Exception exception = ex as HttpException;
        //        HttpException httpException = exception as HttpException;
        //        Session["Error"] = httpException;
        //        int httpCode = httpException?.GetHttpCode() ?? 500;
        //        Response.Redirect(httpCode != 403 ? $"./Error?err={httpCode}" : $"./AccessDenied?err={httpCode}");
        //    }
        //    Server.ClearError();
        //}
    }
}