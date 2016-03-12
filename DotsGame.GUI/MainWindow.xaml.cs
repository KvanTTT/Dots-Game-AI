using Perspex.Controls;
using Perspex.Input;
using Perspex.Markup.Xaml;

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
        }

        private void InitializeComponent()
        {
            PerspexXamlLoader.Load(this);
        }
    }
}
