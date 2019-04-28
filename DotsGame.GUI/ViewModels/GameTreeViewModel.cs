using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;
using DotsGame.Formats;
using DotsGame.Sgf;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace DotsGame.GUI
{
    public class GameTreeViewModel : ReactiveObject
    {
        private DotsFieldViewModel _dotsFieldViewModel;
        private GameInfo _gameInfo;
        private UserControl _gameTreeUserControl;
        private Canvas _gameTreeCanvas;
        private ScrollViewer _canvasScrollViewer;
        private string _fileName;
        private bool _autoUpdate;
        private Timer _autoupdateGameTimer;

        private double _dotSize = 23;
        private double _dotSpace = 32;
        private double _padding = 30;

        private Rectangle _selectedTreeRect;
        private GameTree _previousSelectedGameTree;
        private GameTree _selectedGameTree;
        private GameTree[,] _gameTreesOnCanvas;
        private Dictionary<GameTree, Tuple<int, int>> _gameTreesPositions = new Dictionary<GameTree, Tuple<int, int>>();
        int _prevMovesCount;

        private Dictionary<long, Ellipse> _moveCircles = new Dictionary<long, Ellipse>();
        private Dictionary<long, Ellipse> _newMoveCircles;
        private Dictionary<long, Line> _treeLines = new Dictionary<long, Line>();
        private Dictionary<long, Line> _newTreeLines;
        private Dictionary<long, TextBlock> _moveLabels = new Dictionary<long, TextBlock>();
        private Dictionary<long, TextBlock> _newMoveLabels;

        private bool _showLabels = true;

        public ReactiveCommand PrevMoveCommand { get; }

        public ReactiveCommand NextMoveCommand { get; }

        public ReactiveCommand StartMoveCommand { get; }

        public ReactiveCommand EndMoveCommand { get; }

        public ReactiveCommand RemoveCommand { get; }

        public ReactiveCommand OpenFileCommand { get; }

        public ReactiveCommand OpenPlaydotsVkCommand { get; }

        public ReactiveCommand ResetCommand { get; }

        public ReactiveCommand SaveCommand { get; }

        public ReactiveCommand UpdateCommand { get; }

        public DotsFieldViewModel DotsFieldViewModel
        {
            get => _dotsFieldViewModel ?? (_dotsFieldViewModel = ServiceLocator.DotsFieldViewModel);
        }

        public string FileName
        {
            get => _fileName;
            set => this.RaiseAndSetIfChanged(ref _fileName, value);
        }

        public bool AutoUpdate
        {
            get => _autoUpdate;
            set
            {
                if (value)
                {
                    _autoupdateGameTimer.Change(0, Timeout.Infinite);
                }
                this.RaiseAndSetIfChanged(ref _autoUpdate, value);
            }
        }

        public double MinScrollViewerHeight => _padding * 2;

        public double MaxScrollViewerHeight =>_padding + _dotSpace * 4.5;

        public bool ShowLabels
        {
            get => _showLabels;
            set
            {
                this.RaiseAndSetIfChanged(ref _showLabels, value);
                Refresh();
            }
        }

        public GameTreeViewModel(UserControl gameTreeUserControl)
        {
            _autoupdateGameTimer = new Timer(UpdateGame, "timer", Timeout.Infinite, Timeout.Infinite);
            _gameTreeUserControl = gameTreeUserControl;
            _gameTreeCanvas = gameTreeUserControl.Find<Canvas>("GameTreeCanvas");
            _canvasScrollViewer = gameTreeUserControl.Find<ScrollViewer>("GameTreeScrollViewer");
            _gameTreeCanvas.PointerPressed += GameTreeCanvas_PointerPressed;
            FileName = ServiceLocator.Settings.OpenedFileName;
            if (string.IsNullOrEmpty(ServiceLocator.Settings.CurrentGameSgf))
            {
                GameInfo = new GameInfo() { Width = DotsFieldViewModel.Field.Width, Height = DotsFieldViewModel.Field.Height };
                UpdateSelectedGameTree(GameInfo.GameTree);
            }
            else
            {
                var serializer = new SgfParser();
                GameInfo = serializer.Parse(Encoding.UTF8.GetBytes(ServiceLocator.Settings.CurrentGameSgf));
                UpdateSelectedGameTree(GameInfo.GetDefaultLastTree());
                ScrollToSelectedGameTree();
            }

            PrevMoveCommand = ReactiveCommand.Create(() => PrevMoveCommandAction());

            NextMoveCommand = ReactiveCommand.Create(() =>
            {
                if (_selectedGameTree.Childs.Count != 0)
                {
                    UpdateSelectedGameTree(_selectedGameTree.Childs.First());
                    ScrollToSelectedGameTree();
                }
            });

            StartMoveCommand = ReactiveCommand.Create(() =>
            {
                UpdateSelectedGameTree(_gameInfo.GameTree);
                ScrollToSelectedGameTree();
            });

            EndMoveCommand = ReactiveCommand.Create(() =>
            {
                UpdateSelectedGameTree(_selectedGameTree.GetDefaultLastTree());
                ScrollToSelectedGameTree();
            });

            RemoveCommand = ReactiveCommand.Create(() =>
            {
                if (_selectedGameTree != null && _selectedGameTree.Parent != null)
                {
                    var parent = _selectedGameTree.Parent;
                    parent.Childs.Remove(_selectedGameTree);
                    Refresh();
                    UpdateSelectedGameTree(parent);
                    ScrollToSelectedGameTree();
                }
            });

            OpenFileCommand = ReactiveCommand.Create(async () =>
            {
                var dialog = new OpenFileDialog();
                dialog.Filters.Add(new FileDialogFilter() { Name = "Smart Game Format, PointsXT", Extensions = new List<string>() { "sgf", "sav" } });
                dialog.Filters.Add(new FileDialogFilter() { Name = "Smart Game Format", Extensions = new List<string>() { "sgf" } });
                dialog.Filters.Add(new FileDialogFilter() { Name = "PointsXT Save", Extensions = new List<string>() { "sav" } });
                string[] fileNames = await dialog.ShowAsync(ServiceLocator.MainWindow);
                if (fileNames != null) 
                {
                    var extractor = new GameInfoExtractor();
                    FileName = fileNames.First();
                    GameInfo = extractor.DetectFormatAndOpen(FileName);
                    DotsFieldViewModel.Field = new Field(GameInfo.Width, GameInfo.Height);
                    UpdateSelectedGameTree(GameInfo.GetDefaultLastTree());
                    ScrollToSelectedGameTree();
                }
            });

            OpenPlaydotsVkCommand = ReactiveCommand.Create(async () =>
            {
                var dialog = new OpenPlaydotsGame();
                var url = await dialog.ShowDialog<string>(ServiceLocator.MainWindow);
                if (url != null)
                {
                    var extractor = new GameInfoExtractor();
                    FileName = url;
                    GameInfo = extractor.DetectFormatAndOpen(FileName);
                    DotsFieldViewModel.Field = new Field(GameInfo.Width, GameInfo.Height);
                    UpdateSelectedGameTree(GameInfo.GetDefaultLastTree());
                    ScrollToSelectedGameTree();
                }
            });

            ResetCommand = ReactiveCommand.Create(() =>
            {
                FileName = "";
                GameInfoExtractor.InvalidateCache();
                GameInfo = new GameInfo();
                DotsFieldViewModel.Field = new Field(GameInfo.Width, GameInfo.Height);
                UpdateSelectedGameTree(GameInfo.GetDefaultLastTree());
                ScrollToSelectedGameTree();
            });

            SaveCommand = ReactiveCommand.Create(async () =>
            {
                var saveSgfDialog = new SaveFileDialog();
                saveSgfDialog.Filters.Add(new FileDialogFilter() { Name = "Smart Game Format", Extensions = new List<string>() { "sgf" } });
                string fileName = await saveSgfDialog.ShowAsync(null);
                if (fileName != null)
                {
                    var parser = new SgfParser { NewLines = true };
                    byte[] serialized = parser.Serialize(GameInfo);
                    File.WriteAllBytes(fileName, serialized);
                }
            });

            UpdateCommand = ReactiveCommand.Create(() => UpdateGame(null));
        }

        public void PrevMoveCommandAction()
        {
            if (_selectedGameTree.Parent != null)
            {
                UpdateSelectedGameTree(_selectedGameTree.Parent);
                ScrollToSelectedGameTree();
            }
        }

        private void UpdateGame(object state)
        {
            if (!string.IsNullOrEmpty(FileName))
            {
                var extractor = new GameInfoExtractor();
                var gameInfo = extractor.DetectFormatAndOpen(FileName, out bool fromCache);
                bool uiThread = !(state is string && (string)state == "timer");
                if (gameInfo != ServiceLocator.BasicCoreControViewModel.GameInfo)
                {
                    _gameInfo.CopyInfoFrom(gameInfo);
                    if (!uiThread)
                    {
                        Dispatcher.UIThread.InvokeAsync(() => ServiceLocator.BasicCoreControViewModel.GameInfo = gameInfo);
                    }
                    else
                    {
                        ServiceLocator.BasicCoreControViewModel.GameInfo = gameInfo;
                    }
                }
                if (!fromCache)
                {
                    IList<GameTree> gameMoves = gameInfo.GameTree.GetDefaultSequence();
                    if (gameMoves.Count != _prevMovesCount)
                    {
                        _prevMovesCount = gameMoves.Count;
                        if (!uiThread)
                        {
                            Dispatcher.UIThread.InvokeAsync(() => AddMoves(gameMoves));
                        }
                        else
                        {
                            AddMoves(gameMoves);
                        }
                    }
                }
            }
            if (_autoUpdate)
            {
                _autoupdateGameTimer.Change(1000, Timeout.Infinite);
            }
        }

        private void GameTreeCanvas_PointerPressed(object sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            Point position = e.GetPosition(_gameTreeCanvas);
            int xOffset = (int)Math.Round((position.X - _padding) / _dotSpace);
            int yOffset = (int)Math.Round((position.Y - _padding) / _dotSpace);
            if (_gameTreesOnCanvas != null &&
                yOffset >= 0 && yOffset < _gameTreesOnCanvas.GetLength(0) &&
                xOffset >= 0 && xOffset < _gameTreesOnCanvas.GetLength(1) &&
                Math.Abs(yOffset * _dotSpace + _padding - position.Y) <= _dotSize &&
                Math.Abs(xOffset * _dotSpace + _padding - position.X) <= _dotSize &&
                _gameTreesOnCanvas[yOffset, xOffset] != null)
            {
                UpdateSelectedGameTree(_gameTreesOnCanvas[yOffset, xOffset]);
            }
        }

        public GameInfo GameInfo
        {
            get => _gameInfo;
            set
            {
                ServiceLocator.BasicCoreControViewModel.GameInfo = value;
                _gameInfo = value;
                _previousSelectedGameTree = null;
                _selectedGameTree = null;
                DotsFieldViewModel.Player1Name = _gameInfo.Player1Name;
                DotsFieldViewModel.Player2Name = _gameInfo.Player2Name;
                Refresh();
            }
        }

        public void AddMove(GameMove move)
        {
            bool childExists = false;
            foreach (GameTree child in _selectedGameTree.Childs)
            {
                if (child.GameMoves.Count == 1 && move.Equals(child.GameMoves[0]))
                {
                    UpdateSelectedGameTree(child);
                    ScrollToSelectedGameTree();
                    childExists = true;
                    break;
                }
            }
            if (!childExists)
            {
                var newTree = new GameTree(_selectedGameTree, move);
                _selectedGameTree.Childs.Add(newTree);
                Refresh();

                _previousSelectedGameTree = _selectedGameTree;
                _selectedGameTree = newTree;

                RefreshSelectedTreeMarker();
                ScrollToSelectedGameTree();
            }
        }

        public void AddMoves(IList<GameTree> gameTrees)
        {
            GameTree node = _gameInfo.GameTree;
            bool newChild = false;
            foreach (GameTree gameTree in gameTrees)
            {
                if (gameTree.Root)
                {
                    continue;
                }
                if (!newChild)
                {
                    bool childExists = false;
                    foreach (GameTree child in node.Childs)
                    {
                        if (child.GameMoves.Count == 1 && gameTree.Move.Equals(child.GameMoves[0]))
                        {
                            node = child;
                            childExists = true;
                            break;
                        }
                    }
                    if (!childExists)
                    {
                        newChild = true;
                        var newTree = new GameTree(node, gameTree.Move);
                        node.Childs.Insert(0, newTree);
                        node = newTree;
                    }
                }
                else
                {
                    var newTree = new GameTree(node, gameTree.Move);
                    node.Childs.Add(newTree);
                    node = newTree;
                }
            }

            Refresh();
            UpdateSelectedGameTree(node);
            ScrollToSelectedGameTree();
        }

        private void Refresh()
        {
            int maxSequenceLength = _gameInfo.GameTree.GetMaxSequenceLength();
            int maxSequenceWidth = _gameInfo.GameTree.GetMaxSequenceWidth();
            _gameTreesOnCanvas = new GameTree[maxSequenceWidth, maxSequenceLength];
            _gameTreesPositions.Clear();
            double width = _padding + (maxSequenceLength - 1) * _dotSpace + _padding;
            double height = _padding + (maxSequenceWidth - 1) * _dotSpace + _padding;

            _gameTreeCanvas.Width = width;
            _gameTreeCanvas.Height = height;

            RefreshMoves();

            Dispatcher.UIThread.InvokeAsync(() => _canvasScrollViewer.InvalidateMeasure());
        }

        private void UpdateSelectedGameTree(GameTree selectedGameTree)
        {
            if (_selectedGameTree != selectedGameTree)
            {
                _previousSelectedGameTree = _selectedGameTree;
                _selectedGameTree = selectedGameTree;
                if (_previousSelectedGameTree == null)
                {
                    _previousSelectedGameTree = GameInfo.GameTree;
                    DotsFieldViewModel.MakeMoves(GameInfo.GameTree.GameMoves);
                }

                GameMovesDiff movesDiff = _previousSelectedGameTree.GetMovesDiff(_selectedGameTree);
                DotsFieldViewModel.UnmakeMoves(movesDiff.UnmakeMovesCount);
                DotsFieldViewModel.MakeMoves(movesDiff.MakeMoves);

                if (_selectedGameTree.Root)
                {
                    DotsFieldViewModel.ClearMoveMarker();
                }
            }
            RefreshSelectedTreeMarker();
        }

        private void ScrollToSelectedGameTree()
        {
            if (_selectedGameTree != null && _gameTreesPositions.TryGetValue(_selectedGameTree, out Tuple<int, int> pos))
            {
                double left = _padding + pos.Item1 * _dotSpace;
                double top = _padding + pos.Item2 * _dotSpace;
                Dispatcher.UIThread.InvokeAsync(() =>
                    _canvasScrollViewer.Offset = new Vector(
                        left - _canvasScrollViewer.DesiredSize.Width / 2, top - _canvasScrollViewer.DesiredSize.Height / 2));
            }
        }

        private void RefreshSelectedTreeMarker()
        {
            if (_gameTreesPositions.TryGetValue(_selectedGameTree, out Tuple<int, int> pos))
            {
                _gameTreeCanvas.Children.Remove(_selectedTreeRect);
                _selectedTreeRect = new Rectangle
                {
                    [Canvas.LeftProperty] = _padding + pos.Item1 * _dotSpace - _dotSize / 2 - 2,
                    [Canvas.TopProperty] = _padding + pos.Item2 * _dotSpace - _dotSize / 2 - 2,
                    Width = _dotSize + 4,
                    Height = _dotSize + 4,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2,
                };
                _gameTreeCanvas.Children.Add(_selectedTreeRect);
            }
        }

        private void RefreshMoves()
        {
            _newMoveCircles = new Dictionary<long, Ellipse>();
            _newTreeLines = new Dictionary<long, Line>();
            _newMoveLabels = new Dictionary<long, TextBlock>();

            RefreshMoves(_gameInfo.GameTree, 0, 0, 0);

            _gameTreeCanvas.Children.RemoveAll(_moveCircles.Select(moveCircle => moveCircle.Value));
            _gameTreeCanvas.Children.RemoveAll(_treeLines.Select(treeLine => treeLine.Value));
            _gameTreeCanvas.Children.RemoveAll(_moveLabels.Select(moveLabel => moveLabel.Value));
            _moveCircles = _newMoveCircles;
            _treeLines = _newTreeLines;
            _moveLabels = _newMoveLabels;
        }

        private int RefreshMoves(GameTree gameTree, int xOffset, int yOffset, int currentNumber)
        {
            _gameTreesOnCanvas[yOffset, xOffset] = gameTree;
            _gameTreesPositions[gameTree] = new Tuple<int, int>(xOffset, yOffset);
            DrawMove(gameTree, xOffset, yOffset, currentNumber);

            int maxSequenceWidth = 0;
            if (gameTree.Childs.Count == 0)
            {
                maxSequenceWidth = 1;
            }
            else
            {
                foreach (GameTree child in gameTree.Childs)
                {
                    DrawLine(yOffset, xOffset + 1, yOffset + maxSequenceWidth);
                    maxSequenceWidth += RefreshMoves(child, xOffset + 1, yOffset + maxSequenceWidth, currentNumber + 1);
                }
            }
            return maxSequenceWidth;
        }

        private void DrawMove(GameTree gameTree, int xOffset, int yOffset, int currentNumber)
        {
            int playerNumber = gameTree.Root ? 2 : gameTree.Move.PlayerNumber;
            double left = _padding + xOffset * _dotSpace - _dotSize / 2;
            double top = _padding + yOffset * _dotSpace - _dotSize / 2;
            long hash = GetMoveHashCode(xOffset, yOffset, playerNumber);
            if (_moveCircles.TryGetValue(hash, out Ellipse circle))
            {
                _moveCircles.Remove(hash);
            }
            else
            {
                IBrush fill;
                if (gameTree.Root)
                {
                    fill = Brushes.Green;
                }
                else
                {
                    fill = playerNumber == 0 ? Brushes.Blue : Brushes.Red;
                }
                circle = new Ellipse()
                {
                    [Canvas.LeftProperty] = left,
                    [Canvas.TopProperty] = top,
                    Width = _dotSize,
                    Height = _dotSize,
                    ZIndex = 10,
                    Fill = fill,
                };
                _gameTreeCanvas.Children.Add(circle);
            }
            _newMoveCircles[hash] = circle;

            if (_showLabels)
            {
                long textBlockHash = GetLabelHashCode(xOffset, yOffset, currentNumber);
                if (_moveLabels.TryGetValue(textBlockHash, out TextBlock textBlock))
                {
                    _moveLabels.Remove(textBlockHash);
                }
                else
                {
                    textBlock = new TextBlock()
                    {
                        [Canvas.LeftProperty] = left,
                        [Canvas.TopProperty] = top + 3,
                        Text = currentNumber.ToString(),
                        Width = _dotSize,
                        Height = _dotSize,
                        TextAlignment = TextAlignment.Center,
                        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                        Foreground = Brushes.White,
                        FontSize = 11,
                        ZIndex = 15
                    };
                    _gameTreeCanvas.Children.Add(textBlock);
                }
                _newMoveLabels[textBlockHash] = textBlock;
            }
        }

        private void DrawLine(int prevYOffset, int xOffset, int yOffset)
        {
            if (yOffset == prevYOffset)
            {
                // Draw horizontal line
                UpdateLine(xOffset - 1, yOffset, TreeLineDirection.Horizontal, 1);
            }
            else
            {
                if (yOffset - prevYOffset > 1)
                {
                    int newYOffset = prevYOffset;
                    for (int i = yOffset - 1; i > prevYOffset; i--)
                    {
                        if (_gameTreesOnCanvas[i, xOffset] != null)
                        {
                            newYOffset = i - 1;
                            break;
                        }
                    }
                    // Draw vertical line
                    UpdateLine(xOffset - 1, newYOffset, TreeLineDirection.Vertical, yOffset - 1 - newYOffset);
                }
                // Draw diagonal line
                UpdateLine(xOffset - 1, yOffset - 1, TreeLineDirection.Diagonal, 1);
            }
        }

        private void UpdateLine(int x, int y, TreeLineDirection dir, int length)
        {
            long hash = GetLineHashCode(x, y, dir, length);
            if (_treeLines.TryGetValue(hash, out Line line))
            {
                _treeLines.Remove(hash);
            }
            else
            {
                int x2 = x, y2 = y;
                for (int i = 0; i < length; i++)
                {
                    switch (dir)
                    {
                        case TreeLineDirection.Horizontal:
                            x2++;
                            break;
                        case TreeLineDirection.Vertical:
                            y2++;
                            break;
                        case TreeLineDirection.Diagonal:
                            x2++;
                            y2++;
                            break;
                    }
                }

                line = new Line
                {
                    StartPoint = new Point(_padding + x * _dotSpace, _padding + y * _dotSpace),
                    EndPoint = new Point(_padding + x2 * _dotSpace, _padding + y2 * _dotSpace),
                    Stroke = Brushes.Black,
                    ZIndex = 5
                };
                _gameTreeCanvas.Children.Add(line);
            }
            _newTreeLines[hash] = line;
        }

        private long GetMoveHashCode(int x, int y, int playerNumber)
        {
            return (((long)x * 4096 + y) * 4096 + playerNumber);
        }

        private long GetLineHashCode(int x, int y, TreeLineDirection lineDir, int length)
        {
            return ((((long)x * 4096) + y) * 4096 + (int)lineDir) * 4 + length;
        }

        private long GetLabelHashCode(int x, int y, int number)
        {
            return (((long)x * 4096 + y) * 4096 + number);
        }
    }
}
