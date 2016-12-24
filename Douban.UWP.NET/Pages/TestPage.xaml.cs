using Douban.UWP.Core.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Douban.UWP.NET.Pages {
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class TestPage : Page {
        public TestPage() {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            var target = "https://m.douban.com/rexxar/api/v2/recommend_feed?alt=json&next_date=2016-12-25&loc_id=&gender=&birthday=&udid=&for_mobile=true";
            //var result = await DoubanWebProcess.GetDoubanResponseAsync(target, ecd: Encoding.Unicode);
            var result = await DoubanWebProcess.GetMDoubanResponseAsync(target);
            JObject jo = JObject.Parse(result);
            var count = jo["date"];
            TestMessage.Text = count.Value<string>();
            System.Diagnostics.Debug.WriteLine(jo.ToString());
        }
    }
}
