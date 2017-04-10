using System.Net;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using Microsoft.IdentityModel.Tokens;
using LetMeKnowApi.Options;
using LetMeKnowApi.Data;
using LetMeKnowApi.Data.Abstract;
using LetMeKnowApi.Data.Repositories;
using LetMeKnowApi.ViewModels.Mappings;
using LetMeKnowApi.Core;

namespace LetMeKnowApi
{
    public partial class Startup
    {
        private static string _applicationPath = string.Empty;
        string sqlConnectionString = string.Empty;
        bool useInMemoryProvider = false;
        public IConfigurationRoot Configuration { get; }
        private const string SecretKey = "needtogetthisfromenvironment";
        private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
        public Startup(IHostingEnvironment env)
        {        
            _applicationPath = env.WebRootPath;
            // Setup configuration sources.

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // This reads the configuration keys from the secret store.
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            Configuration = builder.Build();
        }        

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {        
            string sqlConnectionString = Configuration.GetConnectionString("DefaultConnection");

            try
            {
                useInMemoryProvider = bool.Parse(Configuration["AppSettings:InMemoryProvider"]);
            }
            catch { }

            services.AddDbContext<LetMeKnowContext>(options => {
                switch (useInMemoryProvider)
                {
                    case true:
                        options.UseInMemoryDatabase();
                        break;
                    default:
                        options.UseSqlite(sqlConnectionString, b => b.MigrationsAssembly("LetMeKnowApi"));
                        break;
                }
            });
            

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IAreaRepository, AreaRepository>();
            services.AddScoped<ISuggestionRepository, SuggestionRepository>();            

            // Automapper Configuration
            AutoMapperConfiguration.Configure();

            /*jwt*/services.AddOptions(); 

            // Enable Cors
            services.AddCors();

            // Add MVC services to the services container.
            services.AddMvc(config => {
                    var policy = new AuthorizationPolicyBuilder()
                                    .RequireAuthenticatedUser()
                                    .Build();
                    config.Filters.Add(new AuthorizeFilter(policy));
                })
                .AddJsonOptions(opts => {
                    // Force Camel Case to JSON
                    opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });

            // Use policy auth.
            services.AddAuthorization(options =>
            {
                options.AddPolicy("DisneyUser",
                                policy => policy.RequireClaim("DisneyCharacter", "IAmMickey"));
            });

            // Get options from app settings
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {   
            //loggerFactory.AddConsole(LogLevel.Debug);
            //loggerFactory.AddDebug();

            ConfigureAuth(app);     

            app.UseStaticFiles();
            
            // Cors services
            app.UseCors(builder =>
                builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());

            // global exception handler
            app.UseExceptionHandler(
              builder =>
              {
                  builder.Run(
                    async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message).ConfigureAwait(false);
                        }
                    });
              });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                // Uncomment the following line to add a route for porting Web API 2 controllers.
                //routes.MapWebApiRoute("DefaultApi", "api/{controller}/{id?}");
            });

            LetMeKnowDbInitializer.Initialize(app.ApplicationServices);
        }
    }
}
