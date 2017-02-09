using Douban.UWP.Core.Models.FMModels;
using Douban.UWP.Core.Tools;
using Douban.UWP.NET.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallace.UWP.Helpers.Tools;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;

namespace Douban.UWP.NET.Tools {
    public class DoubanDownloadService {

        const string DoubanMusicGroup = "DOUBAN_MUSIC_DOWNLOAD_GROUP";

        public async Task<DownloadResult> DownloadMusicAsync(MHzSongBase song) {
            try {
                var folder = await KnownFolders.MusicLibrary.CreateFolderAsync(song.Artist??"Unknown Artist", CreationCollisionOption.OpenIfExists);
                folder = await folder.CreateFolderAsync(song.AlbumTitle??"Unknown Album", CreationCollisionOption.OpenIfExists);

                var filename = song.Title + " - " + song.Artist ?? "Unknown Artist" + ".mp3";

                var file = default(StorageFile);
                try {
                    file = await folder.CreateFileAsync(filename, CreationCollisionOption.FailIfExists);
                } catch {
                    throw new FileExistException();
                }

                var music_ptoprt = await file.Properties.GetMusicPropertiesAsync();
                music_ptoprt.Album = song.Album;
                music_ptoprt.AlbumArtist = song.Artist;
                music_ptoprt.Artist = song.Artist;
                music_ptoprt.Title = song.Title;
                //await music_ptoprt.SavePropertiesAsync();
                await file.Properties.SavePropertiesAsync();

                var fail_toast = ToastHelper.PopToast(
                        title:  UWPStates.GetUIString("Download_Failed") + " : " + filename,
                        content: DateTime.Now.ToString("h:mm tt"),
                        imageUri: " -- ",
                        uri: " -- ",
                        logoOverride: song.Picture);

                var succeed_toast = ToastHelper.PopToast(
                        title: UWPStates.GetUIString("Download_Succeed") + " : " + filename,
                        content: DateTime.Now.ToString("h:mm tt"),
                        imageUri: " -- ",
                        uri: " -- ",
                        logoOverride: song.Picture);

                var downloader = new BackgroundDownloader {
                    FailureToastNotification = fail_toast,
                    SuccessToastNotification = succeed_toast,
                    TransferGroup = BackgroundTransferGroup.CreateGroup(DoubanMusicGroup),
                };
                var succeed_trans = Uri.TryCreate(song.Url, UriKind.Absolute, out var do_url);
                if (!succeed_trans)
                    return DownloadResult.ActionInvalid;

                var operation = downloader.CreateDownload(do_url, file);
                operation.CostPolicy = BackgroundTransferCostPolicy.UnrestrictedOnly;

                var control = await operation.StartAsync();

                return DownloadResult.Successfully;

            } catch (FileExistException ) {
                return DownloadResult.FileExist;
            } catch {
                return DownloadResult.Failed;
            }
        }
    }

    class FileExistException : Exception {
        public override string Message { get { return "File is exist."; } }
    }

    public enum DownloadResult {
        ActionInvalid,
        Successfully,
        Failed,
        FileExist,
    }

}
