using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DotsGame.GUI
{
    public class SgfCoreControl : UserControl
    {
        public SgfCoreControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
