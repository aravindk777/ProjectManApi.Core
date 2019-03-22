using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using PM.Api.Controllers;
using PM.BL.Projects;
using PM.BL.Tasks;
using PM.BL.Users;
using PM.Data.Entities;
using PM.Data.Repos;
using PM.Data.Repos.Projects;
using PM.Data.Repos.Tasks;
using PM.Data.Repos.Users;
using PM.Models.DataModels;
using Swashbuckle.AspNetCore.Swagger;

namespace PM.Api
{
    /// <summary>
    /// Startup object class
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Constructor with DI
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;            
        }

        /// <summary>
        /// Configuration object
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">Servicecollection object</param>
        public void ConfigureServices(IServiceCollection services)
        {
            #region Cors
            // Enable Cors
            services.AddCors(feature => {
                feature.AddPolicy(
                    "ProjectManagerApiCors",
                    builder => builder
                                    .SetIsOriginAllowed((host) => true)
                                    .AllowAnyHeader()
                                    .AllowAnyMethod()
                                    .AllowAnyOrigin()
                                    .AllowCredentials()
                                );
            });
            #endregion

            #region Swagger and documentation
            // Documentation
            var documentationXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var XmlFilePath = Path.Combine(AppContext.BaseDirectory, documentationXmlFile);
            // Add Swagger documentation
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Project Manager API Documentation", Version = "v1", Description = "Lists all the operations for the Project Manager api" });
                c.IncludeXmlComments(XmlFilePath);
            });
            #endregion

            #region Enbale Mvc
            // Enable Mvc
            services.AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());
            services.AddMvcCore().AddApiExplorer();
            #endregion

            #region DB context instantiation
            // Setup DB connection
            services.AddDbContext<PMDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("PMDb")));
            #endregion  

            #region Dependency Injection
            // ---- Db connections ----
            services.AddScoped<PMDbContext, PMDbContext>()

                // ---- Repositories ----
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IRepository<User>, Repository<User>>()
                .AddScoped<IProjectRepo, ProjectRepo>()
                .AddScoped<IRepository<Project>, Repository<Project>>()
                .AddScoped<ITaskRepository, TaskRepository>()
                .AddScoped<IRepository<Task>, Repository<Task>>()

                // ---- Service Providers ----
                .AddScoped<IUserLogic, UserLogic>()
                .AddScoped<IProjectLogic, ProjectLogic>()
                .AddScoped<ITaskLogic, TaskLogic>()

                // ---- Logging ----
                .AddLogging(log =>
                {
                    log.AddConsole();
                    log.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
                    SetupLogging();
                })

                // ---- API Controllers ----
                .AddScoped<UsersController, UsersController>()
                .AddScoped<ProjectsController, ProjectsController>()
                .AddScoped<HealthController, HealthController>()
                .AddScoped<LogsController, LogsController>()
                .AddScoped<TasksController, TasksController>();
            #endregion
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">app builder</param>
        /// <param name="env">environment configuration</param>
        /// <param name="loggerFactory">logging</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseStatusCodePages();

            //app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(c => 
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Project Manager API");
                c.RoutePrefix = string.Empty;
            });
            app.UseCors("ProjectManagerApiCors");
            app.UseAuthentication();
            app.UseMvc();
        }

        /// <summary>
        /// Logging configuration
        /// </summary>
        void SetupLogging()
        {
            // Initialize the Logger
            var nlogConfig = new LoggingConfiguration();

            // Targets
            var fileTarget = new FileTarget("FileTarget")
            {
                //ArchiveAboveSize = 1024 * 1024,
                ArchiveEvery = FileArchivePeriod.Day,
                CreateDirs = true,
                FileName = @"c:\Logs\PMApi\PMApi.Core.log",
                Layout = @"${date:format=yyyy-MM-dd-hh\:mm\:ss} ${level} ${message}  ${exception:format=tostring}    ${exception:format=stackTrace}    ${exception:format=InnerException}",
                ArchiveNumbering = ArchiveNumberingMode.Date,
                Header = NLog.Layouts.Layout.FromString("_________________________________"),
                Footer = NLog.Layouts.Layout.FromString("=================================")
            };
            nlogConfig.AddTarget(fileTarget);
            nlogConfig.AddRuleForAllLevels(fileTarget);

            //configuration.AddTarget(fileTarget);
            //configuration.AddRuleForAllLevels(fileTarget);

            // Setup the configuration
            NLog.LogManager.Configuration = nlogConfig;
        }
    }
}
