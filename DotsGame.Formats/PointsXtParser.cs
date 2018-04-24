using System;
using System.Text;

namespace DotsGame.Formats
{
    public class PointsXtParser : IDotsGameFormatParser
    {
        public GameInfo Parse(byte[] data)
        {
            var result = new GameInfo
            {
                AppName = "PointsXT",
                GameType = GameType.Kropki,
                Width = 39,
                Height = 32
            };
            GameTree gameTree = null;
            GameTree rootGameTree = null;

            result.Player1Name = Encoding.Default.GetString(data, 11, 9).TrimEnd();
            result.Player2Name = Encoding.Default.GetString(data, 20, 9).TrimEnd();

            int playerNumber = 0;
            int currentNumber = 1;
            for (var i = 58; i < data.Length; i += 13)
            {
                var newGameTree = new GameTree { Number = currentNumber++ };
                if (rootGameTree == null)
                {
                    newGameTree.Parent = result.GameTree;
                    rootGameTree = newGameTree;
                }
                else
                {
                    newGameTree.Parent = gameTree;
                }
                newGameTree.AddMove(new GameMove(playerNumber, data[i + 1] + 1, data[i] + 1));
                if (gameTree != null)
                {
                    gameTree.Childs.Add(newGameTree);
                }
                gameTree = newGameTree;
                playerNumber = (playerNumber + 1) % 2;
            }
            result.GameTree.Childs.Add(rootGameTree);

            return result;
        }

        public byte[] Serialize(GameInfo gameInfo)
        {
            throw new NotImplementedException();
        }
    }
}
