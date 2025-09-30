using System;
using System.ComponentModel.DataAnnotations;
using FundraiseUp.Client.Exceptions;

namespace FundraiseUp.Client.Configuration
{
    /// <summary>
    /// Configuration options for the FundraiseUp client.
    /// </summary>
    public class FundraiseUpClientOptions
    {
        /// <summary>
        /// Gets or sets the API key for authenticating with the FundraiseUp API.
        /// </summary>
        [Required]
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the base URL for the FundraiseUp API.
        /// </summary>
        [Required]
        [Url]
        public string BaseUrl { get; set; } = "https://api.fundraiseup.com";

        /// <summary>
        /// Gets or sets the timeout for HTTP requests.
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Gets or sets the maximum number of retry attempts for failed requests.
        /// </summary>
        [Range(0, 10)]
        public int MaxRetryAttempts { get; set; } = 3;

        /// <summary>
        /// Gets or sets the delay between retry attempts.
        /// </summary>
        public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);

        /// <summary>
        /// Gets or sets a value indicating whether to enable logging.
        /// </summary>
        public bool EnableLogging { get; set; } = true;

        /// <summary>
        /// Gets or sets the minimum log level.
        /// </summary>
        public LogLevel LogLevel { get; set; } = LogLevel.Information;

        /// <summary>
        /// Gets or sets the user agent string for HTTP requests.
        /// </summary>
        public string UserAgent { get; set; } = "FundraiseUp-DotNet-Client/1.0.0";

        /// <summary>
        /// Gets or sets additional headers to include with requests.
        /// </summary>
        public System.Collections.Generic.Dictionary<string, string> AdditionalHeaders { get; set; } =
            new System.Collections.Generic.Dictionary<string, string>();
    }

    /// <summary>
    /// Log levels for the FundraiseUp client.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Trace level logging.
        /// </summary>
        Trace,

        /// <summary>
        /// Debug level logging.
        /// </summary>
        Debug,

        /// <summary>
        /// Information level logging.
        /// </summary>
        Information,

        /// <summary>
        /// Warning level logging.
        /// </summary>
        Warning,

        /// <summary>
        /// Error level logging.
        /// </summary>
        Error,

        /// <summary>
        /// Critical level logging.
        /// </summary>
        Critical
    }

    /// <summary>
    /// Validator for FundraiseUp client options.
    /// </summary>
    public static class FundraiseUpClientOptionsValidator
    {
        /// <summary>
        /// Validates the provided options and throws an exception if invalid.
        /// </summary>
        /// <param name="options">The options to validate.</param>
        /// <exception cref="FundraiseUpConfigurationException">Thrown when options are invalid.</exception>
        public static void Validate(FundraiseUpClientOptions options)
        {
            if (options == null)
            {
                throw new FundraiseUpConfigurationException("Options cannot be null");
            }

            if (string.IsNullOrWhiteSpace(options.ApiKey))
            {
                throw new FundraiseUpConfigurationException("API key is required and cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(options.BaseUrl))
            {
                throw new FundraiseUpConfigurationException("Base URL is required and cannot be empty");
            }

            if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var uri) ||
                (uri.Scheme != "http" && uri.Scheme != "https"))
            {
                throw new FundraiseUpConfigurationException("Base URL must be a valid HTTP or HTTPS URL");
            }

            if (options.Timeout <= TimeSpan.Zero)
            {
                throw new FundraiseUpConfigurationException("Timeout must be greater than zero");
            }

            if (options.MaxRetryAttempts < 0 || options.MaxRetryAttempts > 10)
            {
                throw new FundraiseUpConfigurationException("Max retry attempts must be between 0 and 10");
            }

            if (options.RetryDelay < TimeSpan.Zero)
            {
                throw new FundraiseUpConfigurationException("Retry delay cannot be negative");
            }
        }
    }
}