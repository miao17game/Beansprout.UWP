using static Wallace.UWP.Helpers.Tools.UWPStates;
using static Douban.UWP.NET.Resources.AppResources;

using Douban.UWP.Core.Models.FMModels;
using System;
using System.Collections.Generic;
using Windows . UI . Text;
using Windows . UI . Xaml;
using Windows . UI . Xaml . Controls;
using Windows . UI . Xaml . Media;
using Windows . UI . Xaml . Media . Animation;
using System.Linq;
using System.Threading.Tasks;
using Douban.UWP.NET.Tools;
using Douban.UWP.NET.Models;

namespace Douban.UWP.NET.Controls {
    public class LrcListView : ListView{
        public LrcListView ( ) {
            RenderTransform = new CompositeTransform ( );
        }

        #region Lrc Animations

        public void SetLrcAnimation(IList<LrcInfo> list, ref VisualBoardVM vm, LrcListViewAnimationStyle anima_style = LrcListViewAnimationStyle.SmoothRoll) {
            ThisVM = vm;
            _animation_style = anima_style;
            _lrc_list = list;
            _index_new = -1;
            Canvas.SetTop(this, 230);
            if (_timer != null)
                _timer.Start();
            else {
                _timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 300) };
                _timer.Tick += DispatcherTimerEventAsync;
                _timer.Start();
            }
        }

        public async void DispatcherTimerEventAsync(object sender, object e) {
            ThisVM.CurrentTime = Service.Session.Position;
            if (_lrc_list.Count == 0)
                return;
            var current = ThisVM.CurrentTime.TotalMilliseconds;
            var newTurn = _lrc_list.Select(i => i.LrcTime).Where(i => i < current).Count() - 1;
            await SetLrcListSelectedAsync(newTurn);
        }

        private async Task SetLrcListSelectedAsync(int newTurn) {
            _index = newTurn;
            if (_index == -1 && this.RenderTransform is CompositeTransform)
                (this.RenderTransform as CompositeTransform).TranslateY = 0;
            if (_index == _index_new)
                return;
            await Dispatcher.UpdateUI(() => {
                try {
                    this.SelectedIndex = _index;
                    SetLrcAnimation(_index);
                    _index_new = _index;
                } catch (ArgumentException) { /* Ingore. */}
            });
        }

        private void SetLrcAnimation(int index) {
            Canvas.SetTop(this, 230 - index * 44);
            double milis = 0;
            double turn = -44;
            if (index >= _lrc_list.Count - 1)
                return;
            try {
                milis = 1.0 * (_lrc_list[index + 1].LrcTime - _lrc_list[index].LrcTime);
                turn *= 1.0;
            } catch (Exception) { /* Ingore. */ }
            SetAnimation(turn, milis);
        }

        public void SetAnimation(double turn, double milis) {
            if (_listview_sb != null) { _listview_sb.Stop(); }
            _listview_sb = new Storyboard();
            switch (_animation_style) {
                case LrcListViewAnimationStyle.FastSlide:
                    _listview_sb.Children.Add(GetTimelineFastSlide(turn, milis, this));
                    break;
                case LrcListViewAnimationStyle.SmoothSlide:
                    _listview_sb.Children.Add(GetTimelineSmoothSlide(turn, milis, this));
                    break;
                case LrcListViewAnimationStyle.FastRoll:
                    _listview_sb.Children.Add(GetTimelineFastRoll(turn, milis, this));
                    break;
                default:
                    _listview_sb.Children.Add(GetTimelineSmoothRoll(turn, milis, this));
                    break;
            }
            _listview_sb.Begin();
        }

        #region Lrc animation stye collection

        private DoubleAnimationUsingKeyFrames GetTimelineSmoothRoll(double y, double milis, DependencyObject obj) {
            var Translate = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTargetProperty(Translate, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)").Path);
            Storyboard.SetTarget(Translate, obj);
            Translate.KeyFrames.Add(new EasingDoubleKeyFrame {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(milis)),
                Value = y
            });
            return Translate;
        }

        private DoubleAnimationUsingKeyFrames GetTimelineFastRoll(double y, double milis, DependencyObject obj) {
            var Translate = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTargetProperty(Translate, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)").Path);
            Storyboard.SetTarget(Translate, obj);
            Translate.KeyFrames.Add(new EasingDoubleKeyFrame {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0.6 * milis)),
                Value = y
            });
            return Translate;
        }

        private DoubleAnimationUsingKeyFrames GetTimelineSmoothSlide(double y, double milis, DependencyObject obj) {
            var Translate = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTargetProperty(Translate, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)").Path);
            Storyboard.SetTarget(Translate, obj);
            Translate.KeyFrames.Add(new EasingDoubleKeyFrame {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0.35 * milis)),
                Value = y,
            });
            return Translate;
        }

        private DoubleAnimationUsingKeyFrames GetTimelineFastSlide(double y, double milis, DependencyObject obj) {
            var Translate = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTargetProperty(Translate, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)").Path);
            Storyboard.SetTarget(Translate, obj);
            Translate.KeyFrames.Add(new EasingDoubleKeyFrame {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0.15 * milis)),
                Value = y
            });
            return Translate;
        }

        #endregion

        #endregion

        Storyboard _listview_sb;
        LrcListViewAnimationStyle _animation_style;
        IList<LrcInfo> _lrc_list;
        int _index = 0;
        int _index_new = -1;

        DispatcherTimer _timer;
        public DispatcherTimer Timer { get { return _timer; } set { _timer = value; } }

        public VisualBoardVM ThisVM { get; set; }

    }

    public enum LrcListViewAnimationStyle { SmoothRoll, FastRoll, SmoothSlide, FastSlide }

}
