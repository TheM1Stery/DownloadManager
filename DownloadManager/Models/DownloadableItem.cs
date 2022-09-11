using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DownloadManager.Models;

public partial class DownloadableItem : ObservableObject
{
    [ObservableProperty]
    private string? _name;
    
    public long Size { get; set; }
    
    public string? InstalledPath { get; set; }

    public List<string>? Tags { get; set; }

    public string? LinkToDownload { get; set; }
    
    [ObservableProperty]
    private bool _isPaused;
}