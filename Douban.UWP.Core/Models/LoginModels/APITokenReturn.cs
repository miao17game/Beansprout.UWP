using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.LoginModels {
    public class APITokenReturn {

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public string UserName { get; set; }

        public string UserId { get; set; }

        public string ExpiresIn { get; set; }

    }
}
