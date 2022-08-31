using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using FluentAvalonia.Styling;
using FluentAvalonia.UI.Controls;

namespace DownloadManager.Views
{
    public partial class MainView : CoreWindow, IRecipient<ValueChangedMessage<string>>, IRecipient<RequestMessage<string>>
    {

        private FluentAvaloniaTheme _theme;
        
        public MainView()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            _theme = AvaloniaLocator.Current.GetRequiredService<FluentAvaloniaTheme>();
            _theme.ForceWin32WindowToTheme(this);
            _theme.PreferSystemTheme = false;
            _theme.RequestedTheme = "Dark";
            _theme.PreferUserAccentColor = false;
            _theme.CustomAccentColor = Colors.DarkGoldenrod;
            _theme.UseSystemFontOnWindows = true;
            SplashScreen = new SampleAppSplashScreen();
            WeakReferenceMessenger.Default.RegisterAll(this);
        }

        public void Receive(ValueChangedMessage<string> message)
        {
            _theme.RequestedTheme = message.Value;
        }

        public void Receive(RequestMessage<string> message)
        {
            message.Reply(_theme.RequestedTheme);
        }
    }
}