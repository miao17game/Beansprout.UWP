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
using Douban.UWP.Core.Models;
using System.Collections.ObjectModel;

namespace Douban.UWP.NET.Tools {
    public class DoubanDownloadService {

        #region Properties

        const string DoubanMusicGroup = "DOUBAN_MUSIC_DOWNLOAD_GROUP";
        const string DoubanMusicCache = "BeansproutMusic";

        public void RemoveItemFromListByValue(DownloadOperationValue value) {
            var index = DownloadList.ToList().FindIndex(i => i.Value == value);
            if (index == -1)
                return;
            DownloadList.RemoveAt(index);
        }

        ObservableCollection<KeyValuePair<DownloadOperationKey, DownloadOperationValue>> _downloadList;
        public ObservableCollection<KeyValuePair< DownloadOperationKey, DownloadOperationValue>> DownloadList {
            get { return _downloadList ?? (_downloadList = new ObservableCollection<KeyValuePair<DownloadOperationKey, DownloadOperationValue>>()); }
        }

        #endregion

        public DoubanDownloadService() {
            StorageHelper.ClearDownloadTemporaryFilesAsync();
        }

        public async void DownloadMusicNotWaitAsync(MHzSongBase song, DownloadNotifType notif_type, DownloadReportType report_type) {
            await DownloadMusicAsync(song, notif_type, report_type);
        }

        public async Task<DownloadResult> DownloadMusicAsync(MHzSongBase song, bool not_show = false, bool show_start = true, bool show_succ_notif = true, bool show_fail_notif = true) {
            var file = default(StorageFile);
            try {
                var folder = await GetBeansproutMusicFolderAsync();
                var filename = MHzSongBaseHelper.GetIdentity(song) + StorageHelper.MusicTemporaryExtension;
                file = await CreateFileInMusicFolderWithNameAsync(folder, filename);

                var fail_toast = ToastHelper.CreateToastNotificaion(
                        title: GetUIString("Download_Failed") + " : " + song.Title,
                        content: DateTime.Now.ToString("h:mm tt"),
                        imageUri: " -- ",
                        uri: " -- ",
                        logoOverride: song.Picture,
                        voice: "ms-appx:///Voice/yellsedtsr.mp3");

                var succeed_toast = ToastHelper.CreateToastNotificaion(
                        title: GetUIString("Download_Succeed") + " : " + song.Title,
                        content: DateTime.Now.ToString("h:mm tt"),
                        imageUri: " -- ",
                        uri: " -- ",
                        logoOverride: song.Picture,
                        voice: "ms-appx:///Voice/haizi.mp3");

                var group = BackgroundTransferGroup.CreateGroup(DoubanMusicGroup);
                group.TransferBehavior = BackgroundTransferBehavior.Serialized;

                var downloader = new BackgroundDownloader {
                    FailureToastNotification = show_succ_notif ? new Windows.UI.Notifications.ToastNotification(fail_toast) : null,
                    SuccessToastNotification = show_fail_notif ? new Windows.UI.Notifications.ToastNotification(succeed_toast) : null,
                    TransferGroup = group,
                };

                var succeed_trans = Uri.TryCreate(song.Url, UriKind.Absolute, out var do_url);
                if (!succeed_trans)
                    throw new DownloadCanNotRunException();

                var operation = downloader.CreateDownload(do_url, file);
                operation.CostPolicy = BackgroundTransferCostPolicy.UnrestrictedOnly;

                if (show_start)
                    ReportHelper.ReportAttentionAsync(GetUIString("Download_Start"));

                if (!not_show) {
                    DownloadListAddNewItem(song, operation);
                    operation = await operation.StartAsync();
                } else {
                    var control = await operation.StartAsync();
                }

                var mess_succeed = await CreateBJSONMessageAsync(song, folder, file.Path);
                if (!mess_succeed)
                    throw new JSONCanNotCreateException();

                await file.RenameAsync(filename.Replace(StorageHelper.MusicTemporaryExtension, StorageHelper.MusicExtension), NameCollisionOption.ReplaceExisting);
                return DownloadResult.Successfully;

            } catch (JSONCanNotCreateException) {
                await file.DeleteAsync();
                return DownloadResult.Failed;
            } catch (DownloadCanNotRunException) {
                await file.DeleteAsync();
                return DownloadResult.Failed;
            } catch (FileCannNotCreateException) {
                return DownloadResult.Failed;
            } catch (FileExistException) {
                return DownloadResult.FileExist;
            } catch (Exception e) {
                Debug.WriteLine(e.Message);
                return DownloadResult.Failed;
            }
        }

