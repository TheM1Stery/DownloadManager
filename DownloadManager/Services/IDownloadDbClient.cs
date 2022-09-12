using System.Collections.Generic;
using System.Threading.Tasks;
using DownloadManager.Models;

namespace DownloadManager.Services;

public interface IDownloadDbClient
{
    public Task InitializeAsync();

    public Task AddDownloadAsync(DownloadableItem downloadableItem);

    public Task RemoveDownloadAsync(DownloadableItem downloadableItem);

    public Task EditDownloadAsync(DownloadableItem downloadableItem);

    public Task<List<DownloadableItem>> GetAllDownloadsAsync();

    public List<DownloadableItem> GetAllDownloads();

    public Task<DownloadableItem> GetLatestDownload();
}