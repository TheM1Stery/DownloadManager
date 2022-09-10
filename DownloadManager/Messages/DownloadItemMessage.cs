using System;
using CommunityToolkit.Mvvm.Messaging.Messages;
using DownloadManager.Models;

namespace DownloadManager.Messages;

public class DownloadItemMessage : ValueChangedMessage<(DownloadableItem, int)>
{
    public DownloadItemMessage((DownloadableItem, int) value) : base(value)
    {
    }
}