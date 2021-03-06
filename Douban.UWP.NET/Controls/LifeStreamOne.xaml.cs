﻿using System;
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
    public sealed partial class LifeStreamOne : UserControl {
        public LifeStreamOne() {
            this.InitializeComponent();
            this.Main.DataContext = this;
        }

        public static readonly DependencyProperty ActivityProperty = DependencyProperty.Register("Activity", typeof(string), typeof(LifeStreamOne), null);
        public string Activity {
            get { return GetValue(ActivityProperty) as string; }
            set { SetValue(ActivityProperty, value); }
        }

        public static readonly DependencyProperty TimeProperty = DependencyProperty.Register("Time", typeof(string), typeof(LifeStreamOne), null);
        public string Time {
            get { return GetValue(TimeProperty) as string; }
            set { SetValue(TimeProperty, value); }
        }

        public static readonly DependencyProperty CoverProperty = DependencyProperty.Register("Cover", typeof(string), typeof(LifeStreamOne), null);
        public string Cover {
            get { return GetValue(CoverProperty) as string; }
            set { SetValue(CoverProperty, value); }
        }

        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(string), typeof(LifeStreamOne), null);
        public string Type {
            get { return GetValue(TypeProperty) as string; }
            set { SetValue(TypeProperty, value); }
        }

        public static readonly DependencyProperty HasCoverProperty = DependencyProperty.Register("HasCover", typeof(bool), typeof(LifeStreamOne), PropertyMetadata.Create(false));
        public bool HasCover {
            get { return (bool)GetValue(HasCoverProperty); }
            set { SetValue(HasCoverProperty, value); }
        }

        public static readonly DependencyProperty HasImagesProperty = DependencyProperty.Register("HasImages", typeof(bool), typeof(LifeStreamOne), PropertyMetadata.Create(false));
        public bool HasImages {
            get { return (bool)GetValue(HasImagesProperty); }
            set { SetValue(HasImagesProperty, value); }
        }

        public static readonly DependencyProperty HasAlbumProperty = DependencyProperty.Register("HasAlbum", typeof(bool), typeof(LifeStreamOne), PropertyMetadata.Create(false));
        public bool HasAlbum {
            get { return (bool)GetValue(HasAlbumProperty); }
            set { SetValue(HasAlbumProperty, value); }
        }

        public static readonly DependencyProperty StreamContentProperty = DependencyProperty.Register("StreamContent", typeof(object), typeof(LifeStreamOne), null);
        public object StreamContent {
            get { return GetValue(StreamContentProperty) as object; }
            set { SetValue(StreamContentProperty, value); }
        }

        public static readonly DependencyProperty LikersCountProperty = DependencyProperty.Register("LikersCount", typeof(int), typeof(LifeStreamOne), PropertyMetadata.Create(0));
        public int LikersCount {
            get { return (int)GetValue(LikersCountProperty); }
            set { SetValue(LikersCountProperty, value); }
        }

        public static readonly DependencyProperty CommentsCountProperty = DependencyProperty.Register("CommentsCount", typeof(int), typeof(LifeStreamOne), PropertyMetadata.Create(0));
        public int CommentsCount {
            get { return (int)GetValue(CommentsCountProperty); }
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
