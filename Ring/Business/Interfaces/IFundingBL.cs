using System;

namespace OTP.Ring.Business
{
    public interface IFundingBL
    {
        int CreateCopyFunding(string fundingType, string projectId, string organizationId, string sportId, string yearId, int? fundingId, bool copyBlockCategory, bool copyDetail, bool copyRequestAmount, bool copyAmount, string copyAmountType);
        int CreateNewFunding(string fundingType, string projectId, string organizationId, string sportId, string yearId);
        int DeleteFundingDetail(int fundingId, int fundingDetailId);
        bool DoesFundingExist(string fundingType, string projectId, string organizationId, string sportId, string yearId);
        System.Collections.Generic.IEnumerable<OTP.Ring.Common.Decode> GetFundingResources(string fundingCategoryType, int fundingId);
        System.Collections.Generic.IEnumerable<OTP.Ring.Common.Decode> GetFundingResourcesByCsc(string fundingCategoryType, int fundingId, string cscId);
        OTP.Ring.Common.Funding GetFunding(int Id);
        System.Collections.Generic.IEnumerable<OTP.Ring.Common.FundingBlock> GetFundingBlocksByFundingId(int fundingId);
        System.Collections.Generic.IEnumerable<OTP.Ring.Common.FundingBlockType> GetFundingBlockTypes();
        System.Collections.Generic.IEnumerable<OTP.Ring.Common.FundingCategory> GetFundingCategoriesByFundingBlockId(int fundingId, int fundingBlockId);
        System.Collections.Generic.IEnumerable<OTP.Ring.Common.FundingCategoryType> GetFundingCategoryTypes();
        System.Collections.Generic.IEnumerable<OTP.Ring.Common.FundingDetail> GetFundingDetailsByFundingCategoryId(int fundingId, int fundingCategoryId);
        int? GetFundingIdBySportAndYear(string sportId, string yearId);
        System.Collections.Generic.IEnumerable<OTP.Ring.Common.FundingSummary> GetFundingSummariesByFundingIds(System.Collections.Generic.IEnumerable<int> fundingIds);
        System.Collections.Generic.IEnumerable<OTP.Ring.Common.FundingTotal> GetFundingTotalsByFundingId(int fundingId);
        System.Collections.Generic.IEnumerable<int> GetNonNSOFundingIdsByYear(string yearId);
        int SaveFundingDetail(string actionName, OTP.Ring.Common.FundingDetail postedFundingDetail, int fundingId, int fundingCategoryId);
        int UpdateFundingAvailableFunds(OTP.Ring.Common.FundingAllocation fundingAllocation);
        int UpdateFundingBlockComment(int fundingId, int fundingBlockId, string comment);
        int UpdateFundingCategoryComment(int fundingId, int fundingCategoryId, string comment);
        int UpdateFundingDetailComment(int fundingId, int fundingDetailId, string comment);
        int UpdateFundingStatus(int id, string fromStatus, string toStatus);
        OTP.Ring.Common.FundingAllocation GetFundingAllocation(string type, string yearId, string sportId);
    }
}
