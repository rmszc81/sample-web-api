﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SampleWebApi
{
    using Extensions;
    using Extensions.Formatters;

    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        private const string ExceptionRoute = "/error";
        private const string DatabaseName = "Values";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="env"></param>
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// 
        /// </summary>
        public IHostingEnvironment Env { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<Database.Context>(opt => opt.UseInMemoryDatabase(DatabaseName));

            services.AddMvc()
                    .ConfigureXml()
                    .ConfigureJson()
                    .ConfigureMessagePack()
                    .ConfigureProtobuf()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.ConfigureHsts();
            services.ConfigureHttpsRedirection(Env);
            services.ConfigureSwaggerServices();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
            {
                app.UseExceptionHandler(ExceptionRoute);
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseCookiePolicy();

            app.UseMvc();

            app.ConfigureExceptionHandler();
            app.ConfigureSwagger();
        }
    }
}
