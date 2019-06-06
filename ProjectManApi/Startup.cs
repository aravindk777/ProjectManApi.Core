using System;
using System.IO;
using System.Linq;
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
using PM.Models.Config;
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
        /// <param name="_configuration">Configuration</param>
        /// <param name="environment">Environment</param>
        public Startup(IConfiguration _configuration, IHostingEnvironment environment)
        {
            //var builder = new ConfigurationBuilder()
            //    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            //    .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
            //    .AddEnvironmentVariables()
            //    .Build();

            //Configuration = builder;
            Configuration = _configuration;

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

            #region Dependency Injection

            // ---- Bind the Configuration to the AppSettings File
            services.Configure<ApplicationSettings>(options => Configuration.Bind(options));

            // ---- Db connections ----
            services.AddTransient<DbContext, PMDbContext>()

            // ---- Repositories ----
            .AddTransient<IUserRepository, UserRepository>()
            .AddTransient<IRepository<User>, Repository<User>>()
            .AddTransient<IProjectRepo, ProjectRepo>()
            .AddTransient<IRepository<Project>, Repository<Project>>()
            .AddTransient<ITaskRepository, TaskRepository>()
            .AddTransient<IRepository<Task>, Repository<Task>>()

            // ---- Service Providers ----
            .AddScoped<IUserLogic, UserLogic>()
            .AddScoped<IProjectLogic, ProjectLogic>()
            .AddScoped<ITaskLogic, TaskLogic>()

            // ---- Logging ----
            .AddLogging(log =>
            {
                log.AddConsole();
                log.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true, });
                var appConfig = Configuration.Get<ApplicationSettings>();
                SetupLogging(appConfig);
            })

            // ---- API Controllers ----
            .AddScoped<UsersController, UsersController>()
            .AddScoped<ProjectsController, ProjectsController>()
            .AddScoped<HealthController, HealthController>()
            .AddScoped<LogsController, LogsController>()
            .AddScoped<TasksController, TasksController>();
            #endregion

            #region DB context instantiation
            // Setup DB connection
            var dbConnString = Configuration.GetConnectionString("PMDb");
            services.AddDbContext<PMDbContext>(options => options.UseSqlServer(dbConnString));
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
            //loggerFactory.AddNLog(Configuration);

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
        void SetupLogging(ApplicationSettings applicationSettings)
        {
            // Initialize the Logger
            var nlogConfig = new LoggingConfiguration();

            applicationSettings.LogSettings.ToList()
                .ForEach(setting =>
                {
                    // Targets
                    var fileTarget = new FileTarget(setting.LoggerName)
                    {
                        ArchiveEvery = FileArchivePeriod.Day,
                        CreateDirs = true,
                        FileName = setting.LogFile,
                        Layout = setting.LogLayout,
                        ArchiveNumbering = ArchiveNumberingMode.Date,
                        Header = NLog.Layouts.Layout.FromString("_________________________________"),
                        Footer = NLog.Layouts.Layout.FromString("=================================")
                    };
                    nlogConfig.AddTarget(fileTarget);
                    nlogConfig.AddRuleForAllLevels(fileTarget);
                });

            // Setup the configuration
            NLog.LogManager.Configuration = nlogConfig;
        }
    }
}
