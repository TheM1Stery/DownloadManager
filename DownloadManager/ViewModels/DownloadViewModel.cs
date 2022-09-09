using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DownloadManager.Models;
using DownloadManager.Services;
using FluentAvalonia.UI.Controls;

namespace DownloadManager.ViewModels;

public partial class DownloadViewModel : ViewModelBase
{
    private readonly IFolderPicker _folderPicker;
    public ObservableCollection<DownloadableItemViewModel> Items { get; } = new();

    public ObservableCollection<int> ThreadNumbers { get; } = new(Enumerable.Range(1, 5));

    [ObservableProperty]
    private string? _link;

    [ObservableProperty]
    private string? _path;

    [ObservableProperty]
    private int _numberOfThreads = 1;

    public DownloadViewModel(IFolderPicker folderPicker)
    {
        _folderPicker = folderPicker;
        
        
        
        // Items.Add(new DownloadableItemViewModel()
        // {
        //     DownloadableItem = new DownloadableItem()
        //     {
        //         Name = "salam.txt",
        //         Size = 1000000L,
        //         IsPaused = false
        //     }
        // });
    }

    [RelayCommand]
    private async Task SelectPathAsync()
    {
        var path = await _folderPicker.GetPathAsync();
        if (path is null)
        {
            var dialog = new ContentDialog()
            {
                Title = "Error", Content = "Error getting the file", PrimaryButtonText = "OK", DefaultButton = ContentDialogButton.Primary
            };
            await dialog.ShowAsync();
            return;
        }
        Path = path;
    }
    
}