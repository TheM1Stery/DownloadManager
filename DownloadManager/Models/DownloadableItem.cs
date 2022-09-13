using System;
using System.Collections.Generic;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DownloadManager.Models;

public partial class DownloadableItem : ObservableObject
{
    public int? Id { get; set; }

    [ObservableProperty]
    private string? _name;
    
    
    public long? Size { get; set; }
    
    [ObservableProperty]
    private string? _installedPath;

    public List<Tag>? Tags { get; set; }

    public string TagsToString
    {
        get
        {
            var builder = new StringBuilder();
            builder.Append("Tags: ");
            if (Tags != null)
                foreach (var tag in Tags)
                {
                    builder.Append(tag.Name + " ");
                }

            return builder.ToString();
        }
    }

    public string? LinkToDownload { get; set; }
    
    public bool IsFinished { get; set; }
    
    [ObservableProperty]
    private bool _isPaused;


    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.Append(Name + "\n");
        builder.Append("Tags: ");
        if (Tags != null)
            foreach (var tag in Tags)
            {
                builder.Append(tag.Name + " ");
            }
        builder.Append('\n');
        builder.Append($"File directory: {InstalledPath}");
        return builder.ToString();
    }
}