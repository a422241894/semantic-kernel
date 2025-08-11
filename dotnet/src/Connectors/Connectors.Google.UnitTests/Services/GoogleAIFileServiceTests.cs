using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.Connectors.Google;
using Xunit;

namespace SemanticKernel.Connectors.Google.UnitTests.Services;

public sealed class GoogleAIFileServiceTests : IDisposable
{
    private const string ApiKey = "test-key";
    private readonly HttpClient _httpClient;
    private readonly HttpMessageHandlerStub _messageHandlerStub;

    public GoogleAIFileServiceTests()
    {
        this._messageHandlerStub = new HttpMessageHandlerStub
        {
            ResponseToReturn = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("{\"name\":\"files/123\",\"displayName\":\"file.txt\",\"mimeType\":\"text/plain\",\"sizeBytes\":\"4\"}", Encoding.UTF8, "application/json")
            }
        };
        this._httpClient = new HttpClient(this._messageHandlerStub, disposeHandler: false);
    }

    [Fact]
    public async Task UploadSendsMultipartRequestAsync()
    {
        // Arrange
        var service = new GoogleAIFileService(ApiKey, httpClient: this._httpClient);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("test"));

        // Act
        var file = await service.UploadFileAsync("file.txt", "text/plain", stream);

        // Assert
        Assert.Equal(HttpMethod.Post, this._messageHandlerStub.Method);
        Assert.NotNull(this._messageHandlerStub.RequestUri);
        Assert.Contains("upload", this._messageHandlerStub.RequestUri.ToString(), StringComparison.Ordinal);
        Assert.True(this._messageHandlerStub.RequestHeaders!.Contains("x-goog-api-key"));
        Assert.Equal("multipart/form-data", this._messageHandlerStub.ContentHeaders!.ContentType!.MediaType);
        Assert.Contains("file.txt", Encoding.UTF8.GetString(this._messageHandlerStub.FirstMultipartContent!));
        Assert.Equal("files/123", file.Name);
    }

    public void Dispose()
    {
        this._httpClient.Dispose();
        this._messageHandlerStub.Dispose();
    }
}
