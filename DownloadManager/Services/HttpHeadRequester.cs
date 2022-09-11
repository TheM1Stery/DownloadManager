using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DownloadManager.Services;

public class HttpHeadRequester : IHttpHeadRequester
{
    private readonly IHttpClientFactory _factory;

    public HttpHeadRequester(IHttpClientFactory factory)
    {
        _factory = factory;
    }
    
    
    public async Task<HttpResponseMessage> SendHeadRequestAsync(string url)
    {
        using var client = _factory.CreateClient();
        return await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
    }
}