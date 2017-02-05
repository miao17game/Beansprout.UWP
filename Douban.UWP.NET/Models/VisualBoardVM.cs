using static Wallace.UWP.Helpers.Tools.UWPStates;
using static Douban.UWP.NET.Resources.AppResources;


using System.Collections.ObjectModel;
using System.Diagnostics;
using Windows.UI.Xaml;
using System.Threading.Tasks;
using System;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Text;
using Windows.Storage;
using Douban.UWP.Core.Models;
using Douban.UWP.Core.Models.FMModels;
using System.Collections.Generic;

namespace Douban.UWP.NET.Models {
    class VisualBoardVM :  ViewModelBase {

        #region Fields
        private Color TextColor;
        private string FileName;
        private string Artist;
        public  string LrcPath;
        static string TitleFromMainCS = default(string);
        #endregion

        public VisualBoardVM ( ) {
            GetRequestedThemeColer ( );
        }

        private void GetRequestedThemeColer ( ) {
            if (IsGlobalDark) {
                TextColor = Colors.White;
            } else {
                TextColor = Colors.White;
            }
        }

        #region VisualBoardViewModel

        private IList<LrcInfo> _lrcList;
        public IList<LrcInfo> LrcList {
            get { return _lrcList; }
            set { _lrcList = value; RaisePropertyChanged ( "LrcList" ); }
        }

        private string _lrcTitle = default(string);
        public string LrcTitle {
            get { return _lrcTitle; }
            set { _lrcTitle = value; RaisePropertyChanged ( "LrcTitle" ); }
        }

        private string _backimage = default(string);
        public string BackImage {
            get { return _backimage; }
            set { _backimage = value; RaisePropertyChanged("BackImage"); }
        }

        private TimeSpan _currenttime;
        public TimeSpan CurrentTime {
            get { return _currenttime; }
            set { _currenttime = value; RaisePropertyChanged("CurrentTime"); }
        }

        private TimeSpan _duration;
        public TimeSpan Duration {
            get { return _duration; }
            set { _duration = value; RaisePropertyChanged("Duration"); }
        }

        LrcInfo _Selected = default(LrcInfo);
        public LrcInfo Selected {
            get { return _Selected; }
            set {
                if ( Selected != null ) {
                    Selected . Color = TextColor;
                    Selected . LrcFontWeight = FontWeights . Thin;
                    Selected . LrcFontSize = 16;
                }
                SetProperty ( ref _Selected , value );
                try {
                    value . Color = ( ( SolidColorBrush ) Application . Current . Resources ["DoubanForeground03"] ) . Color;
                    value . LrcFontWeight = FontWeights . Medium;
                    value . LrcFontSize = 22;
                } catch {
                    Debug . WriteLine ( "歌词解析异常，无法应用歌词样式变更" );
                }
            }
        }

        #endregion

    }
}
