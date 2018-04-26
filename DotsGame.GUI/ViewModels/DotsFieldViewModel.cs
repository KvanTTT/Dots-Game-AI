﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotsGame.GUI
{
    public class DotsFieldViewModel : ReactiveObject
    {
        private Canvas _canvasField;
        private Field _field;
        private int _player1Score, _player2Score;
        private string _player1Name, _player2Name;

        private int _currentBaseZInd;
        
        private List<Line> _lineShapes = new List<Line>();
        private List<Ellipse> _dotShapes = new List<Ellipse>();
        private Stack<List<Shape>> _movesShapes = new Stack<List<Shape>>();
        private Shape _lastMoveMarker;
        private List<Shape> _additionalShapes = new List<Shape>();

        private GameTreeViewModel _gameTreeViewModel;

        public double CellSize { get; set; } = 19;
        public double LineThickness { get; set; } = 0.75;
        public double DotRadius { get; set; } = 5;
        public double FieldMargin { get; set; } = 10;

        public Color Player1Color { get; set; } = Color.FromRgb(0, 0, 255);
        public Color Player2Color { get; set; } = Color.FromRgb(255, 0, 0);
        public Color Player1BaseColor { get; private set; }
        public Color Player2BaseColor { get; private set; }

        public IBrush LineBrush { get; set; } = Brushes.DimGray;

        public GameTreeViewModel GameTreeViewModel
        {
            get => _gameTreeViewModel ?? (_gameTreeViewModel = ServiceLocator.GameTreeViewModel);
        }

        public DotsFieldViewModel(Canvas canvas, Field field = null)
        {
            _canvasField = canvas;
            _canvasField.PointerPressed += CanvasField_PointerPressed;
            _field = field ?? new Field(39, 32);
            Player1BaseColor = Color.FromArgb(50, Player1Color.R, Player1Color.G, Player1Color.B);
            Player2BaseColor = Color.FromArgb(50, Player2Color.R, Player1Color.G, Player2Color.B);
            RefreshField();
            _currentBaseZInd = 10;
        }

        public int Player1Score
        {
            get => _player1Score;
            set => this.RaiseAndSetIfChanged(ref _player1Score, value);
        }

        public int Player2Score
        {
            get => _player2Score;
            set => this.RaiseAndSetIfChanged(ref _player2Score, value);
        }

        public string Player1Name
        {
            get => _player1Name;
            set => this.RaiseAndSetIfChanged(ref _player1Name, value);
        }

        public string Player2Name
        {
            get => _player2Name;
            set => this.RaiseAndSetIfChanged(ref _player2Name, value);
        }

        public IBrush Player1Brush => new SolidColorBrush(Player1Color);

        public IBrush Player2Brush => new SolidColorBrush(Player2Color);

        public Field Field
        {
            get => _field;
            set
            {
                _field = value;
                _canvasField.Children.Clear();
                RefreshField();
                ResetAll();
            }
        }

        public void MakeMoves(GameMove move)
        {
            if (_field.MakeMove(move.Column, move.Row, move.PlayerNumber))
            {
                AddLastMoveState();
                UpdateInfo();
            }
        }

        public void MakeMoves(IList<GameMove> moves)
        {
            foreach (var move in moves)
            {
                if (_field.MakeMove(move.Column, move.Row, move.PlayerNumber))
                {
                    AddLastMoveState();
                }
            }
            UpdateInfo();
        }

        public void UnmakeMoves(int movesCount = 1)
        {
            for (int i = 0; i < movesCount; i++)
            {
                State lastState = _field.States.Last();
                if (_field.UnmakeMove())
                {
                    RemoveMoveState(lastState);
                }
            }
            UpdateInfo();
        }

        public void ClearMoveMarker()
        {
            _canvasField.Children.Remove(_lastMoveMarker);
            _lastMoveMarker = null;
        }

        public void AddShapes(IList<Shape> shapes)
        {
            _additionalShapes.AddRange(shapes);
            _canvasField.Children.AddRange(shapes);
        }

        private void CanvasField_PointerPressed(object sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            var pos = e.GetPosition(_canvasField) - new Point(FieldMargin, FieldMargin);
            pos = pos / CellSize;
            int fieldPosX = (int)Math.Round(pos.X) + 1;
            int fieldPosY = (int)Math.Round(pos.Y) + 1;
            if (e.MouseButton == Avalonia.Input.MouseButton.Left)
            {
                if (_field.MakeMove(fieldPosX, fieldPosY))
                {
                    AddLastMoveState();
                    UpdateInfo();
                    GameTreeViewModel.AddMove(new GameMove((int)Field.CurrentPlayer.NextPlayer(), fieldPosY, fieldPosX));
                }
            }
            else if (e.MouseButton == Avalonia.Input.MouseButton.Right)
            {
                Field.GetPosition(_field.LastMakedPosition, out int lastX, out int lastY);
                if (fieldPosX == lastX && fieldPosY == lastY)
                {
                    GameTreeViewModel.PrevMoveCommandAction();
                }
            }
        }

        private void UpdateInfo()
        {
            UpdateLastMoveMarker();
            Player1Score = _field.Player0CaptureCount;
            Player2Score = _field.Player1CaptureCount;
        }

        private void RefreshField()
        {
            _lineShapes.Clear();

            _canvasField.Width = CellSize * _field.Width;
            _canvasField.Height = CellSize * _field.Height;

            double lineLength = CellSize * (_field.Height - 1);
            double x = FieldMargin;
            for (int i = 0; i < _field.Width; i++)
            {
                Line line = new Line
                {
                    Stroke = LineBrush,
                    StrokeThickness = LineThickness,
                    StartPoint = new Point(x, FieldMargin),
                    EndPoint = new Point(x, FieldMargin + lineLength),
                    ZIndex = 0,
                };

                _lineShapes.Add(line);
                _canvasField.Children.Add(line);
                x += CellSize;
            }

            lineLength = CellSize * (_field.Width - 1);
            double y = FieldMargin;
            for (int i = 0; i < _field.Height; i++)
            {
                Line line = new Line
                {
                    Stroke = LineBrush,
                    StrokeThickness = LineThickness,
                    StartPoint = new Point(FieldMargin, y),
                    EndPoint = new Point(FieldMargin + lineLength, y),
                    ZIndex = 0,
                };

                _lineShapes.Add(line);
                _canvasField.Children.Add(line);
                y += CellSize;
            }
        }

        private void ResetAll()
        {
            _additionalShapes.Clear();
            _dotShapes.Clear();
            _movesShapes.Clear();
            UpdateLastMoveMarker();
            _currentBaseZInd = 10;
        }

        private void AddLastMoveState()
        {
            ClearAdditionalShapes();
            State state = _field.States.Last();
            var moveShapes = new List<Shape>();

            int dotZIndex;
            if (state.Base == null || state.Base.LastCaptureCount == 0)
            {
                dotZIndex = _currentBaseZInd;
            }
            else
            {
                _currentBaseZInd += 2;
                dotZIndex = _currentBaseZInd;
            }

            short pos = state.Move.Position;
            Point point = GetGraphicalPoint(pos);
            DotState dotState = _field[pos];
            var dotCircle = new Ellipse
            {
                Fill = new SolidColorBrush(dotState.IsRealPlayer0() ? Player1Color : Player2Color),
                Width = DotRadius * 2,
                Height = DotRadius * 2,
                [Canvas.LeftProperty] = point.X - DotRadius,
                [Canvas.TopProperty] = point.Y - DotRadius,
                ZIndex = _currentBaseZInd,
                Tag = pos
            };
            _dotShapes.Add(dotCircle);
            _canvasField.Children.Add(dotCircle);
            moveShapes.Add(dotCircle);

            if (state.Base != null && state.Base.LastCaptureCount != 0)
            {
                List<short> positions = state.Base.ChainPositions;
                var polygonPoints = new List<Point>(positions.Count);
                foreach (short chainPos in positions)
                {
                    var baseDot = _dotShapes.FirstOrDefault(dot => (short)dot.Tag == chainPos);
                    baseDot.ZIndex = dotZIndex;
                    polygonPoints.Add(GetGraphicalPoint(chainPos));
                }
                bool redBaseColor = state.Base.ChainDotPositions.Last().Dot.IsRealPlayer0();
                var basePolygon = new Polygon
                {
                    Fill = new SolidColorBrush(redBaseColor ? Player1BaseColor : Player2BaseColor),
                    Stroke = new SolidColorBrush(redBaseColor ? Player1Color : Player2Color),
                    StrokeThickness = LineThickness,
                    Points = polygonPoints,
                    ZIndex = dotZIndex - 1
                };
                
                _canvasField.Children.Add(basePolygon);
                moveShapes.Add(basePolygon);
            }

            _movesShapes.Push(moveShapes);
        }

        private void RemoveMoveState(State state)
        {
            ClearAdditionalShapes();
            if (state.Base != null && state.Base.LastCaptureCount != 0)
            { 
                _currentBaseZInd -= 2;
            }
            List<Shape> moveShapes = _movesShapes.Pop();
            _dotShapes.Remove((Ellipse)moveShapes.First());
            _canvasField.Children.RemoveAll(moveShapes);
        }

        private void UpdateLastMoveMarker()
        {
            _canvasField.Children.Remove(_lastMoveMarker);
            if (_field.States.Count > 0)
            {
                short pos = _field.States.Last().Move.Position;
                Point point = GetGraphicalPoint(pos);
                _lastMoveMarker = new Ellipse
                {
                    Fill = Brushes.White,
                    Width = DotRadius * 1.2,
                    Height = DotRadius * 1.2,
                    [Canvas.LeftProperty] = point.X - DotRadius * 0.6,
                    [Canvas.TopProperty] = point.Y - DotRadius * 0.6,
                    ZIndex = 1000,
                };
                _canvasField.Children.Add(_lastMoveMarker);
            }
            else
            {
                _lastMoveMarker = null;
            }
        }

        private Point GetGraphicalPoint(int dotPos)
        {
            Field.GetPosition(dotPos, out int posX, out int posY);
            return new Point((posX - 1) * CellSize + FieldMargin, (posY - 1) * CellSize + FieldMargin);
        }

        private void ClearAdditionalShapes()
        {
            _canvasField.Children.RemoveAll(_additionalShapes);
            _additionalShapes.Clear();
        }
    }
}
