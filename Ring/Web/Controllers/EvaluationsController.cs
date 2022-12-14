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
    public class EvaluationsController : BaseController
    {
        private IDecodeBL _decodeBL;
        private IUserBL _userBL;
        private ISportBL _sportBL;
        private IEvaluationsBL _evaluationsBL;
        private IActionItemBL _actionItemBL;

        public EvaluationsController(IDecodeBL decodeBL, IUserBL userBL, ISportBL sportBL, IEvaluationsBL evaluationsBL, IActionItemBL actionItemBL)
            : base(decodeBL, userBL, sportBL)
        {
            this._decodeBL = decodeBL;
            this._userBL = userBL;
            this._sportBL = sportBL;
            this._evaluationsBL = evaluationsBL;
            this._actionItemBL = actionItemBL;
        }

        public ActionResult Index()
        {
            string currentPageId = Constant.PageId.EvaluationsIndex;
            Logger.LogMessage(string.Format("Entering Controller/Action PageId {0}", currentPageId));

            bool validUserOnPage = this.ValidateUserOnPage(currentPageId);
            if (!validUserOnPage)
            {
                return this.RedirectToUserHomePage();
            }

            var model = new EvaluationsIndexViewModel();
            model.Menu = this.Menu(currentPageId);
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();
            model.SummaryPageList = this.GetSummaryPageList(currentPageId);

            return View(model);
        }

        public ActionResult Maintenance()
        {
            string currentPageId = Constant.PageId.EvaluationsMaintenance;
            Logger.LogMessage(string.Format("Entering Controller/Action PageId {0}", currentPageId));

            bool validUserOnPage = this.ValidateUserOnPage(currentPageId);
            if (!validUserOnPage)
            {
                return this.RedirectToUserHomePage();
            }

            var model = new EvaluationsMaintenanceViewModel();
            model.Menu = this.Menu(currentPageId);
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();
            model.SummaryPageList = this.GetSummaryPageList(currentPageId);

            return View(model);
        }

        #region ActionItems

        public ActionResult ActionItems(string referenceId, string type)
        {
            string currentPageId = Constant.PageId.EvaluationAction;
            Logger.LogMessage(string.Format("Entering Controller/Action PageId {0}", currentPageId));

            bool validUserOnPage = this.ValidateUserOnPage(currentPageId);
            if (!validUserOnPage)
            {
                return this.RedirectToUserHomePage();
            }

            var model = new EvaluationActionItemViewModel();
            model.Menu = this.Menu(currentPageId);
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();

            model.SelectorItems = new List<Models.ListItem>() { new Models.ListItem { Id = "O", Description = Shared.SharedStrings.ListItem_Open }, new Models.ListItem { Id = "C", Description = Shared.SharedStrings.Listitem_Close } };
            model.SelectorValue = this.SelectedActionItemStatus();

            this.PopulateActionItemDecodeList();

            return View(model);
        }

        [GridAction]
        public ActionResult GetActionItemsAjax(string referenceId, string type)
        {
            //Set action item page editable/readable based on permission
            string currentPageId = Constant.PageId.EvaluationAction;
            var menu = this.Menu(currentPageId).Menu;

            //set readable
            IList<Models.ListItem> selectedSports = null;
            List<RingMenuItem> appliedSports = new List<RingMenuItem>();

            if (menu.ReadAccess == Constant.AccessLevel.Assigned)
            {
                selectedSports = GetSelectedPermittedSports(new SportDetail { Id = Constant.SportId.All, Level = Constant.SportLevelId.All }, menu.SportMenuItems);
                appliedSports = menu.SportMenuItems;
            }
            else if (menu.ReadAccess == Constant.AccessLevel.All)
            {
                appliedSports = new Common.MenuHierarchy(this.AllSports, new List<Data.Models.Sport> { new Data.Models.Sport { SportID = Constant.SportId.All } }, null, null, this.CurrentLanguageCode(), false).SportMenuItems;
            }

            if (string.IsNullOrEmpty(referenceId))
            {
                //sports are filtered by selected sport in the left sport menu
                SportDetail selectedSport = this._sportBL.GetValidSportForDisplaySummary(this.SelectedSport());
                selectedSports = GetSelectedPermittedSports(selectedSport, appliedSports);
            }

            //retrieve
            List<ActionItem> actionItems = this._actionItemBL.GetActionItems(this.SelectedActionItemStatus(), referenceId, Constant.ActionItemSource.Evaluation, type, this.CurrentUser.Id, selectedSports);

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

        #region EvaluationQuestions

        public ActionResult Questions()
        {
            var currentPageId = Constant.PageId.EvaluationsQuestions;
            Logger.LogMessage(string.Format("Entering Controller/Action PageId {0}", currentPageId));

            bool validUserOnPage = this.ValidateUserOnPage(currentPageId);

            if (!validUserOnPage)
            {
                return this.RedirectToUserHomePage();
            }

            var model = new EvaluationsQuestionsViewModel();
            model.Menu = this.Menu(currentPageId);
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();
            model.CurrentOlympicYears = this.OlympicYears();
            model.VersionsGridModel = this._evaluationsBL.GetEvaluationsQuestionsEvaluationVersionGridViewModel(this.SelectedYear());

            model.Editable = (model.Menu.Menu.WriteAccess != Constant.AccessLevel.No);

            this._evaluationsBL.PopulateEvaluationsQuestionsViewData(ViewData, this.SelectedYear());

            return View(model);
        }

        [GridAction]
        public ActionResult LoadEvaluationsQuestionsEvaluationVersionGridAjax(string id)
        {
            var model = this._evaluationsBL.GetEvaluationsQuestionsEvaluationVersionGridViewModel(this.SelectedYear());
            return View(new GridModel(model));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertEvaluationVersion(string id)
        {
            var updatedModel = new EvaluationsQuestionsEvaluationVersionGridViewModel();
            updatedModel.YearId = this.SelectedYear();

            if (TryUpdateModel(updatedModel))
            {
                // Check if the evaluation version already exists
                if (!string.IsNullOrEmpty(this._evaluationsBL.IfEvaluationVersionExist(updatedModel.OrganizationTypeId, updatedModel.YearId, updatedModel.VersionId)))
                {
                    throw new Exception(Shared.SharedStrings.Error_AddEvaluationVersion);
                }

                id = this._evaluationsBL.AddEvaluationVersion(
                    updatedModel.OrganizationTypeId,
                    updatedModel.YearId,
                    updatedModel.VersionId,
                    updatedModel.VersionPhase,
                    updatedModel.Name_EN,
                    updatedModel.Name_FR,
                    null).ToString();
            }

            return LoadEvaluationsQuestionsEvaluationVersionGridAjax(id);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult UpdateEvaluationVersion(string id)
        {
            var updatedModel = new EvaluationsQuestionsEvaluationVersionGridViewModel();
            updatedModel.EvaluationVersionId = int.Parse(id);
            updatedModel.YearId = this.SelectedYear();

            if (TryUpdateModel(updatedModel))
            {
                int changes = this._evaluationsBL.UpdateEvaluationVersion(
                evaluationVersionId: updatedModel.EvaluationVersionId,
                organizationTypeId: updatedModel.OrganizationTypeId,
                yearId: updatedModel.YearId,
                versionId: updatedModel.VersionId,
                versionPhase: updatedModel.VersionPhase,
                name_EN: updatedModel.Name_EN,
                name_FR: updatedModel.Name_FR);
            }

            return LoadEvaluationsQuestionsEvaluationVersionGridAjax(id);
        }

        [GridAction]
        public ActionResult LoadEvaluationsQuestionsEvaluationPillarsGridAjax(string evaluationVersionId, bool showDeleted)
        {
            int id = 0;

            if (string.IsNullOrEmpty(evaluationVersionId))
            {
                id = this._evaluationsBL.GetFirstEvaluationVersionId(this.SelectedYear());
            }
            else
            {
                id = int.Parse(evaluationVersionId);
            }

            var model = this._evaluationsBL.GetEvaluationsQuestionsEvaluationPillarsGridViewModel(id, showDeleted);

            return View(new GridModel(model));
        }

        [GridAction]
        public ActionResult LoadEvaluationsQuestionsEvaluationPillarQuestionsGridAjax(string evaluationVersionId, string evaluationMasterPillarId, bool showDeleted)
        {
            var model = this._evaluationsBL.GetEvaluationsQuestionsEvaluationPillarQuestionsGridViewModel(int.Parse(evaluationVersionId), evaluationMasterPillarId, showDeleted);

            return View(new GridModel(model));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertEvaluationVersionQuestion(string evaluationVersionId, string evaluationMasterPillarId)
        {
            var updatedModel = new EvaluationsQuestionsEvaluationPillarQuestionsGridViewModel();
            updatedModel.EvaluationVersionId = int.Parse(evaluationVersionId);
            updatedModel.EvaluationMasterPillarId = evaluationMasterPillarId;

            if (TryUpdateModel(updatedModel))
            {
                if (updatedModel.SortOrder == 0)
                {
                    ModelState.AddValidationMessage("SortOrder", Shared.SharedStrings.Error_InvalidSortOrder);
                }
                else
                {
                    var id = this._evaluationsBL.AddEvaluationVersionQuestion(
                        updatedModel.EvaluationVersionId,
                        updatedModel.EvaluationMasterPillarId,
                        updatedModel.VersionQuestion,
                        updatedModel.QuestionPhase,
                        updatedModel.SortOrder,
                        null,
                        updatedModel.SportId).ToString();
                }
            }

            return LoadEvaluationsQuestionsEvaluationPillarQuestionsGridAjax(evaluationVersionId, evaluationMasterPillarId, false);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult UpdateEvaluationVersionQuestion(string id, string evaluationVersionId, string evaluationMasterPillarId)
        {
            var updatedModel = new EvaluationsQuestionsEvaluationPillarQuestionsGridViewModel();
            updatedModel.EvaluationVersionQuestionId = int.Parse(id);

            if (TryUpdateModel(updatedModel))
            {
                if (updatedModel.SortOrder == 0)
                {
                    ModelState.AddValidationMessage("SortOrder", Shared.SharedStrings.Error_InvalidSortOrder);
                }
                else
                {
                    int changes = this._evaluationsBL.UpdateEvaluationVersionQuestion(
                    evaluationVersionQuestionId: updatedModel.EvaluationVersionQuestionId,
                    versionQuestion: updatedModel.VersionQuestion,
                    questionPhase: updatedModel.QuestionPhase,
                    sortOrder: updatedModel.SortOrder,
                    sportId: updatedModel.SportId);
                }
            }

            return LoadEvaluationsQuestionsEvaluationPillarQuestionsGridAjax(evaluationVersionId, evaluationMasterPillarId, false);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult DeleteEvaluationVersionQuestion(string id, string evaluationVersionId, string evaluationMasterPillarId)
        {
            this._evaluationsBL.DeleteEvaluationVersionQuestion(int.Parse(id));

            return LoadEvaluationsQuestionsEvaluationPillarQuestionsGridAjax(evaluationVersionId, evaluationMasterPillarId, false);
        }

        public ActionResult CopyEvaluationVersion(string copyEvaluationVersionId, string copyToYear)
        {
            this._evaluationsBL.CopyEvaluationVersion(int.Parse(copyEvaluationVersionId), copyToYear);
            return RedirectToAction("Questions", "Evaluations");
        }

        #endregion

        #region EvaluationSummary

        public ActionResult Summary(string id)
        {
            var currentPageId = Constant.PageId.EvaluationsSummary;
            Logger.LogMessage(string.Format("Entering Controller/Action PageId {0}", currentPageId));

            bool validUserOnPage = this.ValidateUserOnPage(currentPageId);

            if (!validUserOnPage)
            {
                return this.RedirectToUserHomePage();
            }

            NavMenuViewModel menu = this.Menu(currentPageId);

            int? evaluationId = null;

            if (!string.IsNullOrEmpty(id))
            {
                evaluationId = int.Parse(id);
            }

            //set readable sports based on permission
            List<RingMenuItem> allPermittedSports = new List<RingMenuItem>();
            if (menu.Menu.ReadAccess == Constant.AccessLevel.All)
            {
                allPermittedSports = new Common.MenuHierarchy(this.AllSports, new List<Data.Models.Sport> { new Data.Models.Sport { SportID = Constant.SportId.All } }, null, null, this.CurrentLanguageCode(), false).SportMenuItems;
            }
            else if (menu.Menu.ReadAccess == Constant.AccessLevel.Assigned)
            {
                allPermittedSports = menu.Menu.SportMenuItems;
            }

            IList<Models.ListItem> selectedSports = GetSelectedPermittedSports(new SportDetail { Id = Constant.SportId.All, Level = Constant.SportLevelId.All }, allPermittedSports);

            var model = this._evaluationsBL.PopulateEvaluationsSummaryViewModel(this.SelectedYear(), evaluationId, selectedSports);

			this.SetSelectedYear(model.Year);

            model.Menu = menu;
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();
            model.CurrentOlympicYears = this.OlympicYears();

            ViewData["ShowCopyWindow"] = TempData["ShowCopyWindow"] == null ? false : (bool)TempData["ShowCopyWindow"];

            return View(model);
        }

        public ActionResult AddEvaluation(string evaluationVersionId, string sportId)
        {
            var evaluationId = this._evaluationsBL.AddEvaluation(
                int.Parse(evaluationVersionId),
                sportId,
                Constant.EvaluationPhase.Edit,
                null);
            return RedirectToAction("Summary", "Evaluations", new { id = evaluationVersionId });
        }

        public ActionResult CopyEvaluation(string copyToEvaluationVersionId, string copyFromEvaluationVersionId, string evaluationId)
        {
            string redirectToVersion = copyFromEvaluationVersionId;

            if (!string.IsNullOrEmpty(copyToEvaluationVersionId) && !string.IsNullOrEmpty(evaluationId))
            {
                if (this._evaluationsBL.CanCopyEvaluation(int.Parse(evaluationId),
                        int.Parse(copyToEvaluationVersionId), ModelState))
                {
                    var newEvaluationId = this._evaluationsBL.CopyEvaluation(
                        int.Parse(evaluationId),
                        int.Parse(copyToEvaluationVersionId));

                    redirectToVersion = copyToEvaluationVersionId;
                }
                else
                {
                    TempData["ModelState"] = ModelState;
                }
            }

            return RedirectToAction("Summary", "Evaluations", new { id = redirectToVersion });
        }

        #endregion

        #region EvaluationForm

        public ActionResult EvaluationForm(int? id, string spillar)
        {
            string currentPageId = Constant.PageId.EvaluationForm;
            Logger.LogMessage(string.Format("Entering Controller/Action PageId {0}", currentPageId));

            bool validUserOnPage = this.ValidateUserOnPage(currentPageId);
            if (!validUserOnPage)
            {
                return this.RedirectToUserHomePage();
            }

            var model = new EvaluationFormViewModel();
            model.Menu = this.Menu(currentPageId);
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();

            if (id.HasValue)
            {
                var evaluation = this._evaluationsBL.GetEvaluation(id.Value);

                if (evaluation != null && this.ValidateSportOnPage(currentPageId, evaluation.SportId))
                {
                    model.Evaluation = evaluation;

                    model.SelectedTabIndex = GetSelectedTabIndex(model.Evaluation, spillar);

                    model.ToolbarButtons = GetToolbarButtons(model.Menu.Menu.PageMenuItems);

                    //Setup Editable based on security info
                    model.Editable = false;
                    if (model.Menu.Menu.WriteAccess == Constant.AccessLevel.Assigned)
                    {
                        IList<Models.ListItem> permittedSports = GetSelectedPermittedSports(new SportDetail { Id = Constant.SportId.All, Level = Constant.SportLevelId.All }, model.Menu.Menu.SportMenuItems);
                        model.Editable = (from p in permittedSports where p.Id == model.Evaluation.SportId select p).Any();
                    }
                    else if (model.Menu.Menu.WriteAccess == Constant.AccessLevel.All)
                    {
                        model.Editable = true;
                    }

                    model.Editable = model.Editable && evaluation.Phase != Constant.EvaluationPhase.Closed;
                    model.CanEditSportQuestions = model.Editable && evaluation.VersionPhase != Constant.VersionPhase.Closed;

                    model.ChangeToStatus = GetChangeToStatus(model.Evaluation);

                    model.SportTitle = string.Format(" - {0}", model.Evaluation.SportDesc);
                    model.EvaluationTitle = string.Format("{0} - {1} - {2}", model.Evaluation.VersionYearId, model.Evaluation.VersionName, model.Evaluation.PhaseDesc);
                }
            }

            this.PopulateActionItemDecodeList();

            if (TempData.ContainsKey("Errors"))
            {
                ModelState.AddModelError("", TempData["Errors"].ToString());
            }

            return View(model);
        }

        public ActionResult EvaluationFormPillar(int id, string pillarId, bool hasActionItem, bool editable)
        {
            var model = new EvaluationPillarViewModel();

            Evaluation evaluation = this._evaluationsBL.GetEvaluation(id);

            if (evaluation != null && evaluation.Pillars != null)
            {
                model.EvaluationId = evaluation.Id;
                model.SportId = evaluation.SportId;

                model.Pillar = (from p in evaluation.Pillars where p.Id == pillarId select p).FirstOrDefault();

                model.Editable = editable;
                model.HasActionItem = hasActionItem;
                model.InitialStatusEditable = evaluation.Phase == Constant.EvaluationPhase.Edit;
                model.CurrentStatusVisible = evaluation.Phase != Constant.EvaluationPhase.Edit;
                model.CanEditSportQuestions = model.Editable && evaluation.VersionPhase != Constant.VersionPhase.Closed;

                model.ActionItemRatio = this._evaluationsBL.GetActionItemRatioForEvaluationPillar(id, pillarId);
            }

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult ChangeToPhase(int id, string changeFromPhase, string changeToPhase)
        {
            //validation: when submit or close an evluation, ensure all status are entered
            if (changeToPhase == Constant.EvaluationPhase.Submit || changeToPhase == Constant.EvaluationPhase.Closed)
            {
                Evaluation evaluation = this._evaluationsBL.GetEvaluation(id);

                if (evaluation != null && evaluation.Pillars != null)
                {
                    foreach (var pillar in evaluation.Pillars)
                    {
                        if (pillar.Questions == null) continue;

                        var questionWithoutAnswer = (from q in pillar.Questions where q.Id == 0 select q).Any();

                        if (questionWithoutAnswer) ModelState.AddModelError("", string.Format(Shared.SharedStrings.Error_MissingAssessment, pillar.Name));
                    }
                }
            }

            if (ModelState.IsValid)
            {
                this._evaluationsBL.UpdateEvaluationPhase(id, changeFromPhase, changeToPhase);
            }
            else
            {
                TempData["ModelState"] = ModelState;
            }

            return RedirectToAction("EvaluationForm", new { id = id });
        }

        [GridAction]
        public ActionResult GetPillarQuestionsAjax(int id, string pillarId)
        {
            IEnumerable<EvaluationPillar> pillars = this._evaluationsBL.GetEvaluationQuestions(id);

            var pillar = (from p in pillars where p.Id == pillarId select p).FirstOrDefault();

            return View(new GridModel(pillar.Questions));
        }

        private void SavePillarQuestions(int evaluationId, string pillarId, string evaluationSportId, EvaluationQuestion postedModel)
        {
            //Validation
            if (postedModel.NotApplicable)
            {
                postedModel.InitialStatus = null;
                postedModel.CurrentStatus = null;
                postedModel.InitialAssessment = null;
                postedModel.CurrentAssessment = null;
            }
            else
            {
                if (string.IsNullOrEmpty(postedModel.InitialStatus) && string.IsNullOrEmpty(postedModel.CurrentStatus))
                {
                    ModelState.AddValidationMessage("InitialStatus", Shared.SharedStrings.Error_Evaluation_IniStatusRequired);
                    ModelState.AddValidationMessage("CurrentStatus", Shared.SharedStrings.Error_Evaluation_CurStatusRequired);
                }

                if (string.IsNullOrEmpty(postedModel.InitialAssessment) && string.IsNullOrEmpty(postedModel.CurrentAssessment))
                {
                    ModelState.AddValidationMessage("InitialAssessment", Shared.SharedStrings.Error_Evaluation_IniAssRequired);
                    ModelState.AddValidationMessage("CurrentAssessment", Shared.SharedStrings.Error_Evaluation_CurAssRequired);
                }
            }

            if ((postedModel.VersionQuestionId == 0 || postedModel.SportId == evaluationSportId) && string.IsNullOrEmpty(postedModel.Question))
            {
                ModelState.AddValidationMessage("Question", Shared.SharedStrings.Error_Evaluation_QuestionRequired);
            }

            if (ModelState.IsValid)
            {
                this._evaluationsBL.SaveEvaluationQuestion(evaluationId, pillarId, postedModel);
            }

            return;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertPillarQuestionsAjax(int evaluationId, string pillarId, string evaluationSportId)
        {
            var postedModel = new EvaluationQuestion();

            if (TryUpdateModel(postedModel))
            {
                postedModel.Id = 0;

                SavePillarQuestions(evaluationId, pillarId, evaluationSportId, postedModel);
            }

            return GetPillarQuestionsAjax(evaluationId, pillarId);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult UpdatePillarQuestionsAjax(int evaluationId, string pillarId, string evaluationSportId)
        {
            var postedModel = new EvaluationQuestion();

            if (TryUpdateModel(postedModel))
            {
                SavePillarQuestions(evaluationId, pillarId, evaluationSportId, postedModel);
            }

            return GetPillarQuestionsAjax(evaluationId, pillarId);
        }

        [GridAction]
        public ActionResult GetBestPracticesAjax(string pillarId)
        {
            IEnumerable<EvaluationBestPractice> bestPractices = new List<EvaluationBestPractice>();

            if (!(string.IsNullOrEmpty(pillarId)))
            {
                bestPractices = this._evaluationsBL.GetEvaluationBestPractices(pillarId);

                //Retrieve all sports allowed based on permission
                var allPermittedSports = new Common.MenuHierarchy(this.AllSports, this.CurrentUser.Sports, null, null, this.CurrentLanguageCode(), false).FlatSportMenuItems;

                foreach (var bestPractice in bestPractices)
                {
                    bestPractice.Editable = (from s in allPermittedSports where s.Id == bestPractice.SportId select s).Any();
                }
            }

            return View(new GridModel(bestPractices));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertBestPracticesAjax()
        {
            var postedModel = new EvaluationBestPractice();

            if (TryUpdateModel(postedModel))
            {
                this._evaluationsBL.InsertEvaluationBestPractice(postedModel);
            }

            return GetBestPracticesAjax(postedModel.MasterPillarId);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult UpdateBestPracticesAjax()
        {
            var postedModel = new EvaluationBestPractice();

            if (TryUpdateModel(postedModel))
            {
                this._evaluationsBL.UpdateEvaluationBestPractice(postedModel);
            }

            return GetBestPracticesAjax(postedModel.MasterPillarId);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult DeleteBestPracticesAjax(int id)
        {
            string pillarId = null;

            var bestPractice = this._evaluationsBL.GetEvaluationBestPractice(id);

            if (bestPractice != null)
            {
                pillarId = bestPractice.MasterPillarId;
            }

            this._evaluationsBL.DeleteEvaluationBestPractice(id);

            return GetBestPracticesAjax(pillarId);
        }

        private int GetSelectedTabIndex(Evaluation evaluation, string selectedTabName)
        {
            int selectedTabIndex = 0;

            if (evaluation == null || evaluation.Pillars == null) return selectedTabIndex;

            selectedTabIndex = (from p in evaluation.Pillars select p.Id).ToList().IndexOf(selectedTabName);

            if (selectedTabIndex < 0 || selectedTabIndex >= evaluation.Pillars.Count()) selectedTabIndex = 0;

            return selectedTabIndex;
        }

        private void PopulateActionItemDecodeList()
        {
            ViewData["actionPriorities"] = this._decodeBL.GetDecodesByCategory(Constant.DecodeCategoryId.ActionItemPriority);
            ViewData["actionStatus"] = this._decodeBL.GetDecodesByCategory(Constant.DecodeCategoryId.ActionItemStatus);
        }

        private List<string> GetToolbarButtons(List<RingMenuItem> allPermittedPages)
        {
            List<string> buttons = new List<string>();

            if (IsPagePermitted(allPermittedPages, Constant.PageId.EvaluationAction))
            {
                buttons.Add(Constant.PageId.EvaluationAction);
            }

            return buttons;
        }

        private List<EvaluationStatusChangeViewModel> GetChangeToStatus(Evaluation evaluation)
        {
            List<EvaluationStatusChangeViewModel> changeToStatus = new List<EvaluationStatusChangeViewModel>();

            if (evaluation.Phase == Constant.EvaluationPhase.Edit)
            {
                changeToStatus.Add(new EvaluationStatusChangeViewModel { FromStatus = Constant.EvaluationPhase.Edit, ToStatus = Constant.EvaluationPhase.Submit });
            }
            else if (evaluation.Phase == Constant.EvaluationPhase.Submit)
            {
                changeToStatus.Add(new EvaluationStatusChangeViewModel { FromStatus = Constant.EvaluationPhase.Submit, ToStatus = Constant.EvaluationPhase.Edit });
                changeToStatus.Add(new EvaluationStatusChangeViewModel { FromStatus = Constant.EvaluationPhase.Submit, ToStatus = Constant.EvaluationPhase.Closed });
            }

            return changeToStatus;
        }

        #endregion
    }
}
