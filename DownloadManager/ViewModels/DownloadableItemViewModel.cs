using System;
using System.IO;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using DownloadManager.Messages;
using DownloadManager.Models;
using DownloadManager.Services;

namespace DownloadManager.ViewModels;

public partial class DownloadableItemViewModel : ViewModelBase, IRecipient<DownloadItemMessage>
{
    private readonly IFileDownloader _downloader;
    private readonly IDownloadDbClient _dbClient;

    [ObservableProperty] 
    private DownloadableItem _downloadableItem = null!;


    [ObservableProperty]
    private long _progress;
    
    [ObservableProperty]
    private long? _maximum;

    [ObservableProperty]
    private string? _downloadStatus;

    [ObservableProperty]
    private IBrush? _statusBrush;

    [ObservableProperty]
    private bool? _isStatusMessageVisible;
    
    public DownloadableItemViewModel(IFileDownloader downloader, IDownloadDbClient dbClient)
    {
        _downloader = downloader;
        _dbClient = dbClient;
        WeakReferenceMessenger.Default.Register(this);
    }
    

    [RelayCommand]
    private void Pause()
    {
        DownloadableItem.IsPaused = !DownloadableItem.IsPaused;
    }
    
    public async void Receive(DownloadItemMessage message)
    {
        DownloadableItem = message.Value.Item1;
        var numberOfThreads = message.Value.Item2;
        WeakReferenceMessenger.Default.Unregister<DownloadItemMessage>(this);
        if (_downloadableItem.LinkToDownload is null || _downloadableItem.InstalledPath is null) 
            return;
        Maximum = _downloadableItem.Size;
        var progress = new Progress<long>(OnProgressReport);
        try
        {
            await _downloader.DownloadFileAsync(_downloadableItem.LinkToDownload, _downloadableItem.InstalledPath, 
                numberOfThreads,progress);
            DownloadStatus = "Success";
            DownloadableItem.IsFinished = true;
            StatusBrush = Brushes.Green;
            IsStatusMessageVisible = true;
        }
        catch (Exception)
        {
            DownloadStatus = "Error! Couldn't download the file";
            StatusBrush = Brushes.Red;
            IsStatusMessageVisible = true;
            DownloadableItem.IsFinished = false;
        }
        await _dbClient.EditDownloadAsync(_downloadableItem);
    }

    private void OnProgressReport(long progress)
    {
        Progress += progress;
    }
}