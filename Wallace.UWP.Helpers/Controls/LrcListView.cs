using System;
using Windows . UI . Text;
using Windows . UI . Xaml;
using Windows . UI . Xaml . Controls;
using Windows . UI . Xaml . Media;
using Windows . UI . Xaml . Media . Animation;

namespace Wallace.UWP.Helpers.Controls {
    public class LrcListView : ListView{
        public LrcListView ( ) {
            RenderTransform = new CompositeTransform ( );
        }
    }
}
