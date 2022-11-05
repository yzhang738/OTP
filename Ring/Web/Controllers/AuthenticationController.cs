using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

using OTP.Ring.Web.Utility;
using OTP.Ring.Business;

namespace OTP.Ring.Web.Controllers
{
    public class AuthenticationController : BaseController
    {
        private IDecodeBL _decodeBL;
        private IUserBL _userBL;
        private ISportBL _sportBL;

        public AuthenticationController(IDecodeBL decodeBL, IUserBL userBL, ISportBL sportBL)
            : base(decodeBL, userBL, sportBL)
        {
            this._decodeBL = decodeBL;
            this._userBL = userBL;
            this._sportBL = sportBL;
        }

        public ActionResult Handle()
        {
            string action = Request.Params["action"];

            var wll = new WindowsLiveLogin(true);

            
            if (action == "logout")
            {
                HttpCookie loginCookie = new HttpCookie("webauthtoken");
                loginCookie.Expires = DateTime.Now.AddYears(-10);
                Response.Cookies.Add(loginCookie);

                return this.RedirectToUserHomePage();
            }
            else if (action == "clearcookie")
            {
                HttpCookie loginCookie = new HttpCookie("webauthtoken");
                loginCookie.Expires = DateTime.Now.AddYears(-10);
                Response.Cookies.Add(loginCookie);

                string type;
                byte[] content;
                wll.GetClearCookieResponse(out type, out content);
                return new FileContentResult(content, type);
            }
            else
            {
                WindowsLiveLogin.User user = wll.ProcessLogin(Request.Form);
                HttpCookie loginCookie = new HttpCookie("webauthtoken");
                if (user != null)
                {
                    loginCookie.Value = user.Token;

                    if (user.UsePersistentCookie)
                    {
                        loginCookie.Expires = DateTime.Now.AddYears(10);
                    }
                    //else
                    //{
                    //    loginCookie.Expires = DateTime.Now.AddDays(1);
                    //}
                }
                else
                {
                    loginCookie.Expires = DateTime.Now.AddYears(-10);
                }
                Response.Cookies.Add(loginCookie);

                if (user != null)
                {
                    var otpUser = this._userBL.GetUserByLiveId(user.Id);
                    if (otpUser.LiveId.ToUpper() == "GUEST")
                    {
                        return RedirectToAction("WhoAmI", "Maintenance");
                    }
                }

                return this.RedirectToUserHomePage();
            }
        }
    }
}