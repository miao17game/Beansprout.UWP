using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml.Media;

namespace Douban.UWP.Core.Models.FMModels {
    public class LrcInfo : ViewModelBase {
        private string lrcString;
        private int lrcTime;
        private double translateY;
        string _Title = default(string);
        Color _Color = Colors.White;
        FontWeight _LrcFontWeight = FontWeights.Thin;
        double _LrcFontSize = 18;

        public string LrcString{
            get {
                return lrcString;
            }
            set {
                lrcString = value;
                RaisePropertyChanged ( "LrcString" );
            }
        }

        public int LrcTime {
            get {
                return lrcTime;
            }
            set {
                lrcTime = value;
                RaisePropertyChanged ( "LrcTime" );
            }
        }

        public double TranslateY {
            get {
                return translateY;
            }
            set {
                translateY = value;
                RaisePropertyChanged ( "TranslateY" );
            }
        }

        public double LrcFontSize {
            get {
                return _LrcFontSize;
            }
            set { SetProperty ( ref _LrcFontSize , value ); }
        }

        public FontWeight LrcFontWeight {
            get {
                return _LrcFontWeight;
            }
            set { SetProperty ( ref _LrcFontWeight , value ); }
        }

        public string Title {
            get { return _Title; }
            set { SetProperty ( ref _Title , value ); }
        }

        public Color Color {
            get { return _Color; }
            set { SetProperty ( ref _Color , value ); }
        }
    }
}
