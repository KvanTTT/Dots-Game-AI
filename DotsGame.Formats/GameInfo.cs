using DotsGame.Formats;
using System;

namespace DotsGame
{
    public class GameInfo
    {
        public bool FromUrl { get; set; }

        public GameType GameType { get; set; } = GameType.Kropki;

        public string AppName { get; set; } = "Dots Game AI";

        public int Width { get; set; } = 39;

        public int Height { get; set; } = 32;

        public string Player1Name { get; set; } = "Player1";

        public string Player2Name { get; set; } = "Player2";

        public Rank Player1Rank { get; set; } = Rank.Unknown;

        public Rank Player2Rank { get; set; } = Rank.Unknown;

        public double Player1Rating { get; set; } = 0;

        public double Player2Rating { get; set; } = 0;

        public DateTime Date { get; set; } = DateTime.Now;

        public TimeSpan TimeLimits { get; set; } = TimeSpan.Zero;

        public string OverTime { get; set; } = "";

        public string Rules { get; set; } = "";

        public string Event { get; set; } = "";

        public int WinPlayerNumber { get; set; } = 0;

        public WinReason WinReason { get; set; } = WinReason.Unknown;

        public int WinScore { get; set; } = 0;

        public string Result
        {
            get
            {
                string result;
                switch (WinReason)
                {
                    case WinReason.Unknown:
                        result = "?";
                        break;
                    case WinReason.Draw:
                        result = "0";
                        break;
                    case WinReason.Void:
                        result = "Void";
                        break;
                    default:
                        result = (WinPlayerNumber == 0 ? "B" : "W") + "+" +
                            (WinReason == WinReason.Score ? WinScore.ToString() : WinReason.ToString());
                        break;
                }
                return result;
            }
            set
            {
                string[] strs = value.Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
                if (strs.Length > 1)
                {
                    string reasonStr = strs[1];
                    if (reasonStr == "R" || reasonStr == "Resing")
                    {
                        WinReason = WinReason.Resign;
                    }
                    else if (reasonStr == "T" || reasonStr == "Time")
                    {
                        WinReason = WinReason.Time;
                    }
                    else if (reasonStr == "F" || reasonStr == "Forfeit")
                    {
                        WinReason = WinReason.Forfeit;
                    }
                    else if (reasonStr == "Void")
                    {
                        WinReason = WinReason.Void;
                    }
                    else if (reasonStr == "?")
                    {
                        WinReason = WinReason.Unknown;
                    }
                    else
                    {
                        WinReason = WinReason.Score;
                        WinScore = int.Parse(reasonStr);
                    }
                }
                else
                {
                    WinReason = WinReason.Draw;
                }
                WinPlayerNumber = strs[0] == "B" ? 0 : 1;
            }
        }

        public string Source { get; set; } = "";

        public string Description { get; set; } = "";

        public GameTree GameTree { get; set; }

        public GameInfo()
        {
            GameTree = new GameTree() { Number = 0, Root = true };
        }

        public void CopyInfoFrom(GameInfo sourceInfo)
        {
            FromUrl = sourceInfo.FromUrl;
            GameType = sourceInfo.GameType;
            AppName = sourceInfo.AppName;
            Width = sourceInfo.Width;
            Height = sourceInfo.Height;
            Player1Name = sourceInfo.Player1Name;
            Player2Name = sourceInfo.Player2Name;
            Player1Rank = sourceInfo.Player1Rank;
            Player2Rank = sourceInfo.Player2Rank;
            Player1Rating = sourceInfo.Player1Rating;
            Player2Rating = sourceInfo.Player2Rating;
            Date = sourceInfo.Date;
            TimeLimits = sourceInfo.TimeLimits;
            OverTime = sourceInfo.OverTime;
            Rules = sourceInfo.Rules;
            Event = sourceInfo.Event;
            WinPlayerNumber = sourceInfo.WinPlayerNumber;
            WinReason = sourceInfo.WinReason;
            WinScore = sourceInfo.WinScore;
            Source = sourceInfo.Source;
            Description = sourceInfo.Description;
        }

        public GameTree GetDefaultLastTree()
        {
            return GameTree.GetDefaultLastTree();
        }
    }
}
