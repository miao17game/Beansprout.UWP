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
using Douban.UWP.Core.Models.FMModels.MHzSongListModels;
using Wallace.UWP.Helpers;
using Douban.UWP.Core.Tools;
using System.Collections.ObjectModel;

namespace Douban.UWP.NET.Tools {
    public class DoubanMusicService : ViewModelBase {

        public DoubanMusicService() {
            Player.Source = PlayList;
            PlayList.CurrentItemChanged += OnCurrentItemChangedAsync;
            Player.MediaEnded += OnMediaEnded;
            var type = SettingsHelper.ReadSettingsValue(SettingsSelect.BackPlayType);
            PlayType = type is string ?
                EnumHelper.Parse<MusicServicePlayType>(type as string) :
                MusicServicePlayType.StreamPlay;
            this.InitPlayStyle(PlayType);
        }

        #region SongList Methods

        private void OnCurrentItemChangedAsync(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args) {
            var newItem = args.NewItem;
            if (newItem == null)
                return;
            if (newItem.Source.CustomProperties["Message"] is MusicBoardParameter para)
                MusicIsCurrent = para;
            CacheItems.Add(newItem);
            if (CacheItems.Count <= CacheMax)
                return;
            if (CacheItems.ToList().Exists(i => i.Source.CustomProperties["SHA256"] as string == newItem.Source.CustomProperties["SHA256"] as string))
                return;
            CacheItems[0].Source.Reset();
            CacheItems.RemoveAt(0);
        }

        private void OnMediaEnded(MediaPlayer sender, object args) {

        }

        public bool InsertMusicItem(MHzSong song, int index = -1) {
            var find_index = SongList.ToList().FindIndex(i => i.SHA256 == song.SHA256);
            if (find_index != -1) {
                currentInsert = find_index;
                return true;
            }
            var item = MusicServiceHelper.CreatePlayItem(song);
            var retuenIndex =  InsertMusicItem(item, index);
            SongList.Insert(retuenIndex, song);
            RaisePropertyChanged("SongList");
            return retuenIndex == -1 ? false : true;
        }

        public int InsertMusicItem(MediaPlaybackItem musicItem, int index = -1) {
            bool succeed = true;
            if (index < 0)
                PlayList.Items.Add(musicItem);
            else if (PlayList.Items.Count > index)
                PlayList.Items.Insert(index, musicItem);
            else if (PlayList.Items.Count == index)
                PlayList.Items.Add(musicItem);
            else
                succeed = false;
            return succeed ? currentInsert = (index >= 0 ? index : (PlayList.Items.Count - 1)) : -1;
        }

        public bool IsExistPlayItem(MediaPlaybackItem musicItem) {
            var find_index = PlayList.Items.ToList().FindIndex(i => i.Source.CustomProperties["SHA256"] as string == musicItem.Source.CustomProperties["SHA256"] as string);
            return find_index == -1 ? false : true;
        }

        public bool IsExistPlayItem(MHzSong song) {
            var find_index = SongList.ToList().FindIndex(i => i.SHA256 == song.SHA256);
            return find_index == -1 ? false : true;
        }

        public MediaPlaybackItem SelectItemBySHA256(string sha256) {
            return PlayList.Items.ToList().Find(i => i.Source.CustomProperties["SHA256"] as string == sha256);
        }

        public MHzSong SelectSongItemBySHA256(string sha256) {
            return SongList.ToList().Find(i => i.SHA256 == sha256);
        }

        public bool IsCurrentItem(MHzSong song) {
            var item = SelectItemBySHA256(song.SHA256);
            if (item == null)
                return false;
            return IsCurrentItem(item);
        }

        public bool IsCurrentItem(MediaPlaybackItem musicItem) {
            return PlayList.CurrentItem == musicItem;
        }

        public bool RemovePlaybackItem(MHzSong song) {
            var item = SelectItemBySHA256(song.SHA256);
            if (item == null)
                return false;
            if (IsCurrentItem(item))
                return false;
            var succeed = PlayList.Items.Remove(item);
            if (!succeed)
                return succeed;
            succeed = SongList.Remove(song);
            RaisePropertyChanged("SongList");
            return succeed;
        }

        public bool RemovePlaybackItem(MediaPlaybackItem item) {
            if (IsCurrentItem(item))
                return false;
            var succeed = PlayList.Items.Remove(item);
            if (!succeed)
                return succeed;
            var song = SelectSongItemBySHA256(item.Source.CustomProperties["SHA256"] as string);
            if (song == null)
                return false;
            succeed = SongList.Remove(song);
            RaisePropertyChanged("SongList");
            return succeed;
        }

        #endregion

        public void PlayMoveTo(int index = -1) {
            if (index < 0) {
                if (PlayList.CurrentItem == PlayList.Items[currentInsert])
                    return;
                else
                    PlayList.MoveTo((uint)currentInsert);
            } else
                PlayList.MoveTo((uint)index);
            PlayAnyway();
        }

        public void PlayMoveTo(MediaPlaybackItem item) {
            if (PlayList.CurrentItem == item)
                return;
            var index = PlayList.Items.ToList().FindIndex(i=>i==item);
            PlayList.MoveTo(index < 0 ? (uint)currentInsert : (uint)index);
            PlayAnyway();
        }

        public void MoveNext() {
            PlayList.MoveNext();
            PlayAnyway();
        }

        public void MovePrevious() {
            PlayList.MovePrevious();
            PlayAnyway();
        }

        public void PlayAnyway() {
            Player.Play();
        }

