﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OTP.Ring;
using OTP.Ring.Common;
using OTP.Ring.Models;
using System.Runtime.Caching;

namespace OTP.Ring.Business
{
    public class FundingBL : IFundingBL
    {
        private bool _frenchLanguage = false;
        private Data.Models.RingEntities _repository;
        private IDecodeBL _decodeBL;
        private ISportBL _sportBL;
        private IActionItemBL _actionItemBL;

        public FundingBL(string connectionString, IDecodeBL decodeBL, ISportBL sportBL, IActionItemBL actionItemBL)
        {
            this._repository = new Data.Models.RingEntities(connectionString);
            this._decodeBL = decodeBL;
            this._sportBL = sportBL;
            this._actionItemBL = actionItemBL;
            this._frenchLanguage = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToUpper() == Constant.LanguageCode.French;
        }

#region PUBLIC 

        public Funding GetFunding(int Id)
        {
            Funding funding = null;

            string cacheKey = string.Format(Constant.Cache.KeyTemplate, Constant.Cache.Funding, null, Id);
            ObjectCache cache = MemoryCache.Default;

            if (cache.Contains(cacheKey))
            {
                funding = (Funding)cache.Get(cacheKey);
            }
            else
            {
                var dataFundings = from f in this._repository.FundingSummaries
                              where f.FundingSummaryID == Id
                              select f;

                if (dataFundings != null && dataFundings.Count() ==1)
                {
                    funding = ConvertFunding(dataFundings.First());
                    funding = CalculateFunding(funding);
                    funding = CountFundingActionItem(funding);
                    funding = CalculateFundingTotal(funding);

                    cache.Add(cacheKey, funding, new CacheItemPolicy() { SlidingExpiration = new TimeSpan(0, 0, 1) });
                }
            }

            funding = PopulateCodeDescription(funding);

            return funding;
        }

        public IEnumerable<FundingTotal> GetFundingTotalsByFundingId(int fundingId)
        {
            IEnumerable<FundingTotal> fundingTotals = new List<FundingTotal>();

            Funding funding = GetFunding(fundingId);

            if (funding != null && funding.FundingTotals != null)
            {
                fundingTotals = funding.FundingTotals;
            }

            return fundingTotals;
        }

        public IEnumerable<FundingBlock> GetFundingBlocksByFundingId(int fundingId)
        {
            IEnumerable<FundingBlock> fundingBlocks = new List<FundingBlock>();

            Funding funding = GetFunding(fundingId);

            if (funding != null && funding.FundingBlocks != null)
            {
                fundingBlocks = funding.FundingBlocks;
            }

            return fundingBlocks;
        }

        public IEnumerable<FundingCategory> GetFundingCategoriesByFundingBlockId(int fundingId, int fundingBlockId)
        {
            Funding funding = GetFunding(fundingId);

            IEnumerable<FundingCategory> fundingCategories = new List<FundingCategory>();

            if (funding != null && funding.FundingBlocks != null)
            {
                var blocks = from b in funding.FundingBlocks
                             where b.Id == fundingBlockId
                             select b;

                if (blocks != null && blocks.Count() == 1)
                {
                    fundingCategories = blocks.First().FundingCategories;
                }
            }

            return fundingCategories;
        }

        public IEnumerable<FundingDetail> GetFundingDetailsByFundingCategoryId(int fundingId, int fundingCategoryId)
        {
            Funding funding = GetFunding(fundingId);

            IEnumerable<FundingDetail> fundingDetails = new List<FundingDetail>();

            if (funding != null && funding.FundingBlocks != null)
            {
                foreach (var fundingBlock in funding.FundingBlocks)
                {
                    if (fundingBlock.FundingCategories == null) continue;

                    var categories = from c in fundingBlock.FundingCategories
                                     where c.Id == fundingCategoryId
                                     orderby c.Id descending
                                     select c;

                    if (categories != null && categories.Count() == 1)
                    {
                        fundingDetails = categories.First().FundingDetails;
                        break;
                    }
                }
            }

            return fundingDetails;
        }

        public int? GetFundingIdBySportAndYear(string sportId, string yearId)
        {
            var fundings = from f in this._repository.FundingSummaries
                           where f.OrganizationTypeID == Common.Constant.OrganizationTypeId.NSO && f.SportID == sportId && f.YearID == yearId && f.FundingStatus != Constant.FundingStatus.SysAloc
                            select f.FundingSummaryID;

            if (fundings != null && fundings.Count() == 1)
            {
                return fundings.Single();
            }

            return null;
        }

        public IEnumerable<int> GetNonNSOFundingIdsByYear(string yearId)
        {
            return (from f in this._repository.FundingSummaries
                    where (f.OrganizationTypeID == Constant.OrganizationTypeId.CSC || f.OrganizationTypeID == Constant.OrganizationTypeId.OTP) && f.YearID == yearId && f.FundingStatus != Constant.FundingStatus.SysAloc
                        select f.FundingSummaryID).ToList();
        }

        public IEnumerable<FundingBlockType> GetFundingBlockTypes()
        {
            string cacheKey = string.Format(Constant.Cache.KeyTemplate, Constant.Cache.FundingBlockType, _frenchLanguage, null);
            ObjectCache cache = MemoryCache.Default;

            if (cache.Contains(cacheKey))
            {
                return (IEnumerable<FundingBlockType>)cache.Get(cacheKey);
            }
            else
            {
                var decodes = (from t in this._repository.FundingBlockTypes
                        select new FundingBlockType
                        {
                            Type = t.FundingBlockTypeID,
                            Name = _frenchLanguage ? t.Name_FR : t.Name_EN,
                            Comment = t.Comments,
                            SortOrder = t.SortOrder
                        }).ToList();

                cache.Add(cacheKey, decodes, new CacheItemPolicy() { SlidingExpiration = new TimeSpan(1, 0, 0) });

                return decodes;
            }
        }

        public IEnumerable<FundingCategoryType> GetFundingCategoryTypes()
        {
            string cacheKey = string.Format(Constant.Cache.KeyTemplate, Constant.Cache.FundingCategoryType, _frenchLanguage, null);
            ObjectCache cache = MemoryCache.Default;

            if (cache.Contains(cacheKey))
            {
                return (IEnumerable<FundingCategoryType>)cache.Get(cacheKey);
            }
            else
            {
                var decodes = (from t in this._repository.FundingCategoryTypes
                               select new FundingCategoryType
                                   {
                                       Type = t.FundingCategoryTypeID,
                                       BlockType = t.FundingBlockTypeID,
                                       Name = _frenchLanguage ? t.Name_FR : t.Name_EN,
                                       CscRequired = t.CSCRequired,
                                       ResourceRequired = t.ResourceRequired,
                                       ResourceType = t.ResourceTypeID,
                                       Comment = t.Comments,
                                       SortOrder = t.SortOrder
                                   }).ToList();

                cache.Add(cacheKey, decodes, new CacheItemPolicy() { SlidingExpiration = new TimeSpan(1, 0, 0) });

                return decodes;
            }
        }

        public IEnumerable<FundingSummary> GetFundingSummariesByFundingIds(IEnumerable<int> fundingIds)
        {
            IList<Funding> fundings = new List<Funding>();

            if (fundingIds != null)
            {
                foreach (var fundingId in fundingIds)
                {
                    fundings.Add(GetFunding(fundingId));
                }
            }

            IEnumerable<FundingSummary> fundingSummaries = (from a in (from f in fundings group f by f.Type)
                    select new FundingSummary() { Type = a.Key, Fundings = a.ToList() }).ToList();

            fundingSummaries = PadFundingSummaries(fundingSummaries);
            fundingSummaries = CalculateFundingSummaries(fundingSummaries);
            fundingSummaries = PopulateCodeDescription(fundingSummaries);
            fundingSummaries = SortFundingSummaries(fundingSummaries);

            return fundingSummaries;
        }

        public IEnumerable<Decode> GetFundingResources(string fundingCategoryType, int fundingId)
        {
            IEnumerable<Decode> list = new List<Decode>();

            FundingCategoryType categoryType = (from t in this.GetFundingCategoryTypes() where t.Type == fundingCategoryType select t).SingleOrDefault();
            var fundingSportId = (from f in this._repository.FundingSummaries
                                  where f.FundingSummaryID == fundingId
                                  select f.SportID).SingleOrDefault();

            if (categoryType != null)
            {
                if (string.IsNullOrEmpty(categoryType.ResourceType))
                {
                    var resources = from r in this._repository.Resources
                                    where (r.ResourceTypeID == Constant.ResourceType.ISTMembers
                                    || r.ResourceTypeID == Constant.ResourceType.NSOMembers)
                                    && (fundingSportId == null
                                    || r.ResourceSports.Any(s => s.SportID == fundingSportId))
                                    select new
                                    {
                                        IntId = r.ResourceID,
                                        FirstName = r.FirstName,
                                        LastName = r.LastName,
                                        ResourceTypeID = r.ResourceTypeID
                                    };

                    list =
                        (from r in resources.AsEnumerable()
                         orderby r.ResourceTypeID, r.LastName, r.FirstName
                         select new Decode
                         {
                             IntId = r.IntId,
                             Name = ResourceBL.FormatResourceName(r.IntId, r.LastName, r.FirstName, r.ResourceTypeID)
                         }).ToList();

                    return list;
                }
                else
                {
                    var resources = from r in this._repository.Resources
                                    where r.ResourceTypeID == categoryType.ResourceType
                                    && (fundingSportId == null
                                    || r.ResourceSports.Any(s => s.SportID == fundingSportId))
                                    select new
                                    {
                                        IntId = r.ResourceID,
                                        FirstName = r.FirstName,
                                        LastName = r.LastName,
                                        ResourceTypeID = r.ResourceTypeID
                                    };

                    list =
                        (from r in resources.AsEnumerable()
                         orderby r.ResourceTypeID, r.LastName, r.FirstName
                         select new Decode
                         {
                             Id = r.IntId.ToString(),
                             IntId = r.IntId,
                             Name = ResourceBL.FormatResourceName(r.IntId, r.LastName, r.FirstName, r.ResourceTypeID)
                         }).ToList();
                }
            }

            return list;
        }



