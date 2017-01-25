using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Tools.PersonalExpressions {
    public static class EscapeReplace {
        public static string ToEscape(string stringExpress) { return ChangeToEscape(stringExpress); }

        static string ChangeToEscape(string stringExpress) {
            var str = new Regex(@"\&.+?\;").Matches(stringExpress);
            foreach (var item in str) {
                switch ((item as Match).Value) {
                    case "&amp;":
                        stringExpress = new Regex(@"\&amp;").Replace(stringExpress, "&");
                        break;
                    case "&mdash;":
                        stringExpress = new Regex(@"\&mdash;").Replace(stringExpress, "—");
                        break;
                    case "&quot;":
                        stringExpress = new Regex(@"\&quot;").Replace(stringExpress, "\"");
                        break;
                    case "&tilde;":
                        stringExpress = new Regex(@"\&tilde;").Replace(stringExpress, "˜");
                        break;
                    case "&lsquo;":
                        stringExpress = new Regex(@"\&lsquo;").Replace(stringExpress, "‘");
                        break;
                    case "&bdquo;":
                        stringExpress = new Regex(@"\&bdquo;").Replace(stringExpress, "„");
                        break;
                    case "&rsaquo;":
                        stringExpress = new Regex(@"\&rsaquo;").Replace(stringExpress, "›");
                        break;
                    case "&rsquo;":
                        stringExpress = new Regex(@"\&rsquo;").Replace(stringExpress, "’");
                        break;
                    case "&euro;":
                        stringExpress = new Regex(@"\&euro;").Replace(stringExpress, "€");
                        break;
                    case "&lt;":
                        stringExpress = new Regex(@"\&lt;").Replace(stringExpress, "<");
                        break;
                    case "&sbquo;":
                        stringExpress = new Regex(@"\&sbquo;").Replace(stringExpress, "‚");
                        break;
                    case "&gt;":
                        stringExpress = new Regex(@"\&gt;").Replace(stringExpress, ">");
                        break;
                    case "&ndash;":
                        stringExpress = new Regex(@"\&ndash;").Replace(stringExpress, "–");
                        break;
                    case "&ldquo;":
                        stringExpress = new Regex(@"\&ldquo;").Replace(stringExpress, "“");
                        break;
                    case "&permil;":
                        stringExpress = new Regex(@"\&permil;").Replace(stringExpress, "‰");
                        break;
                    case "&circ;":
                        stringExpress = new Regex(@"\&circ;").Replace(stringExpress, "ˆ");
                        break;
                    case "&rdquo;":
                        stringExpress = new Regex(@"\&rdquo;").Replace(stringExpress, "”");
                        break;
                    case "&lsaquo;":
                        stringExpress = new Regex(@"\&lsaquo;").Replace(stringExpress, "‹");
                        break;
                    case "&middot;":
                        stringExpress = new Regex(@"\&middot;").Replace(stringExpress, "·");
                        break;
                    case "&nbsp;":
                        stringExpress = new Regex(@"\&nbsp;").Replace(stringExpress, "");
                        break;
                    default: break;
                }
            }
            return stringExpress;
        }
    }
}
