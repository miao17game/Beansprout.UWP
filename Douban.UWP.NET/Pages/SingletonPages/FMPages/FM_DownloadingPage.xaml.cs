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

    public sealed partial class FM_DownloadingPage : Page {
        public FM_DownloadingPage() { 
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            this.DataContext = Downloader;
            IncrementalLoadingBorder.SetVisibility(false);
        }

        private void IndexList_Loaded(object sender, RoutedEventArgs e) {

        }

        private async void IndexList_ItemClickAsync(object sender, ItemClickEventArgs e) {
            var item = e.ClickedItem as MHzSongBase;
            if (item == null)
                return;
            
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e) {
            ReportHelper.ReportAttentionAsync(GetUIString("StillInDeveloping"));
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e) {
            ReportHelper.ReportAttentionAsync(GetUIString("StillInDeveloping"));
        }

    }
}
