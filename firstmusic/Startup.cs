using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using infrastructure;
using infrastructure.Repositories;
using Domain.Interfaces;
using Domain.Models.Enteties;
using Tasks.contract;
using Tasks;
using Unity;
using firstmusic.Tools;
using Unity.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Hosting;
using infrastructure.Services;
using Serilog;
using StackExchange.Redis;
using System.IO;
using infrastructure.Helpers;

namespace firstmusic
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ConfigurationHelper.Initialize(configuration);
        }
        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<CancellationService>();
           
            // ConfigInfrastructure x = new ConfigInfrastructure(Configuration);
            var result = Configuration;
            services.AddCors();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IMorceauRepository, MorceauRepository>();
            services.AddScoped<IMorceauPlaylistRepository, MorceauPlaylistRepository>();
            services.AddScoped<IApiCloudRepository, ApiCloudRepository>();
            services.AddScoped<IPlaylistRepository, PlaylistRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISettingRepository, SettingRepository>();
            services.AddScoped<IkeyvalidationRepository, keyvalidationRepository>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddSingleton<ConfigInfrastructure, ConfigInfrastructure>(serviceProvider => new ConfigInfrastructure(Configuration));
            services.AddScoped<EmailService>();
            services.AddDbContext<DbContextClass>(option =>
            {
                option.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
            }, ServiceLifetime.Scoped);
            services.AddMvc(options => options.EnableEndpointRouting = false);

            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                 .AddJwtBearer(jwt => {
                     var Key = Encoding.ASCII.GetBytes(Configuration["JwtConfig:secret"]);
                     jwt.SaveToken = true;
                     jwt.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateIssuerSigningKey = true,
                         IssuerSigningKey = new SymmetricSecurityKey(Key),
                         ValidateIssuer = false,
                         ValidateAudience = false,
                         ValidateLifetime = true,
                         ClockSkew = TimeSpan.Zero
                     };
                 });
            
                var emailService = new EmailService(Configuration);
                var logDirectory = Configuration["Logging:Paths:Default"];

                LogService.ConfigureLogger(logDirectory, emailService);
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var redisConnectionString = Configuration.GetConnectionString("Redis");
                return ConnectionMultiplexer.Connect(redisConnectionString);
            });
            var container = new UnityContainer();
            // Register your types with the Unity container here
            // container.RegisterType<IMyService, MyService>();
            services.AddSingleton<IUnityContainer>(container);
            services.AddAuthorization();
            services.AddControllers();
            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DbContextClass _myCtxDB)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts(); 
            }

            // Middleware pour le routage
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            _myCtxDB.Database.EnsureCreated();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "api/{controller=Home}/{action=Index}/{id?}");
            });
        }

        public void ConfigureContainer(IUnityContainer container)
        {
            IoCContainer.Current = container; // Assign the container to IoCContainer
            container.RegisterType<IMorceauTask, MorceauTask>();
            container.RegisterType<IUserTask, UserTask>();
            container.RegisterType<IPlaylistTask, PlaylistTask>();
            container.RegisterType<IkeyvalidationTask, keyvalidationTask>();
            container.RegisterType<IMorceauPlaylistTask, MorceauPlaylistTask>();
            container.RegisterType<ISettingTask, SettingTask>();
            container.RegisterType<ApiCloudTask>();

            container.RegisterType<EmailService>();
        }
    }
    public static class ConfigurationHelper
    {
        public static IConfiguration config;
        public static void Initialize(IConfiguration Configuration)
        {
            config = Configuration;
        }
    }
   }
