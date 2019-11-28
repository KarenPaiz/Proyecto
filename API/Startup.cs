using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API
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
            services.Configure<ChatDatabaseSettings>(
       Configuration.GetSection(nameof(ChatDatabaseSettings)));

            services.AddSingleton<IChatDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<ChatDatabaseSettings>>().Value);

            services.AddSingleton<ChatDatabaseService>();

            services.AddControllers();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer=true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer="Chapi.com",
                    ValidAudience="Chapi.com",
                    IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes("M taecDaoynhbj letE2ui aaaan yo rcSm rnepo eiat rd"))
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
