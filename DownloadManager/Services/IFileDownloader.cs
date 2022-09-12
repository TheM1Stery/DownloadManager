using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DownloadManager.Services;

public interface IFileDownloader
{
    public int NumberOfThreads { get; set; }


    public Task DownloadFileAsync(string url, string toPath, IProgress<long>? progress = null);
    
}