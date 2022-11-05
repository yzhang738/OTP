using System;
using OTP.Ring.Common;
namespace OTP.Ring.Business
{
    public interface IResourceBL
    {
        int AddBenchmarkResource(int resourceSportId, int benchmarkTypeId, int benchmarkTierId, string yearId, int? nsoValue, int? otpValue, string comments, bool active);
        int AddResource(
            string resourceTypeId,
            string firstName,
            string lastName,
            string countryId,
            DateTime? dateOfBirth,
            bool tbd,
            bool pooled,
            bool active,
            string resourceClassId = null,
            string resourceSpecialtyId = null,
            string gender = null,
            string mailingAddress = null,
            string mailingCity = null,
            string mailingProvinceId = null,
            string emailAddress = null,
            string hometownCity = null,
            string hometownProvinceId = null,
            string primaryCSCId = null,
            string secondaryCSCId = null,
            string comments = null,
            bool isNew = true,
            string assignSportId = null,
            string yearId = null,
            int? personalCoachId = null
            );
        int AddResourceAffiliate(string affiliateTypeId, int resourceId, string yearFromId, string yearToId, string affiliateName, string description, string affiliateContact, string affiliateEmail, string comments);
        int AddResourceDirectAthleteSupport(int resourceId, string directAthleteSupportId, string yearId, string supportName, string description, string supportContact, string supportEmail, decimal? amount, string comments);
        int AddResourceQualification(int resourceId, string qualificationTypeId, string description, bool qualified, string comments);
        int AddResourceSport(int resourceId, string sportId, string yearId, string resourceClassId, string comments, string sportTrackingRef, int? coachResourceId, bool? istCrossCompetency, bool? istPrimary);
        int AddResourceResults(
            int resourceID,
            int competitionID,
            string sportID_Level5,
            DateTime eventDate,
            int position,
            TimeSpan? perfTime,
            decimal? perfValue,
            string comments);
        void UpdateResourceResults(
            int resultID,
            int competitionID,
            DateTime date,
            string sportID_Level5,
            int position,
            TimeSpan? perfTime,
            decimal? perfValue,
            string comments);
        int? GetResourceIDByResult(int resultId);
        void DeleteResourceResults(int resultId);
        System.Collections.Generic.List<OTP.Ring.Models.ViewModel.MaintenanceAthletesAffiliatesInfoViewModel> GetMaintenanceAthletesAffiliatesInfoViewModel(int resourceId);
        System.Collections.Generic.List<OTP.Ring.Models.ViewModel.MaintenanceAthletesAffiliatesInfoViewModel> GetMaintenanceAthletesAffiliatesInfoViewModelParameter(int resourceId, Telerik.Web.Mvc.GridCommand command);
        System.Collections.Generic.List<OTP.Ring.Models.ViewModel.MaintenanceAthletesResultsViewModel> GetMaintenanceAthletesResultsViewModel(int resourceId);
        System.Collections.Generic.List<OTP.Ring.Models.ViewModel.MaintenanceAthletesResultsViewModel> GetMaintenanceAthletesResultsViewModelParameter(int resourceId, Telerik.Web.Mvc.GridCommand command);
        System.Collections.Generic.List<OTP.Ring.Models.ViewModel.MaintenanceAthletesAthleteGridViewModel> GetMaintenanceAthletesAthleteGridViewModel(string selectedSportId, string yearId, Telerik.Web.Mvc.GridCommand command, out int total);
        System.Collections.Generic.List<OTP.Ring.Models.ViewModel.MaintenanceAthletesAthleteGridViewModel> GetMaintenanceAthletesAthleteGridViewModelParameter(string selectedSportId, string yearId, Telerik.Web.Mvc.GridCommand command);
        OTP.Ring.Models.ViewModel.MaintenanceAthletesContactInfoViewModel GetMaintenanceAthletesContactInfoViewModel(int resourceId);
        System.Collections.Generic.List<OTP.Ring.Models.ViewModel.MaintenanceAthletesDirectAthleteSupportViewModel> GetMaintenanceAthletesDirectAthleteSupportViewModel(int resourceId);
        System.Collections.Generic.List<OTP.Ring.Models.ViewModel.MaintenanceAthletesDirectAthleteSupportViewModel> GetMaintenanceAthletesDirectAthleteSupportViewModelParameter(int resourceId, Telerik.Web.Mvc.GridCommand command);
        OTP.Ring.Models.ViewModel.MaintenanceAthletesGeneralInfoViewModel GetMaintenanceAthletesGeneralInfoViewModel(int resourceId);
        System.Collections.Generic.List<OTP.Ring.Models.ViewModel.MaintenanceAthletesSportsInfoViewModel> GetMaintenanceAthletesSportsInfoViewModel(int resourceId, string yearId);
        System.Collections.Generic.List<OTP.Ring.Models.ViewModel.MaintenanceAthletesSportsInfoViewModel> GetMaintenanceAthletesSportsInfoViewModelParameter(int resourceId, string yearId, Telerik.Web.Mvc.GridCommand command);
        OTP.Ring.Models.ViewModel.MaintenanceISTMembersContactInfoViewModel GetMaintenanceISTMembersContactInfoViewModel(int resourceId);
        OTP.Ring.Models.ViewModel.MaintenanceISTMembersGeneralInfoViewModel GetMaintenanceISTMembersGeneralInfoViewModel(int resourceId);
        System.Collections.Generic.List<OTP.Ring.Models.ViewModel.MaintenanceISTMembersGridViewModel> GetMaintenanceISTMembersGridViewModel(string selectedSportId, Telerik.Web.Mvc.GridCommand command, out int total);
        System.Collections.Generic.List<OTP.Ring.Models.ViewModel.MaintenanceISTMembersGridViewModel> GetMaintenanceISTMembersGridViewModelParameter(string selectedSportId, Telerik.Web.Mvc.GridCommand command);
        System.Collections.Generic.List<OTP.Ring.Models.ViewModel.MaintenanceISTMembersQualificationInfoViewModel> GetMaintenanceISTMembersQualificationInfoViewModel(int resourceId);
        System.Collections.Generic.List<OTP.Ring.Models.ViewModel.MaintenanceISTMembersSportsInfoViewModel> GetMaintenanceISTMembersSportsInfoViewModel(int resourceId);
        System.Collections.Generic.List<OTP.Ring.Models.ViewModel.MaintenanceISTMembersSportsInfoViewModel> GetMaintenanceISTMembersSportsInfoViewModelParameter(int resourceId, Telerik.Web.Mvc.GridCommand command);
        OTP.Ring.Models.ViewModel.MaintenanceNSOMembersContactInfoViewModel GetMaintenanceNSOMembersContactInfoViewModel(int resourceId);
        OTP.Ring.Models.ViewModel.MaintenanceNSOMembersGeneralInfoViewModel GetMaintenanceNSOMembersGeneralInfoViewModel(int resourceId);
        System.Collections.Generic.List<OTP.Ring.Models.ViewModel.MaintenanceNSOMembersGridViewModel> GetMaintenanceNSOMembersGridViewModel(string selectedSportId, Telerik.Web.Mvc.GridCommand command, out int total);
        System.Collections.Generic.List<OTP.Ring.Models.ViewModel.MaintenanceNSOMembersGridViewModel> GetMaintenanceNSOMembersGridViewModelParameter(string selectedSportId, Telerik.Web.Mvc.GridCommand command);
        System.Collections.Generic.List<OTP.Ring.Models.ViewModel.MaintenanceNSOMembersQualificationInfoViewModel> GetMaintenanceNSOMembersQualificationInfoViewModel(int resourceId);
        System.Collections.Generic.List<OTP.Ring.Models.ViewModel.MaintenanceNSOMembersSportsInfoViewModel> GetMaintenanceNSOMembersSportsInfoViewModel(int resourceId);
        System.Collections.Generic.List<OTP.Ring.Models.ViewModel.MaintenanceNSOMembersSportsInfoViewModel> GetMaintenanceNSOMembersSportsInfoViewModelParameter(int resourceId, Telerik.Web.Mvc.GridCommand command);
        System.Collections.Generic.List<OTP.Ring.Models.ListItem> GetResourceClasses();
        System.Collections.Generic.IEnumerable<Decode> GetCoachResources(string sportId);
        System.Collections.Generic.List<OTP.Ring.Models.ListItem> GetResourceQualificationTypes();
        System.Collections.Generic.List<OTP.Ring.Models.ListItem> GetResourceSpecialties(string resourceType, bool includeNullItem = false);
        string GetResourceType(string id);
        int SavePersonalCoach(int athleteId, int? coachId, string yearId);
        void PopulateMaintenanceResourcesViewData(System.Web.Mvc.ViewDataDictionary viewData, string resourceId, string selectedSportId);
        void PopulateMaintenanceResourcesTabsExceptResultViewData(System.Web.Mvc.ViewDataDictionary viewData, string resourceId);
        void PopulateMaintenanceResourcesResultTabViewData(System.Web.Mvc.ViewDataDictionary viewData, string resourceId);
        void PopulateMaintenanceISTMembersViewData(System.Web.Mvc.ViewDataDictionary viewData, string resourceId, string selectedSportId);
        void PopulateMaintenanceISTMembersTabViewData(System.Web.Mvc.ViewDataDictionary viewData, string resourceId);
        void PopulateMaintenanceNSOMembersViewData(System.Web.Mvc.ViewDataDictionary viewData, string resourceId, string selectedSportId);
        void PopulateMaintenanceNSOMembersTabViewData(System.Web.Mvc.ViewDataDictionary viewData, string resourceId);
        void PopulateMaintenanceResourcesBenchmarkViewData(System.Web.Mvc.ViewDataDictionary viewData, string resourceId);
        int UpdateBenchmarkResource(int benchmarkResourceId, int? resourceSportId = null, int? benchmarkTypeId = null, int? benchmarkTierId = null, string yearId = Constant.UndefinedString, int? nsoValue = null, int? otpValue = null, string comments = Constant.UndefinedString, bool? active = null);
        int UpdateResource(
            int resourceId,
            string resourceTypeId = null,
            string resourceClassId = null,
            string resourceSpecialtyId = null,
            string firstName = null,
            string lastName = null,
            string countryId = null,
            string gender = null,
            DateTime? dateOfBirth = null,
            string mailingAddress = null,
            string mailingCity = null,
            string mailingProvinceId = null,
            string mailingCountryId = null,
            string mailingPostalCode = null,
            string emailAddress = null,
            string skypeName = null,
            string hometownCity = null,
            string hometownProvinceId = null,
            string hometownCountryId = null,
            string homePhone = null,
            string cellPhone = null,
            string primaryCSCId = null,
            string secondaryCSCId = null,
            bool active = true,
            bool tbd = true,
            bool pooled = true,
            bool isNew = true,
            string comments = null,
            string assignSportId = null,
            string yearId = null,
            string sportTrackingRef = null,
            int? personalCoachId = null
            );       
        int UpdateResourceAffiliate(int resourceAffiliateId, string affiliateTypeId = Constant.UndefinedString, int? resourceId = null, string yearFromId = Constant.UndefinedString, string yearToId = Constant.UndefinedString, string affiliateName = Constant.UndefinedString, string description = Constant.UndefinedString, string affiliateContact = Constant.UndefinedString, string affiliateEmail = Constant.UndefinedString, string comments = Constant.UndefinedString);
        int UpdateResourceDirectAthleteSupport(int resourceDirectAthleteSupportId, int? resourceId = null, string directAthleteSupportId = Constant.UndefinedString, string yearId = Constant.UndefinedString, string supportName = Constant.UndefinedString, string description = Constant.UndefinedString, string supportContact = Constant.UndefinedString, string supportEmail = Constant.UndefinedString, decimal? amount = null, string comments = Constant.UndefinedString);
        int UpdateResourceQualification(int resourceQualificationId, int? resourceId = null, string qualificationTypeId = Constant.UndefinedString, string description = Constant.UndefinedString, bool? qualified = null, string comment = Constant.UndefinedString);
        int UpdateResourceSport(int resourceSportId, int? resourceId = null, string sportId = Constant.UndefinedString, string yearId = Constant.UndefinedString, string resourceClassId = Constant.UndefinedString, string comment = Constant.UndefinedString, string sportTrackingRef = Constant.UndefinedString, int? coachResourceId = null, bool? istCrossCompetency = null, bool? istPrimary = null);
        void DeleteReource(int resourceId);
        int DeleteResourceSport(int resourceSportId);
        int DeleteResourceDirectAthleteSupport(int resourceDirectAthleteSupportId);
        int DeleteResourceAffiliate(int resourceAffiliateId);
        int DeleteResourceQualification(int resourceQualificationId);
        int DeleteBenchmarkResource(int benchmarkResourceId);
        int GetResourceIdByDirectAthleteSupport(int resourceDirectAthleteSupportId);
        int GetResourceIdByAthleteAffiliate(int resourceAthletesAffiliateId);
        int GetResourceIdByResourceSport(int resourceSportId);
        int GetResourceIdByResourceQualification(int resourceQualificationId);
        System.Collections.Generic.List<int> GetBenchmarkResourceIdsByResourceSportYearTypeTier(int resourceSportId, string yearId, int benchmarkTypeId, int benchmarkTierId);
        System.Collections.Generic.List<int> GetResourceSportIdsByResourceSport(int resourceId, string sportId, string yearId);
        System.Collections.Generic.List<int> GetResourceDirectAthleteSupportIdsByResourceSupportYear(int resourceId, string yearId, string directAthleteSupportId);
        System.Collections.Generic.List<int> GetResourceAffiliateIdsByResourceTypeNameYear(int resourceId, string affiliateTypeId, string affiliateName, string yearFromId);
        int GetResourceResultsByCompetition(string competitionType, DateTime competitionDate, string sportId, string location, string countryId);
        System.Collections.Generic.List<int> GetResourceResultsBySportEvent(int resourceId, string sportId, int competitionId);
        System.Collections.Generic.List<OTP.Ring.Models.ViewModel.MaintenanceISTMembersPoolInfoViewModel> GetMaintenanceISTMembersPoolInfoViewModel(int resourceId);
        System.Collections.Generic.List<OTP.Ring.Models.ViewModel.MaintenanceNSOMembersPoolInfoViewModel> GetMaintenanceNSOMembersPoolInfoViewModel(int resourceId);
        int AddResourcePoolAssignment(int resourceIdPool, int resourceIdAssignment);
        int UpdateResourcePoolAssignment(int resourcePoolAssignmentId, int resourceIdPool, int resourceIdAssignment);
        int DeleteResourcePoolAssignment(int resourcePoolAssignmentId);
        System.Collections.Generic.List<OTP.Ring.Models.ListItem> GetResourcePoolAssignmentList(int resourceId);
        bool ShouldPrompResourceExists(string resourceTypeId, string firstName, string lastName, string countryId);
        string GetResourceNameByResourceId(int resourceId);
    }
}
