using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallace.UWP.Helpers.Tools {

    public class NativeJavascriptBag {

        private StringBuilder string_builder;
        private StringBuilder Builder { get { return string_builder ?? (string_builder = new StringBuilder()); } }

        public string Script { get { return string_builder.ToString(); } }

        public NativeJavascriptBag AppendScript(string javascript) {
            this.Builder.AppendLine(javascript);
            return this;
        }

    }

    public static class NativeJavascriptHelper {

        public const string ScrollHide = @"
            document.body.style.overflow = 'hidden';
            window.external.notify(JSON.stringify('scrollheight:'+document.body.scrollHeight));";

        public const string ActionLinkExpand = @"
            var coll = document.getElementsByTagName('a');
            for(i=0;i<coll.length;i++){
                coll[i].setAttribute('onclick','send_path_url(""actionlink:'+ coll[i].getAttribute('href') +'"")');
                coll[i].setAttribute('href','');
            }";

        public const string ImageClick = @"
            var pics = document.getElementsByTagName('img');
            for(i=0;i<pics.length;i++){
                pics[i].setAttribute('onclick','send_path_url(""picturelink:'+ pics[i].getAttribute('src') +'"")');
            }";

        public const string LikeBtnClick = @"
            var likebtn = document.getElementById('yeslike-btn');
            if(likebtn!=null){
                likebtn.setAttribute('onclick','send_path_url(""like-note-link:'+ likebtn.getAttribute('data-url') +'/like"")');
                window.external.notify(JSON.stringify('isliked:'+'false'));
            }
            var dislikebtn = document.getElementById('dislike-btn');
            if(dislikebtn!=null){
                dislikebtn.setAttribute('onclick','send_path_url(""like-note-link:'+ dislikebtn.getAttribute('data-url') +'/unlike"")');
                window.external.notify(JSON.stringify('isliked:'+'true'));
            }";

        public static string MobileScrollEvent = @"
            window.onscroll = function () {  
                window.external.notify(JSON.stringify('scrolltop:'+document.body.scrollTop));
            };";

    }

}
