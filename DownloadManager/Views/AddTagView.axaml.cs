using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DownloadManager.Views;

public partial class AddTagView : UserControl
{
    public AddTagView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}