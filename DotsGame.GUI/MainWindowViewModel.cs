using DotsGame.AI;
using Perspex.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotsGame.GUI
{
    public class MainWindowViewModel : ReactiveObject
    {
        private CoreType _selectedCoreType;
        private UserControl _coreCotnrol;

        public CoreType SelectedCoreType
        {
            get
            {
                return _selectedCoreType;
            }
            set
            {
                _selectedCoreType = value;
                CoreControl = CoreControlFactory.Create(_selectedCoreType);
            }
        }

        public CoreType[] CoreTypes => new CoreType[]
        {
            CoreType.BasicCore,
            CoreType.GroupsCore
        };
        
        public UserControl CoreControl
        {
            get
            {
                return _coreCotnrol;
            }
            set
            {
                _coreCotnrol = value;
            }
        }
    }
}
