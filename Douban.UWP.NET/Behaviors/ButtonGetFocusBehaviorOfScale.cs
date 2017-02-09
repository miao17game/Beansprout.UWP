using System;
using System . Collections . Generic;
using System . Diagnostics;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using Microsoft . Xaml . Interactions;
using Microsoft . Xaml . Interactivity;
using Windows . System . Profile;
using Windows . UI . Xaml;
using Windows . UI . Xaml . Controls;
using Windows . UI . Xaml . Media;
using Windows . UI . Xaml . Media . Animation;

namespace Douban.UWP.NET.Behaviors {
    /// <summary>
    /// 圆形按钮鼠标事件，放大于缩小
    /// </summary>
    public class ButtonGetFocusBehaviorOfScale : DependencyObject, IBehavior {

        private readonly Storyboard storyboard = new Storyboard();
        private readonly Storyboard soliodStoryboard = new Storyboard();
        private const double AnimFrom = 1;
        private const double AnimTo01 = 1.2;
        private const double AnimTo02 = 1.5;
        private double CenterFrom ;
        private double CenterTo ;
        private readonly DoubleAnimation _scaleXAnim = new DoubleAnimation();
        private readonly DoubleAnimation _scaleYAnim = new DoubleAnimation();
        private readonly DoubleAnimation centerX = new DoubleAnimation();
        private readonly DoubleAnimation centerY = new DoubleAnimation();
        private readonly DoubleAnimation soliod_scaleXAnim = new DoubleAnimation();
        private readonly DoubleAnimation soliod_scaleYAnim = new DoubleAnimation();
        private readonly DoubleAnimation soliodcenterX = new DoubleAnimation();
        private readonly DoubleAnimation soliodcenterY = new DoubleAnimation();
        private bool IsReceivedPointer = false;
        private bool IsBoardOpened = false;


        /// <summary>
        /// Attach方法就是当该Behavior被关联到某一元素时要执行的代码
        /// </summary>
        /// <param name="ObjectInstance">元素对象的引用</param>
        public void Attach ( DependencyObject ObjectInstance ) {
            /// 获取关联的对象
            AssociatedObject = ObjectInstance;
            if ( ObjectInstance == null )
                return;
            var element = AssociatedObject as FrameworkElement;
            if ( element == null )
                return;

            /// 添加CompositeTransform缩放支持
            var CT = new CompositeTransform ( );
            element . RenderTransform = CT;

            CenterFrom = element . Width / 2;
            CenterTo = element . Width / 2;

            /// 设置动画关联对象
            Storyboard . SetTarget ( _scaleXAnim , element . RenderTransform );
            Storyboard . SetTargetProperty ( _scaleXAnim , nameof ( CompositeTransform . ScaleX ) );
            Storyboard . SetTarget ( _scaleYAnim , element . RenderTransform );
            Storyboard . SetTargetProperty ( _scaleYAnim , nameof ( CompositeTransform . ScaleY ) );
            Storyboard . SetTarget ( soliod_scaleXAnim , element . RenderTransform );
            Storyboard . SetTargetProperty ( soliod_scaleXAnim , nameof ( CompositeTransform . ScaleX ) );
            Storyboard . SetTarget ( soliod_scaleYAnim , element . RenderTransform );
            Storyboard . SetTargetProperty ( soliod_scaleYAnim , nameof ( CompositeTransform . ScaleY ) );

            /// 设置控件动画中心
            Storyboard . SetTarget ( centerX , element . RenderTransform );
            Storyboard . SetTargetProperty ( centerX , nameof ( CompositeTransform . CenterX ) );
            Storyboard . SetTarget ( centerY , element . RenderTransform );
            Storyboard . SetTargetProperty ( centerY , nameof ( CompositeTransform . CenterY ) );
            Storyboard . SetTarget ( soliodcenterX , element . RenderTransform );
            Storyboard . SetTargetProperty ( soliodcenterX , nameof ( CompositeTransform . CenterX ) );
            Storyboard . SetTarget ( soliodcenterY , element . RenderTransform );
            Storyboard . SetTargetProperty ( soliodcenterY , nameof ( CompositeTransform . CenterY ) );

            /// 将动画添加到故事版
            storyboard . Children . Add ( _scaleYAnim );
            storyboard . Children . Add ( _scaleXAnim );
            storyboard . Children . Add ( centerX );
            storyboard . Children . Add ( centerY );

            InitSoliodStoryBoard ( );

            /// 关联事件
            if ( !AnalyticsInfo . VersionInfo . DeviceFamily . Equals ( "Windows.Mobile" ) ) {
                element . PointerEntered += Obj_PointerEntered;
                element . PointerExited += Obj_PointerExited;
            }
            if ( element is Button )
                ( element as Button ) . Click += Obj_Click;

        }

