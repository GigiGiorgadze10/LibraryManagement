using Autofac;
using Autofac.Integration.WebApi;
using LibraryManagement.Application.Contracts.Infrastructure;
using LibraryManagement.Application.Mappings;
using LibraryManagement.Application.Services;
using LibraryManagement.Application.Services.Interfaces;
using LibraryManagement.Domain.Interfaces;
using LibraryManagement.Infrastructure.Identity;
using LibraryManagement.Infrastructure.Persistence;
using LibraryManagement.Infrastructure.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin; 
using Microsoft.Owin;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Configuration;
using System.Reflection;
using System.Web; 
using System.Web.Http;
using AutoMapper;
using FluentValidation;
using LibraryManagement.Api.Middleware;
using LibraryManagement.Api.Providers;
using System.Collections.Generic;

[assembly: OwinStartup(typeof(LibraryManagement.Api.Startup))]

namespace LibraryManagement.Api
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = GlobalConfiguration.Configuration;
            var builder = new ContainerBuilder();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<AppDbContext>().AsSelf().InstancePerRequest(); 

            builder.RegisterType<UserStore<AppUser>>().As<IUserStore<AppUser>>().InstancePerRequest();
            builder.RegisterType<RoleStore<IdentityRole>>().As<IRoleStore<IdentityRole, string>>().InstancePerRequest();

            builder.Register(ctx =>
            {
                IOwinContext owinContext = null;
                if (HttpContext.Current != null)
                {
                    owinContext = HttpContext.Current.GetOwinContext();
                }

                if (owinContext != null)
                {
                    return owinContext.GetUserManager<AppUserManager>();
                }

                throw new InvalidOperationException("Could not resolve AppUserManager: OWIN context not available for Autofac resolution. Ensure this is resolved within an HTTP request context.");
            }).As<AppUserManager>().InstancePerRequest(); 

            builder.Register(ctx =>
            {
                IOwinContext owinContext = null;
                if (HttpContext.Current != null) owinContext = HttpContext.Current.GetOwinContext();
                if (owinContext != null) return owinContext.GetUserManager<AppRoleManager>(); 
                throw new InvalidOperationException("Could not resolve AppRoleManager: OWIN context not available.");
            }).As<AppRoleManager>().InstancePerRequest();


            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerRequest();
            builder.RegisterType<AuthorService>().As<IAuthorService>().InstancePerRequest();
            builder.RegisterType<BookService>().As<IBookService>().InstancePerRequest();
            builder.RegisterType<GenreService>().As<IGenreService>().InstancePerRequest();
            builder.RegisterType<DateTimeProvider>().As<IDateTimeProvider>().SingleInstance();
            builder.RegisterType<JwtTokenGenerator>().As<IJwtTokenGenerator>().InstancePerRequest(); 

            builder.Register(ctx => AutoMapperConfig.Initialize(ctx.Resolve<IDateTimeProvider>())).As<IMapper>().SingleInstance();

            builder.RegisterAssemblyTypes(typeof(LibraryManagement.Application.Validation.AuthorCreateDtoValidator).Assembly)
                .Where(t => t.IsClosedTypeOf(typeof(IValidator<>)))
                .AsImplementedInterfaces()
                .InstancePerRequest();

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            app.CreatePerOwinContext(AppDbContext.Create);
            app.CreatePerOwinContext<AppUserManager>(AppUserManager.Create);
            app.CreatePerOwinContext<AppRoleManager>(AppRoleManager.Create);

            app.UseAutofacMiddleware(container);

            app.Use<LoggingMiddleware>();

            app.Use<ErrorHandlingMiddleware>();
            ConfigureOAuth(app, container); 
            app.UseWebApi(config);
        }

        public void ConfigureOAuth(IAppBuilder app, Autofac.IContainer container) 
        {
            var oAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/api/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(Convert.ToDouble(ConfigurationManager.AppSettings["jwt:DurationInMinutes"])),
                Provider = new CustomOAuthProvider(container), 
                AccessTokenFormat = new CustomJwtFormat(ConfigurationManager.AppSettings["jwt:Issuer"])
            };
            app.UseOAuthAuthorizationServer(oAuthServerOptions);

            var issuer = ConfigurationManager.AppSettings["jwt:Issuer"];
            var audience = ConfigurationManager.AppSettings["jwt:Audience"];
            var secret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["jwt:Secret"]);

            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
                    AllowedAudiences = new[] { audience },
                    IssuerSecurityKeyProviders = new IIssuerSecurityKeyProvider[]
                    {
                        new SymmetricKeyIssuerSecurityKeyProvider(issuer, secret)
                    }
                });
        }
    }
}