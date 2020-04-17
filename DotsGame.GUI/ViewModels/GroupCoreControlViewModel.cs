using System.Collections.Generic;
using System.Reactive;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using DotsGame.AI;
using ReactiveUI;

namespace DotsGame.GUI
{
    public class GroupCoreControlViewModel : ReactiveObject
    {
        public ReactiveCommand<Unit, Unit> ShowCrosswisesCommand { get; }

        public GroupCoreControlViewModel()
        {
            ShowCrosswisesCommand = ReactiveCommand.Create(() =>
            {
                var dotsField = ServiceLocator.DotsFieldViewModel;
                var groupsCore = new GroupsCore(ServiceLocator.DotsFieldViewModel.Field);
                var crosswises = groupsCore.GetCrosswises();

                var shapes = new List<Shape>();
                foreach (var crosswise in crosswises)
                {
                    shapes.Add(new Rectangle
                    {
                        [Canvas.LeftProperty] = crosswise.X * dotsField.CellSize + dotsField.FieldMargin,
                        [Canvas.TopProperty] = crosswise.Y * dotsField.CellSize + dotsField.FieldMargin,
                        Width = (crosswise.Pattern.Width - 1) * dotsField.CellSize,
                        Height = (crosswise.Pattern.Height - 1) * dotsField.CellSize,
                        StrokeThickness = 3,
                        Stroke = Brushes.LightSeaGreen,
                        ZIndex = 5
                    });
                }
                dotsField.AddShapes(shapes);
            });
        }
    }
}
