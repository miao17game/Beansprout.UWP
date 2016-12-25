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

namespace Douban.UWP.NET.Controls {
    public sealed partial class IndexDesContainer : UserControl {
        public IndexDesContainer() {
            this.InitializeComponent();
            this.Main.DataContext = this; // important !!!!
            InnerButton = this.ClickButton;
            ContainerImage.Width = ContainerImage.Height;
            //if (MorePictures != null && MorePictures.Count > 1) {
            //    SummaryBlock.Visibility = Visibility.Collapsed;
            //    Grid.SetRow(TitleBlock, 2);
            //    Grid.SetColumn(ContainerImage, 0);
            //    Grid.SetRowSpan(ContainerImage, 2);
            //    Image newimg01 = new Image { Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(MorePictures.ElementAt(0)) };
            //    Image newimg02 = new Image { Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(MorePictures.ElementAt(1)) };
            //    ConatinerGrid.Children.Add(newimg01);
            //    ConatinerGrid.Children.Add(newimg02);
            //    Grid.SetColumn(newimg01, 1);
            //    Grid.SetColumn(newimg02, 1);
            //    Grid.SetRow(newimg01, 0);
            //    Grid.SetRow(newimg02, 1);
            //}
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

        public static readonly DependencyProperty CoverProperty = DependencyProperty.Register("Cover", typeof(Uri), typeof(IndexDesContainer), null);
        public Uri Cover {
            get { return GetValue(CoverProperty) as Uri; }
            set {
                SetValue(CoverProperty, value);
                if (value == null) {
                    ContainerImage.Visibility = Visibility.Collapsed;
                    Grid.SetColumnSpan(TitleBlock, 2);
                    Grid.SetColumnSpan(SummaryBlock, 2);
                }
            }
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

        public static readonly DependencyProperty LikersCountProperty = DependencyProperty.Register("LikersCount", typeof(int), typeof(IndexDesContainer), null);
        public int LikersCount {
            get { return (int)GetValue(LikersCountProperty); }
            set { SetValue(LikersCountProperty, value); }
        }

        public static readonly DependencyProperty CommentCountProperty = DependencyProperty.Register("CommentCount", typeof(int), typeof(IndexDesContainer), null);
        public int CommentCount {
            get { return (int)GetValue(CommentCountProperty); }
            set { SetValue(CommentCountProperty, value); }
        }

        public static readonly DependencyProperty ClickVisibleProperty = DependencyProperty.Register("ClickVisible", typeof(Visibility), typeof(IndexDesContainer), null);
        public Visibility ClickVisible {
            get { return (Visibility)GetValue(ClickVisibleProperty); }
            set { SetValue(ClickVisibleProperty, value); }
        }

        public static readonly DependencyProperty MorePicturesProperty = DependencyProperty.Register("MorePictures", typeof(ICollection<Uri>), typeof(IndexDesContainer), null);
        public ICollection<Uri> MorePictures {
            get { return GetValue(MorePicturesProperty) as ICollection<Uri>; }
            set { SetValue(MorePicturesProperty, value); }
        }

        public Button InnerButton;
    }
}
