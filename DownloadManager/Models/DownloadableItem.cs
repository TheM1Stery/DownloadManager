using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DownloadManager.Models;

public partial class DownloadableItem : ObservableObject
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    
    public long? Size { get; set; }
    
    public string? InstalledPath { get; set; }

    public List<Tag>? Tags { get; set; }

    public string? LinkToDownload { get; set; }
    
    public bool IsFinished { get; set; }
    
    [ObservableProperty]
    private bool _isPaused;
}