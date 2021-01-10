using System;
using System.Security.Cryptography;

namespace DotsGame.AI
{
    /// <summary>
    /// TODO: take in calculation Current Red and Blue Capture Count.
    /// </summary>
    public class ZobristHashField
    {
        #region Fields

        private ulong[] HashTable_;
        private int CaptureCountOffset_;

        #endregion

        #region Constructors

        public ZobristHashField(Field field, int? seed = null,
            RandomGenerateMethod randomGenerateMethod = RandomGenerateMethod.Standart)
        {
            Field = field;
            Seed = seed;
            RandomGenerateMethod = randomGenerateMethod;

            CaptureCountOffset_ = field.RealDotsCount * 2;
            HashTable_ = new ulong[CaptureCountOffset_ + Field.RealWidth * 2];
            Key ^= HashTable_[CaptureCountOffset_ + Field.Player0CaptureCount % Field.RealWidth];
            Key ^= HashTable_[CaptureCountOffset_ + Field.RealWidth + Field.Player1CaptureCount % Field.RealWidth];
            FillWithRandomValues();
            Key = 0;
        }

        #endregion

        #region Public Methods

        public void UpdateHash()
        {
            if (Field.LastMoveState == MoveState.Add)
            {
                int pos = Field.LastPosition;
                if (Field.CurrentPlayer == DotState.Player0)
                    pos *= 2;
                Key ^= HashTable_[pos];

                if (Field.LastMoveCaptureCount != 0)
                    UpdateLastBaseHash();
            }
            else
            {
                if (Field.LastMoveCaptureCount != 0)
                    UpdateLastBaseHash();

                int pos = Field.LastPosition;
                if (Field.CurrentPlayer == DotState.Player1)
                    pos *= 2;
                Key ^= HashTable_[pos];
            }
        }

        #endregion

        #region Helpers

        private void FillWithRandomValues()
        {
            if (RandomGenerateMethod == RandomGenerateMethod.Standart)
            {
                var random = Seed.HasValue ? new Random((int)Seed) : new Random();
                var buffer = new byte[sizeof(ulong)];
                for (int i = 0; i < HashTable_.Length; i++)
                {
                    random.NextBytes(buffer);
                    HashTable_[i] = BitConverter.ToUInt64(buffer, 0);
                }
            }
            else
                if (RandomGenerateMethod == RandomGenerateMethod.Crypto)
            {
                // TODO: Understand how to generate sequence with define seed.
                using (var generator = new RNGCryptoServiceProvider())
                {
                    var bytes = new byte[sizeof(ulong)];
                    for (int i = 0; i < HashTable_.Length; i++)
                    {
                        generator.GetBytes(bytes);
                        HashTable_[i] = BitConverter.ToUInt64(bytes, 0);
                    }
                }
            }
        }

        /// <summary>
        /// TODO: Try to optimize it.
        /// </summary>
        private void UpdateLastBaseHash()
        {
            bool isRed = !((Field.CurrentPlayer == DotState.Player0 && Field.LastMoveState == MoveState.Add)
                || (Field.CurrentPlayer == DotState.Player1 && Field.LastMoveState == MoveState.Remove)) ? true : false;
            if (Field.LastMoveCaptureCount < 0)
                isRed = !isRed;
            if (isRed)
                foreach (var surroundPos in Field.SurroundPositions)
                {
                    if ((Field[surroundPos].IsOneSurroundLevel() && Field.LastMoveState == MoveState.Add) ||
                         Field[surroundPos].IsZeroSurroundLevel() && Field.LastMoveState == MoveState.Remove)
                    {
                        if (Field[surroundPos].IsRealPutted())
                        {
                            if (Field[surroundPos].IsRealPlayer1())
                            {
                                Key ^= HashTable_[surroundPos * 2];
                                Key ^= HashTable_[surroundPos];
                            }
                        }
                        else
                        {
                            if (Field.LastMoveCaptureCount < 0 && Field.LastMoveState == MoveState.Remove &&
                                Field.LastPosition == surroundPos)
                                Key ^= HashTable_[surroundPos * 2];
                            Key ^= HashTable_[surroundPos];
                        }
                    }
                    else if ((Field[surroundPos].IsMoreThanOneSurroundLevel() && Field.LastMoveState == MoveState.Add) ||
                            (Field[surroundPos].IsOneSurroundLevel() && Field.LastMoveState == MoveState.Remove))
                    {
                        Key ^= HashTable_[surroundPos * 2];
                        Key ^= HashTable_[surroundPos];
                    }
                }
            else
                foreach (var surroundPos in Field.SurroundPositions)
                {
                    if ((Field[surroundPos].IsOneSurroundLevel() && Field.LastMoveState == MoveState.Add) ||
                        (Field[surroundPos].IsZeroSurroundLevel() && Field.LastMoveState == MoveState.Remove))
                    {
                        if (Field[surroundPos].IsRealPutted())
                        {
                            if (Field[surroundPos].IsRealPlayer0())
                            {
                                Key ^= HashTable_[surroundPos];
                                Key ^= HashTable_[surroundPos * 2];
                            }
                        }
                        else
                        {
                            if (Field.LastMoveCaptureCount < 0 && Field.LastMoveState == MoveState.Remove &&
                                   Field.LastPosition == surroundPos)
                            {
                                Key ^= HashTable_[surroundPos];
                            }
                            Key ^= HashTable_[surroundPos * 2];
                        }
                    }
                    else if ((Field[surroundPos].IsMoreThanOneSurroundLevel() && Field.LastMoveState == MoveState.Add) ||
                            (Field[surroundPos].IsOneSurroundLevel() && Field.LastMoveState == MoveState.Remove))
                    {
                        Key ^= HashTable_[surroundPos];
                        Key ^= HashTable_[surroundPos * 2];
                    }
                }

            Key ^= HashTable_[CaptureCountOffset_ + Field.OldPlayer0CaptureCount % Field.RealWidth];
            Key ^= HashTable_[CaptureCountOffset_ + Field.Player0CaptureCount % Field.RealWidth];
            Key ^= HashTable_[CaptureCountOffset_ + Field.RealWidth + Field.OldPlayer1CaptureCount % Field.RealWidth];
            Key ^= HashTable_[CaptureCountOffset_ + Field.RealWidth + Field.Player1CaptureCount % Field.RealWidth];
        }

        #endregion

        #region Properties

        public Field Field
        {
            get;
            set;
        }

        public RandomGenerateMethod RandomGenerateMethod
        {
            get;
            private set;
        }

        public int? Seed
        {
            get;
            private set;
        }

        public ulong Key
        {
            get;
            private set;
        }

        #endregion
    }
}
