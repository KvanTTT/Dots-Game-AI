using Perspex.Controls;
using Perspex.Markup.Xaml;

namespace DotsGame.GUI
{
    public class BasicCoreControl : UserControl
    {
        public BasicCoreControl()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            PerspexXamlLoader.Load(this);
        }
    }
}
