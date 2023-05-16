using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KP.Register.API.Helpers;
using KP.Register.API.Middleware;
using KP.Register.Business;
using KP.Register.IServices.Register;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;

namespace KP.Register.API
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
            // add on 
            //services.AddDbContext<ApplicationDbContext>(options =>
            //   options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            //// add on 
            //services.AddIdentity<ApplicationUser, IdentityRole>(o =>
            //{
            //    o.Password.RequireDigit = false;
            //    o.Password.RequiredLength = 0;
            //    o.Password.RequireLowercase = false;
            //    o.Password.RequireNonAlphanumeric = false;
            //    o.Password.RequireUppercase = false;

            //})
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //    .AddDefaultTokenProviders();

            services.AddMvc();

            // add on 
            var tokenValidationParameters = new TokenValidationParameters
            {
                //The signing key must match !
                ValidateIssuerSigningKey = false,
                //IssuerSigningKey = signingKey,

                //Validate the JWT Issuer (iss) claim
                ValidateIssuer = false,
                //ValidIssuer = issure,

                //validate the JWT Audience (aud) claim

                ValidateAudience = false,
                //ValidAudience = audience,

                //validate the token expiry
                ValidateLifetime = true,

                // If you  want to allow a certain amout of clock drift
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = tokenValidationParameters;
            });

            // Register no-op EmailSender used by account confirmation and password reset during development
            // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=532713
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new Info
            //    {
            //        Version = "v1",
            //        Title = "Document API",
            //        Description = "Document Description API",
            //        TermsOfService = "Terms Of Service"
            //        //Contact = new Contact() { Name = "Talking Dotnet", Email = "contact@talkingdotnet.com", Url = "www.talkingdotnet.com" }
            //    });
            //});
            services.AddSwaggerDocumentation();
            services.AddSingleton<IPathProvider, PathProvider>();
            services.AddSingleton<IRegister, RegisterBL>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //add on
               // app.UseSwaggerDocumentation();
                //app.UseBrowserLink();
                //app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
           // app.UseStaticFiles();
            app.UseAuthentication(); //needs to be up in the pipeline, before MVC
            //var jwtOptions = new TokenProviderOptions
            //{
            //    Audience = audience,
            //    Issuer = issure,
            //    SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            //};
            //app.UseJWTTokenProviderMiddleware(Options.Create(jwtOptions));

            app.UseMiddleware<VerifyTokenSenderMiddleWare>();
            app.UseMvc();
            app.UseSwaggerDocumentation();
            //app.UseSwagger();
            //app.UseSwaggerUI(c =>
            //{
            //    c.RoutePrefix = "help";
            //    c.SwaggerEndpoint("../swagger/v1/swagger.json", "My API V1");
            //});

        }
    }
}
