using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotsGame.Sgf
{
    public class SgfParser : IDotsGameFormatParser
    {
        private readonly Encoding DefaultEncoding = Encoding.Default;
        private GameInfo _gameInfo;
        private int _currentDataPos;
        private byte[] _data;
        private GameTree _oldGameTree, _currentGameTree;
        private int _commentNumber;

        public GameInfo Parse(byte[] data)
        {
            _gameInfo = new GameInfo();
            _data = data;
            _currentDataPos = 0;
            _commentNumber = 0;
            _gameInfo.Encoding = Encoding.UTF8;

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

        public byte[] Save(GameInfo gameInfo)
        {
            throw new NotImplementedException();
        }

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
            string propertyId = DefaultEncoding.GetString(_data, propertyIdStartInd, _currentDataPos - propertyIdStartInd);

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
            string stringValue = _gameInfo.Encoding.GetString(propertyValue);
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
                        _gameInfo.Encoding = Encoding.GetEncoding(stringValue);
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
                    _gameInfo.Player2Name = stringValue;
                    break;

                case "PW":
                    _gameInfo.Player1Name = stringValue;
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
    }
}
