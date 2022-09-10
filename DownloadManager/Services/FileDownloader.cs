using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace DownloadManager.Services;

public class FileDownloader : IFileDownloader
{
    private readonly HttpClient _client;

    public FileDownloader(IHttpClientFactory factory)
    {
        _client = factory.CreateClient();
    }
    public int NumberOfThreads { get; set; }
    public Task DownloadFile(Stream to)
    {
        throw new System.NotImplementedException();
    }
}