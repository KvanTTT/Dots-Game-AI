﻿using Perspex.Controls;
using Perspex.Markup.Xaml;

namespace DotsGame.GUI
{
    public class GameTreeControl : UserControl
    {
        public GameTreeControl()
        {
            this.InitializeComponent();
            ServiceLocator.GameTreeViewModel = new GameTreeViewModel(this);
            this.DataContext = ServiceLocator.GameTreeViewModel;
        }

        private void InitializeComponent()
        {
            PerspexXamlLoader.Load(this);
        }
    }
}
