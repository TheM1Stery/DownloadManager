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

    [ObservableProperty] 
    private DownloadableItem _downloadableItem = null!;


    [ObservableProperty]
    private long _progress;


    [ObservableProperty]
    private long? _maximum;

    [ObservableProperty]
    private string? _error;

    [ObservableProperty]
    private IBrush? _errorBrush;

    [ObservableProperty]
    private bool? _isErrorMessageVisible;
    
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
        _downloadableItem.Name = fileInfo.ContentDisposition?.FileName ?? Path.GetFileName(_downloadableItem.LinkToDownload);
        if (!Path.HasExtension(_downloadableItem.Name))
        {
            _downloadableItem.Name += "." + fileInfo.ContentType?.MediaType?.Split("/")[1];
        }
        _downloader.BytesDownloaded += OnDownloaderOnBytesDownloaded;
        try
        {
            await _downloader.DownloadFile(_downloadableItem.LinkToDownload, _downloadableItem.InstalledPath);
        }
        catch (Exception)
        {
            Error = "Error! Couldn't download the file";
            ErrorBrush = Brushes.Red;
            IsErrorMessageVisible = true;
            _downloader.BytesDownloaded -= OnDownloaderOnBytesDownloaded;
            return;
        }
        Error = "Success";
        ErrorBrush = Brushes.Green;
        IsErrorMessageVisible = true;
        _downloader.BytesDownloaded -= OnDownloaderOnBytesDownloaded;
    }

    private void OnDownloaderOnBytesDownloaded(long progress)
    {
        Progress += progress;
    }
}