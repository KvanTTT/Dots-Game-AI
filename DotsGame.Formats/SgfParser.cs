using DotsGame.Formats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotsGame.Sgf
{
    public class SgfParser : IDotsGameFormatParser
    {
        private const string Indent = "  ";

        private GameInfo _gameInfo;
        private int _currentDataPos;
        private GameTree _oldGameTree, _currentGameTree;
        private int _commentNumber;
        private byte[] _data;
        private Encoding _encoding = Encoding.UTF8;
        private int _fileFormat = 4;

        public bool NewLines { get; set; }

        public GameInfo Parse(byte[] data)
        {
            _gameInfo = new GameInfo();
            _currentDataPos = 0;
            _oldGameTree = _currentGameTree = null;
            _commentNumber = 0;
            _data = data;

            SkipSpaces();
            bool atLeastOneGameTreeShouldBeSpecified = false;
            while (Accept('('))
            {
                GameTree childGameTree = ParseGameTree(out bool ignoreParent);
                if (ignoreParent)
                {
                    foreach (var tree in childGameTree.Childs)
                    {
                        _gameInfo.GameTree.Childs.Add(tree);
                        tree.Parent = _gameInfo.GameTree;
                    }
                }
                else
                {
                    _gameInfo.GameTree.Childs.Add(childGameTree);
                    childGameTree.Parent = _gameInfo.GameTree;
                }
                Expect(')');
                SkipSpaces();
                atLeastOneGameTreeShouldBeSpecified = true;
            }
            if (!atLeastOneGameTreeShouldBeSpecified)
            {
                throw new Exception("At least one game tree should be specified");
            }

            return _gameInfo;
        }

        public byte[] Serialize(GameInfo gameInfo)
        {
            var builder = new StringBuilder("(;");
            builder.Append("AP[" + gameInfo.AppName + "]");
            builder.Append("GM[" + (int)gameInfo.GameType + "]");
            builder.Append("FF[" + 4 + "]");
            builder.Append("CA[" + Encoding.UTF8.HeaderName.ToUpperInvariant() + "]");
            builder.Append("SZ[" + gameInfo.Width + (gameInfo.Width == gameInfo.Height ? "" : ":" + gameInfo.Height)  + "]");
            if (!string.IsNullOrEmpty(gameInfo.Rules))
            {
                builder.Append("RU[" + gameInfo.Rules + "]");
            }
            builder.Append("PB[" + gameInfo.Player1Name + "]");
            builder.Append("PW[" + gameInfo.Player2Name + "]");
            builder.Append("BR[" + gameInfo.Player1Rank + ", " + gameInfo.Player1Rating + "]");
            builder.Append("WR[" + gameInfo.Player2Rank + ", " + gameInfo.Player2Rating + "]");
            if (gameInfo.Date != DateTime.MinValue)
            {
                builder.Append("DT[" + gameInfo.Date.ToString("u").Replace("Z", "") + "]");
            }
            if (!string.IsNullOrEmpty(gameInfo.Event))
            {
                builder.Append("EV[" + gameInfo.Event + "]");
            }
            if (gameInfo.WinReason != WinReason.Unknown)
            {
                builder.Append("RE[" + gameInfo.Result + "]");
            }
            if (!string.IsNullOrEmpty(gameInfo.Source))
            {
                builder.Append("SO[" + gameInfo.Source + "]");
            }
            if (gameInfo.TimeLimits != TimeSpan.MinValue)
            {
                builder.Append("TM[" + gameInfo.TimeLimits.TotalSeconds + "]");
            }
            if (!string.IsNullOrEmpty(gameInfo.OverTime))
            {
                builder.Append("OT[" + gameInfo.OverTime + "]");
            }
            if (!string.IsNullOrEmpty(gameInfo.Description))
            {
                builder.Append("C[" + gameInfo.Description + "]");
            }

            Serialize(gameInfo.GameTree, builder, 0);

            builder.Append(")");

            return Encoding.UTF8.GetBytes(builder.ToString());
        }

        #region Parse

        private GameTree ParseGameTree(out bool ignoreParent)
        {
            SkipSpaces();
            var gameTree = new GameTree();
            _currentGameTree = gameTree;
            bool atLeastOnePropertySpecified = false;
            while (Accept(';') || (_data[_currentDataPos] != '(' && _data[_currentDataPos] != ')'))
            {
                ParseProperty();
                atLeastOnePropertySpecified = true;
                SkipSpaces();
            }
            if (!atLeastOnePropertySpecified)
            {
                throw new Exception("At least one property should be specified");
            }
            if (_oldGameTree != null)
            {
                ignoreParent = false;
                _oldGameTree.Childs.Clear();
            }
            else
            {
                ignoreParent = true;
            }

            while (Accept('('))
            {
                GameTree childGameTree = ParseGameTree(out bool ignoreParent2);
                if (ignoreParent2)
                {
                    foreach (var tree in childGameTree.Childs)
                    {
                        gameTree.Childs.Add(tree);
                        tree.Parent = gameTree;
                    }
                }
                else
                {
                    gameTree.Childs.Add(childGameTree);
                    childGameTree.Parent = gameTree;
                }
                Expect(')');
                SkipSpaces();
            }

            return gameTree;
        }

        private void ParseProperty()
        {
            SkipSpaces();
            int propertyIdStartInd = _currentDataPos;
            if (IsUpperLatin((char)_data[_currentDataPos]))
            {
                _currentDataPos++;
                while (IsUpperLatin((char)_data[_currentDataPos]) || char.IsDigit((char)_data[_currentDataPos]))
                {
                    _currentDataPos++;
                }
            }
            string propertyId = _encoding.GetString(_data, propertyIdStartInd, _currentDataPos - propertyIdStartInd);

            SkipSpaces();
            int propertyValueNumber = 0;
            while (Accept('['))
            {
                int propertyValueStartInd = _currentDataPos;
                var propertyValue = new List<byte>();
                while (_data[_currentDataPos] != ']')
                {
                    if (_data[_currentDataPos] == '\\')
                    {
                        _currentDataPos++;
                    }
                    propertyValue.Add(_data[_currentDataPos]);
                    _currentDataPos++;
                }

                ProcessProperty(propertyId, propertyValue.ToArray(), propertyValueNumber++);

                Expect(']');
                SkipSpaces();
            }

            if (propertyId == "B" || propertyId == "W")
            {
                var newGameTree = new GameTree();
                _currentGameTree.Childs.Add(newGameTree);
                _oldGameTree = _currentGameTree;
                _currentGameTree = newGameTree;
                _currentGameTree.Parent = _oldGameTree;
            }

            if (propertyValueNumber == 0)
            {
                throw new Exception("At least one property should be specified");
            }
        }

        // https://en.wikipedia.org/wiki/Smart_Game_Format
        private void ProcessProperty(string id, byte[] propertyValue, int propertyValueNumber)
        {
            string stringValue = _encoding.GetString(propertyValue);
            string[] strs;
            switch (id)
            {
                case "AP":
                    _gameInfo.AppName = stringValue;
                    break;

                case "C":
                    if (_commentNumber == 0)
                    {
                        _gameInfo.Description = stringValue;
                    }
                    _commentNumber++;
                    break;

                case "CA":
                    try
                    {
                        _encoding = Encoding.GetEncoding(stringValue);
                    }
                    catch
                    {
                        throw new Exception($"Encoding {stringValue} unrecognized");
                    }
                    break;

                case "DT":
                    DateTime dateTime;
                    if (DateTime.TryParse(stringValue, out dateTime))
                    {
                        _gameInfo.Date = dateTime;
                    }
                    break;

                case "GM":
                    try
                    {
                        _gameInfo.GameType = (GameType)int.Parse(stringValue);
                    }
                    catch
                    {
                        throw new Exception($"Game type {stringValue} unrecognized");
                    }
                    break;

                case "EV":
                    _gameInfo.Event = stringValue;
                    break;

                case "FF": // FileFormat == 4
                    _fileFormat = int.Parse(stringValue);
                    break;

                case "RE":
                    _gameInfo.Result = stringValue;
                    break;

                case "SZ":
                    try
                    {
                        string[] sizes = stringValue.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        if (sizes.Length == 1)
                        {
                            _gameInfo.Width = _gameInfo.Height = int.Parse(stringValue);
                        }
                        else if (sizes.Length == 2)
                        {
                            _gameInfo.Width = int.Parse(sizes[0]);
                            _gameInfo.Height = int.Parse(sizes[1]);
                        }
                        else
                        {
                            throw new Exception($"Board size {stringValue} parsing failed");
                        }
                    }
                    catch
                    {
                        throw new Exception($"Board size parsing {stringValue} failed");
                    }
                    break;

                case "TM":
                    double doubleValue;
                    if (double.TryParse(stringValue, out doubleValue))
                    {
                        _gameInfo.TimeLimits = TimeSpan.FromSeconds(doubleValue);
                    }
                    break;

                case "OT":
                    _gameInfo.OverTime = stringValue;
                    break;

                case "PB":
                    _gameInfo.Player1Name = stringValue;
                    break;

                case "PW":
                    _gameInfo.Player2Name = stringValue;
                    break;

                case "BR":
                case "WR":
                    strs = stringValue.Replace(" ", "").Split(',');
                    Rank rank;
                    if (Enum.TryParse(strs[0], true, out rank))
                    {
                        if (id == "BR")
                        {
                            _gameInfo.Player1Rank = rank;
                        }
                        else
                        {
                            _gameInfo.Player2Rank = rank;
                        }
                    }
                    if (strs.Length > 1)
                    {
                        if (int.TryParse(strs[1], out int rating))
                        {
                            if (id == "BR")
                            {
                                _gameInfo.Player1Rating = rating;
                            }
                            else
                            {
                                _gameInfo.Player2Rating = rating;
                            }
                        }
                    }
                    break;

                case "RU":
                    _gameInfo.Rules = stringValue;
                    break;

                case "SO":
                    _gameInfo.Source = stringValue;
                    break;

                case "MN":
                    break;

                case "AB":
                case "AW":
                    _gameInfo.GameTree.AddMove(GetGameMove(id[1], stringValue[0], stringValue[1]));
                    break;

                case "B":
                case "W":
                    if (stringValue[0] != '.')
                    {
                        _currentGameTree.GameMoves.Add(GetGameMove(id[0], stringValue[0], stringValue[1]));
                    }
                    else
                    {
                        string opponentColor = id == "B" ? "W" : "B";
                        int moveCount = stringValue.Length - 1;
                        for (int i = 1; i < moveCount; i += 2)
                        {
                            _currentGameTree.GameMoves.Add(GetGameMove(opponentColor[0], stringValue[i], stringValue[i + 1]));
                        }
                    }
                    break;

                default:
                    // Unknown property.
                    break;
            }
        }

        private GameMove GetGameMove(char c, char x, char y)
        {
            return new GameMove(ColorToNumber(c), CharToIntMove(y), CharToIntMove(x));
        }

        private int CharToIntMove(char c)
        {
            if (c >= 'a' && c <= 'z')
            {
                return c - 'a' + 1;
            }
            if (c >= 'A' && c <= 'Z')
            {
                return c - 'A' + 27;
            }
            throw new Exception($"Wrong move value: {c}");
        }

        private bool IsUpperLatin(char c)
        {
            return c >= 'A' && c <= 'Z';
        }

        private void SkipSpaces()
        {
            while (_currentDataPos < _data.Length && char.IsWhiteSpace((char)_data[_currentDataPos]))
            {
                _currentDataPos++;
            }
        }

        private bool Expect(char c)
        {
            if (Accept(c))
            {
                return true;
            }
            else
            {
                throw new Exception($"Unexpected symbol {c}");
            }
        }

        private bool Accept(char c)
        {
            if (_currentDataPos < _data.Length && _data[_currentDataPos] == c)
            {
                _currentDataPos++;
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Serialize

        private void Serialize(GameTree gameTree, StringBuilder builder, int level)
        {
            if (!gameTree.Root)
            {
                builder.Append(";");
            }
            AppendMoves(builder, gameTree);

            if (gameTree.Childs.Count == 1)
            {
                Serialize(gameTree.Childs.First(), builder, 0);
            }
            else if (gameTree.Childs.Count > 0)
            {
                string spaces = null;
                foreach (var child in gameTree.Childs)
                {
                    if (NewLines)
                    {
                        spaces = string.Concat(Enumerable.Repeat(Indent, level));
                        builder.AppendLine();
                        builder.Append(spaces);
                    }
                    builder.Append('(');
                    Serialize(child, builder, level + 1);
                    if (NewLines && child.Childs.Count > 1)
                    {
                        builder.AppendLine();
                        builder.Append(spaces);
                    }
                    builder.Append(')');
                }
            }
        }

        private void AppendMoves(StringBuilder builder, GameTree gameTree)
        {
            for (int i = 0; i < 2; i++)
            {
                bool colorAppended = false;
                foreach (var gameMove in gameTree.GameMoves)
                {
                    if (gameMove.PlayerNumber == i)
                    {
                        if (!colorAppended)
                        {
                            builder.Append((gameTree.Root ? "A" : "") + NumberToColor(i));
                            colorAppended = true;
                        }
                        builder.Append('[');
                        builder.Append(IntToChar(gameMove.Column));
                        builder.Append(IntToChar(gameMove.Row));
                        builder.Append(']');
                    }
                }
            }
        }

        private char IntToChar(int i)
        {
            if (i - 1 >= 0 && i - 1 <= 'z' - 'a')
            {
                return (char)(i - 1 + 'a');
            }
            if (i - 27 >= 0 && i - 27 <= 'Z' - 'A')
            {
                return (char)(i - 27 + 'A');
            }
            throw new Exception($"Too much position: {i}");
        }

        private char NumberToColor(int number)
        {
            return number == 0 ? 'B' : 'W';
        }

        private int ColorToNumber(char color)
        {
            return color == 'B' ? 0 : 1;
        }

        #endregion
    }
}
