using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace DownloadManager.Services;

public class FolderPicker : IFolderPicker
{
    public async Task<string?> GetPathAsync()
    {
        var dialog = new OpenFolderDialog();
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
        {
            return null;
        }
        var path = await dialog.ShowAsync(desktop.MainWindow);
        return path;
    }
}