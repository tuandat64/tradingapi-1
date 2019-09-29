﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Examples;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Gateway.TradingApi.Web.Middleware
{
    public class Swagger
    {
        public static readonly string Version = "v0.8.03";
        public static readonly string Title = $"Bitcoin Suisse Trading API {Version}";

        // SWAGGER
        public class SwaggerTitleFilter : ISchemaFilter
        {
            public void Apply(Schema schema, SchemaFilterContext context)
            {
                var type = context.SystemType;
                schema.Extensions.Add("title", type.Name);
            }
        }

        public static void ConfigureServices(IServiceCollection services)
        {
            // Inject an implementation of ISwaggerProvider with defaulted settings applied
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    Version,
                    new Info
                    { Title = Title, Version = Version });

                c.OperationFilter<ExamplesOperationFilter>();

                //Locate the XML file being generated by ASP.NET...
                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.XML";
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                //... and tell Swagger to use those XML comments.
                //c.IncludeXmlComments(xmlPath);

                //... and ensure the models are decorated with "title"
                c.SchemaFilter<SwaggerTitleFilter>();
            });
        }

        public static void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();

            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{Version}/swagger.json", Title);
            });
        }
    }
}
