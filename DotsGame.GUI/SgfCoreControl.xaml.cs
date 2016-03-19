using Perspex.Controls;
using Perspex.Markup.Xaml;

namespace DotsGame.GUI
{
    public class SgfCoreControl : UserControl
    {
        public SgfCoreControl()
        {
            this.InitializeComponent();
            this.DataContext = ServiceLocator.BasicCoreControViewModel;
        }

        private void InitializeComponent()
        {
            PerspexXamlLoader.Load(this);
        }
    }
}
