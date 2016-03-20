using DotsGame.Formats;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            get
            {
                return _gameInfo;
            }
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
            get
            {
                return _gameInfo.AppName;
            }
            set
            {
                _gameInfo.AppName = value;
            }
        }

        public string FirstPlayer
        {
            get
            {
                return _gameInfo.Player1Name;
            }
            set
            {
                _gameInfo.Player1Name = value;
            }
        }

        public string SecondPlayer
        {
            get
            {
                return _gameInfo.Player2Name;
            }
            set
            {
                _gameInfo.Player2Name = value;
            }
        }

        public string FirstPlayerRank
        {
            get
            {
                return _gameInfo.Player1Rank.ToString();
            }
            set
            {
                Rank rank;
                if (Enum.TryParse(value, true, out rank))
                {
                    _gameInfo.Player1Rank = rank;
                }
            }
        }

        public string SecondPlayerRank
        {
            get
            {
                return _gameInfo.Player2Rank.ToString();
            }
            set
            {
                Rank rank;
                if (Enum.TryParse(value, out rank))
                {
                    _gameInfo.Player2Rank = rank;
                }
            }
        }

        public string FirstPlayerRating
        {
            get
            {
                return _gameInfo.Player1Rating.ToString();
            }
            set
            {
                double rating;
                if (double.TryParse(value, out rating))
                {
                    _gameInfo.Player1Rating = rating;
                }
            }
        }

        public string SecondPlayerRating
        {
            get
            {
                return _gameInfo.Player2Rating.ToString();
            }
            set
            {
                double rating;
                if (double.TryParse(value, out rating))
                {
                    _gameInfo.Player2Rating = rating;
                }
            }
        }

        public string Date
        {
            get
            {
                return _gameInfo.Date.ToString();
            }
            set
            {
                DateTime date;
                if (DateTime.TryParse(value, out date))
                {
                    _gameInfo.Date = date;
                }
            }
        }

        public string Time
        {
            get
            {
                return _gameInfo.TimeLimits.ToString();
            }
            set
            {
                TimeSpan timeSpan;
                if (TimeSpan.TryParse(value, out timeSpan))
                {
                    _gameInfo.TimeLimits = timeSpan;
                }
            }
        }

        public string Overtime
        {
            get
            {
                return _gameInfo.OverTime;
            }
            set
            {
                _gameInfo.OverTime = value;
            }
        }

        public string Event
        {
            get
            {
                return _gameInfo.Event;
            }
            set
            {
                _gameInfo.Event = value;
            }
        }

        public string Source
        {
            get
            {
                return _gameInfo.Source;
            }
            set
            {
                _gameInfo.Source = value;
            }
        }

        public string Result
        {
            get
            {
                return _gameInfo.Result;
            }
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
            get
            {
                return _gameInfo.Description;
            }
            set
            {
                _gameInfo.Description = value;
            }
        }
    }
}