        public IEnumerable<Decode> GetFundingResourcesByCsc(string fundingCategoryType, int fundingId, string cscId)
        {
            IEnumerable<Decode> list = new List<Decode>();

            FundingCategoryType categoryType = (from t in this.GetFundingCategoryTypes() where t.Type == fundingCategoryType select t).SingleOrDefault();


            if (categoryType != null)
            {

				var fundingSports = (from s in this._repository.Sports
									 where s.OrganizationID == cscId
									 select s.SportID).Distinct().ToList();


				

				if (cscId.StartsWith("CSC"))
				{
					var resources = from r in this._repository.Resources
									from o in this._repository.Organizations.Where( x => x.OrganizationID == r.PrimaryCSCID || x.OrganizationID == r.SecondaryCSCID)
									//on
									//r.PrimaryCSCID equals o.OrganizationID
									where o.OrganizationID == cscId && r.Active == true
									select new
									{
										IntId = r.ResourceID,
										FirstName = r.FirstName,
										LastName = r.LastName,
										ResourceTypeID = r.ResourceTypeID
									};
					list =
						(from r in resources.AsEnumerable()
						 orderby r.ResourceTypeID, r.LastName, r.FirstName
						 select new Decode
						 {
							 Id = r.IntId.ToString(),
							 IntId = r.IntId,
							 Name = ResourceBL.FormatResourceName(r.IntId, r.LastName, r.FirstName, r.ResourceTypeID)
						 }).ToList();
				}
				else
				{
					var resources = from r in this._repository.Resources
                                    where r.ResourceTypeID == categoryType.ResourceType && r.Active == true
									from rs in r.ResourceSports
									where fundingSports.Contains(rs.SportID)
									select new
									{
										IntId = r.ResourceID,
										FirstName = r.FirstName,
										LastName = r.LastName,
										ResourceTypeID = r.ResourceTypeID
									};
					list =
						(from r in resources.AsEnumerable()
						 orderby r.ResourceTypeID, r.LastName, r.FirstName
						 select new Decode
						 {
							 Id = r.IntId.ToString(),
							 IntId = r.IntId,
							 Name = ResourceBL.FormatResourceName(r.IntId, r.LastName, r.FirstName, r.ResourceTypeID)
						 }).ToList();
				}

                    
                
            }

            return list;
        }





        public int UpdateFundingAvailableFunds(FundingAllocation fundingAllocation)
        {
            int result = 0;

            var dataFundingSummaries = from f in this._repository.FundingSummaries
                                        where f.FundingSummaryID == fundingAllocation.Id
                                        select f;

            if (dataFundingSummaries != null && dataFundingSummaries.Count() == 1)
            {
                var dataFundingSummary = dataFundingSummaries.First();

                var output = this._repository.uspUpdateFundingSummary(dataFundingSummary.FundingSummaryID, dataFundingSummary.FundingStatus,
                    fundingAllocation.AvailableFundsSC, fundingAllocation.AvailableFundsCoc, fundingAllocation.AvailableFundsOther).SingleOrDefault();
                result = output == null ? result : (int)output;
            }
            else
            {
                var output = this._repository.uspAddFundingSummary(fundingAllocation.Type,
                    null,
                    fundingAllocation.SportId,
                    Constant.FundingStatus.SysAloc,
                    null,
                    fundingAllocation.YearId,
                    fundingAllocation.AvailableFundsSC,
                    fundingAllocation.AvailableFundsCoc,
                    fundingAllocation.AvailableFundsOther).SingleOrDefault();
                result = output == null ? result : (int)output;
            }

            return result;
        }

        public int UpdateFundingStatus(int id, string fromStatus, string toStatus)
        {
            int result = 0;

            var dataFundingSummaries = (from f in this._repository.FundingSummaries
                                where f.FundingSummaryID == id
                                select f).ToList();

            if (dataFundingSummaries != null && dataFundingSummaries.Count() == 1)
            {
                var dataFundingSummary = dataFundingSummaries.First();

                var output = this._repository.uspUpdateFundingSummary(id, toStatus, 
                    dataFundingSummary.FundingSC, dataFundingSummary.FundingCOC, dataFundingSummary.FundingOther).SingleOrDefault();
                result = output == null ? result : (int)output;
            }

            result += UpdateFundingDetailForStatusChange(id, fromStatus, toStatus);

            this.RemoveFundingFromCache(id);

            return result;
        }

        public int UpdateFundingBlockComment(int fundingId, int fundingBlockId, string comment)
        {
            int result = 0;

            var dataFundingBlocks = from b in this._repository.FundingBlocks
                                    where b.FundingBlockID == fundingBlockId
                                    select b;

            if (dataFundingBlocks != null && dataFundingBlocks.Count() == 1)
            {
                var output = this._repository.uspUpdateFundingBlock(fundingBlockId, comment).SingleOrDefault();
                result = output == null ? result : (int)output;
            }

            this.RemoveFundingFromCache(fundingId);

            return result;
        }

        public int UpdateFundingCategoryComment(int fundingId, int fundingCategoryId, string comment)
        {
            int result = 0;

            var dataFundingCategories = from c in this._repository.FundingCategories
                                    where c.FundingCategoryID == fundingCategoryId
                                    select c;

            if (dataFundingCategories != null && dataFundingCategories.Count() == 1)
            {
                var output = this._repository.uspUpdateFundingCategory(fundingCategoryId, comment).SingleOrDefault();
                result = output == null ? result : (int)output;
            }

            this.RemoveFundingFromCache(fundingId);

            return result;
        }

        public int UpdateFundingDetailComment(int fundingId, int fundingDetailId, string comment)
        {
            int result = 0;

            var dataFundingDetails = from d in this._repository.FundingDetails
                                        where d.FundingDetailID == fundingDetailId
                                        select d;

            if (dataFundingDetails != null && dataFundingDetails.Count() == 1)
            {
                var dataFundingDetail = dataFundingDetails.First();

                var output = this._repository.uspUpdateFundingDetail(
                        dataFundingDetail.FundingDetailID,
                        dataFundingDetail.FundingCategoryID,
                        dataFundingDetail.Description,
                        dataFundingDetail.OrganizationID,
                        dataFundingDetail.ResourceID,
                        dataFundingDetail.PercentCommitted,
                        dataFundingDetail.FundingPriority,
                        dataFundingDetail.RequestTotal,
                        dataFundingDetail.ActualTotal,
                        dataFundingDetail.RequestNSO,
                        dataFundingDetail.RequestSC,
                        dataFundingDetail.RequestOTP,
                        dataFundingDetail.RecommendTotalHPA,
                        dataFundingDetail.RecommendTotalOTP,
                        dataFundingDetail.RecommendTotal,
                        dataFundingDetail.RecommendSC,
                        dataFundingDetail.RecommendCOC,
                        dataFundingDetail.FundsTotal,
                        dataFundingDetail.FundsSC,
                        dataFundingDetail.FundsCOC,
                        dataFundingDetail.RevisionTotal,
                        dataFundingDetail.RevisionSC,
                        dataFundingDetail.RevisionCOC,
                        dataFundingDetail.Recurring,
                        comment).SingleOrDefault();

                result = output == null ? result : (int)output;
            }

            this.RemoveFundingFromCache(fundingId);

            return result;
        }

        public int DeleteFundingDetail(int fundingId, int fundingDetailId)
        {
            int result = 0;

            var dataFundingDetails = from d in this._repository.FundingDetails
                                        where d.FundingDetailID == fundingDetailId
                                        select d;

            if (dataFundingDetails != null && dataFundingDetails.Count() == 1)
            {
                var output = this._repository.uspDeleteFundingDetail(fundingDetailId).SingleOrDefault();
                result = output == null ? result : (int)output;
            }            

            this.RemoveFundingFromCache(fundingId);

            return result;
        }

        public int SaveFundingDetail(string actionName, FundingDetail postedFundingDetail, int fundingId, int fundingCategoryId)
        {
            int result = 0;
            int? originalResourceId = null;

            var dataFundingDetails = from d in this._repository.FundingDetails
                                        where d.FundingDetailID == postedFundingDetail.Id
                                        select d;

            if (dataFundingDetails != null && dataFundingDetails.Count() == 1)
            {
                //Update
                Data.Models.FundingDetail dataFundingDetail = dataFundingDetails.First();
                originalResourceId = dataFundingDetail.ResourceID;

                dataFundingDetail = ConvertFundingDetail(dataFundingDetail, postedFundingDetail, actionName);

                var output = this._repository.uspUpdateFundingDetail(
                        dataFundingDetail.FundingDetailID,
                        dataFundingDetail.FundingCategoryID,
                        dataFundingDetail.Description,
                        dataFundingDetail.OrganizationID,
                        dataFundingDetail.ResourceID,
                        dataFundingDetail.PercentCommitted,
                        dataFundingDetail.FundingPriority,
                        dataFundingDetail.RequestTotal,
                        dataFundingDetail.ActualTotal,
                        dataFundingDetail.RequestNSO,
                        dataFundingDetail.RequestSC,
                        dataFundingDetail.RequestOTP,
                        dataFundingDetail.RecommendTotalHPA,
                        dataFundingDetail.RecommendTotalOTP,
                        dataFundingDetail.RecommendTotal,
                        dataFundingDetail.RecommendSC,
                        dataFundingDetail.RecommendCOC,
                        dataFundingDetail.FundsTotal,
                        dataFundingDetail.FundsSC,
                        dataFundingDetail.FundsCOC,
                        dataFundingDetail.RevisionTotal,
                        dataFundingDetail.RevisionSC,
                        dataFundingDetail.RevisionCOC,
                        dataFundingDetail.Recurring,
                        dataFundingDetail.Comments).SingleOrDefault();

                result = output == null ? result : (int)output;
            }
            else
            {
                //Insert
                Data.Models.FundingDetail dataFundingDetail = new Data.Models.FundingDetail() { FundingCategoryID = fundingCategoryId };
                originalResourceId = dataFundingDetail.ResourceID;

                dataFundingDetail = ConvertFundingDetail(dataFundingDetail, postedFundingDetail, actionName);

                var output = this._repository.uspAddFundingDetail(
                        dataFundingDetail.FundingCategoryID,
                        dataFundingDetail.Description,
                        dataFundingDetail.OrganizationID,
                        dataFundingDetail.ResourceID,
                        dataFundingDetail.PercentCommitted,
                        dataFundingDetail.FundingPriority,
                        dataFundingDetail.RequestTotal,
                        dataFundingDetail.ActualTotal,
                        dataFundingDetail.RequestNSO,
                        dataFundingDetail.RequestSC,
                        dataFundingDetail.RequestOTP,
                        dataFundingDetail.RecommendTotalHPA,
                        dataFundingDetail.RecommendTotalOTP,
                        dataFundingDetail.RecommendTotal,
                        dataFundingDetail.RecommendSC,
                        dataFundingDetail.RecommendCOC,
                        dataFundingDetail.FundsTotal,
                        dataFundingDetail.FundsSC,
                        dataFundingDetail.FundsCOC,
                        dataFundingDetail.RevisionTotal,
                        dataFundingDetail.RevisionSC,
                        dataFundingDetail.RevisionCOC,
                        dataFundingDetail.Recurring,
                        dataFundingDetail.Comments).SingleOrDefault();

                result = output == null ? result : (int)output;
            }


            this.EvaluateAndCreateActionItem(originalResourceId, postedFundingDetail, actionName, fundingId);

            this.RemoveFundingFromCache(fundingId);

            return result;
        }

