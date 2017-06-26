using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Douban.UWP.Core.Models.LifeStreamModels;

namespace Douban.UWP.NET.Controls {
    public sealed partial class LifeStreamSingleton : UserControl {
        public LifeStreamSingleton() {
            this.InitializeComponent();
            this.Main.DataContext = this; 
        }

        public static readonly DependencyProperty ActivityProperty = DependencyProperty.Register("Activity", typeof(string), typeof(LifeStreamSingleton), null);
        public string Activity {
            get { return GetValue(ActivityProperty) as string; }
            set { SetValue(ActivityProperty, value); }
        }

        //public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(InfosItemBase.JsonType), typeof(LifeStreamSingleton), PropertyMetadata.Create(LifeStreamItem.JsonType.Undefined));
        //public InfosItemBase.JsonType Type {
        //    get { return (InfosItemBase.JsonType)GetValue(TypeProperty); }
        //    set { SetValue(TypeProperty, value); }
        //}

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(LifeStreamSingleton), null);
        public string Title {
            get { return GetValue(TitleProperty) as string; }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(LifeStreamSingleton), null);
        public string Text {
            get { return GetValue(TextProperty) as string; }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty AbstractProperty = DependencyProperty.Register("Abstract", typeof(string), typeof(LifeStreamSingleton), null);
        public string Abstract {
            get { return GetValue(AbstractProperty) as string; }
            set { SetValue(AbstractProperty, value); }
        }

        public static readonly DependencyProperty TimeProperty = DependencyProperty.Register("Time", typeof(string), typeof(LifeStreamSingleton), null);
        public string Time {
            get { return GetValue(TimeProperty) as string; }
            set { SetValue(TimeProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(LifeStreamSingleton), null);
        public string Description {
            get { return GetValue(DescriptionProperty) as string; }
            set { SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty HasCoverProperty = DependencyProperty.Register("HasCover", typeof(bool), typeof(LifeStreamSingleton), PropertyMetadata.Create(false));
        public bool HasCover {
            get { return (bool)GetValue(HasCoverProperty); }
            set { SetValue(HasCoverProperty, value); }
        }

        public static readonly DependencyProperty HasImagesProperty = DependencyProperty.Register("HasImages", typeof(bool), typeof(LifeStreamSingleton), PropertyMetadata.Create(false));
        public bool HasImages {
            get { return (bool)GetValue(HasImagesProperty); }
            set { SetValue(HasImagesProperty, value); }
        }

        public static readonly DependencyProperty HasAlbumProperty = DependencyProperty.Register("HasAlbum", typeof(bool), typeof(LifeStreamSingleton), PropertyMetadata.Create(false));
        public bool HasAlbum {
            get { return (bool)GetValue(HasAlbumProperty); }
            set { SetValue(HasAlbumProperty, value); }
        }

        public static readonly DependencyProperty CoverProperty = DependencyProperty.Register("Cover", typeof(Uri), typeof(LifeStreamSingleton), null);
        public Uri Cover {
            get { return GetValue(CoverProperty) as Uri; }
            set { SetValue(CoverProperty, value); }
        }

        public static readonly DependencyProperty MorePicturesProperty = DependencyProperty.Register("MorePictures", typeof(List<PictureItemBase>), typeof(LifeStreamSingleton), null);
        public List<PictureItemBase> MorePictures {
            get { return GetValue(MorePicturesProperty) as List<PictureItemBase>; }
            set { SetValue(MorePicturesProperty, value); }
        }

        public static readonly DependencyProperty AlbumListProperty = DependencyProperty.Register("AlbumList", typeof(List<PictureItem>), typeof(LifeStreamSingleton), null);
        public List<PictureItem> AlbumList {
            get { return GetValue(AlbumListProperty) as List<PictureItem>; }
            set { SetValue(AlbumListProperty, value); }
        }

        public static readonly DependencyProperty LikersCountProperty = DependencyProperty.Register("LikersCount", typeof(string), typeof(LifeStreamSingleton), null);
        public string LikersCount {
            get { return GetValue(LikersCountProperty) as string; }
            set { SetValue(LikersCountProperty, value); }
        }

        public static readonly DependencyProperty CommentsCountProperty = DependencyProperty.Register("CommentsCount", typeof(string), typeof(LifeStreamSingleton), null);
        public string CommentsCount {
            get { return GetValue(CommentsCountProperty) as string; }
            set { SetValue(CommentsCountProperty, value); }
        }

        private void StatusGrid_SizeChanged(object sender, SizeChangedEventArgs e) {
            GridView01.Width = (sender as Grid).ActualWidth;
        }

        private void AlbumGrid_SizeChanged(object sender, SizeChangedEventArgs e) {
            GridView02.Width = (sender as Grid).ActualWidth;
        }
    }
}
