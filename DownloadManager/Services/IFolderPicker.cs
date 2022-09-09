using System.Threading.Tasks;

namespace DownloadManager.Services;

public interface IFolderPicker
{
    public Task<string?> GetPathAsync();
}