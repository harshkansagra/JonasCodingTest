using System;
using System.Diagnostics;
using System.IO;
using System.Web.Http;
using Microsoft.Extensions.Logging;
using Serilog;

namespace WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            // Configure Serilog
            var logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "log.txt");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();

            // Log the error
            Log.Error(exception, "An unhandled exception occurred");

            // Optional: Clear the error and redirect to a custom error page
            //Server.ClearError();
            //Response.Redirect("~/Error");
        }

        protected void Application_End(object sender, EventArgs e)
        {
            // Ensure to flush and close the log
            Log.CloseAndFlush();
        }
    }
}
