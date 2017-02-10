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
using Douban.UWP.NET.Pages.SpecificPages;

namespace Douban.UWP.NET.Pages.SingletonPages.FMPages {

    public sealed partial class FMPage : Page {
        public FMPage() {
            this.InitializeComponent();
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var item = (sender as Pivot).SelectedItem as PivotItem;
            Frame frame = default(Frame);
            Type type = default(Type);
            switch (item.Name) {
                case "ChannelsItem":
                    frame = ChannelsFrame;
                    type = typeof(FMIndexPage);
                    break;
                case "MusicListItem":
                    frame = MusicListFrame;
                    type = typeof(FM_MusicListPage);
                    break;
                case "RedHeartItem":
                    frame = RedHeartFrame;
                    type = typeof(StillOnWorkPage);
                    break;
                case "MineItem":
                    frame = MineFrame;
                    type = typeof(StillOnWorkPage);
                    break;
                case "CacheItem":
                    frame = CacheFrame;
                    type = typeof(FM_CachePage);
                    break;
                case "DownloadingItem":
                    frame = DownloadingFrame;
                    type = typeof(StillOnWorkPage);
                    break;
            }
            if (frame.Content == null)
                frame.Navigate(type);
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e) {

        }
    }
}
