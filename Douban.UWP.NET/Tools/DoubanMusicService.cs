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

namespace Douban.UWP.NET.Tools {
    public class DoubanMusicService : ViewModelBase {

        public DoubanMusicService() {
            Player.Source = PlayList;
            PlayList.AutoRepeatEnabled = true;
            PlayList.ShuffleEnabled = false;
            PlayList.CurrentItemChanged += OnCurrentItemChangedAsync;
        }

        private void OnCurrentItemChangedAsync(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args) {
            var newItem = args.NewItem;
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
            if (succeed)
                RaisePropertyChanged("SongList");
            return succeed;
        }

        #region

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

        public static MediaPlaybackItem CreatePlayItem(Uri uri, Uri img, string title, string artist, string albumTitle, string albunmArtist) {
            var source = MediaSource.CreateFromUri(uri);
            source.CustomProperties["Title"] = title;
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

        public static MediaPlaybackItem CreatePlayItem(string url, string img, string title, string artist, string albumTitle, string albunmArtist) {
            return CreatePlayItem(new Uri(url), new Uri(img), title, artist, albumTitle, albunmArtist);
        }

        public static MediaPlaybackItem CreatePlayItem(Uri uri, Uri img, string title, string artist) {
            return CreatePlayItem(uri, img, title, artist, "", "");
        }

        public static MediaPlaybackItem CreatePlayItem(string url, string img, string title, string artist) {
            return CreatePlayItem(new Uri(url), new Uri(img), title, artist);
        }

        public static MediaPlaybackItem CreatePlayItem(Uri uri, Uri img) {
            return CreatePlayItem(uri, img, GetUIString("UnknownMusicTitle"), GetUIString("UnknownMusicArtist"));
        }

        public static MediaPlaybackItem CreatePlayItem(string url, string img) {
            return CreatePlayItem(new Uri(url), new Uri(img));
        }

    }

}
