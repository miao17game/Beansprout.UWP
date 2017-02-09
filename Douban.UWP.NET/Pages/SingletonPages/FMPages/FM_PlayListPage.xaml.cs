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
using Douban.UWP.NET.Tools;
using Douban.UWP.Core.Models.FMModels.MHzSongListModels;
using Douban.UWP.Core.Models.FMModels;
using Douban.UWP.Core.Models;

namespace Douban.UWP.NET.Pages.SingletonPages.FMPages {

    public sealed partial class FM_PlayListPage : Page {
        public FM_PlayListPage() {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            this.DataContext = Service;
        }

        private void IndexList_Loaded(object sender, RoutedEventArgs e) {
            
        }

        private void IndexList_ItemClick(object sender, ItemClickEventArgs e) {
            var item = e.ClickedItem as MHzSongBase;
            if (item == null)
                return;
            var succeed = Service.InsertItem(item);
            if (!succeed)
                return;
            Service.MoveToAnyway(item);
            if (MainUpContentFrame.Content != null)
                (MainUpContentFrame.Content as FM_SongBoardPage)?.UnregisterServiceEvents();
            NavigateToBase?.Invoke(
                null,
                new MusicBoardParameter {
                    SID = item.SID,
                    SSID = item.SSID,
                    AID = item.AID,
                    SHA256 = item.SHA256,
                    FrameType = FrameType.UpContent
                },
                GetFrameInstance(FrameType.UpContent),
                GetPageType(NavigateType.MusicBoard));
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e) {
            ReportHelper.ReportAttentionAsync(GetUIString("StillInDeveloping"));
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e) {
            var succeed = Service.RemoveItem((sender as Button).CommandParameter as MHzSongBase);
            if (!succeed)
                ReportHelper.ReportAttentionAsync(GetUIString("Delete_Music_Failed"));
        }
    }
}
