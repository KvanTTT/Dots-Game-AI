using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DotsGame.GUI
{
    public class GameTreeControl : UserControl
    {
        public GameTreeControl()
        {
            InitializeComponent();
            ServiceLocator.GameTreeViewModel = new GameTreeViewModel(this);
            DataContext = ServiceLocator.GameTreeViewModel;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
