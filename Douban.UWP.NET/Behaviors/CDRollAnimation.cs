using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Douban.UWP.NET.Behaviors {
    public class CDRollAnimation : DependencyObject, IBehavior {

        /// <summary>
        /// AssociatedObject是获取与Behavior关联起来的元素对象
        /// </summary>
        public DependencyObject AssociatedObject { get; private set; }

        public bool IsRunning { get; set; }

        public bool IsPaused { get; set; }

        Storyboard cd_sb;

        public void Attach(DependencyObject ObjectInstance) {
            /// 获取关联的对象
            AssociatedObject = ObjectInstance;
            if (ObjectInstance == null)
                return;
            var element = AssociatedObject as FrameworkElement;
            if (element == null)
                return;

            /// 添加CompositeTransform支持
            var CT = new CompositeTransform();
            element.RenderTransform = CT;

            element.SizeChanged += OnSizeChanged;

            InitCDStoryboard(element);

        }

        private void InitCDStoryboard(FrameworkElement element) {
            if (cd_sb != null)
                return;
            var comp = element.RenderTransform as CompositeTransform;
            if (comp == null)
                element.RenderTransform = comp = new CompositeTransform();
            var Translate = new DoubleAnimationUsingKeyFrames {
                EnableDependentAnimation =false,
                RepeatBehavior = new RepeatBehavior { Type = RepeatBehaviorType.Forever },
            };
            Storyboard.SetTargetProperty(Translate, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.Rotation)").Path);
            Storyboard.SetTarget(Translate, element);
            Translate.KeyFrames.Add(new LinearDoubleKeyFrame {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)),
                Value = 0,
            });
            Translate.KeyFrames.Add(new LinearDoubleKeyFrame{
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(10)),
                Value = 180,
            });
            Translate.KeyFrames.Add(new LinearDoubleKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(20)),
                Value = 360,
            });
            comp.CenterX = element.ActualWidth / 2;
            comp.CenterY = element.ActualHeight / 2;
            cd_sb = new Storyboard();
            cd_sb.Children.Add(Translate);
            cd_sb.Completed += OnStoryboardCompleted;
        }

        private void OnStoryboardCompleted(object sender, object e) {
            IsRunning = false;
            IsPaused = false;
            AnimationPlay();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e) {
            if (!(sender is FrameworkElement))
                return;
            var element = sender as FrameworkElement;
            var comp = element.RenderTransform as CompositeTransform;
            if (comp == null)
                element.RenderTransform = comp = new CompositeTransform();
            comp.CenterX = element.ActualWidth / 2;
            comp.CenterY = element.ActualHeight / 2;
        }

        public void AnimationPlay() {
            IsRunning = true;
            IsPaused = false;
            cd_sb?.Begin();
        }

        public void AnimationPause() {
            if (!IsRunning) {
                AnimationPlay();
                AnimationPause();
                return;
            }
            if (IsPaused)
                return;
            IsPaused = true;
            cd_sb?.Pause();
        }

        public void AnimationResume() {
            if (!IsRunning){
                AnimationPlay();
                return;
            }
            if (!IsPaused)
                return;
            IsPaused = false;
            cd_sb?.Resume();
        }

        public void AnimationStop() {
            IsPaused = false;
            IsRunning = false;
            cd_sb?.Stop();
        }

        /// <summary>
        /// Detach方法是当Behavior与关联的元素分离时要执行的代码
        /// </summary>
        public void Detach() {
            var element = AssociatedObject as FrameworkElement;
            if (element == null)
                return;

            cd_sb.Completed -= OnStoryboardCompleted;
            element.SizeChanged -= OnSizeChanged;

        }

    }
}
