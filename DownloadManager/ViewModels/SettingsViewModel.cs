using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace DownloadManager.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    [ObservableProperty]
    private string? _chosenTheme;

    public ObservableCollection<string> Themes { get; } = new();


    public SettingsViewModel()
    {
        ChosenTheme = WeakReferenceMessenger.Default.Send(new RequestMessage<string>());
        Themes.Add("Dark");
        Themes.Add("Light");
        Themes.Add("HighContrast");
    }


    partial void OnChosenThemeChanged(string? value)
    {
        if (value != null) 
            WeakReferenceMessenger.Default.Send(new ValueChangedMessage<string>(value));
    }
}