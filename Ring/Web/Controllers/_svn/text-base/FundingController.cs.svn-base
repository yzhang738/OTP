using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OTP.Ring.Common;
using OTP.Ring.Web.Utility;
using OTP.Ring.Models;
using OTP.Ring.Models.ViewModel;
using Telerik.Web.Mvc;
using System.Web.Routing;
using OTP.Ring.Business;

namespace OTP.Ring.Web.Controllers
{
    public class FundingController : BaseController
    {
        private IDecodeBL _decodeBL;
        private IFundingBL _fundingBL;
        private IActionItemBL _actionItemBL;
        private IUserBL _userBL;
        private ISportBL _sportBL;

        public FundingController(IFundingBL fundingBL, IDecodeBL decodeBL, IActionItemBL actionItemBL, IUserBL userBL, ISportBL sportBL)
            : base (decodeBL, userBL, sportBL)
        {
            this._fundingBL = fundingBL;
            this._decodeBL = decodeBL;
            this._actionItemBL = actionItemBL;
            this._userBL = userBL;
            this._sportBL = sportBL;
        }

        public ActionResult Index()
        {

            string controllerName = (string)this.RouteData.Values["controller"];
            string actionName = (string)this.RouteData.Values["action"];

            string currentPageId = Constant.PageId.FundingIndex;
            Logger.LogMessage(string.Format("Entering Controller/Action PageId {0}", currentPageId));

            bool validUserOnPage = this.ValidateUserOnPage(currentPageId);
            if (!validUserOnPage)
            {
                return this.RedirectToUserHomePage();
            }

            var model = new FundingIndexViewModel();
            model.Menu = this.Menu(currentPageId);
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();
            model.SummaryPageList = this.GetSummaryPageList(currentPageId);

            return View(model);
        }

        public ActionResult ActionItems(string referenceId, string type)
        {
            string currentPageId = Constant.PageId.FundingAction;
            Logger.LogMessage(string.Format("Entering Controller/Action PageId {0}", currentPageId));

            bool validUserOnPage = this.ValidateUserOnPage(currentPageId);
            if (!validUserOnPage)
            {
                return this.RedirectToUserHomePage();
            }

            var model = new FundingActionItemViewModel();
            model.Menu = this.Menu(currentPageId);
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();
            
            model.SelectorItems = new List<ListItem>() { new ListItem { Id = "O", Description = Shared.SharedStrings.ListItem_Open }, new ListItem { Id = "C", Description = Shared.SharedStrings.Listitem_Close } };
            model.SelectorValue = this.SelectedActionItemStatus();

            this.PopulateActionItemDecodeList();

            return View(model);
        }

        public ActionResult Recommendation(int? id)
        {
            string currentPageId = Constant.PageId.FundingRecommendation;
            Logger.LogMessage(string.Format("Entering Controller/Action PageId {0}", currentPageId));

            bool validUserOnPage = this.ValidateUserOnPage(currentPageId);
            if (!validUserOnPage)
            {
                return this.RedirectToUserHomePage();
            }

            var model = new FundingViewModel();
            model.Menu = this.Menu(currentPageId);
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();

            model.CurrentOlympicYears = this.OlympicYears();

            //If fundingId is not passed, use selected season and sport to look for existing funding
            if (!id.HasValue)
            {
                string selectedYear = this.SelectedYear();
                string selectedSport = this.SelectedSport();

                id = this._fundingBL.GetFundingIdBySportAndYear(selectedSport, selectedYear);
            }

            if (id.HasValue)
            {
                var funding = this._fundingBL.GetFunding(id.Value);

                if (funding != null && (funding.Type != Constant.OrganizationTypeId.NSO || this.ValidateSportOnPage(currentPageId, funding.SportId)))
                {
                    model.Funding = funding;

                    model.Title = GetTitle(model.Funding);
                    model.Editable = Editable(model.Funding, Constant.FundingAction.Recommend, model.Menu.Menu.SportMenuItems, model.Menu.Menu.WriteAccess);
                    model.Deleteable = model.Editable && (model.Funding.StatusCode == Constant.FundingStatus.New);
                    model.ChangeToStatus = GetChangeToStatus(model.Funding, Constant.FundingAction.Recommend, model.Menu.Menu.SportMenuItems, model.Menu.Menu.WriteAccess);
                    model.CurrentOlympicYears = this.OlympicYears(model.Funding.YearId);

                    model.ToolbarButtons = GetToolbarButtons(model.Menu.Menu.PageMenuItems);

                    model.CanAccessAllocation = (this.CurrentUser.GroupProfileId != Constant.SecurityGroupProfile.NSO);
                    model.CanAccessAvailableFund = (this.CurrentUser.GroupProfileId == Constant.SecurityGroupProfile.Admin || this.CurrentUser.GroupProfileId == Constant.SecurityGroupProfile.DBA) && (
                        model.Funding.StatusCode == Constant.FundingStatus.OtpRecommended
                        || model.Funding.StatusCode == Constant.FundingStatus.Approved
                        || model.Funding.StatusCode == Constant.FundingStatus.Allocated
                        || model.Funding.StatusCode == Constant.FundingStatus.Actualize
                        || model.Funding.StatusCode == Constant.FundingStatus.Revise
                        || model.Funding.StatusCode == Constant.FundingStatus.Closed);
                    model.CanEditActual = (model.Funding.StatusCode == Constant.FundingStatus.Actualize);
                    model.CanEditRevision = (model.Funding.StatusCode == Constant.FundingStatus.Revise);

                    model.CanEditHpaRecommend = (model.Funding.StatusCode == Constant.FundingStatus.Requested);
                    model.CanEditOtpRecommend = (model.Funding.StatusCode == Constant.FundingStatus.HpaRecommended);
                    model.CanEditFinalRecommend = (model.Funding.StatusCode == Constant.FundingStatus.OtpRecommended || model.Funding.StatusCode == Constant.FundingStatus.Approved);

                    model.ShrinkMode = (model.Funding.StatusCode == Constant.FundingStatus.Approved
                        || model.Funding.StatusCode == Constant.FundingStatus.Allocated
                        || model.Funding.StatusCode == Constant.FundingStatus.Actualize
                        || model.Funding.StatusCode == Constant.FundingStatus.Revise
                        || model.Funding.StatusCode == Constant.FundingStatus.Closed);

                    this.PopulateFundingDecodeList(model.Funding.OrganizationId);
                }
            }

            if (TempData.ContainsKey("Errors"))
            {
                ModelState.AddModelError("", TempData["Errors"].ToString());
            }

            return View(model);
        }

