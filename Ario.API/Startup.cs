/** 
 * Filename: Startup.cs
 * Description: Contains configuration details for Ario API including authentication
 *              redirection with Okta and database table contexts and scopes
 * 
 * Author: Jaxon Cramer, November 2017
 *
 * Copyright: The following source code is the sole property of Ario, Inc., all
 *              rights reserved. This code should not be copied or distributed
 *              without the express written consent of Ario, Inc.
 * 
 **/

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Ario.API.Contexts;
using Ario.API.Repositories;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Ario.API
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
            AddContexts(services);
            AddSingletons(services);
            AddScopes(services);

            // Authentication handled by Okta, a third party security platform
            // More information can be found at https://developer.okta.com/
            //services.AddAuthentication(sharedOptions =>
            //{
            //    sharedOptions.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            //})
            //.AddCookie()
            //.AddOpenIdConnect(options =>
            //{
            //    // Configuration pulled from appsettings.json by default:
            //    options.ClientId = Configuration["okta:ClientId"];
            //    options.ClientSecret = Configuration["okta:ClientSecret"];
            //    options.Authority = Configuration["okta:Authority"];
            //    options.CallbackPath = "/authorization-code/callback";
            //    options.ResponseType = "code";
            //    options.SaveTokens = true;
            //    options.UseTokenLifetime = false;
            //    options.GetClaimsFromUserInfoEndpoint = true;
            //    options.Scope.Add("openid");
            //    options.Scope.Add("profile");
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuer = true,
            //        RoleClaimType = ClaimTypes.Role
            //    };
            //});

            services.AddMvc();
        }

        private void AddScopes(IServiceCollection services) 
        {
            services.AddScoped<IRolesRepository, RolesRepository>();
            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<INodesRepository, NodesRepository>();
            services.AddScoped<ITeamsRepository, TeamsRepository>();
            services.AddScoped<IBusinessesRepository, BusinessesRepository>();
        }

        private void AddSingletons(IServiceCollection services)
        {
            services.TryAddSingleton<IRolesRepository, RolesRepository>();
            services.TryAddSingleton<IUsersRepository, UsersRepository>();
            services.TryAddSingleton<INodesRepository, NodesRepository>();
            services.TryAddSingleton<IBusinessesRepository, BusinessesRepository>();
            services.TryAddSingleton<ITeamsRepository, TeamsRepository>();
        }

        //TODO: add better security to Ario database
        //Connection string to internal Ario database defined in appsettings.json
        private void AddContexts(IServiceCollection services)
        {
            services.AddDbContext<TeamRolesContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<BusinessRolesContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<UsersContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<NodesContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<NodeComponentsContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<LabelsContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<QRAnchorComponentsContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<BusinessesContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<TeamsContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<BusinessUserJoinContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<NodeTeamJoinContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<UserTeamJoinContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<PDFComponentsContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
