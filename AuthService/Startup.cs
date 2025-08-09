using Domain.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tasks;
using Tasks.contract;
using infrastructure;
using infrastructure.Repositories;
using Domain.Interfaces;
using Domain.Models;
using Domain.Models.Enteties;
using Microsoft.EntityFrameworkCore;
using Unity;
using AuthService.Tools;
using Domain.config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Hosting;
using infrastructure.Services;
using StackExchange.Redis;
using infrastructure.Helpers;

namespace AuthService
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

            var result = Configuration;
            services.AddCors();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IMorceauTask, MorceauTask>();
            services.AddScoped<IMorceauRepository, MorceauRepository>();
            services.AddScoped<IMorceauPlaylistRepository, MorceauPlaylistRepository>();
            services.AddScoped<IApiCloudRepository, ApiCloudRepository>();
            services.AddScoped<IPlaylistRepository, PlaylistRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISettingRepository, SettingRepository>();
            services.AddScoped<IkeyvalidationRepository, keyvalidationRepository>();
            services.AddScoped<EmailService>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
             services.AddDbContext<DbContextClass>(option =>
            {
                option.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddMvc(options => options.EnableEndpointRouting = false);
            var container = new UnityContainer();
            
            var emailService = new EmailService(Configuration);
            var logDirectory = Configuration["Logging:Paths:Default"];

            LogService.ConfigureLogger(logDirectory, emailService);
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var redisConnectionString = Configuration.GetConnectionString("Redis");
                return ConnectionMultiplexer.Connect(redisConnectionString);
            });
            services.AddSingleton<IUnityContainer>(container);
            services.AddControllers();
            services.Configure<JwtConf>(Configuration.GetSection("JwtConfig"));
            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                    .AddJwtBearer(jwt=>{
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
                app.UseHsts(); // Sécurise les requêtes
            }
            _myCtxDB.Database.EnsureCreated();
            //app.UseHttpsRedirection();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "api/{controller=Home}/{action=Index}/{id?}");
            });
            app.UseAuthentication();
        }
        public void ConfigureContainer(IUnityContainer container)
        {
            IoCContainer.Current = container; // Assign the container to IoCContainer
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
