namespace DotsGame.AI
{
    public class Estimator
    {
        #region Constructors

        public Estimator(Field field)
        {
            Field = field;
        }

        #endregion

        #region Properties

        public readonly Field Field;

        #endregion

        #region Public Methods

        public float Estimate(DotState player)
        {
            if (player == DotState.Player0)
                return (Field.Player0CaptureCount - Field.Player1CaptureCount) +
                       (Field.Player0Square - Field.Player1Square) * AiSettings.SquareCoef;
            else
                return (Field.Player1CaptureCount - Field.Player0CaptureCount) +
                       (Field.Player1Square - Field.Player0Square) * AiSettings.SquareCoef;
        }

        #endregion
    }
}
