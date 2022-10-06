using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace WebDemo.Api.Options
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, s_createVersionInfo(description));

                //https://www.youtube.com/watch?v=IYWOWxw7dys&ab_channel=TechWithPat
                //https://www.infoworld.com/article/3650668/implement-authorization-for-swagger-in-aspnet-core-6.html
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath); //then add 1591 at errors and warning (propriete)             
            }

            //Security 
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Standard Authorization header using Bearer scheme {\"bearer {token}\")",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "oauth2"
            });
            options.OperationFilter<SecurityRequirementsOperationFilter>();

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                      Reference = new OpenApiReference
                      {
                          Type = ReferenceType.SecurityScheme,
                          Id = "oauth2"
                      }
                    },
                   new string[]{}

                }
            });
        }

        private static OpenApiInfo s_createVersionInfo(ApiVersionDescription description)
        {
            var info = new OpenApiInfo
            {
                Title = "WebDemo Api",
                Version = description.ApiVersion.ToString(),
                Description = "Web API for manage Users :3",
                 Contact = new OpenApiContact
                 {
                     Name = "Contact Me",
                     Url = new Uri("https://imakhlaf-youba.fr/contact")
                 },
            };

            if (description.IsDeprecated)
            {
                info.Description += " (deprecated)";
            }

            return info;
        }
    }
}
