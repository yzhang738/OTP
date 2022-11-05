using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace OTP.Ring.Common
{
    [Serializable]
    public static class ExtensionMethods
    {
        public static string ToHtmlStyleString(this HtmlStyleAttribute.Styles stylingCode)
        {
            return HtmlStyleAttribute.GetStyleString(stylingCode);
        }

        public static string GetConfigurationSetting(string configurationSettingName)
        {
            string value = null;
            if (RoleEnvironment.IsAvailable)
            {
                value = RoleEnvironment.GetConfigurationSettingValue(configurationSettingName);
            }
            else
            {
                value = System.Configuration.ConfigurationManager.AppSettings[configurationSettingName].ToString();
                //var abc = System.Configuration.ConfigurationManager.ConnectionStrings["OTPDEVConnectionString"].ConnectionString;
            }

            return value;
        }

        public static string GetFormStringValue(this FormCollection form, string key)
        {
            if (!form.AllKeys.Contains(key) || string.IsNullOrEmpty(form[key])) return null;
            return form[key];
        }

        public static int? GetFormIntValue(this FormCollection form, string key)
        {
            if (!form.AllKeys.Contains(key)) return null;
            try
            { 
                return Convert.ToInt32(form[key]);
            }
            catch
            {
                return null;
            }
        }

        public static decimal? GetFormDecimalValue(this FormCollection form, string key)
        {
            if (!form.AllKeys.Contains(key)) return null;
            try
            {
                return Convert.ToDecimal(form[key]);
            }
            catch
            {
                return null;
            }
        }

        public static DateTime GetFormDateTimeValue(this FormCollection form, string key)
        {
            if (!form.AllKeys.Contains(key)) return DateTime.MinValue;
            try
            {
                return Convert.ToDateTime(form[key]);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        public static bool IsPerfTimeValid(this string time)
        {
            try
            {
                if (string.IsNullOrEmpty(time))
                {
                    return true;
                }
                else if (time.Contains(":")) 
                {
                    TimeSpan tsp = TimeSpan.Parse(time);
                    return true;
                }

                int hours = 0;
                int minutes = 0;
                int seconds = 0;
                int millseconds = 0;

                string[] arr = time.Split('h', 'm', 's', '.');

                if (time.Contains("h") && time.Contains("m") && time.Contains("s") && time.Contains("."))
                {
                    hours = int.Parse(arr[0].Trim());
                    minutes = int.Parse(arr[1].Trim());
                    seconds = int.Parse(arr[2].Trim());
                    millseconds = (int.Parse(arr[3].Trim()) < 100) ? int.Parse(arr[3].Trim()) * 10 : int.Parse(arr[3].Trim());
                }
                else if (time.Contains("h") && time.Contains("m") && time.Contains("s") && !time.Contains("."))
                {
                    hours = int.Parse(arr[0].Trim());
                    minutes = int.Parse(arr[1].Trim());
                    seconds = int.Parse(arr[2].Trim());
                    millseconds = 0;
                }
                else if (time.Contains("h") && time.Contains("m") && !time.Contains("s") && !time.Contains("."))
                {
                    hours = int.Parse(arr[0].Trim());
                    minutes = int.Parse(arr[1].Trim());
                    seconds = 0;
                    millseconds = 0;
                }
                else if (time.Contains("h") && !time.Contains("m") && time.Contains("s") && time.Contains("."))
                {
                    hours = int.Parse(arr[0].Trim());
                    minutes = 0;
                    seconds = int.Parse(arr[1].Trim());
                    millseconds = (int.Parse(arr[2].Trim()) < 100) ? int.Parse(arr[2].Trim()) * 10 : int.Parse(arr[2].Trim());
                }
                else if (time.Contains("h") && !time.Contains("m") && time.Contains("s") && !time.Contains("."))
                {
                    hours = int.Parse(arr[0].Trim());
                    minutes = 0;
                    seconds = int.Parse(arr[1].Trim());
                    millseconds = 0;
                }
                else if (time.Contains("h") && !time.Contains("m") && !time.Contains("s") && !time.Contains("."))
                {
                    hours = int.Parse(arr[0].Trim());
                    minutes = 0;
                    seconds = 0;
                    millseconds = 0;
                }
                if (!time.Contains("h") && time.Contains("m") && time.Contains("s") && time.Contains("."))
                {
                    hours = 0;
                    minutes = int.Parse(arr[0].Trim());
                    seconds = int.Parse(arr[1].Trim());
                    millseconds = (int.Parse(arr[2].Trim()) < 100) ? int.Parse(arr[2].Trim()) * 10 : int.Parse(arr[2].Trim());
                }
                else if (!time.Contains("h") && time.Contains("m") && time.Contains("s") && !time.Contains("."))
                {
                    hours = 0;
                    minutes = int.Parse(arr[0].Trim());
                    seconds = int.Parse(arr[1].Trim());
                    millseconds = 0;
                }
                else if (!time.Contains("h") && time.Contains("m") && !time.Contains("s") && !time.Contains("."))
                {
                    hours = 0;
                    minutes = int.Parse(arr[0].Trim());
                    seconds = 0;
                    millseconds = 0;
                }
                else if (!time.Contains("h") && !time.Contains("m") && time.Contains("s") && time.Contains("."))
                {
                    hours = 0;
                    minutes = 0;
                    seconds = int.Parse(arr[0].Trim());
                    millseconds = (int.Parse(arr[1].Trim()) < 100) ? int.Parse(arr[1].Trim()) * 10 : int.Parse(arr[1].Trim());
                }
                else if (!time.Contains("h") && !time.Contains("m") && time.Contains("s") && !time.Contains("."))
                {
                    hours = 0;
                    minutes = 0;
                    seconds = int.Parse(arr[0].Trim());
                    millseconds = 0;
                }
                else if (!time.Contains("h") && !time.Contains("m") && !time.Contains("s") && !time.Contains("."))
                {
                    hours = 0;
                    minutes = 0;
                    seconds = 0;
                    millseconds = 0;
                }

                TimeSpan timespan = new TimeSpan(0, hours, minutes, seconds, millseconds);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static TimeSpan? ConvertStringToTimeSpan(this string time)
        {
            try
            {
                if (string.IsNullOrEmpty(time))
                {
                    return null;
                }
                else if (time.Contains(":")) 
                {
                    TimeSpan tsp = TimeSpan.Parse(time);
                    return (tsp == TimeSpan.Zero) ? null : (TimeSpan?)tsp;
                }

                int hours = 0;
                int minutes = 0;
                int seconds = 0;
                int millseconds = 0;

                string[] arr = time.Split('h', 'm', 's', '.');

                if (time.Contains("h") && time.Contains("m") && time.Contains("s") && time.Contains("."))
                {
                    hours = int.Parse(arr[0].Trim());
                    minutes = int.Parse(arr[1].Trim());
                    seconds = int.Parse(arr[2].Trim());
                    millseconds = int.Parse(arr[3].Trim());
                }
                else if (time.Contains("h") && time.Contains("m") && time.Contains("s") && !time.Contains("."))
                {
                    hours = int.Parse(arr[0].Trim());
                    minutes = int.Parse(arr[1].Trim());
                    seconds = int.Parse(arr[2].Trim());
                    millseconds = 0;
                }
                else if (time.Contains("h") && time.Contains("m") && !time.Contains("s") && !time.Contains("."))
                {
                    hours = int.Parse(arr[0].Trim());
                    minutes = int.Parse(arr[1].Trim());
                    seconds = 0;
                    millseconds = 0;
                }
                else if (time.Contains("h") && !time.Contains("m") && time.Contains("s") && time.Contains("."))
                {
                    hours = int.Parse(arr[0].Trim());
                    minutes = 0;
                    seconds = int.Parse(arr[1].Trim());
                    millseconds = int.Parse(arr[2].Trim());
                }
                else if (time.Contains("h") && !time.Contains("m") && time.Contains("s") && !time.Contains("."))
                {
                    hours = int.Parse(arr[0].Trim());
                    minutes = 0;
                    seconds = int.Parse(arr[1].Trim());
                    millseconds = 0;
                }
                else if (time.Contains("h") && !time.Contains("m") && !time.Contains("s") && !time.Contains("."))
                {
                    hours = int.Parse(arr[0].Trim());
                    minutes = 0;
                    seconds = 0;
                    millseconds = 0;
                }
                if (!time.Contains("h") && time.Contains("m") && time.Contains("s") && time.Contains("."))
                {
                    hours = 0;
                    minutes = int.Parse(arr[0].Trim());
                    seconds = int.Parse(arr[1].Trim());
                    millseconds = int.Parse(arr[2].Trim());
                }
                else if (!time.Contains("h") && time.Contains("m") && time.Contains("s") && !time.Contains("."))
                {
                    hours = 0;
                    minutes = int.Parse(arr[0].Trim());
                    seconds = int.Parse(arr[1].Trim());
                    millseconds = 0;
                }
                else if (!time.Contains("h") && time.Contains("m") && !time.Contains("s") && !time.Contains("."))
                {
                    hours = 0;
                    minutes = int.Parse(arr[0].Trim());
                    seconds = 0;
                    millseconds = 0;
                }
                else if (!time.Contains("h") && !time.Contains("m") && time.Contains("s") && time.Contains("."))
                {
                    hours = 0;
                    minutes = 0;
                    seconds = int.Parse(arr[0].Trim());
                    millseconds = int.Parse(arr[1].Trim());
                }
                else if (!time.Contains("h") && !time.Contains("m") && time.Contains("s") && !time.Contains("."))
                {
                    hours = 0;
                    minutes = 0;
                    seconds = int.Parse(arr[0].Trim());
                    millseconds = 0;
                }
                else if (!time.Contains("h") && !time.Contains("m") && !time.Contains("s") && !time.Contains("."))
                {
                    hours = 0;
                    minutes = 0;
                    seconds = 0;
                    millseconds = 0;
                }

                TimeSpan timespan = new TimeSpan(0, hours, minutes, seconds, millseconds * 10);

                return (timespan == TimeSpan.Zero) ? null : (TimeSpan?)timespan;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string ConvertTimeSpanToString(this TimeSpan timespan)
        {
            string answer = string.Empty;

            if (timespan == null)
                return answer;

            if (timespan.TotalSeconds < 60)
            {
                answer = string.Format("{0}.{1:00}s", timespan.Seconds, timespan.Milliseconds / 10);
            }
            else if (timespan.TotalMinutes < 60)
            {
                answer = string.Format("{0}m {1}.{2:00}s", timespan.Minutes, timespan.Seconds, timespan.Milliseconds / 10);
            }
            else
            {
                answer = string.Format("{0}h {1}m {2}.{3:00}s", timespan.Hours, timespan.Minutes, timespan.Seconds, timespan.Milliseconds / 10);
            }

            return answer;
        }
        
        public static bool GetFormBooleanValue(this FormCollection form, string key)
        {
            if (!form.AllKeys.Contains(key)) return false;
            try
            {
                return Convert.ToBoolean(form[key]);
            }
            catch
            {
                return false;
            }
        }

        public static bool GetFormBooleanValue(this FormCollection form, string name, string key)
        {
            if (!form.AllKeys.Contains(name)) return false;
            try
            {
                return form[name].Contains(key);
            }
            catch
            {
                return false;
            }
        }

        public static string ToUrlString(this string url)
        {
            string result = url;

            if (!string.IsNullOrEmpty(url))
            {
                if (!url.ToLower().StartsWith("http://"))
                {
                    url = "http://" + url;
                }

                Uri uri = new Uri(url);
                result = new Uri(url).AbsoluteUri;
            }

            return result;
        }

        public static void AddValidationMessage(this ModelStateDictionary modelState, string key, string message)
        {
            modelState.AddModelError(key,
                string.Format("<span class=\"\" htmlfor=\"{0}\" generated=\"true\" _for_validation_message=\"#{0}_validationMessage\">{1}</span>", key, message));
        }

        public static object GetReflectedPropertyValue(this object subject, string propertyName)
        {
            // Get the property value of the object
            object reflectedValue = subject.GetType().GetProperty(propertyName).GetValue(subject, null);

            Type type = subject.GetType().GetProperty(propertyName).PropertyType;
            if (type == typeof(int))
            {
                return Convert.ToInt32(reflectedValue);
            }
            //else if (type == typeof(bool))
            //{
            //    return Convert.ToBoolean(reflectedValue);
            //}
            else
            {
                return reflectedValue != null ? reflectedValue.ToString().ToLower() : "";
            }
        }

        public static object GetReflectedPropertyValue(this object subject, string propertyName, bool ignoreCase)
        {
            // Get the property value of the object
            object reflectedValue = subject.GetType().GetProperty(propertyName).GetValue(subject, null);

            Type type = subject.GetType().GetProperty(propertyName).PropertyType;
            if (type == typeof(int))
            {
                return Convert.ToInt32(reflectedValue);
            }
            //else if (type == typeof(bool))
            //{
            //    return Convert.ToBoolean(reflectedValue);
            //}
            else
            {
                if (ignoreCase)
                {
                    return reflectedValue != null ? reflectedValue.ToString().ToLower() : "";
                }
                else
                {
                    return reflectedValue != null ? reflectedValue.ToString() : "";
                }
            }
        }

        public static string ConvertDateTimeToString(this object subject, string propertyName)
        {
            // Get the property value of the object
            object propertyValue = subject.GetType().GetProperty(propertyName).GetValue(subject, null);

            if (propertyValue == null)
                return "";

            DateTime datetime = (DateTime)propertyValue;

            return datetime.ToString("yyyy/MM/dd");
        }
    }
}
