using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace Douban.UWP.Core.Models {
    public class LoginReturnBag {
        public string HtmlResouces { get; set; }
        public HttpCookie CookieBag { get; set; }
    }
}
