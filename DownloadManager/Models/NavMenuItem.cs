using System;
using FluentAvalonia.UI.Controls;

namespace DownloadManager.Models;

public class NavMenuItem
{
    public Symbol Icon { get; set; }
    
    public string? Header { get; set; }
    
    // please avoid using reflection for creating the object with this type. Create a factory class instead
    public Type? ContentViewModelType { get; set; }
}