using System;
using System.Web;

namespace OTP.Ring.Web
{
    public class CookieLocalizationModule : IHttpModule
    {
        /// <summary>
        /// You will need to configure this module in the web.config file of your
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpModule Members

        public void Dispose()
        {
            //clean-up code here.
        }

        public void Init(HttpApplication context)
        {
            // Below is an example of how you can handle LogRequest event and provide 
            // custom logging implementation for it
            context.BeginRequest += new EventHandler(context_BeginRequest);
        }

        #endregion

        void context_BeginRequest(object sender, EventArgs e)
        {
            // eat the cookie (if any) and set the culture
            if (HttpContext.Current.Request.Cookies["ck_lang"] != null)
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies["ck_lang"];
                string lang = cookie.Value;

                var culture = new System.Globalization.CultureInfo(lang);
                System.Threading.Thread.CurrentThread.CurrentCulture = culture;
                System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
            }
        }

    }
}
