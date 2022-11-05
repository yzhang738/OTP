﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OTP.Ring.Common;
using OTP.Ring.Web.Utility;
using OTP.Ring.Models.ViewModel;
using OTP.Ring.Business;
using System.IO;
using System.Data;
using Telerik.Web.Mvc;
using System.ComponentModel;
using System.Diagnostics;

namespace OTP.Ring.Web.Controllers
{
    public class ResultsController : BaseController
    {
        private IDecodeBL _decodeBL;
        private IUserBL _userBL;
        private ISportBL _sportBL;
        private IResultsBL _resultsBL;

        public ResultsController(IDecodeBL decodeBL, IUserBL userBL, ISportBL sportBL, IResultsBL resultsBL)
            : base(decodeBL, userBL, sportBL)
        {
            this._decodeBL = decodeBL;
            this._userBL = userBL;
            this._sportBL = sportBL;
            this._resultsBL = resultsBL;
        }

        public ActionResult Index()
        {
            string currentPageId = Constant.PageId.ResultsIndex;
            Logger.LogMessage(string.Format("Entering Controller/Action PageId {0}", currentPageId));

            bool validUserOnPage = this.ValidateUserOnPage(currentPageId);
            if (!validUserOnPage)
            {
                return this.RedirectToUserHomePage();
            }

            var model = new ResultsIndexViewModel();
            model.Menu = this.Menu(currentPageId);
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();
            model.SummaryPageList = this.GetSummaryPageList(currentPageId);

            return View(model);
        }

        public ActionResult Maintenance()
        {
            string currentPageId = Constant.PageId.ResultsMaintenance;
            Logger.LogMessage(string.Format("Entering Controller/Action PageId {0}", currentPageId));

            bool validUserOnPage = this.ValidateUserOnPage(currentPageId);
            if (!validUserOnPage)
            {
                return this.RedirectToUserHomePage();
            }

            var model = new ResultsMaintenanceViewModel();
            model.Menu = this.Menu(currentPageId);
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();
            model.SummaryPageList = this.GetSummaryPageList(currentPageId);

            return View(model);
        }

        #region LoadResults

        public ActionResult LoadResults(int? id, string refresh)
        {
            string currentPageId = Constant.PageId.ResultsLoadResults;
            Logger.LogMessage(string.Format("Entering Controller/Action PageId {0}", currentPageId));

            bool validUserOnPage = this.ValidateUserOnPage(currentPageId);
            if (!validUserOnPage)
            {
                return this.RedirectToUserHomePage();
            }

            if (id == null)
            {
                return RedirectToAction("ManageResults");
            }

            var model = this._resultsBL.PopulateResultsLoadResultsViewModel((int)id);

            if (model == null)
            {
                return RedirectToAction("ManageResults");
            }

            model.Menu = this.Menu(currentPageId);
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();

            if (TempData.ContainsKey("ResultsErrorsModel"))
            {
                model.ErrorGridViewModel = (List<ResultsLoadResultsErrorGridViewModel>)TempData["ResultsErrorsModel"];
            }

            if (TempData.ContainsKey("Errors"))
            {
                model.Errors = TempData["Errors"].ToString();
            }

            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Upload(int competitionId, int downloadTemplateId, int benchmarkTypeId, string eventDate, string eventDescription, string sportId)
        {
            foreach (string inputTagName in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[inputTagName];
                if (file.ContentLength > 0)
                {
                    try
                    {
                        var processedResults = this._resultsBL.ProcessResultsFile(file.InputStream, competitionId, downloadTemplateId, benchmarkTypeId, DateTime.Parse(eventDate), eventDescription, sportId);
                        TempData.Add("ResultsErrorsModel", processedResults);
                    }
                    catch (EntityCommandExecutionException e)
                    {
                        Logger.LogError(e.ToString());
                        TempData.Add("Errors", Shared.SharedStrings.Error_CannotLoadFile + "<br>" + e.InnerException.Message);
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e.ToString());
                        TempData.Add("Errors", Shared.SharedStrings.Error_CannotLoadFile + "<br>" + e.Message);
                    }
                }
            }

            string refreshKey = DateTime.Now.Ticks.ToString();
            return RedirectToAction("LoadResults", new { id = competitionId });
        }

