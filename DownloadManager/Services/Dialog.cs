using System.Threading.Tasks;
using FluentAvalonia.UI.Controls;

namespace DownloadManager.Services;

public class Dialog : IDialog
{
    public async Task ShowMessageAsync(string title, string message)
    {
        var dialog = new ContentDialog
        {
            Title = title, Content = message, PrimaryButtonText = "OK"
        };
        await dialog.ShowAsync();
    }

    public async Task<ContentDialogResult> ShowContentAsync(string title, object content, 
        string primaryButtonText, string secondaryButtonText)
    {
        var dialog = new ContentDialog()
        {
            Title = title,
            Content = content,
            PrimaryButtonText = primaryButtonText,
            SecondaryButtonText = secondaryButtonText
        };
        return await dialog.ShowAsync();
    }
}