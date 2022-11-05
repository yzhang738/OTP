using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OTP.Ring.Common;
using OTP.Ring.Web.Utility;
using OTP.Ring.Models.ViewModel;
using OTP.Ring.Business;
using Telerik.Web.Mvc;

namespace OTP.Ring.Web.Controllers
{
    public class ReportingController : BaseController
    {
        private IReportBL _reportBL;
        private IDecodeBL _decodeBL;
        private IUserBL _userBL;
        private ISportBL _sportBL;

        public ReportingController(IReportBL reportBL, IDecodeBL decodeBL, IUserBL userBL, ISportBL sportBL)
            : base(decodeBL, userBL, sportBL)
        {
            this._reportBL = reportBL;
            this._decodeBL = decodeBL;
            this._userBL = userBL;
            this._sportBL = sportBL;
        }

        public ActionResult Index()
        {
            string currentPageId = Constant.PageId.ReportingIndex;
            Logger.LogMessage(string.Format("Entering Controller/Action PageId {0}", currentPageId));

            bool validUserOnPage = this.ValidateUserOnPage(currentPageId);
            if (!validUserOnPage)
            {
                return this.RedirectToUserHomePage();
            }

            var model = new ReportingIndexViewModel();
            model.Menu = this.Menu(currentPageId);
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();
            model.SummaryPageList = this.GetSummaryPageList(currentPageId);

            return View(model);
        }

        #region Design Analysis

        public ActionResult DesignAnalysis(string id)
        {
            string currentPageId = Constant.PageId.ReportingDesignAnalysis;
            Logger.LogMessage(string.Format("Entering Controller/Action PageId {0}", currentPageId));

            bool validUserOnPage = this.ValidateUserOnPage(currentPageId);
            
            if (!validUserOnPage)
            {
                return this.RedirectToUserHomePage();
            }

            var model = new ReportingDesignAnalysisViewModel();
            model.Menu = this.Menu(currentPageId);
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();
            model.ReportList = this._reportBL.GetReportList(Constant.ReportType.AdHocReporting, CurrentUser.GroupProfileId);

            model.Editable = (model.Menu.Menu.WriteAccess != Constant.AccessLevel.No);
            
            this._reportBL.PopulateDesignAnalysisViewData(ViewData, id);
            ViewData["securityUserId"] = CurrentUser.Id;
            ViewData["securityUserProfileId"] = CurrentUser.UserProfileId;

            return View(model);
        }

