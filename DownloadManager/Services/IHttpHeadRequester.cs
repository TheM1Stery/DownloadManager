using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DownloadManager.Services;

public interface IHttpHeadRequester
{
    public Task<HttpResponseMessage> SendHeadRequestAsync(string url);
}