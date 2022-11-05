﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OTP.Ring.Data.Models;
using OTP.Ring.Common;
using OTP.Ring.Models;
using Microsoft.Reporting.WebForms;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using OTP.Ring.Models.ViewModel;
using System.IO;

using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using System.Reflection;
using System.Collections;
using System.Transactions;
using System.Web.Mvc;
using System.Web;
using Telerik.Web.Mvc;
using System.ComponentModel;

namespace OTP.Ring.Business
{
    public class ResultsBL : IResultsBL
    {
        private bool _frenchLanguage = false;
        private Data.Models.RingEntities _repository;
        private const int NumberOfExcelDefaultRows = 0;
        private IDecodeBL _decodeBL;
        private ISportBL _sportBL;

        public ResultsBL(string connectionString, IDecodeBL decodeBL, ISportBL sportBL)
        {
            _frenchLanguage = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToUpper() == Constant.LanguageCode.French;
            _repository = new Data.Models.RingEntities(connectionString);
            this._decodeBL = decodeBL;
            this._sportBL = sportBL;
        }

        public int UnloadResultStagingResults(int competitionId)
        {
            //if results are already posted, and moved to Results table, need to move all results back to ResultStaging table
            int result = 0;

            if (this._repository.ResultStagings.Where(s => s.CompetitionID == competitionId).Count() == 0
                && this._repository.Results.Where(r => r.CompetitionID == competitionId).Count() != 0)
            {
                //move results
                result = this._repository.uspUnloadResultStagingResults(competitionId).SingleOrDefault() ?? 0;
            }

            return result;
        }

        public List<ResultsManageResultsStagingGridViewModel> GetResultsManageResultsStagingGridViewModel(int competitionId, string eventId)
        {
            var competitionExists = (from c in this._repository.Competitions
                                    where c.CompetitionID == competitionId
                                    select c).Any();

            if (!competitionExists)
            {
                return null;
            }

            var model = (from r in this._repository.ResultStagings
                         where r.CompetitionID == competitionId
                         && (eventId == Constant.SportId.All || r.SportID == eventId)
                         select new ResultsManageResultsStagingGridViewModel
                                                  {
                                                      ResultStagingId = r.ResultStagingID,
                                                      DownloadSourceType = r.DownloadSourceType,
                                                      DownloadTemplateId = r.DownloadTemplateID,
                                                      DownloadSourceId = r.DownloadSourceID,
                                                      StatusCode = r.Status,
                                                      StatusDesc = (from d in this._repository.Decodes
                                                                    where d.DecodeCategoryID == Constant.DecodeCategoryId.DownloadStatus
                                                                    && d.DecodeID == r.Status
                                                                    select _frenchLanguage ? d.DecodeID_FR : d.DecodeID_EN).FirstOrDefault(),
                                                      Approved = r.Approved,
                                                      CountryCode = r.CountryID,
                                                      CountryDesc = _frenchLanguage ? r.Country.Name_FR : r.Country.Name_EN,
                                                      Position = r.Position,
                                                      PerfValue = r.PerfValue,
                                                      PerfTime = r.PerfTime,
                                                      HasComments = !string.IsNullOrEmpty(r.Comments),
                                                      BenchmarkTypeId = r.BenchmarkTypeID,
                                                      BenchmarkTypeName = _frenchLanguage ? r.BenchmarkType.Name_FR : r.BenchmarkType.Name_EN,
                                                      EventDate = r.EventDate,
                                                      EventDescription = r.EventDescription,
                                                      EventId = r.SportID,
                                                      EventDesc = (_frenchLanguage ? r.Sport.ParentSport.Name_FR : r.Sport.ParentSport.Name_EN) + " - " + (_frenchLanguage ? r.Sport.Name_FR : r.Sport.Name_EN),
                                                      TeamSport = r.Sport.TeamSport,
                                                      TeamName = r.TeamName
                                                  }).OrderBy(x => x.EventDesc).ThenBy(x => x.Position).ToList();

            //Update PotisionDesc and PerfTimeDesc strings for the entire set
            model = model.Select(x => { x.PositionDesc = (this._repository.ResultPositionMappings.Where(p => p.ResultPositionDecode == x.Position).Select(p => p.ResultPositionValue).SingleOrDefault()) ?? x.Position.ToString(); x.PerfTimeDesc = x.PerfTime.ToString(); return x; }).ToList();

            return model;
        }

        public List<ResultManageResultsStagingResourceGridViewModel> GetResultManageResultsStagingResourceGridViewModel(int resultStagingId)
        {
            var model = (from r in this._repository.ResultStagingResources
                     where r.ResultStagingID == resultStagingId
                     select new ResultManageResultsStagingResourceGridViewModel
                     {
                         BirthDate = r.BirthDate,
                         DownloadFirstName = r.DownloadFirstName,
                         DownloadLastName = r.DownloadLastName,
                         GenderCode = r.Gender,
                         GenderDesc = this._repository.Decodes.Where(d => d.DecodeCategoryID == Constant.DecodeCategoryId.Gender && d.DecodeID == r.Gender).Select(d => _frenchLanguage ? d.DecodeID_FR : d.DecodeID_EN).FirstOrDefault(),
                         HasComments = !string.IsNullOrEmpty(r.Comments),
                         ResourceId = r.ResourceID,
                         ResourceName = r.ResourceID == null
                                        ? CommonLocalization.ListItem_NotSet
                                        : (r.ResourceID == -1
                                            ? "{NEW}" // TODO: Localize this
                                            : this._repository.Resources.Where(r2 => r2.ResourceID == r.ResourceID).Select(r2 => r2.LastName + ", " + r2.FirstName).FirstOrDefault()),
                         ResultStagingId = r.ResultStagingID,
                         ResultStagingResourceId = r.ResultStagingResourceID,
                         SourceRow = r.SourceRow,
                         SportTrackingRef = r.SportTrackingRef,
                         TeamMember = r.TeamMember
                     }).ToList();

            return model;
        }

        /// <summary>
        /// Faster method for retrieving and calculating competitions
        /// </summary>
        public IEnumerable<Common.Competition> GetCompetitionsFast(string yearId, string selectedSportId, int userId, string summaryFilter, int pageWriteAccess, GridCommand command, out int total)
        {
            throw new NotImplementedException("Waiting for SQL procedure to release this into the wild.");
            string sortDirection = null;
            string sortDescriptor = null;

            if (command.SortDescriptors.Count != 0)
            {
                switch (command.SortDescriptors[0].SortDirection)
                {
                    case ListSortDirection.Ascending:
                        sortDirection = "asc";
                        break;
                    case ListSortDirection.Descending:
                        sortDirection = "desc";
                        break;
                }

                sortDescriptor = command.SortDescriptors[0].Member;
            }

            var results = this._repository.uspGetCompetitions(
                yearId,
                selectedSportId,
                userId,
                summaryFilter,
                pageWriteAccess,
                sortDescriptor,
                sortDirection,
                command.Page,
                command.PageSize,
                _frenchLanguage);

            total = 0;
            if (results != null && results.Count() > 0)
            {
                total = results.First().TotalRecords;
            }

            var competitionList = (from r in results
                                   select new Common.Competition
                                   {
                                       ApproveCount = r.ApproveCount,
                                       ApproveStatus = r.ApproveStatus,
                                       BenchmarkCode = r.BenchmarkTypeID,
                                       BenchmarkDesc = r.BenchmarkTypeName,
                                       Comment = r.Comments,
                                       CountryCode = r.CountryID,
                                       CountryDesc = r.CountryName,
                                       Date = r.CompetitionDate,
                                       DownloadCount = r.DownloadCount,
                                       DownloadStatus = r.DownloadStatus,
                                       Editable = (bool)r.Editable,
                                       Id = r.CompetitionID,
                                       Location = r.Location,
                                       Name = r.Name,
                                       SportDesc = r.SportName,
                                       SportId = r.SportID,
                                       Type = r.CompetitionTypeID,
                                       TypeDesc = r.CompetitionTypeName,
                                       YearId = r.YearID
                                   }).ToList();

            return competitionList;
        }

