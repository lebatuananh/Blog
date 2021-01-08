﻿using Meowv.Blog.Api.Filters;
using Meowv.Blog.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Linq;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.ExceptionHandling;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Meowv.Blog.Api
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(AbpAspNetCoreSerilogModule),
        typeof(AbpAspNetCoreMvcModule),
        typeof(MeowvBlogApplicationModule),
        typeof(MeowvBlogMongoDbModule)
    )]
    public class MeowvBlogApiModule : AbpModule
    {
        public SwaggerOptions SwaggerOptions { get; set; }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            SwaggerOptions = context.Services.ExecutePreConfiguredActions<SwaggerOptions>();

            ConfigureExceptionFilter();
            ConfigureAutoApiControllers();
            ConfigureRouting(context.Services);
            ConfigureSwaggerServices(context.Services);
        }

        private void ConfigureExceptionFilter()
        {
            Configure<MvcOptions>(options =>
            {
                var filterMetadata = options.Filters.FirstOrDefault(x => x is ServiceFilterAttribute attribute && attribute.ServiceType.Equals(typeof(AbpExceptionFilter)));
                options.Filters.Remove(filterMetadata);
                options.Filters.Add(typeof(MeowvBlogExceptionFilter));
            });
        }

        private void ConfigureAutoApiControllers()
        {
            Configure<AbpAspNetCoreMvcOptions>(options =>
            {
                options.ConventionalControllers.Create(typeof(MeowvBlogApplicationModule).Assembly, opts => { opts.RootPath = "meowv"; });
            });
        }

        private static void ConfigureRouting(IServiceCollection services)
        {
            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
                options.AppendTrailingSlash = true;
            });
        }

        private void ConfigureSwaggerServices(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(SwaggerOptions.Name, new OpenApiInfo
                {
                    Title = SwaggerOptions.Title,
                    Version = SwaggerOptions.Version,
                    Description = SwaggerOptions.Description
                });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
                options.DocumentFilter<SwaggerDocumentFilter>();
            });
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint($"/swagger/{SwaggerOptions.Name}/swagger.json", SwaggerOptions.Title);
                options.DefaultModelsExpandDepth(-1);
                options.DocExpansion(DocExpansion.List);
                options.RoutePrefix = SwaggerOptions.RoutePrefix;
                options.DocumentTitle = SwaggerOptions.DocumentTitle;
            });
            app.UseAbpSerilogEnrichers();
            app.UseConfiguredEndpoints();
        }
    }
}