using Perspex;
using Perspex.Controls;
using Perspex.Controls.Shapes;
using Perspex.Input;
using Perspex.Markup.Xaml;
using Perspex.Media;
using System;
using System.Collections.Generic;

namespace DotsGame.GUI
{
    public class DotsFieldGameControl : UserControl
    {
        internal readonly GameTreeControl GameTreeControl;

        public DotsFieldGameControl()
        {
            this.InitializeComponent();
            var _canvas = this.Find<Canvas>("CanvasField");
            var expander = this.Find<Expander>("GameTreeExpander");
            var dotsFieldViewModel = new DotsFieldViewModel(_canvas);
            this.DataContext = dotsFieldViewModel;
            ServiceLocator.DotsFieldViewModel = dotsFieldViewModel;

            GameTreeControl = new GameTreeControl();
            expander.Content = GameTreeControl;
        }

        private void InitializeComponent()
        {
            PerspexXamlLoader.Load(this);
        }
    }
}