        public ActionResult Requests(int? id)
        {
            string currentPageId = Constant.PageId.FundingRequests;
            Logger.LogMessage(string.Format("Entering Controller/Action PageId {0}", currentPageId));

            bool validUserOnPage = this.ValidateUserOnPage(currentPageId);
            if (!validUserOnPage)
            {
                return this.RedirectToUserHomePage();
            }

            var model = new FundingViewModel();
            model.Menu = this.Menu(currentPageId);
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();

            model.CurrentOlympicYears = this.OlympicYears();

            //If fundingId is not passed, use selected season and sport to look for existing funding
            if (!id.HasValue)
            {
                string selectedYear = this.SelectedYear();
                string selectedSport = this.SelectedSport();

                id = this._fundingBL.GetFundingIdBySportAndYear(selectedSport, selectedYear);
            }

            if (id.HasValue)
            {
                var funding = this._fundingBL.GetFunding(id.Value);

                if (funding != null && (funding.Type != Constant.OrganizationTypeId.NSO || this.ValidateSportOnPage(currentPageId, funding.SportId)))
                {
                    model.Funding = funding;

                    model.Title = GetTitle(model.Funding);
                    model.Editable = Editable(model.Funding, Constant.FundingAction.Request, model.Menu.Menu.SportMenuItems, model.Menu.Menu.WriteAccess);
                    model.Deleteable = model.Editable && (model.Funding.StatusCode == Constant.FundingStatus.New);
                    model.ChangeToStatus = GetChangeToStatus(model.Funding, Constant.FundingAction.Request, model.Menu.Menu.SportMenuItems, model.Menu.Menu.WriteAccess);
                    model.CurrentOlympicYears = this.OlympicYears(model.Funding.YearId);

                    model.ToolbarButtons = GetToolbarButtons(model.Menu.Menu.PageMenuItems);

                    model.CanAccessAllocation = (this.CurrentUser.GroupProfileId != Constant.SecurityGroupProfile.NSO);
                    model.CanAccessAvailableFund = false;
                    model.CanEditActual = (model.Funding.StatusCode == Constant.FundingStatus.Actualize);
                    model.CanEditRevision = (model.Funding.StatusCode == Constant.FundingStatus.Revise);

                    model.CanEditHpaRecommend = (model.Funding.StatusCode == Constant.FundingStatus.Requested);
                    model.CanEditOtpRecommend = (model.Funding.StatusCode == Constant.FundingStatus.HpaRecommended);
                    model.CanEditFinalRecommend = (model.Funding.StatusCode == Constant.FundingStatus.OtpRecommended || model.Funding.StatusCode == Constant.FundingStatus.Approved);

                    model.ShrinkMode = (model.Funding.StatusCode == Constant.FundingStatus.Approved
                        || model.Funding.StatusCode == Constant.FundingStatus.Allocated
                        || model.Funding.StatusCode == Constant.FundingStatus.Actualize
                        || model.Funding.StatusCode == Constant.FundingStatus.Revise
                        || model.Funding.StatusCode == Constant.FundingStatus.Closed);

                    this.PopulateFundingDecodeList(model.Funding.OrganizationId);
                }
            }

            if (TempData.ContainsKey("Errors"))
            {
                ModelState.AddModelError("", TempData["Errors"].ToString());
            }

            return View(model);
        }

