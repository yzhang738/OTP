using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Configuration;
using OTP.Ring.Web.Utility;
using OTP.Ring.Business;
using OTP.Ring.Common;
using OTP.Ring.Models.ViewModel;

namespace OTP.Ring.Web.Controllers
{
    [HandleError]
    public class HomeController : BaseController 
    {
        private IDecodeBL _decodeBL;
        private IUserBL _userBL;
        private ISportBL _sportBL;

        public HomeController(IDecodeBL decodeBL, IUserBL userBL, ISportBL sportBL)
            : base(decodeBL, userBL, sportBL)
        {
            this._decodeBL = decodeBL;
            this._userBL = userBL;
            this._sportBL = sportBL;
        }

        public ActionResult Index()
        {
            string currentPageId = Constant.PageId.HomeIndex;
            Logger.LogMessage(string.Format("Entering Controller/Action PageId {0}", currentPageId));

            SetCurrentUser();

            bool validUserOnPage = this.ValidateUserOnPage(currentPageId);

            var model = new HomeIndexViewModel();
            model.Menu = this.Menu(currentPageId);
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();
            model.SummaryPageList = this.GetSummaryPageList(currentPageId);
            model.HasAccess = !(model.SummaryPageList == null || model.SummaryPageList.Count == 0);

            return View(model);
        }

        public ActionResult SignIn()
        {
            string currentPageId = "signin";
            Logger.LogMessage(string.Format("Entering Controller/Action PageId {0}", currentPageId));

            SetCurrentUser();

            var model = new HomeSignInViewModel();
            model.Menu = this.Menu(currentPageId);
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();
            model.AppId = ExtensionMethods.GetConfigurationSetting("wll_appid");

            return View(model);
        }

        public ActionResult NoScript()
        {
            string currentPageId = Constant.PageId.NoScript;
            Logger.LogMessage(string.Format("Entering Controller/Action PageId {0}", currentPageId));

            var model = new HomeIndexViewModel();
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();

            return View(model);
        }

        public ActionResult Browser()
        {
            string currentPageId = Constant.PageId.NoScript;
            Logger.LogMessage(string.Format("Entering Controller/Action PageId {0}", currentPageId));

            var model = new HomeIndexViewModel();
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();

            return View(model);
        }
    }
}
