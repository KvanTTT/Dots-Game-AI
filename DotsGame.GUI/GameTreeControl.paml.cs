using Perspex.Controls;
using Perspex.Markup.Xaml;

namespace DotsGame.GUI
{
    public class GameTreeControl : UserControl
    {
        public GameTreeControl()
            : this(null)
        {
        }

        public GameTreeControl(DotsFieldViewModel dotsFieldViewModel)
        {
            this.InitializeComponent();
            var gameTreeViewModel = new GameTreeViewModel(this, dotsFieldViewModel);
            this.DataContext = gameTreeViewModel;
            dotsFieldViewModel.GameTreeViewModel = gameTreeViewModel;
        }

        private void InitializeComponent()
        {
            PerspexXamlLoader.Load(this);
        }
    }
}
