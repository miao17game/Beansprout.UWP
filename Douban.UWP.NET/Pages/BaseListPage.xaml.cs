using static Wallace.UWP.Helpers.Tools.UWPStates;
using static Douban.UWP.NET.Resources.AppResources;

using Wallace.UWP.Helpers.Tools;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Douban.UWP.Core.Models;
using System.Text;
using System.Threading.Tasks;
using Wallace.UWP.Helpers.DataVirtualization;
using Windows.UI.Xaml.Media.Animation;
using Douban.UWP.NET.Tools;

namespace LNU.NET.Pages {

    public sealed partial class BaseListPage : Page {

        #region Constructor
        public BaseListPage() {
            this.InitializeComponent();
        }
        #endregion

        #region Events
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            InitViewAsync();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Init the Page Initual databundle resources.
        /// </summary>

        private async void InitViewAsync() {
            //StringBuilder urlString = await WebProcess.GetHtmlResources(HomeHost);
            //if (urlString == null)
            //    return;
            //var headerList = DataHandler.SetHeaderGroupResources(urlString.ToString());
            //foreach (var item in headerList) {
            //    cacheDic.Add(
            //        item.Title,
            //        new IncrementalLoadingContext<ContentListModel>(
            //            FetchMoreResources,
            //            item.Number,
            //            15,
            //            HomeHost,
            //            InitSelector.Special));
            //}
            //HeaderResources.Source = headerList;

            //if (headerList == null)
            //    return;
        }

        //private async Task<List<InfosListItem>> FetchMoreResources(int number, uint rollNum, uint nowWholeCountX) {
        //    var Host = "http://www.dongqiudi.com?tab={0}&page={1}";
        //    Host = string.Format(
        //        Host,
        //        number,
        //        nowWholeCountX / 15);
        //    return await DataHandler.SetHomeListResources(Host);
        //}

        #endregion

        #region Events



        private void ListView_ItemClick(object sender, ItemClickEventArgs e) {

            //var itemUri = (e.ClickedItem as ContentListModel).Path;
            //var itemNum = (e.ClickedItem as ContentListModel).ID;
            //MainPage.Current.ItemClick?.Invoke(
            //    this,
            //    typeof(ContentPage),
            //    MainPage.Current.ContFrame,
            //    itemUri,
            //    itemNum,
            //    null)
            //MainPage.Current.SideGrid.Visibility = Visibility.Visible;

        }



        private void MyPivot_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            //var item = (sender as Pivot).SelectedItem as HeaderModel;
            //var num = MyPivot.SelectedIndex;
            //nowItem = item.Title;
            //itemNumber = item.Number;
            //if (!cacheDic.ContainsKey(item.Title)) {
            //    homeLlistResources =
            //        new DQDDataContext<ContentListModel>(
            //            FetchMoreResources,
            //            item.Number,
            //            15,
            //            HomeHost,
            //            InitSelector.Special);
            //    cacheDic.Add(item.Title, homeLlistResources);
            //}

            //ListResources.Source = cacheDic[nowItem];
        }

        //private void RefreshBtn_Click(object sender, RoutedEventArgs e) {
        //    ListResources.Source =
        //        cacheDic[nowItem] =
        //        new DQDDataContext<ContentListModel>(
        //            FetchMoreResources,
        //            itemNumber,
        //            15,
        //            HomeHost,
        //            InitSelector.Special);

        //}



        //private void BackToTopBtn_Click(object sender, RoutedEventArgs e) {

        //    int num = MyPivot.SelectedIndex;

        //    MainPage.GetScrollViewer(

        //        MainPage.GetPVItemViewer(

        //            MyPivot, ref num))

        //            .ChangeView(0, 0, 1);

        //}



        private void grid_SizeChanged(object sender, SizeChangedEventArgs e) { MyPivot.Width = (sender as Grid).ActualWidth; }



        private void LocalPageListView_Loaded(object sender, RoutedEventArgs e) {
            var num = MyPivot.SelectedIndex;
            scroll = GlobalHelpers.GetScrollViewer(GlobalHelpers.GetPVItemViewer(MyPivot, ref num));
            scroll.ViewChanged += ScrollViewer_ViewChanged;
            scroll.ViewChanged += ScrollViewerChangedForFlip;
            loadingAnimation.IsActive = false;
        }

        #endregion

        #region Animations

        #region Animations Properties

        public StackPanel ButtonThisPage { get; private set; }
        private ScrollViewer scroll;
        private Dictionary<string, double> listViewOffset = new Dictionary<string, double>();
        Storyboard BtnStackSlideIn = new Storyboard();
        Storyboard BtnStackSlideOut = new Storyboard();
        TranslateTransform transToButtonThisPage;
        DoubleAnimation doubleAnimation;
        bool IsAnimaEnabled;

        #endregion

        #region Handler of ListView Scroll 

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e) {

            try {

                if (!listViewOffset.ContainsKey(nowItem)) {

                    listViewOffset[nowItem] = (sender as ScrollViewer).VerticalOffset;

                    return;
                }

                if (listViewOffset[nowItem] - (sender as ScrollViewer).VerticalOffset < -10

                    && ButtonThisPage.Visibility == Visibility.Visible

                    && IsAnimaEnabled) {

                    scroll.ViewChanged -= ScrollViewer_ViewChanged;

                    BtnStackSlideOut.Begin();

                } else if (listViewOffset[nowItem] - (sender as ScrollViewer).VerticalOffset > 10

                    && ButtonThisPage.Visibility == Visibility.Collapsed

                    && IsAnimaEnabled) {

                    scroll.ViewChanged -= ScrollViewer_ViewChanged;

                    ButtonThisPage.Visibility = Visibility.Visible;

                    BtnStackSlideIn.Begin();
                }

                listViewOffset[nowItem] = (sender as ScrollViewer).VerticalOffset;

            } catch { Debug.WriteLine("Save scroll positions error."); }

        }



        private void ScrollViewerChangedForFlip(object sender, ScrollViewerViewChangedEventArgs e) {

            try {

                if ((sender as ScrollViewer).VerticalOffset <= 240)

                    MyFlip.Margin = new Thickness(0, -(sender as ScrollViewer).VerticalOffset, 0, 0);

                if ((sender as ScrollViewer).VerticalOffset > 240)

                    MyFlip.Margin = new Thickness(0, -240, 0, 0);

            } catch { Debug.WriteLine("Save scroll positions error."); }

        }

        #endregion

        #endregion

        #region Properties and States

        public static BaseListPage Current { get; private set; }

        private ProgressRing loadingAnimation;

        private Dictionary<string, IncrementalLoadingContext<InfosListItem>> cacheDic;

        private IncrementalLoadingContext<InfosListItem> homeLlistResources;

        private delegate Task<List<InfosListItem>> HeadListActionEvent();

        private const string HomeHost = "http://www.dongqiudi.com/";

        private const string HomeHostInsert = "http://www.dongqiudi.com";

        private string nowItem;

        private int itemNumber;



        #endregion

    }
}