        public ActionResult RevisionActual(int? id)
        {
            string currentPageId = Constant.PageId.FundingRevision;
            Logger.LogMessage(string.Format("Entering Controller/Action PageId {0}", currentPageId));

            bool validUserOnPage = this.ValidateUserOnPage(currentPageId);
            if (!validUserOnPage)
            {
                return this.RedirectToUserHomePage();
            }

            var model = new FundingViewModel();
            model.Menu = this.Menu(currentPageId);
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();

            model.CurrentOlympicYears = this.OlympicYears();

            //If fundingId is not passed, use selected season and sport to look for existing funding
            if (!id.HasValue)
            {
                string selectedYear = this.SelectedYear();
                string selectedSport = this.SelectedSport();

                id = this._fundingBL.GetFundingIdBySportAndYear(selectedSport, selectedYear);
            }

            if (id.HasValue)
            {
                var funding = this._fundingBL.GetFunding(id.Value);

                if (funding != null && (funding.Type != Constant.OrganizationTypeId.NSO || this.ValidateSportOnPage(currentPageId, funding.SportId)))
                {
                    model.Funding = funding;

                    model.Title = GetTitle(model.Funding);
                    model.Editable = Editable(model.Funding, Constant.FundingAction.Revise, model.Menu.Menu.SportMenuItems, model.Menu.Menu.WriteAccess);
                    model.Deleteable = model.Editable && (model.Funding.StatusCode == Constant.FundingStatus.New);
                    model.ChangeToStatus = GetChangeToStatus(model.Funding, Constant.FundingAction.Revise, model.Menu.Menu.SportMenuItems, model.Menu.Menu.WriteAccess);
                    model.CurrentOlympicYears = this.OlympicYears(model.Funding.YearId);

                    model.ToolbarButtons = GetToolbarButtons(model.Menu.Menu.PageMenuItems);

                    model.CanAccessAllocation = (this.CurrentUser.GroupProfileId != Constant.SecurityGroupProfile.NSO);
                    model.CanAccessAvailableFund = false;
                    model.CanEditActual = (model.Funding.StatusCode == Constant.FundingStatus.Actualize);
                    model.CanEditRevision = (model.Funding.StatusCode == Constant.FundingStatus.Revise);

                    model.CanEditHpaRecommend = (model.Funding.StatusCode == Constant.FundingStatus.Requested);
                    model.CanEditOtpRecommend = (model.Funding.StatusCode == Constant.FundingStatus.HpaRecommended);
                    model.CanEditFinalRecommend = (model.Funding.StatusCode == Constant.FundingStatus.OtpRecommended || model.Funding.StatusCode == Constant.FundingStatus.Approved);

                    model.ShrinkMode = (model.Funding.StatusCode == Constant.FundingStatus.Approved
                        || model.Funding.StatusCode == Constant.FundingStatus.Allocated
                        || model.Funding.StatusCode == Constant.FundingStatus.Actualize
                        || model.Funding.StatusCode == Constant.FundingStatus.Revise
                        || model.Funding.StatusCode == Constant.FundingStatus.Closed);

                    this.PopulateFundingDecodeList(model.Funding.OrganizationId);
                }
            }

            if (TempData.ContainsKey("Errors"))
            {
                ModelState.AddModelError("", TempData["Errors"].ToString());
            }

            return View(model);
        }

        public ActionResult Summary()
        {
            string currentPageId = Constant.PageId.FundingSummary;
            Logger.LogMessage(string.Format("Entering Controller/Action PageId {0}", currentPageId));

            bool validUserOnPage = this.ValidateUserOnPage(currentPageId);
            if (!validUserOnPage)
            {
                return this.RedirectToUserHomePage();
            }

            var model = new FundingSummaryViewModel();
            model.Menu = this.Menu(currentPageId);
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();

            model.CurrentOlympicYears = this.OlympicYears();

            SportDetail selectedSport = this._sportBL.GetValidSportForDisplaySummary(this.SelectedSport());
            if (selectedSport != null)
            {
                model.VerifiedSelectedSport = selectedSport.Id;
                model.VerifiedSelectedSportDesc = selectedSport.Name;

                model.FundingSummaries = GetFundingSummaries(selectedSport, this.SelectedYear(), model.Menu.Menu.SportMenuItems, this.CurrentUser.GroupProfileId, model.Menu.Menu.ReadAccess);

                model.Editable = model.Menu.Menu.WriteAccess != Constant.AccessLevel.No;

                this.PopulateFundingSummaryDecodeList(model.Menu.Menu.SportMenuItems);
            }

            return View(model);
        }

        public ActionResult FundingAllocation(string actionName, int fundingId, string type, string yearId, string sportId)
        {
            string level1SportId = GetParentSportOnLevel1(sportId);

            var model = new FundingAllocationViewModel();
            model.actionName = actionName;
            model.fundingId = fundingId;

            model.FundingAllocation = this._fundingBL.GetFundingAllocation(type, yearId, level1SportId);

            return View(model);
        }