        public int CreateCopyFunding(string fundingType, string projectId, string organizationId, string sportId, string yearId,
            int? fundingId, bool copyBlockCategory, bool copyDetail, bool copyRequestAmount, bool copyAmount, string copyAmountType)
        {
            var result = this._repository.uspCopyFunding(fundingId, sportId, yearId).SingleOrDefault();

            return result == null ? 0 : (int)result;

            //if (!fundingId.HasValue || !copyBlockCategory || !copyDetail) return CreateNewFunding(fundingType, projectId, organizationId, sportId, yearId);

            //Funding oldFunding = GetFunding(fundingId.Value);
            //Funding newFunding = BuildNewFunding(fundingType, projectId, organizationId, sportId, yearId);

            //if (newFunding != null && newFunding.FundingBlocks != null)
            //{
            //    foreach (var newFundingBlock in newFunding.FundingBlocks)
            //    {
            //        string fundingBlockType = newFundingBlock.Type;

            //        if (newFundingBlock != null && newFundingBlock.FundingCategories != null)
            //        {
            //            foreach (var newFundingCategory in newFundingBlock.FundingCategories)
            //            {
            //                if (newFundingCategory != null)
            //                {
            //                    string fundingCategoryType = newFundingCategory.Type;

            //                    //Find old funding category by fundingblocktype and fundingcategorytype
            //                    FundingCategory oldFundingCategory = LocateFundingCategory(oldFunding, fundingBlockType, fundingCategoryType);

            //                    //Copy fundingdetail
            //                    if (oldFundingCategory != null && oldFundingCategory.FundingDetails != null)
            //                    {
            //                        newFundingCategory.FundingDetails = new List<FundingDetail>();

            //                        foreach (var oldFundingDetail in oldFundingCategory.FundingDetails)
            //                        {
            //                            if (!oldFundingDetail.Recurring) continue;

            //                            newFundingCategory.FundingDetails.Add(CopyFundingDetail(oldFundingDetail, copyRequestAmount, copyAmount, copyAmountType));
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            //return SaveFunding(newFunding);
        }

        public int CreateNewFunding(string fundingType, string projectId, string organizationId, string sportId, string yearId)
        {
            Funding newFunding = BuildNewFunding(fundingType, projectId, organizationId, sportId, yearId);

            return SaveFunding(newFunding);
        }

        public bool DoesFundingExist(string fundingType, string projectId, string organizationId, string sportId, string yearId)
        {
            if (fundingType == Constant.OrganizationTypeId.NSO)
            {
                return (from f in this._repository.FundingSummaries
                        where f.OrganizationTypeID == Constant.OrganizationTypeId.NSO && f.SportID == sportId && f.YearID == yearId && f.FundingStatus != Constant.FundingStatus.SysAloc
                            select f).Count() != 0;
            }
            else if (fundingType == Constant.OrganizationTypeId.CSC)
            {
                return (from f in this._repository.FundingSummaries
                        where f.OrganizationTypeID == Constant.OrganizationTypeId.CSC && f.OrganizationID == organizationId && f.YearID == yearId && f.FundingStatus != Constant.FundingStatus.SysAloc
                        select f).Count() != 0;
            }
            else if (fundingType == Constant.OrganizationTypeId.OTP)
            {
                return (from f in this._repository.FundingSummaries
                        where f.OrganizationTypeID == Constant.OrganizationTypeId.OTP && f.OrganizationID == projectId && f.YearID == yearId && f.FundingStatus != Constant.FundingStatus.SysAloc
                        select f).Count() != 0;
            }

            return false;
        }

        public FundingAllocation GetFundingAllocation(string type, string yearId, string level1SportId)
        {
            //Retreive availabe funds
            FundingAllocation allocation = null;

            if (string.IsNullOrEmpty(level1SportId))
            {
                allocation = (from f in this._repository.FundingSummaries
                              where f.FundingStatus == Constant.FundingStatus.SysAloc
                                && f.OrganizationTypeID == type && f.YearID == yearId
                              select new FundingAllocation
                              {
                                  Id = f.FundingSummaryID,
                                  Type = f.OrganizationTypeID,
                                  SportId = f.SportID,
                                  YearId = f.YearID,
                                  AvailableFundsSC = f.FundingSC,
                                  AvailableFundsCoc = f.FundingCOC,
                                  AvailableFundsOther = f.FundingOther
                              }).FirstOrDefault();
            }
            else
            {
                allocation = (from f in this._repository.FundingSummaries
                              where f.FundingStatus == Constant.FundingStatus.SysAloc
                                && f.OrganizationTypeID == type && f.YearID == yearId && f.SportID == level1SportId 
                              select new FundingAllocation
                              {
                                  Id = f.FundingSummaryID,
                                  Type = f.OrganizationTypeID,
                                  SportId = f.SportID,
                                  YearId = f.YearID,
                                  AvailableFundsSC = f.FundingSC,
                                  AvailableFundsCoc = f.FundingCOC,
                                  AvailableFundsOther = f.FundingOther
                              }).FirstOrDefault();
            }

            if (allocation == null)
            {
                allocation = new FundingAllocation { Type = type, SportId = level1SportId, YearId = yearId };
            }

            //Retrieve allocated funds
            IEnumerable<Data.Models.FundingSummary> dataFundingSummaries;

            if (string.IsNullOrEmpty(level1SportId))
            {
                dataFundingSummaries = (from s in this._repository.FundingSummaries
                                        where s.FundingStatus != Constant.FundingStatus.SysAloc
                                        && s.OrganizationTypeID == type && s.YearID == yearId
                                        select s).ToList();

            }
            else
            {
                dataFundingSummaries = (from s in this._repository.FundingSummaries
                                            join sportL3 in this._repository.Sports on s.SportID equals sportL3.SportID 
                                            join sportL2 in this._repository.Sports on sportL3.ParentSportID equals sportL2.SportID 
                                        where s.OrganizationTypeID == type && s.YearID == yearId && s.FundingStatus != Constant.FundingStatus.SysAloc
                                            && sportL3.SportLevelID == Constant.SportLevelId.Sport && sportL2.SportLevelID == Constant.SportLevelId.Season && sportL2.ParentSportID == level1SportId 
                                        select s).ToList();
            }

            if (dataFundingSummaries == null || dataFundingSummaries.Count() == 0) return allocation;

            allocation.Fundings = new List<Funding>();

            foreach (var dataFundingSummary in dataFundingSummaries)
            {
                allocation.Fundings.Add(this.GetFunding(dataFundingSummary.FundingSummaryID));
            }

            allocation = CalculateFundingAllocation(allocation);
            
            return allocation;
        }

#endregion
        
#region PRIVATE

        private IEnumerable<Decode> GetAllResources()
        {
            var resources = from r in this._repository.Resources
                            where r.ResourceTypeID == Constant.ResourceType.ISTMembers || r.ResourceTypeID == Constant.ResourceType.NSOMembers
                            select new
                            {
                                IntId = r.ResourceID,
                                FirstName = r.FirstName,
                                LastName = r.LastName,
                                ResourceTypeID = r.ResourceTypeID
                            };

            var list =
                (from r in resources.AsEnumerable()
                 orderby r.ResourceTypeID, r.LastName, r.FirstName
                 select new Decode
                 {
                     IntId = r.IntId,
                     Name = ResourceBL.FormatResourceName(r.IntId, r.LastName, r.FirstName, r.ResourceTypeID)
                 }).ToList();

            return list;
        }

        private class FundingStatusChange
        {
            public int CategoryId { get; set; }
            public FundingDetail FundingDetail { get; set; }
        }

