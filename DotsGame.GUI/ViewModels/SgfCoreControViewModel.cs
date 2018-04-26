using DotsGame.Formats;
using ReactiveUI;
using System;

namespace DotsGame.GUI
{
    public class SgfCoreControViewModel : ReactiveObject
    {
        private GameInfo _gameInfo = new GameInfo();

        public SgfCoreControViewModel()
        {
        }

        public GameInfo GameInfo
        {
            get => _gameInfo;
            set
            {
                _gameInfo = value;
                this.RaisePropertyChanged(nameof(AppName));
                this.RaisePropertyChanged(nameof(FirstPlayer));
                this.RaisePropertyChanged(nameof(SecondPlayer));
                this.RaisePropertyChanged(nameof(FirstPlayerRank));
                this.RaisePropertyChanged(nameof(SecondPlayerRank));
                this.RaisePropertyChanged(nameof(FirstPlayerRating));
                this.RaisePropertyChanged(nameof(SecondPlayerRating));
                this.RaisePropertyChanged(nameof(Date));
                this.RaisePropertyChanged(nameof(Time));
                this.RaisePropertyChanged(nameof(Overtime));
                this.RaisePropertyChanged(nameof(Event));
                this.RaisePropertyChanged(nameof(Source));
                this.RaisePropertyChanged(nameof(Result));
                this.RaisePropertyChanged(nameof(Description));
            }
        }

        public string AppName
        {
            get => _gameInfo.AppName;
            set => _gameInfo.AppName = value;
        }

        public string FirstPlayer
        {
            get => _gameInfo.Player1Name;
            set => _gameInfo.Player1Name = value;
        }

        public string SecondPlayer
        {
            get => _gameInfo.Player2Name;
            set => _gameInfo.Player2Name = value;
        }

        public string FirstPlayerRank
        {
            get => _gameInfo.Player1Rank.ToString();
            set
            {
                if (Enum.TryParse(value, true, out Rank rank))
                {
                    _gameInfo.Player1Rank = rank;
                }
            }
        }

        public string SecondPlayerRank
        {
            get => _gameInfo.Player2Rank.ToString();
            set
            {
                if (Enum.TryParse(value, out Rank rank))
                {
                    _gameInfo.Player2Rank = rank;
                }
            }
        }

        public string FirstPlayerRating
        {
            get => _gameInfo.Player1Rating.ToString();
            set
            {
                if (double.TryParse(value, out double rating))
                {
                    _gameInfo.Player1Rating = rating;
                }
            }
        }

        public string SecondPlayerRating
        {
            get => _gameInfo.Player2Rating.ToString();
            set
            {
                if (double.TryParse(value, out double rating))
                {
                    _gameInfo.Player2Rating = rating;
                }
            }
        }

        public string Date
        {
            get => _gameInfo.Date.ToString();
            set
            {
                if (DateTime.TryParse(value, out DateTime date))
                {
                    _gameInfo.Date = date;
                }
            }
        }

        public string Time
        {
            get => _gameInfo.TimeLimits.ToString();
            set
            {
                if (TimeSpan.TryParse(value, out TimeSpan timeSpan))
                {
                    _gameInfo.TimeLimits = timeSpan;
                }
            }
        }

        public string Overtime
        {
            get => _gameInfo.OverTime;
            set => _gameInfo.OverTime = value;
        }

        public string Event
        {
            get => _gameInfo.Event;
            set => _gameInfo.Event = value;
        }

        public string Source
        {
            get => _gameInfo.Source;
            set => _gameInfo.Source = value;
        }

        public string Result
        {
            get => _gameInfo.Result;
            set
            {
                try
                {
                    _gameInfo.Result = value;
                }
                catch
                {
                }
            }
        }

        public string Description
        {
            get => _gameInfo.Description;
            set => _gameInfo.Description = value;
        }
    }
}
