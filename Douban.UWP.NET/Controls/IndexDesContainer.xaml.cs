using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Wallace.UWP.Helpers.Tools;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Douban.UWP.NET.Controls {
    public sealed partial class IndexDesContainer : UserControl {

        public IndexDesContainer() {
            this.InitializeComponent();
            this.Main.DataContext = this; // important !!!!
            InnerButton = this.ClickButton;
        }

        public static readonly DependencyProperty SourceHeadProperty = DependencyProperty.Register("SourceHead", typeof(string), typeof(IndexDesContainer), null);
        public string SourceHead {
            get { return GetValue(SourceHeadProperty) as string; }
            set { SetValue(SourceHeadProperty, value); }
        }

        public static readonly DependencyProperty ThisDateProperty = DependencyProperty.Register("ThisDate", typeof(string), typeof(IndexDesContainer), null);
        public string ThisDate {
            get { return GetValue(ThisDateProperty) as string; }
            set { SetValue(ThisDateProperty, value); }
        }

        public static readonly DependencyProperty AuthorNameProperty = DependencyProperty.Register("AuthorName", typeof(string), typeof(IndexDesContainer), null);
        public string AuthorName {
            get { return GetValue(AuthorNameProperty) as string; }
            set { SetValue(AuthorNameProperty, value); }
        }

        public static readonly DependencyProperty AuthorAvatarProperty = DependencyProperty.Register("AuthorAvatar", typeof(Uri), typeof(IndexDesContainer), null);
        public Uri AuthorAvatar {
            get { return GetValue(AuthorAvatarProperty) as Uri; }
            set { SetValue(AuthorAvatarProperty, value); }
        }

        public static readonly DependencyProperty HasCoverProperty = DependencyProperty.Register("HasCover", typeof(bool), typeof(IndexDesContainer), PropertyMetadata.Create(true));
        public bool HasCover {
            get { return (bool)GetValue(HasCoverProperty); }
            set { SetValue(HasCoverProperty, value); }
        }

        public static readonly DependencyProperty HasSourceHeadProperty = DependencyProperty.Register("HasSourceHead", typeof(bool), typeof(IndexDesContainer), PropertyMetadata.Create(false));
        public bool HasSourceHead {
            get { return (bool)GetValue(HasSourceHeadProperty); }
            set { SetValue(HasSourceHeadProperty, value); }
        }

        public static readonly DependencyProperty CoverProperty = DependencyProperty.Register("Cover", typeof(Uri), typeof(IndexDesContainer), null);
        public Uri Cover {
            get { return GetValue(CoverProperty) as Uri; }
            set { SetValue(CoverProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(IndexDesContainer), null);
        public string Title {
            get { return GetValue(TitleProperty) as string; }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(IndexDesContainer), null);
        public string Description {
            get { return GetValue(DescriptionProperty) as string; }
            set { SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty ColumnNameProperty = DependencyProperty.Register("ColumnName", typeof(string), typeof(IndexDesContainer), null);
        public string ColumnName {
            get { return GetValue(ColumnNameProperty) as string; }
            set { SetValue(ColumnNameProperty, value); }
        }

        public static readonly DependencyProperty LikersCountProperty = DependencyProperty.Register("LikersCount", typeof(uint), typeof(IndexDesContainer), PropertyMetadata.Create((uint)0));
        public uint LikersCount {
            get { return (uint)GetValue(LikersCountProperty); }
            set { SetValue(LikersCountProperty, value); }
        }

        public static readonly DependencyProperty PicturesCountProperty = DependencyProperty.Register("PicturesCount", typeof(uint), typeof(IndexDesContainer), PropertyMetadata.Create((uint)0));
        public uint PicturesCount {
            get { return (uint)GetValue(PicturesCountProperty); }
            set { SetValue(PicturesCountProperty, value); }
        }

        public static readonly DependencyProperty CommentCountProperty = DependencyProperty.Register("CommentCount", typeof(uint), typeof(IndexDesContainer), PropertyMetadata.Create((uint)0));
        public uint CommentCount {
            get { return (uint)GetValue(CommentCountProperty); }
            set { SetValue(CommentCountProperty, value); }
        }

        public static readonly DependencyProperty ClickVisibleProperty = DependencyProperty.Register("ClickVisible", typeof(Visibility), typeof(IndexDesContainer), PropertyMetadata.Create(Visibility.Collapsed));
        public Visibility ClickVisible {
            get { return (Visibility)GetValue(ClickVisibleProperty); }
            set { SetValue(ClickVisibleProperty, value); }
        }

        public static readonly DependencyProperty MorePicturesProperty = DependencyProperty.Register("MorePictures", typeof(List<Uri>), typeof(IndexDesContainer), null);
        public List<Uri> MorePictures {
            get { return GetValue(MorePicturesProperty) as List<Uri>; }
            set { SetValue(MorePicturesProperty, value); }
        }

        public static readonly DependencyProperty PanelTypeProperty = DependencyProperty.Register("PanelType", typeof(Core.Models.ListModel.IndexItem.ItemType), typeof(IndexDesContainer), PropertyMetadata.Create(Core.Models.ListModel.IndexItem.ItemType.Normal));
        public Core.Models.ListModel.IndexItem.ItemType PanelType {
            get { return (Core.Models.ListModel.IndexItem.ItemType)GetValue(PanelTypeProperty); }
            set { SetValue(PanelTypeProperty, value); }
        }

        public Button InnerButton;

    }
}
