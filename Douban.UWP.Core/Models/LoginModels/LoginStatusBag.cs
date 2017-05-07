using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models {
    public class LoginStatusBag {

        public string UserName { get; set; }

        public string UserId { get; set; }

        public string ImageUrl { get; set; }

        public string BigHeadUrl { get; set; }

        public string LocationString { get; set; }

        public string LocationUrl { get; set; }

        public string Description { get; set; }

        public APIUserinfos APIUserinfos { get; set; }

    }

    [DataContract]
    public class APIUserinfos {

        [DataMember(Name = "name")]
        public string AbstractName { get; set; }

        [DataMember(Name = "avatar")]
        public string Avatar { get; set; }

        [DataMember(Name = "birthday")]
        public string Birthday { get; set; }

        [DataMember(Name = "can_donate")]
        public bool CanDonate { get; set; }

        [DataMember(Name = "can_set_original")]
        public bool CanSetOriginal { get; set; }

        [DataMember(Name = "ark_published_count")]
        public uint ArkPublishedCount { get; set; }

        [DataMember(Name = "collected_subject_count")]
        public uint CollectedSubjectsCount { get; set; }

        [IgnoreDataMember]
        public uint DramasCount { get; set; }

        [DataMember(Name = "followed")]
        public bool Followed { get; set; }

        [DataMember(Name = "following_count")]
        public uint FollowingCount { get; set; }

        [DataMember(Name = "followers_count")]
        public uint FollowersCount { get; set; }

        [DataMember(Name = "following_doulist_count")]
        public uint FollowingDouListCount { get; set; }

        [DataMember(Name = "gender")]
        public string Gender { get; set; }

        [DataMember(Name = "group_chat_count")]
        public uint GroupChatCount { get; set; }

        [DataMember(Name = "has_user_hot_module")]
        public bool HasUserHotModule { get; set; }

        [IgnoreDataMember]
        public string ID { get => UserUid; }

        [DataMember(Name = "in_blacklist")]
        public bool InBlackList { get; set; }

        [DataMember(Name = "intro")]
        public string Introductions { get; set; }

        [DataMember(Name = "is_normal")]
        public bool IsNormal { get; set; }

        [DataMember(Name = "joined_group_count")]
        public uint JoinedGroupCount { get; set; }

        [DataMember(Name = "kind")]
        public string Kind { get; set; }

        [DataMember(Name = "large_avatar")]
        public string LargeAvatar { get; set; }

        [IgnoreDataMember]
        public string LocationID { get => Location.ID; }

        [IgnoreDataMember]
        public string LocationUid { get => Location.UID; }

        [IgnoreDataMember]
        public string LocationName { get => Location.Name; }

        [DataMember(Name = "notes_count")]
        public uint NotesCount { get; set; }

        [DataMember(Name = "owned_doulist_count")]
        public uint OwnedDouListCount { get; set; }

        [DataMember(Name = "photo_albums_count")]
        public uint PhotoAlbumsCount { get; set; }

        [IgnoreDataMember]
        public string ProfileBannerLarge { get => ProfileBanner?.Large; }

        [IgnoreDataMember]
        public string ProfileBannerNormal { get => ProfileBanner?.Normal; }

        [DataMember(Name = "reg_time")]
        public string RegisterTime { get; set; }

        [DataMember(Name = "remark")]
        public string Remark { get; set; }

        [IgnoreDataMember]
        public uint SetiChannelCount { get; set; }

        [DataMember(Name = "statuses_count")]
        public uint StatusesCount { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "uid")]
        public string UserUid { get; set; }

        [DataMember(Name = "updated_profile")]
        public bool UpdatedProfile { get; set; }

        [DataMember(Name = "uri")]
        public string Uri { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "user_hot_module_enabled")]
        public bool UserHotModuleEnabled { get; set; }

        [DataMember(Name = "verify_reason")]
        public string VerifyReason { get; set; }

        [DataMember(Name = "verify_type")]
        public string VerifyType { get; set; }

        [DataMember(Name = "loc")]
        public UserLocation Location { get; set; }

        [DataMember(Name = "profile_banner")]
        public UserProfileBanner ProfileBanner { get; set; }

    }

    [DataContract]
    public class UserLocation {

        [DataMember(Name = "id")]
        public string ID { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "uid")]
        public string UID { get; set; }

    }

    [DataContract]
    public class UserProfileBanner {

        [DataMember(Name = "is_default")]
        public bool IsDefault { get; set; }

        [DataMember(Name = "large")]
        public string Large { get; set; }

        [DataMember(Name = "normal")]
        public string Normal { get; set; }

    }
}
