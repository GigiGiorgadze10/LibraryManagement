// src/LibraryManagementSystem.Api/App_Start/SwaggerConfig.cs
using System.Web.Http;
using WebActivatorEx; // NuGet: WebActivatorEx
using LibraryManagement.Api; // For AssignJwtSecurityRequirements
using Swashbuckle.Application; // NuGet: Swashbuckle.Core

// Fully qualify PreApplicationStartMethod to avoid ambiguity
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
                    c.PrettyPrint(); // Makes the JSON output more readable

                    // Enable JWT token input in Swagger UI
                    c.ApiKey("Authorization")
                        .Description("JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"")
                        .Name("Authorization")
                        .In("header");
                    c.OperationFilter<AssignJwtSecurityRequirements>(); // Custom filter to apply security
                })
                .EnableSwaggerUi(c =>
                {
                    c.EnableApiKeySupport("Authorization", "header");
                });
        }
    }
}