// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json.Serialization;

namespace Microsoft.SemanticKernel.Connectors.Google;

/// <summary>
/// Represents a file reference returned by Gemini services.
/// </summary>
public sealed class GoogleAIFile
{
    /// <summary>
    /// Identifier of the uploaded file.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Display name of the file.
    /// </summary>
    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }

    /// <summary>
    /// File mime type.
    /// </summary>
    [JsonPropertyName("mimeType")]
    public string? MimeType { get; set; }

    /// <summary>
    /// Size of the file in bytes.
    /// </summary>
    [JsonPropertyName("sizeBytes")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long SizeBytes { get; set; }
}
