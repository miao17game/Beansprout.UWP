using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models {
    public enum NavigateType {
        NULL,
        A_D_T,
        Search,
        Login,
        UserInfo,
        InfoItemClick,
        Index,
        Movie,
        TV,
        Book,
        Music,
        FM,
        FM_Extensions,
        Settings,
        Webview,
        ItemClick,
        ItemClickNative,
        DouList,
        MovieContent,
        MovieFilter,
        TVContent,
        TVFilter,
        BookContent,
        BookFilter,
        MusicContent,
        MusicFilter,
        Undefined
    }

    public enum FrameType {
        Content,
        Metro,
        UpContent,
        UserInfos,
        InfosDeatils,
        LeftPart,
        Login,
    }
}
