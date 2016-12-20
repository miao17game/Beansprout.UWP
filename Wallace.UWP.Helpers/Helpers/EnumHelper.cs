//*********************************************************
//
// 版权所有 （c） 微软。保留所有权利。
// 此代码受 MIT 许可证 （麻省理工学院）的许可。
// 若提供此代码，作为无担保任何一种，无论是明示或暗示的担保，
// 包括任何用于某一特定的适用性的暗示的担保目的、 适销性或非侵权。
//
//*********************************************************

using System;

namespace Wallace.UWP.Helpers. Helpers {
    /// <summary>
    /// 简单的帮助器，用于将一个字符串值转换为其相应的枚举文字。
    /// 如"Running"-> BackgroundTaskState.Running
    /// </summary>
    public static class EnumHelper {
        public static T Parse<T>(string value) where T : struct {
            return (T)Enum.Parse(typeof(T), value);
        }
    }
}
