using static Wallace.UWP.Helpers.Tools.UWPStates;
using static Douban.UWP.NET.Resources.AppResources;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Core;
using Windows.Media.Core;
using Windows.Storage.Streams;
using Douban.UWP.NET.Models;
using Douban.UWP.Core.Models;
using Douban.UWP.Core.Models.FMModels;

namespace Douban.UWP.NET.Tools {
    public class DoubanMusicService : ViewModelBase {

        public DoubanMusicService() {
            Player.Source = PlayList;
            PlayList.AutoRepeatEnabled = false;
            PlayList.ShuffleEnabled = false;
            PlayList.CurrentItemChanged += OnCurrentItemChangedAsync;
        }

        private void OnCurrentItemChangedAsync(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args) {
            var newItem = args.NewItem;
            if (newItem == null)
                return;
            CacheItems.Add(newItem);
            if (CacheItems.Count <= CacheMax)
                return;
            CacheItems[0].Source.Reset();
            CacheItems.RemoveAt(0);
        }

        public bool InsertMusicItem(MediaPlaybackItem musicItem, int index = -1) {
            bool succeed = true;
            if (index < 0)
                _playlist.Items.Add(musicItem);
            else if (_playlist.Items.Count > index)
                _playlist.Items.Insert(index, musicItem);
            else if (_playlist.Items.Count == index)
                _playlist.Items.Add(musicItem);
            else
                succeed = false;
            if (succeed) {
                currentInsert = index >= 0 ? index : (_playlist.Items.Count - 1);
                RaisePropertyChanged("SongList");
            }
            return succeed;
        }

        public void PlayMoveTo(int index = -1) {
            _playlist.MoveTo(index < 0 ? (uint)currentInsert : (uint)index);
        }

        public void PlayAnyway() {
            Player.Play();
        }

        #region

        int currentInsert;

        MediaPlayer _player;
        public MediaPlayer Player { get { return _player ?? (_player = new MediaPlayer()); } }

        MediaPlaybackList _playlist;
        public MediaPlaybackList PlayList { get { return _playlist ?? (_playlist = new MediaPlaybackList()); } }

        IList<MediaPlaybackItem> _cachelist;
        IList<MediaPlaybackItem> CacheItems { get { return _cachelist ?? (_cachelist = new List<MediaPlaybackItem>()); } }

        uint _cacheMax = 5;
        public uint CacheMax {
            get { return _cacheMax; }
            set { _cacheMax = value; }
        }

        CoreDispatcher dispatcher = Window.Current.Dispatcher;

        #endregion

        #region Binding properties

        IList<MediaPlaybackItem> _songList;
        public IList<MediaPlaybackItem> SongList { get { return _songList ?? (_songList = _playlist.Items); } }
        
        #endregion

    }

    public static class MusicServiceHelper{

        public static DateTime UTCPoint { get { return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc); } }

        public static MediaPlaybackItem CreatePlayItem(Uri uri, Uri img, MusicBoardParameter para, string title, string artist, string albumTitle, string albunmArtist) {
            var source = MediaSource.CreateFromUri(uri);
            source.CustomProperties["Title"] = title;
            source.CustomProperties["CheckPoint"] = SetCheckPoint(UTCPoint);
            source.CustomProperties["Message"] = para;
            var item = new MediaPlaybackItem(source);
            var properties = item.GetDisplayProperties();
            properties.Type = Windows.Media.MediaPlaybackType.Music;
            properties.Thumbnail = RandomAccessStreamReference.CreateFromUri(img);
            properties.MusicProperties.Title = title;
            properties.MusicProperties.Artist = artist;
            properties.MusicProperties.AlbumTitle = albumTitle;
            properties.MusicProperties.AlbumArtist = albunmArtist;
            item.ApplyDisplayProperties(properties);
            return item;
        }

        public static long SetCheckPoint(DateTime point) {
            return (long)((DateTime.Now - point).TotalMilliseconds);
        }

        public static MediaPlaybackItem CreatePlayItem(string url, string img, MusicBoardParameter para, string title, string artist, string albumTitle, string albunmArtist) {
            return CreatePlayItem(new Uri(url), new Uri(img), para, title, artist, albumTitle, albunmArtist);
        }

        public static MediaPlaybackItem CreatePlayItem(string url, Uri img, MusicBoardParameter para, string title, string artist, string albumTitle, string albunmArtist) {
            return CreatePlayItem(new Uri(url), img, para, title, artist, albumTitle, albunmArtist);
        }

        public static MediaPlaybackItem CreatePlayItem(Uri uri, Uri img, MusicBoardParameter para, string title, string artist) {
            return CreatePlayItem(uri, img, para, title, artist, "", "");
        }

        public static MediaPlaybackItem CreatePlayItem(string url, string img, MusicBoardParameter para, string title, string artist) {
            return CreatePlayItem(new Uri(url), new Uri(img), para, title, artist);
        }

        public static MediaPlaybackItem CreatePlayItem(Uri uri, Uri img, MusicBoardParameter para) {
            return CreatePlayItem(uri, img, para, GetUIString("UnknownMusicTitle"), GetUIString("UnknownMusicArtist"));
        }

        public static MediaPlaybackItem CreatePlayItem(string url, string img, MusicBoardParameter para) {
            return CreatePlayItem(new Uri(url), new Uri(img), para);
        }

    }

}
