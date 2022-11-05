using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTP.Ring.Data.Models;
using System.Text.RegularExpressions;
using OTP.Ring.Common;

namespace OTP.Ring.Models.ViewModel
{
    [Serializable]
    public class SummaryListViewModel
    {
        private const string DefaultIconUrl = "~/Content/Images/shape_square.png";
        private const string URLRegexPattern = @"\A(?<Controller>[^\\]+)\\?(?<Action>[^\\]+)?";

        public string SecurityPageId { get; set; }

        public string URL { get; set; }

        public string Controller
        {
            get
            {
                if (URL != null)
                {
                    var match = Regex.Match(URL, URLRegexPattern);
                    return match.Groups["Controller"].Value;
                }
                else
                {
                    return null;
                }
            }
        }

        public string Action
        {
            get
            {
                if (URL != null)
                {
                    var match = Regex.Match(URL, URLRegexPattern);
                    return match.Groups["Action"].Value;
                }
                else
                {
                    return null;
                }
            }
        }

        public object HtmlStyle
        {
            get
            {
                return new { style = HtmlStyleAttribute.GetStyleString(HtmlStyleCode) };
            }
        }

        public string IconUrl { get; set; }
        public string Name { get; set; }
        public string Order { get; set; }
        public string Description { get; set; }
        public int HtmlStyleCode { get; set; }

        public string IconClass
        {
            get
            {
                return "sprite sprite-" + IconUrl.Substring(IconUrl.LastIndexOf('/') + 1, IconUrl.LastIndexOf('.') - IconUrl.LastIndexOf('/') - 1);
            }
        }
    }
}