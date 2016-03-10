using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                bool ignoreParent;
                GameTree childGameTree = ParseGameTree(out ignoreParent);
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
            //builder.Append("BR[" +  + "]");
            //builder.Append("BW[" +  + "]");
            builder.Append("DT[" + gameInfo.Date.ToString("u").Replace("Z", "") + "]");
            //builder.Append("EV[" +  +"]");
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
                bool ignoreParent2;
                GameTree childGameTree = ParseGameTree(out ignoreParent2);
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
            bool atLeastOneValueSpecified = false;
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

                ProcessProperty(propertyId, propertyValue.ToArray());
                Expect(']');
                SkipSpaces();
                atLeastOneValueSpecified = true;
            }
            if (!atLeastOneValueSpecified)
            {
                throw new Exception("At least one property should be specified");
            }
        }

        // https://en.wikipedia.org/wiki/Smart_Game_Format
        private void ProcessProperty(string id, byte[] propertyValue)
        {
            string stringValue = _encoding.GetString(propertyValue);
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
                    try
                    {
                        _gameInfo.Date = DateTime.Parse(stringValue);
                    }
                    catch
                    {
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

                case "FF": // FileFormat == 4
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

                case "PB":
                    _gameInfo.Player1Name = stringValue;
                    break;

                case "PW":
                    _gameInfo.Player2Name = stringValue;
                    break;

                case "RU":
                    _gameInfo.Rules = stringValue;
                    break;

                case "B":
                case "W":
                    _currentGameTree.GameMoves.Add(new GameMove(id == "B" ? 0 : 1,
                        CharToIntMove(stringValue[1]), CharToIntMove(stringValue[0])));
                    var newGameTree = new GameTree();
                    _currentGameTree.Childs.Add(newGameTree);
                    _oldGameTree = _currentGameTree;
                    _currentGameTree = newGameTree;
                    _currentGameTree.Parent = _oldGameTree;
                    break;

                default:
                    // Unknown property.
                    break;
            }
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
            if (!gameTree.Move.IsRoot)
            {
                builder.Append(";");
                foreach (var gameMove in gameTree.GameMoves)
                {
                    string color = gameMove.PlayerNumber == 0 ? "B" : "W";
                    builder.Append(color + "[" + IntToChar(gameMove.Column) + IntToChar(gameMove.Row) + "]");
                }
            }
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
                    builder.Append("(");
                    Serialize(child, builder, level + 1);
                    if (NewLines && child.Childs.Count > 1)
                    {
                        builder.AppendLine();
                        builder.Append(spaces);
                    }
                    builder.Append(")");
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
            throw new Exception($"Too mouch position: {i}");
        }

        #endregion
    }
}
