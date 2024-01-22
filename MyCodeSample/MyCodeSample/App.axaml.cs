using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MyCodeSample.ViewModels;
using MyCodeSample.Views;

namespace MyCodeSample
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                System.Linq.IGrouping<string, ParamGeneral> testSetts = null;
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new SectionViewModel(null, testSetts),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}