using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DownloadManager.Services;

public class FileDownloader : IFileDownloader
{
    private readonly IHttpClientFactory _factory;
    
    public FileDownloader(IHttpClientFactory factory)
    {
        _factory = factory;
    }
    public int NumberOfThreads { get; set; }


    public event Action<long>? BytesDownloaded;
    
    public async Task DownloadFile(string urlToFile, string toPath)
    {
        using var client = new HttpClient();
        var head = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, urlToFile));
        if (head.Headers.AcceptRanges.First() != "bytes")
        {
            NumberOfThreads = 1;
        }
        var filename = head.Content.Headers.ContentDisposition?.FileName ?? Path.GetFileName(urlToFile);
        File.Create(filename);
        var length = head.Content.Headers.ContentLength;
        var bytePerTask = length / NumberOfThreads;
        var remainder = length % NumberOfThreads;
        var list = new List<Task>();
        long? start = 0L;
        long? end = 0L;
        for (var i = 0; i < NumberOfThreads; i++)
        {
            var tempStart = start ?? 0;
            end += bytePerTask;
            var tempEnd = end;
            if (i == NumberOfThreads - 1)
            {
                tempEnd += remainder ?? 0;
            }
            list.Add(Task.Run( async () =>
            {
                using var httpClient = new HttpClient();
                var request = new HttpRequestMessage { RequestUri = new Uri(urlToFile) };
                request.Headers.Range = new RangeHeaderValue(tempStart, tempEnd);
                using var response = await httpClient.SendAsync(request);
                await using var stream = await response.Content.ReadAsStreamAsync();
                await using var fileStream = new FileStream(toPath + $"/{filename}", new FileStreamOptions
                {
                    Share = FileShare.Write,
                    Access = FileAccess.Write,
                    Mode = FileMode.OpenOrCreate
                });
                fileStream.Position = tempStart;
                await stream.CopyToAsync(fileStream);
                BytesDownloaded?.Invoke(stream.Length);                
            }));
            start = end;
        }
        await Task.WhenAll(list);
    }
}