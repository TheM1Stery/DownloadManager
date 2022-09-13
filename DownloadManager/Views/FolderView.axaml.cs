using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DownloadManager.Views;

public partial class FolderView : UserControl
{
    public FolderView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}