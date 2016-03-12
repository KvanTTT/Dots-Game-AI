﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotsGame
{
    /// <remarks>
    /// Struct for field changes on each move
    /// if surround on Move then Base != null
    /// </remarks>
    public class State
    {
        #region Properties

        public Base Base
        {
            get;
            set;
        }

        public DotPosition Move
        {
            get;
            set;
        }

        public int DiagonalGroupCount
        {
            get;
            set;
        }

        #endregion

        #region Constructors

        public State()
        {
        }

        public State(DotPosition dotPosition, Base b, int diagonalGroupCount = 0) : this()
        {
            Base = b;
            Move = dotPosition;
            DiagonalGroupCount = diagonalGroupCount;
        }

        #endregion

        public bool HasBase()
        {
            return Base != null;
        }

        public override string ToString()
        {
            return Move.ToString() + (Base == null ? string.Empty : "; " + Base.ToString());
        }

        public State Clone()
        {
            var result = new State();
            if (Base != null)
            {
                result.Base = new Base(Base.LastCaptureCount, Base.LastFreedCount,
                    new List<DotPosition>(Base.ChainDotPositions), new List<DotPosition>(Base.SurrroundDotPositions),
                    new List<short>(Base.ChainPositions), new List<short>(Base.SurroundPositions), Base.Player0Square, Base.Player1Square);
            }
            result.Move = Move;
            result.DiagonalGroupCount = DiagonalGroupCount;
            return result;
        }
    }
}
