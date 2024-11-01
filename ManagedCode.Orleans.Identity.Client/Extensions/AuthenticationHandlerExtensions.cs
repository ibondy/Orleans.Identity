using System;
using ManagedCode.Orleans.Identity.Client.Middlewares;
using ManagedCode.Orleans.Identity.Core.Constants;
using ManagedCode.Orleans.Identity.Core.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace ManagedCode.Orleans.Identity.Client.Extensions;

public static class AuthenticationHandlerExtensions
{
    /// <summary>
    /// Use Orleans.Identity authentication as default authentication scheme
    /// </summary>
    /// <param name="services"></param>
    /// <param name="sessionOption">Options for working with session</param>
    public static void AddOrleansIdentity(this IServiceCollection services, Action<SessionOption> sessionOption)
    {
        var option = new SessionOption();
        sessionOption?.Invoke(option);
        AddOrleansIdentity(services, option);
    }

    /// <summary>
    /// Use Orleans.Identity authentication as default authentication scheme
    /// </summary>
    /// <param name="services"></param>
    /// <param name="sessionOption">Options for working with session</param>
    public static void AddOrleansIdentity(this IServiceCollection services, SessionOption? sessionOption = null, Action<AuthorizationPolicyBuilder>? authenticationBuilder = null)

    {
        sessionOption ??= new SessionOption();
        
        //services.AddScoped<SessionAuthorizationHandler>();     
        //services.AddScoped<SessionAuthenticationHandler>();      

        services.AddAuthentication(options => options.DefaultScheme = OrleansIdentityConstants.AUTHENTICATION_TYPE)
            .AddScheme<AuthenticationSchemeOptions, SessionAuthenticationHandler>(OrleansIdentityConstants.AUTHENTICATION_TYPE, op => { });

        services.AddAuthorization(options =>
        {
            var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(OrleansIdentityConstants.AUTHENTICATION_TYPE);
            defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
            authenticationBuilder?.Invoke(defaultAuthorizationPolicyBuilder);
            options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
        });
    }
}