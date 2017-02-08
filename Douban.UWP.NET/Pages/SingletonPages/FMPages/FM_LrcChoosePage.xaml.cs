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
using Windows.UI;

namespace Douban.UWP.NET.Pages.SingletonPages.FMPages {

    public sealed partial class FM_LrcChoosePage : Page {
        public FM_LrcChoosePage() {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            var args = e.Parameter as IEnumerable<LrcMetaData>;
            if (args == null)
                return;
            ListResources.Source = args;
        }

        private void IndexList_Loaded(object sender, RoutedEventArgs e) {
            
        }

        private async void IndexList_ItemClick(object sender, ItemClickEventArgs e) {
            var item = e.ClickedItem as LrcMetaData;
            if (item == null)
                return;
            var lrc = await LrcProcessHelper.FetchLrcByIdAsync(item.ID);
            if (lrc == null)
                return;
            if(MainUpContentFrame.Content is FM_SongBoardPage) {
                var board_page = MainUpContentFrame.Content as FM_SongBoardPage;
                board_page.VMForPublic.LrcList = await LrcProcessHelper.ReadLRCFromWebAsync(null, null, Colors.White, lrc);
                await board_page.SetDefaultLrcAndAnimationsAsync(false);
                board_page.CloseInnerContentPanel();
                board_page.ResetCanvasViewVisibility();
            }
        }

    }
}
