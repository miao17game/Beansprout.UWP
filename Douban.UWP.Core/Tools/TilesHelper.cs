using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;

namespace Douban.UWP.Core.Tools {

    public class TilesHelper {
        public static async Task<SecondaryTile> PinNewSecondaryTileAsync() {
            SecondaryTile tile = GenerateSecondaryTile("SecondaryTitle", "Beansprout UWP");
            await tile.RequestCreateAsync();
            return tile;
        }

        public static SecondaryTile GenerateSecondaryTile(string tileId, string displayName, Windows.UI.Color color) {
            SecondaryTile tile = new SecondaryTile(tileId, displayName, "args", new Uri("ms-appx:///Assets/guapi.png"), TileSize.Default);
            tile.VisualElements.Square71x71Logo = new Uri("ms-appx:///Assets/guapi.png");
            tile.VisualElements.Wide310x150Logo = new Uri("ms-appx:///Assets/guapi.png");
            tile.VisualElements.Square310x310Logo = new Uri("ms-appx:///Assets/guapi.png");
            tile.VisualElements.Square44x44Logo = new Uri("ms-appx:///Assets/guapi.png"); // Branding logo
            tile.VisualElements.BackgroundColor = color;
            return tile;
        }

        public static SecondaryTile GenerateSecondaryTile(string titleId , string displayName) {
            return GenerateSecondaryTile(titleId, displayName, Windows.UI.Colors.Transparent);
        }

        public static async Task<SecondaryTile> FindExistingAsync(string tileId) {
            return (await SecondaryTile.FindAllAsync()).FirstOrDefault(i => i.TileId.Equals(tileId));
        }

        public static async Task<SecondaryTile> PinNewSecondaryTileAsync(string titleId, string displayName, string xml) {
            SecondaryTile tile = GenerateSecondaryTile(titleId, displayName);
            await tile.RequestCreateAsync();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            TileUpdateManager.CreateTileUpdaterForSecondaryTile(tile.TileId).Update(new TileNotification(doc));

            return tile;
        }

        public static async Task UpdateTilesAsync(string xml) {
            XmlDocument doc;

            doc = new XmlDocument();
            try {
                doc.LoadXml(xml);
            } catch (Exception) { /* Ignore */ }

            await UpdateTilesAsync(doc);
        }

        public static async Task UpdateTilesAsync(XmlDocument doc) {
            try {
                TileUpdateManager.CreateTileUpdaterForApplication().Update(new TileNotification(doc));
                await UpdateTileAsync("Small", doc);
                await UpdateTileAsync("Medium", doc);
                await UpdateTileAsync("Wide", doc);
                await UpdateTileAsync("Large", doc);
            } catch (Exception) { /* Ignore */ }
        }

        public static async Task UpdateTileAsync(string tileId, XmlDocument doc) {
            if (!SecondaryTile.Exists(tileId)) {
                SecondaryTile tile = GenerateSecondaryTile(tileId, tileId, Windows.UI.Colors.Transparent);
                tile.VisualElements.ShowNameOnSquare310x310Logo = true;
                await tile.RequestCreateAsync();
            }
            //await GenerateSecondaryTile(tileId, tileId).RequestCreateAsync();
            TileUpdateManager.CreateTileUpdaterForSecondaryTile(tileId).Update(new TileNotification(doc));
        }

        public static async void UpdateTitlesAsync(List<string> news) {
            if (news == null || !news.Any()) { return; }
            try {
                var updater = TileUpdateManager.CreateTileUpdaterForApplication();
                InitLiveTitleState(updater);
                foreach (var item in news) { updater.Update(new TileNotification(CreatXMLDocument(item))); }
                foreach (var item in await SecondaryTile.FindAllAsync()) {
                    var updaterForTrans = TileUpdateManager.CreateTileUpdaterForSecondaryTile(item.TileId);
                    InitLiveTitleState(updaterForTrans);
                    foreach (var newsItem in news) { updaterForTrans.Update(new TileNotification(CreatXMLDocument(newsItem))); }
                }
            } catch (Exception) { /*do not need to report excepton. */}
        }

        private static void InitLiveTitleState(TileUpdater updater) {
            updater.EnableNotificationQueueForWide310x150(true);
            updater.EnableNotificationQueueForSquare150x150(true);
            updater.EnableNotificationQueueForSquare310x310(true);
            updater.EnableNotificationQueue(true);
            updater.Clear();
        }

        private static XmlDocument CreatXMLDocument(string newsItem) {
            var doc = new XmlDocument();
            var TileTemplateXml = $@"
                                        <tile branding='nameAndLogo' displayName='{DateTime.Now.ToString("tt h:mm")}'> 
                                             <visual version='3'>
                                                 <binding template='TileMedium'>
                                                    <text hint-wrap='true' >{newsItem}</text>
                                                 </binding>
                                                 <binding template='TileWide'>
                                                    <text hint-wrap='true' >{newsItem}</text>
                                                 </binding>
                                                 <binding template='TileLarge'>
                                                    <text hint-wrap='true' >{newsItem}</text>
                                                 </binding>
                                             </visual>
                                        </tile>";
            doc.LoadXml(TileTemplateXml, new XmlLoadSettings {
                ProhibitDtd = false,
                ValidateOnParse = false,
                ElementContentWhiteSpace = false,
                ResolveExternals = false
            });
            return doc;
        }

        public static async Task<string> GetNewsAsync() {
            try {
                var date = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
                var Host = "https://m.douban.com/rexxar/api/v2/recommend_feed?alt=json&next_date={0}&loc_id=&gender=&birthday=&udid=&for_mobile=true";
                var listfor = await FetchMessageFromAPIAsync(string.Format(Host, date), 0);
                UpdateTitlesAsync(listfor.ToList());
            } catch (Exception) { /* ignore */ }
            return null;
        }

        public  static async Task<IList<string>> FetchMessageFromAPIAsync(string target, int offset = 0) {
            IList<string> list = new List<string>();
            try {
                var result = await DoubanWebProcess.GetMDoubanResponseAsync(target, client: new Windows.Web.Http.HttpClient());
                if (result == null) 
                    return list;
                JObject jo = JObject.Parse(result);
                var feeds = jo["recommend_feeds"];
                if (feeds == null || !feeds.HasValues) 
                    return list;
                if (feeds.HasValues) {
                    feeds.Children().Take(5).ToList().ForEach(singleton => {
                        try {
                            list.Add(singleton["title"].Value<string>());
                        } catch { /* Ignore, item error. */ }
                    });
                }
            } catch { /* Ignore */ }
            return list;
        }

    }
}
