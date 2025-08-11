// Copyright (c) Microsoft. All rights reserved.

using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Microsoft.SemanticKernel.Connectors.Google.Core;

/// <summary>
/// Client for Google Gemini file upload operations.
/// </summary>
internal sealed class GoogleAIFileClient : ClientBase
{
    private readonly Uri _fileUploadEndpoint;

    /// <summary>
    /// Initializes a new instance for GoogleAI.
    /// </summary>
    /// <param name="httpClient">HTTP client.</param>
    /// <param name="apiKey">GoogleAI API key.</param>
    /// <param name="apiVersion">Version of the Google API.</param>
    /// <param name="logger">Logger.</param>
    public GoogleAIFileClient(HttpClient httpClient, string apiKey, GoogleAIVersion apiVersion, ILogger? logger = null)
        : base(httpClient: httpClient, logger: logger, apiKey: apiKey)
    {
        Verify.NotNullOrWhiteSpace(apiKey);
        string versionSubLink = GetApiVersionSubLink(apiVersion);
        this._fileUploadEndpoint = new Uri($"https://generativelanguage.googleapis.com/upload/{versionSubLink}/files?uploadType=multipart");
    }

    /// <summary>
    /// Uploads a file to the Gemini API.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="mimeType">Mime type of the file.</param>
    /// <param name="content">Stream with file content.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<GoogleAIFile> UploadFileAsync(string fileName, string mimeType, Stream content, CancellationToken cancellationToken = default)
    {
        Verify.NotNullOrWhiteSpace(fileName);
        Verify.NotNullOrWhiteSpace(mimeType);
        Verify.NotNull(content);

        using var request = await this.CreateHttpRequestAsync(null!, this._fileUploadEndpoint).ConfigureAwait(false);

        var metadata = new
        {
            file = new { displayName = fileName, mimeType }
        };

        using var metadataContent = new StringContent(JsonSerializer.Serialize(metadata), Encoding.UTF8, "application/json");
        using var fileContent = new StreamContent(content);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(mimeType);

        var multipartContent = new MultipartFormDataContent
        {
            { metadataContent, "metadata" },
            { fileContent, "file", fileName }
        };

        request.Content = multipartContent;

        string body = await this.SendRequestAndGetStringBodyAsync(request, cancellationToken).ConfigureAwait(false);
        return DeserializeResponse<GoogleAIFile>(body);
    }
}
