using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Media;
using Avalonia.Threading;
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


    [ObservableProperty]
    private ObservableCollection<DownloadableItemViewModel> _downloads = new();
    
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

    private int count = 0;
    
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
            // await Populate();
        });
        _dbClient.GetAllDownloads();
        Downloads.CollectionChanged += DownloadsOnCollectionChanged;
    }

    private void DownloadsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        count++;
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
        Path = path;
    }
    
    // private async Task Populate()
    // {
    //     var downloadableItems = await _dbClient.GetAllDownloadsAsync();
    //     var items = downloadableItems.Select(x =>
    //     {
    //         var vm = _factory.Create<DownloadableItemViewModel>();
    //         vm.DownloadableItem = x;
    //         vm.Maximum = 100;
    //         vm.Progress = 100;
    //         switch (x.IsFinished)
    //         {
    //             case true:
    //                 vm.DownloadStatus = "Success";
    //                 vm.DownloadableItem.IsFinished = true;
    //                 vm.StatusBrush = Brushes.Green;
    //                 vm.IsStatusMessageVisible = true;
    //                 break;
    //             case false:
    //                 vm.DownloadStatus = "Error! Couldn't download the file";
    //                 vm.StatusBrush = Brushes.Red;
    //                 vm.IsStatusMessageVisible = true;
    //                 vm.DownloadableItem.IsFinished = false;
    //                 break;
    //         }
    //         return vm;
    //     });
    //     foreach (var downloadableItemViewModel in items)
    //     {
    //         Downloads.Add(downloadableItemViewModel);
    //     }
    // }

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
        Path = string.Empty;
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
                       System.IO.Path.GetFileName(Link);
        fileName = fileName?.Replace("\"", "");
        if (!System.IO.Path.HasExtension(fileName))
        {
            fileName += "." + headResponse.Content.Headers.ContentType?.MediaType?.Split("/")[1];
        }
        var fileLength = headResponse.Content.Headers.ContentLength;
        var driveInfo = new DriveInfo(System.IO.Path.GetPathRoot(Path) ?? string.Empty);
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
        var downloadItem = new DownloadableItem
        {
            Name = fileName,
            Size = fileLength,
            InstalledPath = Path,
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