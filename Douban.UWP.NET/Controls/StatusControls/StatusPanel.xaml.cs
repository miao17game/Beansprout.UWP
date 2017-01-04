using Douban.UWP.Core.Models.LifeStreamModels;
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
    public sealed partial class StatusPanel : UserControl {
        public StatusPanel() {
            this.InitializeComponent();
            this.Main.DataContext = this;
        }

        public static readonly DependencyProperty ActivityProperty = DependencyProperty.Register("Activity", typeof(string), typeof(StatusPanel), null);
        public string Activity {
            get { return GetValue(ActivityProperty) as string; }
            set { SetValue(ActivityProperty, value); }
        }

        public static readonly DependencyProperty HasImagesProperty = DependencyProperty.Register("HasImages", typeof(bool), typeof(StatusPanel), PropertyMetadata.Create(false));
        public bool HasImages {
            get { return (bool)GetValue(HasImagesProperty); }
            set { SetValue(HasImagesProperty, value); }
        }

        public static readonly DependencyProperty HasCardProperty = DependencyProperty.Register("HasCard", typeof(bool), typeof(StatusPanel), PropertyMetadata.Create(false));
        public bool HasCard {
            get { return (bool)GetValue(HasCardProperty); }
            set { SetValue(HasCardProperty, value); }
        }

        public static readonly DependencyProperty HasTextProperty = DependencyProperty.Register("HasText", typeof(bool), typeof(StatusPanel), PropertyMetadata.Create(false));
        public bool HasText {
            get { return (bool)GetValue(HasTextProperty); }
            set { SetValue(HasTextProperty, value); }
        }

        public static readonly DependencyProperty MorePicturesProperty = DependencyProperty.Register("MorePictures", typeof(List<PictureItemBase>), typeof(StatusPanel), null);
        public List<PictureItemBase> MorePictures {
            get { return GetValue(MorePicturesProperty) as List<PictureItemBase>; }
            set { SetValue(MorePicturesProperty, value); }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(StatusPanel), null);
        public string Text {
            get { return GetValue(TextProperty) as string; }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty LikersCountProperty = DependencyProperty.Register("LikersCount", typeof(string), typeof(StatusPanel), null);
        public string LikersCount {
            get { return GetValue(LikersCountProperty) as string; }
            set { SetValue(LikersCountProperty, value); }
        }

        public static readonly DependencyProperty CommentsCountProperty = DependencyProperty.Register("CommentsCount", typeof(string), typeof(StatusPanel), null);
        public string CommentsCount {
            get { return GetValue(CommentsCountProperty) as string; }
            set { SetValue(CommentsCountProperty, value); }
        }

        public static readonly DependencyProperty ResharesCountProperty = DependencyProperty.Register("ResharesCount", typeof(string), typeof(StatusPanel), null);
        public string ResharesCount {
            get { return GetValue(ResharesCountProperty) as string; }
            set { SetValue(ResharesCountProperty, value); }
        }

        public static readonly DependencyProperty TimeProperty = DependencyProperty.Register("Time", typeof(string), typeof(StatusPanel), null);
        public string Time {
            get { return GetValue(TimeProperty) as string; }
            set { SetValue(TimeProperty, value); }
        }

        public static readonly DependencyProperty CardProperty = DependencyProperty.Register("Card", typeof(StatusCard), typeof(StatusPanel), null);
        public StatusCard Card {
            get { return GetValue(CardProperty) as StatusCard; }
            set { SetValue(CardProperty, value); }
        }

        public static readonly DependencyProperty AvatarProperty = DependencyProperty.Register("Avatar", typeof(string), typeof(StatusPanel), null);
        public string Avatar {
            get { return GetValue(AvatarProperty) as string; }
            set { SetValue(AvatarProperty, value); }
        }

        private void StatusGrid_SizeChanged(object sender, SizeChangedEventArgs e) {
            GridView01.Width = (sender as Grid).ActualWidth;
        }

    }
}
