using System.Collections.Generic;

namespace OTP.Ring.Business
{
    public interface IDecodeBL
    {
        IEnumerable<OTP.Ring.Common.Decode> GetActionItemTypes();
        List<OTP.Ring.Models.ListItem> GetAffiliateTypes();
        List<OTP.Ring.Models.ListItem> GetAllCountriesList();
        OTP.Ring.Models.ViewModel.MaintenanceAthletesResultsViewModel GetMostRecentOlympicGameInfo(string id);
        List<OTP.Ring.Models.ListItem> GetAllCompetitionTypeList();
        List<OTP.Ring.Models.ListItem> GetAllCompetitionTypeList(string id);
        List<OTP.Ring.Models.ListItem> GetAllCompetitionNameList();
        List<OTP.Ring.Models.ListItem> GetAllHostCityList();
        string GetCurrentYearID();
        string GetCompetitionDateByCompetitionID(string id);
        string GetSportIdFromSportInfos(string id);
        string GetDisciplineIdFromSportInfos(string id);
        string GetEventIdFromSportInfos(string id);
        string GetCategoryFromSportInfos(string id);
        string GetCoachIdFromSportInfos(string id);
        string GetCoachIdFromResources(string id);
        List<OTP.Ring.Common.Organization> GetAllOrganizations();
        List<OTP.Ring.Common.Organization> GetAllOrganizationsByType(string organizationTypeId, bool includeNullItem = false);
        List<OTP.Ring.Data.Models.SecurityPage> GetAllPages();
        List<OTP.Ring.Models.ListItem> GetAllYearsList();
        IEnumerable<OTP.Ring.Common.Decode> GetDecodesByCategory(string categoryId);
        List<OTP.Ring.Models.ListItem> GetDirectAthleteSupportCategoriesByType(string directAthleteSupportTypeId);
        List<OTP.Ring.Models.ListItem> GetDirectAthleteSupportTypes();
        List<OTP.Ring.Models.ListItem> GetGenders(bool includeNull);
        List<OTP.Ring.Models.ListItem> GetHumanGenders(bool includeNull);
        IEnumerable<OTP.Ring.Common.Decode> GetBenchmarkTypes();
        IEnumerable<OTP.Ring.Common.Decode> GetOrganizationTypes();
        IEnumerable<OTP.Ring.Common.Decode> GetCompetitionTypes();
        IEnumerable<OTP.Ring.Common.Decode> GetTeamTypes();
        IEnumerable<OTP.Ring.Common.Decode> GetSpecialResultPositionTypes();
        List<OTP.Ring.Models.ViewModel.SummaryListViewModel> GetSummaryPageList(string currentPageId, OTP.Ring.Common.User user);
        char[] GetColumnLetterArray(int numberofColumns);
        List<OTP.Ring.Models.ListItem> GetColumnFormatList(int downloadTemplateId, string fieldName);
        string GetDiscipline(string sportid);
        string GetGenderCode(string sportid);
        System.IO.MemoryStream CreateExcelFile<T>(dynamic query);
    }
}