        private int UpdateFundingDetailForStatusChange(int id, string fromStatus, string toStatus)
        {
            int result = 0;
            List<FundingStatusChange> newFundingDetails = new List<FundingStatusChange>();
            Funding funding = GetFunding(id);

            if (funding == null || funding.FundingBlocks == null) return result;

            foreach (var fundingBlock in funding.FundingBlocks)
            {
                if (fundingBlock == null || fundingBlock.FundingCategories == null) return result;

                foreach (var fundingCategory in fundingBlock.FundingCategories)
                {
                    if (fundingCategory == null || fundingCategory.FundingDetails == null) return result;

                    int fundingCategoryId = fundingCategory.Id;

                    foreach (var fundingDetail in fundingCategory.FundingDetails)
                    {
                        newFundingDetails.Add(new FundingStatusChange { CategoryId = fundingCategoryId, FundingDetail = ConvertFundingDetailForStatusChange(fundingDetail, fromStatus, toStatus) });
                    }
                }
            }

            if (newFundingDetails.Count != 0)
            {
                bool addRevisionVersion = false;
                string revisionType = string.Empty;
                string revisionName = string.Empty;
                int versionId = 0;

                if (toStatus == Constant.FundingStatus.Allocated)
                {
                    if (fromStatus == Constant.FundingStatus.Approved)
                    {
                        addRevisionVersion = true;
                        revisionType = "FNL";
                        revisionName = "Final Recommendation";
                    }
                    if (fromStatus == Constant.FundingStatus.Revise)
                    {
                        addRevisionVersion = true;
                        revisionType = "REV";
                        revisionName = "Revision";
                    }
                }

                //Add RevisionVersion 
                if (addRevisionVersion)
                {
                    DateTime rightNow = DateTime.Now;

                    var versionOutput = this._repository.uspAddFundingRevisionVersion(
                        string.Format("{0}_{1}", revisionType, rightNow.ToString("yyyyMMddHHmmss")),
                        id,
                        string.Format("{0} - {1} ({2})", revisionName, rightNow.ToString("MMMM dd, yyyy"), rightNow.ToString("HHmmss")),
                        string.Format("{0} - {1} ({2})", revisionName, rightNow.ToString("MMMM dd, yyyy"), rightNow.ToString("HHmmss")),
                        rightNow,
                        new TimeSpan(rightNow.Hour, rightNow.Minute, rightNow.Second),
                        null).SingleOrDefault();

                    versionId = versionOutput == null ? 0 : (int)versionOutput;
                }

                foreach (var item in newFundingDetails)
                { 
                    FundingDetail newFundingDetail = item.FundingDetail;
                    int fundingCategoryId = item.CategoryId;

                    //Update fundingdetail
                    var dataFundingDetails = (from d in this._repository.FundingDetails
                                             where d.FundingDetailID == newFundingDetail.Id
                                             select d).ToList();

                    if (dataFundingDetails != null && dataFundingDetails.Count() == 1)
                    {
                        var output = this._repository.uspUpdateFundingDetail(
                            newFundingDetail.Id,
                            fundingCategoryId,
                            newFundingDetail.Description,
                            newFundingDetail.CscId,
                            newFundingDetail.ResourceId,
                            newFundingDetail.CommittedPercent,
                            newFundingDetail.PriorityCode,
                            newFundingDetail.OriginalBudget,
                            newFundingDetail.Actual,
                            newFundingDetail.RequestNso,
                            newFundingDetail.RequestSC,
                            newFundingDetail.RequestOtp,
                            newFundingDetail.HpaRecommend,
                            newFundingDetail.OtpRecommend,
                            newFundingDetail.FinalRecommend,
                            newFundingDetail.RecommendSC,
                            newFundingDetail.RecommendCoc,
                            newFundingDetail.Revised,
                            newFundingDetail.RevisedSC,
                            newFundingDetail.RevisedCoc,
                            newFundingDetail.PendingRevise,
                            newFundingDetail.PendingReviseSC,
                            newFundingDetail.PendingReviseCoc,
                            newFundingDetail.Recurring,
                            newFundingDetail.Comment).SingleOrDefault();

                        result += output == null ? result : (int)output;
                    }

                    //Add Revision
                    if (addRevisionVersion)
                    {
                        var revisionOutput = this._repository.uspAddFundingRevision(
                            versionId,
                            newFundingDetail.Id,
                            newFundingDetail.Description,
                            newFundingDetail.CscId,
                            newFundingDetail.ResourceId,
                            newFundingDetail.CommittedPercent,
                            newFundingDetail.PriorityCode,
                            newFundingDetail.Recurring,
                            newFundingDetail.Revised,
                            newFundingDetail.RevisedSC,
                            newFundingDetail.RevisedCoc).SingleOrDefault();
                    }
                }
            }

            return result;
        }

        private FundingDetail ConvertFundingDetailForStatusChange(FundingDetail oldFundingDetail, string fromStatus, string toStatus)
        {
            FundingDetail newFundingDetail = new FundingDetail();

            newFundingDetail.Id = oldFundingDetail.Id;
            newFundingDetail.Description = oldFundingDetail.Description;
            newFundingDetail.CscId = oldFundingDetail.CscId;
            newFundingDetail.ResourceId = oldFundingDetail.ResourceId;
            newFundingDetail.CommittedPercent = oldFundingDetail.CommittedPercent;
            newFundingDetail.PriorityCode = oldFundingDetail.PriorityCode;
            newFundingDetail.Recurring = oldFundingDetail.Recurring;
            newFundingDetail.OriginalBudget = oldFundingDetail.OriginalBudget;
            newFundingDetail.Actual = oldFundingDetail.Actual;
            newFundingDetail.RequestNso = oldFundingDetail.RequestNso;
            newFundingDetail.RequestSC = oldFundingDetail.RequestSC;
            newFundingDetail.RequestOtp = oldFundingDetail.RequestOtp;

            newFundingDetail.HpaRecommend = oldFundingDetail.HpaRecommend;
			if (toStatus == Constant.FundingStatus.Requested)
			{
				if (!oldFundingDetail.HpaRecommend.HasValue || (oldFundingDetail.HpaRecommend.HasValue ? oldFundingDetail.HpaRecommend.Value == 0 : false))
					newFundingDetail.HpaRecommend = oldFundingDetail.RequestOtp;
			}

            newFundingDetail.OtpRecommend = oldFundingDetail.OtpRecommend;
            if (toStatus == Constant.FundingStatus.HpaRecommended)
            {
                newFundingDetail.OtpRecommend = oldFundingDetail.HpaRecommend;
            }

            newFundingDetail.FinalRecommend = oldFundingDetail.FinalRecommend;
            newFundingDetail.RecommendCoc = oldFundingDetail.RecommendCoc;
            newFundingDetail.RecommendSC = oldFundingDetail.RecommendSC;

            if (toStatus == Constant.FundingStatus.OtpRecommended)
            {
                newFundingDetail.FinalRecommend = oldFundingDetail.OtpRecommend;
                newFundingDetail.RecommendSC = newFundingDetail.FinalRecommend;
            }

            newFundingDetail.Revised = oldFundingDetail.Revised;
            newFundingDetail.RevisedCoc = oldFundingDetail.RevisedCoc;
            newFundingDetail.RevisedSC = oldFundingDetail.RevisedSC;
            if (toStatus == Constant.FundingStatus.Allocated)
            {
                if (fromStatus == Constant.FundingStatus.Approved)
                {
                    //version 1
                    newFundingDetail.Revised = oldFundingDetail.FinalRecommend;
                    newFundingDetail.RevisedCoc = oldFundingDetail.RecommendCoc;
                    newFundingDetail.RevisedSC = oldFundingDetail.RecommendSC;
                }
                if (fromStatus == Constant.FundingStatus.Revise)
                { 
                    //following versions
                    newFundingDetail.Revised = oldFundingDetail.PendingRevise;
                    newFundingDetail.RevisedCoc = oldFundingDetail.PendingReviseCoc;
                    newFundingDetail.RevisedSC = oldFundingDetail.PendingReviseSC;
                }
            }

            newFundingDetail.PendingRevise = oldFundingDetail.PendingRevise;
            newFundingDetail.PendingReviseCoc = oldFundingDetail.PendingReviseCoc;
            newFundingDetail.PendingReviseSC = oldFundingDetail.PendingReviseSC;
            if (toStatus == Constant.FundingStatus.Revise)
            {
                newFundingDetail.PendingRevise = oldFundingDetail.Revised;
                newFundingDetail.PendingReviseCoc = oldFundingDetail.RevisedCoc;
                newFundingDetail.PendingReviseSC = oldFundingDetail.RevisedSC;
            }

            newFundingDetail.Comment = oldFundingDetail.Comment;

            return newFundingDetail;
        }

        private int EvaluateAndCreateActionItem(int? originalResourceId, FundingDetail postedFundingDetail, string actionName, int fundingId)
        {
            int result = 0;

            //if resource is changed, check if new resource tbd'd resource
            if (originalResourceId != postedFundingDetail.ResourceId && postedFundingDetail.ResourceId.HasValue)
            {
                var resource = (from r in this._repository.Resources
                            where r.ResourceID == postedFundingDetail.ResourceId.Value
                            select new { pool = r.Pooled, tbd = r.TBD }).SingleOrDefault();

                if (resource.tbd)
                {
                    var funding = (from f in this._repository.FundingSummaries
                                    where f.FundingSummaryID == fundingId
                                    select new { orgId = f.OrganizationID, sportId = f.SportID }).SingleOrDefault();

                    string type = Constant.ActionItemType.FundingRequest;
                    if (actionName == Constant.FundingAction.Request) type = Constant.ActionItemType.FundingRequest;
                    if (actionName == Constant.FundingAction.Recommend) type = Constant.ActionItemType.FundingRecommendation;
                    if (actionName == Constant.FundingAction.Revise) type = Constant.ActionItemType.FundingRevision;

                    ActionItem actionItem = new ActionItem() 
                    { 
                        TypeId = type,
                        ReferenceId = fundingId.ToString(),
                        OrganizationId = funding.orgId,
                        SportId = funding.sportId,
                        Description = "TBD resource for Funding",
                        DueDate = DateTime.Now.AddDays(7),
                        Status = Constant.ActionItemStatus.New,
                        Priority = Constant.ActionItemPriority.Medium,
                        Comment = "TBD resource is used for Funding Detail line " + postedFundingDetail.Description
                    };

                    result = this._actionItemBL.CreateActionItem(actionItem);
                }
            }

            return result;
        }

