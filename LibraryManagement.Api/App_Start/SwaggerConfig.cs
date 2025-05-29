using System.Web.Http;
using WebActivatorEx; 
using LibraryManagement.Api; 
using Swashbuckle.Application; 

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace LibraryManagement.Api
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "Library Management API");
                    c.PrettyPrint();

                    c.ApiKey("Authorization")
                        .Description("JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"")
                        .Name("Authorization")
                        .In("header");
                    c.OperationFilter<AssignJwtSecurityRequirements>(); 
                })
                .EnableSwaggerUi(c =>
                {
                    c.EnableApiKeySupport("Authorization", "header");
                });
        }
    }
}