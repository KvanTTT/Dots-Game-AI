using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DotsGame.GUI
{
    public class DotsFieldGameControl : UserControl
    {
        internal readonly GameTreeControl GameTreeControl;

        public DotsFieldGameControl()
        {
            InitializeComponent();
            var _canvas = this.Find<Canvas>("CanvasField");
            var expander = this.Find<Expander>("GameTreeExpander");
            var dotsFieldViewModel = new DotsFieldViewModel(_canvas);
            DataContext = dotsFieldViewModel;
            ServiceLocator.DotsFieldViewModel = dotsFieldViewModel;

            GameTreeControl = new GameTreeControl();
            expander.Content = GameTreeControl;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