        private int SaveFunding(Funding funding)
        {
            int result = 0;

            if (funding == null) return result;

            var saveFundingOutput = this._repository.uspAddFundingSummary(funding.Type, 
                funding.OrganizationId, 
                funding.SportId, 
                funding.StatusCode, 
                funding.Comment, 
                funding.YearId,
                funding.AvailableFundsSC,
                funding.AvailableFundsCoc,
                funding.AvailableFundsOther).SingleOrDefault();

            if (saveFundingOutput != null)
            {
                int fundingId = (int)saveFundingOutput;

                if (funding.FundingBlocks != null)
                {
                    foreach (FundingBlock fundingBlock in funding.FundingBlocks)
                    {
                        if (fundingBlock == null) continue;

                        var saveFundingBlockOutput = this._repository.uspAddFundingBlock(fundingId, fundingBlock.Type, fundingBlock.Comment).SingleOrDefault();

                        if (saveFundingBlockOutput != null)
                        {
                            int fundingBlockId = (int)saveFundingBlockOutput;

                            if (fundingBlock.FundingCategories != null)
                            { 
                                foreach (FundingCategory fundingCategory in fundingBlock.FundingCategories)
                                {
                                    if (fundingCategory == null) continue;

                                    var saveFundingCategoryOutput = this._repository.uspAddFundingCategory(fundingBlockId, fundingCategory.Type, fundingCategory.Comment).SingleOrDefault();

                                    if (saveFundingCategoryOutput != null)
                                    {
                                        int fundingCategoryId = (int)saveFundingCategoryOutput;

                                        if (fundingCategory.FundingDetails != null)
                                        {
                                            foreach (FundingDetail fundingDetail in fundingCategory.FundingDetails)
                                            {
                                                if (fundingDetail == null) continue;

                                                var saveFundingDetailOutput = this._repository.uspAddFundingDetail(fundingCategoryId,
                                                    fundingDetail.Description,
                                                    fundingDetail.CscId,
                                                    fundingDetail.ResourceId,
                                                    fundingDetail.CommittedPercent,
                                                    fundingDetail.PriorityCode,
                                                    fundingDetail.OriginalBudget,
                                                    fundingDetail.Actual,
                                                    fundingDetail.RequestNso,
                                                    fundingDetail.RequestSC,
                                                    fundingDetail.RequestOtp,
                                                    fundingDetail.HpaRecommend,
                                                    fundingDetail.OtpRecommend,
                                                    fundingDetail.FinalRecommend,
                                                    fundingDetail.RecommendSC,
                                                    fundingDetail.RecommendCoc,
                                                    fundingDetail.Revised,
                                                    fundingDetail.RevisedSC,
                                                    fundingDetail.RevisedCoc,
                                                    fundingDetail.PendingRevise,
                                                    fundingDetail.PendingReviseSC,
                                                    fundingDetail.PendingReviseCoc,
                                                    fundingDetail.Recurring,
                                                    fundingDetail.Comment).SingleOrDefault();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        private Funding BuildNewFunding(string fundingType, string projectId, string organizationId, string sportId, string yearId)
        {
            Funding newFunding = new Funding();

            //populate funding head information
            newFunding.Type = fundingType;
            newFunding.YearId = yearId;
            newFunding.StatusCode = Constant.FundingStatus.New;

            if (newFunding.Type == Constant.OrganizationTypeId.NSO)
            {
                newFunding.SportId = sportId;

                //NSO is based on sportId, use sportId to retrieve organizationId, just for the information
                IEnumerable<Data.Models.Sport> sports = this._sportBL.GetAllSportsToSportLevel();
                newFunding.OrganizationId = (from s in sports
                                          where s.SportID == sportId
                                          select s.OrganizationID).SingleOrDefault();
            }
            else if (newFunding.Type == Constant.OrganizationTypeId.CSC)
            {
                newFunding.OrganizationId = organizationId;
            }
            else //remainings are treated as OTP special projects
            {
                newFunding.Type = Constant.OrganizationTypeId.OTP;
                newFunding.OrganizationId = projectId;
            }

            //build funding structure
            var fundingStructure = (from s in this._repository.FundingBlockOrganizationTypes
                                    where s.OrganizationTypeID == fundingType
                                    select s).ToList();

            newFunding.FundingBlocks = new List<FundingBlock>();

            foreach (var fundingBlockType in fundingStructure)
            {
                if (!fundingBlockType.FundingBlockType.Active) continue;

                FundingBlock newFundingBlock = new FundingBlock() { Type = fundingBlockType.FundingBlockTypeID, FundingCategories = new List<FundingCategory>() };

                foreach (var fundingCategoryType in fundingBlockType.FundingBlockType.FundingCategoryTypes)
                {
                    if (!fundingCategoryType.Active) continue;

                    FundingCategory newFundingCategory = new FundingCategory() { Type = fundingCategoryType.FundingCategoryTypeID, FundingDetails = new List<FundingDetail>() };
                    newFundingBlock.FundingCategories.Add(newFundingCategory);
                }

                newFunding.FundingBlocks.Add(newFundingBlock);
            }

            return newFunding;
        }

        private IEnumerable<FundingSummary> PadFundingSummaries(IEnumerable<FundingSummary> fundingSummaries)
        { 
            //There has to be three FundingSummaries, NSO, CSC and OTP
            if (fundingSummaries == null) fundingSummaries = new List<FundingSummary>();

            if ((from s in fundingSummaries where s.Type == Constant.OrganizationTypeId.NSO select s).Count() == 0)
                ((List<FundingSummary>)fundingSummaries).Add(new FundingSummary { Type = Constant.OrganizationTypeId.NSO, Fundings = new List<Funding>() });

            if ((from s in fundingSummaries where s.Type == Constant.OrganizationTypeId.CSC select s).Count() == 0)
                ((List<FundingSummary>)fundingSummaries).Add(new FundingSummary { Type = Constant.OrganizationTypeId.CSC, Fundings = new List<Funding>() });

            if ((from s in fundingSummaries where s.Type == Constant.OrganizationTypeId.OTP select s).Count() == 0)
                ((List<FundingSummary>)fundingSummaries).Add(new FundingSummary { Type = Constant.OrganizationTypeId.OTP, Fundings = new List<Funding>() });

            return fundingSummaries;
        }

        private IEnumerable<FundingSummary> CalculateFundingSummaries(IEnumerable<FundingSummary> fundingSummaries)
        {
            if (fundingSummaries == null) return fundingSummaries;

            foreach (var fundingSummary in fundingSummaries)
            {
                fundingSummary.OriginalBudget = (from f in fundingSummary.Fundings select f.OriginalBudget).Sum();
                fundingSummary.RequestOtp = (from f in fundingSummary.Fundings select f.RequestOtp).Sum();
                fundingSummary.HpaRecommend = (from f in fundingSummary.Fundings select f.HpaRecommend).Sum();
                fundingSummary.OtpRecommend = (from f in fundingSummary.Fundings select f.OtpRecommend).Sum();
                fundingSummary.FinalRecommend = (from f in fundingSummary.Fundings select f.FinalRecommend).Sum();
                fundingSummary.Revised = (from f in fundingSummary.Fundings select f.Revised).Sum();
                fundingSummary.Paid = (from f in fundingSummary.Fundings select f.Paid).Sum();
                fundingSummary.Actual = (from f in fundingSummary.Fundings select f.Actual).Sum();
                fundingSummary.RevisedBudget = (from f in fundingSummary.Fundings select f.RevisedBudget).Sum();
            }

            return fundingSummaries;
        }

        private IEnumerable<FundingSummary> SortFundingSummaries(IEnumerable<FundingSummary> fundingSummaries)
        {
            ((List<FundingSummary>)fundingSummaries).Sort(new FundingSummary.Comparer());

            foreach (var fundingSummary in fundingSummaries)
            {
                fundingSummary.Fundings.Sort(new Funding.Comparer());
            }

            return fundingSummaries;
        }

        private IEnumerable<FundingSummary> PopulateCodeDescription(IEnumerable<FundingSummary> fundingSummaries)
        {
            if (fundingSummaries == null) return fundingSummaries;

            IEnumerable<Decode> orgTypes = this._decodeBL.GetOrganizationTypes();

            foreach (var fundingSummary in fundingSummaries)
            {
                fundingSummary.TypeDesc = (from t in orgTypes where t.Id == fundingSummary.Type select t.Name).SingleOrDefault();
            }

            return fundingSummaries;
        }

        private Funding PopulateCodeDescription(Funding funding)
        {
            if (funding == null) return funding;

            IEnumerable<FundingBlockType> blockTypes = this.GetFundingBlockTypes();
            IEnumerable<FundingCategoryType> categoryTypes = this.GetFundingCategoryTypes();
            IEnumerable<Organization> orgs = this._decodeBL.GetAllOrganizations();
            IEnumerable<Decode> resources = this.GetAllResources();
            IEnumerable<Decode> priorities = this._decodeBL.GetDecodesByCategory(Constant.DecodeCategoryId.FundingPriority);
            IEnumerable<Decode> status = this._decodeBL.GetDecodesByCategory(Constant.DecodeCategoryId.FundingStatus);
            IEnumerable<ListItem> sports = this._sportBL.GetAllSportsOnSportLevel();

            funding.OrganizationDesc = (from o in orgs where o.Id == funding.OrganizationId select o.Name).SingleOrDefault();
            funding.SportDesc = (from s in sports where s.Id == funding.SportId select s.Description).SingleOrDefault();
            funding.StatusDesc = (from s in status where s.Id == funding.StatusCode select s.Name).SingleOrDefault();

            if (funding.FundingBlocks == null) return funding;

            foreach (var fundingBlock in funding.FundingBlocks)
            {
                if (fundingBlock == null) continue;

                fundingBlock.TypeDesc = (from t in blockTypes where t.Type == fundingBlock.Type select t.Name).SingleOrDefault();

                if (fundingBlock.FundingCategories != null)
                {
                    foreach (var fundingCategory in fundingBlock.FundingCategories)
                    {
                        if (fundingCategory == null) continue;

                        FundingCategoryType categoryType = (from t in categoryTypes where t.Type == fundingCategory.Type select t).SingleOrDefault();

                        if (categoryType != null)
                        {
                            fundingCategory.TypeDesc = categoryType.Name;
                            fundingCategory.ResourceType = categoryType.ResourceType;
                            fundingCategory.ResourceRequired = categoryType.ResourceRequired;
                        }

                        if (fundingCategory.FundingDetails != null)
                        {
                            foreach (var fundingDetail in fundingCategory.FundingDetails)
                            {
                                if (fundingDetail == null) continue;

                                fundingDetail.CscDesc = (from t in orgs where t.Id == fundingDetail.CscId select t.Name).SingleOrDefault();
                                fundingDetail.ResourceDesc = (from t in resources where t.IntId == fundingDetail.ResourceId select t.Name).SingleOrDefault();
                                fundingDetail.PriorityDesc = (from t in priorities where t.Id == fundingDetail.PriorityCode select t.Name).SingleOrDefault();
                            }
                        }
                    }

                    //Sort FundingCategories
                    fundingBlock.FundingCategories = (from c in fundingBlock.FundingCategories 
                               join t in categoryTypes on c.Type equals t.Type 
                               orderby t.SortOrder 
                               select c).ToList();
                }
            }

            //Sort FundingBlocks
            funding.FundingBlocks = (from b in funding.FundingBlocks
                      join t in blockTypes on b.Type equals t.Type
                      orderby t.SortOrder
                      select b).ToList();

            return funding;
        }

        private FundingDetail CopyFundingDetail(FundingDetail oldFundingDetail, bool copyRequestAmount, bool copyAmount, string copyAmountType)
        {
            FundingDetail newFundingDetail = new FundingDetail();

            newFundingDetail.Description = oldFundingDetail.Description;
            newFundingDetail.CscId = oldFundingDetail.CscId;
            newFundingDetail.ResourceId = oldFundingDetail.ResourceId;
            newFundingDetail.CommittedPercent = oldFundingDetail.CommittedPercent;
            newFundingDetail.PriorityCode = oldFundingDetail.PriorityCode;
            newFundingDetail.Recurring = oldFundingDetail.Recurring;

            if (copyRequestAmount)
            {
                newFundingDetail.OriginalBudget = oldFundingDetail.Revised;

                newFundingDetail.RequestOtp = oldFundingDetail.RequestOtp;
                newFundingDetail.RequestSC = oldFundingDetail.RequestSC;
                newFundingDetail.RequestNso = oldFundingDetail.RequestNso;
            }

            if (copyAmount)
            {
                if (copyAmountType == Constant.FundingCopyAmountType.Revised)
                {
                    newFundingDetail.HpaRecommend = oldFundingDetail.Revised;
                }
                else if (copyAmountType == Constant.FundingCopyAmountType.FinalRecommend)
                {
                    newFundingDetail.HpaRecommend = oldFundingDetail.FinalRecommend;
                }
                else if (copyAmountType == Constant.FundingCopyAmountType.OtpRecommend)
                {
                    newFundingDetail.HpaRecommend = oldFundingDetail.OtpRecommend;
                }
                else if (copyAmountType == Constant.FundingCopyAmountType.HpaRecommend)
                {
                    newFundingDetail.HpaRecommend = oldFundingDetail.HpaRecommend;
                }
            }

            return newFundingDetail;
        }

        private FundingCategory LocateFundingCategory(Funding funding, string fundingBlockType, string fundingCategoryType)
        {
            if (funding == null || funding.FundingBlocks == null) return null;

            FundingBlock fundingBlock = (from b in funding.FundingBlocks
                                         where b.Type == fundingBlockType
                                         select b).FirstOrDefault();

            if (fundingBlock == null || fundingBlock.FundingCategories == null) return null;

            return (from c in fundingBlock.FundingCategories
                                               where c.Type == fundingCategoryType
                                               select c).FirstOrDefault();
        }

        private Funding CalculateFunding(Funding funding)
        {
            if (funding == null || funding.FundingBlocks == null) return funding;

            foreach (var fundingBlock in funding.FundingBlocks)
            {
                if (fundingBlock == null) continue;

                if (fundingBlock.FundingCategories != null)
                {
                    foreach (var fundingCategory in fundingBlock.FundingCategories)
                    {

                        if (fundingCategory == null) continue;

                        if (fundingCategory.FundingDetails != null)
                        {

                            foreach (var fundingDetail in fundingCategory.FundingDetails)
                            {
                                if (fundingDetail == null) continue;

                                //Deal with nullable decimal
                                fundingDetail.CommittedPercent = fundingDetail.CommittedPercent ?? 0m;
                                fundingDetail.OriginalBudget = fundingDetail.OriginalBudget ?? 0m;
                                fundingDetail.RequestOtp = fundingDetail.RequestOtp ?? 0m;
                                fundingDetail.RequestNso = fundingDetail.RequestNso ?? 0m;
                                fundingDetail.RequestSC = fundingDetail.RequestSC ?? 0m;
                                fundingDetail.RequestOther = fundingDetail.RequestOther ?? 0m;
                                fundingDetail.HpaRecommend = fundingDetail.HpaRecommend ?? 0m;
                                fundingDetail.OtpRecommend = fundingDetail.OtpRecommend ?? 0m;
                                fundingDetail.FinalRecommend = fundingDetail.FinalRecommend ?? 0m;
                                fundingDetail.RecommendCoc = fundingDetail.RecommendCoc ?? 0m;
                                fundingDetail.RecommendSC = fundingDetail.RecommendSC ?? 0m;
                                fundingDetail.RecommendOther = fundingDetail.RecommendOther ?? 0m;
                                fundingDetail.Revised = fundingDetail.Revised ?? 0m;
                                fundingDetail.RevisedCoc = fundingDetail.RevisedCoc ?? 0m;
                                fundingDetail.RevisedSC = fundingDetail.RevisedSC ?? 0m;
                                fundingDetail.RevisedOther = fundingDetail.RevisedOther ?? 0m;
                                fundingDetail.PendingRevise = fundingDetail.PendingRevise ?? 0m;
                                fundingDetail.PendingReviseSC = fundingDetail.PendingReviseSC ?? 0m;
                                fundingDetail.PendingReviseCoc = fundingDetail.PendingReviseCoc ?? 0m;
                                fundingDetail.PendingReviseOther = fundingDetail.PendingReviseOther ?? 0m;
                                fundingDetail.Actual = fundingDetail.Actual ?? 0m;
                                fundingDetail.RevisedBudget = fundingDetail.RevisedBudget ?? 0m;
                                fundingDetail.Variance = fundingDetail.Variance ?? 0m;

                                //deal with calculated fields
                                fundingDetail.RequestOther = fundingDetail.OriginalBudget - fundingDetail.RequestOtp - fundingDetail.RequestNso - fundingDetail.RequestSC;
                                fundingDetail.RecommendOther = fundingDetail.FinalRecommend - fundingDetail.RecommendCoc - fundingDetail.RecommendSC;
                                fundingDetail.RevisedOther = fundingDetail.Revised - fundingDetail.RevisedCoc - fundingDetail.RevisedSC;
                                fundingDetail.PendingReviseOther = fundingDetail.PendingRevise - fundingDetail.PendingReviseCoc - fundingDetail.PendingReviseSC;
                                if (funding.StatusCode == Constant.FundingStatus.Approved 
                                    || funding.StatusCode == Constant.FundingStatus.Allocated 
                                    || funding.StatusCode == Constant.FundingStatus.Revise 
                                    || funding.StatusCode == Constant.FundingStatus.Actualize 
                                    || funding.StatusCode == Constant.FundingStatus.Closed)
                                {
                                    fundingDetail.RevisedBudget = fundingDetail.OriginalBudget - fundingDetail.RequestOtp + fundingDetail.FinalRecommend;
                                    fundingDetail.Variance = fundingDetail.RevisedBudget - fundingDetail.Actual;
                                }
                            }

                            fundingCategory.OriginalBudget = (from d in fundingCategory.FundingDetails select d.OriginalBudget).Sum();
                            fundingCategory.RequestOtp = (from d in fundingCategory.FundingDetails select d.RequestOtp).Sum();
                            fundingCategory.RequestNso = (from d in fundingCategory.FundingDetails select d.RequestNso).Sum();
                            fundingCategory.RequestSC = (from d in fundingCategory.FundingDetails select d.RequestSC).Sum();
                            fundingCategory.RequestOther = (from d in fundingCategory.FundingDetails select d.RequestOther).Sum();
                            fundingCategory.HpaRecommend = (from d in fundingCategory.FundingDetails select d.HpaRecommend).Sum();
                            fundingCategory.OtpRecommend = (from d in fundingCategory.FundingDetails select d.OtpRecommend).Sum();
                            fundingCategory.FinalRecommend = (from d in fundingCategory.FundingDetails select d.FinalRecommend).Sum();
                            fundingCategory.RecommendCoc = (from d in fundingCategory.FundingDetails select d.RecommendCoc).Sum();
                            fundingCategory.RecommendSC = (from d in fundingCategory.FundingDetails select d.RecommendSC).Sum();
                            fundingCategory.RecommendOther = (from d in fundingCategory.FundingDetails select d.RecommendOther).Sum();
                            fundingCategory.Revised = (from d in fundingCategory.FundingDetails select d.Revised).Sum();
                            fundingCategory.RevisedCoc = (from d in fundingCategory.FundingDetails select d.RevisedCoc).Sum();
                            fundingCategory.RevisedSC = (from d in fundingCategory.FundingDetails select d.RevisedSC).Sum();
                            fundingCategory.RevisedOther = (from d in fundingCategory.FundingDetails select d.RevisedOther).Sum();
                            fundingCategory.PendingRevise = (from d in fundingCategory.FundingDetails select d.PendingRevise).Sum();
                            fundingCategory.PendingReviseCoc = (from d in fundingCategory.FundingDetails select d.PendingReviseCoc).Sum();
                            fundingCategory.PendingReviseSC = (from d in fundingCategory.FundingDetails select d.PendingReviseSC).Sum();
                            fundingCategory.PendingReviseOther = (from d in fundingCategory.FundingDetails select d.PendingReviseOther).Sum();
                            fundingCategory.Actual = (from d in fundingCategory.FundingDetails select d.Actual).Sum();
                            fundingCategory.RevisedBudget = (from d in fundingCategory.FundingDetails select d.RevisedBudget).Sum();
                            fundingCategory.Variance = (from d in fundingCategory.FundingDetails select d.Variance).Sum();
                        }
                    }

                    fundingBlock.OriginalBudget = (from c in fundingBlock.FundingCategories select c.OriginalBudget).Sum();
                    fundingBlock.RequestOtp = (from c in fundingBlock.FundingCategories select c.RequestOtp).Sum();
                    fundingBlock.RequestNso = (from c in fundingBlock.FundingCategories select c.RequestNso).Sum();
                    fundingBlock.RequestSC = (from c in fundingBlock.FundingCategories select c.RequestSC).Sum();
                    fundingBlock.RequestOther = (from c in fundingBlock.FundingCategories select c.RequestOther).Sum();
                    fundingBlock.HpaRecommend = (from c in fundingBlock.FundingCategories select c.HpaRecommend).Sum();
                    fundingBlock.OtpRecommend = (from c in fundingBlock.FundingCategories select c.OtpRecommend).Sum();
                    fundingBlock.FinalRecommend = (from c in fundingBlock.FundingCategories select c.FinalRecommend).Sum();
                    fundingBlock.RecommendCoc = (from c in fundingBlock.FundingCategories select c.RecommendCoc).Sum();
                    fundingBlock.RecommendSC = (from c in fundingBlock.FundingCategories select c.RecommendSC).Sum();
                    fundingBlock.RecommendOther = (from c in fundingBlock.FundingCategories select c.RecommendOther).Sum();
                    fundingBlock.Revised = (from c in fundingBlock.FundingCategories select c.Revised).Sum();
                    fundingBlock.RevisedCoc = (from c in fundingBlock.FundingCategories select c.RevisedCoc).Sum();
                    fundingBlock.RevisedSC = (from c in fundingBlock.FundingCategories select c.RevisedSC).Sum();
                    fundingBlock.RevisedOther = (from c in fundingBlock.FundingCategories select c.RevisedOther).Sum();
                    fundingBlock.PendingRevise = (from c in fundingBlock.FundingCategories select c.PendingRevise).Sum();
                    fundingBlock.PendingReviseCoc = (from c in fundingBlock.FundingCategories select c.PendingReviseCoc).Sum();
                    fundingBlock.PendingReviseSC = (from c in fundingBlock.FundingCategories select c.PendingReviseSC).Sum();
                    fundingBlock.PendingReviseOther = (from c in fundingBlock.FundingCategories select c.PendingReviseOther).Sum();
                    fundingBlock.Actual = (from c in fundingBlock.FundingCategories select c.Actual).Sum();
                    fundingBlock.RevisedBudget = (from c in fundingBlock.FundingCategories select c.RevisedBudget).Sum();
                    fundingBlock.Variance = (from c in fundingBlock.FundingCategories select c.Variance).Sum();
                }
            }

            funding.OriginalBudget = (from b in funding.FundingBlocks select b.OriginalBudget).Sum();
            funding.RequestOtp = (from b in funding.FundingBlocks select b.RequestOtp).Sum();
            funding.HpaRecommend = (from b in funding.FundingBlocks select b.HpaRecommend).Sum();
            funding.OtpRecommend = (from b in funding.FundingBlocks select b.OtpRecommend).Sum();
            funding.FinalRecommend = (from b in funding.FundingBlocks select b.FinalRecommend).Sum();
            funding.RecommendSC = (from b in funding.FundingBlocks select b.RecommendSC).Sum();
            funding.RecommendCoc = (from b in funding.FundingBlocks select b.RecommendCoc).Sum();
            funding.RecommendOther = (from b in funding.FundingBlocks select b.RecommendOther).Sum();
            funding.Revised = (from b in funding.FundingBlocks select b.Revised).Sum();
            funding.Actual = (from b in funding.FundingBlocks select b.Actual).Sum();
            funding.RevisedBudget = (from b in funding.FundingBlocks select b.RevisedBudget).Sum();

            //TODO: funding.Paid, should come from Funding Tracking

            funding.ShowRevisedLink = funding.StatusCode == Constant.FundingStatus.Allocated || funding.StatusCode == Constant.FundingStatus.Revise || funding.StatusCode == Constant.FundingStatus.Actualize || funding.StatusCode == Constant.FundingStatus.Closed;
            funding.ShowFinalRecommendLink = funding.ShowRevisedLink || funding.StatusCode == Constant.FundingStatus.OtpRecommended || funding.StatusCode == Constant.FundingStatus.Approved;
            funding.ShowOtpRecommendLink = funding.ShowFinalRecommendLink || funding.StatusCode == Constant.FundingStatus.HpaRecommended;
            funding.ShowHpaRecommendLink = funding.ShowOtpRecommendLink || funding.StatusCode == Constant.FundingStatus.Requested;

            return funding;
        }

        private Funding CountFundingActionItem(Funding funding)
        {
            if (funding == null) return funding;

            string fundingId = funding.Id.ToString();

            var dataAllActionItems = (from a in this._repository.ActionItems
                                      join t in this._repository.ActionItemTypes on a.ActionItemTypeID equals t.ActionItemTypeID 
                                        where t.SourceName_EN == Constant.ActionItemSource.Funding
                                        && a.ReferenceID == fundingId
                                        select a).ToList();

            int all = dataAllActionItems.Count;

            int open = (from a in dataAllActionItems
                        where a.ActionItemStatus == Constant.ActionItemStatus.New
                            || a.ActionItemStatus == Constant.ActionItemStatus.InProgress
                        select a).Count();

            funding.ActionItem = string.Format("{0}/{1}", open, all);

            return funding;
        }

        private FundingAllocation CalculateFundingAllocation(FundingAllocation fundingAllocation)
        {
            if (fundingAllocation == null || fundingAllocation.Fundings == null) return fundingAllocation;

            fundingAllocation.AllocatedFundsSC = fundingAllocation.AllocatedFundsSC ?? 0m;
            fundingAllocation.AllocatedFundsCoc = fundingAllocation.AllocatedFundsCoc ?? 0m;
            fundingAllocation.AllocatedFundsOther = fundingAllocation.AllocatedFundsOther ?? 0m;
            fundingAllocation.AvailableFundsSC = fundingAllocation.AvailableFundsSC ?? 0m;
            fundingAllocation.AvailableFundsCoc = fundingAllocation.AvailableFundsCoc ?? 0m;
            fundingAllocation.AvailableFundsOther = fundingAllocation.AvailableFundsOther ?? 0m;

            fundingAllocation.AllocatedFundsSC = (from f in fundingAllocation.Fundings select f.RecommendSC).Sum();
            fundingAllocation.AllocatedFundsCoc = (from f in fundingAllocation.Fundings select f.RecommendCoc).Sum();
            fundingAllocation.AllocatedFundsOther = (from f in fundingAllocation.Fundings select f.RecommendOther).Sum();

            fundingAllocation.UnallocatedFundsSC = fundingAllocation.AvailableFundsSC - fundingAllocation.AllocatedFundsSC;
            fundingAllocation.UnallocatedFundsCoc = fundingAllocation.AvailableFundsCoc - fundingAllocation.AllocatedFundsCoc;
            fundingAllocation.UnallocatedFundsOther = fundingAllocation.AvailableFundsOther - fundingAllocation.AllocatedFundsOther;

            return fundingAllocation;
        }

        private Funding CalculateFundingTotal(Funding funding)
        {
            IEnumerable<FundingCategoryType> fundingCategoryTypes = GetFundingCategoryTypes();
             
            FundingTotal programSupportSummary = new FundingTotal { Type = Constant.FundingTotalType.ProgramSupport };
            FundingTotal istCscSummary = new FundingTotal { Type = Constant.FundingTotalType.IstCsc };
            FundingTotal istOtherSummary = new FundingTotal { Type = Constant.FundingTotalType.IstOther };
            FundingTotal totalSummary = new FundingTotal { Type = Constant.FundingTotalType.Total };

            if (funding == null || funding.FundingBlocks == null) return funding;

            foreach (var fundingBlock in funding.FundingBlocks)
            {
                if (fundingBlock == null || fundingBlock.FundingCategories == null) continue;

                //Program Support
                var fundingCategories = from c in fundingBlock.FundingCategories
                                        join t in fundingCategoryTypes on c.Type equals t.Type
                                        where t.BlockType != Constant.FundingBlockType.IST
                                        select c;
                programSupportSummary = SumFundingTotal(programSupportSummary, fundingCategories);

                //IST-CSC
                fundingCategories = from c in fundingBlock.FundingCategories
                                        join t in fundingCategoryTypes on c.Type equals t.Type
                                        where t.BlockType == Constant.FundingBlockType.IST && t.CscRequired 
                                        select c;
                istCscSummary = SumFundingTotal(istCscSummary, fundingCategories);

                //IST-Other
                fundingCategories = from c in fundingBlock.FundingCategories
                                    join t in fundingCategoryTypes on c.Type equals t.Type
                                    where t.BlockType == Constant.FundingBlockType.IST && !t.CscRequired
                                    select c;
                istOtherSummary = SumFundingTotal(istOtherSummary, fundingCategories);

                //Total
                fundingCategories = from c in fundingBlock.FundingCategories
                                        select c;
                totalSummary = SumFundingTotal(totalSummary, fundingCategories);
            }

            funding.FundingTotals = new List<FundingTotal>();

            funding.FundingTotals.Add(programSupportSummary);
            funding.FundingTotals.Add(istCscSummary);
            funding.FundingTotals.Add(istOtherSummary);
            funding.FundingTotals.Add(totalSummary);

            return funding;
        }

        private FundingTotal SumFundingTotal(FundingTotal fundingTotal, IEnumerable<FundingCategory> fundingCategories)
        {
            fundingTotal.OriginalBudget = SumNullableDecimal(fundingTotal.OriginalBudget, (from s in fundingCategories select s.OriginalBudget).Sum());
            fundingTotal.RequestOtp = SumNullableDecimal(fundingTotal.RequestOtp, (from s in fundingCategories select s.RequestOtp).Sum());
            fundingTotal.RequestNso = SumNullableDecimal(fundingTotal.RequestNso, (from s in fundingCategories select s.RequestNso).Sum());
            fundingTotal.RequestSC = SumNullableDecimal(fundingTotal.RequestSC, (from s in fundingCategories select s.RequestSC).Sum());
            fundingTotal.RequestOther = SumNullableDecimal(fundingTotal.RequestOther, (from s in fundingCategories select s.RequestOther).Sum());
            fundingTotal.HpaRecommend = SumNullableDecimal(fundingTotal.HpaRecommend, (from s in fundingCategories select s.HpaRecommend).Sum());
            fundingTotal.OtpRecommend = SumNullableDecimal(fundingTotal.OtpRecommend, (from s in fundingCategories select s.OtpRecommend).Sum());
            fundingTotal.FinalRecommend = SumNullableDecimal(fundingTotal.FinalRecommend, (from s in fundingCategories select s.FinalRecommend).Sum());
            fundingTotal.RecommendCoc = SumNullableDecimal(fundingTotal.RecommendCoc, (from s in fundingCategories select s.RecommendCoc).Sum());
            fundingTotal.RecommendSC = SumNullableDecimal(fundingTotal.RecommendSC, (from s in fundingCategories select s.RecommendSC).Sum());
            fundingTotal.RecommendOther = SumNullableDecimal(fundingTotal.RecommendOther, (from s in fundingCategories select s.RecommendOther).Sum());
            fundingTotal.Revised = SumNullableDecimal(fundingTotal.Revised, (from s in fundingCategories select s.Revised).Sum());
            fundingTotal.RevisedCoc = SumNullableDecimal(fundingTotal.RevisedCoc, (from s in fundingCategories select s.RevisedCoc).Sum());
            fundingTotal.RevisedSC = SumNullableDecimal(fundingTotal.RevisedSC, (from s in fundingCategories select s.RevisedSC).Sum());
            fundingTotal.RevisedOther = SumNullableDecimal(fundingTotal.RevisedOther, (from s in fundingCategories select s.RevisedOther).Sum());
            fundingTotal.PendingRevise = SumNullableDecimal(fundingTotal.PendingRevise, (from s in fundingCategories select s.PendingRevise).Sum());
            fundingTotal.PendingReviseCoc = SumNullableDecimal(fundingTotal.PendingReviseCoc, (from s in fundingCategories select s.PendingReviseCoc).Sum());
            fundingTotal.PendingReviseSC = SumNullableDecimal(fundingTotal.PendingReviseSC, (from s in fundingCategories select s.PendingReviseSC).Sum());
            fundingTotal.PendingReviseOther = SumNullableDecimal(fundingTotal.PendingReviseOther, (from s in fundingCategories select s.PendingReviseOther).Sum());
            fundingTotal.Actual = SumNullableDecimal(fundingTotal.Actual, (from s in fundingCategories select s.Actual).Sum());
            fundingTotal.RevisedBudget = SumNullableDecimal(fundingTotal.RevisedBudget, (from s in fundingCategories select s.RevisedBudget).Sum());
            fundingTotal.Variance = SumNullableDecimal(fundingTotal.Variance, (from s in fundingCategories select s.Variance).Sum());

            return fundingTotal;
        }

        private decimal? SumNullableDecimal(decimal? a, decimal? b)
        {
            return (a.HasValue ? a.Value : 0m) + (b.HasValue ? b.Value : 0m);
        }

        private Funding ConvertFunding(Data.Models.FundingSummary dataFunding)
        {
            if (dataFunding == null) return null;

            return new Funding 
            { 
                Id = dataFunding.FundingSummaryID,
                Type = dataFunding.OrganizationTypeID,
                OrganizationId = dataFunding.OrganizationID,
                SportId = dataFunding.SportID,
                YearId = dataFunding.YearID,
                StatusCode = dataFunding.FundingStatus,
                Comment = dataFunding.Comments,
                AvailableFundsSC = dataFunding.FundingSC,
                AvailableFundsCoc = dataFunding.FundingCOC,
                AvailableFundsOther = dataFunding.FundingOther,

                FundingBlocks = ConvertFundingBlocks(dataFunding)

                
            };
        }

        private List<FundingBlock> ConvertFundingBlocks(Data.Models.FundingSummary dataFunding)
        {
            List<FundingBlock> fundingBlocks = new List<FundingBlock>();

            if (dataFunding.FundingBlocks == null) return fundingBlocks;

            foreach (var dataFundingBlock in dataFunding.FundingBlocks)
            {
                fundingBlocks.Add(
                    new FundingBlock 
                    { 
                         Id = dataFundingBlock.FundingBlockID,
                         Type = dataFundingBlock.FundingBlockTypeID,
                         Comment = dataFundingBlock.Comments,
                         HasComment = !string.IsNullOrEmpty(dataFundingBlock.Comments),
                         FundingCategories = ConvertFundingCategories(dataFundingBlock)
                    });
            }

            return fundingBlocks;
        }

        private List<FundingCategory> ConvertFundingCategories(Data.Models.FundingBlock dataFundingBlock)
        {
            List<FundingCategory> fundingCategories = new List<FundingCategory>();

            if (dataFundingBlock.FundingCategories == null) return fundingCategories;

            foreach (var dataFundingCategory in dataFundingBlock.FundingCategories)
            {
                fundingCategories.Add(
                    new FundingCategory 
                    { 
                        Id = dataFundingCategory.FundingCategoryID,
                        Type = dataFundingCategory.FundingCategoryTypeID,
                        Comment = dataFundingCategory.Comments,
                        HasComment = !string.IsNullOrEmpty(dataFundingCategory.Comments),
                        FundingDetails = ConvertFundingDetails(dataFundingCategory)
                    });
            }

            return fundingCategories;
        }

        private List<FundingDetail> ConvertFundingDetails(Data.Models.FundingCategory dataFundingCategory)
        {
            List<FundingDetail> fundingDetails = new List<FundingDetail>();

            if (dataFundingCategory.FundingDetails == null) return fundingDetails;

            foreach (var dataFundingDetail in dataFundingCategory.FundingDetails)
            {
                fundingDetails.Add(new FundingDetail 
                {
                    Id = dataFundingDetail.FundingDetailID,
                    Description = dataFundingDetail.Description,
                    CscId = dataFundingDetail.OrganizationID,
                    ResourceId = dataFundingDetail.ResourceID,
                    CommittedPercent = dataFundingDetail.PercentCommitted,
                    PriorityCode = dataFundingDetail.FundingPriority,
                    Comment = dataFundingDetail.Comments,
                    HasComment = !string.IsNullOrEmpty(dataFundingDetail.Comments),

                    OriginalBudget = dataFundingDetail.RequestTotal,
                    RequestOtp = dataFundingDetail.RequestOTP,
                    RequestNso = dataFundingDetail.RequestNSO,
                    RequestSC = dataFundingDetail.RequestSC,

                    HpaRecommend = dataFundingDetail.RecommendTotalHPA,
                    OtpRecommend = dataFundingDetail.RecommendTotalOTP,
                    FinalRecommend = dataFundingDetail.RecommendTotal,
                    RecommendSC = dataFundingDetail.RecommendSC,
                    RecommendCoc = dataFundingDetail.RecommendCOC,

                    Revised = dataFundingDetail.FundsTotal,
                    RevisedSC = dataFundingDetail.FundsSC,
                    RevisedCoc = dataFundingDetail.FundsCOC,

                    PendingRevise = dataFundingDetail.RevisionTotal,
                    PendingReviseSC = dataFundingDetail.RevisionSC,
                    PendingReviseCoc = dataFundingDetail.RevisionCOC,

                    Recurring = dataFundingDetail.Recurring,

                    Actual = dataFundingDetail.ActualTotal 
                });
            }

            fundingDetails.Sort(new FundingDetail.Comparer());

            return fundingDetails;
        }

        private Data.Models.FundingDetail ConvertFundingDetail(Data.Models.FundingDetail dataFundingDetail, FundingDetail postedFundingDetail, string actionName)
        {
            dataFundingDetail.Description = postedFundingDetail.Description;
            dataFundingDetail.OrganizationID = postedFundingDetail.CscId;
            dataFundingDetail.ResourceID = postedFundingDetail.ResourceId;
            dataFundingDetail.PercentCommitted = postedFundingDetail.CommittedPercent;
            dataFundingDetail.FundingPriority = postedFundingDetail.PriorityCode;
            dataFundingDetail.Recurring = postedFundingDetail.Recurring;
            dataFundingDetail.Comments = string.IsNullOrEmpty(postedFundingDetail.Comment) ? dataFundingDetail.Comments : postedFundingDetail.Comment;

            if (actionName == Constant.FundingAction.Request)
            {
                dataFundingDetail.RequestTotal = postedFundingDetail.OriginalBudget;
                dataFundingDetail.RequestOTP = postedFundingDetail.RequestOtp;
                dataFundingDetail.RequestNSO = postedFundingDetail.RequestNso;
                dataFundingDetail.RequestSC = postedFundingDetail.RequestSC;

            }

            if (actionName == Constant.FundingAction.Recommend)
            {
                dataFundingDetail.RecommendTotalHPA = postedFundingDetail.HpaRecommend == null ? dataFundingDetail.RecommendTotalHPA : postedFundingDetail.HpaRecommend;
                dataFundingDetail.RecommendTotalOTP = postedFundingDetail.OtpRecommend == null ? dataFundingDetail.RecommendTotalOTP : postedFundingDetail.OtpRecommend;
                dataFundingDetail.RecommendTotal = postedFundingDetail.FinalRecommend == null ? dataFundingDetail.RecommendTotal : postedFundingDetail.FinalRecommend;
                dataFundingDetail.RecommendSC = postedFundingDetail.RecommendSC == null ? dataFundingDetail.RecommendSC : postedFundingDetail.RecommendSC;
                dataFundingDetail.RecommendCOC = postedFundingDetail.RecommendCoc == null ? dataFundingDetail.RecommendCOC : postedFundingDetail.RecommendCoc;
            }

            if (actionName == Constant.FundingAction.Revise)
            {
                dataFundingDetail.FundsTotal = postedFundingDetail.Revised == null ? dataFundingDetail.FundsTotal : postedFundingDetail.Revised;
                dataFundingDetail.FundsSC = postedFundingDetail.RevisedSC == null ? dataFundingDetail.FundsSC : postedFundingDetail.RevisedSC;
                dataFundingDetail.FundsCOC = postedFundingDetail.RevisedCoc == null ? dataFundingDetail.FundsCOC : postedFundingDetail.RevisedCoc;

                dataFundingDetail.RevisionTotal = postedFundingDetail.PendingRevise == null ? dataFundingDetail.RevisionTotal : postedFundingDetail.PendingRevise;
                dataFundingDetail.RevisionSC = postedFundingDetail.PendingReviseSC == null ? dataFundingDetail.RevisionSC : postedFundingDetail.PendingReviseSC;
                dataFundingDetail.RevisionCOC = postedFundingDetail.PendingReviseCoc == null ? dataFundingDetail.RevisionCOC : postedFundingDetail.PendingReviseCoc;

                dataFundingDetail.ActualTotal = postedFundingDetail.Actual == null ? dataFundingDetail.ActualTotal : postedFundingDetail.Actual;
            }

            return dataFundingDetail;
        
        }

        private void RemoveFundingFromCache(int id)
        {
            string cacheKey = string.Format(Constant.Cache.KeyTemplate, Constant.Cache.Funding, null, id);
            ObjectCache cache = MemoryCache.Default;
            if (cache.Contains(cacheKey))
            {
                cache.Remove(cacheKey);
            }
            return;
        }
#endregion

    }
}
