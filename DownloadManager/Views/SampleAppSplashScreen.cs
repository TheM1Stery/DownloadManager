using System;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using FluentAvalonia.Core.ApplicationModel;

namespace DownloadManager.Views;

public class SampleAppSplashScreen : IApplicationSplashScreen
{
    public SampleAppSplashScreen()
    {
        var al = AvaloniaLocator.Current.GetService<IAssetLoader>();
        using var s = al?.Open(new Uri("avares://DownloadManager/Assets/file-download-icon-19.jpg"));
        AppIcon = new Bitmap(s);
    }

    string? IApplicationSplashScreen.AppName { get; }

    public IImage AppIcon { get; }

    object? IApplicationSplashScreen.SplashScreenContent { get; }

    
    int IApplicationSplashScreen.MinimumShowTime => 2000;

    
    void IApplicationSplashScreen.RunTasks()
    {

    }
}