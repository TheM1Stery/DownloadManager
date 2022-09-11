using System.Threading.Tasks;
using FluentAvalonia.UI.Controls;

namespace DownloadManager.Services;

public interface IDialog
{
    public Task ShowMessageAsync(string title, string message);

    public Task<ContentDialogResult> ShowContentAsync(string title, object content, string primaryButtonText,
        string secondaryButtonText);
}