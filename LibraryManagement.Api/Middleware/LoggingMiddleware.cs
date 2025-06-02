// File: LibraryManagement.Api/Middleware/LoggingMiddleware.cs
using Microsoft.Owin;
using System;
using System.Diagnostics; // For Stopwatch and Debug
using System.Threading.Tasks;

namespace LibraryManagement.Api.Middleware // Ensure this namespace matches your project
{
    public class LoggingMiddleware : OwinMiddleware
    {
        public LoggingMiddleware(OwinMiddleware next)
            : base(next)
        {
        }

        public override async Task Invoke(IOwinContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            // Log request details
            Debug.WriteLine($"Request Starting: {context.Request.Scheme} {context.Request.Method} {context.Request.Path}{context.Request.QueryString} | Remote IP: {context.Request.RemoteIpAddress}");

            // Add a try-finally to ensure response logging happens even if an unhandled exception occurs
            // further down the pipeline (though ErrorHandlingMiddleware should catch most app exceptions)
            try
            {
                await Next.Invoke(context); // Call the next middleware in the pipeline
            }
            finally
            {
                stopwatch.Stop();
                // Log response details
                Debug.WriteLine($"Request Finished: {context.Request.Path}{context.Request.QueryString} responded {context.Response.StatusCode} in {stopwatch.ElapsedMilliseconds}ms");
            }
        }
    }
}