        public ActionResult GetTemplateFile(string id, string competitionId)
        {
            var templateFileName = "template.xls";
            Stream stream = this._resultsBL.BuildExcelDownloadTemplate(int.Parse(id), int.Parse(competitionId), out templateFileName);
            var result = new FileStreamResult(stream, "application/vnd.ms-excel");
            result.FileDownloadName = templateFileName;

            return result;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetTemplateDescription(string downloadTemplateId)
        {
            var description = "";

            if (!string.IsNullOrEmpty(downloadTemplateId))
            {
                description = this._resultsBL.GetDownloadTemplateDescription(int.Parse(downloadTemplateId));
            }

            return Json(description, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetTemplateURL(string downloadTemplateId)
        {
            var url = "";

            if (!string.IsNullOrEmpty(downloadTemplateId))
            {
                url = this._resultsBL.GetDownloadTemplateURL(int.Parse(downloadTemplateId));
            }

            return Json(url, JsonRequestBehavior.AllowGet);
        }

        #endregion LoadResults

        #region LoadTemplates

        public ActionResult LoadTemplates(string id)
        {
            string currentPageId = Constant.PageId.ResultsLoadTemplates;
            Logger.LogMessage(string.Format("Entering Controller/Action PageId {0}", currentPageId));

            bool validUserOnPage = this.ValidateUserOnPage(currentPageId);
            if (!validUserOnPage)
            {
                return this.RedirectToUserHomePage();
            }

            var model = new ResultsLoadTemplatesViewModel();
            model.Menu = this.Menu(currentPageId);
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();

            model.Editable = model.Menu.Menu.WriteAccess != Constant.AccessLevel.No;

            this._resultsBL.PopulateResultsLoadTemplatesViewData(ViewData, id);

            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertDownloadTemplate(string id)
        {
            var updatedModel = new ResultsLoadTemplateDownloadTemplateGridViewModel();

            if (TryUpdateModel(updatedModel))
            {
                if (!(bool)updatedModel.TeamSport && !(bool)updatedModel.ResourceTracking)
                {
                    ModelState.AddValidationMessage("TeamSport", Shared.ResultsStrings.LoadTemplates_Error_TeamOrResourceTracking);
                }
                else
                {
                    id = this._resultsBL.AddDownloadTemplate(
                        "RESULTS",
                        updatedModel.Name_EN,
                        updatedModel.Name_FR,
                        updatedModel.Instructions,
                        updatedModel.URL,
                        null,
                        (bool)updatedModel.TeamSport,
                        (bool)updatedModel.ResourceTracking).ToString();
                }
            }

            return LoadTemplateDownloadTemplateGridAjax(id);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult UpdateDownloadTemplate(string id)
        {
            var updatedModel = new ResultsLoadTemplateDownloadTemplateGridViewModel();
            updatedModel.DownloadTemplateId = int.Parse(id);

            if (TryUpdateModel(updatedModel))
            {
                int changes = this._resultsBL.UpdateDownloadTemplate(
                    downloadTemplateId: updatedModel.DownloadTemplateId,
                    name_EN: updatedModel.Name_EN,
                    name_FR: updatedModel.Name_FR,
                    instructions: updatedModel.Instructions,
                    url: updatedModel.URL);
            }

            return LoadTemplateDownloadTemplateGridAjax(id);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult DeleteDownloadTemplate(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                int changes = this._resultsBL.DeleteDownloadTemplate(int.Parse(id));
            }

            return LoadTemplateDownloadTemplateGridAjax(id);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult DeleteDownloadTemplateMapping(string id, int downloadTemplateId)
        {
            var key = this._resultsBL.DeleteDownloadTemplateMapping(int.Parse(id));

            return LoadTemplateDownloadTemplateMappingsGridAjax(downloadTemplateId.ToString());
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertDownloadTemplateMapping(string id, int downloadTemplateId)
        {
            var updatedModel = new ResultsLoadTemplateDownloadTemplateMappingsGridViewModel();
            updatedModel.DownloadTemplateId = downloadTemplateId;

            if (TryUpdateModel(updatedModel))
            {
                if (updatedModel.SourceRowOffset <= 0)
                {
                    updatedModel.SourceRowOffset = 1;
                }

                var isValid = true;
                #region Validation

                var validator = this._resultsBL.GetDownloadTemplateMappingValidator(updatedModel.DownloadTemplateId, updatedModel.FieldName);

                if (string.IsNullOrEmpty(updatedModel.SourceColumn) && string.IsNullOrEmpty(updatedModel.SourceDefault))
                {
                    isValid = false;
                    ModelState.AddValidationMessage("SourceDefault", Shared.ResultsStrings.LoadTemplates_Error_NoSourceOrDefault);
                }
                else if (!string.IsNullOrEmpty(updatedModel.SourceColumn) && string.IsNullOrEmpty(updatedModel.SourceColumnName))
                {
                    isValid = false;
                    ModelState.AddValidationMessage("SourceColumnName", Shared.ResultsStrings.LoadTemplates_Error_NoColumnName);
                }

                if ((!string.IsNullOrEmpty(validator.RequiredFormatType) && string.IsNullOrEmpty(updatedModel.ColumnFormat))
                    && (!string.IsNullOrEmpty(updatedModel.SourceDefault) || !string.IsNullOrEmpty(updatedModel.SourceColumn)))
                {
                    isValid = false;
                    ModelState.AddValidationMessage("ColumnFormat", Shared.ResultsStrings.LoadTemplates_Error_NoFormatType);
                }

                #endregion

                if (isValid)
                {
                    var key = this._resultsBL.AddDownloadTemplateMapping(
                        updatedModel.DownloadTemplateId,
                        updatedModel.FieldName,
                        updatedModel.SourceColumnName,
                        updatedModel.SourceColumn,
                        updatedModel.SourceDefault,
                        updatedModel.ColumnFormat,
                        null,
                        updatedModel.TeamMember,
                        updatedModel.SourceRowOffset).ToString();
                }
            }

            return LoadTemplateDownloadTemplateMappingsGridAjax(downloadTemplateId.ToString());
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult UpdateDownloadTemplateMapping(string id, int downloadTemplateId)
        {
            var updatedModel = new ResultsLoadTemplateDownloadTemplateMappingsGridViewModel();
            updatedModel.DownloadTemplateMappingId = int.Parse(id);

            if (TryUpdateModel(updatedModel))
            {
                var isValid = true;
                #region Validation

                var originalMapping = this._resultsBL.GetDownloadTemplateMapping(updatedModel.DownloadTemplateMappingId);
                
                if (originalMapping.Required && string.IsNullOrEmpty(updatedModel.SourceColumnName) && string.IsNullOrEmpty(updatedModel.SourceDefault))
                {
                    isValid = false;
                    ModelState.AddValidationMessage("SourceDefault", Shared.ResultsStrings.LoadTemplates_Error_NoSourceOrDefault);
                }
                else if (!string.IsNullOrEmpty(updatedModel.SourceColumn) && string.IsNullOrEmpty(updatedModel.SourceColumnName))
                {
                    isValid = false;
                    ModelState.AddValidationMessage("SourceColumnName", Shared.ResultsStrings.LoadTemplates_Error_NoColumnName);
                }

                if ((!string.IsNullOrEmpty(originalMapping.RequiredFormatType) && string.IsNullOrEmpty(updatedModel.ColumnFormat))
                    && (!string.IsNullOrEmpty(updatedModel.SourceDefault) || !string.IsNullOrEmpty(updatedModel.SourceColumn)))
                {
                    isValid = false;
                    ModelState.AddValidationMessage("ColumnFormat", Shared.ResultsStrings.LoadTemplates_Error_NoFormatType);
                }

                #endregion

                if (isValid)
                {
                    var key = this._resultsBL.UpdateDownloadTemplateMapping(
                        downloadTemplateMappingId: updatedModel.DownloadTemplateMappingId,
                        sourceColumnName: updatedModel.SourceColumnName,
                        sourceColumn: updatedModel.SourceColumn,
                        columnFormat: updatedModel.ColumnFormat,
                        sourceDefault: updatedModel.SourceDefault,
                        teamMember: updatedModel.TeamMember,
                        sourceRowOffset: updatedModel.SourceRowOffset).ToString();
                }
            }

            return LoadTemplateDownloadTemplateMappingsGridAjax(downloadTemplateId.ToString());
        }

        [GridAction]
        public ActionResult LoadTemplateDownloadTemplateGridAjax(string id)
        {
            var model = this._resultsBL.GetResultsLoadTemplateDownloadTemplateGridViewModel();
            return View(new GridModel(model));
        }

        [GridAction]
        public ActionResult LoadTemplateDownloadTemplateMappingsGridAjax(string downloadTemplateId)
        {
            int id = 0;

            if (string.IsNullOrEmpty(downloadTemplateId))
            {
                id = this._resultsBL.GetFirstDownloadTemplateId();
            }
            else
            {
                id = int.Parse(downloadTemplateId);
            }

            var model = this._resultsBL.GetResultsLoadTemplateDownloadTemplateMappingsGridViewModel(id);

            return View(new GridModel(model));
        }

        #endregion

        #region Summary

        public ActionResult Summary(string summaryFilter)
        {
            string currentPageId = Constant.PageId.ResultsSummary;
            Logger.LogMessage(string.Format("Entering Controller/Action PageId {0}", currentPageId));

            bool validUserOnPage = this.ValidateUserOnPage(currentPageId);
            if (!validUserOnPage)
            {
                return this.RedirectToUserHomePage();
            }

            if (TempData.ContainsKey("RedirectionMessage"))
            {
                ViewData.Add("RedirectionMessage", TempData["RedirectionMessage"]);
            }

            var model = new ResultsSummaryViewModel();
            model.Menu = this.Menu(currentPageId);
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();
            model.CurrentOlympicYears = this.OlympicYears();

            model.Editable = model.Menu.Menu.WriteAccess != Constant.AccessLevel.No;

            SportDetail selectedSport = this._sportBL.GetValidSportForDisplaySummary(this.SelectedSport());
            if (selectedSport != null)
            {
                model.VerifiedSelectedSport = selectedSport.Id;
                model.VerifiedSelectedSportDesc = selectedSport.Name;

                model.SelectorItems = new List<Models.ListItem>() { 
                    new Models.ListItem { Id = Constant.ResultSummaryFilter.All, Description = Shared.SharedStrings.ListItem_All }, 
                    new Models.ListItem { Id = Constant.ResultSummaryFilter.PendingDownload, Description = Shared.SharedStrings.ListItem_PendingDownload }, 
                    new Models.ListItem { Id = Constant.ResultSummaryFilter.PendingApprove, Description = Shared.SharedStrings.ListItem_PendingApproval }, 
                    new Models.ListItem { Id = Constant.ResultSummaryFilter.PendingAll, Description = Shared.SharedStrings.ListItem_PendingAll }};
                model.SelectorValue = Constant.ResultSummaryFilter.All;
                if (summaryFilter == Constant.ResultSummaryFilter.PendingDownload
                    || summaryFilter == Constant.ResultSummaryFilter.PendingApprove
                    || summaryFilter == Constant.ResultSummaryFilter.PendingAll)
                {
                    model.SelectorValue = summaryFilter;
                }
            }

            return View(model);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult GetCompetitionsAjax(GridCommand command, string summaryFilter)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            //Set sport readable based on permission
            string currentPageId = Constant.PageId.ResultsSummary;
            var menu = this.Menu(currentPageId).Menu;

            List<RingMenuItem> appliedSports = new List<RingMenuItem>();
            if (menu.ReadAccess == Constant.AccessLevel.Assigned)
            {
                appliedSports = menu.SportMenuItems;
            }
            else if (menu.ReadAccess == Constant.AccessLevel.All)
            {
                appliedSports = new Common.MenuHierarchy(this.AllSports, new List<Data.Models.Sport> { new Data.Models.Sport { SportID = Constant.SportId.All } }, null, null, this.CurrentLanguageCode(), false).SportMenuItems;
            }

            //competitions are filtered by selected sport in the left sport menu
            SportDetail selectedSport = this._sportBL.GetValidSportForDisplaySummary(this.SelectedSport());
            IList<Models.ListItem> selectedSports = GetSelectedPermittedSports(selectedSport, appliedSports);
            List<RingMenuItem> allPermittedSports = menu.FlatSportMenuItems;

            int total = 0;
            var model = this._resultsBL.GetCompetitions(this.SelectedYear(), selectedSports, allPermittedSports, menu.WriteAccess, command, summaryFilter, out total);

            watch.Stop();
            Logger.LogDiagnostic(string.Format("Method call took {0} milliseconds", watch.ElapsedMilliseconds));

            return View(new GridModel
            {
                Data = model,
                Total = total
            });
        }

        [GridAction]
        public ActionResult GetCompetitionEventsAjax(int competitionId)
        {
            IEnumerable<CompetitionEvent> competitionEvents = this._resultsBL.GetCompetitionEvents(competitionId);

            return View(new GridModel(competitionEvents));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertCompetitionsAjax(GridCommand command, string summaryFilter)
        {
            var postedModel = new Competition();

            if (TryUpdateModel(postedModel))
            {
                //validation
                List<int> competitions = this._resultsBL.GetCompetitionIdsByNameDate(postedModel.Name, postedModel.Date);
                if (competitions != null && competitions.Count != 0)
                {
                    throw new Exception(Shared.SharedStrings.Error_AddExistingCompetiton);
                }

                postedModel.YearId = this.SelectedYear();
                this._resultsBL.InsertCompetition(postedModel);
            }

            return GetCompetitionsAjax(command, summaryFilter);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult UpdateCompetitionsAjax(GridCommand command, string summaryFilter)
        {
            var postedModel = new Competition();

            if (TryUpdateModel(postedModel))
            {
                //validation
                List<int> competitions = this._resultsBL.GetCompetitionIdsByNameDate(postedModel.Name, postedModel.Date);
                if (competitions != null && competitions.Count != 0 && competitions.First() != postedModel.Id)
                {
                    throw new Exception(Shared.SharedStrings.Error_UpdateExistingCompetition);
                }

                postedModel.YearId = this.SelectedYear();
                this._resultsBL.UpdateCompetition(postedModel);
            }

            return GetCompetitionsAjax(command, summaryFilter);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult DeleteCompetitionsAjax(GridCommand command, int id, string summaryFilter)
        {
            //validation
            List<int> competitionEvents = this._resultsBL.GetCompetitionEventIdsByCompetition(id);
            if (competitionEvents != null && competitionEvents.Count != 0)
            {
                throw new Exception(Shared.SharedStrings.Error_DeleteCompetition);
            }

            this._resultsBL.DeleteCompetition(id);

            return GetCompetitionsAjax(command, summaryFilter);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertCompetitionEventsAjax(int competitionId)
        {
            var postedModel = new CompetitionEvent();

            if (TryUpdateModel(postedModel))
            {
                //validation
                List<int> competitionEvents = this._resultsBL.GetCompetitionEventIdsByCompetitionSport(competitionId, postedModel.EventId);
                if (competitionEvents != null && competitionEvents.Count != 0)
                {
                    throw new Exception(Shared.SharedStrings.Error_AddExistingEvent);
                }

                this._resultsBL.InsertCompetitionEvent(competitionId, postedModel);
            }

            return GetCompetitionEventsAjax(competitionId);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult UpdateCompetitionEventsAjax(int competitionId)
        {
            var postedModel = new CompetitionEvent();

            if (TryUpdateModel(postedModel))
            {
                //validation
                List<int> competitionEvents = this._resultsBL.GetCompetitionEventIdsByCompetitionSport(competitionId, postedModel.EventId);
                if (competitionEvents != null && competitionEvents.Count != 0 && competitionEvents.First() != postedModel.Id)
                {
                    throw new Exception(Shared.SharedStrings.Error_UpdateExistingEvent);
                }

                this._resultsBL.UpdateCompetitionEvent(postedModel);
            }

            return GetCompetitionEventsAjax(competitionId);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult DeleteCompetitionEventsAjax(int id, int competitionId)
        {
            this._resultsBL.DeleteCompetitionEvent(id);

            return GetCompetitionEventsAjax(competitionId);
        }

        #endregion Summary

        #region ManageResults

        public ActionResult ManageResults(int? id, string downloadOption, string resultsFilter)
        {
            string currentPageId = Constant.PageId.ResultsManage;
            Logger.LogMessage(string.Format("Entering Controller/Action PageId {0}", currentPageId));

            bool validUserOnPage = this.ValidateUserOnPage(currentPageId);
            if (!validUserOnPage)
            {
                return this.RedirectToUserHomePage();
            }

            //Update result status
            if (downloadOption == Constant.ResultAction.Approve || downloadOption == Constant.ResultAction.Unapprove)
            {
                var errors = this._resultsBL.ChangeCompetitionResultStagingStatus(id.Value, downloadOption, resultsFilter);
                if (errors != null && errors.Count > 0)
                {
                    ModelState.AddModelError("", "Approve All failed.  The following errors occured:"); // TODO:Localize
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }

            //Discard results
            else if (downloadOption == Constant.DownloadOption.Discard)
            {
                this._resultsBL.DiscardCompetitionResults(id.Value, resultsFilter);
                return RedirectToAction("Summary");
            }

            //Post results
            else if (downloadOption == Constant.DownloadOption.Replace)
            {
                List<string> results = this._resultsBL.PostCompetitionResultStaging(id.Value, downloadOption, resultsFilter);
                if (results == null || results.Count == 0)
                {
                    return RedirectToAction("Summary");
                }
                else
                {
                    foreach (var result in results)
                    {
                        ModelState.AddModelError("", Shared.SharedStrings.Error_CannotPostResults + " " + result);
                    }
                }
            }

            //load screen
            var model = this._resultsBL.PopulateResultsManageViewModel(id, resultsFilter);
            model.Menu = this.Menu(currentPageId);
            model.CurrentLanguageCode = this.CurrentLanguageCode();
            model.CurrentUserName = this.CurrentUserName();

            if (!string.IsNullOrEmpty(model.CompetitionSportId) && this.ValidateSportOnPage(currentPageId, model.CompetitionSportId))
            {
                ViewData["competitionId"] = model.CompetitionId;

                //Set editable based on permission
                model.Editable = false;
                if (model.Menu.Menu.WriteAccess == Constant.AccessLevel.All)
                {
                    model.Editable = true;
                }
                else if (model.Menu.Menu.WriteAccess == Constant.AccessLevel.Assigned)
                {
                    List<RingMenuItem> allPermittedSports = model.Menu.Menu.FlatSportMenuItems;
                    model.Editable = (from p in allPermittedSports where p.Id == model.CompetitionSportId select p).Any();
                }
            }

            return View(model);
        }

        public ActionResult UnloadResultStagingResults(int id)
        {
            var result = this._resultsBL.UnloadResultStagingResults(id);
            return RedirectToAction("ManageResults", new { id = id });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ApproveSelectedResults(int[] selectedResults)
        {
            var results = this._resultsBL.ApproveCompetitionResultStagings(selectedResults.ToList());

            return Json(results.Count, JsonRequestBehavior.AllowGet);
        }

        [GridAction]
        public ActionResult GetCompetitionResultsAjax(int competitionId, string resultsFilter)
        {
            if (string.IsNullOrEmpty(resultsFilter)) resultsFilter = Constant.SportId.All;

            var model = this._resultsBL.GetResultsManageResultsStagingGridViewModel(competitionId, resultsFilter);

            return View(new GridModel(model));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertCompetitionResultsAjax(int competitionId, string resultsFilter)
        {
            var updatedModel = new ResultsManageResultsStagingGridViewModel();
            updatedModel.CompetitionId = competitionId;

            if (TryUpdateModel(updatedModel))
            {
                //PerfTime field format validation
                if (!string.IsNullOrEmpty(updatedModel.PerfTimeDesc))
                {
                    TimeSpan ts;
                    if (TimeSpan.TryParse(updatedModel.PerfTimeDesc, out ts))
                    {
                        updatedModel.PerfTime = ts;
                    }
                    else
                    {
                        ModelState.AddValidationMessage("PerfTimeDesc", string.Format(Shared.SharedStrings.Error_InvalidTime, updatedModel.PerfTimeDesc));
                    }
                }

                if (!string.IsNullOrEmpty(updatedModel.PositionDesc))
                {
                    try
                    {
                        updatedModel.Position = this._resultsBL.ConvertPositionDescription(updatedModel.PositionDesc);
                    }
                    catch (ArgumentException e)
                    {
                        ModelState.AddValidationMessage("PositionDesc", e.Message);
                    }
                }

                if (ModelState.IsValid)
                {
                    this._resultsBL.AddResultStaging(
                        updatedModel.Approved,
                        updatedModel.EventId,
                        updatedModel.CompetitionId,
                        updatedModel.EventDate,
                        updatedModel.EventDescription,
                        updatedModel.BenchmarkTypeId,
                        updatedModel.CountryCode,
                        updatedModel.TeamName,
                        updatedModel.Position,
                        updatedModel.PerfValue,
                        updatedModel.PerfTime,
                        null);
                }
            }

            return GetCompetitionResultsAjax(competitionId, resultsFilter);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult UpdateCompetitionResultsAjax(int id, int competitionId, string resultsFilter)
        {
            var updatedModel = new ResultsManageResultsStagingGridViewModel();
            updatedModel.CompetitionId = competitionId;
            updatedModel.ResultStagingId = id;

            if (TryUpdateModel(updatedModel))
            {
                //PerfTime field format validation
                if (!string.IsNullOrEmpty(updatedModel.PerfTimeDesc))
                {
                    TimeSpan ts;
                    if (TimeSpan.TryParse(updatedModel.PerfTimeDesc, out ts))
                    {
                        updatedModel.PerfTime = ts;
                    }
                    else
                    {
                        ModelState.AddValidationMessage("PerfTimeDesc", string.Format(Shared.SharedStrings.Error_InvalidTime, updatedModel.PerfTimeDesc));
                    }
                }

                if (!string.IsNullOrEmpty(updatedModel.PositionDesc))
                {
                    try
                    {
                        updatedModel.Position = this._resultsBL.ConvertPositionDescription(updatedModel.PositionDesc);
                    }
                    catch (ArgumentException e)
                    {
                        ModelState.AddValidationMessage("PositionDesc", e.Message);
                    }
                }

                if (updatedModel.Approved)
                {
                    var errors = this._resultsBL.ValidateResultStagingResultsForApproval(updatedModel.ResultStagingId, null, null);

                    foreach (var error in errors)
                    {
                        ModelState.AddValidationMessage("Approved", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    this._resultsBL.UpdateResultStaging(
                        updatedModel.ResultStagingId,
                        approved: updatedModel.Approved,
                        sportId: updatedModel.EventId,
                        eventDate: updatedModel.EventDate,
                        eventDescription: updatedModel.EventDescription,
                        benchmarkTypeId: updatedModel.BenchmarkTypeId,
                        countryId: updatedModel.CountryCode,
                        position: updatedModel.Position,
                        perfValue: updatedModel.PerfValue,
                        perfTime: updatedModel.PerfTime,
                        teamName: updatedModel.TeamName);
                }
            }

            return GetCompetitionResultsAjax(competitionId, resultsFilter);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult DeleteCompetitionResultsAjax(int id, int competitionId, string resultsFilter)
        {
            this._resultsBL.DeleteCompetitionResultStaging(id);

            return GetCompetitionResultsAjax(competitionId, resultsFilter);
        }

        [GridAction]
        public ActionResult GetCompetitionResultsResourcesAjax(int resultStagingId)
        {
            var model = this._resultsBL.GetResultManageResultsStagingResourceGridViewModel(resultStagingId);
            return View(new GridModel(model));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertResultStagingResource(int id, int resultStagingId)
        {
            var updatedModel = new ResultManageResultsStagingResourceGridViewModel();
            updatedModel.ResultStagingId = resultStagingId;

            if (TryUpdateModel(updatedModel))
            {
                this._resultsBL.AddResultStagingResource(
                    updatedModel.ResultStagingId,
                    updatedModel.SourceRow,
                    updatedModel.TeamMember,
                    updatedModel.SportTrackingRef,
                    updatedModel.DownloadLastName,
                    updatedModel.DownloadFirstName,
                    updatedModel.GenderCode,
                    updatedModel.BirthDate,
                    updatedModel.ResourceId,
                    null);
            }

            return GetCompetitionResultsResourcesAjax(resultStagingId);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult UpdateResultStagingResource(int id, int resultStagingId)
        {
            var updatedModel = new ResultManageResultsStagingResourceGridViewModel();
            updatedModel.ResultStagingId = resultStagingId;
            updatedModel.ResultStagingResourceId = id;

            if (TryUpdateModel(updatedModel))
            {
                this._resultsBL.UpdateResultStagingResource(
                    updatedModel.ResultStagingResourceId,
                    updatedModel.SourceRow,
                    updatedModel.TeamMember,
                    updatedModel.SportTrackingRef,
                    updatedModel.DownloadLastName,
                    updatedModel.DownloadFirstName,
                    updatedModel.GenderCode,
                    updatedModel.BirthDate,
                    updatedModel.ResourceId);
            }

            return GetCompetitionResultsResourcesAjax(resultStagingId);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult DeleteResultStagingResource(int id, int resultStagingId)
        {
            this._resultsBL.DeleteResultStagingResource(id);

            return GetCompetitionResultsResourcesAjax(resultStagingId);
        }

        #endregion ManageResults

        #region AjaxTemplates

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetNextPositionByEvent(int competitionId, string eventId)
        {
            int position = 0;

            if (!string.IsNullOrEmpty(eventId))
            {
                position = this._resultsBL.GetNextPositionByEvent(competitionId, eventId);
            }

            return Json(position, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetSummaryTypesAjax()
        {
            List<Decode> types = new List<Decode>() { new Decode { Id = null, Name = Shared.SharedStrings.ListItem_SelectAType } };
            types.AddRange(this._decodeBL.GetCompetitionTypes());

            return new JsonResult
            {
                Data = new SelectList(types, "Id", "Name")
            };
        }

        [HttpPost]
        public ActionResult GetSummaryBenchmarksAjax()
        {
            List<Decode> types = new List<Decode>() { new Decode { Id = null, Name = Shared.SharedStrings.ListItem_SelectAType } };
            types.AddRange(this._decodeBL.GetBenchmarkTypes());

            return new JsonResult
            {
                Data = new SelectList(types, "Id", "Name")
            };
        }

        [HttpPost]
        public ActionResult GetSportsOnSportLevelAjax(bool? filterByRole)
        {
            filterByRole = filterByRole ?? false;

            List<Models.ListItem> sports = new List<Models.ListItem>() { new Models.ListItem { Id = null, Description = Shared.SharedStrings.ListItem_SelectASport } };

            if (filterByRole.Value)
            {
                var sportHierachy = new Common.MenuHierarchy(this.AllSports, this.CurrentUser.Sports, null, null, this.CurrentLanguageCode(), false);
                sports.AddRange(GetSelectedPermittedSports(new SportDetail { Id = Constant.SportId.All, Level = Constant.SportLevelId.All }, sportHierachy.SportMenuItems));
            }
            else
            {
                sports.AddRange(this._sportBL.GetAllSportsOnSportLevel());
            }

            return new JsonResult
            {
                Data = new SelectList(sports, "Id", "Description")
            };
        }

        [HttpPost]
        public ActionResult GetDisciplinesBySportAjax(string sportId)
        {
            List<Models.ListItem> disciplines = new List<Models.ListItem>() { new Models.ListItem { Id = null, Description = Shared.SharedStrings.ListItem_SelectADiscipline } };
            disciplines.AddRange(this._sportBL.GetDisciplinesBySport(sportId));

            return new JsonResult
            {
                Data = new SelectList(disciplines, "Id", "Description")
            };
        }

        [HttpPost]
        public ActionResult GetEventsByDisciplineAjax(string disciplineId)
        {
            List<Models.ListItem> events = new List<Models.ListItem>() { new Models.ListItem { Id = null, Description = Shared.SharedStrings.ListItem_SelectAnEvent } };
            events.AddRange(this._sportBL.GetEventsByDiscipline(disciplineId));

            return new JsonResult
            {
                Data = new SelectList(events, "Id", "Description")
            };
        }

        [HttpPost]
        public ActionResult GetEventsByCompetitionAjax(int competitionId)
        {
            List<Models.ListItem> events = new List<Models.ListItem>() { new Models.ListItem { Id = null, Description = Shared.SharedStrings.ListItem_SelectAnEvent } };
            events.AddRange(this._resultsBL.GetEventsByCompetition(competitionId));

            return new JsonResult
            {
                Data = new SelectList(events, "Id", "Description")
            };
        }

        [HttpPost]
        public ActionResult GetTeamsAjax(string teamType, string countryCode, string sportId)
        {
            List<Team> teams = new List<Team>();

            if (!(string.IsNullOrEmpty(countryCode) || string.IsNullOrEmpty(sportId)))
            {
                teams = this._resultsBL.GetTeams(teamType, countryCode, sportId);
            }

            List<Models.ListItem> teamList = new List<Models.ListItem>() { new Models.ListItem { Id = null, Description = Shared.SharedStrings.ListItem_SelectATeam } };
            teamList.AddRange((from t in teams select new Models.ListItem { Id = t.Id.ToString(), Description = t.Name }).ToList());

            return new JsonResult
            {
                Data = new SelectList(teamList, "Id", "Description")
            };
        }

        [HttpPost]
        public ActionResult GetAthletesBySportAjax(string sportId, string countryId)
        {
            List<Models.ListItem> resources = new List<Models.ListItem>() { new Models.ListItem { Id = null, Description = Shared.SharedStrings.ListItem_SelectAResource } };
            resources.AddRange(this._resultsBL.GetAthletesBySport(sportId, countryId));

            return new JsonResult
            {
                Data = new SelectList(resources, "Id", "Description")
            };
        }

        #endregion AjaxTemplates
    }
}
