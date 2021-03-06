﻿using Avalonia.Controls;
using DotsGame.AI;
using ReactiveUI;

namespace DotsGame.GUI
{
    public class MainWindowViewModel : ReactiveObject
    {
        private CoreType _selectedCoreType;
        private UserControl _coreControl;

        public CoreType SelectedCoreType
        {
            get => _selectedCoreType;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedCoreType, value);
                CoreControl = CoreControlFactory.Create(_selectedCoreType);
            }
        }

        public CoreType[] CoreTypes => new[]
        {
            CoreType.SgfCore,
            CoreType.GroupsCore
        };

        public UserControl CoreControl
        {
            get => _coreControl;
            set => this.RaiseAndSetIfChanged(ref _coreControl, value);
        }
    }
}
