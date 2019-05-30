using System;
using System.IO;
using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using Swashbuckle.AspNetCore.Swagger;

namespace SampleWebApi.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class Swagger
    {
        private const string SwaggerDocVersion = "v1";
        private const string SwaggerDocTitle = "API title";
        private const string SwaggerDocDescription = "API description";
        private const string SwaggerDocTermsOfService = "API terms of service";

        private const string SwaggerDocContactName = "Contact Name";
        private const string SwaggerDocContactEmail = "Contact E-mail";
        private const string SwaggerDocContactUrl = "Contact Url";

        private const string XmlDocsExtension = "xml";
        private const string XmlDocsPath = @"docs\";
        private const string SwaggerEndpointUrl = "/swagger/v1/swagger.json";
        private const string SwaggerEndpointDescription = "My API V1";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureSwaggerServices(this IServiceCollection services)
        {
            services.AddSwaggerGen(action =>
            {
                action.SwaggerDoc(SwaggerDocVersion, new Info
                {
                    Version = SwaggerDocVersion,
                    Title = SwaggerDocTitle,
                    Description = SwaggerDocDescription,
                    TermsOfService = SwaggerDocTermsOfService,
                    Contact = new Contact
                    {
                        Name = SwaggerDocContactName,
                        Email = SwaggerDocContactEmail,
                        Url = SwaggerDocContactUrl
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.{XmlDocsExtension}";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, XmlDocsPath, xmlFile);

                action.IncludeXmlComments(xmlPath);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        public static void ConfigureSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(action =>
            {
                action.SwaggerEndpoint(SwaggerEndpointUrl, SwaggerEndpointDescription);
            });
        }
    }
}
