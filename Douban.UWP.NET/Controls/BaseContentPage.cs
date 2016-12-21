
using static Wallace.UWP.Helpers.Tools.UWPStates;
using Douban.UWP.Core.Tools;
using Douban.UWP.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Douban.UWP.NET.Tools;
using Douban.UWP.NET.Resources;
using Wallace.UWP.Helpers;

namespace Douban.UWP.NET.Controls {
    public class BaseContentPage : Page {

        public BaseContentPage() {
            transToSideGrid = this.RenderTransform as TranslateTransform;
            if (transToSideGrid == null) this.RenderTransform = transToSideGrid = new TranslateTransform();
            InitPageState();
        }

        protected virtual void InitPageState() {
            // TO DO ...
        }

        #region Page Animations

        public void InitSlideInBoard() {
            doubleAnimation = new DoubleAnimation() {
                Duration = new Duration(TimeSpan.FromMilliseconds(220)),
                From = this.ActualWidth,
                To = 0,
            };
            doubleAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            doubleAnimation.Completed += DoublAnimation_Completed;
            storyToSideGrid = new Storyboard();
            Storyboard.SetTarget(doubleAnimation, transToSideGrid);
            Storyboard.SetTargetProperty(doubleAnimation, "X");
            storyToSideGrid.Children.Add(doubleAnimation);
        }

        public void InitSlideOutBoard(bool isToLeft) {
            doubleAnimation = new DoubleAnimation() {
                Duration = new Duration(TimeSpan.FromMilliseconds(220)),
                From = 0,
                To = isToLeft ? -this.ActualWidth : this.ActualWidth,
            };
            doubleAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            doubleAnimation.Completed += DoublAnimationSlideOut_Completed;
            storyToSideGridOut = new Storyboard();
            Storyboard.SetTarget(doubleAnimation, transToSideGrid);
            Storyboard.SetTargetProperty(doubleAnimation, "X");
            storyToSideGridOut.Children.Add(doubleAnimation);
        }

        private void DoublAnimationSlideOut_Completed(object sender, object e) {
            storyToSideGridOut.Stop();
            doubleAnimation.Completed -= DoublAnimation_Completed;
            AppResources.MainContentFrame.Content = null;
        }

        private void DoublAnimation_Completed(object sender, object e) {
            storyToSideGrid.Stop();
            doubleAnimation.Completed -= DoublAnimation_Completed;
        }

        public virtual void PageSlideOutStart(bool isToLeft) {
            InitSlideOutBoard(isToLeft);
            storyToSideGridOut.Begin();
        }

        public virtual void StartPageInAnima() {
            InitSlideInBoard();
            storyToSideGrid.Begin();
        }
        #endregion

        #region Properties
        internal bool isFirstLoaded = true;
        internal bool isDivideScreen = true;
        internal string navigateTitle;
        internal DataFetchType thisPageType;
        internal NavigateType thisNaviType;
        internal Uri currentUri;
        private Storyboard storyToSideGrid = new Storyboard();
        public Storyboard storyToSideGridOut = new Storyboard();
        TranslateTransform transToSideGrid;
        DoubleAnimation doubleAnimation;
        #endregion

    }
}
