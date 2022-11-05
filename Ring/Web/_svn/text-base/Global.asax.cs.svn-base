using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using OTP.Ring.Common;

namespace OTP.Ring.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.IgnoreRoute("Views/Reporting/ReportViewer.aspx");
            //routes.MapPageRoute(
            //     "ReportRoute",                         // Route name
            //     "Reports",                // URL
            //     "~/Reports/ReportViewer.aspx"   // File
            //     );


            routes.MapRoute(
                "authentication",
                "authentication",
                new { controller = "authentication", action = "handle" }
            );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);

            ControllerBuilder.Current.SetControllerFactory(new Utility.NinjectControllerFactory());
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var application = sender as HttpApplication;
            if (application != null)
            {
                var request = application.Request;
                
                //Fix for Report Viewer in non-ie browsers
                if (request.Path.EndsWith("Reserved.ReportViewerWebControl.axd") &&
                    request.QueryString["ResourceStreamID"] != null &&
                    request.QueryString["ResourceStreamID"].ToLower().Contains("blank.gif"))
                {
                    Response.Redirect(request.ApplicationPath + "Content/Images/blank.gif");
                }
                else if (!request.Path.Contains("Home/Browser")
                    && !IsValidBrowser(request.ServerVariables["HTTP_USER_AGENT"]))
                {
                    //Invalid browser type
                    Response.Redirect("~/Home/Browser");
                }
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            Logger.LogError(exception.ToString());
        }

        /// <summary>
        /// Based on Telerik Supported Browser Types
        /// </summary>
        /// <param name="httpUserAgent"></param>
        /// <returns></returns>
        private bool IsValidBrowser(string httpUserAgent)
        {
            if (string.IsNullOrEmpty(httpUserAgent))
            {
                return true;
            }

            //Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US) AppleWebKit/525.13 (KHTML, like Gecko) Chrome/0.X.Y.Z Safari/525.13.
            if (httpUserAgent.Contains("Gecko"))
            {
                if (httpUserAgent.Contains("Netscape"))
                {
                    //Netscape
                    return false;
                }
                else if (httpUserAgent.Contains("Firefox"))
                {
                    //Firefox
                    return true;
                }
                else if (httpUserAgent.Contains("Chrome"))
                {
                    //Chrome
                    return true;
                }
                else if (httpUserAgent.Contains("Safari"))
                {
                    //Safari
                    return true;
                }
                else
                {
                    //Mozilla
                    return false;
                }
            }
            else if (httpUserAgent.Contains("MSIE"))
            {
                if (httpUserAgent.Contains("Opera"))
                {
                    //Opera
                    return true;
                }
                else
                {
                    //Internet Explorer
                    return true;
                }
            }
            else
            {
                //All Other browsers
                return false;
            }
        }
    }
}