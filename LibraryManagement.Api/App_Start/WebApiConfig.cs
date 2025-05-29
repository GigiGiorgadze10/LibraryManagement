using Newtonsoft.Json.Serialization;
using System.Web.Http;
using FluentValidation.WebApi;
using System.Collections.Generic; 
using System.Linq;
using Swashbuckle.Application; 
using System.Web.Http.Description;
using Swashbuckle.Swagger;

namespace LibraryManagement.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            var jsonFormatter = config.Formatters.JsonFormatter;
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            jsonFormatter.UseDataContractJsonSerializer = false; 

            config.Formatters.Remove(config.Formatters.XmlFormatter);

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            FluentValidationModelValidatorProvider.Configure(config);

        }
    }

    public class AssignJwtSecurityRequirements : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            var authorizeAttributes = apiDescription.ActionDescriptor.GetCustomAttributes<AuthorizeAttribute>(true); 
            var allowAnonymousAttributes = apiDescription.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>(true); 

            if (authorizeAttributes.Any() && !allowAnonymousAttributes.Any())
            {
                if (operation.security == null)
                    operation.security = new List<IDictionary<string, IEnumerable<string>>>();

                var req = new Dictionary<string, IEnumerable<string>>
                {
                    { "Authorization", Enumerable.Empty<string>() } 
                };
                operation.security.Add(req);
            }
        }
    }
}