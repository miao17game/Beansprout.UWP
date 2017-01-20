using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models {
    public class LoginStatusBag {

        public string UserName { get; set; }

        public string UserUid { get; set; }

        public string ImageUrl { get; set; }

        public string BigHeadUrl { get; set; }

        public string LocationString { get; set; }

        public string LocationUrl { get; set; }

        public string Description { get; set; }

        public APIUserinfos APIUserinfos { get; set; }

    }

    public class APIUserinfos {

        public string AbstractName { get; set; }

        public string Avatar { get; set; }

        public string Birthday { get; set; }

        public bool CanDonate { get; set; }

        public bool CanSetOriginal { get; set; }

        public uint ArkPublishedCount { get; set; }

        public uint CollectedSubjectsCount { get; set; }

        public uint DramasCount { get; set; }

        public bool Followed { get; set; }

        public uint FollowingCount { get; set; }

        public uint FollowersCount { get; set; }

        public uint FollowingDouListCount { get; set; }

        public string Gender { get; set; }

        public uint GroupChatCount { get; set; }

        public bool HasUserHotModule { get; set; }

        public string ID { get; set; }

        public bool InBlackList { get; set; }

        public string Introductions { get; set; }

        public bool IsNormal { get; set; }

        public uint JoinedGroupCount { get; set; }

        public string Kind { get; set; }

        public string LargeAvatar { get; set; }

        public string LocationID { get; set; }

        public string LocationUid { get; set; }

        public string LocationName { get; set; }

        public uint NotesCount { get; set; }

        public uint OwnedDouListCount { get; set; }

        public uint PhotoAlbumsCount { get; set; }

        public string ProfileBannerLarge { get; set; }

        public string ProfileBannerNormal { get; set; }

        public string RegisterTime { get; set; }

        public string Remark { get; set; }

        public uint SetiChannelCount { get; set; }

        public uint StatusesCount { get; set; }

        public string Type { get; set; }

        public string UserUid { get; set; }

        public bool UpdatedProfile { get; set; }

        public string Uri { get; set; }

        public string Url { get; set; }

        public bool UserHotModuleEnabled { get; set; }

        public string VerifyReason { get; set; }

        public string VerifyType { get; set; }

    }
}