        private void InitSoliodStoryBoard ( ) { 

            soliodStoryboard . Children . Add ( soliod_scaleYAnim );
            soliodStoryboard . Children . Add ( soliod_scaleXAnim );
            soliodStoryboard . Children . Add ( soliodcenterX );
            soliodStoryboard . Children . Add ( soliodcenterY );

            StartScaleStoryboardSoliod ( AnimFrom , AnimTo01 , 2 , 1 );
            StartScaleStoryboardCenterSoliod ( CenterFrom , CenterTo );
            soliodStoryboard . AutoReverse = true;
            soliodStoryboard . RepeatBehavior = RepeatBehavior . Forever;
            soliodStoryboard . Begin ( );
        }

        ///// <summary>
        ///// 常态动画第一阶段
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void SoliodAnimation01Completed ( object sender , object e ) {
        //    soliodStoryboard . Completed -= SoliodAnimation01Completed;
        //    StartScaleStoryboardSoliod ( AnimTo01 , AnimFrom , 0.3 , 4 );
        //    StartScaleStoryboardCenterSoliod ( CenterFrom , CenterTo );
        //    soliodStoryboard . Completed += SoliodAnimation02Completed;
        //    soliodStoryboard . AutoReverse = true;
        //    soliodStoryboard . RepeatBehavior = RepeatBehavior . Forever;
        //    soliodStoryboard . Begin ( );
        //}

        ///// <summary>
        ///// 常态动画第二阶段
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void SoliodAnimation02Completed ( object sender , object e ) {
        //    soliodStoryboard . Completed -= SoliodAnimation02Completed;
        //    StartScaleStoryboardSoliod ( AnimFrom , AnimTo01 , 2 , 1 );
        //    StartScaleStoryboardCenterSoliod ( CenterFrom , CenterTo );
        //    soliodStoryboard . Completed += SoliodAnimation01Completed;
        //    soliodStoryboard . Begin ( );
        //}

        /// <summary>
        /// 区域内点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Obj_Click ( object sender , RoutedEventArgs e ) {
            soliodStoryboard . Stop ( );
            if ( !AnalyticsInfo . VersionInfo . DeviceFamily . Equals ( "Windows.Mobile" ) ) {
                ( sender as FrameworkElement ) . PointerEntered -= Obj_PointerEntered;
                ( sender as FrameworkElement ) . PointerExited -= Obj_PointerExited;
            }
            if ( !IsBoardOpened ) {
                ( sender as Button ) . Click -= Obj_Click;
                IsBoardOpened = true;
                if ( IsReceivedPointer ) {
                    StartScaleStoryboard ( AnimTo01 , AnimTo02 , 0.12 , 1 );
                    storyboard . Completed += OnClickCompleted;
                    storyboard . Begin ( );
                } else {
                    StartScaleStoryboard ( AnimFrom , AnimTo02 , 0.12 , 1 );
                    StartScaleStoryboardCenter ( CenterTo , CenterFrom );
                    storyboard . Completed += OnClickCompleted;
                    storyboard . Begin ( );
                }
            } else {
                IsBoardOpened = false;
                if ( IsReceivedPointer ) {
                    StartScaleStoryboard ( AnimTo02 * 20 , AnimTo01 , 0.3 , 1 );
                    StartScaleStoryboardCenter ( CenterTo , CenterFrom );
                    storyboard . Completed += OnClickCloseCompleted;
                    storyboard . Begin ( );
                } else {
                    StartScaleStoryboard ( AnimTo02 * 20 , AnimFrom , 0.3 , 1 );
                    StartScaleStoryboardCenter ( CenterTo , CenterFrom );
                    storyboard . Completed += OnClickCloseCompleted;
                    storyboard . Begin ( );
                }
            }
        }

        /// <summary>
        /// 按钮点击完全取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickCloseCompleted ( object sender , object e ) {
            storyboard . Completed -= OnClickCloseCompleted;
            if ( !AnalyticsInfo . VersionInfo . DeviceFamily . Equals ( "Windows.Mobile" ) ) {
                ( AssociatedObject as FrameworkElement ) . PointerEntered += Obj_PointerEntered;
                ( AssociatedObject as FrameworkElement ) . PointerExited += Obj_PointerExited;
            }
            soliodStoryboard . Begin( );
        }

        /// <summary>
        /// 区域检测到点击事件初始化放大完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickCompleted ( object sender , object e ) {
            storyboard . Completed -= OnClickCompleted;
            StartScaleStoryboard ( AnimTo02 , AnimTo02 * 20 , 0.6 , 1 );
            StartScaleStoryboardCenter ( CenterTo , CenterFrom );
            storyboard . Completed += OnClickOpenCompleted;
            storyboard . Begin ( );
        }

