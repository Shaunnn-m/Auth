using Authentication.Application.Abstractions.Persistence;
using Authentication.Infrastructure.Persistence.Repositories;
using Authentication.Infrastructure.Persistence;
using Authentication.Infrastructure.Data;
using Authentication.Infrastructure.Configurations.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Authentication.Infrastructure.HealthChecks;
using Authentication.Application.Persistence;
using Authentication.Application.Interfaces.Authentication;
using Authentication.Infrastructure.Services.Authentications;
using Authentication.Infrastructure.Configurations.Email;
using Authentication.Application.Interfaces.Common;
using Authentication.Infrastructure.Services.Email;
using Authentication.Infrastructure.Configurations;
using Authentication.Infrastructure.Services.Common;


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

        services.AddSingleton( new SqlServerHealthCheck(configuration));

        services.AddSingleton<EmailTemplateService>();

        services.AddHealthChecks()
            .AddCheck<SqlServerHealthCheck>("sql-server");

        services.AddOptions<JwtOptions>()
        .Bind(configuration.GetSection(JwtOptions.SectionName))
        .Validate(options =>
            !string.IsNullOrWhiteSpace(options.Key),
            "JWT signing key is required.")
        .Validate(options =>
            !string.IsNullOrWhiteSpace(options.Issuer),
            "JWT issuer is required.")
        .Validate(options =>
            !string.IsNullOrWhiteSpace(options.Audience),
            "JWT audience is required.")
        .ValidateOnStart();

        services.Configure<PasswordOptions>(
            configuration.GetSection(PasswordOptions.SectionName));

        services.Configure<SmtpOptions>(configuration.GetSection(
            SmtpOptions.SectionName));

        services.Configure<ApplicationOptions>(configuration.GetSection(
            ApplicationOptions.SectionName));

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();

        services.AddScoped<IPasswordPolicy, PasswordPolicy>();

        services.AddScoped<IAuthenticationTokenService, JwtAuthenticationTokenService>();

        services.AddScoped<ICurrentUser, CurrentUser>();

        services.AddScoped<IRefreshTokenService, RefreshTokenService>();

        services.AddScoped<IRequestContext, RequestContext>();

        services.AddScoped<IEmailConfirmationTokenRepository, EmailConfirmationTokenRepository>();

        services.AddScoped<IEmailService, SmtpEmailService>();

        services.AddScoped<ISecureTokenService, SecureTokenService>();

        services.AddScoped<
            IAuthenticationLinkService,
            AuthenticationLinkService>();


        return services;
    }
}