        public bool ResetPlayer() {
            try {
                _player.Pause();
                _player.Source = null;
                _player.MediaEnded -= OnMediaEnded;
                _player.Dispose();
                _player = new MediaPlayer();
                return true;
            } catch { return false; }
        }

        #region

        int currentInsert;

        MediaPlayer _player;
        public MediaPlayer Player { get { return _player ?? (_player = new MediaPlayer()); } }

        public MediaPlaybackSession Session { get { return Player.PlaybackSession; } }

        #region PlayBackList

        MediaPlaybackList _playlist;
        public MediaPlaybackList PlayList { get { return _playlist ?? (_playlist = new MediaPlaybackList()); } }

        MediaPlaybackList _mhzlist;
        public MediaPlaybackList MHzList { get { return _mhzlist ?? (_mhzlist = new MediaPlaybackList()); } }

        #endregion

        IList<MediaPlaybackItem> _cachelist;
        IList<MediaPlaybackItem> CacheItems { get { return _cachelist ?? (_cachelist = new List<MediaPlaybackItem>()); } }

        public bool SingletonPlay { get { return Player.IsLoopingEnabled; } set { Player.IsLoopingEnabled = value; } }

        public MusicServicePlayType PlayType { get; set; }

        public MusicServiceType ServiceType { get; set; }

        uint _cacheMax = 5;
        public uint CacheMax {
            get { return _cacheMax; }
            set { _cacheMax = value; }
        }

        #endregion

        #region Binding properties

        ObservableCollection<MHzSong> _songList;
        public ObservableCollection<MHzSong> SongList { get { return _songList ?? (_songList = new ObservableCollection<MHzSong>()); } }

        #endregion

    }

    public enum MusicServicePlayType { StreamPlay, AutoRepeat, SingletonPlay, ShufflePlay }

    public enum MusicServiceType { MHz, SongList }

    public static class MusicServiceHelper{

        public static DateTime UTCPoint { get { return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc); } }

        #region Create Playback Item

        public static MediaPlaybackItem CreatePlayItem(MHzSong song) {
            var succeed_img = Uri.TryCreate(song.Picture, UriKind.Absolute, out var img_url);
            if (!succeed_img)
                img_url = new Uri("ms-appx:///Assets/star006.png");
            return CreatePlayItem(
                url: song.Url,
                img: img_url,
                title: song.Title,
                artist: song.Artist,
                albumTitle: song.AlbumTitle,
                albunmArtist: song.SingerShow,
                para: MusicIsCurrent = new MusicBoardParameter { AID = song.AID, SID = song.SID, SSID = song.SSID, SHA256 = song.SHA256 });
        }

        public static MediaPlaybackItem CreatePlayItem(Uri uri, Uri img, MusicBoardParameter para, string title, string artist, string albumTitle, string albunmArtist) {
            var source = MediaSource.CreateFromUri(uri);
            source.CustomProperties["Title"] = title;
            source.CustomProperties["CheckPoint"] = SetCheckPoint(UTCPoint);
            source.CustomProperties["SHA256"] = para.SHA256;
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

        #endregion

        #region MHz Extensions

        public static void ChangeServiceChoice(this DoubanMusicService service, bool is_mhz) {
            var succeed = service.ResetPlayer();
            if (succeed)
                service.Player.Source = is_mhz ? service.MHzList : service.PlayList;
        }

        public static void InitServiceStyle(this DoubanMusicService service, MusicServiceType type) {
            switch (type) {
                case MusicServiceType.MHz:
                    service.ChangeServiceChoice(true);
                    break;
                case MusicServiceType.SongList:
                    service.ChangeServiceChoice(false);
                    break;
            }
        }

        #endregion

        #region SongList Extensions

        public static void ChangePlayChoice(this DoubanMusicService service, bool shuffle, bool autoRepeat, bool singleton) {
            service.PlayList.ShuffleEnabled = shuffle;
            service.PlayList.AutoRepeatEnabled = autoRepeat;
            service.SingletonPlay = singleton;
        }

        public static void InitPlayStyle(this DoubanMusicService service, MusicServicePlayType type) {
            switch (type) {
                case MusicServicePlayType.ShufflePlay:
                    service.ChangePlayChoice(true, true, false);
                    break;
                case MusicServicePlayType.AutoRepeat:
                    service.ChangePlayChoice(false, true, false);
                    break;
                case MusicServicePlayType.SingletonPlay:
                    service.ChangePlayChoice(false, false, false);
                    break;
                default:
                    service.ChangePlayChoice(false, false, false);
                    break;
            }
        }

        public static MusicServicePlayType ChangePlayStyle(this DoubanMusicService service) {
            var returns = default(MusicServicePlayType);
            switch (service.PlayType) {
                case MusicServicePlayType.ShufflePlay:
                    service.ChangePlayChoice(false, false, false);
                    returns = service.PlayType = MusicServicePlayType.StreamPlay;
                    break;
                case MusicServicePlayType.AutoRepeat:
                    service.ChangePlayChoice(true, true, false);
                    returns = service.PlayType = MusicServicePlayType.ShufflePlay;
                    break;
                case MusicServicePlayType.SingletonPlay:
                    service.ChangePlayChoice(false, true, false);
                    returns = service.PlayType = MusicServicePlayType.AutoRepeat;
                    break;
                default:
                    service.ChangePlayChoice(false, false, true);
                    returns = service.PlayType = MusicServicePlayType.SingletonPlay;
                    break;
            }
            return returns;
        }

        #endregion

    }

}
