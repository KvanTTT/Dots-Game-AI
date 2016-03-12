using Perspex.Controls;
using Perspex.Markup.Xaml;

namespace DotsGame.GUI
{
    public class GameTreeControl : UserControl
    {
        internal readonly GameTreeViewModel GameTreeViewModel;

        public GameTreeControl()
            : this(null)
        {
        }

        public GameTreeControl(DotsFieldViewModel dotsFieldViewModel)
        {
            this.InitializeComponent();
            GameTreeViewModel = new GameTreeViewModel(this, dotsFieldViewModel);
            this.DataContext = GameTreeViewModel;
            dotsFieldViewModel.GameTreeViewModel = GameTreeViewModel;
        }

        private void InitializeComponent()
        {
            PerspexXamlLoader.Load(this);
        }
    }
}
