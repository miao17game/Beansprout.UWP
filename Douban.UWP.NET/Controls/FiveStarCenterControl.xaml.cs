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
    public sealed partial class FiveStarCenterControl : UserControl {
        public FiveStarCenterControl() {
            this.InitializeComponent();
            this.Main.DataContext = this;
        }

        public static readonly DependencyProperty RatingValueProperty = DependencyProperty.Register("RatingValue", typeof(double), typeof(FiveStarCenterControl), null);
        public double RatingValue {
            get { return (double)GetValue(RatingValueProperty); }
            set { SetValue(RatingValueProperty, value); }
        }

        public static readonly DependencyProperty PointSizeProperty = DependencyProperty.Register("PointSize", typeof(double), typeof(FiveStarCenterControl), PropertyMetadata.Create((double)30));
        public double PointSize {
            get { return (double)GetValue(PointSizeProperty); }
            set { SetValue(PointSizeProperty, value); }
        }

    }
}
