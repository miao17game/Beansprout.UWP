//*********************************************************
//
// 版权所有 （c） 微软。保留所有权利。
// 此代码受 MIT 许可证 （麻省理工学院）的许可。
// 若提供此代码，作为无担保任何一种，无论是明示或暗示的担保，
// 包括任何用于某一特定的适用性的暗示的担保目的、 适销性或非侵权。
//
//*********************************************************

using System . IO;
using System . Runtime . Serialization;
using System . Runtime . Serialization . Json;
using System . Text;

namespace Wallace.UWP.Helpers. Helpers {
    /// <summary>
    /// 简单的 JSON 序列化 / 解序列化程序用于在两个进程之间传递消息
    /// </summary>
    public static class JsonHelper {
        /// <summary>
        /// 将可序列化的对象转换为 JSON
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="data">Data model to convert to JSON</param>
        /// <returns>JSON serialized string of data model</returns>
        public static string ToJson<T>(T data) {
            var serializer = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream()) {
                serializer.WriteObject(ms, data);
                var jsonArray = ms.ToArray();
                return Encoding.UTF8.GetString(jsonArray, 0, jsonArray.Length);
            }
        }

        /// <summary>
        /// 将 JSON 转换成一个非序列化对象
        /// </summary>
        /// <typeparam name="T">Type to convert to</typeparam>
        /// <param name="json">JSON serialized object to convert from</param>
        /// <returns>Object deserialized from JSON</returns>
        public static T FromJson<T>(string json) {
            var deserializer = new DataContractJsonSerializer(typeof(T));
            try {
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                    return (T)deserializer.ReadObject(ms);
            } catch (SerializationException ex) {
                // 如果字符串不能从 JSON 解序列化
                // 则将原始字符串通过异常处理抛出
                throw new SerializationException("无法解序列化 JSON: " + json, ex);
            }
        }
    }
}