        /// <summary>
        /// Slower linq to entities query to retrieve and calculate competitions.
        /// Uncomment below line when SQL procedure supporting GetCompetitionsFast() is complete
        /// </summary>
        //[Obsolete("Linq to Entities is too slow, use GetCompetitionsFast()")]
        public IEnumerable<Common.Competition> GetCompetitions(string yearId, IList<ListItem> selectedSports, List<RingMenuItem> allPermittedSports, int pageWriteAccess, GridCommand command, string summaryFilter, out int total)
        {
            IEnumerable<string> selectedSportIds = (from a in selectedSports select a.Id);
            IEnumerable<string> allPermittedSportsIds = (from a in allPermittedSports select a.Id);

            DateTime competitionEffectiveDate = DateTime.Now.Date.AddDays(7);

            var dataCompetitions = (from c in this._repository.Competitions
                                    where c.YearID == yearId
                                    && selectedSportIds.Contains(c.SportID)
                                    let totalResults = (from r in this._repository.ResultStagings
                                                        where r.CompetitionID == c.CompetitionID
                                                        select r).Count()
                                    let approvedCount = (from r in this._repository.ResultStagings
                                                         where r.CompetitionID == c.CompetitionID
                                                             && (r.Status == Constant.DownloadStatus.Approved
                                                             || r.Status == Constant.DownloadStatus.Posted)
                                                         select r).Count()
                                    let totalEventCount = (from e in this._repository.CompetitionSports
                                                           where e.CompetitionID == c.CompetitionID
                                                              && !e.Cancelled
                                                           select e).Count()
                                    let posted = (from r in this._repository.Results
                                                  where r.CompetitionID == c.CompetitionID
                                                  select r).Any() && totalResults == 0
                                    let downloadedEventsCount = (from r in this._repository.ResultStagings
                                                                 where r.CompetitionID == c.CompetitionID
                                                                 select r.SportID).Distinct().Count()
                                    let downloadStatus =
                                            c.CompetitionDate <= competitionEffectiveDate
                                            ? (
                                                    totalEventCount == 0
                                                    ? Constant.CompetitionStatus.NoEvents
                                                    : (
                                                        totalResults == 0
                                                        ? (
                                                            posted
                                                            ? Constant.CompetitionStatus.Complete
                                                            : Constant.CompetitionStatus.Pending
                                                          )
                                                        : (
                                                            downloadedEventsCount == totalEventCount
                                                            ? Constant.CompetitionStatus.Complete
                                                            : Constant.CompetitionStatus.Partial
                                                          )
                                                      )
                                                )
                                            : Constant.CompetitionStatus.NotStarted
                                    let approvedStatus =
                                        c.CompetitionDate <= competitionEffectiveDate
                                        ? (
                                            totalEventCount == 0
                                            ? Constant.CompetitionStatus.NoEvents
                                            : (
                                                posted
                                                ? Constant.CompetitionStatus.Posted
                                                : (
                                                    approvedCount == 0
                                                    ? (downloadStatus == Constant.CompetitionStatus.Pending
                                                        ? Constant.CompetitionStatus.NotStarted
                                                        : Constant.CompetitionStatus.Pending
                                                      )
                                                    : (
                                                        downloadStatus == Constant.CompetitionStatus.Partial || approvedCount != totalResults
                                                        ? Constant.CompetitionStatus.Partial
                                                        : Constant.CompetitionStatus.Complete
                                                      )
                                                  )
                                              )
                                            )
                                        : Constant.CompetitionStatus.NotStarted
                                    select new Common.Competition
                                    {
                                        Id = c.CompetitionID,
                                        SportId = c.SportID,
                                        SportDesc = _frenchLanguage ? c.Sport.Name_FR : c.Sport.Name_EN,
                                        Type = c.CompetitionTypeID,
                                        TypeDesc = _frenchLanguage ? c.CompetitionType.Name_FR : c.CompetitionType.Name_EN,
                                        YearId = c.YearID,
                                        Name = c.Name,
                                        Date = c.CompetitionDate,
                                        Location = c.Location,
                                        CountryCode = c.CountryID,
                                        CountryDesc = (from o in this._repository.Countries
                                                       where o.CountryID == c.CountryID
                                                       select _frenchLanguage ? o.Name_FR : o.Name_EN).FirstOrDefault(),
                                        BenchmarkCode = c.BenchmarkTypeID,
                                        BenchmarkDesc = _frenchLanguage ? c.BenchmarkType.Name_FR : c.BenchmarkType.Name_EN,
                                        Comment = c.Comments,
                                        DownloadCount = totalResults,
                                        ApproveCount = approvedCount,
                                        DownloadStatus = downloadStatus,
                                        ApproveStatus = approvedStatus,
                                        Editable = pageWriteAccess == Constant.AccessLevel.All
                                            ? true
                                            : (
                                                pageWriteAccess == Constant.AccessLevel.Assigned
                                                ? allPermittedSportsIds.Contains(c.SportID)
                                                : false
                                              )
                                    });

            //Summary Filter
            if (summaryFilter == Constant.ResultSummaryFilter.PendingAll)
			{
				dataCompetitions = dataCompetitions.Where(c => c.DownloadStatus == Constant.CompetitionStatus.Pending
							|| c.DownloadStatus == Constant.CompetitionStatus.Partial
							|| c.ApproveStatus == Constant.CompetitionStatus.Pending
							|| c.ApproveStatus == Constant.CompetitionStatus.Partial);
			}
			else if (summaryFilter == Constant.ResultSummaryFilter.PendingApprove)
			{
				dataCompetitions = dataCompetitions
					.Where(c => c.ApproveStatus == Constant.CompetitionStatus.Pending
							|| c.ApproveStatus == Constant.CompetitionStatus.Partial);
			}
			else if (summaryFilter == Constant.ResultSummaryFilter.PendingDownload)
			{
				dataCompetitions = dataCompetitions
					.Where(c => c.DownloadStatus == Constant.CompetitionStatus.Pending
							|| c.DownloadStatus == Constant.CompetitionStatus.Partial);
			}

            if (command.FilterDescriptors.Any())
            {
                //TODO: This isn't working
                //modelQuery = modelQuery.Where(ExpressionBuilder.Expression<Common.Competition>(command.FilterDescriptors));
            }

            if (command.SortDescriptors.Count == 0)
            {
                dataCompetitions = dataCompetitions.OrderBy(c => c.Date).ThenBy(c => c.SportDesc);
            }
            else
            {
                // Apply sorting from the Grid
                foreach (SortDescriptor sortDescriptor in command.SortDescriptors)
                {
                    if (sortDescriptor.SortDirection == ListSortDirection.Ascending)
                    {
                        switch (sortDescriptor.Member)
                        {
                            case "SportId":
                                dataCompetitions = dataCompetitions.OrderBy(x => x.SportDesc);
                                break;
                            case "Type":
                                dataCompetitions = dataCompetitions.OrderBy(x => x.TypeDesc);
                                break;
                            case "Name":
                                dataCompetitions = dataCompetitions.OrderBy(x => x.Name);
                                break;
                            case "Date":
                                dataCompetitions = dataCompetitions.OrderBy(x => x.Date);
                                break;
                            case "Location":
                                dataCompetitions = dataCompetitions.OrderBy(x => x.Location);
                                break;
                            case "CountryCode":
                                dataCompetitions = dataCompetitions.OrderBy(x => x.CountryDesc);
                                break;
                            case "BenchmarkCode":
                                dataCompetitions = dataCompetitions.OrderBy(x => x.BenchmarkDesc);
                                break;
                            case "DownloadStatus":
                                dataCompetitions = dataCompetitions.OrderBy(x => x.DownloadStatus);
                                break;
                            case "ApproveStatus":
                                dataCompetitions = dataCompetitions.OrderBy(x => x.ApproveStatus);
                                break;
                        }
                    }
                    else
                    {
                        switch (sortDescriptor.Member)
                        {
                            case "SportId":
                                dataCompetitions = dataCompetitions.OrderByDescending(x => x.SportDesc);
                                break;
                            case "Type":
                                dataCompetitions = dataCompetitions.OrderByDescending(x => x.TypeDesc);
                                break;
                            case "Name":
                                dataCompetitions = dataCompetitions.OrderByDescending(x => x.Name);
                                break;
                            case "Date":
                                dataCompetitions = dataCompetitions.OrderByDescending(x => x.Date);
                                break;
                            case "Location":
                                dataCompetitions = dataCompetitions.OrderByDescending(x => x.Location);
                                break;
                            case "CountryCode":
                                dataCompetitions = dataCompetitions.OrderByDescending(x => x.CountryDesc);
                                break;
                            case "BenchmarkCode":
                                dataCompetitions = dataCompetitions.OrderByDescending(x => x.BenchmarkDesc);
                                break;
                            case "DownloadStatus":
                                dataCompetitions = dataCompetitions.OrderByDescending(x => x.DownloadStatus);
                                break;
                            case "ApproveStatus":
                                dataCompetitions = dataCompetitions.OrderByDescending(x => x.ApproveStatus);
                                break;
                        }
                    }
                }
            }

            //Get the count for UI paging
            total = dataCompetitions.Count();

            //Set the paging size
            if (command.PageSize > 0)
            {
                //Set start index to take records from
                dataCompetitions = dataCompetitions.Skip((command.Page - 1) * command.PageSize);
            }

            //Return only the records needed to fit on the current page
            dataCompetitions = dataCompetitions.Take(command.PageSize);

            return dataCompetitions.ToList();
        }

        public IEnumerable<Common.CompetitionEvent> GetCompetitionEvents(int competitionId)
        {
            List<Data.Models.Sport> events = this._sportBL.GetAllSportsToEventLevel();

            var competitionEvents = (from e in this._repository.CompetitionSports
                      where e.CompetitionID == competitionId
                      select new Common.CompetitionEvent 
                      {
                        Id = e.CompetitionSportID,
                        EventId = e.SportID,
                        FieldSize = e.FieldSize,
                        Cancelled = e.Cancelled 
                      }).ToList();

            foreach (var competitionEvent in competitionEvents)
            {
                Data.Models.Sport sportEvent = (from e in events where e.SportID == competitionEvent.EventId select e).SingleOrDefault();
                if (sportEvent == null) continue;

                competitionEvent.EventDesc = _frenchLanguage ? sportEvent.Name_FR : sportEvent.Name_EN;

                Data.Models.Sport sportDiscipline = (from e in events where e.SportID == sportEvent.ParentSportID select e).SingleOrDefault();
                if (sportDiscipline == null) continue;

                competitionEvent.DisciplineId = sportDiscipline.SportID;
                competitionEvent.DisciplineDesc = _frenchLanguage ? sportDiscipline.Name_FR : sportDiscipline.Name_EN;
            }

            return competitionEvents;
        }

        public int InsertCompetition(Common.Competition competition)
        { 
            int result = 0;

            var output = this._repository.uspAddCompetition(
                competition.SportId,
                competition.YearId,
                competition.Type,
                competition.BenchmarkCode,
                competition.Name,
                competition.Date,
                competition.CountryCode,
                competition.Location,
                competition.Comment).SingleOrDefault();

            result = output == null ? result : (int)output;

            return result;

        }

        public int UpdateCompetition(Common.Competition competition)
        {
            int result = 0;

            var dataCompetitions = (from c in this._repository.Competitions
                                   where c.CompetitionID == competition.Id
                                   select c).ToList();

            if (dataCompetitions != null && dataCompetitions.Count() == 1)
            {
                var dataCompetition = dataCompetitions.First();

                var output = this._repository.uspUpdateCompetition(
                    competition.Id,
                    dataCompetition.SportID,
                    dataCompetition.YearID,
                    competition.Type,
                    competition.BenchmarkCode,
                    competition.Name,
                    competition.Date,
                    competition.CountryCode,
                    competition.Location,
                    dataCompetition.Comments).SingleOrDefault();

                result = output == null ? result : (int)output;

                this._repository.Competitions.MergeOption = System.Data.Objects.MergeOption.OverwriteChanges;
            }

            return result;
        }

        public int DeleteCompetition(int competitionId)
        {
            int result = 0;

            var dataCompetitions = (from c in this._repository.Competitions
                                    where c.CompetitionID == competitionId
                                    select c).ToList();

            if (dataCompetitions != null && dataCompetitions.Count() == 1)
            {
                var output = this._repository.uspDeleteCompetition(competitionId).SingleOrDefault();
                result = output == null ? result : (int)output;
            }

            return result;
        }

        public int InsertCompetitionEvent(int competitionId, Common.CompetitionEvent competitionEvent)
        {
            int result = 0;

            var output = this._repository.uspAddCompetitionEvent(competitionId, competitionEvent.EventId, competitionEvent.FieldSize, competitionEvent.Cancelled, null).SingleOrDefault();

            result = output == null ? result : (int)output;

            return result;
        }

        public int UpdateCompetitionEvent(Common.CompetitionEvent competitionEvent)
        {
            int result = 0;

            var dataCompetitionEvents = (from c in this._repository.CompetitionSports
                                         where c.CompetitionSportID == competitionEvent.Id
                                        select c).ToList();

            if (dataCompetitionEvents != null && dataCompetitionEvents.Count() == 1)
            {
                var dataCompetitionEvent = dataCompetitionEvents.First();

                var output = this._repository.uspUpdateCompetitionEvent(competitionEvent.Id, dataCompetitionEvent.CompetitionID, competitionEvent.EventId, competitionEvent.FieldSize, competitionEvent.Cancelled, null).SingleOrDefault();

                result = output == null ? result : (int)output;

                this._repository.CompetitionSports.MergeOption = System.Data.Objects.MergeOption.OverwriteChanges;
            }

            return result;
        }

        public int DeleteCompetitionEvent(int competitionEventId)
        {
            int result = 0;

            var dataCompetitionEvents = (from c in this._repository.CompetitionSports
                                         where c.CompetitionSportID == competitionEventId
                                         select c).ToList();

            if (dataCompetitionEvents != null && dataCompetitionEvents.Count() == 1)
            {
                var output = this._repository.uspDeleteCompetitionEvent(competitionEventId).SingleOrDefault();
                result = output == null ? result : (int)output;
            }

            return result;
        }

