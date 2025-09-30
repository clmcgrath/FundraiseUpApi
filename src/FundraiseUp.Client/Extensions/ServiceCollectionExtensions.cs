using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using FundraiseUp.Client.Configuration;

namespace FundraiseUp.Client
{
    /// <summary>
    /// Extension methods for configuring FundraiseUp client services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds FundraiseUp client services to the specified service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configure">The configuration delegate.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddFundraiseUpClient(
            this IServiceCollection services,
            Action<FundraiseUpClientOptions> configure)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var options = new FundraiseUpClientOptions();
            configure(options);

            // Validate options during registration
            FundraiseUpClientOptionsValidator.Validate(options);

            // Register the options
            services.Configure(configure);

            // Register the client as singleton
            services.TryAddSingleton<IFundraiseUpClient>(serviceProvider =>
            {
                var logger = serviceProvider.GetService<ILogger<FundraiseUpClient>>();
                return new FundraiseUpClient(options, logger);
            });

            return services;
        }

        /// <summary>
        /// Adds FundraiseUp client services to the specified service collection with a simple API key.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="apiKey">The API key for authentication.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddFundraiseUpClient(
            this IServiceCollection services,
            string apiKey)
        {
            return services.AddFundraiseUpClient(options =>
            {
                options.ApiKey = apiKey;
            });
        }

        /// <summary>
        /// Adds FundraiseUp client services to the specified service collection with API key and base URL.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="apiKey">The API key for authentication.</param>
        /// <param name="baseUrl">The base URL for the FundraiseUp API.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddFundraiseUpClient(
            this IServiceCollection services,
            string apiKey,
            string baseUrl)
        {
            return services.AddFundraiseUpClient(options =>
            {
                options.ApiKey = apiKey;
                options.BaseUrl = baseUrl;
            });
        }
    }
}