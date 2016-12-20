using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Globalization.Collation;

namespace Wallace.UWP.Helpers.Tools {
    /// <summary>
    /// 分组模型模板
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AlphaKeyGroup<T> {
        const string GlobeGroupKey = "?";
        public string Key { get; private set; }
        public List<T> InternalList { get; private set; }
        public AlphaKeyGroup(string key) {
            Key = key;
            InternalList = new List<T>();
        }

        /// <summary>
        /// 原始分组创建
        /// </summary>
        /// <param name="groupings"></param>
        /// <returns></returns>
        private static List<AlphaKeyGroup<T>> CreateDefaultGroups(CharacterGroupings groupings) {
            List<AlphaKeyGroup<T>> list = new List<AlphaKeyGroup<T>>();
            foreach (CharacterGrouping one in groupings) {
                if (one.Label == "")
                    continue;
                if (one.Label == "...") {
                    list.Add(new AlphaKeyGroup<T>(GlobeGroupKey));
                } else {
                    list.Add(new AlphaKeyGroup<T>(one.Label));
                }
            }
            return list;
        }

        /// <summary>
        /// 自定义分组风格的创建
        /// </summary>
        /// <returns></returns>
        private static List<AlphaKeyGroup<T>> CreateAZGroups() {
            char[] alpha = "&ABCDEFGHIJKLMNOPQRSTUVWXYZ@#".ToCharArray();
            var list = alpha.Select(c => new AlphaKeyGroup<T>(c.ToString())).ToList();
            return list;
        }

        /// <summary>
        /// 创建Groups分组
        /// </summary>
        /// <param name="items"></param>
        /// <param name="keySelector"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public static List<AlphaKeyGroup<T>> CreateGroups(IEnumerable<T> items, Func<T, string> keySelector, bool sort) {
            CharacterGroupings newGroupings = new CharacterGroupings();
            List<AlphaKeyGroup<T>> list = CreateAZGroups();
            foreach (T item in items) {
                int index = 0;
                string label = ConvertAlphaKeyGroup(keySelector(item));
                index = list.FindIndex(alphakeygroup => (alphakeygroup.Key.Equals(label, StringComparison.CurrentCulture)));
                if (index >= 0 && index < list.Count) {
                    list[index].InternalList.Add(item);
                }
            }
            if (sort) {
                foreach (AlphaKeyGroup<T> group in list) {
                    group.InternalList.Sort((c0, c1) => { return keySelector(c0).CompareTo(keySelector(c0)); });
                }
            }
            return list;
        }

        /// <summary>
        /// 创建MatchGroups分组
        /// </summary>
        /// <param name="items">未分组数据源集合</param>
        /// <param name="keySelector">筛选键值</param>
        /// <param name="sort">是否进行排序</param>
        /// <returns></returns>
        public static List<AlphaKeyGroup<T>> CreateGroupsForMatch(IEnumerable<T> items, Func<T, string> keySelector, bool sort) {
            List<AlphaKeyGroup<T>> list = new List<AlphaKeyGroup<T>>();
            foreach (T item in items) {
                int index = 0;
                string label = keySelector(item);
                index = list.FindIndex(alphakeygroup => (alphakeygroup.Key.Equals(label, StringComparison.CurrentCulture)));
                if (index == -1) {
                    AlphaKeyGroup<T> group = new AlphaKeyGroup<T>(label);
                    group.InternalList.Add(item);
                    list.Add(group);
                } else if (index >= 0 && index < list.Count) {
                    list[index].InternalList.Add(item);
                }
            }
            if (sort) {
                foreach (AlphaKeyGroup<T> group in list) {
                    group.InternalList.Sort((c0, c1) => { return keySelector(c0).CompareTo(keySelector(c0)); });
                }
            }
            return list;
        }

        /// <summary>
        /// 值转换器
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string ConvertAlphaKeyGroup(string item) {
            string value = default(string);
            switch (item) {
                case "0":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                    value = "&";
                    break;
                case "?":
                    value = "#";
                    break;
                case "@":
                    value = "@";
                    break;
                default:
                    value = item;
                    break;
            }
            return value;
        }
    }
}