        public List<int> ApproveCompetitionResultStagings(List<int> resultStagingIds)
        {
            List<int> updatedResults = new List<int>();

            if (resultStagingIds == null) return updatedResults;

            foreach (var id in resultStagingIds)
            {
                int result = 0;

                var dataResultStagings = (from c in this._repository.ResultStagings
                                          where c.ResultStagingID == id && c.Status == Constant.DownloadStatus.Pending 
                                          select c).ToList();

                if (dataResultStagings != null && dataResultStagings.Count() == 1)
                {
                    var dataResultStaging = dataResultStagings.First();

                    //var output = this._repository.uspUpdateResultStaging(id,
                    //    dataResultStaging.DownloadSourceType,
                    //    dataResultStaging.DownloadTemplateID,
                    //    dataResultStaging.DownloadSourceID,
                    //    Constant.DownloadStatus.Approved,
                    //    true,
                    //    dataResultStaging.SportID,
                    //    dataResultStaging.CompetitionID,
                    //    //dataResultStaging.SportTrackingRef,
                    //    //dataResultStaging.DownloadLastName,
                    //    //dataResultStaging.DownloadFirstName,
                    //    dataResultStaging.CountryID,
                    //    //dataResultStaging.Gender,
                    //    //dataResultStaging.BirthDate,
                    //    dataResultStaging.TeamID,
                    //    dataResultStaging.Position,
                    //    dataResultStaging.PerfValue,
                    //    dataResultStaging.PerfTime,
                    //    dataResultStaging.Comments,
                    //    dataResultStaging.EventDate,
                    //    dataResultStaging.EventDescription,
                    //    dataResultStaging.BenchmarkTypeID).SingleOrDefault();

                    //result = output == null ? result : (int)output;

                    if (result == 1) updatedResults.Add(id);
                }
            }

            return updatedResults;
        }

        public List<string> DiscardCompetitionResults(int competitionId, string eventId)
        {
            List<string> results = new List<string>();
            int result = 0;

            var output = this._repository.uspDiscardResultStagingResults(eventId, competitionId).SingleOrDefault();
            result = output == null ? result : (int)output;

            return results;
        }

        public List<string> ChangeCompetitionResultStagingStatus(int competitionId, string resultAction, string eventId)
        {
            List<string> errors = new List<string>();
            int result = 0;

            if (resultAction == Constant.ResultAction.Approve)
            {
                errors = ValidateResultStagingResultsForApproval(null, competitionId, eventId);

                if (errors.Count == 0)
                {
                    var output = this._repository.uspUpdateResultStagingStatus(eventId, competitionId, Constant.DownloadStatus.Pending, Constant.DownloadStatus.Approved).SingleOrDefault();
                    result = output == null ? result : (int)output;
                }
            }
            else if (resultAction == Constant.ResultAction.Unapprove)
            {
                var output = this._repository.uspUpdateResultStagingStatus(eventId, competitionId, "ALL", Constant.DownloadStatus.Pending).SingleOrDefault();
                result = output == null ? result : (int)output;
            }

            return errors;
        }

        public List<string> ValidateResultStagingResultsForApproval(int? resultStagingId, int? competitionId, string eventId)
        {
            List<string> errors = new List<string>();

            if (resultStagingId == null && competitionId == null)
            {
                throw new ArgumentException("resultStagingId or competitionId must be specified.");
            }

            if (string.IsNullOrEmpty(eventId))
            {
                eventId = Constant.SportId.All;
            }

            //Validate we can approve

            //Verify Required Fields ….
            //a.	Individual
            //i.	CountryID
            //ii.	Position
            //iii.	FullName or LastName/FirstName (doubles as TeamName)
            //b.	Squad
            //i.	CountryID
            //ii.	Position
            //iii.	TeamName
            //iv.	FullName or LastName/FirstName
            //c.	National
            //i.	Position
            //ii.	CountryID

			var resultStagingList = (from r in this._repository.ResultStagings
									 where ((competitionId != null && r.CompetitionID == competitionId) || (resultStagingId != null && r.ResultStagingID == resultStagingId))
									 && (eventId == Constant.SportId.All || r.SportID == eventId)
                                     && !r.Approved
									 select r).ToList();

            foreach (var staging in resultStagingList)
            {
                string message = "";

                if (staging.Position <= 0)
                {
                    message += ", Missing Position";
                }

                if (string.IsNullOrEmpty(staging.CountryID))
                {
                    message += ", Missing Country";
                }

                if (staging.Sport.ResourceTracking != null && (bool)staging.Sport.ResourceTracking)
                {
                    if (staging.ResultStagingResources.Count == 0)
                    {
                        message += ", Team Missing Resources";
                    }
                    else
                    {
                        var missingResourceInformation = (from rr in staging.ResultStagingResources
                                                          where string.IsNullOrEmpty(rr.DownloadLastName)
                                                          || string.IsNullOrEmpty(rr.DownloadFirstName)
                                                          || string.IsNullOrEmpty(rr.Gender)
                                                          || rr.ResourceID == null
                                                          select rr).Any();

                        if (missingResourceInformation)
                        {
                            message += ", Resource(s) Missing Required Information";
                        }
                    }
                }

                if (staging.Sport.TeamSport != null && (bool)staging.Sport.TeamSport && string.IsNullOrEmpty(staging.TeamName))
                {
                    message += ", Missing Team Name";
                }

                if (!string.IsNullOrEmpty(message))
                {
                    if (resultStagingId == null)
                    {
                        //error message for mass-approve all
                        errors.Add(string.Format("Team {0} at Position {1}: {2}", string.IsNullOrEmpty(staging.TeamName) ? "{Not Set}" : staging.TeamName,
                            (this._repository.ResultPositionMappings.Where(p => p.ResultPositionDecode == staging.Position).Select(p => p.ResultPositionValue).SingleOrDefault()) ?? staging.Position.ToString(),
                            message.Substring(2)));
                    }
                    else
                    {
                        errors.Add(message.Substring(2));
                    }
                }
            }

            return errors;
        }

        //TODO: Localize
        public List<string> PostCompetitionResultStaging(int competitionId, string downloadOption, string eventId)
        {
            List<string> results = new List<string>();

            //Validation
            if (!this._repository.ResultStagings.Where(r => r.CompetitionID == competitionId && (eventId == Constant.SportId.All || r.SportID == eventId)).Select(r => r).Any())
            {
                results.Add("No Transactions"); 
            }
            else
            {
                bool notApprovedResults = this._repository.ResultStagings.Where(r => r.CompetitionID == competitionId && (eventId == Constant.SportId.All || r.SportID == eventId) && !r.Approved).Select(r => r).Any();

                if (notApprovedResults)
                {
                    results.Add("Not All Transactions Approved");
                }
            }

            if (results.Count != 0) return results;

            //Post results
            string dbResult = this._repository.uspLoadResultStagingResults(competitionId, eventId).SingleOrDefault();
            if (!string.IsNullOrEmpty(dbResult))
            {
                results.Add(dbResult);
            }

            return results;
        }

        public int AddResultStaging(
	        bool approved,
	        string sportId,
	        int competitionId,
	        DateTime eventDate,
	        string eventDescription,
	        int benchmarkTypeId,
            string countryId,
	        string teamName,
	        int position,
	        decimal? perfValue,
	        TimeSpan? perfTime,
	        string comments)
        {
            var result = this._repository.uspAddResultStaging(
                Constant.DownloadSourceType.Manual,
                null,
                1,
                approved ? Constant.DownloadStatus.Approved : Constant.DownloadStatus.Pending,
                approved,
                sportId,
                competitionId,
                eventDate,
                eventDescription,
                benchmarkTypeId,
                0,
                countryId,
                teamName,
                null,
                position,
                perfValue,
                perfTime,
                comments).FirstOrDefault();

            return result == null ? 0 : (int)result;
        }

        public int ConvertPositionDescription(string positionDesc)
        {
            IEnumerable<Common.Decode> translations = this._decodeBL.GetSpecialResultPositionTypes();

            var ts = (from t in this._decodeBL.GetSpecialResultPositionTypes() where t.Name == positionDesc.ToUpper() select t.IntId);
            if (ts.Count() == 1)
            {
                return ts.First();
            }
            else
            {
                try
                {
                    return Convert.ToInt32(positionDesc);
                }
                catch
                {
                    throw new ArgumentException("Invalid Position Value"); //TODO: Localization
                }
            }
        }

        public int UpdateResultStaging(
            int resultStagingId,
            bool? approved = null,
            string sportId = Constant.UndefinedString,
            DateTime? eventDate = null,
            string eventDescription = Constant.UndefinedString,
            int? benchmarkTypeId = null,
            string countryId = Constant.UndefinedString,
            string teamName = Constant.UndefinedString,
            int? position = null,
            decimal? perfValue = null,
            TimeSpan? perfTime = null,
            string comments = null)
        {
            var staging = (from r in this._repository.ResultStagings
                           where r.ResultStagingID == resultStagingId
                           select r).SingleOrDefault();

            staging.Approved = approved ?? staging.Approved;
            staging.BenchmarkTypeID = benchmarkTypeId ?? staging.BenchmarkTypeID;
            staging.Comments = comments == Constant.UndefinedString ? staging.Comments : comments;
            staging.CountryID = countryId == Constant.UndefinedString ? staging.CountryID : countryId;
            staging.EventDate = eventDate ?? staging.EventDate;
            staging.EventDescription = eventDescription == Constant.UndefinedString ? staging.EventDescription : eventDescription;
            staging.PerfTime = perfTime;
            staging.PerfValue = perfValue;
            staging.Position = position ?? staging.Position;
            if (approved != null)
            {
                staging.Status = (bool)approved ? Constant.DownloadStatus.Approved : Constant.DownloadStatus.Pending;
            }
            staging.TeamName = teamName == Constant.UndefinedString ? staging.TeamName : teamName;

            var result = this._repository.uspUpdateResultStaging(
                staging.ResultStagingID,
                staging.DownloadSourceType,
                staging.DownloadTemplateID,
                staging.DownloadSourceID,
                staging.Status,
                staging.Approved,
                staging.SportID,
                staging.CompetitionID,
                staging.EventDate,
                staging.EventDescription,
                staging.BenchmarkTypeID,
                staging.SourceRow,
                staging.CountryID,
                staging.TeamName,
                staging.TeamID,
                staging.Position,
                staging.PerfValue,
                staging.PerfTime,
                staging.Comments).FirstOrDefault();

            return result == null ? 0 : (int)result;
        }

        public int DeleteCompetitionResultStaging(int resultStagingId)
        {
            int result = 0;

            var dataResultStagings = (from c in this._repository.ResultStagings
                                      where c.ResultStagingID == resultStagingId
                                      select c).ToList();

            if (dataResultStagings != null && dataResultStagings.Count() == 1)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    var resultStagingResourceIds = (from r in dataResultStagings.First().ResultStagingResources
                                                    select r.ResultStagingResourceID).ToList();

                    bool canContinue = true;
                    foreach (var resultStagingResourceId in resultStagingResourceIds)
                    {
                        var deleted = this._repository.uspDeleteResultStagingResource(resultStagingResourceId).SingleOrDefault();
                        if (deleted == null || deleted <= 0)
                        {
                            canContinue = false;
                        }
                    }

                    if (canContinue)
                    {
                        var output = this._repository.uspDeleteResultStaging(resultStagingId).SingleOrDefault();
                        result = output == null ? result : (int)output;
                    }

                    if (result > 0)
                    {
                        scope.Complete();
                    }
                }
            }

