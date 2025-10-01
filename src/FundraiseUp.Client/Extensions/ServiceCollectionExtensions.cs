using System;
using System.Net.Http;
using FundraiseUp.Client.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FundraiseUp.Client
{
    /// <summary>
    /// Extension methods for configuring FundraiseUp client services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        private const string HttpClientName = "FundraiseUpApiClient";

        /// <summary>
        /// Adds FundraiseUp client services to the specified service collection with HttpClientFactory.
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

            // Register HttpClient with factory for proper lifecycle management
            services.AddHttpClient(HttpClientName, (serviceProvider, httpClient) =>
            {
                var clientOptions = serviceProvider.GetRequiredService<IOptions<FundraiseUpClientOptions>>().Value;

                httpClient.BaseAddress = new Uri(clientOptions.BaseUrl);
                httpClient.Timeout = clientOptions.Timeout;
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {clientOptions.ApiKey}");
                httpClient.DefaultRequestHeaders.Add("User-Agent", clientOptions.UserAgent);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                // Add any additional headers
                foreach (var header in clientOptions.AdditionalHeaders)
                {
                    httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            });

            // Register the client using HttpClientFactory
            services.TryAddTransient<IFundraiseUpClient>(serviceProvider =>
            {
                var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient(HttpClientName);
                var clientOptions = serviceProvider.GetRequiredService<IOptions<FundraiseUpClientOptions>>().Value;
                var logger = serviceProvider.GetService<ILogger<FundraiseUpClient>>();

                return new FundraiseUpClient(clientOptions.ApiKey, clientOptions, httpClient, logger);
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

        /// <summary>
        /// Adds FundraiseUp client services with advanced HttpClient configuration.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configure">The configuration delegate for FundraiseUp options.</param>
        /// <param name="configureHttpClient">Optional delegate to configure the HttpClient.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddFundraiseUpClient(
            this IServiceCollection services,
            Action<FundraiseUpClientOptions> configure,
            Action<HttpClient>? configureHttpClient)
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

            // Register HttpClient with factory and additional configuration
            var httpClientBuilder = services.AddHttpClient(HttpClientName, (serviceProvider, httpClient) =>
            {
                var clientOptions = serviceProvider.GetRequiredService<IOptions<FundraiseUpClientOptions>>().Value;

                httpClient.BaseAddress = new Uri(clientOptions.BaseUrl);
                httpClient.Timeout = clientOptions.Timeout;
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {clientOptions.ApiKey}");
                httpClient.DefaultRequestHeaders.Add("User-Agent", clientOptions.UserAgent);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                // Add any additional headers
                foreach (var header in clientOptions.AdditionalHeaders)
                {
                    httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }

                // Apply additional configuration if provided
                configureHttpClient?.Invoke(httpClient);
            });

            // Register the client using HttpClientFactory
            services.TryAddTransient<IFundraiseUpClient>(serviceProvider =>
            {
                var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient(HttpClientName);
                var clientOptions = serviceProvider.GetRequiredService<IOptions<FundraiseUpClientOptions>>().Value;
                var logger = serviceProvider.GetService<ILogger<FundraiseUpClient>>();

                return new FundraiseUpClient(clientOptions.ApiKey, clientOptions, httpClient, logger);
            });

            return services;
        }
    }
}
