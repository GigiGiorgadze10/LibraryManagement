using Microsoft.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using LibraryManagement.Application.Exceptions; 
using FluentValidation;
using System.Linq;
using Newtonsoft.Json.Serialization;
using System.Web;

namespace LibraryManagement.Api.Middleware
{
    public class ErrorHandlingMiddleware : OwinMiddleware
    {
        public ErrorHandlingMiddleware(OwinMiddleware next)
            : base(next)
        {
        }

        public override async Task Invoke(IOwinContext context)
        {
            try
            {
                await Next.Invoke(context);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"ErrorHandlingMiddleware caught exception: {ex.ToString()}");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(IOwinContext context, Exception exception)
        {
            var httpContext = context.Get<HttpContextBase>("System.Web.HttpContextBase");

            if (httpContext != null && httpContext.Response.IsClientConnected && httpContext.Response.HeadersWritten)
            {
                System.Diagnostics.Trace.TraceError("ErrorHandlingMiddleware: Response headers already written, cannot modify response for this exception.");
                return Task.CompletedTask;
            }

            HttpStatusCode code = HttpStatusCode.InternalServerError;
            object errorObject = new { message = "An unexpected error occurred. Please try again later." };

            if (exception is NotImplementedException && exception.StackTrace != null && exception.StackTrace.Contains("System.Web.HttpContextBase.get_Response"))
            {
                System.Diagnostics.Trace.TraceError("ErrorHandlingMiddleware: Caught the HttpContextBase.get_Response NotImplementedException within HandleExceptionAsync itself.");
            }
            else if (exception is NotFoundException notFoundException)
            {
                code = HttpStatusCode.NotFound;
                errorObject = new { message = notFoundException.Message };
            }
            else if (exception is LibraryManagement.Application.Exceptions.ValidationException appValidationException)
            {
                code = HttpStatusCode.BadRequest;
                errorObject = new { message = appValidationException.Message, errors = appValidationException.Errors };
            }
            else if (exception is FluentValidation.ValidationException fluentValidationException)
            {
                code = HttpStatusCode.BadRequest;
                var errors = fluentValidationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => string.IsNullOrEmpty(g.Key) ? "general" : g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );
                errorObject = new { message = "Validation failed.", errors = errors };
            }

            var resultSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var result = JsonConvert.SerializeObject(errorObject, resultSettings);

            try
            {
                if (httpContext == null || (httpContext.Response.IsClientConnected && !httpContext.Response.HeadersWritten))
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)code;
                    return context.Response.WriteAsync(result);
                }
                else if (httpContext != null && !httpContext.Response.IsClientConnected)
                {
                    System.Diagnostics.Trace.TraceWarning("ErrorHandlingMiddleware: Client disconnected, cannot write error response.");
                }
            }
            catch (Exception writeEx)
            {
                System.Diagnostics.Trace.TraceError($"ErrorHandlingMiddleware: CRITICAL - Failed to write error response: {writeEx.ToString()}");
            }
            return Task.CompletedTask;
        }
    }
}