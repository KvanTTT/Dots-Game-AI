using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;

namespace DotsGame.GUI
{
    class App : Application
    {
        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.MainWindow = new MainWindow();
            base.OnFrameworkInitializationCompleted();
        }

        static void Main(string[] args)
        {
            AppBuilder.Configure<App>()
                .UseReactiveUI()
                .UsePlatformDetect()
                .StartWithClassicDesktopLifetime(args);
        }
    }
}
