using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using DotsGame.Shell.Properties;
using DotsGame.AI;
using DotsGame;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;

namespace DotsGame.Shell
{
	/// <summary>
	/// Interaction logic for DotsGameControl.xaml
	/// </summary>
	public partial class DotsGameControl : UserControl, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		double CellSize;
		double CellSizeDiv2;
		double Radius;

		Brush Player1Fill = new SolidColorBrush(Color.FromArgb(127, 255, 0, 0));
		Brush Player2Fill = new SolidColorBrush(Color.FromArgb(127, 0, 0, 255));
		Brush Player1Stroke = Brushes.Red;
		Brush Player2Stroke = Brushes.Blue;
		Brush Background_ = Brushes.White;
		Brush Line = Brushes.Black;
		Brush CrosswiseStroke = Brushes.SpringGreen;
		double PointCellRatio = 0.23;
		double LineCellRatio = 0.14;
        
		int FieldWidth, FieldHeight;
		GameField GameField;
		Field Field;
		ZobristHashField HashField;

		private Dictionary<int, List<Shape>> OneMoveShapes = new Dictionary<int, List<Shape>>();
		private List<Polygon> GroupPolygons = new List<Polygon>();
		private List<Polygon> CrosswisePolygons = new List<Polygon>();

		byte Depth;

		private bool InputEnabled_ = true;
		private bool InputEnabled
		{
			get
			{
				return InputEnabled_;
			}
			set
			{
				InputEnabled_ = value;
				if (PropertyChanged != null)
					PropertyChanged(this, new PropertyChangedEventArgs("InputEnabled")); 
			}
		}

		Thread SearchingThread;

		#region Constructors

		public DotsGameControl(int FieldWidth, int FieldHeight)
		{
			this.DataContext = this;
			InitializeComponent();
			
			//PropertyChanged += new PropertyChangedEventHandler((sender, e) => 
			//	PropertyChanged(this, new PropertyChangedEventArgs("InputEnabled")));

			this.FieldWidth = FieldWidth;
			this.FieldHeight = FieldHeight;

			Field = new FieldWithGroups(FieldWidth, FieldHeight, enmSurroundCondition.Standart);
			HashField = new ZobristHashField(Field, 0);
			GameField = new GameField(Field, enmBeginPattern.Clean);
			GameField.Move += OnMakeMove;

			CellSize = 647 / Field.Width;
			CellSizeDiv2 = CellSize / 2;
		}

		#endregion

		#region Event Handlers

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			RedrawField();

