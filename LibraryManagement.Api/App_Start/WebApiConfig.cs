// src/LibraryManagementSystem.Api/App_Start/WebApiConfig.cs
using Newtonsoft.Json.Serialization;
using System.Web.Http;
using FluentValidation.WebApi;
using System.Collections.Generic; // For IEnumerable in AssignJwtSecurityRequirements
using System.Linq; // For LINQ in AssignJwtSecurityRequirements
using Swashbuckle.Application; // For IOperationFilter
using System.Web.Http.Description; // For ApiDescription
using Swashbuckle.Swagger;

namespace LibraryManagement.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Configure JSON formatter to use camelCase and ignore reference loops
            var jsonFormatter = config.Formatters.JsonFormatter;
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            jsonFormatter.UseDataContractJsonSerializer = false; // Ensure Newtonsoft.Json is used

            // Remove XML formatter if you only want JSON responses
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            // Web API routes - Enable Attribute Routing
            config.MapHttpAttributeRoutes();

            // Conventional-based routing (optional if you primarily use attribute routing)
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // FluentValidation integration
            FluentValidationModelValidatorProvider.Configure(config);

            // Dependency Resolver will be set by Autofac in Startup.cs.
        }
    }

    // This class can be here or in SwaggerConfig.cs, ensure namespace is correct
    public class AssignJwtSecurityRequirements : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            var authorizeAttributes = apiDescription.ActionDescriptor.GetCustomAttributes<AuthorizeAttribute>(true); // include inherited
            var allowAnonymousAttributes = apiDescription.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>(true); // include inherited

            if (authorizeAttributes.Any() && !allowAnonymousAttributes.Any())
            {
                if (operation.security == null)
                    operation.security = new List<IDictionary<string, IEnumerable<string>>>();

                var req = new Dictionary<string, IEnumerable<string>>
                {
                    { "Authorization", Enumerable.Empty<string>() } // "Authorization" matches c.ApiKey name in SwaggerConfig
                };
                operation.security.Add(req);
            }
        }
    }
}