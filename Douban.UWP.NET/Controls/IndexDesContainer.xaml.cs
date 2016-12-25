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

        private int num = 0;

        public IndexDesContainer() {
            this.InitializeComponent();
            this.Main.DataContext = this; // important !!!!
            InnerButton = this.ClickButton;
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

        public static readonly DependencyProperty LikersCountProperty = DependencyProperty.Register("LikersCount", typeof(uint), typeof(IndexDesContainer), null);
        public uint LikersCount {
            get { return (uint)GetValue(LikersCountProperty); }
            set { SetValue(LikersCountProperty, value); }
        }

        public static readonly DependencyProperty PicturesCountProperty = DependencyProperty.Register("PicturesCount", typeof(uint), typeof(IndexDesContainer), null);
        public uint PicturesCount {
            get { return (uint)GetValue(PicturesCountProperty); }
            set { SetValue(PicturesCountProperty, value); }
        }

        public static readonly DependencyProperty CommentCountProperty = DependencyProperty.Register("CommentCount", typeof(uint), typeof(IndexDesContainer), null);
        public uint CommentCount {
            get { return (uint)GetValue(CommentCountProperty); }
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

        private void AdaptViewStatusChanged() {
            //if (num <= 1) { num++; return; }
            if (this.MorePictures != null && this.MorePictures.Count > 1) {
                Grid.SetColumnSpan(this.TitleBlock, 2);
                Grid.SetColumnSpan(this.SummaryBlock, 2);
                Grid.SetRow(this.TitleBlock, 2);
                Grid.SetRow(this.SummaryBlock, 3);
                this.ConatinerGrid.Children.Remove(this.ContainerImage);
                var temp = this.ContainerImage;
                this.ContainerImage = new Image { Height = 240, Source = temp.Source, HorizontalAlignment = HorizontalAlignment.Stretch, Stretch = Stretch.Fill, MaxWidth = 480 };
                Grid.SetRow(this.TitleBlock, 2);
                this.ConatinerGrid.Children.Add(this.ContainerImage);
                Grid.SetColumn(this.ContainerImage, 0);
                Grid.SetRowSpan(this.ContainerImage, 2);
                Image newimg01 = new Image { Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(this.MorePictures.ElementAt(0)), Width = 120, Height = 120 };
                Image newimg02 = new Image { Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(this.MorePictures.ElementAt(1)), Width = 120, Height = 120 };
                Grid grid = new Grid();
                grid.Children.Add(newimg02);
                grid.Children.Add(new Windows.UI.Xaml.Shapes.Rectangle { Fill = new SolidColorBrush(Windows.UI.Colors.Black), Opacity = 0.3 });
                grid.Children.Add(new TextBlock {
                    Text = (this.PicturesCount - 3).ToString() + "+",
                    Foreground = new SolidColorBrush(Windows.UI.Colors.White),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                });
                this.ConatinerGrid.Children.Add(newimg01);
                this.ConatinerGrid.Children.Add(grid);
                Grid.SetColumn(newimg01, 1);
                Grid.SetColumn(grid, 1);
                Grid.SetRow(newimg01, 0);
                Grid.SetRow(grid, 1);
            } else if (this.Cover == null) {
                //this.ContainerImage.Visibility = Visibility.Collapsed;
                //Grid.SetColumnSpan(this.TitleBlock, 2);
                //Grid.SetColumnSpan(this.SummaryBlock, 2);
            }
        }

        private void ContainerImage_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args) {
            
        }

        private void Main_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args) {
            AdaptViewStatusChanged();
        }
    }
}
