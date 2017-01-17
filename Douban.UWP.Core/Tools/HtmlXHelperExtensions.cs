
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Tools {
    public static class HtmlXHelperExtensions {

        public static HtmlNode RemoveFormat(this HtmlNode value, string xFormat) {
            var downloadApp = value.SelectSingleNode(xFormat);
            if (downloadApp != null)
                downloadApp.Remove();
            return value;
        }

        public static HtmlNode RemoveFormat(this HtmlNode value, string childType, string elementType, string attributeName, bool isIgnoreGeneration = true) {
            return isIgnoreGeneration?
                value.RemoveFormat($"//{childType}[@{elementType}='{attributeName}']"): 
                value.RemoveFormat($"{childType}[@{elementType}='{attributeName}']");
        }

        public static bool ContainsFormat(this HtmlNode value, string xFormat) {
            var downloadApp = value.SelectSingleNode(xFormat);
            return downloadApp != null;
        }

        public static bool ContainsFormat(this HtmlNode value, string childType, string elementType, string attributeName, bool isIgnoreGeneration = true) {
            return isIgnoreGeneration ?
                value.ContainsFormat($"//{childType}[@{elementType}='{attributeName}']") :
                value.ContainsFormat($"{childType}[@{elementType}='{attributeName}']");
        }

        public static string GetHtmlFormat(this HtmlNode value, string xFormat) {
            var result = value.SelectSingleNode(xFormat);
            StringBuilder builder = new StringBuilder();
            result.ChildNodes.ToList().ForEach(item => builder.Append(item.NodeType == HtmlNodeType.Text ? "" : item.InnerHtml));
            return result != null ? builder.ToString() : "";
        }

        public static string GetHtmlFormat(this HtmlNode value, string childType, string elementType, string attributeName, bool isIgnoreGeneration = true) {
            return isIgnoreGeneration ?
                value.GetHtmlFormat($"//{childType}[@{elementType}='{attributeName}']") :
                value.GetHtmlFormat($"{childType}[@{elementType}='{attributeName}']");
        }

        public static string CreateHtml(string value, bool isGlobalDark = true) {
            return @"<html>" + SetCss(value, isGlobalDark) + @"</html>";
        }

        public static string SetCss(string value, bool isGlobalDark) {
            return default_CSS + "<body><div>" + SetThemeStyle(isGlobalDark) + value + "</div></body>";
        }

        public static string SetThemeStyle(bool isGlobalDark) {
            return CreateCss(@"
                body{
                font-family:Segoe UI;
                font-size:15px;
                background: " + (isGlobalDark ? "#323232" : "#ffffff") + @";
                color:" + (isGlobalDark ? "#ffffff" : "#323232") + @";
                }
            ");
        }

        public static string CreateCss(string css) {
            return @"<style type='text/css'>" + css + @"</style>";
        }

        #region CSS
        static string default_CSS = @"<head>
        <style type=""text/css"">
        
        /*滚动条宽度*/  
        ::-webkit-scrollbar {  
        width: 8px;  
        }  
   
        /* 轨道样式 */  
        ::-webkit-scrollbar-track {  
        }  
   
        /* Handle样式 */  
        ::-webkit-scrollbar-thumb {  
        border-radius: 10px;  
        background: rgba(0,10,10,10);   
        }  
  
        /*当前窗口未激活的情况下*/  
        ::-webkit-scrollbar-thumb:window-inactive {  
        background: rgba(0,0,0,0.1);   
        }  
  
        /*hover到滚动条上*/  
        ::-webkit-scrollbar-thumb:vertical:hover{  
        background-color: rgba(0,20,20,20);  
        }  
        /*滚动条按下*/  
        ::-webkit-scrollbar-thumb:vertical:active{  
        background-color: rgba(0,30,30,30);  
        }  

	    </style>
        </head>";
        #endregion

    }
}
