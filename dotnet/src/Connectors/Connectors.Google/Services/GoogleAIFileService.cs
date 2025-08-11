// Copyright (c) Microsoft. All rights reserved.

using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Google.Core;
using Microsoft.SemanticKernel.Http;

namespace Microsoft.SemanticKernel.Connectors.Google;

/// <summary>
/// Service for uploading files to Google AI Gemini endpoints.
/// </summary>
public sealed class GoogleAIFileService
{
    private readonly GoogleAIFileClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="GoogleAIFileService"/> class.
    /// </summary>
    /// <param name="apiKey">API key used for authentication.</param>
    /// <param name="apiVersion">Version of Google AI API.</param>
    /// <param name="httpClient">Optional HTTP client instance.</param>
    /// <param name="loggerFactory">Logger factory.</param>
    public GoogleAIFileService(string apiKey, GoogleAIVersion apiVersion = GoogleAIVersion.V1_Beta, HttpClient? httpClient = null, ILoggerFactory? loggerFactory = null)
    {
        Verify.NotNullOrWhiteSpace(apiKey);
        this._client = new GoogleAIFileClient(
            httpClient: HttpClientProvider.GetHttpClient(httpClient),
            apiKey: apiKey,
            apiVersion: apiVersion,
            logger: loggerFactory?.CreateLogger(typeof(GoogleAIFileService)));
    }

    /// <summary>
    /// Uploads a file to Gemini service.
    /// </summary>
    /// <param name="fileName">Name of file.</param>
    /// <param name="mimeType">Mime type.</param>
    /// <param name="content">Stream content.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task<GoogleAIFile> UploadFileAsync(string fileName, string mimeType, Stream content, CancellationToken cancellationToken = default)
        => this._client.UploadFileAsync(fileName, mimeType, content, cancellationToken);
}
