// <copyright file="JsonConfiguration.cs" company="FundraiseUpApi Team">
// Copyright (c) 2025 FundraiseUpApi Team. All rights reserved.
// </copyright>

namespace FundraiseUp.Client.Configuration
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    /// <summary>
    /// JSON serialization configuration for FundraiseUp API client.
    /// </summary>
    public static class JsonConfiguration
    {
        /// <summary>
        /// Gets default JSON serializer options for FundraiseUp API requests and responses.
        /// </summary>
        public static JsonSerializerOptions DefaultOptions { get; } = new JsonSerializerOptions
        {
            // Property naming
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower,

            // Null handling
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            IgnoreReadOnlyProperties = false,

            // Number handling
            NumberHandling = JsonNumberHandling.AllowReadingFromString,

            // String handling
            PropertyNameCaseInsensitive = true,

            // Error handling
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,

            // Performance
            WriteIndented = false,

            // Date handling
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower),
                new DateTimeOffsetConverter(),
                new DecimalConverter(),
            },
        };

        /// <summary>
        /// Gets JSON serializer options for development/debugging (with indentation).
        /// </summary>
        public static JsonSerializerOptions DebugOptions { get; } = new JsonSerializerOptions(DefaultOptions)
        {
            WriteIndented = true,
        };
    }

    /// <summary>
    /// Custom DateTimeOffset converter for consistent API date formatting.
    /// </summary>
    public class DateTimeOffsetConverter : JsonConverter<DateTimeOffset>
    {
        /// <summary>
        /// Reads DateTimeOffset from JSON.
        /// </summary>
        /// <param name="reader">The JSON reader.</param>
        /// <param name="typeToConvert">The type to convert.</param>
        /// <param name="options">Serializer options.</param>
        /// <returns>The parsed DateTimeOffset.</returns>
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTimeOffset.Parse(reader.GetString()!);
        }

        /// <summary>
        /// Writes DateTimeOffset to JSON.
        /// </summary>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="value">The DateTimeOffset value.</param>
        /// <param name="options">Serializer options.</param>
        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
        }
    }

    /// <summary>
    /// Custom decimal converter for precise monetary values.
    /// </summary>
    public class DecimalConverter : JsonConverter<decimal>
    {
        /// <summary>
        /// Reads decimal from JSON.
        /// </summary>
        /// <param name="reader">The JSON reader.</param>
        /// <param name="typeToConvert">The type to convert.</param>
        /// <param name="options">Serializer options.</param>
        /// <returns>The parsed decimal.</returns>
        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return decimal.Parse(reader.GetString()!);
            }

            return reader.GetDecimal();
        }

        /// <summary>
        /// Writes decimal to JSON.
        /// </summary>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="value">The decimal value.</param>
        /// <param name="options">Serializer options.</param>
        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
}