        [GridAction]
        public ActionResult DesignAnalysisReportGridAjax()
        {
            //Set sport editable/readable based on permission
            string currentPageId = Constant.PageId.ReportingDesignAnalysis;
            var menu = this.Menu(currentPageId).Menu;

            //set readable
            List<RingMenuItem> appliedSports = new List<RingMenuItem>();
            if (menu.ReadAccess == Constant.AccessLevel.Assigned)
            {
                appliedSports = menu.FlatSportMenuItems;
            }
            else if (menu.ReadAccess == Constant.AccessLevel.All)
            {
                appliedSports = new Common.MenuHierarchy(this.AllSports, new List<Data.Models.Sport> { new Data.Models.Sport { SportID = Constant.SportId.All } }, null, null, this.CurrentLanguageCode(), false).FlatSportMenuItems;
            }

            var model = this._reportBL.GetReportingDesignAnalysisReportGridViewModel(this.CurrentUser.UserProfileId, appliedSports);

            return View(new GridModel(model));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertReport(string id)
        {
            var updatedModel = new ReportingDesignAnalysisReportGridViewModel();

            if (TryUpdateModel(updatedModel))
            {
                int changes = this._reportBL.AddAdhocReport(
                    updatedModel.Name_EN,
                    updatedModel.Name_FR,
                    updatedModel.Description_EN,
                    updatedModel.Description_FR,
                    updatedModel.Locked,
                    updatedModel.SportId,
                    null,
                    updatedModel.ReportCategoryId,
                    updatedModel.SecurityUserSport,
                    updatedModel.ReportViewId,
                    this.CurrentUser.UserProfileId);
            }

            return DesignAnalysisReportGridAjax();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult UpdateReport(string id)
        {
            var updatedModel = new ReportingDesignAnalysisReportGridViewModel();
            updatedModel.ReportId = int.Parse(id);

            if (TryUpdateModel(updatedModel))
            {
                bool canUpdate = true;

                if (!updatedModel.Locked)
                {
                    string message = "";
                    var canUnlockReport = this._reportBL.CanUnlockReport(updatedModel.ReportId, out message);

                    if (!canUnlockReport)
                    {
                        canUpdate = false;
                        ModelState.AddValidationMessage("Locked", message);
                    }
                }

                if (canUpdate)
                {
                    int changes = this._reportBL.UpdateAdhocReport(
                        updatedModel.ReportId,
                        updatedModel.Name_EN,
                        updatedModel.Name_FR,
                        updatedModel.Description_EN,
                        updatedModel.Description_FR,
                        updatedModel.Locked,
                        updatedModel.SportId,
                        null,
                        updatedModel.ReportCategoryId,
                        updatedModel.SecurityUserSport);
                }
            }

            return DesignAnalysisReportGridAjax();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult DeleteReport(string id)
        {
            int key = this._reportBL.DeleteAdhocReport(int.Parse(id));

            return DesignAnalysisReportGridAjax();
        }

        public ActionResult CopyReport(string copyReportId)
        {
            int key = this._reportBL.CopyReport(int.Parse(copyReportId));
            return RedirectToAction("DesignAnalysis", "Reporting", new { id = key });
        }

        #region Grouping Info

        public ActionResult DesignAnalysisGroupingInfo(string reportId, bool editable)
        {
            this._reportBL.PopulateDesignAnalysisViewData(ViewData, reportId);

            var model = new ReportingEditableViewModel { Editable = editable };
            return PartialView(model);
        }

        [GridAction]
        public ActionResult DesignAnalysisGroupingGridAjax(string reportId)
        {
            List<ReportingDesignAnalysisGroupingsGridViewModel> model = null;

            if (!string.IsNullOrEmpty(reportId))
            {
                model = this._reportBL.GetReportingDesignAnalysisGroupingsGridViewModel(int.Parse(reportId), this.CurrentUser.UserProfileId);
            }

            return View(new GridModel(model));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertReportGroup(string id)
        {
            var updatedModel = new ReportingDesignAnalysisGroupingsGridViewModel();
            updatedModel.ReportId = int.Parse(id);

            if (TryUpdateModel(updatedModel))
            {
                int changes = this._reportBL.AddReportGroup(
                    updatedModel.ReportId,
                    updatedModel.FieldName_EN,
                    updatedModel.FieldName_FR,
                    null);
            }

            return DesignAnalysisGroupingGridAjax(id);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult UpdateReportGroup(string id, string reportId)
        {
            var updatedModel = new ReportingDesignAnalysisGroupingsGridViewModel();
            updatedModel.ReportGroupId = int.Parse(id);

            if (TryUpdateModel(updatedModel))
            {
                int changes = this._reportBL.UpdateReportGroup(
                    updatedModel.ReportGroupId,
                    updatedModel.FieldName_EN,
                    updatedModel.FieldName_FR);
            }

            return DesignAnalysisGroupingGridAjax(reportId);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult DeleteReportGroup(string id, string reportId)
        {
            bool isValidForDeleting = this._reportBL.IsReportGroupValidForDeleting(int.Parse(id));
            if (!isValidForDeleting)
            {
                throw new Exception("Only the last Report Group Level can be deleted");
            }
            else
            {
                int key = this._reportBL.DeleteReportGroup(int.Parse(id));
            }

            return DesignAnalysisGroupingGridAjax(reportId);
        }

        [GridAction]
        public ActionResult DesignAnalysisGroupColumnsGridAjax(string id, string reportGroupId)
        {
            List<ReportingDesignAnalysisGroupingColumnsGridViewModel> model = null;

            if (!string.IsNullOrEmpty(reportGroupId))
            {
                model = this._reportBL.GetReportingDesignAnalysisGroupingColumnsGridViewModel(int.Parse(reportGroupId), this.CurrentUser.UserProfileId);
            }

            return View(new GridModel(model));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertReportColumn(string id, string reportGroupId)
        {
            var updatedModel = new ReportingDesignAnalysisGroupingColumnsGridViewModel();
            updatedModel.ReportGroupId = int.Parse(reportGroupId);

            if (TryUpdateModel(updatedModel))
            {
                var key = this._reportBL.AddReportColumn(
                    updatedModel.ReportGroupId,
                    updatedModel.ColumnDisplayName_EN,
                    updatedModel.ColumnDisplayName_FR,
                    updatedModel.ColumnFieldName_EN,
                    updatedModel.ColumnFieldName_FR,
                    updatedModel.ReportAggregation,
                    updatedModel.FieldSize,
                    null,
                    updatedModel.FormatString);
            }

            return DesignAnalysisGroupColumnsGridAjax(id, reportGroupId);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult UpdateReportColumn(string id, string reportGroupId)
        {
            var updatedModel = new ReportingDesignAnalysisGroupingColumnsGridViewModel();
            updatedModel.ReportColumnId = int.Parse(id);
            updatedModel.ReportGroupId = int.Parse(reportGroupId);

            if (TryUpdateModel(updatedModel))
            {
                var key = this._reportBL.UpdateReportColumn(
                    updatedModel.ReportColumnId,
                    updatedModel.ColumnDisplayName_EN,
                    updatedModel.ColumnDisplayName_FR,
                    updatedModel.ColumnFieldName_EN,
                    updatedModel.ColumnFieldName_FR,
                    updatedModel.ReportAggregation,
                    updatedModel.FieldSize,
                    formatString: updatedModel.FormatString);
            }

            return DesignAnalysisGroupColumnsGridAjax(id, reportGroupId);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult DeleteReportColumn(string id, string reportGroupId)
        {
            var reportColumnId = int.Parse(id);
            int changes = this._reportBL.DeleteReportColumn(reportColumnId);

            return DesignAnalysisGroupColumnsGridAjax(id, reportGroupId);
        }

        #endregion

        #region Filter Info

        public ActionResult DesignAnalysisFiltersInfo(string reportId, bool editable)
        {
            this._reportBL.PopulateDesignAnalysisViewData(ViewData, reportId);

            var model = new ReportingEditableViewModel { Editable = editable };
            return PartialView(model);
        }

        [GridAction]
        public ActionResult DesignAnalysisFiltersGridAjax(string reportId)
        {
            List<ReportingDesignAnalysisFiltersGridViewModel> model = null;

            if (!string.IsNullOrEmpty(reportId))
            {
                model = this._reportBL.GetReportingDesignAnalysisFiltersGridViewModel(int.Parse(reportId), this.CurrentUser.UserProfileId);
            }

            return View(new GridModel(model));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertReportFilter(string id, string reportId)
        {
            var updatedModel = new ReportingDesignAnalysisFiltersGridViewModel();
            updatedModel.ReportId = int.Parse(reportId);

            if (TryUpdateModel(updatedModel))
            {
                if (!updatedModel.Display && string.IsNullOrEmpty(updatedModel.Default_EN))
                {
                    ModelState.AddValidationMessage("Default_EN", "A value for Default_EN is required if not displayed.");
                }
                else
                {
                    int changes = this._reportBL.AddReportFilter(
                        updatedModel.ReportId,
                        updatedModel.FilterName_EN,
                        updatedModel.FilterName_FR,
                        updatedModel.FilterFieldName_EN,
                        updatedModel.FilterFieldName_FR,
                        updatedModel.Display,
                        updatedModel.Default_EN,
                        updatedModel.ReportOperand,
                        null);
                }
            }

            return DesignAnalysisFiltersGridAjax(reportId);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult UpdateReportFilter(string id, string reportId)
        {
            var updatedModel = new ReportingDesignAnalysisFiltersGridViewModel();
            updatedModel.ReportFilterId = int.Parse(id);

            if (TryUpdateModel(updatedModel))
            {
                if (!updatedModel.Display && string.IsNullOrEmpty(updatedModel.Default_EN))
                {
                    ModelState.AddValidationMessage("Default_EN", "A value for Default_EN is required if not displayed.");
                }
                else
                {
                    int changes = this._reportBL.UpdateReportFilter(
                        updatedModel.ReportFilterId,
                        updatedModel.FilterName_EN,
                        updatedModel.FilterName_FR,
                        updatedModel.FilterFieldName_EN,
                        updatedModel.FilterFieldName_FR,
                        updatedModel.Display,
                        updatedModel.Default_EN,
                        updatedModel.ReportOperand);
                }
            }

            return DesignAnalysisFiltersGridAjax(reportId);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult DeleteReportFilter(string id, string reportId)
        {
            int key = this._reportBL.DeleteReportFilter(int.Parse(id));

            return DesignAnalysisFiltersGridAjax(reportId);
        }

        #endregion

        #region Security Info

        public ActionResult DesignAnalysisSecurityInfo(string reportId, bool editable)
        {
            this._reportBL.PopulateDesignAnalysisViewData(ViewData, reportId);
            ViewData["securityUserProfileId"] = CurrentUser.UserProfileId;

            var model = new ReportingEditableViewModel { Editable = editable };
            return PartialView(model);
        }

        [GridAction]
        public ActionResult DesignAnalysisSecurityGridAjax(string reportId)
        {
            List<ReportingDesignAnalysisSecurityGridViewModel> model = null;

            if (!string.IsNullOrEmpty(reportId))
            {
                model = this._reportBL.GetReportingDesignAnalysisSecurityGridViewModel(int.Parse(reportId), this.CurrentUser.UserProfileId);
            }

            return View(new GridModel(model));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertReportSecurityGroupProfile(string id, string reportId)
        {
            var updatedModel = new ReportingDesignAnalysisSecurityGridViewModel();
            updatedModel.ReportId = int.Parse(reportId);

            if (TryUpdateModel(updatedModel))
            {
                int changes = this._reportBL.AddReportSecurityGroupProfile(
                    updatedModel.ReportId,
                    updatedModel.SecurityGroupProfileID,
                    null);
            }

            return DesignAnalysisSecurityGridAjax(reportId);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult DeleteReportSecurityGroupProfile(string id, string reportId)
        {
            int key = this._reportBL.DeleteReportSecurityGroupProfile(int.Parse(id));

            return DesignAnalysisSecurityGridAjax(reportId);
        }

        #endregion

        #region Test Report View

        public ActionResult DesignAnalysisTestReportView(string reportId)
        {
            this._reportBL.PopulateDesignAnalysisViewData(ViewData, reportId);
            var model = new ReportingDesignAnalysisTestReportViewModel();
            ViewData["securityUserId"] = CurrentUser.Id;

            if (!string.IsNullOrEmpty(reportId))
            {
                model.ReportList = this._reportBL.GetReportList(int.Parse(reportId));
            }

            return PartialView(model);
        }

        #endregion

        #endregion Design Analysis

        #region Standard Reports

        public ActionResult StandardReports(string id)
        {
            string currentPageId = Constant.PageId.ReportingStandardReports;
            Logger.LogMessage(string.Format("Entering Controller/Action PageId {0}", currentPageId));

            bool validUserOnPage = this.ValidateUserOnPage(currentPageId);
            if (!validUserOnPage)
            {
                return this.RedirectToUserHomePage();
            }

            var model = this._reportBL.PopulateReportingStandardReportsViewModel(id, CurrentUser.GroupProfileId);
            model.Menu = this.Menu(currentPageId);
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();
            model.CurrentOlympicYears = this.OlympicYears();

            ViewData["securityUserId"] = CurrentUser.Id;

            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult ReportParameters(string reportId)
        {
            ReportingReportParametersViewModel model = new ReportingReportParametersViewModel();
            if (!string.IsNullOrEmpty(reportId))
            {
                model = this._reportBL.GetReportingReportParametersViewModel(int.Parse(reportId));
            }
            ViewData["securityUserId"] = CurrentUser.Id;
            return Json(RenderPartialViewToString("ReportParameters", model), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetReportDescription(string reportId)
        {
            var description = "";

            if (!string.IsNullOrEmpty(reportId))
            {
                description = this._reportBL.GetReportingReportDescription(int.Parse(reportId));
            }
            ViewData["securityUserId"] = CurrentUser.Id;
            return Json(description, JsonRequestBehavior.AllowGet);
        }

        #endregion Standard Reports
    }
}
