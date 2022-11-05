using System;
using OTP.Ring.Common;
namespace OTP.Ring.Business
{
    public interface IBenchmarkBL
    {
        int AddBenchmarkSport(string sportId, int benchmarkTypeId, int benchmarkTierId, string yearId, int? nsoValue, int? otpValue, string comments, bool active);
        int AddBenchmarkTier(string sportId, string name_en, string name_fr, int positionStart, int positionEnd, string comments);
        System.Collections.Generic.List<OTP.Ring.Models.ListItem> GetBenchmarkTiersBySportId(string sportId);
        System.Collections.Generic.List<OTP.Ring.Models.ListItem> GetBenchmarkTypes();
        System.Collections.Generic.List<OTP.Ring.Models.ViewModel.MaintenanceAthletesBenchmarkSportsViewModel> GetMaintenanceAthletesBenchmarkSportsViewModel(string year, string resourceSportId);
        System.Collections.Generic.List<OTP.Ring.Models.ViewModel.MaintenanceBenchmarksGridSportBenchmarkViewModel> GetMaintenanceBenchmarksGridSportBenchmarkViewModel(string year, string sportId);
        System.Collections.Generic.List<OTP.Ring.Models.ViewModel.MaintenanceBenchmarksGridSportViewModel> GetMaintenanceBenchmarksGridSportViewModel(string year, string selectedSport);
        string MaintenanceBenchmarksGetValidSelectedSportId(string currentlySelectedSportId);
        void PopulateMaintenanceBenchmarksViewData(System.Web.Mvc.ViewDataDictionary viewData, string selectedSportId);
        int UpdateBenchmarkSport(int benchmarkSportId, string sportId = Constant.UndefinedString, int? benchmarkTypeId = null, int? benchmarkTierId = null, string yearId = Constant.UndefinedString, int? nsoValue = null, int? otpValue = null, string comments = Constant.UndefinedString, bool? active = null);
        System.Collections.Generic.List<int> GetBenchmarkSportIdsBySportYearTypeTier(string sportId, string yearId, int benchmarkTypeId, int benchmarkTierId);
        int DeleteBenchmarkSport(int benchmarkSportId);
    }
}