        /// <summary>
        /// 点击后展开后的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickOpenCompleted ( object sender , object e ) {
            storyboard . Completed -= OnClickOpenCompleted;
            ( AssociatedObject as Button ) . Click += Obj_Click;
        }

        /// <summary>
        /// 光标离开区域事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Obj_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e) {
            if ( !IsReceivedPointer )
                return;
            if ( IsBoardOpened )
                return;
            StartScaleStoryboard ( AnimTo01 , AnimFrom , 0.4 , 2 );
            storyboard . Completed += OnPointerExitedCompleted;
            storyboard . Begin ( );
        }

        /// <summary>
        /// 光标完全退出区域
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPointerExitedCompleted ( object sender , object e ) {
            IsReceivedPointer = false;
            IsBoardOpened = false;
            storyboard . Completed -= OnPointerExitedCompleted;
        }

        /// <summary>
        /// 光标进入区域事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Obj_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e) {
            if ( IsBoardOpened )
                return;
            IsReceivedPointer = true;
            StartScaleStoryboard ( AnimFrom , AnimTo01 , 0.2 , 4 );
            StartScaleStoryboardCenter ( CenterFrom , CenterTo );
            storyboard . Completed += OnPointerEnteredCompleted;
            storyboard . Begin ( );
        }

        /// <summary>
        /// 光标进入区域完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPointerEnteredCompleted ( object sender , object e ) {
            storyboard . Completed -= OnPointerEnteredCompleted;
            //IsBoardOpened = true;
            //StartScaleStoryboard ( AnimTo , AnimFrom * 30 , 0.8 , 2 );
            //storyboard . Begin ( );
        }

        /// <summary>
        /// 设定动画属性参数
        /// </summary>
        /// <param name="from">起始数值</param>
        /// <param name="to">结束数值</param>
        /// <param name="time">总时长</param>
        /// <param name="exponent">内插指数</param>
        private void StartScaleStoryboard(double from, double to,double time,int exponent ) {
            _scaleYAnim . From = _scaleXAnim . From = from;
            _scaleYAnim . To = _scaleXAnim . To = to;
            _scaleYAnim . EasingFunction = _scaleXAnim . EasingFunction = new ExponentialEase { Exponent = exponent };
            _scaleYAnim . Duration = _scaleXAnim . Duration = new Duration ( TimeSpan . FromSeconds ( time ) );
       }

        /// <summary>
        /// 中心变化动画参数
        /// </summary>
        /// <param name="from">起始参数</param>
        /// <param name="to">终止参数</param>
        private void StartScaleStoryboardCenter ( double from , double to ) {
            centerX . From = centerY . From = from;
            centerX . To = centerY . To = to;
            centerX . EasingFunction = centerY . EasingFunction = new ExponentialEase { Exponent = 4 };
            centerX . Duration = centerY . Duration = new Duration ( TimeSpan . FromSeconds ( 0.3 ) );
        }

        /// <summary>
        /// 设定常态动画属性
        /// </summary>
        /// <param name="from">起始数值</param>
        /// <param name="to">结束数值</param>
        /// <param name="time">总时长</param>
        /// <param name="exponent">内插指数</param>
        private void StartScaleStoryboardSoliod ( double from , double to , double time , int exponent ) {
            soliod_scaleYAnim . From = soliod_scaleXAnim . From = from;
            soliod_scaleYAnim . To = soliod_scaleXAnim . To = to;
            soliod_scaleYAnim . EasingFunction = soliod_scaleXAnim . EasingFunction = new ExponentialEase { Exponent = exponent };
            soliod_scaleYAnim . Duration = soliod_scaleXAnim . Duration = new Duration ( TimeSpan . FromSeconds ( time ) );
        }

        /// <summary>
        /// 常态中心变化动画参数
        /// </summary>
        /// <param name="from">起始参数</param>
        /// <param name="to">终止参数</param>
        private void StartScaleStoryboardCenterSoliod ( double from , double to ) {
            soliodcenterX . From = soliodcenterY . From = from;
            soliodcenterX . To = soliodcenterY . To = to;
            soliodcenterX . EasingFunction = soliodcenterY . EasingFunction = new ExponentialEase { Exponent = 4 };
            soliodcenterX . Duration = soliodcenterY . Duration = new Duration ( TimeSpan . FromSeconds ( 0.3 ) );
        }

        /// <summary>
        /// Detach方法是当Behavior与关联的元素分离时要执行的代码
        /// </summary>
        public void Detach ( ) {
            var element = AssociatedObject as FrameworkElement;
            if ( element == null )
                return;
            if ( !AnalyticsInfo . VersionInfo . DeviceFamily . Equals ( "Windows.Mobile" ) ) {
                element . PointerEntered -= Obj_PointerEntered;
                element . PointerExited -= Obj_PointerExited;
            }
            if ( element is Button )
                ( element as Button ) . Click -= Obj_Click;
        }

        /// <summary>
        /// AssociatedObject是获取与Behavior关联起来的元素对象
        /// </summary>
        public DependencyObject AssociatedObject { get; private set; }

    }
}
