using Microsoft.Owin;
using System;
using System.Diagnostics; 
using System.Threading.Tasks;

namespace LibraryManagement.Api.Middleware
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

            Debug.WriteLine($"Request Starting: {context.Request.Scheme} {context.Request.Method} {context.Request.Path}{context.Request.QueryString} | Remote IP: {context.Request.RemoteIpAddress}");

            try
            {
                await Next.Invoke(context); 
            }
            finally
            {
                stopwatch.Stop();
                Debug.WriteLine($"Request Finished: {context.Request.Path}{context.Request.QueryString} responded {context.Response.StatusCode} in {stopwatch.ElapsedMilliseconds}ms");
            }
        }
    }
}