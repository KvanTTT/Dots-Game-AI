using DotsGame.AI;
using Perspex.Controls;
using Perspex.Markup.Xaml;

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
            PerspexXamlLoader.Load(this);
        }
    }
}
