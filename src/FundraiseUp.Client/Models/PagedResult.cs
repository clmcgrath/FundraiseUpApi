using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FundraiseUp.Client.Models
{
    /// <summary>
    /// Represents a cursor-based paginated collection as used by FundraiseUp API.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// Gets or sets the items in the current page.
        /// </summary>
        [JsonPropertyName("data")]
        public List<T> Items { get; set; } = new List<T>();

        /// <summary>
        /// Gets or sets the cursor for the next page of results.
        /// </summary>
        [JsonPropertyName("next_cursor")]
        public string? NextCursor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether there are more results available.
        /// </summary>
        [JsonPropertyName("has_more")]
        public bool HasMore { get; set; }

        // Legacy properties for backward compatibility
        /// <summary>
        /// Gets or sets the total number of items across all pages.
        /// Note: FundraiseUp API does not provide total count with cursor pagination.
        /// This property is maintained for backward compatibility but will always be 0.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets or sets the current page number.
        /// Note: FundraiseUp API uses cursor-based pagination, not page numbers.
        /// This property is maintained for backward compatibility but is not meaningful.
        /// </summary>
        public int CurrentPage { get; set; } = 1;

        /// <summary>
        /// Gets or sets the number of items per page.
        /// </summary>
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// Gets the total number of pages.
        /// Note: Not applicable with cursor-based pagination.
        /// </summary>
        public int TotalPages => HasMore ? int.MaxValue : CurrentPage;

        /// <summary>
        /// Gets a value indicating whether there is a next page.
        /// </summary>
        public bool HasNextPage => HasMore;

        /// <summary>
        /// Gets a value indicating whether there is a previous page.
        /// Note: Cursor-based pagination doesn't support going backward.
        /// </summary>
        public bool HasPreviousPage => false;
    }

    /// <summary>
    /// Generic cursor-based paginated response matching FundraiseUp API format.
    /// </summary>
    /// <typeparam name="T">The type of items in the data array.</typeparam>
    public class CursorPagedResponse<T>
    {
        /// <summary>
        /// Array of items in the current page.
        /// </summary>
        [JsonPropertyName("data")]
        public List<T> Data { get; set; } = new();

        /// <summary>
        /// Cursor for the next page of results.
        /// </summary>
        [JsonPropertyName("next_cursor")]
        public string? NextCursor { get; set; }

        /// <summary>
        /// Indicates if there are more results available.
        /// </summary>
        [JsonPropertyName("has_more")]
        public bool HasMore { get; set; }
    }
}
