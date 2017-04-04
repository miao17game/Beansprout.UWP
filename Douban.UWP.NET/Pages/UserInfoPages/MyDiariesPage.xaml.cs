using static Wallace.UWP.Helpers.Tools.UWPStates;
using static Douban.UWP.NET.Resources.AppResources;

using Douban.UWP.Core.Models;
using Douban.UWP.NET.Controls;
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
using Wallace.UWP.Helpers;
using Douban.UWP.NET.Tools;
using Douban.UWP.Core.Tools;
using Newtonsoft.Json.Linq;
using Douban.UWP.Core.Models.LifeStreamModels;
using System.Threading.Tasks;

namespace Douban.UWP.NET.Pages {

    public sealed partial class MyDiariesPage : BaseContentPage {
        public MyDiariesPage() {
            this.InitializeComponent();
        }

        protected override void InitPageState() {
            base.InitPageState();
            GlobalHelpers.SetChildPageMargin(this, matchNumber: VisibleWidth, isDivideScreen: IsDivideScreen);
            GlobalHelpers.DivideWindowRange(this, DivideNumber, isDivideScreen: IsDivideScreen);
        }

        #region Events

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            uid = (e.Parameter as NavigateParameter).UserUid;
            ListResources.Source = new DoubanIncrementalContext<DiaryItem>(FetchMoreResourcesAsync);
        }

        private void BaseHamburgerButton_Click(object sender, RoutedEventArgs e) {
            PageSlideOutStart(VisibleWidth > FormatNumber ? false : true);
        }

        private void ContentList_ItemClick(object sender, ItemClickEventArgs e) {
            var iten = e.ClickedItem as DiaryItem;
            if (iten == null)
                return;
            Uri.TryCreate(string.Format(NavigateUrlFormat, iten.ID), UriKind.RelativeOrAbsolute, out var uri);
            NavigateToBase?.Invoke(
                null,
                new NavigateParameter { Title = iten.Title, ToUri = uri, IsFromInfoClick = true },
                UserInfoDetails,
                GetPageType(NavigateType.InfoItemClick));
            UserInfoPopup.IsOpen = false;
        }

        #endregion

        #region Methods

        private async Task<IList<DiaryItem>> FetchMessageFromAPIAsync(string target, int offset) {
            IList<DiaryItem> list = new List<DiaryItem>();
            try {
                var result = await DoubanWebProcess.GetMDoubanResponseAsync(target);
                if (result == null) {
                    ReportHelper.ReportAttentionAsync(GetUIString("WebActionError"));
                    IncrementalLoadingBorder.SetVisibility(false);
                    return list;
                }
                JObject jo = JObject.Parse(result);
                var notes = jo["notes"];
                if (notes == null ) {
                    ReportHelper.ReportAttentionAsync(GetUIString("FetchJsonDataError"));
                    IncrementalLoadingBorder.SetVisibility(false);
                    return list;
                }
                if (notes.HasValues) 
                    notes.Children().ToList().ForEach(noteItem => WorkOnEachNote(noteItem, list));
            } catch { ReportHelper.ReportAttentionAsync(GetUIString("UnknownError")); }
            IncrementalLoadingBorder.SetVisibility(false);
            needContinue = list.Count != 0 ? list.Count == fetchCount : false;
            if(!needContinue)
                ReportHelper.ReportAttentionAsync(GetUIString("SHOULD_STOP"));
            startCount += (offset + 1) * fetchCount;
            return list;
        }

        private void WorkOnEachNote(JToken noteItem, IList<DiaryItem> list) {
            try {
                var item = JsonHelper.FromJson<DiaryItem>(noteItem.ToString());
                item.Type = InfosItemBase.JsonType.Note;
                item.HasCover = noteItem["cover_url"].Value<string>() == "" ? false : true;
                list.Add(item);
            } catch(Exception e) { System.Diagnostics.Debug.WriteLine($"DiaryItem add error : {e.Message + e.StackTrace}"); }
        }

        private async Task<IList<DiaryItem>> FetchMoreResourcesAsync(int offset) {
            if (!needContinue)
                return new List<DiaryItem>();
            IncrementalLoadingBorder.SetVisibility(true);
            return await FetchMessageFromAPIAsync(string.Format(Host, uid, startCount, fetchCount), offset);
        }

        #endregion

        #region Properties and state

        int fetchCount = 20;
        int startCount = 0;
        string uid;
        bool needContinue = true;
        const string Host = "https://m.douban.com/rexxar/api/v2/user/{0}/notes?start={1}&count={2}&for_mobile=1";
        const string NoPictureUrl = "https://www.none-wallace-767fc6vh7653df0jb.com/no_wallace_085sgdfg7447fddds65.jpg";
        const string NavigateUrlFormat = "https://m.douban.com/note/{0}/";

        #endregion

    }
}
