using Wallace.UWP.Helpers.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallace.UWP.Helpers.Tools {
    /// <summary>
    /// 建立AlphaKeyGroupListView绑定数据源
    /// </summary>
    static class GetAlphaKeyGroup {
        /// <summary>
        /// 以ItemContent分组
        /// </summary>
        /// <param name="list">ArchiveCategory列表</param>
        /// <returns></returns>
        /// 暂时注释，需要的时候恢复
        //public static List<AlphaKeyGroup<MatchListModel>> GetAlphaGroupSampleItems(List<MatchListModel> list) {
        //    List<MatchListModel> data = new List<MatchListModel>();
        //    data = list;
        //    List<AlphaKeyGroup<MatchListModel>> groupData = AlphaKeyGroup<MatchListModel>.CreateGroupsForMatch(
        //        data, (MatchListModel s) => {
        //            return s.GroupCategory;
        //        }, true);
        //    return groupData;
        //}
    }
}
