using Wallace.UWP.Helpers.Tools;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Douban.UWP.Core.Tools {

    public static class XHtmlHelpers {

        public static string CreateHtml(string bodyContent, bool isGlobalDark = true) {
            return @"<html>" + SetHtmlChildren(bodyContent, isGlobalDark) + @"</html>";
        }

        public static string SetHtmlChildren(string bodyContent, bool isGlobalDark) {
            return CreateHead(CreateCss(default_css) + SetDefaultJs()) + CreateBody(SetThemeStyle(isGlobalDark) + bodyContent);
        }

        private static string SetThemeStyle(bool isGlobalDark) {
            return CreateCss(@"
                body{
                font-family:Segoe UI;
                font-size:" + (UWPStates.IsMobile ? "10" : "16") + @"px;
                background: #" + (isGlobalDark ? "202020" : "fff") + @";
                color: #" + (isGlobalDark ? "fff" : "202020") + @";
                }
            " + GetCopyrightAndLike() + GetHeaderCss() + GetTagsAndAuthorCss());
        }

        private static string SetDefaultJs() {
            return CreateJs(@"
                function send_path_url(path){
                    window.external.notify(JSON.stringify(path));
                }
        ");
        }

        public static string CreateJs(string js) {
            return $@"<script type='text/javascript'>{js}</script> ";
        }

        public static string CreateCss(string css) {
            return @"<style type='text/css'>" + css + @"</style>";
        }

        public static string CreateCssSrc(string uri) {
            return $@"<style type='text/css' src='{uri}'></style>";
        }

        private static string GetCss(string name) {
            return $@"ms-appx:///Resources/CSS/{name}.css";
        }

        private static string CreateBody(string content) {
            return $@"<body><div>{content}</div></body>";
        }

        private static string CreateHead(string head) {
            return $@"<head>{head}</head>";
        }

        #region Css

        private static string GetCopyrightAndLike() {
            return @"
            .copyright-notice {
                font-size: 12px;
                color: #909090;
                margin: 15px 0;
                line-height: 1.2;
                text-align: justify;
            }

            .like-btn {
                margin: 30px auto 40px;
                width: 111px;
                height: 32px;
                line-height: 32px;
                font-size: 18px;
                text-align: center;
                border-radius: 4px;
                border: solid 1px #31C79B;
                color: #31C79B;
                cursor: pointer;
                display: block;
            }

            .like-btn.active {
                border: solid 0.5px #808080;
                color: #808080;
            }";
        }

        private static string GetHeaderCss() {
            return @"

            .header {
                position: relative;
                margin-bottom: 0px;
            }

            .header .title {
                overflow: hidden;
            }

            h1 {
                margin: 20px 0 5px;
                font-size: 20px;
                line-height: 22px;
                word-break: break-all;
            }

            h1, h2, h3 {
                font-weight: normal;
            }

            .user-title {
                height: 22px;
                line-height: 20px;
            }

            .user-title {
                margin-top: -20px;
                margin-bottom: 40px;
            }

            .user-title .info {
                color: #494949;
            }

            .user-title .info {
                color: #aaa;
                font-size: 13px;
                line-height: normal;
            }

            .user-title .info .timestamp {
                color: #ccc;
                margin-left: 5px;
            }
            ";
        }

        private static string GetTagsAndAuthorCss() {
            return @"

            .tags p {
                color: #aaa;
            }

            section p {
                font-size: 15px;
                color: #494949;
            }

            section p, section h3 {
                line-height: 22px;
                word-wrap: break-word;
                margin: 0;
                padding: 0;
            }

            .tags ul {
                font-size: 0px;
            }

            ul {
                list-style: none;
                margin: 0;
                padding: 0;
            }

            .tags ul li {
                display: inline-block;
                margin: 10px 10px 0 0;
                font-size: 15px;
            }

            .tags ul li a {
                font-size: 15px;
                line-height: 28px;
                padding: 0 12px;
                border-radius: 28px;
                text-align: center;
                color: #494949;
                background: #f5f5f5;
                display: block;
            }

            a {
                color: #42bd56;
                text-decoration: none;
            }

            input, textarea, button, select, a {
                -webkit-tap-highlight-color: transparent;
            }

            input, textarea, button, select, a {
                -webkit-tap-highlight-color: transparent;
            }

            .tags {
                margin: 30px 0;
            }

            .note-author::before {
                left: 0px;
                top: -1px;
                width: 100%;
                height: 1px;
                background: #E8E8E8;
                -webkit-transform: scaleY(0.5);
                content: "";
                position: absolute;
                -webkit-transform-origin: 0px bottom 0px;
            }

            .note-author::after {
                left: 0px;
                bottom: 0px;
                width: 100%;
                height: 1px;
                background: #E8E8E8;
                -webkit-transform: scaleY(0.5);
                content: "";
                position: absolute;
                -webkit-transform-origin: 0px bottom 0px;
            }

            .note-author {
                display: -webkit-box;
                display: -webkit-flex;
                display: flex;
                -webkit-box-align: center;
                -webkit-align-items: center;
                align-items: center;
                height: 100px;
                margin-left: -18px;
                margin-right: -18px;
                border-width: 1px 0;
                position: relative;
                margin-top: 50px;
            }

            section {
                margin-bottom: 30px;
            }

            .note-author .avatar {
                border-radius: 50%;
                width: 52px;
                height: 52px;
                margin-left: 18px;
                margin-right: 18px;
                display: block;
            }

            .note-author .author-info::after {
                content: "";
                display: block;
                position: absolute;
                width: 10px;
                height: 10px;
                right: 18px;
                top: 48px;
                border: 2px solid #ccc;
                border-bottom: 0;
                border-left: 0;
                -webkit-transform: rotate(45deg);
                transform: rotate(45deg);
                -webkit-transform-origin: 50% top 0px;
                transform-origin: 50% top 0px;
            }

            .note-author .author-info {
                font-size: 15px;
                color: #9b9b9b;
                -webkit-box-flex: 1;
                -webkit-flex: 1;
                flex: 1;
            }

            .note-author .author-info .author-name {
                font-weight: bold;
                color: #494949;
                margin-left: 0.3em;
            }

            .note-author .author-info .author-details {
                font-size: 13px;
                line-height: 20px;
            }

            ";
        }

        static string default_css = @"

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
                ::-webkit-scrollbar-thumb:vertical:hover {
                    background-color: rgba(0,20,20,20);
                }
                /*滚动条按下*/
                ::-webkit-scrollbar-thumb:vertical:active {
                    background-color: rgba(0,30,30,30);
                }";
        #endregion

    }

    public static class XHtmlHelperExtensions {

        public static HtmlNode RemoveFormat(this HtmlNode value, string xFormat) {
            var downloadApp = value.SelectSingleNode(xFormat);
            if (downloadApp != null)
                downloadApp.Remove();
            return value;
        }

        public static bool ContainsFormat(this HtmlNode value, string xFormat) {
            var downloadApp = value.SelectSingleNode(xFormat);
            return downloadApp != null;
        }

        public static string GetHtmlFormat(this HtmlNode value, string xFormat) {
            var result = value.SelectSingleNode(xFormat);
            if (result == null)
                return "";
            StringBuilder builder = new StringBuilder();
            result.ChildNodes.ToList().ForEach(item => builder.Append(item.NodeType == HtmlNodeType.Text ? "" : item.OuterHtml + Environment.NewLine ));
            return result != null ? builder.ToString() : "";
        }

        public static HtmlNode SelectSingleNode(this HtmlNode node, string childType, string elementType, string attributeName, bool isIgnoreGeneration = false) {
            return isIgnoreGeneration ?
                node.SelectSingleNode($"//{childType}[@{elementType}='{attributeName}']") :
                node.SelectSingleNode($"{childType}[@{elementType}='{attributeName}']");
        }

        public static HtmlNode RemoveFormat(this HtmlNode value, string childType, string elementType, string attributeName, bool isIgnoreGeneration = true) {
            return isIgnoreGeneration?
                value.RemoveFormat($"//{childType}[@{elementType}='{attributeName}']"): 
                value.RemoveFormat($"{childType}[@{elementType}='{attributeName}']");
        }

        public static bool ContainsFormat(this HtmlNode value, string childType, string elementType, string attributeName, bool isIgnoreGeneration = true) {
            return isIgnoreGeneration ?
                value.ContainsFormat($"//{childType}[@{elementType}='{attributeName}']") :
                value.ContainsFormat($"{childType}[@{elementType}='{attributeName}']");
        }

        public static string GetHtmlFormat(this HtmlNode value, string childType, string elementType, string attributeName, bool isIgnoreGeneration = true) {
            return isIgnoreGeneration ?
                value.GetHtmlFormat($"//{childType}[@{elementType}='{attributeName}']") :
                value.GetHtmlFormat($"{childType}[@{elementType}='{attributeName}']");
        }

    }
}
