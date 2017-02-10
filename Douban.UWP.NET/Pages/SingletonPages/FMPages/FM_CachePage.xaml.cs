using static Wallace.UWP.Helpers.Tools.UWPStates;
using static Douban.UWP.NET.Resources.AppResources;

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
using Douban.UWP.Core.Models.FMModels;
using Douban.UWP.NET.Tools;
using Douban.UWP.Core.Models;

namespace Douban.UWP.NET.Pages.SingletonPages.FMPages {

    public sealed partial class FM_CachePage : Page {
        public FM_CachePage() {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            var query = await StorageHelper.GetAllStorageFilesByExtensionAsync(StorageHelper.JsonExtension);
            var list = new List<MHzSongBase>();
            foreach(var storage in query) {
                list.Add(await StorageHelper.ReadSongModelFromStorageFileAsync(storage));
            }
            ListResources.Source = list;
            IncrementalLoadingBorder.SetVisibility(false);
        }

        private void IndexList_Loaded(object sender, RoutedEventArgs e) {

        }

        private async void IndexList_ItemClickAsync(object sender, ItemClickEventArgs e) {
            var item = e.ClickedItem as MHzSongBase;
            if (item == null)
                return;
            var succeedss = Service.ChangeServiceChoice(MusicServiceType.SongList);
            if (!succeedss)
                return;
            var succeed = await Service.InsertItemAsync(item);
            if (!succeed)
                return;
            if (MainUpContentFrame.Content != null)
                (MainUpContentFrame.Content as FM_SongBoardPage)?.UnregisterServiceEvents();
            Service.SongListMoveTo();
            NavigateToBase?.Invoke(
                null,
                new MusicBoardParameter {
                    SID = item.SID,
                    SSID = item.SSID,
                    AID = item.AID,
                    SHA256 = item.SHA256,
                    Url = item.Url,
                    Song = item,
                    FrameType = FrameType.UpContent
                },
                GetFrameInstance(FrameType.UpContent),
                GetPageType(NavigateType.MusicBoard));
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e) {
            ReportHelper.ReportAttentionAsync(GetUIString("StillInDeveloping"));
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e) {
            ReportHelper.ReportAttentionAsync(GetUIString("StillInDeveloping"));
        }

    }
}
