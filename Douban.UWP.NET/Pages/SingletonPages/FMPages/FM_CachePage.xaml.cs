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
using Douban.UWP.Core.Models.FMModels.MHzSongListModels;
using Windows.Storage;
using System.Collections.ObjectModel;

namespace Douban.UWP.NET.Pages.SingletonPages.FMPages {

    public sealed partial class FM_CachePage : Page {
        public FM_CachePage() {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            json_query = await StorageHelper.GetAllStorageFilesByExtensionAsync(StorageHelper.JsonExtension);
            music_cache_list = new ObservableCollection<MHzSongBase>();
            foreach(var storage in json_query) {
                try { music_cache_list.Add(await StorageHelper.ReadSongModelFromStorageFileAsync(storage)); } catch{ /* Ignore */ }
            }
            ListResources.Source = music_cache_list;
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

        private async void AddButton_ClickAsync(object sender, RoutedEventArgs e) {
            var item = (sender as Button).CommandParameter as MHzSongBase;
            if (item == null)
                return;
            var succeed = await Service.InsertItemAsync(item);
            if (succeed)
                ReportHelper.ReportAttentionAsync(GetUIString("Music_added"));
        }

        private async void DeleteButton_ClickAsync(object sender, RoutedEventArgs e) {
            var item = (sender as Button).CommandParameter as MHzSongBase;
            if (item == null)
                return;
            var item_json = json_query.ToList().Find(i => i.Path == item.LocalPath);
            if (item_json == null)
                return;
            bool succeed = false;
            try {
                var music_file = await StorageHelper.FetchStorageFileByPathAsync(item_json.Path.Replace(StorageHelper.JsonExtension, StorageHelper.MusicExtension));
                await music_file.DeleteAsync();
                await item_json.DeleteAsync();
                succeed = true;
            } catch { ReportHelper.ReportAttentionAsync(GetUIString("Clear_Cache_Failed")); }
            if (succeed)
                music_cache_list.Remove(item);
        }

        ObservableCollection<MHzSongBase> music_cache_list;
        IReadOnlyList<StorageFile> json_query;

    }
}