            return result;
        }

        public int InsertTeam(Common.Team team)
        {
            int result = 0;

            var output = this._repository.uspAddTeam(team.Type, team.SportId, team.CountryCode, team.Name, team.Name, team.Comment).SingleOrDefault();

            result = output == null ? result : (int)output;

            return result;
        }

        public int UpdateTeam(Common.Team team)
        {
            int result = 0;

            var dataTeams = (from c in this._repository.Teams
                                         where c.TeamID == team.Id
                                         select c).ToList();

            if (dataTeams != null && dataTeams.Count() == 1)
            {
                var dataTeam = dataTeams.First();

                var output = this._repository.uspUpdateTeam(team.Id, dataTeam.TeamTypeID, dataTeam.SportID, dataTeam.CountryID, team.Name, team.Name, dataTeam.Comments).SingleOrDefault();

                result = output == null ? result : (int)output;

                this._repository.Teams.MergeOption = System.Data.Objects.MergeOption.OverwriteChanges;
            }

            return result;
        }

        public int DeleteTeam(int teamId)
        {
            int result = 0;

            var dataTeams = (from c in this._repository.Teams
                             where c.TeamID == teamId
                             select c).ToList();

            if (dataTeams != null && dataTeams.Count() == 1)
            {
                var output = this._repository.uspDeleteTeam(teamId).SingleOrDefault();
                result = output == null ? result : (int)output;
            }

            return result;
        }

        public int InsertTeamMember(int teamId, Common.TeamMember teamMember)
        {
            int result = 0;

            var output = this._repository.uspAddTeamMember(teamId, teamMember.ResourceId, teamMember.Comment).SingleOrDefault();

            result = output == null ? result : (int)output;

            return result;
        }

        public int UpdateTeamMember(Common.TeamMember teamMember)
        {
            int result = 0;

            var dataTeamMembers = (from c in this._repository.TeamResources
                             where c.TeamResourceID == teamMember.Id 
                             select c).ToList();

            if (dataTeamMembers != null && dataTeamMembers.Count() == 1)
            {
                var dataTeamMember = dataTeamMembers.First();

                var output = this._repository.uspUpdateTeamMember(teamMember.Id, dataTeamMember.TeamID, teamMember.ResourceId, dataTeamMember.Comments).SingleOrDefault();

                result = output == null ? result : (int)output;

                this._repository.TeamResources.MergeOption = System.Data.Objects.MergeOption.OverwriteChanges;
            }

            return result;
        }

        public int DeleteTeamMember(int teamMemberId)
        {
            int result = 0;

            var dataTeamMembers = (from c in this._repository.TeamResources
                                   where c.TeamResourceID == teamMemberId
                                   select c).ToList();

            if (dataTeamMembers != null && dataTeamMembers.Count() == 1)
            {
                var output = this._repository.uspDeleteTeamMember(teamMemberId).SingleOrDefault();
                result = output == null ? result : (int)output;
            }

            return result;
        }

        public int GetNextPositionByEvent(int competitionId, string eventId)
        {
            int result = (from p in this._repository.ResultStagings
                          where p.CompetitionID == competitionId && p.SportID == eventId && p.Position < 900
                          orderby p.Position descending
                          select p.Position).FirstOrDefault();

            return ++ result;
        }

        public List<ListItem> GetEventsByCompetitionWithStatus(int competitionId)
        {
            IEnumerable<Common.CompetitionEvent> competitionEvents = GetCompetitionEvents(competitionId);

            IEnumerable<Common.Decode> downloadStatus = this._decodeBL.GetDecodesByCategory(Constant.DecodeCategoryId.DownloadStatus);
            string pending = (from d in downloadStatus where d.Id == Constant.DownloadStatus.Pending select d.Name).SingleOrDefault();

            List<ListItem> outputEvents = new List<ListItem>();

            foreach(var competitionEvent in competitionEvents)
            {
                string id = competitionEvent.EventId;
                string status = "Cancelled";

                if (!competitionEvent.Cancelled)
                {
                    var results = (from s in this._repository.ResultStagings
                                   where s.SportID == competitionEvent.EventId && s.CompetitionID == competitionId 
                                   select s.Status).ToList();

                    status = (from r in results
                                     join d in downloadStatus on r equals d.Id
                                     orderby d.SortOrder ascending
                                     select d.Name).FirstOrDefault();
                }

                string description = string.Format("{0} - {1} ({2})", competitionEvent.DisciplineDesc, competitionEvent.EventDesc, string.IsNullOrEmpty(status) ? pending : status);

                outputEvents.Add(new ListItem { Id = id, Description = description });
            }

            return outputEvents;
        }

        public List<ListItem> GetEventsByCompetition(int competitionId)
        {
            List<ListItem> competitionEvents = (from s in this._repository.CompetitionSports
                       where s.CompetitionID == competitionId
                       select new ListItem 
                       {
                            Id = s.SportID,
                            Description = _frenchLanguage ? s.Sport.Name_FR: s.Sport.Name_EN
                       }).ToList();


            List<Data.Models.Sport> allSports = this._sportBL.GetAllSportsToEventLevel();

            foreach (var competitionEvent in competitionEvents)
            {
                Data.Models.Sport sportEvent = (from e in allSports where e.SportID == competitionEvent.Id select e).SingleOrDefault();
                if (sportEvent == null) continue;

                competitionEvent.Description = _frenchLanguage ? sportEvent.Name_FR : sportEvent.Name_EN;

                Data.Models.Sport sportDiscipline = (from e in allSports where e.SportID == sportEvent.ParentSportID select e).SingleOrDefault();
                if (sportDiscipline == null) continue;

                competitionEvent.Description = string.Format("{0} - {1}", 
                    _frenchLanguage ? sportDiscipline.Name_FR : sportDiscipline.Name_EN, 
                    competitionEvent.Description);
            }

            competitionEvents.Sort();

            return competitionEvents;
        }

        public List<Common.Team> GetTeams(string teamType, string countryCode, string sportId)
        {
            List<Data.Models.Team> teams = null;

            //In Team table, for Individual sport, SportID is on Sport Level
            //               for National and Squad sports, SportID is on Event Level
            if (string.IsNullOrEmpty(teamType))
            {
                teams = (from t in this._repository.Teams
                         join disciplineSport in this._repository.Sports on t.SportID equals disciplineSport.ParentSportID
                         join eventSport in this._repository.Sports on disciplineSport.SportID equals eventSport.ParentSportID
                         where t.CountryID == countryCode && (eventSport.SportID == sportId || t.SportID == sportId)
                         select t).ToList();
            }
            else if (teamType == Constant.TeamType.Individual)
            {
                teams = (from t in this._repository.Teams
                         join disciplineSport in this._repository.Sports on t.SportID equals disciplineSport.ParentSportID
                         join eventSport in this._repository.Sports on disciplineSport.SportID equals eventSport.ParentSportID
                         where t.CountryID == countryCode && eventSport.SportID == sportId
                         select t).ToList();
            }
            else
            {
                teams = (from t in this._repository.Teams
                         where t.SportID == sportId && t.CountryID == countryCode
                         select t).ToList();
            }

            return (from t in teams
                        select new Common.Team
                        {
                            Id = t.TeamID,
                            Type = t.TeamTypeID,
                            SportId = t.SportID,
                            SportDesc = _frenchLanguage ? t.Sport.Name_FR : t.Sport.Name_EN,
                            CountryCode = t.CountryID,
                            CountryDesc = _frenchLanguage ? t.Country.Name_FR : t.Country.Name_EN,
                            Name = _frenchLanguage ? t.Name_FR : t.Name_EN,
                            Comment = t.Comments
                        }).ToList();
        }

        public List<Common.TeamMember> GetTeamMembers(int teamId)
        {
            var members = (from m in this._repository.TeamResources
                    where m.TeamID == teamId
                    select new 
                    {
                        Id = m.TeamResourceID,
                        ResourceId = m.ResourceID,
                        FirstName = m.Resource.FirstName,
                        LastName = m.Resource.LastName,
                        Comment = m.Comments
                    }).ToList();

            return (from m in members
                    select new Common.TeamMember
                    {
                        Id = m.Id,
                        ResourceId = m.ResourceId,
                        ResourceDesc = m.FirstName + " " + m.LastName + " - " + m.ResourceId.ToString(),
                        Comment = m.Comment
                    }).ToList();
        }

        public List<ListItem> GetAthletesBySport(string sportId, string countryId)
        { 
            //sport id is at event level
            var resources = (from r in this._repository.Resources
                       join rs in this._repository.ResourceSports on r.ResourceID equals rs.ResourceID
                       where rs.SportID == sportId && r.CountryID == countryId 
                       select new
                       {
                           Id = r.ResourceID,
                           FirstName = r.FirstName,
                           LastName = r.LastName
                       }).ToList();

            return (from s in resources
                    select new ListItem { Id = s.Id.ToString(), Description = s.FirstName + " " + s.LastName + " - " + s.Id.ToString() }).ToList();

        }

        public List<int> GetCompetitionIdsByNameDate(string name, DateTime date)
        {
            return (from c in this._repository.Competitions
                    where c.Name == name && c.CompetitionDate == date
                    select c.CompetitionID).ToList();
        }

        public List<int> GetCompetitionEventIdsByCompetitionSport(int competitionId, string sportId)
        {
            return (from c in this._repository.CompetitionSports
                    where c.CompetitionID == competitionId && c.SportID == sportId
                    select c.CompetitionSportID).ToList();
        }

        public List<int> GetCompetitionEventIdsByCompetition(int competitionId)
        {
            return (from c in this._repository.CompetitionSports
                    where c.CompetitionID == competitionId
                    select c.CompetitionSportID).ToList();
        }

        public List<ResultsLoadResultsErrorGridViewModel> ProcessResultsFile(
            Stream fileStream,
            int competitionId,
            int downloadTempateId,
            int benchmarkTypeId,
            DateTime eventDate,
            string eventDescription,
            string sportId)
        {
            Stream writeableStream = new MemoryStream();
            fileStream.CopyTo(writeableStream);
            writeableStream.Position = 0;

            var model = new List<ResultsLoadResultsErrorGridViewModel>();

            try
            {
                //HACK: NPOI converts . to , based on the canadian culture for some reason.
                //Here we switch to US culture for processing the excel file, then switch back.
                var currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
                System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");

                var workbook = new HSSFWorkbook(writeableStream);
                List<DownloadStaging> stagingData = PopulateStagingData(workbook.GetSheetAt(0), competitionId, downloadTempateId, benchmarkTypeId, eventDate, eventDescription, sportId);

                System.Threading.Thread.CurrentThread.CurrentCulture = currentCulture;

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, System.TimeSpan.MaxValue))
                {
                    this._repository.uspDeleteDownloadStaging(downloadTempateId, competitionId).SingleOrDefault();
                    
                    //Serialize the list
                    System.Xml.Serialization.XmlAttributeOverrides overrides = new System.Xml.Serialization.XmlAttributeOverrides();
                    System.Xml.Serialization.XmlAttributes attr = new System.Xml.Serialization.XmlAttributes();
                    attr.XmlRoot = new System.Xml.Serialization.XmlRootAttribute("Root");
                    overrides.Add(typeof(List<DownloadStaging>), attr);

                    System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                    System.Xml.XPath.XPathNavigator nav = xmlDoc.CreateNavigator();
                    using (System.Xml.XmlWriter writer = nav.AppendChild())
                    {
                        System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(List<DownloadStaging>), overrides);
                        ser.Serialize(writer, stagingData);
                    }

                    var result = this._repository.uspAddDownloadStagingBulk(xmlDoc.InnerXml).SingleOrDefault();

                    var errorList = this._repository.uspLoadDownloadStagingResults(downloadTempateId, competitionId).ToList();
                    model = ResultsLoadResultsErrorGridViewModel.GetListFromErrorResults(errorList);

                    scope.Complete();
                }
            }
            catch
            {
                //fall down go boom
                throw;
            }
            finally
            {
                writeableStream.Close();
            }

            return model;
        }

        private string GetCellStringValue(Cell cell)
        {
            var result = "";
            Exception ex = null;

            if (cell.CellType == CellType.FORMULA)
            {
                try
                {
                    result = cell.StringCellValue.Trim();
                }
                catch(Exception e)
                {
                    ex = e;
                }

                if (ex != null)
                {
                    //Couldn't convert the formula cell using StringCellValue.
                    //Try just using ToString()
                    result = cell.ToString().Trim();

                    //Re-throw the exception if we don't have a result by now
                    if (string.IsNullOrEmpty(result) || result == "")
                    {
                        throw ex;
                    }
                }
            }
            else
            {
                result = cell.ToString().Trim();
            }
                
            return string.IsNullOrEmpty(result) ? null : result;
        }

        private int GetColumnIndexByLetter(string letter)
        {
            if (string.IsNullOrEmpty(letter)
                || letter.Length > 1)
            {
                throw new ArgumentException("Argument \"letter\" is null or invalid");
            }

            letter = letter.ToUpper();
            return (int)(letter[0]) - (int)('A');
        }

        private List<DownloadStaging> PopulateStagingData(
            Sheet sheet,
            int competitionId,
            int downloadTemplateId,
            int benchmarkTypeId,
            DateTime eventDate,
            string eventDescription,
            string sportId)
        {
            List<DownloadStaging> stagingDataList = new List<DownloadStaging>();

            var columnLetters = this._decodeBL.GetColumnLetterArray(Constant.MaxExcelColumnsSupported);

            IEnumerator rows = sheet.GetRowEnumerator();

            while (rows.MoveNext())
            {
                Row row = (Row)rows.Current;

                if (row.RowNum == 0)
                {
                    continue;
                }

                var stagingData = new DownloadStaging();
                stagingData.SourceRow = row.RowNum + 1;
                stagingData.CompetitionID = competitionId;
                stagingData.DownloadTemplateID = downloadTemplateId;
                stagingData.BenchmarkTypeID = benchmarkTypeId;
                stagingData.EventDate = eventDate;
                stagingData.EventDescription = eventDescription;
                stagingData.SportID = sportId;

                IEnumerator cells = row.GetCellEnumerator();

                bool hasRowData = false;

                while (cells.MoveNext())
                {
                    Cell cell = (Cell)cells.Current;

                    if (cell.ColumnIndex > Constant.MaxExcelColumnsSupported - 1)
                    {
                        continue;
                    }

                    var cellValue = GetCellStringValue(cell);

                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        hasRowData = true;
                    }

                    PropertyInfo prop = stagingData.GetType().GetProperty("Column" + columnLetters[cell.ColumnIndex].ToString());
                    prop.SetValue(stagingData, cellValue, null);
                }

                if (hasRowData)
                {
                    stagingDataList.Add(stagingData);
                }
            }

            return stagingDataList;
        }

        public Stream BuildExcelDownloadTemplate(int downloadTemplateId, int competitionId, out string templateFileName)
        {
            var columnLetters = this._decodeBL.GetColumnLetterArray(Constant.MaxExcelColumnsSupported);

            var template = (from t in this._repository.DownloadTemplates
                            where t.DownloadTemplateID == downloadTemplateId
                            select t).SingleOrDefault();

            var templateName = _frenchLanguage ? template.Name_FR : template.Name_EN;
            templateFileName = templateName + ".xls";

            HSSFWorkbook workbook = new HSSFWorkbook();

            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "OTP";
            workbook.DocumentSummaryInformation = dsi;

            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = templateName;
            workbook.SummaryInformation = si;

            var templateSheet = workbook.CreateSheet(templateName);
            var instructionSheet = workbook.CreateSheet("Instructions");

            var headerStyle = GetHeaderCellStyle(workbook);
            var detailStyle = GetDetailCellStyle(workbook);

            templateSheet.CreateRow(0).RowStyle = headerStyle; //Header Row

            for (int i = 1; i < NumberOfExcelDefaultRows; i++)
            {
                templateSheet.CreateRow(i);
            }

            bool isSquadTemplate = template.ResourceTracking && template.TeamSport;

            foreach (var mapping in template.DownloadTemplateMappings)
            {
                if (!string.IsNullOrEmpty(mapping.SourceColumn))
                {
                    Cell cell = null;
                    var columnIndex = GetColumnIndexByLetter(mapping.SourceColumn);

                    bool isDetailRow = false;

                    //Can only be a detail row if this is a "Squad" sport / template
                    if (isSquadTemplate)
                    {
                        isDetailRow = (from d in _repository.TableTemplateFields
                                       where d.TableTemplateID == "RESULTS"
                                       && d.FieldName == mapping.FieldName
                                       select d.Detail).FirstOrDefault();
                    }

                    //Create a header row (Detail rows won't write their name on the header
                    //Instead, we will fill in sample detail data below as psuedo-headers
                    if (!isDetailRow && templateSheet.GetRow(0).GetCell(columnIndex) == null)
                    {
                        cell = templateSheet.GetRow(0).CreateCell(columnIndex);
                        cell.SetCellValue(mapping.SourceColumnName);
                        cell.CellStyle = headerStyle;
                    }

                    //Fill in sample Detail Rows for Squad Templates
                    if (isSquadTemplate)
                    {
                        string cellContents = "";

                        if (isDetailRow)
                        {
                            cellContents = string.Format("(Member {0}) {1}", mapping.TeamMember, mapping.SourceColumnName);
                        }
                        else
                        {
                            cellContents = string.Format("(Team) {0}", mapping.SourceColumnName);
                        }

                        if (templateSheet.GetRow(mapping.SourceRowOffset) == null)
                        {
                            templateSheet.CreateRow(mapping.SourceRowOffset);
                        }

                        cell = templateSheet.GetRow(mapping.SourceRowOffset).CreateCell(columnIndex);
                        cell.SetCellValue(cellContents);
                        cell.CellStyle = detailStyle;
                    }

                    if (!string.IsNullOrEmpty(mapping.Comments))
                    {
                        var comment = Microsoft.JScript.GlobalObject.unescape(HttpUtility.HtmlDecode(mapping.Comments));
                        SetCellComment(cell, comment);
                    }
                }
            }

            templateSheet.CreateFreezePane(0, 1, 0, 1);

            AddPicture(instructionSheet, OTP.Ring.Business.BusinessResource.otp_logo, new System.Drawing.Rectangle(0, 0, 7, 4));

            if (!string.IsNullOrEmpty(template.Instructions))
            {
                instructionSheet.CreateRow(6).CreateCell(0).SetCellValue(template.Instructions);
            }

            if (!string.IsNullOrEmpty(template.URL))
            {
                var linkCell = instructionSheet.CreateRow(7).CreateCell(0);
                CreateLinkCell(linkCell, template.URL);
            }

            var stream = new MemoryStream();
            workbook.Write(stream);
            stream.Position = 0;

            return stream;
        }

        private IEnumerable<Common.Competition> ConvertCompetitions(IEnumerable<Data.Models.Competition> dataCompetitions)
        {
            if (dataCompetitions == null)
            {
                return null;
            }

            return (from c in dataCompetitions
                    select new Common.Competition
                    {
                        Id = c.CompetitionID,
                        SportId = c.SportID,
                        SportDesc = _frenchLanguage ? c.Sport.Name_FR : c.Sport.Name_EN,
                        Type = c.CompetitionTypeID,
                        TypeDesc = _frenchLanguage ? c.CompetitionType.Name_FR : c.CompetitionType.Name_EN,
                        YearId = c.YearID,
                        Name = c.Name,
                        Date = c.CompetitionDate,
                        Location = c.Location,
                        CountryCode = c.CountryID,
                        CountryDesc = (from o in this._repository.Countries
                                       where o.CountryID == c.CountryID
                                       select _frenchLanguage ? o.Name_FR : o.Name_EN).SingleOrDefault(),
                        BenchmarkCode = c.BenchmarkTypeID,
                        BenchmarkDesc = _frenchLanguage ? c.BenchmarkType.Name_FR : c.BenchmarkType.Name_EN,
                        Comment = c.Comments
                    }).ToList();
        }

        public List<ListItem> GetDownloadTemplateList()
        {
            var templateList = new List<ListItem>();

            var templates = (from t in this._repository.DownloadTemplates
                             select new
                             {
                                 Id = t.DownloadTemplateID,
                                 Description = _frenchLanguage ? t.Name_FR : t.Name_EN
                             }).OrderBy(t => t.Description).ToList();

            foreach(var template in templates)
            {
                //var mappings = GetResultsLoadTemplateDownloadTemplateMappingsGridViewModel(template.Id);

                bool isValidTemplate = true;

                //bool hasFullName = false;
                //bool hasFirstName = false;
                //bool hasLastName = false;

                //TODO: Fix this
                ////Don't show invalid templates in the list
                //foreach (var mapping in mappings)
                //{
                //    if (!string.IsNullOrEmpty(mapping.SourceColumnName) || !string.IsNullOrEmpty(mapping.SourceDefault))
                //    {
                //        switch (mapping.SourceColumnName)
                //        {
                //            case Constant.TableTemplateField.DownloadFirstName:
                //                hasFirstName = true;
                //                break;
                //            case Constant.TableTemplateField.DownloadFullName:
                //                hasFullName = true;
                //                break;
                //            case Constant.TableTemplateField.DownloadLastName:
                //                hasLastName = true;
                //                break;
                //            default:
                //                break;
                //        }
                //    }

                //    if (mapping.Required && string.IsNullOrEmpty(mapping.SourceColumnName) && string.IsNullOrEmpty(mapping.SourceDefault))
                //    {
                //        isValidTemplate = false;
                //    }
                //    else if (!string.IsNullOrEmpty(mapping.SourceColumn) && string.IsNullOrEmpty(mapping.SourceColumnName))
                //    {
                //        isValidTemplate = false;
                //    }

                //    if ((!string.IsNullOrEmpty(mapping.RequiredFormatType) && string.IsNullOrEmpty(mapping.ColumnFormat))
                //        && (!string.IsNullOrEmpty(mapping.SourceDefault) || !string.IsNullOrEmpty(mapping.SourceColumn)))
                //    {
                //        isValidTemplate = false;
                //    }

                //    if (!isValidTemplate)
                //    {
                //        break;
                //    }
                //}

                //if (isValidTemplate)
                //{
                //    //If a name is entered, full name must be populated, or last name & first name must be entered

                //    if (hasFullName)
                //    {
                //        isValidTemplate = !hasLastName && !hasFirstName;
                //    }
                //    else
                //    {
                //        if((hasFirstName && !hasLastName)
                //            || (!hasFirstName && hasLastName))
                //        {
                //            isValidTemplate = false;
                //        }
                //    }
                //}

                if (isValidTemplate)
                {
                    templateList.Add(new ListItem
                    {
                        Id = template.Id.ToString(),
                        Description = template.Description
                    });
                }
            }

            return templateList;
        }

        public List<ListItem> GetCompetitionList(int? competitionId)
        {
            var competitionList = new List<ListItem>();

            var competitions = (from c in this._repository.Competitions
                                where competitionId == null
                                || (int)competitionId == c.CompetitionID
                                select new
                                {
                                    Id = c.CompetitionID,
                                    Description = (_frenchLanguage ? c.CompetitionType.Name_FR : c.CompetitionType.Name_EN) + " - " + c.Name
                                }).ToList();

            competitionList = (from c in competitions
                            orderby c.Description
                            select new ListItem
                            {
                                Id = c.Id.ToString(),
                                Description = c.Description
                            }).ToList();

            return competitionList;
        }

        public string GetDownloadTemplateDescription(int downloadTemplateId)
        {
            var model = "";

            model = (from t in this._repository.DownloadTemplates
                     where t.DownloadTemplateID == downloadTemplateId
                     select t.Instructions).SingleOrDefault();

            return model;
        }

        public string GetDownloadTemplateURL(int downloadTemplateId)
        {
            var model = "";

            model = (from t in this._repository.DownloadTemplates
                     where t.DownloadTemplateID == downloadTemplateId
                     select t.URL).SingleOrDefault();

            return model;
        }

        public List<ListItem> GetTableTemplateList(bool resultsOnly = false)
        {
            var tableTemplateList = (from t in this._repository.TableTemplates
                                     where !resultsOnly || t.TableName == "Result"
                                     select new ListItem
                                     {
                                         Id = t.TableTemplateID,
                                         Description = t.TableName
                                     }).ToList();

            return tableTemplateList;
        }

        public List<ListItem> GetTemplateMappingColumnLetterList(int downloadTemplateId, int sourceRowOffset, string currentValue)
        {
            var letters = this._decodeBL.GetColumnLetterArray(Constant.MaxExcelColumnsSupported).ToList();

            var usedLetters = (from t in this._repository.DownloadTemplates
                               from m in t.DownloadTemplateMappings
                               where t.DownloadTemplateID == downloadTemplateId
                               && !string.IsNullOrEmpty(m.SourceColumn)
                               && m.SourceRowOffset == sourceRowOffset
                               select m.SourceColumn).ToList();

            var usedLettersArray = (from u in usedLetters select u[0]).ToList();

            var unusedColumnLetters = letters.Except(usedLettersArray);

            var columnList = (from l in unusedColumnLetters
                              select new ListItem
                              {
                                  Id = l.ToString(),
                                  Description = l.ToString()
                              }).ToList();

            if (!string.IsNullOrEmpty(currentValue))
            {
                columnList.Add(new ListItem { Description = currentValue, Id = currentValue });
                columnList.Sort();
            }

            columnList.InsertNullListItem(_frenchLanguage);

            return columnList;
        }

        public int GetFirstDownloadTemplateId()
        {
            return (from d in this._repository.DownloadTemplates
                    orderby d.DownloadTemplateID
                    select d.DownloadTemplateID).FirstOrDefault();
        }

        public void PopulateResultsLoadTemplatesViewData(ViewDataDictionary viewData, string downloadTemplateId)
        {
            var id = GetFirstDownloadTemplateId();

            if (!string.IsNullOrEmpty(downloadTemplateId))
            {
                id = int.Parse(downloadTemplateId);
            }

            viewData["downloadTemplateId"] = id;
            viewData["downloadTemplateName"] = "";
            viewData["teamSport"] = false;

            if (id > 0)
            {
                var downloadTemplate = (from d in this._repository.DownloadTemplates
                                        where d.DownloadTemplateID == id
                                        select d).SingleOrDefault();

                viewData["downloadTemplateName"] = _frenchLanguage ? downloadTemplate.Name_FR : downloadTemplate.Name_EN;
                viewData["teamSport"] = downloadTemplate.TeamSport;
            }

            viewData["templates"] = GetResultsLoadTemplateDownloadTemplateGridViewModel();
            viewData["templateMappings"] = GetResultsLoadTemplateDownloadTemplateMappingsGridViewModel(id);
        }

        public List<ResultsLoadTemplateDownloadTemplateGridViewModel> GetResultsLoadTemplateDownloadTemplateGridViewModel()
        {
            List<ResultsLoadTemplateDownloadTemplateGridViewModel> model = null;

            model = (from t in this._repository.DownloadTemplates
                     select new ResultsLoadTemplateDownloadTemplateGridViewModel
                     {
                         DownloadTemplateId = t.DownloadTemplateID,
                         HasComments = !string.IsNullOrEmpty(t.Comments),
                         Instructions = t.Instructions,
                         Name_EN = t.Name_EN,
                         Name_FR = t.Name_FR,
                         Name = _frenchLanguage ? t.Name_FR : t.Name_EN,
                         TableTemplateId = t.TableTemplateID,
                         TableTemplateName = t.TableTemplate.TableName,
                         URL = t.URL,
                         ResourceTracking = t.ResourceTracking,
                         TeamSport = t.TeamSport
                     }).ToList();

            return model;
        }

        public List<ResultsLoadTemplateDownloadTemplateMappingsGridViewModel> GetResultsLoadTemplateDownloadTemplateMappingsGridViewModel(int downloadTemplateId)
        {
            List<ResultsLoadTemplateDownloadTemplateMappingsGridViewModel> model = new List<ResultsLoadTemplateDownloadTemplateMappingsGridViewModel>();

            var downloadTemplate = (from d in this._repository.DownloadTemplates
                                    where d.DownloadTemplateID == downloadTemplateId
                                    select d).SingleOrDefault();

            if (downloadTemplate != null && downloadTemplate.DownloadTemplateMappings != null)
            {
                foreach (var mapping in downloadTemplate.DownloadTemplateMappings)
                {
                    var field = (from f in mapping.DownloadTemplate.TableTemplate.TableTemplateFields
                                 where f.FieldName == mapping.FieldName
                                 select f).SingleOrDefault();

                    var modelItem = new ResultsLoadTemplateDownloadTemplateMappingsGridViewModel
                    {
                        BusinessKey = field.BusinessKey,
                        DownloadTemplateId = mapping.DownloadTemplateID,
                        DownloadTemplateMappingId = mapping.DownloadTemplateMappingID,
                        FieldName = mapping.FieldName,
                        ReplaceKey = field.ReplaceKey,
                        Detail = field.Detail,
                        Required = downloadTemplate.TeamSport
                                    ? (downloadTemplate.ResourceTracking
                                        ? field.RequiredTR // TeamSport + Resource Tracking
                                        : field.RequiredTNR // TeamSPort + No Resource Tracking
                                      )
                                    : (downloadTemplate.ResourceTracking
                                        ? field.RequiredNTR // Not Team Sport + Resource Tracking
                                        : false // No Either (this never happens)
                                      ),
                        SourceColumn = mapping.SourceColumn,
                        SourceColumnName = mapping.SourceColumnName,
                        SourceDefault = mapping.SourceDefault,
                        HasComments = !string.IsNullOrEmpty(mapping.Comments),
                        ColumnFormat = (mapping.DateFormat ?? mapping.NameFormat) ?? mapping.TimeFormat,
                        RequiredFormatType = field.FormatType,
                        SourceRowOffset = mapping.SourceRowOffset,
                        TeamMember = mapping.TeamMember ?? 0
                    };

                    model.Add(modelItem);
                }
            }

            return model;
        }

        public ResultsLoadTemplateDownloadTemplateMappingsGridViewModel GetDownloadTemplateMappingValidator(int downloadTemplateID, string fieldName)
        {
            var downloadTemplate = (from d in this._repository.DownloadTemplates
                                    where d.DownloadTemplateID == downloadTemplateID
                                    select d).SingleOrDefault();

            var field = (from f in this._repository.TableTemplateFields
                         where f.FieldName == fieldName
                         && downloadTemplate.TableTemplateID == f.TableTemplateID
                         select f).SingleOrDefault();

            var modelItem = new ResultsLoadTemplateDownloadTemplateMappingsGridViewModel
            {
                BusinessKey = field.BusinessKey,
                DownloadTemplateId = downloadTemplateID,
                FieldName = fieldName,
                ReplaceKey = field.ReplaceKey,
                Detail = field.Detail,
                Required = downloadTemplate.TeamSport
                            ? (downloadTemplate.ResourceTracking
                                ? field.RequiredTR // TeamSport + Resource Tracking
                                : field.RequiredTNR // TeamSport + No Resource Tracking
                              )
                            : (downloadTemplate.ResourceTracking
                                ? field.RequiredNTR // Not Team Sport + Resource Tracking
                                : false // No Either (this never happens)
                              ),
                RequiredFormatType = field.FormatType
            };

            return modelItem;
        }

        public ResultsLoadTemplateDownloadTemplateMappingsGridViewModel GetDownloadTemplateMapping(int downloadTemplateMappingId)
        {
            var mapping = (from m in this._repository.DownloadTemplateMappings
                            where m.DownloadTemplateMappingID == downloadTemplateMappingId
                            select m).SingleOrDefault();

            var downloadTemplate = (from d in this._repository.DownloadTemplates
                                    where d.DownloadTemplateID == mapping.DownloadTemplateID
                                    select d).SingleOrDefault();

            var field = (from f in mapping.DownloadTemplate.TableTemplate.TableTemplateFields
                            where f.FieldName == mapping.FieldName
                            select f).SingleOrDefault();

            var modelItem = new ResultsLoadTemplateDownloadTemplateMappingsGridViewModel
            {
                BusinessKey = field.BusinessKey,
                DownloadTemplateId = mapping.DownloadTemplateID,
                DownloadTemplateMappingId = mapping.DownloadTemplateMappingID,
                FieldName = mapping.FieldName,
                ReplaceKey = field.ReplaceKey,
                Detail = field.Detail,
                Required = downloadTemplate.TeamSport
                            ? (downloadTemplate.ResourceTracking
                                ? field.RequiredTR // TeamSport + Resource Tracking
                                : field.RequiredTNR // TeamSPort + No Resource Tracking
                              )
                            : (downloadTemplate.ResourceTracking
                                ? field.RequiredNTR // Not Team Sport + Resource Tracking
                                : false // No Either (this never happens)
                              ),
                SourceColumn = mapping.SourceColumn,
                SourceColumnName = mapping.SourceColumnName,
                SourceDefault = mapping.SourceDefault,
                HasComments = !string.IsNullOrEmpty(mapping.Comments),
                ColumnFormat = (mapping.DateFormat ?? mapping.NameFormat) ?? mapping.TimeFormat,
                RequiredFormatType = field.FormatType,
                SourceRowOffset = mapping.SourceRowOffset,
                TeamMember = mapping.TeamMember ?? 0
            };

            return modelItem;
        }

        public int UpdateDownloadTemplate(
            int downloadTemplateId,
            string name_EN = Constant.UndefinedString,
            string name_FR = Constant.UndefinedString,
		    string instructions = Constant.UndefinedString,
		    string url = Constant.UndefinedString,
		    string comments = Constant.UndefinedString,
            bool? teamSport = null,
            bool? resourceTracking = null)
        {
            int changes = 0;

            var template = (from t in this._repository.DownloadTemplates
                            where t.DownloadTemplateID == downloadTemplateId
                            select t).SingleOrDefault();

            if (template == null)
            {
                throw new ArgumentException("No template found for the supplied downloadTemplateId");
            }

            template.Name_EN = name_EN == Constant.UndefinedString ? template.Name_EN : name_EN;
            template.Name_FR = name_FR == Constant.UndefinedString ? template.Name_FR : name_FR;
            template.Instructions = instructions == Constant.UndefinedString ? template.Instructions : instructions;
            template.TeamSport = teamSport ?? template.TeamSport;
            template.ResourceTracking = resourceTracking ?? template.ResourceTracking;

            if (url != Constant.UndefinedString)
            {
                template.URL = url.ToUrlString();
            }

            template.Comments = comments == Constant.UndefinedString ? template.Comments : comments;

            //Update
            var returnValue = this._repository.uspUpdateDownloadTemplate(
                template.DownloadTemplateID,
                template.Name_EN,
                template.Name_FR,
                template.Instructions,
                template.URL,
                template.Comments,
                template.TeamSport,
                template.ResourceTracking).SingleOrDefault();

            changes = returnValue == null ? 0 : (int)returnValue;

            return changes;
        }

        public int DeleteDownloadTemplate(int downloadTemplateId)
        {
            int changes = 0;

            var template = (from t in this._repository.DownloadTemplates
                            where t.DownloadTemplateID == downloadTemplateId
                            select t).SingleOrDefault();

            if (template == null)
            {
                throw new ArgumentException("No template found for the supplied downloadTemplateId");
            }

            //Update
            var returnValue = this._repository.uspDeleteDownloadTemplate(downloadTemplateId).SingleOrDefault();

            changes = returnValue == null ? 0 : (int)returnValue;

            return changes;
        }

        public int AddDownloadTemplate(
	        string tableTemplateId,
		    string name_EN,
		    string name_FR,
		    string instructions,
		    string url,
		    string comments,
            bool teamSport,
            bool resourceTracking)
        {
            int changes = 0;

            var returnValue = this._repository.uspAddDownloadTemplate(
                tableTemplateId,
                name_EN,
                name_FR,
                instructions,
                url.ToUrlString(),
                comments,
                teamSport,
                resourceTracking).SingleOrDefault();

            changes = returnValue == null ? changes : (int)returnValue;

            return changes;
        }

        public ResultsLoadResultsViewModel PopulateResultsLoadResultsViewModel(int competitionId)
        {
            var model = new ResultsLoadResultsViewModel();

            model.DownloadTemplateList = this.GetDownloadTemplateList();
            model.DownloadTemplateList.InsertNullListItem();

            var dataCompetitions = (from c in this._repository.Competitions
                                    where c.CompetitionID == competitionId
                                    select c).ToList();

            IEnumerable<Common.Competition> competitions = ConvertCompetitions(dataCompetitions);

            if (competitions.Count() == 0)
            {
                return null;
            }

            model.Competition = competitions.First();

            model.EventList = (from e in this._repository.CompetitionSports
                               where e.CompetitionID == competitionId
                               && !e.Cancelled
                               select new ListItem
                               {
                                   Id = e.SportID,
                                   Description = _frenchLanguage ? e.Sport.ParentSport.Name_FR + " - " + e.Sport.Name_FR : e.Sport.ParentSport.Name_EN + " - " + e.Sport.Name_EN
                               }).OrderBy(e => e.Description).ToList();

            var benchmarkList = (from b in this._repository.BenchmarkTypes
                                 select new
                                 {
                                     Id = b.BenchmarkTypeID,
                                     Description = _frenchLanguage ? b.Name_FR : b.Name_EN
                                 }).OrderBy(b => b.Description).ToList();

            model.BenchmarkList = new List<ListItem>();

            for (int i = 0; i < benchmarkList.Count; i++)
            {
                model.BenchmarkList.Add(new ListItem { Id = benchmarkList[i].Id.ToString(), Description = benchmarkList[i].Description });
                if (benchmarkList[i].Id == model.Competition.BenchmarkCode)
                {
                    model.BenchmarkSelectedIndex = i;
                }
            }

            return model;
        }

        public ResultsManageViewModel PopulateResultsManageViewModel(int? competitionId, string resultsFilter)
        {
            var model = new ResultsManageViewModel();
            resultsFilter = resultsFilter ?? "ALL";

            model.SelectorItems = new List<ListItem>();
            //TODO: Localization
            model.SelectorItems.Insert(0, new ListItem { Id = "ALL", Description = "ALL" });

            if (competitionId != null)
            {
                var competition = (from c in this._repository.Competitions
                                   where c.CompetitionID == competitionId
                                   select c).SingleOrDefault();

                if (competition != null)
                {
                    model.SelectorItems.AddRange(GetEventsByCompetitionWithStatus((int)competitionId));
                    model.CompetitionId = (int)competitionId;



                    model.CompetitionSportId = competition.SportID;
                    model.TitleSport = string.Format(" : {0}", _frenchLanguage ? competition.Sport.Name_FR : competition.Sport.Name_EN);
                    model.TitleCompetition = string.Format("{0} - {1}", _frenchLanguage ? competition.CompetitionType.Name_FR : competition.CompetitionType.Name_EN, competition.Name);
                }
            }

            model.SelectorValue = resultsFilter;

            return model;
        }

        public int DeleteDownloadTemplateMapping(int downloadTemplateMappingId)
        {
            int changes = 0;

            var returnValue = this._repository.uspDeleteDownloadTemplateMapping(downloadTemplateMappingId).SingleOrDefault();

            changes = returnValue == null ? 0 : (int)returnValue;

            return changes;
        }

        public int AddDownloadTemplateMapping(
            int downloadTemplateId,
            string fieldName,
            string sourceColumnName,
            string sourceColumn,
            string sourceDefault,
            string columnFormat,
            string comments,
            int teamMember,
            int sourceRowOffset)
        {
            int changes = 0;

            sourceRowOffset = sourceRowOffset <= 0 ? 1 : sourceRowOffset;
            teamMember = teamMember <= 0 ? 1 : teamMember;

            string timeFormat = null;
            string nameFormat = null;
            string dateFormat = null;

            var formatCategory = (from d in this._repository.Decodes
                                  where d.DecodeID == columnFormat
                                  select d.DecodeCategoryID).SingleOrDefault();

            if (formatCategory == Constant.DecodeCategoryId.NameFormat)
            {
                nameFormat = columnFormat;
            }
            else if (formatCategory == Constant.DecodeCategoryId.DateFormat)
            {
                dateFormat = columnFormat;
            }
            else if (formatCategory == Constant.DecodeCategoryId.TimeFormat)
            {
                timeFormat = columnFormat;
            }

            var existingMapping = (from m in this._repository.DownloadTemplateMappings
                                   where m.DownloadTemplateID == downloadTemplateId
                                   && m.FieldName == fieldName
                                   && m.SourceRowOffset == sourceRowOffset
                                   && m.TeamMember == teamMember
                                   select m).Any();

            if(existingMapping)
            {
                //TODO: Localization
                throw new ArgumentException("A mapping for the supplied field already exists on the same row & column.");
            }
            else
            {
                var returnValue = this._repository.uspAddDownloadTemplateMapping(
                    downloadTemplateId,
                    fieldName,
                    sourceColumnName,
                    sourceColumn,
                    sourceDefault,
                    nameFormat,
                    dateFormat,
                    comments,
                    timeFormat,
                    teamMember <= 0 ? 1 : teamMember,
                    sourceRowOffset).SingleOrDefault();

                changes = returnValue == null ? changes : (int)returnValue;
            }

            return changes;
        }

        public int UpdateDownloadTemplateMapping(
            int downloadTemplateMappingId,
            string sourceColumnName = Constant.UndefinedString,
            string sourceColumn = Constant.UndefinedString,
            string sourceDefault = Constant.UndefinedString,
            string columnFormat = Constant.UndefinedString,
            string comments = Constant.UndefinedString,
            int? teamMember = null,
            int? sourceRowOffset = null)
        {
            var mapping = (from m in this._repository.DownloadTemplateMappings
                           where m.DownloadTemplateMappingID == downloadTemplateMappingId
                           select m).SingleOrDefault();

            if (mapping == null)
            {
                throw new ArgumentException("No template mapping found for the supplied downloadTemplateMappingId");
            }

            mapping.SourceColumnName = sourceColumnName == Constant.UndefinedString ? mapping.SourceColumnName : (sourceColumnName == null ? null : sourceColumnName.Trim());
            mapping.SourceColumn = sourceColumn == Constant.UndefinedString ? mapping.SourceColumn : sourceColumn;
            mapping.SourceDefault = sourceDefault == Constant.UndefinedString ? mapping.SourceDefault : (sourceDefault == null ? null : sourceDefault.Trim());
            mapping.Comments = comments == Constant.UndefinedString ? mapping.Comments : comments;
            mapping.TeamMember = teamMember ?? mapping.TeamMember;
            mapping.SourceRowOffset = sourceRowOffset ?? mapping.SourceRowOffset;

            mapping.SourceRowOffset = mapping.SourceRowOffset <= 0 ? 1 : mapping.SourceRowOffset;
            mapping.TeamMember = mapping.TeamMember <= 0 ? 1 : mapping.TeamMember;

            if (columnFormat != Constant.UndefinedString)
            {
                var formatCategory = (from d in this._repository.Decodes
                                      where d.DecodeID == columnFormat
                                      select d.DecodeCategoryID).SingleOrDefault();

                if (formatCategory == Constant.DecodeCategoryId.NameFormat)
                {
                    mapping.TimeFormat = null;
                    mapping.NameFormat = columnFormat;
                    mapping.DateFormat = null;
                }
                else if (formatCategory == Constant.DecodeCategoryId.DateFormat)
                {
                    mapping.TimeFormat = null;
                    mapping.NameFormat = null;
                    mapping.DateFormat = columnFormat;
                }
                else if (formatCategory == Constant.DecodeCategoryId.TimeFormat)
                {
                    mapping.TimeFormat = columnFormat;
                    mapping.NameFormat = null;
                    mapping.DateFormat = null;
                }
                else
                {
                    mapping.TimeFormat = null;
                    mapping.NameFormat = null;
                    mapping.DateFormat = null;
                }
            }

            //Update
            var result = this._repository.uspUpdateDownloadTemplateMapping(
                mapping.DownloadTemplateMappingID,
                mapping.SourceColumnName,
                mapping.SourceColumn,
                mapping.SourceDefault,
                mapping.NameFormat,
                mapping.DateFormat,
                mapping.TimeFormat,
                mapping.Comments,
                mapping.TeamMember,
                mapping.SourceRowOffset).SingleOrDefault();

            var returnValue = mapping.DownloadTemplateID;

            return returnValue;
        }

        public List<ListItem> GetTemplateFieldNameList(int downloadTemplateId, string currentFieldName)
        {
            var fieldList = new List<ListItem>();

            var downloadTemplate = (from d in this._repository.DownloadTemplates
                                   where d.DownloadTemplateID == downloadTemplateId
                                   select d).FirstOrDefault();

            var usedHeaderfields = (from m in downloadTemplate.DownloadTemplateMappings
                                    from f in this._repository.TableTemplateFields
                                    where downloadTemplate.TableTemplateID == f.TableTemplateID
                                    && m.FieldName == f.FieldName
                                    && (string.IsNullOrEmpty(currentFieldName) || currentFieldName != m.FieldName)
                                    && !f.Detail
                                    select m.FieldName).ToList();

            var fields = (from f in this._repository.TableTemplateFields
                          where f.TableTemplateID == downloadTemplate.TableTemplateID
                          && !usedHeaderfields.Contains(f.FieldName)
                          && (downloadTemplate.ResourceTracking || !f.Detail) //No detail rows for non-resource tracking
                          let required = downloadTemplate.TeamSport
                            ? (downloadTemplate.ResourceTracking
                                ? f.RequiredTR // TeamSport + Resource Tracking
                                : f.RequiredTNR // TeamSport + No Resource Tracking
                              )
                            : (downloadTemplate.ResourceTracking
                                ? f.RequiredNTR // Not Team Sport + Resource Tracking
                                : false // No Either (this never happens)
                              )
                          select new ListItem
                          {
                              Id = f.FieldName,
                              Description = f.FieldName + (required ? "*" : "") + (f.Detail ? " (Detail)" : "")
                          }).ToList();

            return fields;
        }

        public List<ListItem> GetResultStagingResourceNameList(int resultStagingId, int? currentResourceId)
        {
            currentResourceId = currentResourceId ?? 0;

            var parentSportId = (from rs in this._repository.ResultStagings
                                 where rs.ResultStagingID == resultStagingId
                                 from s in this._repository.Sports
                                 where s.SportLevelID == Constant.SportLevelId.Sport
                                 from dis in s.ChildSports
                                 from evt in dis.ChildSports
                                 where evt.SportID == rs.SportID
                                 select s.SportID).SingleOrDefault();

            var usedResourceIds = (from r in this._repository.ResultStagingResources
                                   where r.ResultStagingID == resultStagingId
                                   && r.ResourceID != currentResourceId
                                   select r.ResourceID).ToList();

                                     //&& rs.YearID == rstage.Competition.YearID

            var avaliableResources =
                                    (from s in this._repository.Sports
                                     where s.SportID == parentSportId
                                     from dis in s.ChildSports
                                     from evt in dis.ChildSports
                                     from rstage in this._repository.ResultStagings
                                     where rstage.ResultStagingID == resultStagingId
                                     from r in this._repository.Resources
                                     where r.ResourceTypeID == Constant.ResourceType.Athletes
                                     from rs in this._repository.ResourceSports
                                     where r.ResourceID == rs.ResourceID
                                     && rs.SportID == evt.SportID
                                     && r.CountryID == rstage.CountryID
                                     && !usedResourceIds.Contains(r.ResourceID)
                                     select new
                                     {
                                         LastName = r.LastName,
                                         FirstName = r.FirstName,
                                         Id = r.ResourceID
                                     }).Distinct().ToList();

            var model = (from r in avaliableResources
                         orderby r.LastName, r.FirstName, r.Id
                         select new ListItem
                         {
                             Id = r.Id.ToString(),
                             Description = string.Format("[{0}] {1}, {2}", r.Id, r.LastName, r.FirstName)
                         }).ToList();

            model.Insert(0, new ListItem{ Id = "-1", Description = "{NEW}"}); // TODO: Localization
            model.InsertNullListItem(_frenchLanguage);

            return model;
        }

        public int AddResultStagingResource(
            int resultStagingId,
            int sourceRow,
            int teamMember,
            string sportTrackingRef,
            string downloadLastName,
            string downloadFirstName,
            string gender,
            DateTime? birthDate,
            int? resourceId,
            string comments)
        {
            var result = this._repository.uspAddResultStagingResource(
                resultStagingId,
                sourceRow,
                teamMember,
                sportTrackingRef,
                downloadLastName,
                downloadFirstName,
                gender,
                birthDate,
                resourceId,
                comments).FirstOrDefault();

            return result == null ? 0 : (int)result;
        }

        public int UpdateResultStagingResource(
            int resultStagingResourceId,
            int? sourceRow = null,
            int? teamMember = null,
            string sportTrackingRef = Constant.UndefinedString,
            string downloadLastName = Constant.UndefinedString,
            string downloadFirstName = Constant.UndefinedString,
            string gender = Constant.UndefinedString,
            DateTime? birthDate = null,
            int? resourceId = null,
            string comments = Constant.UndefinedString)
        {
            var resource = (from r in this._repository.ResultStagingResources
                            where r.ResultStagingResourceID == resultStagingResourceId
                            select r).SingleOrDefault();

            resource.SourceRow = sourceRow ?? resource.SourceRow;
            resource.TeamMember = teamMember ?? resource.TeamMember;
            resource.SportTrackingRef = sportTrackingRef == Constant.UndefinedString ? resource.SportTrackingRef : sportTrackingRef;
            resource.DownloadLastName = downloadLastName == Constant.UndefinedString ? resource.DownloadLastName : downloadLastName;
            resource.DownloadFirstName = downloadFirstName == Constant.UndefinedString ? resource.DownloadFirstName : downloadFirstName;
            resource.Gender = gender == Constant.UndefinedString ? resource.Gender : gender;
            resource.BirthDate = birthDate ?? resource.BirthDate;
            resource.ResourceID = resourceId;
            resource.Comments = comments == Constant.UndefinedString ? resource.Comments : comments;

            var result = this._repository.uspUpdateResultStagingResource(
                resource.ResultStagingResourceID,
                resource.SourceRow,
                resource.TeamMember,
                resource.SportTrackingRef,
                resource.DownloadLastName,
                resource.DownloadFirstName,
                resource.Gender,
                resource.BirthDate,
                resource.ResourceID,
                resource.Comments).FirstOrDefault();

            return result == null ? 0 : (int)result;
        }

        public int DeleteResultStagingResource(int resultStagingResourceId)
        {
            int result = 0;

            var output = this._repository.uspDeleteResultStagingResource(resultStagingResourceId).SingleOrDefault();
            result = output == null ? result : (int)output;

            return result;
        }

        public List<ListItem> GetSourceRowNumberList()
        {
            var model = new List<ListItem>();
            for (int i = 1; i <= 20; i++)
            {
                model.Add(new ListItem { Id = i.ToString(), Description = i.ToString() });
            }

            return model;
        }

        #region NPOI Helper Methods

        private void CreateLinkCell(Cell cell, string url, string caption = "")
        {
            if (string.IsNullOrEmpty(caption))
            {
                caption = url;
            }

            ////cell style for hyperlinks
            ////by default hyperlinks are blue and underlined
            CellStyle linkStyle = cell.Sheet.Workbook.CreateCellStyle();
            Font linkFont = cell.Sheet.Workbook.CreateFont();
            linkFont.Underline = (byte)FontUnderlineType.SINGLE;
            linkFont.Color = NPOI.HSSF.Util.HSSFColor.BLUE.index;
            linkStyle.SetFont(linkFont);

            cell.SetCellValue(caption);
            HSSFHyperlink link = new HSSFHyperlink(HyperlinkType.URL);
            link.Address = (url);
            cell.Hyperlink = (link);
            cell.CellStyle = (linkStyle);
        }

        /// <summary>
        /// Adds an image to the excel sheet at the specified row/column
        /// </summary>
        /// <param name="image">png only</param>
        /// <param name="position">x, y, width, height are represented in cells</param>
        private void AddPicture(Sheet sheet, System.Drawing.Bitmap image, System.Drawing.Rectangle position)
        {
            HSSFPatriarch patriarch = (HSSFPatriarch)sheet.CreateDrawingPatriarch();

            //create the anchor
            HSSFClientAnchor anchor;
            anchor = new HSSFClientAnchor(0, 0, 0, 255, position.Left, position.Top, position.Right, position.Bottom);
            anchor.AnchorType = 2;

            //load the picture and get the picture index in the workbook
            var picStream = new MemoryStream();
            image.Save(picStream, System.Drawing.Imaging.ImageFormat.Png);
            picStream.Position = 0;
            byte[] buffer = new byte[picStream.Length];
            picStream.Read(buffer, 0, (int)picStream.Length);
            var pictureIndex = sheet.Workbook.AddPicture(buffer, PictureType.PNG);

            HSSFPicture picture = (HSSFPicture)patriarch.CreatePicture(anchor, pictureIndex);
            //Reset the image to the original size.
            picture.Resize();
            picture.LineStyle = HSSFPicture.LINESTYLE_SOLID;
        }

        private void SetCellComment(Cell cell, string comment)
        {
            // Create the drawing patriarch. This is the top level container for all shapes including cell comments.
            HSSFPatriarch patr = (HSSFPatriarch)cell.Sheet.CreateDrawingPatriarch();

            //anchor defines size and position of the comment in worksheet
            Comment comment1 = patr.CreateCellComment(new HSSFClientAnchor(0, 0, 0, 0, cell.RowIndex, cell.ColumnIndex, 6, 5));

            // set text in the comment
            comment1.String = (new HSSFRichTextString(comment));

            cell.CellComment = (comment1);
        }

        private CellStyle GetHeaderCellStyle(Workbook workbook)
        {
            var headerStyle = workbook.CreateCellStyle();
            headerStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GOLD.index;
            headerStyle.FillPattern = FillPatternType.SOLID_FOREGROUND;

            Font font = workbook.CreateFont();
            font.Boldweight = (short)FontBoldWeight.BOLD;
            font.FontHeightInPoints = 12;

            headerStyle.SetFont(font);
            return headerStyle;
        }

        private CellStyle GetDetailCellStyle(Workbook workbook)
        {
            var headerStyle = workbook.CreateCellStyle();

            Font font = workbook.CreateFont();
            font.IsItalic = true;

            headerStyle.SetFont(font);
            return headerStyle;
        }

        #endregion
    }
}
