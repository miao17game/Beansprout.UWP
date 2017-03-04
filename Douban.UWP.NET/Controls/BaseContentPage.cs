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
    /// <summary>
    /// The base type of the page which can show the slide-in and out animations in default state.
    /// This class' instance should be contained in the content frames or pages.
    /// Not the main frames or pages.
    /// </summary>
    public class BaseContentPage : Page {

        public BaseContentPage() {
            transToSideGrid = this.RenderTransform as TranslateTransform;
            if (transToSideGrid == null) this.RenderTransform = transToSideGrid = new TranslateTransform();
            InitPageState();
        }

        /// <summary>
        /// Override to do work when page is prepared, if need some work to do.
        /// </summary>
        protected virtual void InitPageState() {
            // FOR OVERRIDE ...
        }

        #region Page Animations

        /// <summary>
        /// Page slide in animations.
        /// </summary>
        public void InitSlideInBoard() {
            doubleAnimation = new DoubleAnimation() {
                Duration = new Duration(TimeSpan.FromMilliseconds(220)),
                From = this.ActualWidth,
                To = 0,
            };
            doubleAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            doubleAnimation.Completed += DoublAnimationSlideIn_Completed;
            storyToSideGridIn = new Storyboard();
            Storyboard.SetTarget(doubleAnimation, transToSideGrid);
            Storyboard.SetTargetProperty(doubleAnimation, "X");
            storyToSideGridIn.Children.Add(doubleAnimation);
        }

        /// <summary>
        /// Page slide out animations.
        /// </summary>
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

        /// <summary>
        /// Do some work to dispose state when out-storyboard stopped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoublAnimationSlideOut_Completed(object sender, object e) {
            storyToSideGridOut.Stop();
            doubleAnimation.Completed -= DoublAnimationSlideOut_Completed;
            DoWorkWhenAnimationCompleted();
        }

        /// <summary>
        /// Dispose the content of MainContentFrame in default definition. Override this method if need.
        /// </summary>
        public virtual void DoWorkWhenAnimationCompleted() {
            AppResources.MainContentFrame.Content = null;
        }

        /// <summary>
        /// Do some work to dispose state when in-storyboard stopped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoublAnimationSlideIn_Completed(object sender, object e) {
            storyToSideGridIn.Stop();
            doubleAnimation.Completed -= DoublAnimationSlideIn_Completed;
        }

        /// <summary>
        /// Override to do the sepcific slide-out work for each platform, if need.
        /// </summary>
        /// <param name="isToLeft"></param>
        public virtual void PageSlideOutStart(bool isToLeft) {
            InitSlideOutBoard(isToLeft);
            storyToSideGridOut.Begin();
        }

        /// <summary>
        /// The open method for the page slide-in animations.
        /// </summary>
        public virtual void StartPageInAnima() {
            InitSlideInBoard();
            storyToSideGridIn.Begin();
        }
        #endregion

        #region Properties
        internal bool isFirstLoaded = true;
        internal bool isDivideScreen = true;
        internal Uri currentUri;
        Storyboard storyToSideGridIn;
        Storyboard storyToSideGridOut;
        TranslateTransform transToSideGrid;
        DoubleAnimation doubleAnimation;
        #endregion

    }
}
