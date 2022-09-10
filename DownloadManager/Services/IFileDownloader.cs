using System.IO;
using System.Threading.Tasks;

namespace DownloadManager.Services;

public interface IFileDownloader
{
    public int NumberOfThreads { get; set; }

    public Task DownloadFile(string urlToFile, string toPath);
}