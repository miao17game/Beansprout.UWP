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

namespace Douban.UWP.NET.Pages.SubjectCollectionPages {

    public sealed partial class BookCollectionPage : BaseContentPage {
        public BookCollectionPage() {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            InitWhenNavigated(e);
        }

        private void ItemsGridView_ItemClick(object sender, ItemClickEventArgs e) {
            var item = e.ClickedItem as BookItem;
            if (item == null || item.PathUrl == null)
                return;
            NavigateToBase?.Invoke(
                null,
                new NavigateParameter { ToUri = new Uri(item.PathUrl), Title = item.Title, FrameType = FrameType.UpContent },
                GetFrameInstance(FrameType.UpContent),
                GetPageType(NavigateType.BookContent));
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
            GridViewResources.Source = new DoubanIncrementalContext<BookItem>(DivideGroupAsync);
        }

        private async Task<IList<BookItem>> DivideGroupAsync(int offset = 0) {
            var group = await SetGridViewResourcesAsync(offset);
            return group != null ? group.Items : empty;
        }

        private async Task<ItemGroup<BookItem>> SetGridViewResourcesAsync(int offset, int count = 20) {
            if ((total != 0 && offset * count >= total) || isFinished)
                return null;
            return await FetchMessageFromAPIAsync(
                headString: api_head,
                start: (uint)(offset * count),
                count: (uint)count,
                loc_id: "108288");
        }

        private async Task<ItemGroup<BookItem>> FetchMessageFromAPIAsync(
            string headString,
            string loc_id = "108288",
            uint start = 0,
            uint count = 8,
            int offset = 0) {
            var gmodel = default(ItemGroup<BookItem>);
            try {
                var minised = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                var result = await BeansproutRequestHelper.FetchTypeCollectionList(headString, loc_id, start, count, minised, SubjectType.Books);
                if (result == null) {
                    ReportWhenGoesWrong("WebActionError");
                    return gmodel;
                }
                JObject jo = JObject.Parse(result);
                gmodel = DataProcess.SetGroupResources(jo, gmodel);
                gmodel = SetSingletonResources(jo, gmodel);
            } catch { ReportHelper.ReportAttentionAsync(GetUIString("FetchJsonDataError")); }
            IncrementalLoadingBorder.SetVisibility(false);
            return gmodel;
        }

        

        private ItemGroup<BookItem> SetSingletonResources(JObject jObject, ItemGroup<BookItem> gModel) {
            var header = jObject["header"];
            if (header != null && header.HasValues) 
                DataProcess.SetEachSingleton(gModel, header);
            var feeds = jObject["subject_collection_items"];
            if (feeds == null || !feeds.HasValues) {
                isFinished = true;
                return gModel;
            }
            if (feeds.HasValues)
                feeds.Children().ToList().ForEach(singleton => DataProcess.SetEachSingleton(gModel, singleton));
            return gModel;
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
        IList<BookItem> empty = new List<BookItem>();
        private string LocationUid { get { return IsLogined ? LoginStatus.APIUserinfos?.LocationUid : "108288"; }}
        #endregion

    }
}
