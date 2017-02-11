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

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            this.DataContext = Downloader;
            IncrementalLoadingBorder.SetVisibility(false);
            timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 1) };
            timer.Tick += OnTicked;
            timer.Start();
        }

        private void OnTicked(object sender, object e) {
            Downloader.DownloadList.Select(i=>i.Value).ToList().ForEach(singleton => {
                if (singleton.IsCompleted)
                    Downloader.RemoveItemFromListByValue(singleton);
                singleton.RefrashProgress();
            });
        }

        private void IndexList_Loaded(object sender, RoutedEventArgs e) {

        }

        private void IndexList_ItemClickAsync(object sender, ItemClickEventArgs e) {
            
        }

        private void StartButton_Click(object sender, RoutedEventArgs e) {
            var operation = ((sender as Button).CommandParameter as DownloadOperationValue).Operation;
            operation.Resume();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e) {
            var operation = ((sender as Button).CommandParameter as DownloadOperationValue).Operation;
            operation.Pause();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e) {
            var item = (KeyValuePair<DownloadOperationKey, DownloadOperationValue>)((sender as Button).CommandParameter);
            try { item.Value.Operation.Pause(); } catch { /* Do nothing. */}
            Downloader.DownloadList.Remove(item);
        }

        #region Properties

        DispatcherTimer timer;

        #endregion

    }
}
