using System;
using System.Collections.Generic;

namespace FundraiseUp.Client.Exceptions
{
    /// <summary>
    /// Base exception for all FundraiseUp client exceptions.
    /// </summary>
    public class FundraiseUpException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FundraiseUpException"/> class.
        /// </summary>
        public FundraiseUpException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FundraiseUpException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public FundraiseUpException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FundraiseUpException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public FundraiseUpException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception thrown when client configuration is invalid.
    /// </summary>
    public class FundraiseUpConfigurationException : FundraiseUpException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FundraiseUpConfigurationException"/> class.
        /// </summary>
        public FundraiseUpConfigurationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FundraiseUpConfigurationException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public FundraiseUpConfigurationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FundraiseUpConfigurationException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public FundraiseUpConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception thrown when API requests fail.
    /// </summary>
    public class FundraiseUpApiException : FundraiseUpException
    {
        /// <summary>
        /// Gets the HTTP status code returned by the API.
        /// </summary>
        public int StatusCode { get; }

        /// <summary>
        /// Gets the error code returned by the API.
        /// </summary>
        public string? ErrorCode { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FundraiseUpApiException"/> class.
        /// </summary>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="message">The exception message.</param>
        public FundraiseUpApiException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FundraiseUpApiException"/> class.
        /// </summary>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="errorCode">The API error code.</param>
        /// <param name="message">The exception message.</param>
        public FundraiseUpApiException(int statusCode, string? errorCode, string message) : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FundraiseUpApiException"/> class.
        /// </summary>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public FundraiseUpApiException(int statusCode, string message, Exception innerException) : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }

    /// <summary>
    /// Exception thrown when an API call returns validation errors (HTTP 422).
    /// </summary>
    public class FundraiseUpValidationException : FundraiseUpApiException
    {
        /// <summary>
        /// Gets the validation errors returned by the API.
        /// </summary>
        public Dictionary<string, string> ValidationErrors { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FundraiseUpValidationException"/> class.
        /// </summary>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="message">The error message.</param>
        /// <param name="validationErrors">The validation errors.</param>
        public FundraiseUpValidationException(int statusCode, string message, Dictionary<string, string> validationErrors)
            : base(statusCode, message)
        {
            ValidationErrors = validationErrors ?? new Dictionary<string, string>();
        }
    }
}