using System.Collections.Generic;

namespace FundraiseUp.Client.Models
{
    /// <summary>
    /// Represents a validation error response from the API.
    /// </summary>
    internal class ValidationErrorResponse
    {
        /// <summary>
        /// Gets or sets the list of validation errors.
        /// </summary>
        public List<ValidationError> Errors { get; set; } = new List<ValidationError>();
    }

    /// <summary>
    /// Represents a single validation error.
    /// </summary>
    internal class ValidationError
    {
        /// <summary>
        /// Gets or sets the field name that has the validation error.
        /// </summary>
        public string Field { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the validation error message.
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}