        #region Format API

        public async Task<DownloadResult> DownloadMusicAsync(MHzSongBase song, DownloadNotifType notif_type, DownloadReportType report_type) {
            return
                notif_type == DownloadNotifType.Null && report_type == DownloadReportType.Null ?
                await DownloadMusicAsync(song, not_show: true, show_start: false, show_succ_notif: false, show_fail_notif: false) :

                notif_type == DownloadNotifType.Null && report_type == DownloadReportType.NotShowButReportStart ?
                await DownloadMusicAsync(song, not_show: true, show_start: true, show_succ_notif: false, show_fail_notif: false) :

                notif_type == DownloadNotifType.Null && report_type == DownloadReportType.ShowListAndReportStart ?
                await DownloadMusicAsync(song, not_show: false, show_start: true, show_succ_notif: false, show_fail_notif: false) :

                notif_type == DownloadNotifType.Null && report_type == DownloadReportType.ShowListButNotReport ?
                await DownloadMusicAsync(song, not_show: false, show_start: false, show_succ_notif: false, show_fail_notif: false) :

                notif_type == DownloadNotifType.SuccessfullyNotification && report_type == DownloadReportType.Null ?
                await DownloadMusicAsync(song, not_show: true, show_start: false, show_succ_notif: true, show_fail_notif: false) :

                notif_type == DownloadNotifType.SuccessfullyNotification && report_type == DownloadReportType.NotShowButReportStart ?
                await DownloadMusicAsync(song, not_show: true, show_start: true, show_succ_notif: true, show_fail_notif: false) :

                notif_type == DownloadNotifType.SuccessfullyNotification && report_type == DownloadReportType.ShowListAndReportStart ?
                await DownloadMusicAsync(song, not_show: false, show_start: true, show_succ_notif: true, show_fail_notif: false) :

                notif_type == DownloadNotifType.SuccessfullyNotification && report_type == DownloadReportType.ShowListButNotReport ?
                await DownloadMusicAsync(song, not_show: false, show_start: false, show_succ_notif: true, show_fail_notif: false) :

                notif_type == DownloadNotifType.FailedNotification && report_type == DownloadReportType.Null ?
                await DownloadMusicAsync(song, not_show: true, show_start: false, show_succ_notif: false, show_fail_notif: true) :

                notif_type == DownloadNotifType.FailedNotification && report_type == DownloadReportType.NotShowButReportStart ?
                await DownloadMusicAsync(song, not_show: true, show_start: true, show_succ_notif: false, show_fail_notif: true) :

                notif_type == DownloadNotifType.FailedNotification && report_type == DownloadReportType.ShowListAndReportStart ?
                await DownloadMusicAsync(song, not_show: false, show_start: true, show_succ_notif: false, show_fail_notif: true) :

                notif_type == DownloadNotifType.FailedNotification && report_type == DownloadReportType.ShowListButNotReport ?
                await DownloadMusicAsync(song, not_show: false, show_start: false, show_succ_notif: false, show_fail_notif: true) :

                notif_type == DownloadNotifType.AllNotification && report_type == DownloadReportType.Null ?
                await DownloadMusicAsync(song, not_show: true, show_start: false, show_succ_notif: true, show_fail_notif: true) :

                notif_type == DownloadNotifType.AllNotification && report_type == DownloadReportType.NotShowButReportStart ?
                await DownloadMusicAsync(song, not_show: true, show_start: true, show_succ_notif: true, show_fail_notif: true) :

                notif_type == DownloadNotifType.AllNotification && report_type == DownloadReportType.ShowListAndReportStart ?
                await DownloadMusicAsync(song, not_show: false, show_start: true, show_succ_notif: true, show_fail_notif: true) :

                await DownloadMusicAsync(song, not_show: false, show_start: false, show_succ_notif: true, show_fail_notif: true);
        }

