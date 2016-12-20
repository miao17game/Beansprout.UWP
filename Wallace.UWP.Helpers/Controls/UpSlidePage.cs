using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Wallace.UWP.Helpers.Controls {
    /// <summary>
    /// 页面的基类，实现单向上的滑动手势
    /// </summary>
    public class UpSlidePage : Page {

        private TranslateTransform translateT;
        private int action;
        private Action actionToDo;
        private int animationTime = 520;

        public UpSlidePage() {
            this.ManipulationMode = ManipulationModes.TranslateY;
            this.ManipulationCompleted += BasePage_ManipulationCompleted;
            this.ManipulationDelta += BasePage_ManipulationDelta;
            translateT = this.RenderTransform as TranslateTransform;

            if (translateT == null)
                this.RenderTransform = translateT = new TranslateTransform();
        }

        /// <summary>
        /// 建立一个在动画结束时附加额外操作的上滑页面
        /// </summary>
        /// <param name="action">当动画结束，需要调用的操作</param>
        public UpSlidePage(Action action) {
            this.actionToDo = action;
            this.ManipulationMode = ManipulationModes.TranslateY;
            this.ManipulationCompleted += BasePage_ManipulationCompleted;
            this.ManipulationDelta += BasePage_ManipulationDelta;
            translateT = this.RenderTransform as TranslateTransform;

            if (translateT == null)
                this.RenderTransform = translateT = new TranslateTransform();
        }

        /// <summary>
        /// 建立一个在动画结束时附加额外操作的上滑页面，并给定动画时间
        /// </summary>
        /// <param name="action">当动画结束，需要调用的操作</param>
        /// <param name="animationTime">动画需要持续的时间，默认520</param>
        public UpSlidePage(Action action, uint animationTime) {
            this.actionToDo = action;
            this.animationTime = Convert.ToInt32(animationTime);
            this.ManipulationMode = ManipulationModes.TranslateY;
            this.ManipulationCompleted += BasePage_ManipulationCompleted;
            this.ManipulationDelta += BasePage_ManipulationDelta;
            translateT = this.RenderTransform as TranslateTransform;

            if (translateT == null)
                this.RenderTransform = translateT = new TranslateTransform();
        }

        private void BasePage_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs args) {
            //上划
            if (translateT.Y + args.Delta.Translation.Y > 0) {
                translateT.Y = 0;
                return;
            }
            translateT.Y += args.Delta.Translation.Y;
        }

        /// <summary>
        /// 滑动操作完成引发动画事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BasePage_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e) {
            double abs_delta = Math.Abs(e.Cumulative.Translation.Y);
            double speed = Math.Abs(e.Velocities.Linear.Y);
            double delta = e.Cumulative.Translation.Y;
            double to = 0;

            if (abs_delta < this.ActualHeight / 3 && speed < 0.7) {
                translateT.Y = 0;
                return;
            }

            action = 0;
            //确定是否上划了
            if (delta < 0)
                to = this.ActualHeight;
            else if (delta > 0)
                return;

            var s = new Storyboard();
            var doubleanimation = new DoubleAnimation() { Duration = new Duration(TimeSpan.FromMilliseconds(animationTime)), From = translateT.Y, To = -to };
            doubleanimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            doubleanimation.Completed += Doubleanimation_Completed;
            Storyboard.SetTarget(doubleanimation, translateT);
            Storyboard.SetTargetProperty(doubleanimation, "Y");
            s.Children.Add(doubleanimation);
            s.Begin();

        }

        private void Doubleanimation_Completed(object sender, object e) {
            if (action == 0) {
                this.Frame.Content = null;
                actionToDo?.Invoke();
            }
            translateT = this.RenderTransform as TranslateTransform;
            if (translateT == null)
                this.RenderTransform = translateT = new TranslateTransform();
            translateT.Y = 0;
        }

    }
}
