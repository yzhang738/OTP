using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OTP.Ring.Common
{
    [Serializable]
    public class HtmlStyleAttribute
    {
        [Flags]
        public enum Styles
        {
            Normal = 0,
            Bold = 1,
            Large = 1 << 1,
            Underline = 1 << 2
        };

        public static string GetStyleString(int styles)
        {
            return GetStyleString((Styles)styles);
        }

        public static string GetStyleString(Styles styles)
        {
            string fontWeight = "normal";
            if (styles.HasFlag(Styles.Bold))
            {
                fontWeight = "bold";
            }

            string fontSize = "small";
            if (styles.HasFlag(Styles.Large))
            {
                fontSize = "large";
            }

            string decoration = "none";
            if (styles.HasFlag(Styles.Underline))
            {
                decoration = "underline";
            }

            string style = string.Format(@"font-size: {0}; font-weight: {1}; text-decoration: {2};", fontSize, fontWeight, decoration);

            return style;
        }
    }
}