        [HttpPost]
        public ActionResult ChangeToStatus(int id, string changeFromStatus, string changeToStatus)
        {
            this._fundingBL.UpdateFundingStatus(id, changeFromStatus, changeToStatus);

            if (changeToStatus == Constant.FundingStatus.Requested || changeToStatus == Constant.FundingStatus.HpaRecommended || changeToStatus == Constant.FundingStatus.OtpRecommended || changeToStatus == Constant.FundingStatus.Approved)
                return RedirectToAction("Recommendation", new { id = id });
            else if (changeToStatus == Constant.FundingStatus.Allocated || changeToStatus == Constant.FundingStatus.Revise || changeToStatus == Constant.FundingStatus.Actualize || changeToStatus == Constant.FundingStatus.Closed)
                return RedirectToAction("RevisionActual", new { id = id });
            else
                return RedirectToAction("Requests", new { id = id });
        }

        #region GridAction

        [HttpPost]
        public ActionResult GetFundingPriorityAjax()
        {
            IEnumerable<Decode> types = this._decodeBL.GetDecodesByCategory(Constant.DecodeCategoryId.FundingPriority);

            return new JsonResult
            {
                Data = new SelectList(types, "Id", "Name")
            };
        }

        [HttpPost]
        public ActionResult GetAllOrganizationsAjax(string organizationId)
        {
            List<Organization> orgs = this._decodeBL.GetAllOrganizations();
            orgs.Sort(new Organization.Comparer(organizationId));

            return new JsonResult
            {
                Data = new SelectList(orgs, "Id", "Name")
            };
        }

        [HttpPost]
        public ActionResult GetFundingResourcesAjax(string fundingCategoryType, int fundingId)
        {
            List<Decode> types = new List<Decode>() { new Decode { Id = null, IntId = 0, Name = Shared.SharedStrings.ListItem_Select } };
            types.AddRange(this._fundingBL.GetFundingResources(fundingCategoryType, fundingId));

            return new JsonResult
            {
                Data = new SelectList(types, "Id", "Name")
            };
        }

        [HttpPost]
        public ActionResult GetFundingResourcesByCscAjax(string fundingCategoryType, int fundingId, string cscId)
        {
            List<Decode> types = new List<Decode>() { new Decode { Id = null, IntId = 0, Name = Shared.SharedStrings.ListItem_Select } };
            types.AddRange(this._fundingBL.GetFundingResourcesByCsc(fundingCategoryType, fundingId, cscId));

            return new JsonResult
            {
                Data = new SelectList(types, "Id", "Name")
            };
        }

        [GridAction]
        public ActionResult GetActionItemsAjax(string referenceId, string type)
        {
            //Set action item page editable/readable based on permission
            string currentPageId = Constant.PageId.FundingAction;
            var menu = this.Menu(currentPageId).Menu;

            //set readable
            IList<Models.ListItem> selectedSports = new List<Models.ListItem>();
            if (menu.ReadAccess == Constant.AccessLevel.Assigned)
            {
                selectedSports = GetSelectedPermittedSports(new SportDetail { Id = Constant.SportId.All, Level = Constant.SportLevelId.All }, menu.SportMenuItems);
            }
            else if (menu.ReadAccess == Constant.AccessLevel.All)
            {
                selectedSports = null;  //Don't filter AccessLevel.All, because SportId is not required for an ActionItem
            }


            //retrieve
            List<ActionItem> actionItems = this._actionItemBL.GetActionItems(this.SelectedActionItemStatus(), referenceId, Constant.ActionItemSource.Funding, type, this.CurrentUser.Id, selectedSports);


            //set editable
            List<RingMenuItem> allPermittedSports = menu.FlatSportMenuItems;

            foreach (var item in actionItems)
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

            return View(new GridModel(actionItems));
        }

        [GridAction]
        public ActionResult GetFundingTotalsAjax(int id)
        {
            IEnumerable<FundingTotal> fundingTotals = this._fundingBL.GetFundingTotalsByFundingId(id);
            return View(new GridModel(fundingTotals));
        }

        [GridAction]
        public ActionResult GetFundingBlocksAjax(int id) 
        {
            IEnumerable<FundingBlock> fundingBlocks = this._fundingBL.GetFundingBlocksByFundingId(id);
            return View(new GridModel(fundingBlocks)); 
        }

        [GridAction]
        public ActionResult GetFundingCategoriesAjax(int id, int blockId)
        {
            IEnumerable<FundingCategory> fundingCategories = this._fundingBL.GetFundingCategoriesByFundingBlockId(id, blockId);
            return View(new GridModel(fundingCategories));
        }

