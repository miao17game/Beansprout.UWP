using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.ListModel {

    [DataContract]
    public class ItemGroupBase {

        [DataMember(Name ="count")]
        public uint NowCount { get; set; }

        [DataMember(Name = "start")]
        public uint StartIndex { get; set; }

        [DataMember(Name = "total")]
        public uint Total { get; set; }

        [DataMember(Name = "subject_collection")]
        public SubjectCollectionBase SubjectCollection { get; set; }

        [IgnoreDataMember]
        public string GroupName { get; set; }

        [IgnoreDataMember]
        public string GroupPathUrl { get; set; }

        [IgnoreDataMember]
        public string GroupInnerUri { get; set; }

        [IgnoreDataMember]
        public string Description { get; set; }

        [IgnoreDataMember]
        public string SharingUrl { get; set; }

        [IgnoreDataMember]
        public string Id { get; set; }

        [IgnoreDataMember]
        public string CoverUrl { get; set; }

        [IgnoreDataMember]
        public bool HasCover { get; set; }

        [IgnoreDataMember]
        public virtual IList<object> Items { get; set; }

    }

    [DataContract]
    public class SubjectCollectionBase {

        [DataMember(Name = "name")]
        public string GroupName { get; set; }

        [DataMember(Name = "url")]
        public string GroupPathUrl { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "sharing_url")]
        public string SharingUrl { get; set; }

        [DataMember(Name = "uri")]
        public string GroupPathUri { get; set; }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "cover_url")]
        public string CoverUrl { get; set; }

        public bool HasCover { get { return CoverUrl == null || CoverUrl == "" ? false : true; } }

    }

}
