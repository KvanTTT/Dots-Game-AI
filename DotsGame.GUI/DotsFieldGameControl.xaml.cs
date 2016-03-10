using Perspex;
using Perspex.Controls;
using Perspex.Controls.Shapes;
using Perspex.Markup.Xaml;
using Perspex.Media;
using System;
using System.Collections.Generic;

namespace DotsGame.GUI
{
    public class DotsFieldGameControl : UserControl
    {
        public DotsFieldGameControl()
        {
            this.InitializeComponent();
            var canvas = this.Find<Canvas>("CanvasField");
            var expander = this.Find<Expander>("GameTreeExpander");
            var dotsFieldViewModel = new DotsFieldViewModel(canvas);
            this.DataContext = dotsFieldViewModel;

            expander.Content = new GameTreeControl(dotsFieldViewModel);
        }

        private void InitializeComponent()
        {
            PerspexXamlLoader.Load(this);
        }
    }
}
