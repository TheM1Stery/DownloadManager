using System.IO;
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

    [ObservableProperty] 
    private DownloadableItem _downloadableItem = null!;


    [ObservableProperty]
    private long _progress;


    [ObservableProperty]
    private long? _maximum;
    
    public DownloadableItemViewModel(IFileDownloader downloader)
    {
        _downloader = downloader;
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
        _downloader.NumberOfThreads = message.Value.Item2;
        WeakReferenceMessenger.Default.Unregister<DownloadItemMessage>(this);
        if (_downloadableItem.LinkToDownload is null || _downloadableItem.InstalledPath is null) 
            return;
        var fileInfo = await _downloader.GetFileInfo(_downloadableItem.LinkToDownload);
        Maximum = fileInfo.ContentLength;
        _downloadableItem.Name = fileInfo.ContentDisposition?.Name ?? Path.GetFileName(_downloadableItem.LinkToDownload);
        _downloader.BytesDownloaded += OnDownloaderOnBytesDownloaded;
        await _downloader.DownloadFile(_downloadableItem.LinkToDownload, _downloadableItem.InstalledPath);
        _downloader.BytesDownloaded -= OnDownloaderOnBytesDownloaded;
    }

    private void OnDownloaderOnBytesDownloaded(long progress)
    {
        Progress += progress;
    }
}