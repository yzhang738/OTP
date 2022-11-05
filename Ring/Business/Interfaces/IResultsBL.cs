using OTP.Ring.Common;
namespace OTP.Ring.Business
{
    public interface IResultsBL
    {
        System.Collections.Generic.List<OTP.Ring.Models.ViewModel.ResultsLoadResultsErrorGridViewModel> ProcessResultsFile(
            System.IO.Stream fileStream,
            int competitionId,
            int downloadTempateId,
            int benchmarkTypeId,
            System.DateTime eventDate,
            string eventDescription,
            string sportId);
        System.Collections.Generic.List<OTP.Ring.Models.ListItem> GetDownloadTemplateList();
        string GetDownloadTemplateDescription(int downloadTemplateId);
        string GetDownloadTemplateURL(int downloadTemplateId);
        System.Collections.Generic.List<OTP.Ring.Models.ListItem> GetCompetitionList(int? competitionId);
        System.IO.Stream BuildExcelDownloadTemplate(int downloadTemplateId, int competitionId, out string templateFileName);
        System.Collections.Generic.IEnumerable<Common.Competition> GetCompetitions(string yearId, System.Collections.Generic.IList<OTP.Ring.Models.ListItem> selectedSports, System.Collections.Generic.List<RingMenuItem> allPermittedSports, int pageWriteAccess, Telerik.Web.Mvc.GridCommand command, string summaryFilter, out int total);
        System.Collections.Generic.IEnumerable<Common.CompetitionEvent> GetCompetitionEvents(int competitionId);
        int InsertCompetition(Common.Competition competition);
        int UpdateCompetition(Common.Competition competition);
        int DeleteCompetition(int competitionId);
        int InsertCompetitionEvent(int competitionId, Common.CompetitionEvent competitionEvent);
        int UpdateCompetitionEvent(Common.CompetitionEvent competitionEvent);
        int DeleteCompetitionEvent(int competitionEventId);
        int DeleteCompetitionResultStaging(int resultStagingId);
        int InsertTeam(Common.Team team);
        int UpdateTeam(Common.Team team);
        int DeleteTeam(int teamId);
        int InsertTeamMember(int teamId, Common.TeamMember teamMember);
        int UpdateTeamMember(Common.TeamMember teamMember);
        int DeleteTeamMember(int teamMemberId);
        System.Collections.Generic.List<string> DiscardCompetitionResults(int competitionId, string eventId);
        System.Collections.Generic.List<string> PostCompetitionResultStaging(int competitionId, string downloadOption, string eventId);
        System.Collections.Generic.List<string> ChangeCompetitionResultStagingStatus(int competitionId, string resultAction, string eventId);
        System.Collections.Generic.List<int> ApproveCompetitionResultStagings(System.Collections.Generic.List<int> resultStagingIds);
        System.Collections.Generic.List<OTP.Ring.Models.ListItem> GetAthletesBySport(string sportId, string countryId);
        int GetNextPositionByEvent(int competitionId, string eventId);
        System.Collections.Generic.List<OTP.Ring.Models.ListItem> GetEventsByCompetition(int competitionId);
        System.Collections.Generic.List<OTP.Ring.Models.ListItem> GetEventsByCompetitionWithStatus(int competitionId);
        System.Collections.Generic.List<Common.Team> GetTeams(string teamType, string countryCode, string sportId);
        System.Collections.Generic.List<Common.TeamMember> GetTeamMembers(int teamId);
        System.Collections.Generic.List<int> GetCompetitionIdsByNameDate(string name, System.DateTime date);
        System.Collections.Generic.List<int> GetCompetitionEventIdsByCompetitionSport(int competitionId, string sportId);
        System.Collections.Generic.List<int> GetCompetitionEventIdsByCompetition(int competitionId);
        System.Collections.Generic.List<OTP.Ring.Models.ListItem> GetTableTemplateList(bool resultsOnly = false);
        System.Collections.Generic.List<OTP.Ring.Models.ViewModel.ResultsManageResultsStagingGridViewModel> GetResultsManageResultsStagingGridViewModel(int competitionId, string eventId);
        void PopulateResultsLoadTemplatesViewData(System.Web.Mvc.ViewDataDictionary viewData, string downloadTemplateId);
        System.Collections.Generic.List<OTP.Ring.Models.ViewModel.ResultsLoadTemplateDownloadTemplateGridViewModel> GetResultsLoadTemplateDownloadTemplateGridViewModel();
        System.Collections.Generic.List<OTP.Ring.Models.ViewModel.ResultsLoadTemplateDownloadTemplateMappingsGridViewModel> GetResultsLoadTemplateDownloadTemplateMappingsGridViewModel(int downloadTemplateId);
        int GetFirstDownloadTemplateId();
        int UpdateDownloadTemplate(
            int downloadTemplateId,
            string name_EN = Constant.UndefinedString,
            string name_FR = Constant.UndefinedString,
            string instructions = Constant.UndefinedString,
            string url = Constant.UndefinedString,
            string comments = Constant.UndefinedString,
            bool? teamSport = null,
            bool? resourceTracking = null);
        int DeleteDownloadTemplate(int downloadTemplateId);
        int AddDownloadTemplate(
            string tableTemplateId,
            string name_EN,
            string name_FR,
            string instructions,
            string url,
            string comments,
            bool teamSport,
            bool resourceTracking);
        System.Collections.Generic.List<OTP.Ring.Models.ListItem> GetTemplateMappingColumnLetterList(int downloadTemplateId, int sourceRowOffset, string currentValue);
        OTP.Ring.Models.ViewModel.ResultsLoadTemplateDownloadTemplateMappingsGridViewModel GetDownloadTemplateMapping(int downloadTemplateMappingId);
        OTP.Ring.Models.ViewModel.ResultsLoadResultsViewModel PopulateResultsLoadResultsViewModel(int competitionId);
        int DeleteDownloadTemplateMapping(int downloadTemplateMappingId);
        int AddDownloadTemplateMapping(
            int downloadTemplateId,
            string fieldName,
            string sourceColumnName,
            string sourceColumn,
            string sourceDefault,
            string columnFormat,
            string comments,
            int teamMember,
            int sourceRowOffset);
        int UpdateDownloadTemplateMapping(
            int downloadTemplateMappingId,
            string sourceColumnName = Constant.UndefinedString,
            string sourceColumn = Constant.UndefinedString,
            string sourceDefault = Constant.UndefinedString,
            string columnFormat = Constant.UndefinedString,
            string comments = Constant.UndefinedString,
            int? teamMember = null,
            int? sourceRowOffset = null);
        OTP.Ring.Models.ViewModel.ResultsLoadTemplateDownloadTemplateMappingsGridViewModel GetDownloadTemplateMappingValidator(int downloadTemplateID, string fieldName);
        System.Collections.Generic.List<OTP.Ring.Models.ListItem> GetTemplateFieldNameList(int downloadTemplateId, string currentFieldName);
        OTP.Ring.Models.ViewModel.ResultsManageViewModel PopulateResultsManageViewModel(int? competitionId, string resultsFilter);
        System.Collections.Generic.List<OTP.Ring.Models.ViewModel.ResultManageResultsStagingResourceGridViewModel> GetResultManageResultsStagingResourceGridViewModel(int resultStagingId);
        int UnloadResultStagingResults(int competitionId);
        System.Collections.Generic.List<OTP.Ring.Models.ListItem> GetResultStagingResourceNameList(int resultStagingId, int? currentResourceId);
        int AddResultStaging(
            bool approved,
            string sportId,
            int competitionId,
            System.DateTime eventDate,
            string eventDescription,
            int benchmarkTypeId,
            string countryId,
            string teamName,
            int position,
            decimal? perfValue,
            System.TimeSpan? perfTime,
            string comments);
        int ConvertPositionDescription(string positionDesc);
        int UpdateResultStaging(
            int resultStagingId,
            bool? approved = null,
            string sportId = Constant.UndefinedString,
            System.DateTime? eventDate = null,
            string eventDescription = Constant.UndefinedString,
            int? benchmarkTypeId = null,
            string countryId = Constant.UndefinedString,
            string teamName = Constant.UndefinedString,
            int? position = null,
            decimal? perfValue = null,
            System.TimeSpan? perfTime = null,
            string comments = null);
        int AddResultStagingResource(
            int resultStagingId,
            int sourceRow,
            int teamMember,
            string sportTrackingRef,
            string downloadLastName,
            string downloadFirstName,
            string gender,
            System.DateTime? birthDate,
            int? resourceId,
            string comments);
        int UpdateResultStagingResource(
            int resultStagingResourceId,
            int? sourceRow = null,
            int? teamMember = null,
            string sportTrackingRef = Constant.UndefinedString,
            string downloadLastName = Constant.UndefinedString,
            string downloadFirstName = Constant.UndefinedString,
            string gender = Constant.UndefinedString,
            System.DateTime? birthDate = null,
            int? resourceId = null,
            string comments = Constant.UndefinedString);
        int DeleteResultStagingResource(int resultStagingResourceId);
        System.Collections.Generic.List<OTP.Ring.Models.ListItem> GetSourceRowNumberList();
        System.Collections.Generic.List<string> ValidateResultStagingResultsForApproval(int? resultStagingId, int? competitionId, string eventId);
    }
}
