using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OTP.Ring.Common
{
    [Serializable]
    public class ActionLocation
    {
        public ActionLocation()
        {
            Controller = "Home";
            Action = "Index";
        }

        public ActionLocation(string url)
        {
            Controller = "Home";
            Action = "Index";

            Regex regex = new Regex(@"\A(?<controller>([-A-Z0-9]+)){1}(\\(?<action>([-A-Z0-9]+)))?", RegexOptions.IgnoreCase);
            var match = regex.Match(url);

            if (match.Success)
            {
                Controller = match.Groups["controller"].Value;
                Action =  match.Groups["action"].Success ? match.Groups["action"].Value : "Index";
            }
        }

        public string Controller { get; set; }
        public string Action { get; set; }
    }
}
