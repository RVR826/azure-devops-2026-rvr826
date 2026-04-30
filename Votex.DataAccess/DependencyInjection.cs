using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Votex.DataAccess.Models;
using Votex.DataAccess.Services;

namespace Votex.DataAccess
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration config)
        {
            // Identity
            services.AddIdentity<User, IdentityRole<int>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;

                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<VotexDbContext>()
            .AddDefaultTokenProviders();

            // Database - configured in the OnConfiguring method
            services.AddDbContext<VotexDbContext>(options =>
            {
                //var connectionString = "Server=(localdb)\\MSSQLLocalDB;initial catalog=Votex;Trusted_Connection=True;MultipleActiveResultSets=True";
                var connectionString = config.GetConnectionString("DefaultConnection");

                options
                   .UseSqlServer(connectionString)
                   .UseLazyLoadingProxies();
            });

            // Services
            services.AddScoped<IVotingsService, VotingsService>();
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
