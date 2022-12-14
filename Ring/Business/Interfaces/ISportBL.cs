using System;
using OTP.Ring.Common;

namespace OTP.Ring.Business
{
    public interface ISportBL
    {
        int AddEventOrDiscipline(string parentSportId,
            string sportCode,
            string name_en,
            string name_fr,
            string comments,
            SportBL.EventOrDiscipline sportLevel,
            bool benchmark,
            int? teamSize,
            bool? medalEvent,
            string gender,
            bool active,
            bool teamSport,
            bool resourceTracking);
        int DeleteEventOrDiscipline(string sportId);
        System.Collections.Generic.List<OTP.Ring.Models.ListItem> GetAllSportsOnSportLevel();
        System.Collections.Generic.List<OTP.Ring.Models.ListItem> GetAllSportsOnSportLevel(string id);
        System.Collections.Generic.List<OTP.Ring.Data.Models.Sport> GetAllSportsToDisciplineLevel();
        System.Collections.Generic.List<OTP.Ring.Data.Models.Sport> GetAllSportsToEventLevel();
        System.Collections.Generic.List<OTP.Ring.Data.Models.Sport> GetAllSportsToSportLevel();
        System.Collections.Generic.List<OTP.Ring.Models.ListItem> GetDisciplinesBySport(string sportId);
        System.Collections.Generic.List<OTP.Ring.Models.ListItem> GetEventsByDiscipline(string sportId);
        System.Collections.Generic.List<OTP.Ring.Models.ListItem> GetResultsEventsByDiscipline(string disciplineId);
        System.Collections.Generic.List<OTP.Ring.Models.ViewModel.MaintenanceSportsGridDisciplineViewModel> GetMaintenanceSportsGridDisciplineViewModel(string parentSportId);
        System.Collections.Generic.List<OTP.Ring.Models.ViewModel.MaintenanceSportsGridEventViewModel> GetMaintenanceSportsGridEventViewModel(string parentSportId);
        System.Collections.Generic.List<OTP.Ring.Models.ViewModel.MaintenanceSportsGridSportViewModel> GetMaintenanceSportsGridSportViewModel(string year, System.Collections.Generic.IList<Models.ListItem> selectedSports);
        OTP.Ring.Data.Models.Sport GetSport(string id);
        void PopulateMaintenanceSportsViewData(System.Web.Mvc.ViewDataDictionary viewData, string selectedSportId);
        int UpdateSport(string sportId,
            string nsoId = Constant.UndefinedString,
            string hpaId = Constant.UndefinedString,
            string name_EN = Constant.UndefinedString,
            string name_FR = Constant.UndefinedString,
            bool? benchmark = null,
            string rankingYear = null,
            bool? medalEvent = null,
            int? teamSize = null,
            string comments = Constant.UndefinedString,
            string rankingCategoryId = Constant.UndefinedString,
            string gender = Constant.UndefinedString,
            bool? active = null,
            string sportCode = Constant.UndefinedString,
            bool? teamSport = null,
            bool? resourceTracking = null);
        System.Collections.Generic.List<int> GetResourceIdsBySport(string sportId);
        OTP.Ring.Common.SportDetail GetValidSportForDisplaySummary(string selectedSportId);
        System.Collections.Generic.List<OTP.Ring.Models.ListItem> GetEvaluationQuestionSportList();
        System.Collections.Generic.List<OTP.Ring.Models.ListItem> GetAllSportsUnderSportLevel();
        System.Collections.Generic.List<OTP.Ring.Models.ListItem> GetReportDesignSportList(System.Collections.Generic.List<RingMenuItem> appliedSports);
        string GetSportNameBySportID(string sportId);
        bool IfExistDescendantSportIDs(string sportId);
    }
}
