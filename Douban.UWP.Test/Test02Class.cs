using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;

namespace Douban.UWP.Test {
    public class Test02Class {

        const string DoubanMusicGroup = "DOUBAN_MUSIC_DOWNLOAD_GROUP";

        public async Task DownloadMusicAsync() {
            var file = await KnownFolders.MusicLibrary.CreateFileAsync("aaa.mp3", CreationCollisionOption.ReplaceExisting);

            var downloader = new BackgroundDownloader {
                TransferGroup = BackgroundTransferGroup.CreateGroup(DoubanMusicGroup),
            };
            var succeed_trans = Uri.TryCreate("http://m2.music.126.net/KP1OE1TKzCOFfoUSt0o_ig==/1192970116145619.mp3", UriKind.Absolute, out var do_url);
            if (!succeed_trans)
                return;

            var operation = downloader.CreateDownload(do_url, file);
            operation.CostPolicy = BackgroundTransferCostPolicy.UnrestrictedOnly;

            var control = await operation.StartAsync();

        }

    }
}
