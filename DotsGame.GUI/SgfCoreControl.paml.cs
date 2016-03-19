using Perspex.Controls;
using Perspex.Markup.Xaml;

namespace DotsGame.GUI
{
    public class SgfCoreControl : UserControl
    {
        public SgfCoreControl()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            PerspexXamlLoader.Load(this);
        }
    }
}
