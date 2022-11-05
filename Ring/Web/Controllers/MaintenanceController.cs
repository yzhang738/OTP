﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using OTP.Ring.Common;
using OTP.Ring.Business;
using OTP.Ring.Web.Utility;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.UI;
using System.Web.Routing;
using Telerik.Web.Mvc.Extensions;
using System.Threading;
using OTP.Ring.Models.ViewModel;
using OTP.Ring.Models;
using System.Globalization;
using NPOI.HSSF.UserModel;

namespace OTP.Ring.Web.Controllers
{
    public class MaintenanceController : BaseController
    {
        private IResourceBL _resourceBL;
        private IBenchmarkBL _benchmarkBL;
        private IDecodeBL _decodeBL;
        private IUserBL _userBL;
        private ISportBL _sportBL;

        public MaintenanceController(IResourceBL resourceBL, IBenchmarkBL benchmarkBL, IDecodeBL decodeBL, IUserBL userBL, ISportBL sportBL)
            : base(decodeBL, userBL, sportBL)
        {
            this._resourceBL = resourceBL;
            this._benchmarkBL = benchmarkBL;
            this._decodeBL = decodeBL;
            this._userBL = userBL;
            this._sportBL = sportBL;
        }

        public ActionResult Index()
        {
            return ValidateUserAndInitIndexView(Constant.PageId.MaintenanceIndex);
        }

        public ActionResult Resources()
        {
            return ValidateUserAndInitIndexView(Constant.PageId.MaintenanceResources);
        }

        public ActionResult Sports()
        {
            return ValidateUserAndInitIndexView(Constant.PageId.MaintenanceSports);
        }

        public ActionResult Other()
        {
            return ValidateUserAndInitIndexView(Constant.PageId.MaintenanceOther);
        }

        private ActionResult ValidateUserAndInitIndexView(string currentPageId)
        {
            Logger.LogMessage(string.Format("Entering Controller/Action PageId {0}", currentPageId));

            bool validUserOnPage = this.ValidateUserOnPage(currentPageId);

            if (!validUserOnPage)
            {
                return this.RedirectToUserHomePage();
            }

            var model = new MaintenanceIndexViewModel();
            model.Menu = this.Menu(currentPageId);
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();
            model.SummaryPageList = this.GetSummaryPageList(currentPageId);

            return View(model);
        }

        #region AthletesMaintenance

