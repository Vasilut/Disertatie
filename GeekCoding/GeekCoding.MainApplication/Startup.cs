using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeekCoding.Common.EmailGenerator;
using GeekCoding.Data.Models;
using GeekCoding.Repository;
using GeekCoding.Repository.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR;
using GeekCoding.MainApplication.Hubs;

namespace GeekCoding.MainApplication
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
            services.AddSingleton(Configuration);
            var hangfireConnectionString = Configuration.GetConnectionString("HangfireDatabase");
            services.AddHangfire(configuration =>
            {
                configuration.UseSqlServerStorage(hangfireConnectionString);
            });
            services.AddMvc();

            var connectionString = Configuration.GetConnectionString("EvaluatorDatabase");
            services.AddDbContext<EvaluatorContext>(option => option.UseSqlServer(connectionString));
            services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<EvaluatorContext>().AddDefaultTokenProviders();
            services.AddScoped<IProblemRepository, ProblemRepository>();
            services.AddScoped<IMessageBuilder, EmailBuilder>();
            services.AddScoped<ISolutionRepository, SolutionRepository>();
            services.AddScoped<ISubmisionRepository, SubmisionRepository>();
            services.AddScoped<IProgressStatusRepository, ProgresStatusRepository>();
            services.AddTransient<SubmissionHub>();

            services.AddSignalR();
        }

       
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHangfireServer();
            app.UseHangfireDashboard();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseAuthentication();
            app.UseStaticFiles();

            app.UseSignalR(route =>
            {
                route.MapHub<SubmissionHub>("/hubs/submission");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            
        }
    }
}
