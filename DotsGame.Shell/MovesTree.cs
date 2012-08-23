using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DotsGame.Shell
{
	/// <remarks>
	/// Type of label
	/// </remarks>
	public enum LabelType
	{
		Triangle,
		Square,
		Circle,
	}

	/// <remarks>
	/// Label for marking any positions on field
	/// </remarks>
	public class Label
	{
		[XmlAttribute]
		public LabelType Type;
		[XmlAttribute]
		public int X { get; set; }
		[XmlAttribute]
		public int Y { get; set; }

		public Label(int X, int Y, LabelType Type)
		{
			this.X = X;
			this.Y = Y;
			this.Type = Type;
		}
	}

	/// <remarks>
	/// Element of MovesTree. It is describe one move with/without labels
	/// </remarks>
	public class MovesTreeNode
	{
		[XmlAttribute]
		public readonly int X;
		[XmlAttribute]
		public readonly int Y;

		public readonly List<Label> Labels;
		public readonly MovesTreeNode Parent;
		public readonly List<MovesTreeNode> Childrens;

		#region Constructors

		public MovesTreeNode(MovesTreeNode parentNode, int x, int y, List<Label> labels = null)
		{
			Parent = parentNode;
			X = x;
			Y = y;
			if (labels != null && labels.Count == 0)
				labels = null;
			else
				Labels = labels;
			Childrens = new List<MovesTreeNode>();
		}

		#endregion
	}

	public delegate void MovesTreeHandler(object sender, EventArgs e);

	/// <remarks>
	/// Tree of all moves. Consists of MovesTreeNode
	/// </remarks>
	public class MovesTree
	{
		public MovesTreeHandler Draw;

		protected void OnDraw(EventArgs e)
		{
			if (Draw != null)
				Draw(this, e);
		}

		MovesTreeNode Root_;
		MovesTreeNode CurrentNode_;

		public MovesTree()
		{
			Root_ = null;
			CurrentNode_ = null;
		}

		public void Add(int x, int y)
		{
			if (Root_ == null)
			{
				Root_ = new MovesTreeNode(null, x, y);
				CurrentNode_ = Root_;
			}
			else
			{
				foreach (var node in CurrentNode_.Childrens)
					if (node.X == x && node.Y == y)
					{
						CurrentNode_ = node;
						return;
					}

				var newNode = new MovesTreeNode(CurrentNode_, x, y);
				CurrentNode_.Childrens.Add(newNode);
				CurrentNode_ = newNode;
			}
		}

		public void Remove()
		{
			if (CurrentNode_ != null)
				CurrentNode_ = CurrentNode_.Parent;
		}
	}
}
