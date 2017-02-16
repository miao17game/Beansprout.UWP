using Douban.UWP.Core.Models.FMModels;
using Douban.UWP.Core.Models.FMModels.MHzSongListModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Wallace.UWP.Helpers;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.Storage.Streams;

namespace Douban.UWP.NET.Tools {
    public static class StorageHelper {

        public const string DoubanMusicCache = "BeansproutMusic";
        public const string MusicTemporaryExtension = ".bmuwpdownload";
        public const string MusicExtension = ".bmuwp";
        public const string LrcExtension = ".blrcuwp";
        public const string JsonExtension = ".bjsonuwp";

        public async static Task<StorageFile> FetchStorageFileByPathAsync(string path) {
            try {
                return await StorageFile.GetFileFromPathAsync(path);
            } catch(FileNotFoundException) {
                return null;
            } catch{
                return null;
            }
        }

        public async static void ClearDownloadTemporaryFilesAsync() {
            try {
                var files = await GetAllStorageFilesByExtensionAsync(MusicTemporaryExtension);
                foreach(var do_file in files) {
                    await do_file.DeleteAsync();
                }
            } catch {
                return;
            }
        }

        public async static Task<StorageFile> FetchLocalJsonBySHA256Async(string sha256) {
            try {
                var files = await GetAllStorageFilesByExtensionAsync(JsonExtension);
                return files.ToList().Find(i => i.Name == sha256 + JsonExtension);
            } catch {
                return null;
            }
        }

        public async static Task<bool> IsExistLocalJsonBySHA256Async(string sha256) {
            try {
                var files = await GetAllStorageFilesByExtensionAsync(JsonExtension);
                return files.ToList().Find(i => i.Name == sha256 + JsonExtension) != null ? true : false;
            } catch {
                return false;
            }
        }

        public static bool IsExistLocalJsonBySHA256(string sha256, IReadOnlyList<StorageFile> query) {
            try {
                return query.ToList().Find(i => i.Name == sha256 + JsonExtension) != null ? true : false;
            } catch {
                return false;
            }
        }

        public async static Task<StorageFile> FetchLocalMusicBySHA256Async(string sha256) {
            try {
                var files = await GetAllStorageFilesByExtensionAsync(MusicExtension);
                return files.ToList().Find(i => i.Name == sha256+ MusicExtension);
            } catch {
                return null;
            }
        }

        public async static Task<IList<LrcInfo>> FetchLocalLrcBySHA256Async(string sha256) {
            try {
                var lrcs = await GetAllStorageFilesByExtensionAsync(LrcExtension);
                var lrc = lrcs.ToList().Find(i => i.Name == sha256 + LrcExtension);
                if (lrc == null)
                    return null;
                return await ReadLrcFromStorageFileAsync(lrc);
            } catch {
                return null;
            }
        }

        public static async Task<IList<LrcInfo>> ReadLrcFromStorageFileAsync(StorageFile lrc) {
            using (var stream = await lrc.OpenReadAsync()) {
                IBuffer buff = new Windows.Storage.Streams.Buffer((uint)stream.Size);
                var bytes = new byte[stream.Size];
                await stream.AsStream().ReadAsync(bytes, 0, (int)stream.Size);
                var result = JsonHelper.FromJson<IList<LrcInfo>>(Encoding.UTF8.GetString(bytes));
                return result;
            }
        }

        public static async Task<MHzSongBase> ReadSongModelFromStorageFileAsync(StorageFile storage) {
            using (var stream = await storage.OpenReadAsync()) {
                IBuffer buff = new Windows.Storage.Streams.Buffer((uint)stream.Size);
                var bytes = new byte[stream.Size];
                await stream.AsStream().ReadAsync(bytes, 0, (int)stream.Size);
                var str = Encoding.UTF8.GetString(bytes);
                var result = default(MHzSongBase);
                try {
                    result = JsonHelper.FromJson<MHzSongBase>(RepairForOldVersion(str));
                    result.LocalPath = storage.Path;
                } catch (SerializationException) {
                    var music_file = await FetchStorageFileByPathAsync(storage.Path.Replace(JsonExtension, MusicExtension));
                    await storage.DeleteAsync();
                    await music_file.DeleteAsync();
                    return null;
                } catch {
                    return null;
                }
                return result;
            }
        }

        private static string RepairForOldVersion(string str) {
            str = str
                .Replace("Title", "title")
                .Replace("Albumtitle", "albumtitle")
                .Replace("Picture", "picture")
                .Replace("Artist", "artist")
                .Replace("AID", "aid")
                .Replace("SSID", "ssid")
                .Replace("SID", "sid")
                .Replace("Url", "url")
                .Replace("Singers", "singers")
                .Replace("Name", "name")
                .Replace("SHA256", "sha256");
            //System.Diagnostics.Debug.WriteLine(str);
            return str;
        }

        public static async Task<IReadOnlyList<StorageFile>> GetAllStorageFilesByExtensionAsync(string extension) {
            var folder = await KnownFolders.MusicLibrary.CreateFolderAsync(DoubanMusicCache, CreationCollisionOption.OpenIfExists);
            QueryOptions queryOption = new QueryOptions(CommonFileQuery.OrderByTitle, new string[] { extension }) { FolderDepth = FolderDepth.Shallow };
            var files = await folder.CreateFileQueryWithOptions(queryOption).GetFilesAsync();
            return files;
        }

    }
}
