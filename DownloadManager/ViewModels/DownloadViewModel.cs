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
    private readonly IDialog _dialog;
    private readonly IDownloadDbClient _dbClient;

    
    public ObservableCollection<DownloadableItemViewModel> Downloads { get; } = new();
    
    public ObservableCollection<int> ThreadNumbers { get; } = new(Enumerable.Range(1, 6));

    public ObservableCollection<string> Tags { get; } = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DownloadCommand))]
    private string? _link;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DownloadCommand))]
    private string? _installedPath;

    [ObservableProperty]
    private int _numberOfThreads = 1;
    

    public bool CanExecuteDownload =>
        !string.IsNullOrWhiteSpace(Link) && !string.IsNullOrWhiteSpace(InstalledPath) && Tags.Count != 0;

    
    public DownloadViewModel(IFolderPicker folderPicker, IViewModelFactory factory, 
        IHttpHeadRequester headRequester, IDialog dialog, IDownloadDbClient dbClient)
    {
        _folderPicker = folderPicker;
        _factory = factory;
        _headRequester = headRequester;
        _dialog = dialog;
        _dbClient = dbClient;
        Tags.CollectionChanged += TagsOnCollectionChanged;
        Task.Run( async () =>
        {
            await _dbClient.InitializeAsync();
        });
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
            await _dialog.ShowMessageAsync("Error", "Error getting path. No path was provided");
            return;
        }
        InstalledPath = path;
    }

    [RelayCommand]
    private async Task AddTagAsync()
    {
        var viewModel = _factory.Create<AddTagViewModel>();
        var result = await _dialog.ShowContentAsync("Enter your tag", viewModel, 
            "Add", "Cancel");
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
        _dbClient.RemoveDownloadAsync(itemViewModel.DownloadableItem);
    }

    [RelayCommand]
    private void ResetOptions()
    {
        Tags.Clear();
        InstalledPath = string.Empty;
        Link = string.Empty;
        NumberOfThreads = 1;
    }
    
    private static bool IsValidUrl(string url)
    {
        const string pattern = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";
        var rgx = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        return rgx.IsMatch(url);
    }

    [RelayCommand(CanExecute = nameof(CanExecuteDownload))]
    private async Task DownloadAsync()
    {
        if (!IsValidUrl(Link!)) // Link will never be null here(bc of CanExecute)
        {
            await _dialog.ShowMessageAsync("Invalid Url", "The provided url is invalid!");
            return;
        }
        var headResponse = await _headRequester.SendHeadRequestAsync(Link!); // Link will never be null here(bc of CanExecute)
        if (!headResponse.IsSuccessStatusCode)
        {
            await _dialog.ShowMessageAsync("Error", "Error retrieving the file");
            return;
        }
        var fileName = headResponse.Content.Headers.ContentDisposition?.FileName ?? 
                       Path.GetFileName(Link);
        fileName = fileName?.Replace("\"", "");
        if (!Path.HasExtension(fileName))
        {
            fileName += "." + headResponse.Content.Headers.ContentType?.MediaType?.Split("/")[1];
        }
        var fileLength = headResponse.Content.Headers.ContentLength;
        var driveInfo = new DriveInfo(Path.GetPathRoot(InstalledPath) ?? string.Empty);
        if (driveInfo.AvailableFreeSpace < fileLength)
        {
            await _dialog.ShowMessageAsync("Size error", "You don't have enough space on this disk");
            return;
        }
        if (NumberOfThreads > 1 &&(headResponse.Headers.AcceptRanges.Count == 0 
                                   || headResponse.Headers.AcceptRanges.First() != "bytes"))
        {
            await _dialog.ShowMessageAsync("Warning", "This file doesn't support " +
                                                      "multi-threaded downloading. Only one thread will be used");
        }
        var filenameInitial = fileName;
        var filenameCurrent = filenameInitial;
        var count = 0;
        var alreadyExists = false;
        while (File.Exists(alreadyExists ? filenameCurrent : InstalledPath + $@"\{filenameCurrent}"))
        {
            count++;
            filenameCurrent = Path.GetDirectoryName(InstalledPath + $@"\{filenameInitial}")
                              + Path.DirectorySeparatorChar
                              + Path.GetFileNameWithoutExtension(InstalledPath + $@"\{filenameInitial}")
                              + $"({count})"
                              + Path.GetExtension(InstalledPath + $@"\{filenameInitial}");
            alreadyExists = true;
        }
        var downloadItem = new DownloadableItem
        {
            Name = Path.GetFileName(InstalledPath + $@"\{filenameCurrent}"),
            Size = fileLength,
            InstalledPath = InstalledPath,
            LinkToDownload = Link,
            Tags = Tags.Select(x => new Tag{Name = x}).ToList()
        };
        Downloads.Add(_factory.Create<DownloadableItemViewModel>());
        await _dbClient.AddDownloadAsync(downloadItem);
        downloadItem = await _dbClient.GetLatestDownload();
        WeakReferenceMessenger.Default.Send(new DownloadItemMessage((downloadItem, _numberOfThreads)));
        ResetOptions();
    }
}