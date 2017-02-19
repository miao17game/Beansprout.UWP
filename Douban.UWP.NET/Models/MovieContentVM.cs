using static Wallace.UWP.Helpers.Tools.UWPStates;
using static Douban.UWP.NET.Resources.AppResources;

using Douban.UWP.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.NET.Models {
    public class MovieContentVM : ViewModelBase {

        string _title;
        public string Title {
            get { return _title; }
            set { _title = value; RaisePropertyChanged("Title"); }
        }

        string _cover;
        public string Cover {
            get { return _cover; }
            set { _cover = value; RaisePropertyChanged("Cover"); }
        }

        double _rating;
        public double Rating {
            get { return _rating; }
            set { _rating = value; RaisePropertyChanged("Rating"); }
        }

        string _commenter_count;
        public string CommentersCount {
            get { return _commenter_count; }
            set { _commenter_count = value; RaisePropertyChanged("CommentersCount"); }
        }

        string _meta;
        public string Meta {
            get { return _meta; }
            set { _meta = value; RaisePropertyChanged("Meta"); }
        }

        string _intro;
        public string Intro {
            get { return _intro; }
            set { _intro = value; RaisePropertyChanged("Intro"); }
        }

        string _wish_title;
        public string WishTitle {
            get { return _wish_title ?? (_wish_title = GetUIString("Wish_See_Title")); }
            set { _wish_title = value; RaisePropertyChanged("WishTitle"); }
        }

        string _collect_title;
        public string CollectTitle {
            get { return _collect_title ?? (_collect_title = GetUIString("Collect_See_Title")); }
            set { _collect_title = value; RaisePropertyChanged("CollectTitle"); }
        }

        IList<MovieContentTag> _tags_list;
        public IList<MovieContentTag> TagsList {
            get { return _tags_list ?? (_tags_list = new List<MovieContentTag>()); }
            set { _tags_list = value; RaisePropertyChanged("TagsList"); }
        }

        IList<string> _images_list;
        public IList<string> ImageList {
            get { return _images_list ?? (_images_list = new List<string>()); }
            set { _images_list = value; RaisePropertyChanged("ImageList"); }
        }

        IList<object> _comments_list;
        public IList<object> CommentsList {
            get { return _comments_list ?? (_comments_list = new List<object>()); }
            set { _comments_list = value; RaisePropertyChanged("CommentsList"); }
        }

        IList<MovieContentQuestion> _questions;
        public IList<MovieContentQuestion> Questions {
            get { return _questions ?? (_questions = new List<MovieContentQuestion>()); }
            set { _questions = value; RaisePropertyChanged("TagsList"); }
        }

        IList<object> _reviews;
        public IList<object> Reviews {
            get { return _reviews ?? (_reviews = new List<object>()); }
            set { _reviews = value; RaisePropertyChanged("Reviews"); }
        }

    }

    public class MovieContentTag {
        public string TagName { get; set; }
        public string PartUrl { get; set; }
    }

    public class MovieContentQuestion {
        public string Title { get; set; }
        public string Count { get; set; }
        public string PartUrl { get; set; }
    }

}
