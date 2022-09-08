using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using DownloadManager.Messages;
using FluentAvalonia.Styling;
using FluentAvalonia.UI.Controls;

namespace DownloadManager.Views
{
    public partial class MainView : CoreWindow, IRecipient<ValueChangedMessage<string>>, 
        IRecipient<RequestMessage<string>>, IRecipient<AccentColorValueChanged>
    {

        private readonly FluentAvaloniaTheme _theme;
        
        public MainView()
        {
            InitializeComponent();
            WeakReferenceMessenger.Default.RegisterAll(this);
#if DEBUG
            // just used for debugging purposes. press F12 to see the debug menu
            this.AttachDevTools();
#endif
            // changing theme stuff
            _theme = AvaloniaLocator.Current.GetRequiredService<FluentAvaloniaTheme>();
            _theme.ForceWin32WindowToTheme(this);
            _theme.PreferSystemTheme = false;
            _theme.RequestedTheme = "Dark";
            _theme.PreferUserAccentColor = false;
            _theme.UseSystemFontOnWindows = true;
            SplashScreen = new SampleAppSplashScreen();
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            // this is used to replace the standard title bar with our custom written one
            if (TitleBar is null) 
                return;
            TitleBar.ExtendViewIntoTitleBar = true;
            SetTitleBar(TitleBarHost);
            TitleBarHost.Margin = new Thickness(0, 0, TitleBar.SystemOverlayRightInset, 0);
        }

        // MainView will receive messages from SettingsViewModel to change the theme of the application.
        public void Receive(ValueChangedMessage<string> message)
        {
            _theme.RequestedTheme = message.Value;
        }
        
        // this is used to send the current theme upon the application start to the ViewModel which requests it
        // Currently it's SettingsViewModel
        public void Receive(RequestMessage<string> message)
        {
            message.Reply(_theme.RequestedTheme);
        }

        public void Receive(AccentColorValueChanged message)
        {
            var current = _theme.CustomAccentColor;
            _theme.CustomAccentColor = message.Value ?? current;
        }
    }
}