using ASC.Business;
using ASC.Business.Interface;
using ASC.DataAccess;
using ASC.DataAccess.Interfaces;
using AutoMapper;
using ElCamino.AspNetCore.Identity.AzureTable.Model;
using Lap1.Data;
using Lap1.Models;
using Lap1.Web.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NuGet.Protocol.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Web.ServiceHub;
using Webb.Data;
using Webb.Service;
using IHostingEnvironment = Microsoft.Extensions.Hosting.IHostingEnvironment;
namespace Webb
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(env.ContentRootPath)
                             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                             .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddSignalR(options =>
            {
                //options.EnableDetailedErrors = true;
            });
            services.AddControllersWithViews();
            services.AddMvc();
            services.AddOptions();
            //services.AddDistributedMemoryCache();
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = Configuration.GetSection("CacheSettings:CacheConnectionString").Value;
               
                options.InstanceName = Configuration.GetSection("CacheSettings:CacheInstance").Value;
            });
            services.AddSession();
          
            services.Configure<ApplicationSettings>(Configuration.GetSection("AppSettings"));
            services.AddIdentity<ApplicationUser, ApplicationRole>((options) =>
            {
                options.User.RequireUniqueEmail = true;
            })
            .AddAzureTableStores<ApplicationDbContext>(new Func<IdentityConfiguration>(() =>
            {
                IdentityConfiguration idconfig = new IdentityConfiguration();
                idconfig.TablePrefix = Configuration.GetSection("IdentityAzureTable:IdentutyConfiguration:TablePrefix").Value;
                idconfig.StorageConnectionString = Configuration.GetSection("IdentityAzureTable:IdentutyConfiguration:StorageConnectionString").Value;
                idconfig.LocationMode = Configuration.GetSection("IdentityAzureTable:IdentutyConfiguration:LocationMode").Value;
                return idconfig;
            }))
            .AddDefaultTokenProviders()
            .CreateAzureTablesIfNotExists<ApplicationDbContext>();
           
            services.AddSingleton<IIdentitySeed, IdentitySeed>();           
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUnitOfWork>(p => new UnitOfWork(Configuration.GetSection("ConnectionStrings:DefaultConnection").Value));
            services.AddMvc().AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.PropertyNamingPolicy = null;
                o.JsonSerializerOptions.DictionaryKeyPolicy = null;
            });
            services.AddSingleton<IEmailSender, AuthMessageSender>();
            services.AddScoped<IMasterDataOperations, MasterDataOperations>();
            services.AddAutoMapper();
            services.AddAuthentication()
              .AddGoogle(options =>
              {
                  IConfigurationSection googleAuthenNSection =
                  Configuration.GetSection("Authentication:Google");
                  options.ClientId = Configuration["Google:Identity:ClientId"];
                  options.ClientSecret = Configuration["Google:Identity:ClientSecret"];
              });
            // Add support to embedded views from ASC.Utilities project.
            var assembly = typeof(ASC.Utilities.Navigation.LeftNavigationViewComponent).GetTypeInfo().Assembly;
            var embeddedFileProvider = new EmbeddedFileProvider(assembly, "ASC.Utilities");
            services.AddControllersWithViews().AddRazorRuntimeCompilation(options =>
            {
                options.FileProviders.Add(embeddedFileProvider);
            });
            services.AddScoped<IMasterDataCacheOperations, MasterDataCacheOperations>();
            services.AddSingleton<INavigationCacheOperations, NavigationCacheOperations>();
            services.AddScoped<IServiceRequestOperations, ServiceRequestOperations>();
            
            services.AddScoped<IServiceRequestMessageOperations, ServiceRequestMessageOperations>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app,
            IWebHostEnvironment env,
            IIdentitySeed storageSeed,
            IMasterDataCacheOperations masterDataCacheOperations,
            INavigationCacheOperations navigationCacheOperations,
            IUnitOfWork unitOfWork)
        {
            app.UseCors(builder => builder.WithOrigins("https://localhost:44374").AllowAnyHeader().AllowAnyMethod().AllowCredentials());
            
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();
            app.UseSession();
            app.UseAuthentication();
            
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ServiceMessagesHub>("/chatHub");
                endpoints.MapControllerRoute(
                    name: "areaRoute",
                   pattern: "{area:exists}/{controller=Home}/{action=Index}"
                   );
                endpoints.MapControllerRoute(
                   name: "default",
                   pattern: "{controller=Home}/{action=Index}/{id?}");
                

                endpoints.MapRazorPages();
                
            });
            
            using (var scope = app.ApplicationServices.CreateScope())
            {
                await storageSeed.Seed(scope.ServiceProvider.GetService<UserManager<ApplicationUser>>(),
                                        scope.ServiceProvider.GetService<RoleManager<ApplicationRole>>(),
                                         scope.ServiceProvider.GetService<IOptions<ApplicationSettings>>());
            }
            var models = Assembly.Load(new AssemblyName("ASC.Models")).GetTypes().Where(type => type.Namespace == "ASC.Models.Models");
            foreach(var model in models)
            {
                var repositoryInstance = Activator.CreateInstance(typeof(Respository<>).MakeGenericType(model), unitOfWork);
                MethodInfo method = typeof(Respository<>).MakeGenericType(model).GetMethod("CreateTableAsync");
                method.Invoke(repositoryInstance, new object[0]);

            }
            await masterDataCacheOperations.CreateMasterDataCacheAsync();
            await navigationCacheOperations.CreateNavigationCacheAsync();
        }
    }
}
