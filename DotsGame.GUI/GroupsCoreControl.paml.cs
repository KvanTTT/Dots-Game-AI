using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DotsGame.GUI
{
    public class GroupsCoreControl : UserControl
    {
        public GroupsCoreControl()
        {
            this.InitializeComponent();
            this.DataContext = new GroupCoreControlViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
