using DotsGame.AI;
using Perspex.Controls;
using Perspex.Markup.Xaml;

namespace DotsGame.GUI
{
    public class GroupsCoreControl : UserControl
    {
        private GroupsCore _groupsCore;

        public GroupsCoreControl()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            PerspexXamlLoader.Load(this);
        }
    }
}
