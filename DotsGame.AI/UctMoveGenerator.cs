using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotsGame.AI
{
	public class UctNode
	{
		public int Wins
		{
			get;
			set;
		}

		public int Visits
		{
			get;
			set;
		}

		public int Pos
		{
			get;
			set;
		}

		public UctNode Child
		{
			get;
			set;
		}

		public UctNode Sibling
		{
			get;
			set;
		}

		public UctNode(int pos = 0)
		{
			Pos = pos;
		}

		public void Update(int val) {
			Visits++;
			Wins += val;
		}

		public double GetWinRate() {
			if (Visits > 0) 
				return (double)Wins / Visits;
			else
				return 0;
		}
	}

	public class UctMoveGenerator : CMoveGenerator
	{
		private Random _random;
		public UctNode root = null;
		public const double UCTK = 0.44; // 0.44 = sqrt(1/5)
		// Larger values give uniform search
		// Smaller values give very selective search

		public UctMoveGenerator(Field field)
			: base(field)
		{
			_random = new Random();
		}

		public UctNode UCTSelect(UctNode UctNode)
		{
			UctNode res = null;
			UctNode next = UctNode.Child;
			double best_uct = 0;
			while (next != null)
			{ // for all children
				double uctvalue;
				if (next.Visits > 0)
				{
					double winrate = next.GetWinRate();
					double uct = UCTK * Math.Sqrt(Math.Log(UctNode.Visits) / next.Visits);
					uctvalue = winrate + uct;
				}
				else
				{
					// Always play a random unexplored move first
					uctvalue = 10000 + 1000 * _random.NextDouble();
				}
				if (uctvalue > best_uct)
				{ // get max uctvalue of all children
					best_uct = uctvalue;
					res = next;
				}
				next = next.Sibling;
			}
			return res;
		}

		// generate a move, using the uct algorithm
		private int UCTSearch(int numsim)
		{
			root = new UctNode(0); //init uct tree
			CreateChildren(Field, root);
			Field clone;
			for (int i = 0; i < numsim; i++)
			{
				clone = Field.Clone();
				PlaySimulation(clone, root);
			}
			UctNode n = GetBestChild(root);
			return n.Pos;
		}

		// expand children in UctNode
		private void CreateChildren(Field field, UctNode parent)
		{
			UctNode last = parent;
			for (int i = 1; i <= field.Width; i++)
				for (int j = 1; j <= field.Height; j++)
				{
					var pos = Field.GetPosition(i, j);
					if (field[pos].IsPuttingAllowed())
					{
						UctNode UctNode = new UctNode(pos);
						if (last == parent)
							last.Child = UctNode;
						else
							last.Sibling = UctNode;
						last = UctNode;
					}
				}
		}

		// return 0=lose 1=win for current player to move
		private int PlaySimulation(Field field, UctNode n)
		{
			int randomresult = 0;
			if (n.Child == null && n.Visits < 10)
			{ // 10 simulations until chilren are expanded (saves memory)
				randomresult = PlayRandomGame(field);
			}
			else
			{
				if (n.Child == null)
					CreateChildren(field, n);
				UctNode next = UCTSelect(n); // select a move
				if (next == null) { /* ERROR */ }
				field.MakeMove(next.Pos);
				int res = PlaySimulation(field, next);
				randomresult = 1 - res;
			}
			n.Update(1 - randomresult); //update UctNode (UctNode-wins are associated with moves in the Nodes)
			return randomresult;
		}

		public UctNode GetBestChild(UctNode root)
		{
			UctNode Child = root.Child;
			UctNode best_child = null;
			int best_visits = -1;
			while (Child != null)
			{ // for all children
				if (Child.Visits > best_visits)
				{
					best_child = Child;
					best_visits = Child.Visits;
				}
				Child = Child.Sibling;
			}
			return best_child;
		}

		// return 0=lose 1=win for current player to move
		private int PlayRandomGame(Field field)
		{
			throw new NotImplementedException();
			/*int cur_player1 = cur_player;
			while (!isGameOver())
			{
				MakeRandomMove(field);
			}
			return getWinner() == curplayer1 ? 1 : 0;*/
		}

		public void MakeRandomMove(Field field)
		{
			int x = 0;
			int y = 0;
			while (true)
			{
				x = _random.Next(field.Width) + 1;
				y = _random.Next(field.Height) + 1;
				if (field[Field.GetPosition(x, y)].IsPuttingAllowed())
					break;
			}
			field.MakeMove(x, y);
		}

		public override void GenerateMoves(Dot player, int depth = 0)
		{
			throw new NotImplementedException();
		}

		public override void UpdateMoves()
		{
			throw new NotImplementedException();
		}
	}
}
