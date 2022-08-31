using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using FluentAvalonia.Styling;
using FluentAvalonia.UI.Controls;

namespace DownloadManager.Views
{
    public partial class MainView : CoreWindow
    {
        public MainView()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            var theme = AvaloniaLocator.Current.GetRequiredService<FluentAvaloniaTheme>();
            theme.ForceWin32WindowToTheme(this);
            theme.RequestedTheme = "Dark";
            theme.PreferSystemTheme = false;
            theme.CustomAccentColor = Colors.DarkGoldenrod;
            theme.PreferUserAccentColor = false;
            theme.UseSystemFontOnWindows = true;
        }
    }
}