			if (!string.IsNullOrEmpty(Settings.Default.LoadedFileName))
				LoadGame(Settings.Default.LoadedFileName);
		}

		private void OnMakeMove(object sender, MoveEventArgs e)
		{
			if (e.Action == enmMoveState.Add)
			{
				HashField.UpdateHash();
				DrawState(Field.States.Last());
			}
			else
				if (e.Action == enmMoveState.Remove)
				{
					HashField.UpdateHash();
					foreach (var shape in OneMoveShapes[e.Pos])
						canvasField.Children.Remove(shape);
					OneMoveShapes.Remove(e.Pos);
				}
			lblDotsCount.Content = Field.DotsSequenceCount.ToString();
			lblRedCaptureCount.Content = Field.Player0CaptureCount.ToString() + "," + Field.Player0Square.ToString();
			lblBlueCaptureCount.Content = Field.Player1CaptureCount.ToString() + "," + Field.Player1Square.ToString();
			tbHash.Text = HashField.Key.ToString();
		}

		private void canvasField_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (InputEnabled)
			{
				int x, y;
				GetFieldPoint(e.GetPosition(canvasField), out x, out y);
				GameField.MakeMove(x, y);
			}
		}

		private void canvasField_MouseMove(object sender, MouseEventArgs e)
		{
			int x, y, pos;
			GetFieldPoint(e.GetPosition(canvasField), out x, out y);
			pos = Field.GetPosition(x, y);
			lblX.Content = x;
			lblY.Content = y;
			lblPos.Content = pos;
			lblDiagGroup.Content = (int)Field[pos].GetDiagGroupNumber() >> (int)DotState.DiagonalGroupMaskShift;
		}

		private void btnUnmakeMove_Click(object sender, RoutedEventArgs e)
		{
			GameField.UnmakeMove();
		}

		private void sliderMain_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (GameField != null)
				GameField.SetMoveNumber((int)Math.Round(e.NewValue));
		}

		private void btnLoadGame_Click(object sender, RoutedEventArgs e)
		{
			using (var ofd = new System.Windows.Forms.OpenFileDialog() { Filter = "pointsxt files|*.sav" })
				if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					LoadGame(ofd.FileName);
		}

		private void btnClear_Click(object sender, RoutedEventArgs e)
		{
			Field = new FieldWithGroups(FieldWidth, FieldHeight, enmSurroundCondition.Standart);
			GameField = new GameField(Field, enmBeginPattern.Clean);
			GameField.Move += OnMakeMove;
			RedrawField();
		}

		private void cbConvexGroups_Checked(object sender, RoutedEventArgs e)
		{
			var analyzer = new StrategicMovesAnalyzer(Field);
			analyzer.GenerateGroups();

			foreach (var group in analyzer.Groups)
			{
				var polygon = new Polygon { Stretch = Stretch.None };
				polygon.Stroke = group.Player == DotState.Player0 ? Player1Stroke : Player2Stroke;
				polygon.Fill = group.Player == DotState.Player0 ? Player1Fill : Player2Fill;
				polygon.StrokeMiterLimit = 1;
				foreach (var pos in group.EnvelopePositions)
					polygon.Points.Add(GetGraphicaPoint(pos));
				GroupPolygons.Add(polygon);
				canvasField.Children.Add(polygon);
			}
		}

		private void cbConvexGroups_Unchecked(object sender, RoutedEventArgs e)
		{
			foreach (var polygon in GroupPolygons)
				canvasField.Children.Remove(polygon);
			GroupPolygons.Clear();
		}

		private void cbCrosswises_Checked(object sender, RoutedEventArgs e)
		{
			var analyzer = new StrategicMovesAnalyzer(Field);
			analyzer.GenerateGroups();
			analyzer.FindCrosswises();

			foreach (var crosswise in analyzer.Crosswises)
			{
				var polygon = new Polygon { Stretch = Stretch.None };
				polygon.Stroke = CrosswiseStroke;
				polygon.Fill = null;
				int x, y;
				Field.GetPosition(crosswise.Value.Position, out x, out y);

				if (crosswise.Value.Orientation == enmCrosswiseOrientation.BottomRight)
				{
					polygon.Points.Add(GetGraphicalPoint(x, y));
					polygon.Points.Add(GetGraphicalPoint(x + 1, y));
					polygon.Points.Add(GetGraphicalPoint(x + 1, y + 1));
					polygon.Points.Add(GetGraphicalPoint(x, y + 1));
				}
				else
					if (crosswise.Value.Orientation == enmCrosswiseOrientation.BottomLeft)
					{
						polygon.Points.Add(GetGraphicalPoint(x, y));
						polygon.Points.Add(GetGraphicalPoint(x - 1, y));
						polygon.Points.Add(GetGraphicalPoint(x - 1, y + 1));
						polygon.Points.Add(GetGraphicalPoint(x, y + 1));
					}

				CrosswisePolygons.Add(polygon);
				canvasField.Children.Add(polygon);
			}
		}

		private void cbCrosswises_Unchecked(object sender, RoutedEventArgs e)
		{
			foreach (var polygon in CrosswisePolygons)
				canvasField.Children.Remove(polygon);
			CrosswisePolygons.Clear();
		}

		private void cbWalls_Checked(object sender, RoutedEventArgs e)
		{
			foreach (var pair in OneMoveShapes)
				foreach (var shape in pair.Value)
					if (shape is Line)
						shape.Visibility = System.Windows.Visibility.Visible;
		}

		private void cbWalls_Unchecked(object sender, RoutedEventArgs e)
		{
			foreach (var pair in OneMoveShapes)
				foreach (var shape in pair.Value)
					if (shape is Line)
						shape.Visibility = System.Windows.Visibility.Hidden;
		}

		private bool AlphaBetaHash;
		private Stopwatch stopwatch;

		private void btnAlphaBetaSolve_Click(object sender, RoutedEventArgs e)
		{
			if (InputEnabled)
			{
				InputEnabled = false;
				Depth = byte.Parse(tbDepth.Text);
				AlphaBetaHash = (string)((Button)sender).Content == "AB" ? false : true;
				SearchingThread = new Thread(new ThreadStart(AlphaBetaSearch));
				stopwatch = new Stopwatch();
				stopwatch.Start();
				SearchingThread.Start();

				((Button)sender).Content = "Stop";
			}
			else
			{
				SearchingThread.Abort();
				InputEnabled = true;

				if (!AlphaBetaHash)
					btnAlphaBetaSolve.Content = "AB";
				else
					btnAlphaBetaHashSolve.Content = "AB Hash";
			}
		}

		private void AlphaBetaSearch()
		{
			var tempField = Field.Clone();

			AlphaBetaAlgoritm algorithm = null;
			AlphaBetaHashAlgoritm hashAlgorithm = null;
			int pos = 0;
			if (!AlphaBetaHash)
			{
				algorithm = new AlphaBetaAlgoritm(tempField);
				pos = algorithm.SearchBestMove(Depth);
			}
			else
			{
				hashAlgorithm = new AlphaBetaHashAlgoritm(tempField);
				pos = hashAlgorithm.SearchBestMove(Depth);
			}
			
			stopwatch.Stop();
			int x, y;
			Field.GetPosition(pos, out x, out y);
			double time = (double)stopwatch.ElapsedMilliseconds / 1000;

			this.Dispatcher.Invoke(new Action(() => { 
				GameField.MakeMove(x, y);
				if (!AlphaBetaHash)
				{
					btnAlphaBetaSolve.Content = "AB";
					lblAlphaBetaTime.Text = string.Format(
						"t:{0:0.000},c:{1},s:{2:0.00}", 
						time, 
						algorithm.CalculatedPositionCount,
						(double)algorithm.CalculatedPositionCount / time / 1000);
				}
				else
				{
					
					btnAlphaBetaHashSolve.Content = "AB Hash";
					lblAlphaBetaHashTime.Text = string.Format(
						"t:{0:0.000},c:{1},s:{2:0.00}", time, hashAlgorithm.CalculatedPositionCount,
							(double)hashAlgorithm.CalculatedPositionCount / time / 1000);
				}
				InputEnabled = true; }));
		}

		private void btnClearHash_Click(object sender, RoutedEventArgs e)
		{
			HashField = new ZobristHashField(Field, 0);
			tbHash.Text = HashField.Key.ToString();
		}

		#endregion
		
		#region Helpers

		private void LoadGame(string fileName)
		{
			Field = new FieldWithGroups(39, 32);
			GameField = new GameField(Field, enmBeginPattern.Crosswise);
			HashField = new ZobristHashField(Field, 0);

			RedrawField();
			GameField.Move += OnMakeMove;
			using (var Stream = new StreamReader(fileName))
			{
				var buffer = new byte[Stream.BaseStream.Length];
				var Count = (int)Stream.BaseStream.Length;
				Stream.BaseStream.Read(buffer, 0, Count);
				Stream.Close();

				for (var i = 58; i < Count; i += 13)
					GameField.MakeMove(buffer[i] + 1, buffer[i + 1] + 1);
				sliderMain.ValueChanged -= sliderMain_ValueChanged;
				sliderMain.SmallChange = 1;
				sliderMain.Minimum = 1;
				sliderMain.Maximum = Field.States.Count();
				sliderMain.Value = sliderMain.Maximum;
				sliderMain.ValueChanged += sliderMain_ValueChanged;
			}
			Settings.Default.LoadedFileName = fileName;
			Settings.Default.Save();
		}

		private void DrawState(State State)
		{
			var point = GetGraphicaPoint(State.Move.Position);
			var pos = State.Move.Position;

			var shapeList = new List<Shape>();

			Ellipse E = new Ellipse
			{
				Fill = Field[State.Move.Position].IsRealPlayer0() ? Player1Stroke : Player2Stroke,
				Width = Radius * 2,
				Height = Radius * 2,
				Stretch = Stretch.Uniform
			};
			Canvas.SetLeft(E, point.X - Radius);
			Canvas.SetTop(E, point.Y - Radius);

			shapeList.Add(E);
			canvasField.Children.Add(E);

			Polygon polygon;
			if (State.Base != null && Field.LastMoveCaptureCount != 0)
			{
				var chainPositions = State.Base.ChainPositions;
				var GraphicsPoints = new PointCollection(chainPositions.Count);
				foreach (var PointPos in chainPositions)
					GraphicsPoints.Add(GetGraphicaPoint(PointPos));

				polygon = new Polygon { StrokeThickness = LineCellRatio * CellSize, Points = GraphicsPoints, Stretch = Stretch.None };
				if (Field[chainPositions.Last()].IsPlayer0Putted())
				{
					polygon.Fill = Player1Fill;
					polygon.Stroke = Player1Stroke;
				}
				else
				{
					polygon.Fill = Player2Fill;
					polygon.Stroke = Player2Stroke;
				}

				shapeList.Add(polygon);
				canvasField.Children.Add(polygon);
			}

			var player = Field.CurrentPlayer.NextPlayer();
			Brush stroke;
			if (Field[State.Move.Position].IsPlayer0Putted())
				stroke = Player1Stroke;
			else
				stroke = Player2Stroke;

			var beginPoint = GetGraphicaPoint(pos);
			double x2 = beginPoint.X;
			double y2 = beginPoint.Y;
			if (Field[pos - 1].IsPlayerPutted(player) && !Field[pos - 1].IsSurrounded())
				AddLineToWalls(beginPoint, GetGraphicaPoint(pos - 1), stroke, shapeList);
			if (Field[pos - Field.RealWidth].IsPlayerPutted(player) && !Field[pos - Field.RealWidth].IsSurrounded())
				AddLineToWalls(beginPoint, GetGraphicaPoint(pos - Field.RealWidth), stroke, shapeList);
			if (Field[pos + 1].IsPlayerPutted(player) && !Field[pos + 1].IsSurrounded())
				AddLineToWalls(beginPoint, GetGraphicaPoint(pos + 1), stroke, shapeList);
			if (Field[pos + Field.RealWidth].IsPlayerPutted(player) && !Field[pos + Field.RealWidth].IsSurrounded())
				AddLineToWalls(beginPoint, GetGraphicaPoint(pos + Field.RealWidth), stroke, shapeList);

			OneMoveShapes.Add(pos, shapeList);
		}

		private void AddLineToWalls(Point p1, Point p2, Brush stroke, List<Shape> shapeList)
		{
			var line = new Line() { StrokeThickness = LineCellRatio * CellSize, Stroke = stroke, Stretch = Stretch.None };
			line.X1 = p1.X;
			line.Y1 = p1.Y;
			line.X2 = p2.X;
			line.Y2 = p2.Y;
			shapeList.Add(line);
			canvasField.Children.Add(line);
		}

		public void RedrawField()
		{
			OneMoveShapes.Clear();
			GroupPolygons.Clear();
			CrosswisePolygons.Clear();

			if (canvasField.RenderSize.Width == 0)
				return;

			CellSize = canvasField.RenderSize.Width / Field.Width;
			CellSizeDiv2 = CellSize / 2.0;
			Radius = CellSize * PointCellRatio;

			canvasField.Height = CellSize * Field.Height;

			double a = CellSizeDiv2;
			canvasField.Background = Background_;
			canvasField.Children.Clear();

			double LineLength = CellSize * (Field.Height + 1);
			for (int i = 0; i < Field.Width; i++)
			{
				Line L = new Line();
				L.X1 = L.X2 = a;
				L.Y1 = 0;
				L.Y2 = LineLength;
				L.Stroke = Line;
				L.StrokeThickness = 0.25;
				canvasField.Children.Add(L);
				a += CellSize;
			}

			LineLength = CellSize * (Field.Width + 1);
			a = CellSizeDiv2;
			for (int i = 0; i < Field.Height; i++)
			{
				Line L = new Line();
				L.Y1 = L.Y2 = a;
				L.X1 = 0;
				L.X2 = LineLength;
				L.Stroke = Line;
				L.StrokeThickness = 0.25;
				canvasField.Children.Add(L);
				a += CellSize;
			}

			foreach (var P in Field.States)
				DrawState(P);
		}

		private Point GetGraphicaPoint(int pos)
		{
			int x, y;
			Field.GetPosition(pos, out x, out y);
			return GetGraphicalPoint(x, y);
		}

		private Point GetGraphicalPoint(int x, int y)
		{
			return new Point((x - 1) * CellSize + CellSizeDiv2, (y - 1) * CellSize + CellSizeDiv2);
		}

		private void GetFieldPoint(Point p, out int x, out int y)
		{
			x = (int)Math.Round((p.X - CellSizeDiv2) / CellSize) + 1;
			y = (int)Math.Round((p.Y - CellSizeDiv2) / CellSize) + 1;
		}

		#endregion
	}
}
