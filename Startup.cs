﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using facdb.Models;
using System.Data.SqlClient;

namespace facdb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //var connection = @"Server=localhost;Database=facdb;Trusted_Connection=True;ConnectRetryCount=0";
            
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "localhost";
            builder.UserID = "sa";
            builder.Password = "M3d1T2018";
            builder.InitialCatalog = "facdb";
            
            services.AddDbContext<FacDBContext>(opt => opt.UseSqlServer(builder.ConnectionString));
            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseCors(
                options => options.WithOrigins("https://localhost").AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
            );
            app.UseMvc();
            app.UseDefaultFiles().UseStaticFiles();
        }
    }
}
