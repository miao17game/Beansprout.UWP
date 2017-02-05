using Douban.UWP.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.NET.Models {
    public class AuthorVM : ViewModelBase {

        string user_name;
        public string UserName {
            get { return user_name; }
            set {
                user_name = value;
                RaisePropertyChanged("UserName");
            }
        }

        Uri user_img;
        public Uri Image {
            get { return user_img; }
            set {
                user_img = value;
                RaisePropertyChanged("Image");
            }
        }

        Uri user_link;
        public Uri Link {
            get { return user_link; }
            set {
                user_link = value;
                RaisePropertyChanged("Link");
            }
        }

        string user_notes;
        public string Notes {
            get { return user_notes; }
            set {
                user_notes = value;
                RaisePropertyChanged("Notes");
            }
        }

        string user_albums;
        public string Albums {
            get { return user_albums; }
            set {
                user_albums = value;
                RaisePropertyChanged("Albums");
            }
        }

        bool is_liked;
        public bool Liked {
            get { return is_liked; }
            set {
                is_liked = value;
                RaisePropertyChanged("Liked");
            }
        }

        int likers_number;
        public int LikersCount {
            get { return likers_number; }
            set {
                likers_number = value;
                RaisePropertyChanged("LikersCount");
            }
        }

    }
}
