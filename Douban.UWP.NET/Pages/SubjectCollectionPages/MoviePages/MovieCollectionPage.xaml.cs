using static Wallace.UWP.Helpers.Tools.UWPStates;
using static Douban.UWP.NET.Resources.AppResources;

using Douban.UWP.Core.Models.ListModel;
using System;
using System.Collections.Generic;
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
using System.Threading.Tasks;
using Douban.UWP.Core.Tools;
using Newtonsoft.Json.Linq;
using Douban.UWP.NET.Tools;
using Douban.UWP.Core.Models;
using System.Diagnostics;
using Douban.UWP.NET.Controls;
using Wallace.UWP.Helpers;

namespace Douban.UWP.NET.Pages.SubjectCollectionPages {

    public sealed partial class MovieCollectionPage : BaseContentPage {
        public MovieCollectionPage() {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            InitWhenNavigated(e);
        }

        private void ItemsGridView_ItemClick(object sender, ItemClickEventArgs e) {
            var item = e.ClickedItem as MovieItem;
            if (item == null || item.PathUrl == null)
                return;
            NavigateToBase?.Invoke(
                null,
                new NavigateParameter {
                    ToUri = new Uri(UriDecoder.GetUrlFromUri(item.PathInnerUri, UriCastEnum.Movie)),
                    Title = item.Title,
                    IsNative = true,
                    FrameType = FrameType.UpContent },
                GetFrameInstance(FrameType.UpContent),
                GetPageType(NavigateType.MovieContent));
        }

        private void BaseHamburgerButton_Click(object sender, RoutedEventArgs e) {
            PageSlideOutStart(VisibleWidth > FormatNumber ? false : true);
        }

        private void InitWhenNavigated(NavigationEventArgs args) {
            var para = args.Parameter as NavigateParameter;
            if (para == null)
                return;
            navigateTitlePath.Text = para.Title ?? GetUIString("LinkContent");
            api_head = para.ApiHeadString ?? "";
            frameType = para.FrameType;
            GridViewResources.Source = new DoubanIncrementalContext<MovieItem>(DivideGroupAsync);
        }

        private async Task<IList<MovieItem>> DivideGroupAsync(int offset = 0) {
            var group = await SetGridViewResourcesAsync(offset);
            return group != null ? group.Items : empty;
        }

        private async Task<ItemGroup<MovieItem>> SetGridViewResourcesAsync(int offset, int count = 20) {
            try {
                IncrementalLoadingBorder.SetVisibility(true);
                if (isFinished || (total != 0 && offset * count >= total))
                    return null;
                return await FetchMessageFromAPIAsync(
                    formatAPI: FormatPath,
                    headString: api_head,
                    start: (uint)(offset * count),
                    count: (uint)count,
                    loc_id: "108288");
            } finally { IncrementalLoadingBorder.SetVisibility(false); }
        }

        private async Task<ItemGroup<MovieItem>> FetchMessageFromAPIAsync(
            string formatAPI,
            string headString,
            string loc_id = "108288",
            uint start = 0,
            uint count = 8,
            int offset = 0) {
            var gmodel = default(ItemGroup<MovieItem>);
            try {
                var minised = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                var result = await DoubanWebProcess.GetMDoubanResponseAsync(string.Format(formatAPI, new object[] { headString, start, count, loc_id, minised }),
                    "m.douban.com",
                    "https://m.douban.com/movie/");
                if (result != null) {
                    gmodel = JsonHelper.FromJson<ItemGroup<MovieItem>>(result);
                    if (gmodel.Items == null || gmodel.Items.Count < count)
                        isFinished = true;
                } else {
                    ReportWhenGoesWrong("WebActionError");
                    return gmodel;
                }
            } catch { ReportHelper.ReportAttentionAsync(GetUIString("FetchJsonDataError")); }
            return gmodel;
        }

        private void ReportWhenGoesWrong(string UIString) {
            ReportHelper.ReportAttentionAsync(GetUIString(UIString));
        }

        #region Override Methods

        protected override void InitPageState() {
            base.InitPageState();
            GlobalHelpers.SetChildPageMargin(this, matchNumber: VisibleWidth, isDivideScreen: IsDivideScreen);
            GlobalHelpers.DivideWindowRange(this, DivideNumber, isDivideScreen: IsDivideScreen);
        }

        public override void DoWorkWhenAnimationCompleted() {
            AdaptToClearContentAfterOnBackPressed();
        }

        private void AdaptToClearContentAfterOnBackPressed() {
            if (VisibleWidth > FormatNumber && IsDivideScreen && MainMetroFrame.Content == null)
                MainMetroFrame.Navigate(typeof(MetroPage));
            GetFrameInstance(frameType).Content = null;
        }

        #endregion

        #region State
        string api_head;
        bool isFinished;
        int total = 0;
        FrameType frameType = FrameType.Content;
        IList<MovieItem> empty = new List<MovieItem>();
        private string LocationUid { get { return IsLogined ? LoginStatus.APIUserinfos?.LocationUid : "108288"; }}
        const string FormatPath = "https://m.douban.com/rexxar/api/v2/{0}items?os=android&start={1}&count={2}&loc_id={3}&_={4}";
        #endregion

    }
}
