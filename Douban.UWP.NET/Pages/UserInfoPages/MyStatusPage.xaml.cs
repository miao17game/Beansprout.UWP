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
using System.Diagnostics;

namespace Douban.UWP.NET.Pages {

    public sealed partial class MyStatusPage : BaseContentPage {
        public MyStatusPage() {
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
            ListResources.Source = new DoubanIncrementalContext<StatusItem>(FetchMoreResourcesAsync);
        }

        private void BaseHamburgerButton_Click(object sender, RoutedEventArgs e) {
            PageSlideOutStart(VisibleWidth > FormatNumber ? false : true);
        }

        private void ContentList_ItemClick(object sender, ItemClickEventArgs e) {
            var iten = e.ClickedItem as StatusItem;
            if (iten == null)
                return;
            Uri.TryCreate(string.Format(NavigateUrlFormat, uid, iten.ID), UriKind.RelativeOrAbsolute, out var uri);
            NavigateToBase?.Invoke(
                null,
                new NavigateParameter { Title = iten.Activity, ToUri = uri, IsFromInfoClick = true },
                UserInfoDetails,
                GetPageType(NavigateType.InfoItemClick));
            UserInfoPopup.IsOpen = false;
        }

        #endregion

        #region Methods

        private async Task<IList<StatusItem>> FetchMessageFromAPIAsync(string target) {
            IList<StatusItem> list = new List<StatusItem>();
            try {
                var result = await DoubanWebProcess.GetMDoubanResponseAsync(target);
                if (result == null) {
                    ReportHelper.ReportAttentionAsync(GetUIString("WebActionError"));
                    IncrementalLoadingBorder.SetVisibility(false);
                    return list;
                }
                JObject jo = JObject.Parse(result);
                var feeds = jo["items"];
                if (feeds == null ) {
                    ReportHelper.ReportAttentionAsync(GetUIString("FetchJsonDataError"));
                    IncrementalLoadingBorder.SetVisibility(false);
                    return list;
                }
                if (feeds.HasValues) {
                    feeds.Children().ToList().ForEach(singleton => {
                        try {
                            var status = singleton["status"];
                            if (!status.HasValues)
                                return;
                            var item = JsonHelper.FromJson<StatusItem>(status.ToString());
                            item.Type = InfosItemBase.JsonType.Status;
                            list.Add(item);
                        } catch(Exception e) { Debug.WriteLine(e.StackTrace + Environment.NewLine + "\n Error_json content:\n" + Environment.NewLine); }
                    });
                }
            } catch { ReportHelper.ReportAttentionAsync(GetUIString("UnknownError")); }
            IncrementalLoadingBorder.SetVisibility(false);
            max_id = list.Count != 0 ? (list[list.Count-1].ID - 1).ToString() : "SHOULD_STOP";
            if(list.Count == 0)
                ReportHelper.ReportAttentionAsync(GetUIString("SHOULD_STOP"));
            return list;
        }

        private async Task<IList<StatusItem>> FetchMoreResourcesAsync(int offset) {
            if (max_id == "SHOULD_STOP")
                return empty;
            IncrementalLoadingBorder.SetVisibility(true);
            return await FetchMessageFromAPIAsync(string.Format(Host, uid, max_id, 20));
        }

        #endregion

        #region Properties and state

        string uid;
        string max_id = "0";
        const string Host = "https://m.douban.com/rexxar/api/v2/status/user_timeline/{0}?max_id={1}&count={2}&for_mobile=1";
        const string NoPictureUrl = "https://www.none-wallace-767fc6vh7653df0jb.com/no_wallace_085sgdfg7447fddds65.jpg";
        const string NavigateUrlFormat = "https://m.douban.com/people/{0}/status/{1}/";

        List<StatusItem> empty = new List<StatusItem>();

        #endregion

    }
}
