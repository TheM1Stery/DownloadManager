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

    public ObservableCollection<string> Tags { get; } = new();

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
                Title = "Error", Content = "Error getting the file", PrimaryButtonText = "OK"
            };
            await dialog.ShowAsync();
            return;
        }
        Path = path;
    }

    [RelayCommand]
    private async Task AddTagAsync()
    {
        var viewModel = new AddTagViewModel();
        var dialog = new ContentDialog
        {
            Title = "Enter your tag",
            Content = viewModel,
            PrimaryButtonText = "Add",
            SecondaryButtonText = "Cancel"
        };
        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary && viewModel.Tag is not null)
        {
            Tags.Add(viewModel.Tag);
        }
    }

    [RelayCommand]
    private void DeleteItemFromTags(string tag)
    {
        Tags.Remove(tag);
    }

    [RelayCommand]
    private void ResetOptions()
    {
        Tags.Clear();
        Path = string.Empty;
        Link = string.Empty;
        NumberOfThreads = 1;
    }
}