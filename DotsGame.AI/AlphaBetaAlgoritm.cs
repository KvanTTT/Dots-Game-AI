namespace DotsGame.AI
{
    public class AlphaBetaAlgoritm
    {
        #region Constructors

        public AlphaBetaAlgoritm(Field field, MoveGenerator moveGenerator = null, Estimator estimator = null)
        {
            Field = field;
            MoveGenerator = moveGenerator ?? new StandartMoveGenerator(field);
            Estimator = estimator ?? new Estimator(field);
        }

        #endregion

        #region Public Methods

        public int SearchBestMove(byte depth = 4)
        {
            return SearchBestMove(depth, Field.CurrentPlayer, -AiSettings.InfinityScore, AiSettings.InfinityScore);
        }

        public int SearchBestMove(byte depth, DotState player, float alpha, float beta)
        {
            int bestMove = 0;

            CalculatedPositionCount = 0;

            MoveGenerator.MaxDepth = depth;
            MoveGenerator.GenerateMoves(player, depth);
            DotState nextPlayer = player.NextPlayer();

            foreach (var move in MoveGenerator.Moves)
            {
                if (alpha < beta)
                {
                    if (Field.MakeMove(move))
                    {
                        CalculatedPositionCount++;
                        MoveGenerator.UpdateMoves();
                        float tmp = -EvaluatePosition((byte)(depth - 1), nextPlayer, -beta, -alpha);
                        Field.UnmakeMove();
                        MoveGenerator.UpdateMoves();
                        if (tmp > alpha)
                        {
                            alpha = tmp;
                            bestMove = move;
                        }
                    }
                }
            }

            return bestMove;
        }

        #endregion

        #region Helpers

        private float EvaluatePosition(byte depth, DotState player, float alpha, float beta)
        {
            if (depth == 0)
                return Estimator.Estimate(player);

            MoveGenerator.GenerateMoves(player, depth);
            DotState nextPlayer = player.NextPlayer();

            foreach (var move in MoveGenerator.Moves)
            {
                if (alpha < beta)
                {
                    if (Field.MakeMove(move))
                    {
                        CalculatedPositionCount++;
                        MoveGenerator.UpdateMoves();
                        float tmp = -EvaluatePosition((byte)(depth - 1), nextPlayer, -beta, -alpha);
                        Field.UnmakeMove();
                        MoveGenerator.UpdateMoves();
                        if (tmp > alpha)
                            alpha = tmp;
                    }
                }
            }

            return alpha;
        }

        #endregion

        #region Properties

        public Field Field
        {
            get;
            set;
        }

        public MoveGenerator MoveGenerator
        {
            get;
            set;
        }

        public Estimator Estimator
        {
            get;
            set;
        }

        public long CalculatedPositionCount
        {
            get;
            private set;
        }

        #endregion
    }
}
