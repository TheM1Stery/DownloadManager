using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DownloadManager.Messages;
using DownloadManager.Models;
using DownloadManager.Services;
using FluentAvalonia.UI.Controls;

namespace DownloadManager.ViewModels;

public partial class DownloadViewModel : ViewModelBase
{
    private readonly IFolderPicker _folderPicker;
    private readonly IViewModelFactory _factory;
    private readonly IHttpHeadRequester _headRequester;
    public ObservableCollection<DownloadableItemViewModel> Downloads { get; } = new();

    public ObservableCollection<int> ThreadNumbers { get; } = new(Enumerable.Range(1, 6));

    public ObservableCollection<string> Tags { get; } = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DownloadCommand))]
    private string? _link;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DownloadCommand))]
    private string? _path;

    [ObservableProperty]
    private int _numberOfThreads = 1;
    

    public bool CanExecuteDownload =>
        !string.IsNullOrWhiteSpace(Link) && !string.IsNullOrWhiteSpace(Path) && Tags.Count != 0;

    public DownloadViewModel(IFolderPicker folderPicker, IViewModelFactory factory, IHttpHeadRequester headRequester)
    {
        _folderPicker = folderPicker;
        _factory = factory;
        _headRequester = headRequester;
        Tags.CollectionChanged += TagsOnCollectionChanged;
    }

    private void TagsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        DownloadCommand.NotifyCanExecuteChanged();
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
        var viewModel = _factory.Create<AddTagViewModel>();
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
    private void DeleteItemFromDownloads(DownloadableItemViewModel itemViewModel)
    {
        Downloads.Remove(itemViewModel);
    }

    [RelayCommand]
    private void ResetOptions()
    {
        Tags.Clear();
        Path = string.Empty;
        Link = string.Empty;
        NumberOfThreads = 1;
    }
    
    private static bool IsValidUrl(string url)
    {
        var pattern = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";
        var rgx = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        return rgx.IsMatch(url);
    }

    [RelayCommand(CanExecute = nameof(CanExecuteDownload))]
    private async Task DownloadAsync()
    {
        if (!IsValidUrl(Link!))
        {
            var dialog = new ContentDialog
            {
                Title = "Invalid Url", Content = "The provided url is invalid.", PrimaryButtonText = "OK"
            };
            await dialog.ShowAsync();
            return;
        }
        var headResponse = await _headRequester.SendHeadRequestAsync(Link!); // Link will never be null here(bc of CanExecute)
        if (!headResponse.IsSuccessStatusCode)
        {
            var dialog = new ContentDialog
            {
                Title = "Error", Content = "Error retrieving the file", PrimaryButtonText = "OK"
            };
            await dialog.ShowAsync();
            return;
        }
        var fileName = headResponse.Content.Headers.ContentDisposition?.FileName ?? 
                       System.IO.Path.GetFileName(Link);
        if (!System.IO.Path.HasExtension(fileName))
        {
            fileName += "." + headResponse.Content.Headers.ContentType?.MediaType?.Split("/")[1];
        }
        var fileLength = headResponse.Content.Headers.ContentLength;
        var driveInfo = new DriveInfo(System.IO.Path.GetPathRoot(Path) ?? string.Empty);
        if (driveInfo.AvailableFreeSpace < fileLength)
        {
            var dialog = new ContentDialog
            {
                Title = "Size error", Content = "You don't have enough space on this disk", PrimaryButtonText = "OK"
            };
            await dialog.ShowAsync();
            return;
        }
        Downloads.Add(_factory.Create<DownloadableItemViewModel>());
        WeakReferenceMessenger.Default.Send(new DownloadItemMessage((new DownloadableItem()
        {
            Name = fileName,
            Size = fileLength,
            InstalledPath = Path,
            LinkToDownload = Link,
            Tags = Tags.ToList()
        }, _numberOfThreads)));
        ResetOptions();
    }
}