        public ActionResult Athletes(string id, GridCommand command)
        {
            string currentPageId = Constant.PageId.MaintenanceAthletes;
            Logger.LogMessage(string.Format("Entering Controller/Action PageId {0}", currentPageId));

            bool validUserOnPage = this.ValidateUserOnPage(currentPageId);
            if (!validUserOnPage)
            {
                return this.RedirectToUserHomePage();
            }

            string resourceType = this._resourceBL.GetResourceType(id);

            if (resourceType == null)
            {
                id = null;
            }
            else if (resourceType == Constant.ResourceType.NSOMembers)
            {
                return RedirectToAction("NSOMembers", "Maintenance", new { id = id });
            }
            else if (resourceType == Constant.ResourceType.ISTMembers)
            {
                return RedirectToAction("ISTMembers", "Maintenance", new { id = id });
            }

            if (!string.IsNullOrEmpty(Request.Params.Get("size")))
            {
                command = GridCommand.Parse(
                    int.Parse(Request.Params.Get("page")),
                    int.Parse(Request.Params.Get("size")),
                    Request.Params.Get("orderBy"),
                    string.Empty,
                    Request.Params.Get("filter"));
            }

            var model = new MaintenanceAthletesViewModel();
            model.Menu = this.Menu(currentPageId);
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();
            model.CurrentOlympicYears = this.OlympicYears();
            model.CurrentSport = this.SelectedSport();
            model.CurrentYear = this.SelectedYear();

            model.Editable = (model.Menu.Menu.WriteAccess != Constant.AccessLevel.No);

            this._resourceBL.PopulateMaintenanceResourcesViewData(ViewData, id, this.SelectedSport());
            ViewData["athletesCurrentPage"] = command.Page;

            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertAthlete(GridCommand command, string id)
        {
            var updatedModel = new MaintenanceAthletesAthleteGridViewModel();

            if (TryUpdateModel(updatedModel))
            {
                if (updatedModel.BirthDate != null)
                {
                    updatedModel.DateOfBirth = DateTime.Parse(updatedModel.BirthDate);
                }
                updatedModel.CountryId = updatedModel.CountryName;
                updatedModel.GenderId = updatedModel.GenderName;
                updatedModel.PersonalCoachId = Convert.ToInt32(updatedModel.PersonalCoachName);

                int resourceId = this._resourceBL.AddResource(
                    Constant.ResourceType.Athletes,
                    updatedModel.FirstName,
                    updatedModel.LastName,
                    updatedModel.CountryId,
                    updatedModel.DateOfBirth,
                    updatedModel.TBD,
                    updatedModel.Pooled,
                    true,
                    resourceSpecialtyId: updatedModel.ResourceSpecialtyId,
                    gender: updatedModel.GenderId,
                    isNew: true,
                    assignSportId: updatedModel.AssignSportId,
                    yearId: this.SelectedYear(),
                    personalCoachId: updatedModel.PersonalCoachId);
            }

            return AthletesMaintenanceAthleteGridAjax(command, id);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult UpdateAthlete(GridCommand command, string id)
        {
            var updatedModel = new MaintenanceAthletesAthleteGridViewModel();
            updatedModel.ResourceId = int.Parse(id);

            if (TryUpdateModel(updatedModel))
            {
                if (updatedModel.BirthDate != null)
                {
                    updatedModel.DateOfBirth = DateTime.Parse(updatedModel.BirthDate);
                }
                updatedModel.CountryId = updatedModel.CountryName;
                updatedModel.GenderId = updatedModel.GenderName;
                updatedModel.PersonalCoachId = Convert.ToInt32(updatedModel.PersonalCoachName);

                int resourceId = this._resourceBL.UpdateResource(
                    updatedModel.ResourceId,
                    resourceTypeId : Constant.ResourceType.Athletes,
                    resourceSpecialtyId: updatedModel.ResourceSpecialtyId,
                    firstName : updatedModel.FirstName,
                    lastName : updatedModel.LastName,
                    countryId : updatedModel.CountryId,
                    gender: updatedModel.GenderId,
                    dateOfBirth: updatedModel.DateOfBirth,
                    tbd : updatedModel.TBD,
                    pooled : updatedModel.Pooled,
                    isNew: updatedModel.IsNew,
                    assignSportId: Constant.SportId.All,
                    yearId: this.SelectedYear(),
                    personalCoachId: updatedModel.PersonalCoachId);
            }

            return AthletesMaintenanceAthleteGridAjax(command, id);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ShouldPrompResourceExists(string resourceTypeId, string firstName, string lastName, string countryId)
        {
            var prompt = this._resourceBL.ShouldPrompResourceExists(resourceTypeId, firstName, lastName, countryId);
            return Json(prompt);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult DeleteAthlete(GridCommand command, string id)
        {
            int key = this._resourceBL.UpdateResource(
               Convert.ToInt32(id),
               active: false);
            
            //this._resourceBL.DeleteReource(Convert.ToInt32(id));

            return AthletesMaintenanceAthleteGridAjax(command, id);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult AthletesMaintenanceAthleteGridAjax(GridCommand command, string id)
        {
            int total = 0;
            var model = this._resourceBL.GetMaintenanceAthletesAthleteGridViewModel(this.SelectedSport(), this.SelectedYear(), command, out total);

            return View(new GridModel
                {
                    Data = model,
                    Total = total
                });
        }

        #region General Tab

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AthletesGeneralInfo(string id, bool editable)
        {
            this._resourceBL.PopulateMaintenanceResourcesTabsExceptResultViewData(ViewData, id);
            MaintenanceAthletesGeneralInfoViewModel model = null;
            if (!string.IsNullOrEmpty(id))
            {
                model = this._resourceBL.GetMaintenanceAthletesGeneralInfoViewModel(int.Parse(id));
                model.Editable = editable;
            }

            return PartialView(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AthletesGeneralInfoSave(string id, FormCollection collection)
        {
            var updatedModel = new MaintenanceAthletesGeneralInfoViewModel();
            updatedModel.ResourceId = int.Parse(id);

            if (TryUpdateModel(updatedModel, null, null, new[] { "Countries", "CSCs", "Genders", "CoachResources" }))
            {
                this._resourceBL.UpdateResource(
                    updatedModel.ResourceId,
                    firstName: updatedModel.FirstName,
                    lastName: updatedModel.LastName,
                    countryId: updatedModel.CountryId,
                    gender: updatedModel.Gender,
                    dateOfBirth: updatedModel.DateOfBirth,
                    primaryCSCId: updatedModel.PrimaryCSCId,
                    secondaryCSCId: updatedModel.SecondaryCSCId,
                    comments: updatedModel.Comments,
                    isNew: updatedModel.IsNew);

                //Add Athlete Personal Coach, SportID == Constant.SportId.All in ResourceSports is used as Athlete personal coach
                int changes = this._resourceBL.SavePersonalCoach(updatedModel.ResourceId, updatedModel.PersonalCoachId, this.SelectedYear());

                return RedirectToAction("Athletes", new { id = id });
            }

            var currentPageId = Constant.PageId.MaintenanceAthletes;

            var model = new MaintenanceAthletesViewModel();
            model.Menu = this.Menu(currentPageId);
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();

            ViewData["resourceId"] = id;

            return View("Athletes", model);
        }

        #endregion General Tab

        #region ContactInfoTab

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AthletesContactInfo(string id, bool editable)
        {
            this._resourceBL.PopulateMaintenanceResourcesTabsExceptResultViewData(ViewData, id);
            MaintenanceAthletesContactInfoViewModel model = null;
            if (!string.IsNullOrEmpty(id))
            {
                model = this._resourceBL.GetMaintenanceAthletesContactInfoViewModel(int.Parse(id));
                model.Editable = editable;
            }

            return PartialView(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AthletesContactInfoSave(string id, FormCollection collection)
        {
            var updatedModel = new MaintenanceAthletesContactInfoViewModel();
            updatedModel.ResourceId = int.Parse(id);

            if (TryUpdateModel(updatedModel, null, null, new[] { "Countries", "Provinces" }))
            {
                this._resourceBL.UpdateResource(
                    updatedModel.ResourceId,
                    emailAddress: updatedModel.EMailAddress,
                    skypeName: updatedModel.SkypeName,
                    hometownCity: updatedModel.HometownCity,
                    hometownProvinceId: updatedModel.HometownProvinceId,
                    hometownCountryId: updatedModel.HometownCountryId,
                    mailingAddress: updatedModel.MailingAddress,
                    mailingCity: updatedModel.MailingCity,
                    mailingProvinceId: updatedModel.MailingProvinceId,
                    mailingCountryId: updatedModel.MailingCountryId,
                    mailingPostalCode: updatedModel.MailingPostalCode,
                    homePhone: updatedModel.HomePhone,
                    cellPhone: updatedModel.CellPhone);

                return RedirectToAction("Athletes", new { id = id });
            }

            string currentPageId = Constant.PageId.MaintenanceAthletes;

            var model = new MaintenanceAthletesViewModel();
            model.Menu = this.Menu(currentPageId);
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();

            ViewData["resourceId"] = id;

            return View("Athletes", model);
        }

        #endregion ContactInfoTab

        #region SportInfoTab

        public ActionResult AthletesSportsInfo(string id, bool editable)
        {
            this._resourceBL.PopulateMaintenanceResourcesTabsExceptResultViewData(ViewData, id);
            this._resourceBL.PopulateMaintenanceResourcesBenchmarkViewData(ViewData, id);

            MaintenanceEditableViewModel model = new MaintenanceEditableViewModel { Editable = editable };
            model.CurrentOlympicYears = this.OlympicYears();

            return PartialView(model);
        }

        [GridAction]
        public ActionResult AthletesSportsInfoAjax(string id)
        {
            List<MaintenanceAthletesSportsInfoViewModel> model = null;

            if (!string.IsNullOrEmpty(id))
            {
                model = this._resourceBL.GetMaintenanceAthletesSportsInfoViewModel(int.Parse(id), this.SelectedYear());
            }

            return View(new GridModel(model));
        }

        [GridAction]
        public ActionResult AthletesBenchmarkSportsAjax(string id, string resourceSportId)
        {
            List<MaintenanceAthletesBenchmarkSportsViewModel> model = null;

            if (!string.IsNullOrEmpty(id))
            {
                model = this._benchmarkBL.GetMaintenanceAthletesBenchmarkSportsViewModel(this.SelectedYear(), resourceSportId);
            }

            return View(new GridModel(model));
        }

        [HttpPost]
        public ActionResult AthletesBenchmarkSportsBenchmarkTierAjax(string text, string sportId, FormCollection collection)
        {
            var benchmarkTiers = this._benchmarkBL.GetBenchmarkTiersBySportId(sportId);
            //if (text.HasValue())
            //{
            //    products = products.Where((p) => p.ProductName.StartsWith(text));
            //}

            return new JsonResult
            {
                Data = new SelectList(benchmarkTiers, "Id", "Description")
            };
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertAthletesBenchmark(string id, string resourceSportId)
        {
            var updatedModel = new MaintenanceAthletesBenchmarkSportsViewModel();
            updatedModel.ResourceSportId = int.Parse(resourceSportId);
            updatedModel.YearId = this.SelectedYear();

            if (TryUpdateModel(updatedModel))
            {
                List<int> benchmarkResources = this._resourceBL.GetBenchmarkResourceIdsByResourceSportYearTypeTier(updatedModel.ResourceSportId, updatedModel.YearId, updatedModel.BenchmarkTypeId, updatedModel.BenchmarkTierId);
                if (benchmarkResources != null && benchmarkResources.Count != 0)
                {
                    throw new Exception(Shared.SharedStrings.Error_AddExistingBenchmark);
                }

                var key = this._resourceBL.AddBenchmarkResource(
                    updatedModel.ResourceSportId,
                    updatedModel.BenchmarkTypeId,
                    updatedModel.BenchmarkTierId,
                    updatedModel.YearId,
                    updatedModel.NSOValue,
                    updatedModel.OTPValue,
                    null,
                    true);
            }

            return AthletesBenchmarkSportsAjax(id, resourceSportId);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult UpdateAthletesBenchmark(string id, string resourceSportId)
        {
            var updatedModel = new MaintenanceAthletesBenchmarkSportsViewModel();
            updatedModel.BenchmarkResourceId = int.Parse(id);
            updatedModel.ResourceSportId = int.Parse(resourceSportId);

            if (TryUpdateModel(updatedModel))
            {
                List<int> benchmarkResources = this._resourceBL.GetBenchmarkResourceIdsByResourceSportYearTypeTier(updatedModel.ResourceSportId, this.SelectedYear(), updatedModel.BenchmarkTypeId, updatedModel.BenchmarkTierId);
                if (benchmarkResources != null && benchmarkResources.Count != 0 && benchmarkResources.First() != updatedModel.BenchmarkResourceId)
                {
                    throw new Exception(Shared.SharedStrings.Error_UpdateExistingBenchmark);
                }

                var key = this._resourceBL.UpdateBenchmarkResource(
                    updatedModel.BenchmarkResourceId,
                    benchmarkTypeId: updatedModel.BenchmarkTypeId,
                    benchmarkTierId: updatedModel.BenchmarkTierId,
                    nsoValue: updatedModel.NSOValue,
                    otpValue: updatedModel.OTPValue);
            }

            return AthletesBenchmarkSportsAjax(id, resourceSportId);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult DeleteAthletesBenchmark(string id, string resourceSportId)
        {
            var benchmarkResourceId = int.Parse(id);
            int changes = this._resourceBL.DeleteBenchmarkResource(benchmarkResourceId);

            return AthletesBenchmarkSportsAjax(id, resourceSportId);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertAthletesSport(string id)
        {
            var updatedModel = new MaintenanceAthletesSportsInfoViewModel();
            //updatedModel.YearId = this.SelectedYear();

            if (TryUpdateModel(updatedModel))
            {
                updatedModel.ResourceId = int.Parse(id);
                updatedModel.SportsId = updatedModel.SportName;
                updatedModel.EventId = updatedModel.EventName;

                //validation
                List<int> resourceSports = this._resourceBL.GetResourceSportIdsByResourceSport(updatedModel.ResourceId, updatedModel.EventId, updatedModel.YearId);
                if (resourceSports != null && resourceSports.Count != 0)
                {
                    throw new Exception(Shared.SharedStrings.Error_SportAssociation);
                }

                var key = this._resourceBL.AddResourceSport(
                    updatedModel.ResourceId,
                    updatedModel.EventId,
                    updatedModel.YearId,
                    updatedModel.ResourceClassId,
                    null,
                    updatedModel.SportTrackingRef,
                    updatedModel.EventCoachId,
                    null,
                    null);
            }

            return AthletesSportsInfoAjax(id);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult UpdateAthletesSport(string id, string ResourceId)
        {
            var updatedModel = new MaintenanceAthletesSportsInfoViewModel();
            updatedModel.ResourceSportId = int.Parse(id);

            if (TryUpdateModel(updatedModel))
            {
                updatedModel.SportsId = updatedModel.SportName;
                updatedModel.EventId = updatedModel.EventName;

                //validation
                List<int> resourceSports = this._resourceBL.GetResourceSportIdsByResourceSport(int.Parse(ResourceId), updatedModel.EventId, updatedModel.YearId);
                if (resourceSports != null && resourceSports.Count != 0 && resourceSports.First()!= updatedModel.ResourceSportId)
                {
                    throw new Exception(Shared.SharedStrings.Error_SportAssociation);
                }

                var key = this._resourceBL.UpdateResourceSport(
                    updatedModel.ResourceSportId, 
                    sportId: updatedModel.EventId, 
                    yearId: updatedModel.YearId,
                    resourceClassId: updatedModel.ResourceClassId,
                    sportTrackingRef: updatedModel.SportTrackingRef,
                    coachResourceId: updatedModel.EventCoachId);
            }

            return AthletesSportsInfoAjax(ResourceId);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult DeleteAthletesSport(int id)
        {
            int resourceId = this._resourceBL.GetResourceIdByResourceSport(id);
            var key = this._resourceBL.DeleteResourceSport(id);
            return AthletesSportsInfoAjax(resourceId.ToString());
        }

        #endregion SportInfoTab

        #region DirectAtheteSupportTab

        public ActionResult AthletesDirectAthleteSupport(string id, bool editable)
        {
            this._resourceBL.PopulateMaintenanceResourcesTabsExceptResultViewData(ViewData, id);

            MaintenanceEditableViewModel model = new MaintenanceEditableViewModel { Editable = editable };
            return PartialView(model);
        }

        [GridAction]
        public ActionResult AthletesDirectAthleteSupportAjax(string id)
        {
            List<MaintenanceAthletesDirectAthleteSupportViewModel> model = null;

            if (!string.IsNullOrEmpty(id))
            {
                model = this._resourceBL.GetMaintenanceAthletesDirectAthleteSupportViewModel(int.Parse(id));
            }

            return View(new GridModel(model));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertAthletesDirectAthleteSupport(string id)
        {
            var updatedModel = new MaintenanceAthletesDirectAthleteSupportViewModel();

            if (TryUpdateModel(updatedModel))
            {
                updatedModel.ResourceId = int.Parse(id);

                //validation
                List<int> athleteDirectSupports = this._resourceBL.GetResourceDirectAthleteSupportIdsByResourceSupportYear(updatedModel.ResourceId, updatedModel.YearId, updatedModel.DirectAthleteSupportCode);
                if (athleteDirectSupports != null && athleteDirectSupports.Count != 0)
                {
                    throw new Exception(Shared.SharedStrings.Error_DASAssociation);
                }

                var key = this._resourceBL.AddResourceDirectAthleteSupport(
                    updatedModel.ResourceId,
                    updatedModel.DirectAthleteSupportCode,
                    updatedModel.YearId,
                    updatedModel.SupportName,
                    updatedModel.Description,
                    updatedModel.SupportContact,
                    updatedModel.SupportEmail,
                    updatedModel.Amount,
                    null);
            }

            return AthletesDirectAthleteSupportAjax(id);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult UpdateAthletesDirectAthleteSupport(string id, string ResourceId)
        {
            var updatedModel = new MaintenanceAthletesDirectAthleteSupportViewModel();

            if (TryUpdateModel(updatedModel))
            {
                //validation
                List<int> athleteDirectSupports = this._resourceBL.GetResourceDirectAthleteSupportIdsByResourceSupportYear(updatedModel.ResourceId, updatedModel.YearId, updatedModel.DirectAthleteSupportCode);
                if (athleteDirectSupports != null && athleteDirectSupports.Count != 0 && athleteDirectSupports.First() != updatedModel.ResourceDirectAthleteSupportId)
                {
                    throw new Exception(Shared.SharedStrings.Error_DASAssociation);
                }

                int key = this._resourceBL.UpdateResourceDirectAthleteSupport(
                    updatedModel.ResourceDirectAthleteSupportId,
                    directAthleteSupportId: updatedModel.DirectAthleteSupportCode,
                    yearId: updatedModel.YearId,
                    supportName: updatedModel.SupportName,
                    description: updatedModel.Description,
                    supportContact: updatedModel.SupportContact,
                    supportEmail: updatedModel.SupportEmail,
                    amount: updatedModel.Amount);
            }

            return AthletesDirectAthleteSupportAjax(ResourceId);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult DeleteAthletesDirectAthleteSupport(int id)
        {
            int resourceId = this._resourceBL.GetResourceIdByDirectAthleteSupport(id);

            int key = this._resourceBL.DeleteResourceDirectAthleteSupport(id);

            return AthletesDirectAthleteSupportAjax(resourceId.ToString());
        }

        #endregion DirectAtheteSupportTab

        #region AffiliatesInfoTab

        public ActionResult AthletesAffiliatesInfo(string id, bool editable)
        {
            this._resourceBL.PopulateMaintenanceResourcesTabsExceptResultViewData(ViewData, id);

            MaintenanceEditableViewModel model = new MaintenanceEditableViewModel { Editable = editable };
            return PartialView(model);
        }

        [GridAction]
        public ActionResult AthletesAffiliatesInfoAjax(string id)
        {
            List<MaintenanceAthletesAffiliatesInfoViewModel> model = null;

            if (!string.IsNullOrEmpty(id))
            {
                model = this._resourceBL.GetMaintenanceAthletesAffiliatesInfoViewModel(int.Parse(id));
            }

            return View(new GridModel(model));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertAthletesAffiliate(string id)
        {
            var updatedModel = new MaintenanceAthletesAffiliatesInfoViewModel();
            
            if (TryUpdateModel(updatedModel))
            {
                updatedModel.ResourceId = int.Parse(id);

                //validation
                List<int> athleteAffiliates = this._resourceBL.GetResourceAffiliateIdsByResourceTypeNameYear(updatedModel.ResourceId, updatedModel.AffiliateTypeId, updatedModel.AffiliateName, updatedModel.YearFromId);
                if (athleteAffiliates != null && athleteAffiliates.Count != 0)
                {
                    throw new Exception(Shared.SharedStrings.Error_AffiliateAssociation);
                }

                var key = this._resourceBL.AddResourceAffiliate(
                    updatedModel.AffiliateTypeId,
                    updatedModel.ResourceId,
                    updatedModel.YearFromId,
                    updatedModel.YearToId,
                    updatedModel.AffiliateName,
                    updatedModel.Description,
                    updatedModel.AffiliateContact,
                    updatedModel.AffiliateEmail,
                    null);
            }

            return AthletesAffiliatesInfoAjax(id);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult UpdateAthletesAffiliate(string id, string ResourceId)
        {
            var updatedModel = new MaintenanceAthletesAffiliatesInfoViewModel();

            if (TryUpdateModel(updatedModel))
            {
                //validation
                List<int> athleteAffiliates = this._resourceBL.GetResourceAffiliateIdsByResourceTypeNameYear(updatedModel.ResourceId, updatedModel.AffiliateTypeId, updatedModel.AffiliateName, updatedModel.YearFromId);
                if (athleteAffiliates != null && athleteAffiliates.Count != 0 && athleteAffiliates.First() != updatedModel.ResourceAffiliateId)
                {
                    throw new Exception(Shared.SharedStrings.Error_AffiliateAssociation);
                }

                int key = this._resourceBL.UpdateResourceAffiliate(
                    updatedModel.ResourceAffiliateId,
                    affiliateTypeId: updatedModel.AffiliateTypeId,
                    yearFromId: updatedModel.YearFromId,
                    yearToId: updatedModel.YearToId,
                    affiliateName: updatedModel.AffiliateName,
                    description: updatedModel.Description,
                    affiliateContact: updatedModel.AffiliateContact,
                    affiliateEmail: updatedModel.AffiliateEmail);
            }

            return AthletesAffiliatesInfoAjax(ResourceId);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult DeleteAthletesAffiliate(int id)
        {
            int resourceId = this._resourceBL.GetResourceIdByAthleteAffiliate(id);
            int key = this._resourceBL.DeleteResourceAffiliate(id);
            return AthletesAffiliatesInfoAjax(resourceId.ToString());
        }

        #endregion AffiliatesInfoTab

        #region ResultsTab

        public ActionResult AthletesResults(string id, bool editable)
        {
            this._resourceBL.PopulateMaintenanceResourcesResultTabViewData(ViewData, id);

            MaintenanceEditableViewModel model = new MaintenanceEditableViewModel { Editable = editable };
            model.CurrentLanguageCode = this.CurrentLanguageCode();

            return PartialView(model);
        }

        [GridAction]
        public ActionResult AthletesResultsAjax(string id)
        {
            List<MaintenanceAthletesResultsViewModel> model = null;

            if (!string.IsNullOrEmpty(id))
            {
                model = this._resourceBL.GetMaintenanceAthletesResultsViewModel(int.Parse(id));
            }

            return View(new GridModel(model));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertAthletesResults(string id)
        {
            var updatedModel = new MaintenanceAthletesResultsViewModel();

            if (TryUpdateModel(updatedModel))
            {
                updatedModel.ResourceID = int.Parse(id);

                updatedModel.CountryID = updatedModel.CountryName;
                updatedModel.SportID = updatedModel.SportName;
                updatedModel.DisciplineID = updatedModel.DisciplineName;
                updatedModel.EventID = updatedModel.EventName;

                // Check if competition already exists
                int competitionId = this._resourceBL.GetResourceResultsByCompetition(updatedModel.CompetitionName,                                                                                    
                                                                                    DateTime.Parse(updatedModel.CompetitionDate),
                                                                                    updatedModel.SportID,
                                                                                    updatedModel.Location,
                                                                                    updatedModel.CountryID);
                if (competitionId == 0)
                {
                    throw new Exception(Shared.SharedStrings.Error_CompetitonNotExist);
                }

                if (competitionId != 0)
                {
                    // Check if sport event already exists for the athlete
                    List<int> resourceResults = this._resourceBL.GetResourceResultsBySportEvent(updatedModel.ResourceID, updatedModel.EventID, competitionId);
                    if (resourceResults != null && resourceResults.Count != 0)
                    {
                        throw new Exception(Shared.SharedStrings.Error_AddExistingEvent);
                    }

                    TimeSpan? perftime = string.IsNullOrEmpty(updatedModel.PerfTime) ? (TimeSpan?)null : updatedModel.PerfTime.ConvertStringToTimeSpan();
                    decimal? perfvalue = (updatedModel.PerfValue == null) ? null : (decimal?)Convert.ToDecimal(updatedModel.PerfValue);

                    updatedModel.ResultID = this._resourceBL.AddResourceResults(updatedModel.ResourceID,
                                                                                competitionId,
                                                                                updatedModel.EventID,
                                                                                DateTime.Parse(updatedModel.EventDate),
                                                                                updatedModel.Position,
                                                                                perftime,
                                                                                perfvalue, 
                                                                                updatedModel.Comments);
                }
            }

            return AthletesResultsAjax(id);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult IncorrectPerfTime(string perfTime)
        {
            var result = perfTime.IsPerfTimeValid();
            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult IfCompetitionExists(string competitionName, string competitionDate, string sportId, string location, string countryId)
        {
            DateTime date = DateTime.Parse(competitionDate);
            int competitionId = this._resourceBL.GetResourceResultsByCompetition(competitionName, date, sportId, location, countryId);

            return Json(competitionId);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult IfResultExists(int resourceId, int competitionId, string eventName)
        {
            List<int> resourceResults = this._resourceBL.GetResourceResultsBySportEvent(resourceId, eventName, competitionId);

            if (resourceResults != null && resourceResults.Count != 0)
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult UpdateAthletesResults(string id)
        {
            var updatedModel = new MaintenanceAthletesResultsViewModel();

            int resultId = int.Parse(id);
            int resourceId = this._resourceBL.GetResourceIDByResult(resultId) ?? 0;

            if (TryUpdateModel(updatedModel))
            {
                updatedModel.ResultID = resultId;

                updatedModel.CountryID = updatedModel.CountryName;
                updatedModel.SportID = updatedModel.SportName;
                updatedModel.DisciplineID = updatedModel.DisciplineName;
                updatedModel.EventID = updatedModel.EventName;

                // Check if competition already exists
                int competitionId = this._resourceBL.GetResourceResultsByCompetition(updatedModel.CompetitionName,
                                                                                    DateTime.Parse(updatedModel.CompetitionDate),
                                                                                    updatedModel.SportID,
                                                                                    updatedModel.Location,
                                                                                    updatedModel.CountryID);
                if (competitionId == 0)
                {
                    throw new Exception(Shared.SharedStrings.Error_CompetitonNotExist);
                }

                TimeSpan? perftime = string.IsNullOrEmpty(updatedModel.PerfTime) ? (TimeSpan?)null : updatedModel.PerfTime.ConvertStringToTimeSpan();
                decimal? perfvalue = (updatedModel.PerfValue == null) ? null : (decimal?)Convert.ToDecimal(updatedModel.PerfValue);

                this._resourceBL.UpdateResourceResults(updatedModel.ResultID,
                                                        competitionId,
                                                        DateTime.Parse(updatedModel.EventDate),
                                                        updatedModel.EventID,
                                                        updatedModel.Position,
                                                        perftime,
                                                        perfvalue,
                                                        updatedModel.Comments);
           }

            return AthletesResultsAjax(resourceId.ToString());
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult DeleteAthletesResults(string id)
        {
            int resultId = int.Parse(id);

            int resourceId = this._resourceBL.GetResourceIDByResult(resultId) ?? 0;

            this._resourceBL.DeleteResourceResults(resultId);

            return AthletesResultsAjax(resourceId.ToString());
        }

        #endregion ResultsTab
        
        #endregion AthletesMaintenance


        #region ISTMembersMaintenance

        public ActionResult ISTMembers(string id, GridCommand command)
        {
            string currentPageId = Constant.PageId.MaintenanceISTMembers;
            Logger.LogMessage(string.Format("Entering Controller/Action PageId {0}", currentPageId));

            bool validUserOnPage = this.ValidateUserOnPage(currentPageId);
            if (!validUserOnPage)
            {
                return this.RedirectToUserHomePage();
            }

            string resourceType = this._resourceBL.GetResourceType(id);

            if (resourceType == null)
            {
                id = null;
            }
            else if (resourceType == Constant.ResourceType.Athletes)
            {
                return RedirectToAction("Athletes", "Maintenance", new { id = id });
            }
            else if (resourceType == Constant.ResourceType.NSOMembers)
            {
                return RedirectToAction("NSOMembers", "Maintenance", new { id = id });
            }

            if (!string.IsNullOrEmpty(Request.Params.Get("size")))
            {
                command = GridCommand.Parse(
                    int.Parse(Request.Params.Get("page")),
                    int.Parse(Request.Params.Get("size")),
                    Request.Params.Get("orderBy"),
                    string.Empty,
                    Request.Params.Get("filter"));
            }

            var model = new MaintenanceISTMembersViewModel();
            model.Menu = this.Menu(currentPageId);
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();
            model.CurrentOlympicYears = this.OlympicYears();
            model.CurrentSport = this.SelectedSport();

            model.Editable = (model.Menu.Menu.WriteAccess != Constant.AccessLevel.No);

            this._resourceBL.PopulateMaintenanceISTMembersViewData(ViewData, id, this.SelectedSport());
            ViewData["istCurrentPage"] = command.Page;

            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertISTMember(GridCommand command, string id)
        {
            var updatedModel = new MaintenanceISTMembersGridViewModel();

            if (TryUpdateModel(updatedModel))
            {
                if (updatedModel.BirthDate != null)
                {
                    updatedModel.DateOfBirth = DateTime.Parse(updatedModel.BirthDate);
                }
                updatedModel.CountryId = updatedModel.CountryName;
                updatedModel.PrimaryCscId = updatedModel.PrimaryCscName;
                updatedModel.ResourceSpecialtyId = updatedModel.ResourceSpecialtyName;

                int changes = this._resourceBL.AddResource(
                    Constant.ResourceType.ISTMembers,
                    updatedModel.FirstName,
                    updatedModel.LastName,
                    updatedModel.CountryId,
                    updatedModel.DateOfBirth,
                    updatedModel.TBD,
                    updatedModel.Pooled,
                    true,
                    resourceSpecialtyId: updatedModel.ResourceSpecialtyId,
                    primaryCSCId: updatedModel.PrimaryCscId,
                    assignSportId: updatedModel.AssignSportId,
                    yearId: this.SelectedYear()
                    );
            }

            return ISTMembersGridAjax(command, id);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult UpdateISTMember(GridCommand command, string id)
        {
            var updatedModel = new MaintenanceISTMembersGridViewModel();
            updatedModel.ResourceId = int.Parse(id);

            if (TryUpdateModel(updatedModel))
            {
                if (updatedModel.BirthDate != null)
                {
                    updatedModel.DateOfBirth = DateTime.Parse(updatedModel.BirthDate);
                }
                updatedModel.CountryId = updatedModel.CountryName;
                updatedModel.PrimaryCscId = updatedModel.PrimaryCscName;
                updatedModel.ResourceSpecialtyId = updatedModel.ResourceSpecialtyName;

                int changes = this._resourceBL.UpdateResource(
                    updatedModel.ResourceId,
                    resourceTypeId: Constant.ResourceType.ISTMembers,
                    resourceSpecialtyId: updatedModel.ResourceSpecialtyId, 
                    firstName: updatedModel.FirstName,
                    lastName: updatedModel.LastName,
                    countryId: updatedModel.CountryId,
                    dateOfBirth: updatedModel.DateOfBirth,
                    tbd: updatedModel.TBD,
                    pooled: updatedModel.Pooled,
                    isNew: true,
                    assignSportId: updatedModel.AssignSportId,
                    primaryCSCId: updatedModel.PrimaryCscId,
                    yearId: this.SelectedYear()
                    );
            }

            return ISTMembersGridAjax(command, id);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult DeleteISTMember(GridCommand command, string id)
        {
            int key = this._resourceBL.UpdateResource(
                Convert.ToInt32(id),
                active: false);

            //this._resourceBL.DeleteReource(Convert.ToInt32(id));
            
            return ISTMembersGridAjax(command, id);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ISTMembersGridAjax(GridCommand command, string id)
        {
            int total = 0;
            var model = this._resourceBL.GetMaintenanceISTMembersGridViewModel(this.SelectedSport(), command, out total);

            return View(new GridModel
            {
                Data = model,
                Total = total
            });
        }

        #region General Tab

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ISTMembersGeneralInfo(string id, bool editable)
        {
            this._resourceBL.PopulateMaintenanceISTMembersTabViewData(ViewData, id);
            MaintenanceISTMembersGeneralInfoViewModel model = null;
            if (!string.IsNullOrEmpty(id))
            {
                model = this._resourceBL.GetMaintenanceISTMembersGeneralInfoViewModel(int.Parse(id));
                model.Editable = editable;
            }

            return PartialView(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ISTMembersGeneralInfoSave(string id, FormCollection collection)
        {
            var updatedModel = new MaintenanceISTMembersGeneralInfoViewModel();
            updatedModel.ResourceId = int.Parse(id);

            if (TryUpdateModel(updatedModel, null, null, new[] { "Countries", "CSCs", "Genders", "Specialties" }))
            {

                this._resourceBL.UpdateResource(
                    updatedModel.ResourceId,
                    firstName: updatedModel.FirstName,
                    lastName: updatedModel.LastName,
                    countryId: updatedModel.CountryId,
                    gender: updatedModel.Gender,
                    dateOfBirth: updatedModel.DateOfBirth,
                    primaryCSCId: updatedModel.PrimaryCSCId,
                    secondaryCSCId: updatedModel.SecondaryCSCId,
                    resourceSpecialtyId: updatedModel.SpecialtyId,
                    comments: updatedModel.Comments);

                return RedirectToAction("ISTMembers", new { id = id });
            }

            var currentPageId = Constant.PageId.MaintenanceISTMembers;

            var model = new MaintenanceISTMembersViewModel();
            model.Menu = this.Menu(currentPageId);
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();

            ViewData["resourceId"] = id;

            return View("ISTMembers", model);
        }

        #endregion General Tab

        #region ContactInfoTab

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ISTMembersContactInfo(string id, bool editable)
        {
            this._resourceBL.PopulateMaintenanceISTMembersTabViewData(ViewData, id);
            MaintenanceISTMembersContactInfoViewModel model = null;
            if (!string.IsNullOrEmpty(id))
            {
                model = this._resourceBL.GetMaintenanceISTMembersContactInfoViewModel(int.Parse(id));
                model.Editable = editable;
            }

            return PartialView(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ISTMembersContactInfoSave(string id, FormCollection collection)
        {
            var updatedModel = new MaintenanceISTMembersContactInfoViewModel();
            updatedModel.ResourceId = int.Parse(id);

            if (TryUpdateModel(updatedModel, null, null, new[] { "Countries", "Provinces" }))
            {
                this._resourceBL.UpdateResource(
                    updatedModel.ResourceId,
                    emailAddress: updatedModel.EMailAddress,
                    skypeName: updatedModel.SkypeName,
                    mailingAddress: updatedModel.MailingAddress,
                    mailingCity: updatedModel.MailingCity,
                    mailingProvinceId: updatedModel.MailingProvinceId,
                    mailingCountryId: updatedModel.MailingCountryId,
                    mailingPostalCode: updatedModel.MailingPostalCode,
                    homePhone: updatedModel.HomePhone,
                    cellPhone: updatedModel.CellPhone);

                return RedirectToAction("ISTMembers", new { id = id });
            }

            string currentPageId = Constant.PageId.MaintenanceISTMembers;

            var model = new MaintenanceISTMembersViewModel();
            model.Menu = this.Menu(currentPageId);
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();

            ViewData["resourceId"] = id;

            return View("ISTMembers", model);
        }

        #endregion ContactInfoTab

        #region SportInfoTab

        public ActionResult ISTMembersSportsInfo(string id, bool editable)
        {
            this._resourceBL.PopulateMaintenanceISTMembersTabViewData(ViewData, id);

            MaintenanceEditableViewModel model = new MaintenanceEditableViewModel { Editable = editable };
            return PartialView(model);
        }

        [GridAction]
        public ActionResult ISTMembersSportsInfoAjax(string id)
        {
            List<MaintenanceISTMembersSportsInfoViewModel> model = null;

            if (!string.IsNullOrEmpty(id))
            {
                model = this._resourceBL.GetMaintenanceISTMembersSportsInfoViewModel(int.Parse(id));
            }

            return View(new GridModel(model));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertISTMembersSport(string id)
        {
            var updatedModel = new MaintenanceISTMembersSportsInfoViewModel();
            updatedModel.YearId = this.SelectedYear();

            if (TryUpdateModel(updatedModel))
            {
                updatedModel.ResourceId = int.Parse(id);

                //validation
                List<int> resourceSports = this._resourceBL.GetResourceSportIdsByResourceSport(updatedModel.ResourceId, updatedModel.SportsId, updatedModel.YearId);
                if (resourceSports != null && resourceSports.Count != 0)
                {
                    throw new Exception(Shared.SharedStrings.Error_SportAssociation);
                }

                var key = this._resourceBL.AddResourceSport(
                    updatedModel.ResourceId,
                    updatedModel.SportsId,
                    updatedModel.YearId,
                    null,
                    null,
                    null,
                    null,
                    updatedModel.CrossCompetency,
                    updatedModel.Primary);
            }

            return ISTMembersSportsInfoAjax(id);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult UpdateISTMembersSport(string id, string ResourceId)
        {
            var updatedModel = new MaintenanceISTMembersSportsInfoViewModel();
            updatedModel.ResourceSportId = int.Parse(id);

            if (TryUpdateModel(updatedModel))
            {
                //validation
                List<int> resourceSports = this._resourceBL.GetResourceSportIdsByResourceSport(int.Parse(ResourceId), updatedModel.SportsId, this.SelectedYear());
                if (resourceSports != null && resourceSports.Count != 0 && resourceSports.First() != updatedModel.ResourceSportId)
                {
                    throw new Exception(Shared.SharedStrings.Error_SportAssociation);
                }

                var key = this._resourceBL.UpdateResourceSport(
                    updatedModel.ResourceSportId,
                    sportId: updatedModel.SportsId,
                    istCrossCompetency: updatedModel.CrossCompetency,
                    istPrimary: updatedModel.Primary);
            }

            return ISTMembersSportsInfoAjax(ResourceId);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult DeleteISTMembersSport(int id)
        {
            int resourceId = this._resourceBL.GetResourceIdByResourceSport(id);
            var key = this._resourceBL.DeleteResourceSport(id);
            return ISTMembersSportsInfoAjax(resourceId.ToString());
        }

        #endregion SportInfoTab

        #region QualificationsTab

        public ActionResult ISTMembersQualificationsInfo(string id, bool editable)
        {
            this._resourceBL.PopulateMaintenanceISTMembersTabViewData(ViewData, id);

            MaintenanceEditableViewModel model = new MaintenanceEditableViewModel { Editable = editable };
            return PartialView(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertISTResourceQualification(string id)
        {
            var updatedModel = new MaintenanceISTMembersQualificationInfoViewModel();
            
            if (TryUpdateModel(updatedModel))
            {
                updatedModel.ResourceId = int.Parse(id);

                var key = this._resourceBL.AddResourceQualification(
                    updatedModel.ResourceId,
                    updatedModel.QualificationTypeId,
                    updatedModel.Description,
                    updatedModel.Qualified,
                    null);
            }

            return ISTMembersQualificationsAjax(id);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult UpdateISTResourceQualification(string id, string ResourceId)
        {
            var updatedModel = new MaintenanceISTMembersQualificationInfoViewModel();

            if (TryUpdateModel(updatedModel))
            {
                int key = this._resourceBL.UpdateResourceQualification(
                    updatedModel.ResourceQualificationId,
                    qualificationTypeId: updatedModel.QualificationTypeId,
                    description: updatedModel.Description,
                    qualified: updatedModel.Qualified);
            }

            return ISTMembersQualificationsAjax(ResourceId);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult DeleteISTResourceQualification(int id)
        {
            int resourceId = this._resourceBL.GetResourceIdByResourceQualification(id);
            int key = this._resourceBL.DeleteResourceQualification(id);
            return ISTMembersQualificationsAjax(resourceId.ToString());
        }

        [GridAction]
        public ActionResult ISTMembersQualificationsAjax(string id)
        {
            List<MaintenanceISTMembersQualificationInfoViewModel> model = null;

            if (!string.IsNullOrEmpty(id))
            {
                model = this._resourceBL.GetMaintenanceISTMembersQualificationInfoViewModel(int.Parse(id));
            }

            return View(new GridModel(model));
        }

        #endregion QualificationsTab

        #region PoolInfoTab

        public ActionResult ISTMembersPoolInfo(string id, bool editable)
        {
            this._resourceBL.PopulateMaintenanceISTMembersTabViewData(ViewData, id);

            MaintenanceEditableViewModel model = new MaintenanceEditableViewModel { Editable = editable };
            return PartialView(model);
        }

        [GridAction]
        public ActionResult ISTMembersPoolInfoAjax(string id)
        {
            List<MaintenanceISTMembersPoolInfoViewModel> model = null;

            if (!string.IsNullOrEmpty(id))
            {
                model = this._resourceBL.GetMaintenanceISTMembersPoolInfoViewModel(int.Parse(id));
            }

            return View(new GridModel(model));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertISTMembersPoolAssignment(string id, bool pooled, FormCollection coll)
        {
            var updatedModel = new MaintenanceISTMembersPoolInfoViewModel();

            if (pooled)
            {
                updatedModel.ResourceIdPool = int.Parse(id);
            }
            else
            {
                updatedModel.ResourceIdAssignment = int.Parse(id);
            }

            if (TryUpdateModel(updatedModel))
            {
                this._resourceBL.AddResourcePoolAssignment(updatedModel.ResourceIdPool, updatedModel.ResourceIdAssignment);
            }

            return ISTMembersPoolInfoAjax(id);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult UpdateISTMembersPoolAssignment(string id, int resourceId)
        {
            var updatedModel = new MaintenanceISTMembersPoolInfoViewModel();

            if (TryUpdateModel(updatedModel))
            {
                this._resourceBL.UpdateResourcePoolAssignment(updatedModel.ResourcePoolAssignmentId, updatedModel.ResourceIdPool, updatedModel.ResourceIdAssignment);
            }

            return ISTMembersPoolInfoAjax(resourceId.ToString());
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult DeleteISTMembersPoolAssignment(int id, int resourceId)
        {
            this._resourceBL.DeleteResourcePoolAssignment(id);
            return ISTMembersPoolInfoAjax(resourceId.ToString());
        }

        #endregion PoolInfoTab

        #endregion ISTMembersMaintenance

        #region NSOMembersMaintenance

        public ActionResult NSOMembers(string id, GridCommand command)
        {
            string currentPageId = Constant.PageId.MaintenanceNSOMembers;
            Logger.LogMessage(string.Format("Entering Controller/Action PageId {0}", currentPageId));

            bool validUserOnPage = this.ValidateUserOnPage(currentPageId);
            if (!validUserOnPage)
            {
                return this.RedirectToUserHomePage();
            }

            string resourceType = this._resourceBL.GetResourceType(id);

            if (resourceType == null)
            {
                id = null;
            }
            else if (resourceType == Constant.ResourceType.Athletes)
            {
                return RedirectToAction("Athletes", "Maintenance", new { id = id });
            }
            else if (resourceType == Constant.ResourceType.ISTMembers)
            {
                return RedirectToAction("ISTMembers", "Maintenance", new { id = id });
            }

            if (!string.IsNullOrEmpty(Request.Params.Get("size")))
            {
                command = GridCommand.Parse(
                    int.Parse(Request.Params.Get("page")),
                    int.Parse(Request.Params.Get("size")),
                    Request.Params.Get("orderBy"),
                    string.Empty,
                    Request.Params.Get("filter"));
            }

            var model = new MaintenanceNSOMembersViewModel();
            model.Menu = this.Menu(currentPageId);
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();
            model.CurrentOlympicYears = this.OlympicYears();
            model.CurrentSport = this.SelectedSport();

            model.Editable = (model.Menu.Menu.WriteAccess != Constant.AccessLevel.No);

            this._resourceBL.PopulateMaintenanceNSOMembersViewData(ViewData, id, this.SelectedSport());
            ViewData["nsoCurrentPage"] = command.Page;

            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertNSOMember(GridCommand command, string id)
        {
            var updatedModel = new MaintenanceNSOMembersGridViewModel();

            if (TryUpdateModel(updatedModel))
            {
                if (updatedModel.BirthDate != null)
                {
                    updatedModel.DateOfBirth = DateTime.Parse(updatedModel.BirthDate);
                }
                updatedModel.CountryId = updatedModel.CountryName;
                updatedModel.GenderId = updatedModel.GenderName;
                updatedModel.ResourceSpecialtyId = updatedModel.ResourceSpecialtyName;

                int changes = this._resourceBL.AddResource(
                    Constant.ResourceType.NSOMembers,
                    updatedModel.FirstName,
                    updatedModel.LastName,
                    updatedModel.CountryId,
                    updatedModel.DateOfBirth,
                    updatedModel.TBD,
                    updatedModel.Pooled,
                    true,
                    resourceSpecialtyId: updatedModel.ResourceSpecialtyId,
                    gender: updatedModel.GenderId,
                    yearId: this.SelectedYear(),
                    assignSportId: updatedModel.AssignSportId);
            }

            return NSOMembersGridAjax(command, id);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult UpdateNSOMember(GridCommand command, string id)
        {
            var updatedModel = new MaintenanceNSOMembersGridViewModel();
            updatedModel.ResourceId = int.Parse(id);

            if (TryUpdateModel(updatedModel))
            {
                if (updatedModel.BirthDate != null)
                {
                    updatedModel.DateOfBirth = DateTime.Parse(updatedModel.BirthDate);
                }
                updatedModel.CountryId = updatedModel.CountryName;
                updatedModel.GenderId = updatedModel.GenderName;
                updatedModel.ResourceSpecialtyId = updatedModel.ResourceSpecialtyName;

                int changes = this._resourceBL.UpdateResource(
                    updatedModel.ResourceId,
                    resourceTypeId: Constant.ResourceType.NSOMembers,
                    resourceSpecialtyId: updatedModel.ResourceSpecialtyId,
                    firstName: updatedModel.FirstName,
                    lastName: updatedModel.LastName,
                    countryId: updatedModel.CountryId,
                    dateOfBirth: updatedModel.DateOfBirth,
                    tbd: updatedModel.TBD,
                    pooled: updatedModel.Pooled,
                    isNew: true,
                    assignSportId: updatedModel.AssignSportId,
                    gender: updatedModel.GenderId,
                    yearId: this.SelectedYear()
                    );
            }

            return NSOMembersGridAjax(command, id);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult DeleteNSOMember(GridCommand command, string id)
        {
            int key = this._resourceBL.UpdateResource(
                Convert.ToInt32(id),
                active: false);

            //this._resourceBL.DeleteReource(Convert.ToInt32(id));
            
            return NSOMembersGridAjax(command, id);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult NSOMembersGridAjax(GridCommand command, string id)
        {
            int total = 0;
            var model = this._resourceBL.GetMaintenanceNSOMembersGridViewModel(this.SelectedSport(), command, out total);

            return View(new GridModel
            {
                Data = model,
                Total = total
            });
        }

        #region General Tab

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NSOMembersGeneralInfo(string id, bool editable)
        {
            this._resourceBL.PopulateMaintenanceNSOMembersTabViewData(ViewData, id);
            MaintenanceNSOMembersGeneralInfoViewModel model = null;
            if (!string.IsNullOrEmpty(id))
            {
                model = this._resourceBL.GetMaintenanceNSOMembersGeneralInfoViewModel(int.Parse(id));
                model.Editable = editable;
            }

            return PartialView(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NSOMembersGeneralInfoSave(string id, FormCollection collection)
        {
            var updatedModel = new MaintenanceNSOMembersGeneralInfoViewModel();
            updatedModel.ResourceId = int.Parse(id);

            if (TryUpdateModel(updatedModel, null, null, new[] { "Countries", "CSCs", "Genders", "Specialties" }))
            {
                this._resourceBL.UpdateResource(
                    updatedModel.ResourceId,
                    firstName: updatedModel.FirstName,
                    lastName: updatedModel.LastName,
                    countryId: updatedModel.CountryId,
                    gender: updatedModel.Gender,
                    dateOfBirth: updatedModel.DateOfBirth,
                    primaryCSCId: updatedModel.PrimaryCSCId,
                    secondaryCSCId: updatedModel.SecondaryCSCId,
                    resourceSpecialtyId: updatedModel.SpecialtyId,
                    comments: updatedModel.Comments);

                return RedirectToAction("NSOMembers", new { id = id });
            }

            var currentPageId = Constant.PageId.MaintenanceNSOMembers;

            var model = new MaintenanceNSOMembersViewModel();
            model.Menu = this.Menu(currentPageId);
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();

            ViewData["resourceId"] = id;

            return View("NSOMembers", model);
        }

        #endregion General Tab

        #region ContactInfoTab

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NSOMembersContactInfo(string id, bool editable)
        {
            this._resourceBL.PopulateMaintenanceNSOMembersTabViewData(ViewData, id);
            MaintenanceNSOMembersContactInfoViewModel model = null;
            if (!string.IsNullOrEmpty(id))
            {
                model = this._resourceBL.GetMaintenanceNSOMembersContactInfoViewModel(int.Parse(id));
                model.Editable = editable;
            }

            return PartialView(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NSOMembersContactInfoSave(string id, FormCollection collection)
        {
            var updatedModel = new MaintenanceNSOMembersContactInfoViewModel();
            updatedModel.ResourceId = int.Parse(id);

            if (TryUpdateModel(updatedModel, null, null, new[] { "Countries", "Provinces" }))
            {
                this._resourceBL.UpdateResource(
                    updatedModel.ResourceId,
                    emailAddress: updatedModel.EMailAddress,
                    skypeName: updatedModel.SkypeName,
                    mailingAddress: updatedModel.MailingAddress,
                    mailingCity: updatedModel.MailingCity,
                    mailingProvinceId: updatedModel.MailingProvinceId,
                    mailingCountryId: updatedModel.MailingCountryId,
                    mailingPostalCode: updatedModel.MailingPostalCode,
                    homePhone: updatedModel.HomePhone,
                    cellPhone: updatedModel.CellPhone);

                return RedirectToAction("NSOMembers", new { id = id });
            }

            string currentPageId = Constant.PageId.MaintenanceNSOMembers;

            var model = new MaintenanceNSOMembersViewModel();
            model.Menu = this.Menu(currentPageId);
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();

            ViewData["resourceId"] = id;

            return View("NSOMembers", model);
        }

        #endregion ContactInfoTab

        #region SportInfoTab

        public ActionResult NSOMembersSportsInfo(string id, bool editable)
        {
            this._resourceBL.PopulateMaintenanceNSOMembersTabViewData(ViewData, id);

            MaintenanceEditableViewModel model = new MaintenanceEditableViewModel { Editable = editable };
            return PartialView(model);
        }

        [GridAction]
        public ActionResult NSOMembersSportsInfoAjax(string id)
        {
            List<MaintenanceNSOMembersSportsInfoViewModel> model = null;

            if (!string.IsNullOrEmpty(id))
            {
                model = this._resourceBL.GetMaintenanceNSOMembersSportsInfoViewModel(int.Parse(id));
            }

            return View(new GridModel(model));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertNSOMembersSport(string id)
        {
            var updatedModel = new MaintenanceNSOMembersSportsInfoViewModel();
            updatedModel.YearId = this.SelectedYear();

            if (TryUpdateModel(updatedModel))
            {
                updatedModel.ResourceId = int.Parse(id);

                //validation
                List<int> resourceSports = this._resourceBL.GetResourceSportIdsByResourceSport(updatedModel.ResourceId, updatedModel.SportsId, updatedModel.YearId);
                if (resourceSports != null && resourceSports.Count != 0)
                {
                    throw new Exception(Shared.SharedStrings.Error_SportAssociation);
                }

                var key = this._resourceBL.AddResourceSport(
                    updatedModel.ResourceId,
                    updatedModel.SportsId,
                    updatedModel.YearId,
                    null,
                    null,
                    null,
                    null,
                    updatedModel.CrossCompetency,
                    updatedModel.Primary);
            }

            return NSOMembersSportsInfoAjax(id);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult UpdateNSOMembersSport(string id, string ResourceId)
        {
            var updatedModel = new MaintenanceNSOMembersSportsInfoViewModel();
            updatedModel.ResourceSportId = int.Parse(id);

            if (TryUpdateModel(updatedModel))
            {
                //validation
                List<int> resourceSports = this._resourceBL.GetResourceSportIdsByResourceSport(int.Parse(ResourceId), updatedModel.SportsId, this.SelectedYear());
                if (resourceSports != null && resourceSports.Count != 0 && resourceSports.First() != updatedModel.ResourceSportId)
                {
                    throw new Exception(Shared.SharedStrings.Error_SportAssociation);
                }

                var key = this._resourceBL.UpdateResourceSport(
                    updatedModel.ResourceSportId,
                    sportId: updatedModel.SportsId,
                    istCrossCompetency: updatedModel.CrossCompetency,
                    istPrimary: updatedModel.Primary);
            }

            return NSOMembersSportsInfoAjax(ResourceId);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult DeleteNSOMembersSport(int id)
        {
            int resourceId = this._resourceBL.GetResourceIdByResourceSport(id);
            var key = this._resourceBL.DeleteResourceSport(id);
            return NSOMembersSportsInfoAjax(resourceId.ToString());
        }

        #endregion SportInfoTab

        #region QualificationsTab

        public ActionResult NSOMembersQualificationsInfo(string id, bool editable)
        {
            this._resourceBL.PopulateMaintenanceNSOMembersTabViewData(ViewData, id);

            MaintenanceEditableViewModel model = new MaintenanceEditableViewModel { Editable = editable };
            return PartialView(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertNSOResourceQualification(string id)
        {
            var updatedModel = new MaintenanceNSOMembersQualificationInfoViewModel();
            
            if (TryUpdateModel(updatedModel))
            {
                updatedModel.ResourceId = int.Parse(id);

                var key = this._resourceBL.AddResourceQualification(
                    updatedModel.ResourceId,
                    updatedModel.QualificationTypeId,
                    updatedModel.Description,
                    updatedModel.Qualified,
                    null);
            }

            return NSOMembersQualificationsAjax(id);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult UpdateNSOResourceQualification(string id, string ResourceId)
        {
            var updatedModel = new MaintenanceNSOMembersQualificationInfoViewModel();

            if (TryUpdateModel(updatedModel))
            {
                int key = this._resourceBL.UpdateResourceQualification(
                    updatedModel.ResourceQualificationId,
                    qualificationTypeId: updatedModel.QualificationTypeId,
                    description: updatedModel.Description,
                    qualified: updatedModel.Qualified);
            }

            return NSOMembersQualificationsAjax(ResourceId);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult DeleteNSOResourceQualification(int id)
        {
            int resourceId = this._resourceBL.GetResourceIdByResourceQualification(id);
            int key = this._resourceBL.DeleteResourceQualification(id);
            return NSOMembersQualificationsAjax(resourceId.ToString());
        }

        [GridAction]
        public ActionResult NSOMembersQualificationsAjax(string id)
        {
            List<MaintenanceNSOMembersQualificationInfoViewModel> model = null;

            if (!string.IsNullOrEmpty(id))
            {
                model = this._resourceBL.GetMaintenanceNSOMembersQualificationInfoViewModel(int.Parse(id));
            }

            return View(new GridModel(model));
        }

        #endregion QualificationsTab

        #region PoolInfoTab

        public ActionResult NSOMembersPoolInfo(string id, bool editable)
        {
            this._resourceBL.PopulateMaintenanceNSOMembersTabViewData(ViewData, id);

            MaintenanceEditableViewModel model = new MaintenanceEditableViewModel { Editable = editable };
            return PartialView(model);
        }

        [GridAction]
        public ActionResult NSOMembersPoolInfoAjax(string id)
        {
            List<MaintenanceNSOMembersPoolInfoViewModel> model = null;

            if (!string.IsNullOrEmpty(id))
            {
                model = this._resourceBL.GetMaintenanceNSOMembersPoolInfoViewModel(int.Parse(id));
            }

            return View(new GridModel(model));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertNSOMembersPoolAssignment(string id, bool pooled, FormCollection coll)
        {
            var updatedModel = new MaintenanceNSOMembersPoolInfoViewModel();

            if (pooled)
            {
                updatedModel.ResourceIdPool = int.Parse(id);
            }
            else
            {
                updatedModel.ResourceIdAssignment = int.Parse(id);
            }

            if (TryUpdateModel(updatedModel))
            {
                this._resourceBL.AddResourcePoolAssignment(updatedModel.ResourceIdPool, updatedModel.ResourceIdAssignment);
            }

            return NSOMembersPoolInfoAjax(id);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult DeleteNSOMembersPoolAssignment(int id, int resourceId)
        {
            this._resourceBL.DeleteResourcePoolAssignment(id);
            return NSOMembersPoolInfoAjax(resourceId.ToString());
        }

        #endregion PoolInfoTab

        #endregion NSOMembersMaintenance

        #region SportsMaintenance

        public ActionResult MaintainSports(string id)
        {
            string currentPageId = Constant.PageId.MaintenanceMaintainSports;
            Logger.LogMessage(string.Format("Entering Controller/Action PageId {0}", currentPageId));

            bool validUserOnPage = this.ValidateUserOnPage(currentPageId);
            if (!validUserOnPage)
            {
                return this.RedirectToUserHomePage();
            }

            var model = new MaintenanceSportsViewModel();
            model.Menu = this.Menu(currentPageId);
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();
            model.CurrentOlympicYears = this.OlympicYears();

            this._sportBL.PopulateMaintenanceSportsViewData(ViewData, this.SelectedSport());

            return View(model);
        }

        [GridAction]
        public ActionResult _SportsMaintenanceAjax()
        {
            //Set sport editable/readable based on permission
            string currentPageId = Constant.PageId.MaintenanceMaintainSports;
            var menu = this.Menu(currentPageId).Menu;
            
            //set readable
            List<RingMenuItem> appliedSports = new List<RingMenuItem>();
            if (menu.ReadAccess == Constant.AccessLevel.Assigned)
            {
                appliedSports = menu.SportMenuItems;
            }
            else if (menu.ReadAccess == Constant.AccessLevel.All)
            {
                appliedSports = new Common.MenuHierarchy(this.AllSports, new List<Data.Models.Sport> { new Data.Models.Sport { SportID = Constant.SportId.All } }, null, null, this.CurrentLanguageCode(), false).SportMenuItems;
            }

            //sports are filtered by selected sport in the left sport menu
            SportDetail selectedSport = this._sportBL.GetValidSportForDisplaySummary(this.SelectedSport());
            IList<Models.ListItem> selectedSports = GetSelectedPermittedSports(selectedSport, appliedSports);

            //retreive sports
            var model = this._sportBL.GetMaintenanceSportsGridSportViewModel(this.SelectedYear(), selectedSports);

            //set editable
            List<RingMenuItem> allPermittedSports = menu.FlatSportMenuItems;

            foreach (var item in model)
            {
                item.Editable = false;

                if (menu.WriteAccess == Constant.AccessLevel.All)
                {
                    item.Editable = true;
                }
                else if (menu.WriteAccess == Constant.AccessLevel.Assigned)
                {
                    item.Editable = (from s in allPermittedSports where s.Id == item.SportId select s).Any();
                }
            }

            return View(new GridModel(model));
        }

        [GridAction]
        public ActionResult _DisciplinesForSportsMaintenanceAjax(string sportId)
        {
            var model = this._sportBL.GetMaintenanceSportsGridDisciplineViewModel(sportId);
            return View(new GridModel(model));
        }

        [GridAction]
        public ActionResult _EventsForDisciplinesMaintenanceAjax(string disciplineId)
        {
            var model = this._sportBL.GetMaintenanceSportsGridEventViewModel(disciplineId);
            return View(new GridModel(model));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult UpdateSport(string id, string NSO, string HPA, string Ranking)
        {
            var year = this.SelectedYear();

            var updatedModel = new MaintenanceSportsGridSportViewModel();

            if (TryUpdateModel(updatedModel, null, null, new[] { "HPA", "NSO", "Ranking" }))
            {
                int changes = this._sportBL.UpdateSport(
                    sportId: id,
                    nsoId: NSO,
                    hpaId: HPA,
                    benchmark: updatedModel.Benchmark,
                    rankingYear: year,
                    rankingCategoryId: Ranking);
            }

            return _SportsMaintenanceAjax();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult UpdateDiscipline(string id)
        {
            var updatedModel = new MaintenanceSportsGridDisciplineViewModel();
            updatedModel.DisciplineId = id;
            var parentSportId = GetParentSportId(id);

            if (TryUpdateModel(updatedModel))
            {
                int changes = this._sportBL.UpdateSport(
                    sportId: updatedModel.DisciplineId,
                    sportCode: updatedModel.SportCode,
                    name_EN: updatedModel.Name_EN,
                    name_FR: updatedModel.Name_FR,
                    benchmark: updatedModel.Benchmark, 
                    gender: updatedModel.Gender);
            }

            return _DisciplinesForSportsMaintenanceAjax(parentSportId);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult DeleteDiscipline(string id)
        {
            //validation
            List<int> resources = this._sportBL.GetResourceIdsBySport(id);
            if (resources != null && resources.Count != 0)
            {
                throw new Exception(Shared.SharedStrings.Error_DeleteDiscipline);
            }

            if (this._sportBL.IfExistDescendantSportIDs(id))
            {
                throw new Exception(Shared.SharedStrings.Error_DeleteSportID);
            }

            var parentSportId = GetParentSportId(id);

            int changes = this._sportBL.DeleteEventOrDiscipline(id);

            return _DisciplinesForSportsMaintenanceAjax(parentSportId);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertDiscipline(string parentSportId)
        {
            var updatedModel = new MaintenanceSportsGridDisciplineViewModel();

            if (TryUpdateModel(updatedModel))
            {
                try
                {
                    int changes = this._sportBL.AddEventOrDiscipline(
                        parentSportId,
                        updatedModel.SportCode,
                        updatedModel.Name_EN,
                        updatedModel.Name_FR,
                        null,
                        SportBL.EventOrDiscipline.Discipline,
                        updatedModel.Benchmark,
                        null,
                        null,
                        updatedModel.Gender,
                        true,
                        false,
                        false);
                }
                catch (ArgumentException e)
                {
                    ModelState.AddValidationMessage("SportCode", e.Message);
                }
            }

            return _DisciplinesForSportsMaintenanceAjax(parentSportId);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult UpdateEvent(string id)
        {
            var updatedModel = new MaintenanceSportsGridEventViewModel();
            updatedModel.EventId = id;
            var parentSportId = GetParentSportId(id);

            if (TryUpdateModel(updatedModel))
            {
                int changes = this._sportBL.UpdateSport(
                    sportId: updatedModel.EventId,
                    sportCode: updatedModel.SportCode,
                    name_EN: updatedModel.Name_EN,
                    name_FR: updatedModel.Name_FR,
                    medalEvent: updatedModel.MedalEvent,
                    benchmark: updatedModel.Benchmark,
                    teamSize: updatedModel.TeamSize,
                    teamSport: updatedModel.TeamSport,
                    resourceTracking: updatedModel.ResourceTracking);
            }

            return _EventsForDisciplinesMaintenanceAjax(parentSportId);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult DeleteEvent(string id)
        {
            //validation
            List<int> resources = this._sportBL.GetResourceIdsBySport(id);
            if (resources != null && resources.Count != 0)
            {
                throw new Exception(Shared.SharedStrings.Error_DeleteEvent);
            }

            var parentSportId = GetParentSportId(id);

            int changes = this._sportBL.DeleteEventOrDiscipline(id);

            return _EventsForDisciplinesMaintenanceAjax(parentSportId);           
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertEvent(string parentSportId)
        {
            var updatedModel = new MaintenanceSportsGridEventViewModel();

            try
            {
                if (TryUpdateModel(updatedModel))
                {
                    int changes = this._sportBL.AddEventOrDiscipline(
                        parentSportId,
                        updatedModel.SportCode,
                        updatedModel.Name_EN,
                        updatedModel.Name_FR,
                        null,
                        SportBL.EventOrDiscipline.Event,
                        updatedModel.Benchmark,
                        updatedModel.TeamSize,
                        updatedModel.MedalEvent,
                        null,
                        true,
                        updatedModel.TeamSport,
                        updatedModel.ResourceTracking);
                }
            }
            catch (ArgumentException e)
            {
                ModelState.AddValidationMessage("SportCode", e.Message);
            }

            return _EventsForDisciplinesMaintenanceAjax(parentSportId);
        }

        #endregion SportsMaintenance

        #region BenchmarksMaintenance

        public ActionResult MaintainBenchmarks(string id)
        {
            string currentPageId = Constant.PageId.MaintenanceMaintainBenchmarks;
            Logger.LogMessage(string.Format("Entering Controller/Action PageId {0}", currentPageId));

            bool validUserOnPage = this.ValidateUserOnPage(currentPageId);
            if (!validUserOnPage)
            {
                return this.RedirectToUserHomePage();
            }

            var selectedSport = this.SelectedSport();

            var model = new MaintenanceBenchmarksViewModel();
            model.Menu = this.Menu(currentPageId);
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();
            model.CurrentOlympicYears = this.OlympicYears();
            model.InsertBenchmarkTier = new MaintenanceBenchmarksInsertBenchmarkTierViewModel();

            model.SelectedSport = this._benchmarkBL.MaintenanceBenchmarksGetValidSelectedSportId(selectedSport);
            
            //Only load viewdata if needed
            if(model.SelectedSport != null)
            {
                this._benchmarkBL.PopulateMaintenanceBenchmarksViewData(ViewData, model.SelectedSport);
            }

            if (TempData.ContainsKey("Errors"))
            {
                foreach (var error in TempData["Errors"].ToString().Split(new string[] {"|"}, StringSplitOptions.RemoveEmptyEntries))
                {
                    ModelState.AddModelError("", error);
                }
            }

            return View(model);
        }

        [GridAction]
        public ActionResult BenchmarksSportsGridAjax()
        {
            var year = this.SelectedYear();
            var selectedSport = this.SelectedSport();
            var model = this._benchmarkBL.GetMaintenanceBenchmarksGridSportViewModel(year, selectedSport);

            return View(new GridModel(model));
        }

        [GridAction]
        public ActionResult BenchmarksSportsBenchmarkGridAjax(string sportId)
        {
            var year = this.SelectedYear();
            var selectedSport = this.SelectedSport();
            var model = this._benchmarkBL.GetMaintenanceBenchmarksGridSportBenchmarkViewModel(year, sportId);
            
            return View(new GridModel(model));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertSportBenchmark(string sportId)
        {
            var updatedModel = new MaintenanceBenchmarksGridSportBenchmarkViewModel();
            updatedModel.SportId = sportId;
            updatedModel.YearId = this.SelectedYear();

            if (TryUpdateModel(updatedModel))
            {
                //validation
                List<int> benchmarkSports = this._benchmarkBL.GetBenchmarkSportIdsBySportYearTypeTier(updatedModel.SportId, updatedModel.YearId, updatedModel.BenchmarkType, updatedModel.BenchmarkTier);
                if (benchmarkSports != null && benchmarkSports.Count != 0)
                {
                    throw new Exception(Shared.SharedStrings.Error_AddExistingBenchmark);
                }

                int key = this._benchmarkBL.AddBenchmarkSport(
                    updatedModel.SportId,
                    updatedModel.BenchmarkType,
                    updatedModel.BenchmarkTier,
                    updatedModel.YearId,
                    updatedModel.NSOValue.HasValue ? updatedModel.NSOValue.Value : 0,
                    updatedModel.OTPValue.HasValue ? updatedModel.OTPValue.Value : 0,
                    null,
                    true);
            }

            return BenchmarksSportsBenchmarkGridAjax(sportId);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult UpdateSportBenchmark(int id, string sportId)
        {
            var updatedModel = new MaintenanceBenchmarksGridSportBenchmarkViewModel();
            updatedModel.BenchmarkSportId = id;
            updatedModel.SportId = sportId;
            updatedModel.YearId = this.SelectedYear();

            if (TryUpdateModel(updatedModel))
            {
                //validation
                List<int> benchmarkSports = this._benchmarkBL.GetBenchmarkSportIdsBySportYearTypeTier(updatedModel.SportId, updatedModel.YearId, updatedModel.BenchmarkType, updatedModel.BenchmarkTier);
                if (benchmarkSports != null && benchmarkSports.Count != 0 && benchmarkSports.First() != updatedModel.BenchmarkSportId)
                {
                    throw new Exception(Shared.SharedStrings.Error_UpdateExistingBenchmark);
                }

                int key = this._benchmarkBL.UpdateBenchmarkSport(
                    updatedModel.BenchmarkSportId,
                    sportId: updatedModel.SportId,
                    benchmarkTypeId: updatedModel.BenchmarkType,
                    benchmarkTierId: updatedModel.BenchmarkTier,
                    yearId: updatedModel.YearId,
                    nsoValue: updatedModel.NSOValue.HasValue ? updatedModel.NSOValue.Value : 0,
                    otpValue: updatedModel.OTPValue.HasValue ? updatedModel.OTPValue.Value : 0);
            }

            return BenchmarksSportsBenchmarkGridAjax(sportId);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult DeleteSportBenchmark(int id, string sportId)
        {
            int key = this._benchmarkBL.DeleteBenchmarkSport(id);

            return BenchmarksSportsBenchmarkGridAjax(sportId);
        }

        public ActionResult InsertBenchmarkTier(string sportId, FormCollection collection)
        {
            var updatedModel = new MaintenanceBenchmarksViewModel();
            updatedModel.InsertBenchmarkTier = new MaintenanceBenchmarksInsertBenchmarkTierViewModel();

            if (TryUpdateModel(updatedModel))
            {
                try
                {
                    this._benchmarkBL.AddBenchmarkTier(
                        sportId,
                        updatedModel.InsertBenchmarkTier.Name_EN,
                        updatedModel.InsertBenchmarkTier.Name_FR,
                        updatedModel.InsertBenchmarkTier.PositionStart,
                        updatedModel.InsertBenchmarkTier.PositionEnd,
                        null);
                }
                catch (Exception e)
                {
                    Logger.LogError(e.ToString());
                    TempData.Add("Errors", e.Message);
                }
            }
            else
            {
                string errorMessage = "";
                foreach (var item in ModelState.Values)
                {
                    foreach (var error in item.Errors)
                    {
                        errorMessage += error.ErrorMessage + "|";
                    }
                }

                TempData.Add("Errors", errorMessage);
            }

            return RedirectToAction("MaintainBenchmarks", "Maintenance");
        }

        #endregion BenchmarksMaintenance

        public ActionResult WhoAmI()
        {
            string currentPageId = Constant.PageId.HomeIndex;
            Logger.LogMessage(string.Format("Entering Controller/Action PageId {0}", currentPageId));

            var model = new WhoAmIViewModel();
            model.Menu = this.Menu(currentPageId);
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();
            model.User = this.CurrentUser;

            if (model.User.LiveId == "Guest")
            {
                model.User.FirstName = null;
                model.User.LastName = null;
                model.User.Comments = null;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult WhoAmI(WhoAmIViewModel postedModel)
        {
            if (ModelState.IsValid)
            {
                if (postedModel != null && postedModel.User != null)
                {
                    postedModel.User.LiveId = this.CurrentUser.AuthenticatedLiveId;

                    this._userBL.SaveUser(postedModel.User);
                }
                return this.RedirectToUserHomePage();
            }
            else
            {
                ModelState.AddModelError("", Shared.SharedStrings.Error_AllFieldsRequired);

                string currentPageId = Constant.PageId.HomeIndex;
                Logger.LogMessage(string.Format("Entering Controller/Action PageId {0}", currentPageId));

                var model = new WhoAmIViewModel();
                model.Menu = this.Menu(currentPageId);
                model.CurrentLanguageCode = this.CurrentLanguageCode();
                model.CurrentUserName = this.CurrentUserName();
                model.User = this.CurrentUser;

                model.User.FirstName = postedModel.User.FirstName;
                model.User.LastName = postedModel.User.LastName;
                model.User.Comments = postedModel.User.Comments;

                return View(model);
            }
        }

        private string GetParentSportId(string childSportId)
        {
            if (string.IsNullOrEmpty(childSportId)) return null;

            Data.Models.Sport sport = this._sportBL.GetSport(childSportId);

            if (sport == null) return null;

            return sport.ParentSportID;
        }

    }
}
