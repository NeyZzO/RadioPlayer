using System.Net.Http;
using AsyncImageLoader;
using AsyncImageLoader.Loaders;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using RadioPlayer.ViewModels;
using RadioPlayer.Views;

namespace RadioPlayer {
    public partial class App : Application {
        public override void Initialize() {
            AvaloniaXamlLoader.Load(this);

            // Configure AsyncImageLoader with a custom HttpClient that includes User-Agent
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "RadioPlayer/1.0 (Avalonia Desktop App)");

            ImageLoader.AsyncImageLoader = new RamCachedWebImageLoader(httpClient, disposeHttpClient: true);
        }

        public override void OnFrameworkInitializationCompleted() {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
                desktop.MainWindow = new MainWindow {
                    DataContext = new MainWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

    }
}