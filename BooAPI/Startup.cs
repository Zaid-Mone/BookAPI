using BookAPI.Data;
using BookAPI.Validation;
using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BooAPI.Models;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BookAPI.Services;
using BookAPI.Utility;

namespace BooAPI
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
            services.AddValidatorsFromAssemblyContaining<BookValidator>();
            services.AddValidatorsFromAssemblyContaining<AuthorValidator>();

            // Register your validators
            services.AddScoped<IValidator<Book>, BookValidator>();
            services.AddScoped<IValidator<Author>, AuthorValidator>();

            services.AddMvc(option => option.EnableEndpointRouting = true)
            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
            // need Microsoft.AspNetCore.Mvc.NewtonsoftJson package
             .AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
            services.AddDbContext<DataDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<AppUser>
                 (options => 
                 { 
                     options.SignIn.RequireConfirmedAccount = true;
                     // Configure password validation rules
                     options.Password.RequireDigit = true; // Require at least one digit
                     options.Password.RequiredLength = 6; // Minimum length requirement
                     options.Password.RequireNonAlphanumeric = true; // Require at least one non-alphanumeric character
                     options.Password.RequireUppercase = true; // Require at least one uppercase letter
                     options.Password.RequireLowercase = true; // Require at least one lowercase letter
                 })
                 .AddRoles<IdentityRole>()
                 .AddEntityFrameworkStores<DataDbContext>();
            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                        .AddJwtBearer(options =>
                        {
                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuer = false,
                                ValidateAudience = false,
                                ValidateLifetime = true,
                                ValidateIssuerSigningKey = true,
                                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("secret_Key").Value)),
                                ClockSkew=TimeSpan.Zero

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

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
