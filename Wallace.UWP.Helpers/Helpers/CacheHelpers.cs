using System;
using System . Collections . Generic;
using System . Diagnostics;
using System . IO;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using Windows . Storage;

namespace Wallace.UWP.Helpers. Helpers {
    /// <summary>
    /// 在整个解决方案中使用的Cache value字符串常量的集合 ///
    /// 此文件向所有项目共享
    /// </summary>
    public static class CacheHelpers {
        
        /// <summary>
        /// 读取缓存文件 ///
        /// </summary>
        public static async Task<string> ReadResetCacheValue ( string key ) {
            var localFolder = ApplicationData . Current . LocalCacheFolder;
            StorageFile file = default ( StorageFile );
            try {
                switch ( key ) {
                    case CacheConstants.CacheId:
                        file = await localFolder . GetFileAsync ("CacheId.txt");
                        break;
                    case CacheConstants.HomeList:
                        file=await localFolder.GetFileAsync("HomeList.txt");
                        break;
                    case CacheConstants.BackgroundHomeListStorage:
                        file = await localFolder.GetFileAsync("BackgroundHomeListStorage.txt");
                        break;
                    default:
                        return null;
                }
                string value = await FileIO . ReadTextAsync ( file );
                return value;
            } catch ( FileNotFoundException ) { Debug . WriteLine ( "Error -----> 【 读取缓存文件失败 】" ); return null; }
        }

        /// <summary>
        /// 设置缓存文件 ///
        /// </summary>
        public static async Task SaveCacheValue ( string key , object value ) {
            var localFolder = ApplicationData . Current . LocalCacheFolder;
            StorageFile file = default ( StorageFile );
            switch ( key ) {
                case CacheConstants.CacheId:
                    file = await localFolder . CreateFileAsync ("CacheId.txt", CreationCollisionOption . ReplaceExisting );
                    break;
                case CacheConstants.HomeList:
                    file=await localFolder.CreateFileAsync("HomeList.txt", CreationCollisionOption.ReplaceExisting);
                    break;
                case CacheConstants.BackgroundHomeListStorage:
                    file = await localFolder.CreateFileAsync("BackgroundHomeListStorage.txt", CreationCollisionOption.ReplaceExisting);
                    break;
                default:
                    return;
            }
            await FileIO . WriteTextAsync ( file , value . ToString ( ) );
        }

        /// <summary>
        /// 读取特殊缓存文件 ///
        /// </summary>
        public static async Task<string> ReadSpecificCacheValue ( string key ) {
            var localFolder = ApplicationData . Current . LocalCacheFolder;
            StorageFile file = default ( StorageFile );
            try {
                file = await localFolder . GetFileAsync ( key + "_cache.txt" );
                string value = await FileIO . ReadTextAsync ( file );
                return value;
            } catch ( FileNotFoundException ) { Debug . WriteLine ( "Error -----> 【 数据读出缓存失败 】" ); return null; }
        }

        /// <summary>
        /// 设置特殊缓存文件 ///
        /// </summary>
        public static async Task SaveSpecificCacheValue( string key , object value ) {
            var localFolder = ApplicationData . Current . LocalCacheFolder;
            StorageFile file = default ( StorageFile );
            file = await localFolder . CreateFileAsync ( key + "_cache.txt" , CreationCollisionOption . ReplaceExisting );
            await FileIO . WriteTextAsync ( file , value . ToString ( ) );
        }
    }
}
