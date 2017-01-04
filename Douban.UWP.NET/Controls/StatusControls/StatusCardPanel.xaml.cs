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
    public sealed partial class StatusCardPanel : UserControl {
        public StatusCardPanel() {
            this.InitializeComponent();
            this.Main.DataContext = this;
        }

        public static readonly DependencyProperty HasCoverProperty = DependencyProperty.Register("HasCover", typeof(bool), typeof(StatusCardPanel), PropertyMetadata.Create(true));
        public bool HasCover {
            get { return (bool)GetValue(HasCoverProperty); }
            set { SetValue(HasCoverProperty, value); }
        }

        public static readonly DependencyProperty HasRatingProperty = DependencyProperty.Register("HasRating", typeof(bool), typeof(StatusCardPanel), PropertyMetadata.Create(true));
        public bool HasRating {
            get { return (bool)GetValue(HasRatingProperty); }
            set { SetValue(HasRatingProperty, value); }
        }

        public static readonly DependencyProperty RatingProperty = DependencyProperty.Register("Rating", typeof(double), typeof(StatusCardPanel), PropertyMetadata.Create((double)0));
        public double Rating {
            get { return (double)GetValue(RatingProperty); }
            set { SetValue(RatingProperty, value); }
        }

        public static readonly DependencyProperty CoverProperty = DependencyProperty.Register("Cover", typeof(Uri), typeof(StatusCardPanel), null);
        public Uri Cover {
            get { return GetValue(CoverProperty) as Uri; }
            set { SetValue(CoverProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(StatusCardPanel), null);
        public string Title {
            get { return GetValue(TitleProperty) as string; }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty AbstractProperty = DependencyProperty.Register("Abstract", typeof(string), typeof(StatusCardPanel), null);
        public string Abstract {
            get { return GetValue(AbstractProperty) as string; }
            set { SetValue(AbstractProperty, value); }
        }

    }
}