        #endregion

        #region Save JSON

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
                if (song is MHzSong)
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

        #endregion

        #region Save LRC

        public async Task<bool> CreateBLRCAsync(string sha256, IList<LrcInfo> lrc) {
            var folder = await GetBeansproutMusicFolderAsync();
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
            } catch (InvalidDataContractException e) {
                Debug.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        #endregion

        private static async Task<StorageFile> CreateFileInMusicFolderWithNameAsync(StorageFolder folder, string filename, CreationCollisionOption option = CreationCollisionOption.FailIfExists) {
            var file = default(StorageFile);
            try {
                file = await folder.CreateFileAsync(filename, option);
            } catch (FileNotFoundException) {
                throw new FileCannNotCreateException();
            } catch {
                throw new FileExistException();
            }
            return file;
        }

        public static async Task<StorageFolder> GetBeansproutMusicFolderAsync(CreationCollisionOption option = CreationCollisionOption.OpenIfExists) {
            return await KnownFolders.MusicLibrary.CreateFolderAsync(DoubanMusicCache, option);
        }

        private void DownloadListAddNewItem(MHzSongBase song, DownloadOperation operation) {
            DownloadList.Add(new KeyValuePair<DownloadOperationKey, DownloadOperationValue>(new DownloadOperationKey(song.SHA256, song.Title), new DownloadOperationValue(operation)));
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

    public class DownloadOperationValue : ViewModelBase {

        public DownloadOperationValue(DownloadOperation sha256) {
            Operation = sha256;
        }

        public DownloadOperation Operation { get; set; }

        private string _now_value = ((double)0).ToString();
        public string NowValue {
            get { return _now_value; }
            set { _now_value = value; RaisePropertyChanged("NowValue"); }
        }

        private string _whole_value = ((double)1).ToString();
        public string WholeValue {
            get { return _whole_value; }
            set { _whole_value = value; RaisePropertyChanged("WholeValue"); }
        }

        private bool _is_completed;
        public bool IsCompleted {
            get { return _is_completed; }
            set { _is_completed = value; RaisePropertyChanged("IsCompleted"); }
        }

        private bool _is_unstart = true;
        public bool IsUnstart {
            get { return _is_unstart; }
            set { _is_unstart = value; RaisePropertyChanged("IsUnstart"); }
        }

        private bool _is_paused;
        public bool IsPaused {
            get { return _is_paused; }
            set { _is_paused = value; RaisePropertyChanged("IsPaused"); }
        }

        public void RefrashProgress() {
            IsCompleted = Operation.Progress.Status == BackgroundTransferStatus.Completed ? true : false;
            IsPaused = Operation.Progress.Status == BackgroundTransferStatus.Running ? false : true;
            IsUnstart = Operation.Progress.TotalBytesToReceive == 0;
            if (!IsUnstart) {
                WholeValue = (((double)Operation.Progress.TotalBytesToReceive) / (1024 * 1024)).ToString("#.##");
                NowValue = (((double)Operation.Progress.BytesReceived) / (1024 * 1024)).ToString("#.##");
            }
        }

    }

    public enum DownloadNotifType {
        Null,
        SuccessfullyNotification,
        FailedNotification,
        AllNotification,
    }

    public enum DownloadReportType {
        Null,
        NotShowButReportStart,
        ShowListButNotReport,
        ShowListAndReportStart,
    }

    public enum DownloadResult {
        ActionInvalid,
        Successfully,
        Failed,
        FileExist,
    }

    #region Exceptions

    class FileExistException : Exception {
        public override string Message { get { return "File is exist."; } }
    }

    class FileCannNotCreateException : Exception {
        public override string Message { get { return "file create failed."; } }
    }

    class DownloadCanNotRunException : Exception {
        public override string Message { get { return "Download failed."; } }
    }

    class JSONCanNotCreateException : Exception {
        public override string Message { get { return "json create failed."; } }
    }

    #endregion

    public static class DownloadHelper {

        public static void ReportByDownloadResoult(DownloadResult result, bool show_succ_mess = false) {
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
                    if (show_succ_mess)
                        ReportHelper.ReportAttentionAsync(GetUIString("Download_Succeed"));
                    break;
            }
        }

    }

}
