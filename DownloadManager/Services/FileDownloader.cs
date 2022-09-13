using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace DownloadManager.Services;

public class FileDownloader : IFileDownloader
{
    private readonly IHttpClientFactory _factory;

    public FileDownloader(IHttpClientFactory factory)
    {
        _factory = factory;
    }
    public async Task DownloadFileAsync(string url, string toPath,int numberOfThreads, IProgress<long>? progress = null)
    {
        using var client = _factory.CreateClient();
        var head = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
        if (head.Headers.AcceptRanges.Count == 0 || head.Headers.AcceptRanges.First() != "bytes")
        {
            numberOfThreads = 1;
        }
        var name = head.Content.Headers.ContentDisposition?.FileName ?? Path.GetFileName(url);
        var invalidCharacters = Path.GetInvalidFileNameChars();
        var filename = string.Concat(name.Replace("\"", "")
            .Where(x => !invalidCharacters.Contains(x)));
        if (!Path.HasExtension(filename))
        {
            filename += "." + head.Content.Headers.ContentType?.MediaType?.Split("/")[1];
        }
        var filenameInitial = filename;
        var filenameCurrent = filenameInitial;
        var count = 0;
        var alreadyExists = false;
        while (File.Exists(filenameCurrent))
        {
            count++;
            filenameCurrent = Path.GetDirectoryName(toPath + $@"\{filenameInitial}")
                              + Path.DirectorySeparatorChar
                              + Path.GetFileNameWithoutExtension(toPath + $@"\{filenameInitial}")
                              + count
                              + Path.GetExtension(toPath + $@"\{filenameInitial}");
            alreadyExists = true;
        }
        if (!alreadyExists)
        {
            filenameCurrent = toPath + $@"\{filenameInitial}";
        }
        var createdStream = File.Create(filenameCurrent);
        await createdStream.DisposeAsync();
        var length = head.Content.Headers.ContentLength;
        if (numberOfThreads == 1)
        {
            await using var httpStream = await client.GetStreamAsync(url);
            await using var fStream =
                new FileStream(filenameCurrent, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
            await httpStream.CopyToAsync(fStream);
            progress?.Report(length ?? 0);
            return;
        }
        var bytePerTask = length / numberOfThreads;
        var remainder = length % numberOfThreads;
        var list = new List<Task>();
        long? start = 0L;
        long? end = 0L;
        for (var i = 0; i < numberOfThreads; i++)
        {
            var tempStart = start ?? 0;
            end += bytePerTask;
            var tempEnd = end;
            if (i == numberOfThreads - 1)
            {
                tempEnd += remainder ?? 0;
            }
            var i1 = i;
            list.Add(Task.Run( async () =>
            {
                using var httpClient = _factory.CreateClient();
                var request = new HttpRequestMessage { RequestUri = new Uri(url) };
                request.Headers.Range = new RangeHeaderValue(tempStart, tempEnd);
                using var response = await httpClient.SendAsync(request);
                await using var stream = await response.Content.ReadAsStreamAsync();
                await using var fileStream = new FileStream(filenameCurrent, new FileStreamOptions
                {
                    Share = FileShare.Write,
                    Access = FileAccess.Write,
                    Mode = FileMode.OpenOrCreate
                });
                fileStream.Position = tempStart;
                await stream.CopyToAsync(fileStream);
                if (i1 == numberOfThreads - 1)
                {
                    progress?.Report(bytePerTask + remainder ?? 0);
                    return;
                }
                progress?.Report(bytePerTask ?? 0);
            }));
            start = end;
        }
        await Task.WhenAll(list);
    }
}