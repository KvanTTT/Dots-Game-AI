using System;
using System.Text;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DotsGame.Sgf;

namespace DotsGame.GUI
{
    public class MainWindow : Window
    {
        private readonly MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            ServiceLocator.MainWindow = this;
            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;

            Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            ServiceLocator.Settings.OpenedFileName = ServiceLocator.GameTreeViewModel.FileName;
            var serializer = new SgfParser();
            ServiceLocator.Settings.CurrentGameSgf =
                Encoding.UTF8.GetString(serializer.Serialize(ServiceLocator.GameTreeViewModel.GameInfo));
            ServiceLocator.Settings.Save();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
