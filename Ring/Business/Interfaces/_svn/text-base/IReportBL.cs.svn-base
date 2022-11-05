using System;
using System.Collections.Generic;
using OTP.Ring.Models;
using OTP.Ring.Common;
namespace OTP.Ring.Business
{
    public interface IReportBL
    {
        OTP.Ring.Models.ViewModel.ReportingReportDataViewModel GetReportDocumentData(int reportId, string parameters, string securityUserId);
        System.Collections.Generic.List<OTP.Ring.Models.ListItem> GetReportFilterData(string storedProcedureName, string parameters);
        string GetReportingReportDescription(int reportId);
        OTP.Ring.Models.ViewModel.ReportingReportParametersViewModel GetReportingReportParametersViewModel(int reportId);
        System.Collections.Generic.List<OTP.Ring.Models.ListItem> GetReportList(string reportType, string securityGroupProfileId, string reportCategory = null);
        List<ListItem> GetReportViewList(int? reportId);
        List<OTP.Ring.Models.ViewModel.ReportingDesignAnalysisReportGridViewModel> GetReportingDesignAnalysisReportGridViewModel(string securityUserProfileId, List<OTP.Ring.Common.RingMenuItem> appliedSports);
        int AddAdhocReport(
            string name_EN,
            string name_FR,
            string description_EN,
            string description_FR,
            bool locked,
            string sportId,
            string comments,
            string reportCategory,
            bool securityUserSport,
            string reportViewId,
            string securityUserProfileId);
        int UpdateAdhocReport(
            int reportId,
            string name_EN = Constant.UndefinedString,
            string name_FR = Constant.UndefinedString,
            string description_EN = Constant.UndefinedString,
            string description_FR = Constant.UndefinedString,
            bool? locked = null,
            string sportId = Constant.UndefinedString,
            string comments = Constant.UndefinedString,
            string reportCategory = Constant.UndefinedString,
            bool? securityUserSport = null);
        int DeleteAdhocReport(int reportId);
        void PopulateDesignAnalysisViewData(System.Web.Mvc.ViewDataDictionary viewData, string reportId);
        List<OTP.Ring.Models.ViewModel.ReportingDesignAnalysisGroupingsGridViewModel> GetReportingDesignAnalysisGroupingsGridViewModel(int reportId, string securityUserProfileId);
        int AddReportGroup(
            int reportId,
            string fieldName_EN,
            string fieldName_FR,
            string comments);
        int UpdateReportGroup(
            int reportGroupId,
            string fieldName_EN = Constant.UndefinedString,
            string fieldName_FR = Constant.UndefinedString,
            string comments = Constant.UndefinedString);
        int DeleteReportGroup(int reportGroupId);
        bool IsReportGroupValidForDeleting(int reportGroupId);
        List<ListItem> GetReportViewFieldsList(int reportId, bool includeNull);
        List<OTP.Ring.Models.ViewModel.ReportingDesignAnalysisGroupingColumnsGridViewModel> GetReportingDesignAnalysisGroupingColumnsGridViewModel(int reportGroupId, string securityUserProfileId);
        int AddReportColumn(
            int reportGroupId,
            string displayName_EN,
            string displayName_FR,
            string fieldName_EN,
            string fieldName_FR,
            string reportAggregation,
            string fieldSize,
            string comments,
            string formatString);
        int UpdateReportColumn(
            int reportColumnId,
            string displayName_EN = Constant.UndefinedString,
            string displayName_FR = Constant.UndefinedString,
            string fieldName_EN = Constant.UndefinedString,
            string fieldName_FR = Constant.UndefinedString,
            string reportAggregation = Constant.UndefinedString,
            string fieldSize = Constant.UndefinedString,
            string comments = Constant.UndefinedString,
            string formatString = Constant.UndefinedString);
        int DeleteReportColumn(int reportColumnId);
        List<ListItem> GetReportAggregationList(int reportId, string columnFieldName);
        List<Models.ViewModel.ReportingDesignAnalysisFiltersGridViewModel> GetReportingDesignAnalysisFiltersGridViewModel(int reportId, string securityUserProfileId);
        int AddReportFilter(
            int reportId,
            string filterName_EN,
            string filterName_FR,
            string fieldName_EN,
            string fieldName_FR,
            bool display,
            string default_EN,
            string reportOperand,
            string comments);
        int UpdateReportFilter(
            int reportFilterId,
            string filterName_EN = Constant.UndefinedString,
            string filterName_FR = Constant.UndefinedString,
            string fieldName_EN = Constant.UndefinedString,
            string fieldName_FR = Constant.UndefinedString,
            bool? display = null,
            string default_EN = Constant.UndefinedString,
            string reportOperand = Constant.UndefinedString,
            string comments = Constant.UndefinedString);
        int DeleteReportFilter(int reportFilterId);
        List<ListItem> GetReportList(int reportId);
        List<OTP.Ring.Models.ViewModel.ReportingDesignAnalysisSecurityGridViewModel> GetReportingDesignAnalysisSecurityGridViewModel(int reportId, string securityUserProfileId);
        int AddReportSecurityGroupProfile(
            int reportId,
            string securityGroupProfileId,
            string comments);
        int DeleteReportSecurityGroupProfile(int reportSecurityGroupProfileId);
        System.Collections.Generic.List<OTP.Ring.Models.ListItem> GetSecurityGroupProfileList(int reportId, string securityUserProfileId);
        OTP.Ring.Models.ViewModel.ReportingStandardReportsViewModel PopulateReportingStandardReportsViewModel(string reportCategory, string securityUserGroupProfileId);
        int CopyReport(int reportId);
        List<ListItem> GetReportColumnFormatStringList(int reportId, string columnFieldName);
        bool CanUnlockReport(int reportId, out string message);
    }
}