        [GridAction]
        public ActionResult GetFundingDetailsAjax(int id, int categoryId)
        {
            IEnumerable<FundingDetail> fundingDetails = this._fundingBL.GetFundingDetailsByFundingCategoryId(id, categoryId);
            return View(new GridModel(fundingDetails));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult UpdateFundingDetailsAjax(int id, int fundingId, int categoryId, string actionName, FormCollection form)
        {
            FundingDetail fundingDetail = new FundingDetail();
            if (TryUpdateMyModel(fundingDetail, actionName, form))
            {
                this._fundingBL.SaveFundingDetail(actionName, fundingDetail, fundingId, categoryId);
            }

            IEnumerable<FundingDetail> fundingDetails = this._fundingBL.GetFundingDetailsByFundingCategoryId(fundingId, categoryId);
            return View(new GridModel(fundingDetails));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult DeleteFundingDetailsAjax(int id, int fundingId, int categoryId, string actionName, FormCollection form)
        {
            var result = this._fundingBL.DeleteFundingDetail(fundingId, id);

            IEnumerable<FundingDetail> fundingDetails = this._fundingBL.GetFundingDetailsByFundingCategoryId(fundingId, categoryId);
            return View(new GridModel(fundingDetails));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertFundingDetailsAjax(int fundingId, int categoryId, string actionName, FormCollection form)
        {
            FundingDetail fundingDetail = new FundingDetail();
            if (TryUpdateMyModel(fundingDetail, actionName, form))
            {
                this._fundingBL.SaveFundingDetail(actionName, fundingDetail, fundingId, categoryId);
            }

            IEnumerable<FundingDetail> fundingDetails = this._fundingBL.GetFundingDetailsByFundingCategoryId(fundingId, categoryId);
            return View(new GridModel(fundingDetails));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UpdateAvailableFunds(string actionName)
        {
            FundingAllocationViewModel allocation = new FundingAllocationViewModel();

            if (TryUpdateModel(allocation))
            {
                this._fundingBL.UpdateFundingAvailableFunds(allocation.FundingAllocation);
            }

            return RedirectToAction(allocation.actionName, "Funding", new { id = allocation.fundingId });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateFundingRequest(FormCollection form)
        {
            string action = form.GetFormStringValue("actionName");
            string fundingType = form.GetFormStringValue("fundingType");

            string organizationId = form.GetFormStringValue("OrgId");
            string projectId = form.GetFormStringValue("PrjId");
            string sportId = form.GetFormStringValue("SportId");
            string yearId = form.GetFormStringValue("YearId");

            //Validation
            if (this._fundingBL.DoesFundingExist(fundingType, projectId, organizationId, sportId, yearId))
            {
                ModelState.AddModelError("", Shared.SharedStrings.Error_AddExistingFundingRequest);
                TempData["ModelState"] = ModelState;
            }
            else
            {
                if (action == "Copy")   //Copy a funding request
                {
                    int? fundingId = form.GetFormIntValue("fundingId");

                    //this._fundingBL.CreateCopyFunding(fundingType, projectId, organizationId, sportId, yearId, fundingId, true, true, false, false, null);
                    this._fundingBL.CreateCopyFunding(fundingType, projectId, organizationId, sportId, yearId, fundingId, true, true, true, true, Constant.FundingCopyAmountType.Revised);
                }
                else if (action == "New")   //New funding request
                {
                    this._fundingBL.CreateNewFunding(fundingType, projectId, organizationId, sportId, yearId);
                }
            }

            return RedirectToAction("Summary", "Funding");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult UpdateActionItemAjax(string referenceId, string type)
        {
            ActionItem actionItem = new ActionItem();
            if (TryUpdateModel(actionItem))
            {
                var result = this._actionItemBL.UpdateActionItem(actionItem);
            }

            return GetActionItemsAjax(referenceId, type);
        }

        #endregion

        #region HELPER
        
        private string GetParentSportOnLevel1(string selectedSportId)
        {
            List<Data.Models.Sport> dataSports = this._sportBL.GetAllSportsToSportLevel();

            return (from sportL2 in dataSports
                    join sportL3 in dataSports on sportL2.SportID equals sportL3.ParentSportID
                    where sportL2.SportLevelID == Constant.SportLevelId.Season && sportL3.SportLevelID == Constant.SportLevelId.Sport && sportL3.SportID == selectedSportId
                    select sportL2.ParentSportID).FirstOrDefault();
        }

        private IEnumerable<FundingSummary> GetFundingSummariesForOtp(SportDetail selectedSport, string selectedYearId, List<RingMenuItem> allPermittedSports)
        {
            List<int> fundingIds = new List<int>();

            //Get funding Ids for NSOs
            IList<ListItem> selectedSports = GetSelectedPermittedSports(selectedSport, allPermittedSports);

            foreach (var sport in selectedSports)
            {
                int? fundingId = this._fundingBL.GetFundingIdBySportAndYear(sport.Id, selectedYearId);

                if (fundingId.HasValue)
                {
                    fundingIds.Add(fundingId.Value);
                }
            }

            //Get funding Ids for Non-NSOs, e.g. CSC and project
            fundingIds.AddRange(this._fundingBL.GetNonNSOFundingIdsByYear(selectedYearId));

            //Retrieve funding summaries
            IEnumerable<FundingSummary> fundingSummaries = this._fundingBL.GetFundingSummariesByFundingIds(fundingIds);

            return fundingSummaries;
        }

        private IEnumerable<FundingSummary> GetFundingSummariesForNso(SportDetail selectedSport, string selectedYearId, List<RingMenuItem> allPermittedSports)
        {
            List<int> fundingIds = new List<int>();

            //Get funding Ids for NSO Funding Types
            IList<ListItem> selectedSports = GetSelectedPermittedSports(selectedSport, allPermittedSports);

            foreach (var sport in selectedSports)
            {
                int? fundingId = this._fundingBL.GetFundingIdBySportAndYear(sport.Id, selectedYearId);

                if (fundingId.HasValue)
                {
                    fundingIds.Add(fundingId.Value);
                }
            }

            //Retrieve funding summaries
            IEnumerable<FundingSummary> fundingSummaries = this._fundingBL.GetFundingSummariesByFundingIds(fundingIds);

            return (from s in fundingSummaries where s.Type == Constant.OrganizationTypeId.NSO select s).ToList();
        }

        private IEnumerable<FundingSummary> GetFundingSummaries(SportDetail selectedSport, string selectedYearId, List<RingMenuItem> allPermittedSports, string groupProfileId, int readAccess)
        {
            //apply readaccess
            List<RingMenuItem> sportHierachy = new List<RingMenuItem>();
            if (readAccess == Constant.AccessLevel.All)
            {
                sportHierachy = new Common.MenuHierarchy(this.AllSports, new List<Data.Models.Sport> { new Data.Models.Sport { SportID = Constant.SportId.All } }, null, null, this.CurrentLanguageCode(), false).SportMenuItems;
            }
            else if (readAccess == Constant.AccessLevel.Assigned)
            {
                sportHierachy = allPermittedSports;
            }

            if (groupProfileId == Constant.SecurityGroupProfile.NSO)
            {
                return GetFundingSummariesForNso(selectedSport, selectedYearId, sportHierachy);
            }
            else
            {
                return GetFundingSummariesForOtp(selectedSport, selectedYearId, sportHierachy);
            }
        }

        private bool TryUpdateMyModel(FundingDetail fundingDetail, string actionName, FormCollection form)
        {
            //Update Model
            int? fundingDetailId = form.GetFormIntValue("id");

            if (fundingDetailId.HasValue)
            {
                fundingDetail.Id = form.GetFormIntValue("id").Value;
            }
            else
            {
                fundingDetail.Id = -1;
            }

            fundingDetail.Description = form.GetFormStringValue("Description");
            fundingDetail.CscId = form.GetFormStringValue("CscId");
            fundingDetail.ResourceId = form.GetFormIntValue("ResourceId");
            fundingDetail.CommittedPercent = form.GetFormDecimalValue("CommittedPercent");
            fundingDetail.PriorityCode = form.GetFormStringValue("PriorityCode");
            fundingDetail.Recurring = form.GetFormBooleanValue("Recurring");

            if (actionName == Constant.FundingAction.Request)
            {
                fundingDetail.OriginalBudget = form.GetFormDecimalValue("OriginalBudget");
                fundingDetail.RequestOtp = form.GetFormDecimalValue("RequestOtp");
                fundingDetail.RequestNso = form.GetFormDecimalValue("RequestNso");
                fundingDetail.RequestSC = form.GetFormDecimalValue("RequestSC");

                if (!(fundingDetail.RequestOtp.HasValue || fundingDetail.RequestNso.HasValue || fundingDetail.RequestSC.HasValue))
                {
                    fundingDetail.RequestOtp = fundingDetail.OriginalBudget;
                }

                fundingDetail.RequestOtp = fundingDetail.RequestOtp ?? 0m;
                fundingDetail.RequestNso = fundingDetail.RequestNso ?? 0m;
                fundingDetail.RequestSC = fundingDetail.RequestSC ?? 0m;

                if (!(fundingDetail.OriginalBudget == null || fundingDetail.RequestOtp == null || fundingDetail.RequestNso == null || fundingDetail.RequestSC == null))
                {
                    if (fundingDetail.OriginalBudget.Value < fundingDetail.RequestOtp.Value + fundingDetail.RequestNso.Value + fundingDetail.RequestSC.Value)
                    {
                        ModelState.AddModelError("ErrRequest", Shared.SharedStrings.Error_SumExceedTotalFunds);
                    }
                }
            }

            if (actionName == Constant.FundingAction.Recommend)
            {
                fundingDetail.HpaRecommend = form.GetFormDecimalValue("HpaRecommend");
                fundingDetail.OtpRecommend = form.GetFormDecimalValue("OtpRecommend");
                fundingDetail.FinalRecommend = form.GetFormDecimalValue("FinalRecommend");
                fundingDetail.RecommendSC = form.GetFormDecimalValue("RecommendSC");
                fundingDetail.RecommendCoc = form.GetFormDecimalValue("RecommendCoc");

                if (!(fundingDetail.FinalRecommend == null || fundingDetail.RecommendSC == null || fundingDetail.RecommendCoc == null))
                {
                    if (fundingDetail.FinalRecommend.Value < fundingDetail.RecommendSC.Value + fundingDetail.RecommendCoc.Value)
                    {
                        ModelState.AddModelError("ErrRecommend", Shared.SharedStrings.Error_RecommendedSum);
                    }
                }
            }

            if (actionName == Constant.FundingAction.Revise)
            {
                fundingDetail.Revised = form.GetFormDecimalValue("Revised");
                fundingDetail.RevisedSC = form.GetFormDecimalValue("RevisedSC");
                fundingDetail.RevisedCoc = form.GetFormDecimalValue("RevisedCoc");

                if (!(fundingDetail.Revised == null || fundingDetail.RevisedSC == null || fundingDetail.RevisedCoc == null))
                {
                    if (fundingDetail.Revised.Value < fundingDetail.RevisedSC.Value + fundingDetail.RevisedCoc.Value)
                    {
                        ModelState.AddModelError("ErrRevised", Shared.SharedStrings.Error_RevisedSum);
                    }
                }

                fundingDetail.PendingRevise = form.GetFormDecimalValue("PendingRevise");
                fundingDetail.PendingReviseSC = form.GetFormDecimalValue("PendingReviseSC");
                fundingDetail.PendingReviseCoc = form.GetFormDecimalValue("PendingReviseCoc");

                if (!(fundingDetail.PendingRevise == null || fundingDetail.PendingReviseSC == null || fundingDetail.PendingReviseCoc == null))
                {
                    if (fundingDetail.PendingRevise.Value < fundingDetail.PendingReviseSC.Value + fundingDetail.PendingReviseCoc.Value)
                    {
                        ModelState.AddModelError("ErrRevising", Shared.SharedStrings.Error_RevisingSum);
                    }
                }

                fundingDetail.Actual = form.GetFormDecimalValue("Actual");
            }

            //Validation
            return ModelState.IsValid;
        }

        private void PopulateFundingDecodeList(string organizationId)
        {
            ViewData["organizationId"] = organizationId;

            ViewData["actionPriorities"] = this._decodeBL.GetDecodesByCategory(Constant.DecodeCategoryId.ActionItemPriority);
        }

        private void PopulateFundingSummaryDecodeList(List<RingMenuItem> allPermittedSports)
        {
            ViewData["cscs"] = this._decodeBL.GetAllOrganizationsByType(Constant.OrganizationTypeId.CSC);
            ViewData["prjs"] = this._decodeBL.GetAllOrganizationsByType(Constant.OrganizationTypeId.OTP);
            ViewData["years"] = this._decodeBL.GetAllYearsList();
            ViewData["sports"] = GetSelectedPermittedSports(new SportDetail { Id = Constant.SportId.All, Level = Constant.SportLevelId.All }, allPermittedSports);
        }

        private void PopulateActionItemDecodeList()
        {
            ViewData["actionPriorities"] = this._decodeBL.GetDecodesByCategory(Constant.DecodeCategoryId.ActionItemPriority);
            ViewData["actionStatus"] = this._decodeBL.GetDecodesByCategory(Constant.DecodeCategoryId.ActionItemStatus);
        }

        private string GetTitle(Funding funding)
        {
            return string.Format("{0} ({1})", funding.Type == Constant.OrganizationTypeId.NSO ? funding.SportDesc : funding.OrganizationDesc, funding.StatusDesc);
        }

        //Determine when the Edit button is enabled
        private bool Editable(Funding funding, string actionName, List<RingMenuItem> allPermittedSports, int writeAccess)
        {
            return EditableStatus(funding, actionName) && EditableSport(funding, allPermittedSports, writeAccess);
        }

        private bool EditableSport(Funding funding, List<RingMenuItem> allPermittedSports, int writeAccess)
        {
            //TODO: who has permission to edit CSC or OTP fundings
            if (string.IsNullOrEmpty(funding.SportId)) return true; //This could be CSC or OTP fundings

            if (writeAccess == Constant.AccessLevel.No) return false;

            if (writeAccess == Constant.AccessLevel.All) return true;

            return MatchSport(funding.SportId, allPermittedSports);
        }

        private bool MatchSport(string sportId, List<RingMenuItem> allPermittedSports)
        {
            if (allPermittedSports == null) return false;

            foreach (var item in allPermittedSports)
            {
                if (item == null) return false;

                if (item.Id == sportId || MatchSport(sportId, item.ChildItems))
                    return true;
            }

            return false;
        }

        private bool EditableStatus(Funding funding, string actionName)
        {
            return 

                //On Requests action, when status is New, and NSO or HPA
                (actionName == Constant.FundingAction.Request 
                    && funding.StatusCode == Constant.FundingStatus.New)
                ||

                //On Recommendation action, when status is Requested and HPA
                //                                  or HapRecommended and HPA
                //                                  or OtpRecommended and HPA, 
                //                                  or Approved and HPA
                (actionName == Constant.FundingAction.Recommend && 
                    (funding.StatusCode == Constant.FundingStatus.Requested 
                        || funding.StatusCode == Constant.FundingStatus.HpaRecommended 
                        || funding.StatusCode == Constant.FundingStatus.OtpRecommended 
                        || funding.StatusCode == Constant.FundingStatus.Approved))
                || 
                
                //On Revise action, when status is Revise and HPA
                //                              or Actualize and HPA
                (actionName == Constant.FundingAction.Revise && 
                    (funding.StatusCode == Constant.FundingStatus.Actualize 
                        || funding.StatusCode == Constant.FundingStatus.Revise));
        }

        private List<string> GetToolbarButtons(List<RingMenuItem> allPermittedPages)
        {
            List<string> buttons = new List<string>();

            if (IsPagePermitted(allPermittedPages, Constant.PageId.FundingAction))
            {
                buttons.Add(Constant.PageId.FundingAction);
            }

            if (IsPagePermitted(allPermittedPages, Constant.PageId.MaintenanceResources))
            {
                buttons.Add(Constant.PageId.MaintenanceResources);
            }

            return buttons;
        }

        private List<FundingStatusChangeViewModel> GetChangeToStatus(Funding funding, string actionName, List<RingMenuItem> allPermittedSports, int writeAccess)
        {
            List<FundingStatusChangeViewModel> changeToStatus = new List<FundingStatusChangeViewModel>();

            bool isAdmin = (this.CurrentUser.GroupProfileId == Constant.SecurityGroupProfile.Admin || this.CurrentUser.GroupProfileId == Constant.SecurityGroupProfile.DBA);

            //Only HPA and Admin can change status
            if (this.CurrentUser.GroupProfileId != Constant.SecurityGroupProfile.HPA && !isAdmin) return changeToStatus;

            //Can only change Sports assigned to them
            if (!EditableSport(funding, allPermittedSports, writeAccess)) return changeToStatus;

            if (actionName == Constant.FundingAction.Request)
            {
                if (funding.StatusCode == Constant.FundingStatus.New)
                    changeToStatus.Add(new FundingStatusChangeViewModel { FromStatus = Constant.FundingStatus.New, ToStatus = Constant.FundingStatus.Requested });
            }
            else if (actionName == Constant.FundingAction.Recommend)
            {
                if (funding.StatusCode == Constant.FundingStatus.Requested)
                {
                    changeToStatus.Add(new FundingStatusChangeViewModel { FromStatus = Constant.FundingStatus.Requested, ToStatus = Constant.FundingStatus.HpaRecommended });
                    changeToStatus.Add(new FundingStatusChangeViewModel { FromStatus = Constant.FundingStatus.Requested, ToStatus = Constant.FundingStatus.New });
                }
                else if (funding.StatusCode == Constant.FundingStatus.HpaRecommended)
                {
                    changeToStatus.Add(new FundingStatusChangeViewModel { FromStatus = Constant.FundingStatus.HpaRecommended, ToStatus = Constant.FundingStatus.OtpRecommended });
                }
                else if (funding.StatusCode == Constant.FundingStatus.OtpRecommended)
                {
                    if (isAdmin) changeToStatus.Add(new FundingStatusChangeViewModel { FromStatus = Constant.FundingStatus.OtpRecommended, ToStatus = Constant.FundingStatus.Approved });
                }
                else if (funding.StatusCode == Constant.FundingStatus.Approved)
                {
                    if (isAdmin) changeToStatus.Add(new FundingStatusChangeViewModel { FromStatus = Constant.FundingStatus.Approved, ToStatus = Constant.FundingStatus.Allocated });
                }
            }
            else if (actionName == Constant.FundingAction.Revise)
            {
                if (funding.StatusCode == Constant.FundingStatus.Allocated)
                {
                    if (isAdmin) changeToStatus.Add(new FundingStatusChangeViewModel { FromStatus = Constant.FundingStatus.Allocated, ToStatus = Constant.FundingStatus.Closed });
                    changeToStatus.Add(new FundingStatusChangeViewModel { FromStatus = Constant.FundingStatus.Allocated, ToStatus = Constant.FundingStatus.Actualize });
                    if (isAdmin) changeToStatus.Add(new FundingStatusChangeViewModel { FromStatus = Constant.FundingStatus.Allocated, ToStatus = Constant.FundingStatus.Revise });
                }
                else if (funding.StatusCode == Constant.FundingStatus.Revise)
                {
                    if (isAdmin) changeToStatus.Add(new FundingStatusChangeViewModel { FromStatus = Constant.FundingStatus.Revise, ToStatus = Constant.FundingStatus.Allocated });
                }
                else if (funding.StatusCode == Constant.FundingStatus.Actualize)
                {
                    changeToStatus.Add(new FundingStatusChangeViewModel { FromStatus = Constant.FundingStatus.Actualize, ToStatus = Constant.FundingStatus.Allocated });
                }
            }

            return changeToStatus;
        }
        #endregion
    }
}
