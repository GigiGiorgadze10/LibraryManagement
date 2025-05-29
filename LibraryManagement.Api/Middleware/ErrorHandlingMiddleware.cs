// src/LibraryManagementSystem.Api/Middleware/ErrorHandlingMiddleware.cs
// Make sure this file and class definition exist ONLY ONCE in your project.
using Microsoft.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using LibraryManagement.Application.Exceptions; // Your custom exceptions
using FluentValidation; // For FluentValidation.ValidationException
using System.Linq;
using Newtonsoft.Json.Serialization; // For CamelCasePropertyNamesContractResolver
using static System.Net.Mime.MediaTypeNames;

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
                // Log the exception (e.g., using NLog, Serilog, or System.Diagnostics.Trace)
                System.Diagnostics.Trace.TraceError($"Unhandled exception: {ex}"); // Basic tracing
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(IOwinContext context, Exception exception)
        {
            HttpStatusCode code = HttpStatusCode.InternalServerError; // Default
            object errorObject = new { message = "An unexpected error occurred." };

            if (exception is NotFoundException)
            {
                code = HttpStatusCode.NotFound;
                errorObject = new { message = exception.Message };
            }
            else if (exception is Application.Exceptions.ValidationException appValidationException) // Your custom ValidationException
            {
                code = HttpStatusCode.BadRequest;
                errorObject = new { message = appValidationException.Message, errors = appValidationException.Errors };
            }
            else if (exception is FluentValidation.ValidationException fluentValidationException) // FluentValidation's exception
            {
                code = HttpStatusCode.BadRequest;
                var errors = fluentValidationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => string.IsNullOrEmpty(g.Key) ? "general" : g.Key, // Handle empty property names
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );
                errorObject = new { message = "Validation failed.", errors = errors };
            }
            // Add more specific exception handling as needed

            var resultSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var result = JsonConvert.SerializeObject(errorObject, resultSettings);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}