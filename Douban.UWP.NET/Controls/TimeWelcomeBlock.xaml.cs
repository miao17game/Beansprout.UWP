using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Wallace.UWP.Helpers.Tools;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Douban.UWP.NET.Controls {

    public sealed partial class TimeWelcomeBlock : UserControl {

        private TranslateTransform translateT;
        DispatcherTimer WeocomeTimer;
        private int action;
        private Action actionToDo;
        private int animationTime;
        private int offset = 0;

        public TimeWelcomeBlock(uint animationTime, Action action = null) {
            this.InitializeComponent();

            SetStatus();

            this.actionToDo = action;
            this.animationTime = Convert.ToInt32(animationTime);
            this.ManipulationMode = ManipulationModes.TranslateY;
            this.ManipulationCompleted += BasePage_ManipulationCompleted;
            this.ManipulationDelta += BasePage_ManipulationDelta;
            translateT = this.RenderTransform as TranslateTransform;

            if (translateT == null)
                this.RenderTransform = translateT = new TranslateTransform();

            InitSliderTimer();

        }

        public TimeWelcomeBlock() : this(animationTime: 520) { }

        private void InitSliderTimer() {
            WeocomeTimer = new DispatcherTimer();
            WeocomeTimer.Tick += DispatcherTimer_TickSliderbarMP;
            WeocomeTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            WeocomeTimer.Start();
        }

        void DispatcherTimer_TickSliderbarMP(object sender, object e) {
            offset++;
            if(offset>5)
                MakeMainOut(this.ActualHeight);
        }

        private void SetStatus() {
            int nowHour = DateTime.Now.Hour;
            if(nowHour>3 && nowHour<= 11) { // Morning
                this.Main.Background = new SolidColorBrush(Color.FromArgb(255, 130, 150, 190));
                this.IconBlock.Text = char.ConvertFromUtf32(0xE2AD);
                this.MessBlock.Text = UWPStates.GetUIString("GoodMorning");
            } else if(nowHour>11 && nowHour <= 14) { // Noon
                this.Main.Background = new SolidColorBrush(Color.FromArgb(255, 254, 183, 8));
                this.IconBlock.Text = char.ConvertFromUtf32(0xE706);
                this.MessBlock.Text = UWPStates.GetUIString("GoodNoon");
            } else if(nowHour>14 && nowHour <= 18) { // Afternoon
                this.Main.Background = new SolidColorBrush(Color.FromArgb(255, 202, 0, 62));
                this.IconBlock.Text = char.ConvertFromUtf32(0xEC0A);
                this.MessBlock.Text = UWPStates.GetUIString("GoodAfternoon");
            } else { // Night
                this.Main.Background = new SolidColorBrush(Color.FromArgb(255, 178, 150, 123));
                this.IconBlock.Text = char.ConvertFromUtf32(0xEC46);
                this.MessBlock.Text = UWPStates.GetUIString("GoodNight");
            }
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

            if (abs_delta < this.ActualHeight / 30 && speed < 0.2) {
                translateT.Y = 0;
                return;
            }

            action = 0;
            //确定是否上划了
            if (delta < 0)
                to = this.ActualHeight;
            else if (delta > 0)
                return;

            WeocomeTimer.Stop();
            MakeMainOut(to);

        }

        private void MakeMainOut(double to) {
            var s = new Storyboard();
            var doubleanimation = new DoubleAnimation() { Duration = new Duration(TimeSpan.FromMilliseconds(this.animationTime)), From = this.translateT.Y, To = -(5*to) };
            doubleanimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            doubleanimation.Completed += Doubleanimation_Completed;
            Storyboard.SetTarget(doubleanimation, this.translateT);
            Storyboard.SetTargetProperty(doubleanimation, "Y");
            s.Children.Add(doubleanimation);
            s.Begin();
        }

        private void Doubleanimation_Completed(object sender, object e) {
            if (action == 0) {
                this.SetVisibility(false);
                actionToDo?.Invoke();
            }
            translateT = this.RenderTransform as TranslateTransform;
            if (translateT == null)
                this.RenderTransform = translateT = new TranslateTransform();
            translateT.Y = 0;
        }

    }
}
