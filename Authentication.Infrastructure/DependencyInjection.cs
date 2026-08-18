using Authentication.Application.Abstractions.Authentication;
using Authentication.Application.Abstractions.Persistence;
using Authentication.Infrastructure.Persistence.Repositories;
using Authentication.Infrastructure.Persistence;
using Authentication.Infrastructure.Authentication;
using Authentication.Infrastructure.Data;
using Authentication.Infrastructure.Configurations.Authentication;
using Microsoft.EntityFrameworkCore;
using Authentication.Infrastructure.Authentications;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;


namespace Authentication.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"));
        });

        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.SectionName));

        services.Configure<PasswordOptions>(
            configuration.GetSection(PasswordOptions.SectionName));


        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();

        services.AddScoped<IPasswordPolicy, PasswordPolicy>();

        services.AddScoped<IAuthenticationTokenService, JwtAuthenticationTokenService>();

        services.AddScoped<ICurrentUser, CurrentUser>();

        services.AddScoped<IRefreshTokenService, RefreshTokenService>();

        services.AddScoped<IRequestContext, RequestContext>();

        return services;
    }
}