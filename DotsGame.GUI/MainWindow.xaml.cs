using DotsGame.Sgf;
using Perspex.Controls;
using Perspex.Input;
using Perspex.Markup.Xaml;
using System.Text;

namespace DotsGame.GUI
{
    public class MainWindow : Window
    {
        private readonly MainWindowViewModel _viewModel;

        public MainWindow()
        {
            this.InitializeComponent();
            App.AttachDevTools(this);

            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;

            this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, System.EventArgs e)
        {
            ServiceLocator.Settings.OpenedFileName = ServiceLocator.GameTreeViewModel.FileName;
            var serializer = new SgfParser();
            ServiceLocator.Settings.CurrentGameSgf =
                Encoding.UTF8.GetString(serializer.Serialize(ServiceLocator.GameTreeViewModel.GameInfo));
            ServiceLocator.Settings.Save();
        }

        private void InitializeComponent()
        {
            PerspexXamlLoader.Load(this);
        }
    }
}
