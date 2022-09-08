using Avalonia.Media;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace DownloadManager.Messages;

public class AccentColorValueChanged : ValueChangedMessage<Color?>
{
    public AccentColorValueChanged(Color? value) : base(value)
    {
    }
}