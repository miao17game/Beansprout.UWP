using static Wallace.UWP.Helpers.Tools.UWPStates;
using static Douban.UWP.NET.Resources.AppResources;

using Douban.UWP.Core.Models.FMModels;
using Douban.UWP.Core.Models.FMModels.MHzSongListModels;
using Douban.UWP.Core.Tools;
using Douban.UWP.NET.Resources;
using Douban.UWP.NET.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Wallace.UWP.Helpers;
using Wallace.UWP.Helpers.Tools;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Douban.UWP.NET.Tools {
    public class DoubanDownloadService {

        #region Properties

        const string DoubanMusicGroup = "DOUBAN_MUSIC_DOWNLOAD_GROUP";
        const string DoubanMusicCache = "BeansproutMusic";

        IDictionary<DownloadOperationKey, DownloadOperation> _downloadList;
        public IDictionary<DownloadOperationKey, DownloadOperation> DownloadList {
            get { return _downloadList ?? (_downloadList = new Dictionary<DownloadOperationKey, DownloadOperation>()); }
        }

        #endregion

        public async Task<DownloadResult> DownloadMusicAsync(MHzSongBase song, bool is_unshow = true) {
            try {
                var folder = await KnownFolders.MusicLibrary.CreateFolderAsync(DoubanMusicCache, CreationCollisionOption.OpenIfExists);

                var filename = $"{MHzSongBaseHelper.GetIdentity(song)}.bmuwp";

                var file = default(StorageFile);
                try {
                    file = await folder.CreateFileAsync(filename, CreationCollisionOption.FailIfExists);
                } catch (FileNotFoundException) {
                    throw new Exception("Name invalid.");
                } catch {
                    throw new FileExistException();
                }

                var fail_toast = ToastHelper.CreateToastNotificaion(
                        title: GetUIString("Download_Failed") + " : " + song.Title,
                        content: DateTime.Now.ToString("h:mm tt"),
                        imageUri: " -- ",
                        uri: " -- ",
                        logoOverride: song.Picture);

                var succeed_toast = ToastHelper.CreateToastNotificaion(
                        title: GetUIString("Download_Succeed") + " : " + song.Title,
                        content: DateTime.Now.ToString("h:mm tt"),
                        imageUri: " -- ",
                        uri: " -- ",
                        logoOverride: song.Picture);

                var downloader = new BackgroundDownloader {
                    FailureToastNotification = new Windows.UI.Notifications.ToastNotification(fail_toast),
                    SuccessToastNotification = new Windows.UI.Notifications.ToastNotification(succeed_toast),
                    TransferGroup = BackgroundTransferGroup.CreateGroup(DoubanMusicGroup),
                };
                var succeed_trans = Uri.TryCreate(song.Url, UriKind.Absolute, out var do_url);
                if (!succeed_trans)
                    return DownloadResult.ActionInvalid;
                
                var operation = downloader.CreateDownload(do_url, file);
                operation.CostPolicy = BackgroundTransferCostPolicy.UnrestrictedOnly;

                ReportHelper.ReportAttentionAsync(GetUIString("Download_Start"));
                
                if (!is_unshow) {
                    DownloadList.Add(new DownloadOperationKey(song.SHA256, song.Title), operation);
                    operation = await operation.StartAsync();
                } else{
                    var control = await operation.StartAsync();
                }

                var mess_succeed = await CreateBJSONMessageAsync(song, folder, file.Path);
                if (!mess_succeed)
                    throw new Exception("Create mess failed.");

                return DownloadResult.Successfully;

            } catch (FileExistException ) {
                return DownloadResult.FileExist;
            } catch (Exception e) {
                Debug.WriteLine(e.Message);
                return DownloadResult.Failed;
            }
        }

        private static async Task<bool> CreateBJSONMessageAsync(MHzSongBase song, StorageFolder folder, string filePath) {
            var mess_name = $"{MHzSongBaseHelper.GetIdentity(song)}.bjsonuwp";
            var mess = default(StorageFile);
            try {
                mess = await folder.CreateFileAsync(mess_name, CreationCollisionOption.ReplaceExisting);
            } catch {
                return false;
            }

            song.IsCached = true;
            song.LocalPath = filePath;

            try {
                var bytes = default(byte[]);
                if(song is MHzSong) 
                    bytes = Encoding.UTF8.GetBytes(JsonHelper.ToJson(song as MHzSong));
                else 
                    bytes = Encoding.UTF8.GetBytes(JsonHelper.ToJson(song));
                using (var irandom = await mess.OpenAsync(FileAccessMode.ReadWrite)) {
                    var stream = irandom.AsStreamForWrite();
                    await stream.WriteAsync(bytes, 0, bytes.Length);
                    await stream.FlushAsync();
                }
            } catch (SerializationException) {
                return false;
            }

            return true;
        }

        public async Task<bool> CreateBLRCAsync(string sha256, IList<LrcInfo> lrc) {
            var folder = await KnownFolders.MusicLibrary.CreateFolderAsync(DoubanMusicCache, CreationCollisionOption.OpenIfExists);
            var lrc_name = $"{sha256}.blrcuwp";
            var mess = default(StorageFile);
            try {
                mess = await folder.CreateFileAsync(lrc_name, CreationCollisionOption.ReplaceExisting);
            } catch {
                return false;
            }

            try {
                var bytes = Encoding.UTF8.GetBytes(JsonHelper.ToJson(lrc));
                using (var irandom = await mess.OpenAsync(FileAccessMode.ReadWrite)) {
                    var stream = irandom.AsStreamForWrite();
                    await stream.WriteAsync(bytes, 0, bytes.Length);
                    await stream.FlushAsync();
                }
            } catch (SerializationException e) {
                Debug.WriteLine(e.Message);
                return false;
            } catch(InvalidDataContractException e) {
                Debug.WriteLine(e.Message);
                return false;
            }

            return true;
        }

    }

    public class DownloadOperationKey {

        public DownloadOperationKey(string sha256, string title) {
            SHA256 = sha256;
            Title = title;
        }

        public string SHA256 { get; set; }
        public string Title { get; set; }
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

    public static class DownloadHelper {

        public static void ReportByDownloadResoult(DownloadResult result) {
            switch (result) {
                case DownloadResult.ActionInvalid:
                    ReportHelper.ReportAttentionAsync(GetUIString("Download_Error"));
                    break;
                case DownloadResult.FileExist:
                    ReportHelper.ReportAttentionAsync(GetUIString("Download_Exist"));
                    break;
                case DownloadResult.Failed:
                    ReportHelper.ReportAttentionAsync(GetUIString("Download_Failed"));
                    break;
                case DownloadResult.Successfully:
                    ReportHelper.ReportAttentionAsync(GetUIString("Download_Succeed"));
                    break;
            }
        }

    }

}
