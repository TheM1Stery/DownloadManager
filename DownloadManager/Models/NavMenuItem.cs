using System;
using FluentAvalonia.UI.Controls;

namespace DownloadManager.Models;

public class NavMenuItem
{
    public Symbol Icon { get; set; }
    
    public string? Header { get; set; }
    
    public Type? ContentViewModelType { get; set; }
}