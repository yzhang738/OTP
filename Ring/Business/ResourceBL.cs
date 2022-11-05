using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using OTP.Ring.Data.Models;
using OTP.Ring.Common;
using OTP.Ring.Models.ViewModel;
using OTP.Ring.Models;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Transactions;
using System.Data.Objects;
using System.Data.Entity;

namespace OTP.Ring.Business
{
    public class ResourceBL : IResourceBL
    {
        private bool _frenchLanguage = false;
        private Data.Models.RingEntities _repository;
        private IDecodeBL _decodeBL;

        public ResourceBL(string connectionString, IDecodeBL decodeBL)
        {
            _frenchLanguage = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToUpper() == Constant.LanguageCode.French;
            _repository = new Data.Models.RingEntities(connectionString);
            this._decodeBL = decodeBL;
        }

        #region Resource

        public int AddResource(
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
            )
        {
            int changes = 0;

            using (TransactionScope scope = new TransactionScope())
            {
                var returnValue = this._repository.uspAddResource(
                    resourceTypeId,
                    resourceClassId,
                    resourceSpecialtyId,
                    firstName,
                    lastName,
                    countryId,
                    gender,
                    dateOfBirth,
                    mailingAddress,
                    mailingCity,
                    mailingProvinceId,
                    emailAddress,
                    hometownCity,
                    hometownProvinceId,
                    primaryCSCId,
                    secondaryCSCId,
                    comments,
                    active,
                    tbd,
                    pooled,
                    isNew).SingleOrDefault();

                var resourceId = returnValue == null ? changes : (int)returnValue;

                //Add Athlete Personal Coach, SportID == Constant.SportId.All in ResourceSports is used as Athlete personal coach
                if (resourceId != 0)
                {
                    changes = SavePersonalCoach(resourceId, personalCoachId, yearId);
                }

                if (!string.IsNullOrEmpty(assignSportId))
                {
                    AddResourceSport(
                        resourceId,
                        assignSportId,
                        yearId,
                        resourceClassId,
                        null,
                        null,
                        null,
                        null,
                        null);
                }

                scope.Complete();
            }

            return changes;
        }

        public int UpdateResource(
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
            )
        {
            int changes = 0;

            //Update Resource
            var updateResourceReturnValue = this._repository.uspUpdateResourceResourceSport(
                resourceId,
                resourceTypeId,
                resourceClassId,
                resourceSpecialtyId,
                firstName,
                lastName,
                countryId,
                gender,
                dateOfBirth,
                mailingAddress,
                mailingCity,
                mailingProvinceId,
                mailingPostalCode,
                mailingCountryId,
                homePhone,
                cellPhone,
                skypeName,
                emailAddress,
                hometownCity,
                hometownProvinceId,
                hometownCountryId,
                primaryCSCId,
                secondaryCSCId,
                active,
                tbd,
                pooled,
                isNew,
                comments,
                null,
                assignSportId,
                yearId,
                sportTrackingRef,
                personalCoachId, 
                null,
                null).SingleOrDefault();

            changes = updateResourceReturnValue == null ? changes : changes + (int)updateResourceReturnValue;            

            return changes;
        }

        public void DeleteReource(int resourceId)
        {
            int result = 0;

            var output = this._repository.uspDeleteResource(resourceId).SingleOrDefault();
            result = output == null ? result : (int)output;
        }

        public bool ShouldPrompResourceExists(string resourceTypeId, string firstName, string lastName, string countryId)
        {
            firstName = firstName.ToLower();
            lastName = lastName.ToLower();

            var exists = (from r in this._repository.Resources
                          where r.Active
                          && r.ResourceTypeID == resourceTypeId
                          && r.FirstName.ToLower().Equals(firstName)
                          && r.LastName.ToLower().Equals(lastName)
                          select r).Any();

            return exists;
        }

        public List<ListItem> GetResourceClasses()
        {
            List<ListItem> resourceClasses = null;

            resourceClasses = (from rc in this._repository.ResourceClasses
                               select new ListItem
                               {
                                   Id = rc.ResourceClassID,
                                   Description = _frenchLanguage ? rc.Name_FR : rc.Name_EN
                               }).OrderBy(rc => rc.Description).ToList();

            return resourceClasses;
        }

        public IEnumerable<Common.Decode> GetCoachResources(string sportId)
        {
            if (string.IsNullOrEmpty(sportId))
            {
                sportId = null;
            }

            var resources = (from r in this._repository.Resources
                             where r.Active
                             && r.ResourceSpecialty != null
                             && r.ResourceSpecialty.CoachIndicator
                             && (sportId == null ||
                                    (from s in r.ResourceSports
                                     where (s.Sport.SportLevelID == Constant.SportLevelId.Event
                                             && s.Sport.ParentSport.ParentSport.SportID == sportId)
                                             ||
                                             (s.Sport.SportLevelID == Constant.SportLevelId.Discipline
                                             && s.Sport.ParentSport.SportID == sportId)
                                             ||
                                             (s.Sport.SportLevelID == Constant.SportLevelId.Sport
                                             && s.SportID == sportId)
                                     select s).Any())
                             select r).ToList();

            return (from r in resources
                    select new Common.Decode
                    {
                        Id = r.ResourceID.ToString(),
                        IntId = r.ResourceID,
                        Name = FormatResourceName(r.ResourceID, r.LastName, r.FirstName, r.ResourceTypeID)
                    }).ToList();
        }

        public string GetResourceType(string id)
        {
            string resourceType = null;
            int resourceId = 0;

            if (!string.IsNullOrEmpty(id))
            {
                resourceId = int.Parse(id);
            }

            resourceType = (from r in this._repository.Resources
                            where r.ResourceID == resourceId
                            && r.Active
                            select r.ResourceTypeID).SingleOrDefault();

            return resourceType;
        }

        public List<ListItem> GetResourceSpecialties(string resourceType, bool includeNullItem = false)
        {
            List<ListItem> specialties = null;

            specialties = (from s in this._repository.ResourceSpecialties
                           where s.ResourceTypeID == resourceType
                           select new ListItem
                           {
                               Id = s.ResourceSpecialtyID,
                               Description = _frenchLanguage ? s.Name_FR : s.Name_EN
                           }).OrderBy(s => s.Description).ToList();

            if (includeNullItem)
            {
                specialties.InsertNullListItem(_frenchLanguage);
            }

            return specialties;
        }

        private string Parse3rdLevelSportId(string sportId)
        {
            if (string.IsNullOrEmpty(sportId)) return null;

            var sport = (from s in this._repository.Sports
                         where s.SportID == sportId
                         select s).FirstOrDefault();

            if (sport == null) return null;

            if (sport.SportLevelID == Constant.SportLevelId.Sport)
            {
                return sportId.Substring(0, sportId.Length - 2);
            }
            else
            {
                return sportId;
            }
        }

        public static string FormatResourceName(int resourceId, string lastName, string firstName, string orgType)
        {
            return string.Format("{1}, {2} [{0}] - {3}", orgType, lastName, firstName, resourceId);
        }

        public string GetResourceNameByResourceId(int resourceId)
        {
            var name = (from r in this._repository.Resources
                        where r.ResourceID == resourceId
                        select new
                        {
                            firstname = r.FirstName,
                            lastname = r.LastName
                        }).FirstOrDefault();

            return name.firstname + " " + name.lastname;
        }

        #endregion Resource

        #region Resource Affiliate

        public int AddResourceAffiliate(
	        string affiliateTypeId,
	        int resourceId,
	        string yearFromId,
	        string yearToId,
	        string affiliateName,
            string description,
	        string affiliateContact,
	        string affiliateEmail,
	        string comments)
        {
            int changes = 0;

            var returnValue = this._repository.uspAddResourceAffiliate(
                affiliateTypeId,
                resourceId,
                yearFromId,
                yearToId,
                affiliateName,
                description,
                affiliateContact,
                affiliateEmail,
                comments).SingleOrDefault();

            changes = returnValue == null ? changes : (int)returnValue;

            return changes;
        }

        public int UpdateResourceAffiliate(
            int resourceAffiliateId,
            string affiliateTypeId = Constant.UndefinedString,
            int? resourceId = null,
            string yearFromId = Constant.UndefinedString,
            string yearToId = Constant.UndefinedString,
            string affiliateName = Constant.UndefinedString,
            string description = Constant.UndefinedString,
            string affiliateContact = Constant.UndefinedString,
            string affiliateEmail = Constant.UndefinedString,
            string comments = Constant.UndefinedString)
        {
            int changes = 0;

            var affiliate = (from r in this._repository.ResourceAffiliates
                            where r.ResourceAffiliateID == resourceAffiliateId
                            select r).First();

            affiliate.AffiliateTypeID = affiliateTypeId == Constant.UndefinedString ? affiliate.AffiliateTypeID : affiliateTypeId;
            affiliate.ResourceID = resourceId ?? affiliate.ResourceID;
            affiliate.YearFromID = yearFromId == Constant.UndefinedString ? affiliate.YearFromID : yearFromId;
            affiliate.YearToID = yearToId == Constant.UndefinedString ? affiliate.YearToID : yearToId;
            affiliate.AffiliateName = affiliateName == Constant.UndefinedString ? affiliate.AffiliateName : affiliateName;
            affiliate.Description = description == Constant.UndefinedString ? affiliate.Description : description;
            affiliate.AffiliateContact = affiliateContact == Constant.UndefinedString ? affiliate.AffiliateContact : affiliateContact;
            affiliate.AffiliateEmail = affiliateEmail == Constant.UndefinedString ? affiliate.AffiliateEmail : affiliateEmail;
            affiliate.Comments = comments == Constant.UndefinedString ? affiliate.Comments : comments;

            //Update Resource Affiliate
            var updateAffiliateReturnValue = this._repository.uspUpdateResourceAffiliate(
                affiliate.ResourceAffiliateID,
                affiliate.AffiliateTypeID,
                affiliate.ResourceID,
                affiliate.YearFromID,
                affiliate.YearToID,
                affiliate.AffiliateName,
                affiliate.Description,
                affiliate.AffiliateContact,
                affiliate.AffiliateEmail,
                affiliate.Comments).SingleOrDefault();

            changes = updateAffiliateReturnValue == null ? changes : changes + (int)updateAffiliateReturnValue;

            return changes;
        }

        public int DeleteResourceAffiliate(int resourceAffiliateId)
        {
            int result = 0;

            var output = this._repository.uspDeleteResourceAffiliate(resourceAffiliateId).SingleOrDefault();
            result = output == null ? result : (int)output;

            return result;
        }

        public int GetResourceIdByAthleteAffiliate(int resourceAthletesAffiliateId)
        {
            return (from d in this._repository.ResourceAffiliates
                    where d.ResourceAffiliateID == resourceAthletesAffiliateId
                    select d.ResourceID).FirstOrDefault();
        }

        public List<int> GetResourceAffiliateIdsByResourceTypeNameYear(int resourceId, string affiliateTypeId, string affiliateName, string yearFromId)
        {
            return (from s in this._repository.ResourceAffiliates
                    where s.ResourceID == resourceId && s.AffiliateTypeID == affiliateTypeId && s.AffiliateName == affiliateName && s.YearFromID == yearFromId
                    select s.ResourceAffiliateID).ToList();
        }

        #endregion Resource Affiliate

        #region Resource Direct Athlete Support

        public int AddResourceDirectAthleteSupport(
            int resourceId,
            string directAthleteSupportId,
            string yearId,
            string supportName,
            string description,
            string supportContact,
            string supportEmail,
            decimal? amount,
            string comments)
        {
            int changes = 0;

            var returnValue = this._repository.uspAddResourceDirectAthleteSupport(
                resourceId,
                directAthleteSupportId,
                yearId,
                supportName,
                description,
                supportContact,
                supportEmail,
                amount,
                comments).SingleOrDefault();

            changes = returnValue == null ? changes : (int)returnValue;

            return changes;
        }

        public int UpdateResourceDirectAthleteSupport(
            int resourceDirectAthleteSupportId,
            int? resourceId = null,
            string directAthleteSupportId = Constant.UndefinedString,
            string yearId = Constant.UndefinedString,
            string supportName = Constant.UndefinedString,
            string description = Constant.UndefinedString,
            string supportContact = Constant.UndefinedString,
            string supportEmail = Constant.UndefinedString,
            decimal? amount = null,
            string comments = Constant.UndefinedString)
        {
            int changes = 0;

            var support = (from s in this._repository.ResourceDirectAthleteSupports
                                where s.ResourceDirectAthleteSupportID == resourceDirectAthleteSupportId
                                select s).First();

            support.ResourceID = resourceId ?? support.ResourceID;
            support.DirectAthleteSupportID = directAthleteSupportId == Constant.UndefinedString ? support.DirectAthleteSupportID : directAthleteSupportId;
            support.YearID = yearId == Constant.UndefinedString ? support.YearID : yearId;
            support.SupportName = supportName == Constant.UndefinedString ? support.SupportName : supportName;
            support.Description = description == Constant.UndefinedString ? support.Description : description;
            support.SupportContact = supportContact == Constant.UndefinedString ? support.SupportContact : supportContact;
            support.SupportEmail = supportEmail == Constant.UndefinedString ? support.SupportEmail : supportEmail;
            support.Amount = amount;
            support.Comments = comments == Constant.UndefinedString ? support.Comments : comments;

            //Update Resource Direct Athlete Support
            var updateDASReturnValue = this._repository.uspUpdateResourceDirectAthleteSupport(
                support.ResourceDirectAthleteSupportID,
                support.ResourceID,
                support.DirectAthleteSupportID,
                support.YearID,
                support.SupportName,
                support.Description,
                support.SupportContact,
                support.SupportEmail,
                support.Amount,
                support.Comments).SingleOrDefault();

            changes = updateDASReturnValue == null ? changes : changes + (int)updateDASReturnValue;

            return changes;
        }

        public int DeleteResourceDirectAthleteSupport(int resourceDirectAthleteSupportId)
        {
            int result = 0;

            var output = this._repository.uspDeleteResourceDirectAthleteSupport(resourceDirectAthleteSupportId).SingleOrDefault();
            result = output == null ? result : (int)output;

            return result;
        }

        public int GetResourceIdByDirectAthleteSupport(int resourceDirectAthleteSupportId)
        {
            return (from d in this._repository.ResourceDirectAthleteSupports
                    where d.ResourceDirectAthleteSupportID == resourceDirectAthleteSupportId
                    select d.ResourceID).FirstOrDefault();
        }

        public List<int> GetResourceDirectAthleteSupportIdsByResourceSupportYear(int resourceId, string yearId, string directAthleteSupportId)
        {
            return (from s in this._repository.ResourceDirectAthleteSupports
                    where s.ResourceID == resourceId && s.YearID == yearId && s.DirectAthleteSupportID == directAthleteSupportId
                    select s.ResourceDirectAthleteSupportID).ToList();
        }

        #endregion Resource Direct Athlete Support

        #region Resource Results

        public int AddResourceResults(
            int resourceID,
            int competitionID,
            string sportID_Level5,
            DateTime eventDate,
            int position,
            TimeSpan? perfTime,
            decimal? perfValue, 
            string comments)
        {
            var resultID = this._repository.uspAddResult(resourceID, competitionID, sportID_Level5, eventDate, position, perfTime, perfValue, comments).SingleOrDefault();

            return (resultID == null) ? 0 : (int)resultID;
        }

        public void UpdateResourceResults(
            int resultID,
            int competitionID,
            DateTime eventDate,
            string sportID_Level5,
            int position,
            TimeSpan? perfTime,
            decimal? perfValue,
            string comments)
        {
            // Update Result table
            int result = 0;

            var output = this._repository.uspUpdateResult(resultID, competitionID, sportID_Level5, eventDate, position, perfTime, perfValue, comments).SingleOrDefault();
            result = output == null ? result : (int)output;
        }

        public void DeleteResourceResults(int resultId)
        {
            int result = 0;

            var output = this._repository.uspDeleteResult(resultId).SingleOrDefault();
            result = output == null ? result : (int)output;
        }

        public int? GetResourceIDByResult(int resultId)
        {
            var resourceList =  (from r in this._repository.ResultResources
                    where r.ResultID == resultId
                    select r.ResourceID).ToList();

            return (resourceList == null) ? 0 : resourceList[0];
        }

        public int GetResourceResultsByCompetition(string competitionType, DateTime competitionDate, string sportId, string location, string countryId)
        {
            var competitionList = (from c in this._repository.Competitions
                                   where (c.CompetitionTypeID == competitionType && c.SportID == sportId && c.Location == location && c.CountryID == countryId)
                                   select new
                                   {
                                       c.CompetitionID,
                                       c.CompetitionDate
                                   }).ToList();

            foreach (var competition in competitionList)
            {
                if (competition.CompetitionDate.Year == competitionDate.Year && competition.CompetitionDate.Month == competitionDate.Month)
                    return competition.CompetitionID;
            }

            return 0;
        }

        public List<int> GetResourceResultsBySportEvent(int resourceId, string sportId, int competitionId)
        {
            return (from r in this._repository.Results
                    join rr in this._repository.ResultResources
                    on r.ResultID equals rr.ResultID
                    where (rr.ResourceID == (int?)resourceId && r.SportID == sportId && r.CompetitionID == competitionId)
                    select r.ResultID).ToList();
        }

        #endregion Resource Results

        #region Benchmark Resource

        public int AddBenchmarkResource(
 	        int resourceSportId,
	        int benchmarkTypeId,
	        int benchmarkTierId,
	        string yearId,
	        int? nsoValue,
	        int? otpValue,
	        string comments,
	        bool active)
        {
            int changes = 0;

            var returnValue = this._repository.uspAddBenchmarkResource(
                resourceSportId,
                benchmarkTypeId,
                benchmarkTierId,
                yearId,
                nsoValue,
                otpValue,
                comments,
                active).SingleOrDefault();

            changes = returnValue == null ? changes : (int)returnValue;

            return changes;
        }

        public int UpdateBenchmarkResource(
            int benchmarkResourceId,
            int? resourceSportId = null,
            int? benchmarkTypeId = null,
            int? benchmarkTierId = null,
            string yearId = Constant.UndefinedString,
            int? nsoValue = null,
            int? otpValue = null,
            string comments = Constant.UndefinedString,
            bool? active = null)
        {
            int changes = 0;

            var benchmark = (from b in this._repository.BenchmarkResources
                                where b.BenchmarkResourceID == benchmarkResourceId
                                && b.Active
                                select b).First();

            benchmark.ResourceSportID = resourceSportId ?? benchmark.ResourceSportID;
            benchmark.BenchmarkTypeID = benchmarkTypeId ?? benchmark.BenchmarkTypeID;
            benchmark.BenchmarkTierID = benchmarkTierId ?? benchmark.BenchmarkTierID;
            benchmark.YearID = yearId == Constant.UndefinedString ? benchmark.YearID : yearId;
            benchmark.NSOValue = nsoValue ?? benchmark.NSOValue;
            benchmark.OTPValue = otpValue ?? benchmark.OTPValue;
            benchmark.Comments = comments == Constant.UndefinedString ? benchmark.Comments : comments;
            benchmark.Active = active ?? benchmark.Active;

            var returnValue = this._repository.uspUpdateBenchmarkResource(
                benchmark.BenchmarkResourceID,
                benchmark.ResourceSportID,
                benchmark.BenchmarkTypeID,
                benchmark.BenchmarkTierID,
                benchmark.YearID,
                benchmark.NSOValue,
                benchmark.OTPValue,
                benchmark.Comments,
                benchmark.Active).SingleOrDefault();

            changes = returnValue == null ? changes : changes + (int)returnValue;

            return changes;
        }

        public int DeleteBenchmarkResource(int benchmarkResourceId)
        {
            int result = 0;

            var output = this._repository.uspDeleteBenchmarkResource(benchmarkResourceId).SingleOrDefault();
            result = output == null ? result : (int)output;

            return result;
        }

        public List<int> GetBenchmarkResourceIdsByResourceSportYearTypeTier(int resourceSportId, string yearId, int benchmarkTypeId, int benchmarkTierId)
        {
            return (from s in this._repository.BenchmarkResources
                    where s.ResourceSportID == resourceSportId && s.YearID == yearId && s.BenchmarkTypeID == benchmarkTypeId && s.BenchmarkTierID == benchmarkTierId
                    select s.BenchmarkResourceID).ToList();
        }

        #endregion Benchmark Resource

        #region Resource Sport

        public int AddResourceSport(
	       int resourceId,
	       string sportId,
	       string yearId,
	       string resourceClassId,
	       string comments,
           string sportTrackingRef,
           int?  coachResourceId,
           bool? istCrossCompetency,
           bool? istPrimary)
        {
            int changes = 0;

            var returnValue = this._repository.uspAddResourceSport(
                resourceId,
                sportId,
                yearId,
                resourceClassId,
                comments,
                sportTrackingRef,
                coachResourceId,
                istCrossCompetency,
                istPrimary).SingleOrDefault();

            changes = returnValue == null ? changes : (int)returnValue;

            return changes;
        }

        public int UpdateResourceSport(
            int resourceSportId,
            int? resourceId = null,
            string sportId = Constant.UndefinedString,
            string yearId = Constant.UndefinedString,
            string resourceClassId = Constant.UndefinedString,
            string comment = Constant.UndefinedString,
            string sportTrackingRef = Constant.UndefinedString,
            int? coachResourceId = null,
            bool? istCrossCompetency = null,
            bool? istPrimary = null)
        {
            int changes = 0;

            var sport = (from s in this._repository.ResourceSports
                            where s.ResourceSportID == resourceSportId
                            select s).First();

            sport.ResourceID = resourceId ?? sport.ResourceID;
            sport.SportID = sportId == Constant.UndefinedString ? sport.SportID : sportId;
            sport.YearID = yearId == Constant.UndefinedString ? sport.YearID : yearId;
            sport.ResourceClassID = resourceClassId == Constant.UndefinedString ? sport.ResourceClassID : resourceClassId;
            sport.Comments = comment == Constant.UndefinedString ? sport.Comments : comment;
            sport.SportTrackingRef = sportTrackingRef == Constant.UndefinedString ? sport.SportTrackingRef : sportTrackingRef;
            sport.CoachResourceID = coachResourceId;
            sport.ISTCrossCompetency = istCrossCompetency;
            sport.ISTPrimary = istPrimary;

            var returnValue = this._repository.uspUpdateResourceSport(
                sport.ResourceSportID,
                sport.ResourceID,
                sport.SportID,
                sport.YearID,
                sport.ResourceClassID,
                sport.Comments,
                sport.SportTrackingRef,
                sport.CoachResourceID,
                sport.ISTCrossCompetency,
                sport.ISTPrimary).SingleOrDefault();

            changes = returnValue == null ? changes : changes + (int)returnValue;

            return changes;
        }

        public int SavePersonalCoach(int athleteId, int? coachId, string yearId)
        {
            int changes = 0;

            var resources = (from r in this._repository.ResourceSports
                             where r.ResourceID == athleteId && r.SportID == Constant.SportId.All 
                             select r).ToList();

            if (resources != null && resources.Count > 0)
            {
                var resource = resources.First();

                if (coachId.HasValue)
                {
                    var returnValue = this._repository.uspUpdateResourceSport(
                        resource.ResourceSportID,
                        resource.ResourceID,
                        resource.SportID,
                        yearId,
                        null,
                        null,
                        null,
                        coachId,
                        null,
                        null).SingleOrDefault();
                    changes = returnValue == null ? changes : (int)returnValue;
                }
                else
                {
                    var returnValue = this._repository.uspDeleteResourceSport(resource.ResourceSportID).SingleOrDefault();
                    changes = returnValue == null ? changes : (int)returnValue;
                }
            }
            else
            {
                if (coachId.HasValue)
                {
                    var returnValue = this._repository.uspAddResourceSport(
                        athleteId,
                        Constant.SportId.All,
                        yearId,
                        null,
                        null,
                        null,
                        coachId,
                        null,
                        null).SingleOrDefault();
                    changes = returnValue == null ? changes : (int)returnValue;
                }
            }

            return changes;
        }

        public int DeleteResourceSport(int resourceSportId)
        {
            int result = 0;

            var output = this._repository.uspDeleteResourceSport(resourceSportId).SingleOrDefault();
            result = output == null ? result : (int)output;

            return result;
        }

        public int GetResourceIdByResourceSport(int resourceSportId)
        {
            return (from d in this._repository.ResourceSports
                    where d.ResourceSportID == resourceSportId
                    select d.ResourceID).FirstOrDefault();
        }

        public List<int> GetResourceSportIdsByResourceSport(int resourceId, string sportId, string yearId)
        {
            return (from s in this._repository.ResourceSports
                    where s.ResourceID == resourceId
                    && s.SportID == sportId
                    && s.YearID == yearId
                    select s.ResourceSportID).ToList();
        }

        #endregion Resource Sport

        #region Resource Qualification

        public int AddResourceQualification(
	        int resourceId,
	        string qualificationTypeId,
	        string description,
	        bool qualified,
	        string comments)
        {
            int changes = 0;

            var returnValue = this._repository.uspAddResourceQualification(
                resourceId,
                qualificationTypeId,
                description,
                qualified,
                comments).SingleOrDefault();

            changes = returnValue == null ? changes : (int)returnValue;

            return changes;
        }

        public int UpdateResourceQualification(
            int resourceQualificationId,
            int? resourceId = null,
            string qualificationTypeId = Constant.UndefinedString,
            string description = Constant.UndefinedString,
            bool? qualified = null,
            string comment = Constant.UndefinedString)
        {
            int changes = 0;

            var qualification = (from q in this._repository.ResourceQualifications
                                    where q.ResourceQualificationID == resourceQualificationId
                                    select q).First();

            qualification.ResourceID = resourceId ?? qualification.ResourceID;
            qualification.QualificationTypeID = qualificationTypeId == Constant.UndefinedString ? qualification.QualificationTypeID : qualificationTypeId;
            qualification.Description = description == Constant.UndefinedString ? qualification.Description : description;
            qualification.Qualified = qualified ?? qualification.Qualified;
            qualification.Comments = comment == Constant.UndefinedString ? qualification.Comments : comment;

            var returnValue = this._repository.uspUpdateResourceQualification(
                qualification.ResourceQualificationID,
                qualification.ResourceID,
                qualification.QualificationTypeID,
                qualification.Description,
                qualification.Qualified,
                qualification.Comments).SingleOrDefault();

            changes = returnValue == null ? changes : changes + (int)returnValue;

            return changes;
        }

        public int DeleteResourceQualification(int resourceQualificationId)
        {
            int result = 0;

            var output = this._repository.uspDeleteResourceQualification(resourceQualificationId).SingleOrDefault();
            result = output == null ? result : (int)output;

            return result;
        }

        public int GetResourceIdByResourceQualification(int resourceQualificationId)
        {
            return (from d in this._repository.ResourceQualifications
                    where d.ResourceQualificationID == resourceQualificationId
                    select d.ResourceID).FirstOrDefault();
        }

        public List<ListItem> GetResourceQualificationTypes()
        {
            List<ListItem> qualificationTypes = null;

            qualificationTypes = (from q in this._repository.QualificationTypes

                                  select new ListItem
                                  {
                                      Id = q.QualificationTypeID,
                                      Description = _frenchLanguage ? q.Name_FR : q.Name_EN
                                  }).OrderBy(q => q.Description).ToList();

            return qualificationTypes;
        }

        #endregion Resource Qualification

        #region Resource Pool

        public int AddResourcePoolAssignment(
            int resourceIdPool,
            int resourceIdAssignment)
        {
            int changes = 0;

            var returnValue = this._repository.uspAddResourcePoolAssignment(
                resourceIdPool,
                resourceIdAssignment,
                null).SingleOrDefault();

            changes = returnValue == null ? changes : (int)returnValue;

            return changes;
        }

        public int UpdateResourcePoolAssignment(
            int resourcePoolAssignmentId,
            int resourceIdPool,
            int resourceIdAssignment)
        {
            int changes = 0;

            var returnValue = this._repository.uspUpdateResourcePoolAssignment(
                resourcePoolAssignmentId,
                resourceIdPool,
                resourceIdAssignment,
                null).SingleOrDefault();

            changes = returnValue == null ? changes : (int)returnValue;

            return changes;
        }

        public int DeleteResourcePoolAssignment(int resourcePoolAssignmentId)
        {
            int result = 0;

            var output = this._repository.uspDeleteResourcePoolAssignment(resourcePoolAssignmentId).SingleOrDefault();
            result = output == null ? result : (int)output;

            return result;
        }

        public List<ListItem> GetResourcePoolAssignmentList(int resourceId)
        {
            var resource = (from r in this._repository.Resources
                            where r.ResourceID == resourceId
                            select r).SingleOrDefault();

            List<ListItem> assignmentValues = null;

            if (resource.Pooled)
            {
                var assignedResources = (from p in this._repository.ResourcePoolAssignments
                                         where p.ResourceIDPool == resource.ResourceID
                                         select p.ResourceIDAssignment).ToList();

                var results = (from r in this._repository.Resources
                               where !assignedResources.Contains(r.ResourceID)
                                && r.ResourceTypeID == resource.ResourceTypeID
                                && !r.Pooled
                                && r.Active
                               select new
                               {
                                   ResourceId = r.ResourceID,
                                   FirstName = r.FirstName,
                                   LastName = r.LastName,
                                   ResourceTypeName = _frenchLanguage ? r.ResourceType.Name_FR : r.ResourceType.Name_EN
                               }).ToList();

                assignmentValues = (from r in results
                                    orderby r.FirstName, r.LastName
                                    select new ListItem
                                    {
                                        Id = r.ResourceId.ToString(),
                                        Description = string.Format("{0} - {1} {2} [{3}]", r.ResourceId, r.FirstName, r.LastName, r.ResourceTypeName)
                                    }).ToList();
            }
            else
            {

                var pools = (from p in this._repository.ResourcePoolAssignments
                             where p.ResourceIDAssignment == resource.ResourceID
                             select p.ResourceIDPool).ToList();

                var results = (from r in this._repository.Resources
                               where !pools.Contains(r.ResourceID)
                                && r.ResourceTypeID == resource.ResourceTypeID
                                && r.Pooled
                                && r.Active
                               select new
                               {
                                   ResourceId = r.ResourceID,
                                   FirstName = r.FirstName,
                                   LastName = r.LastName,
                                   ResourceTypeName = _frenchLanguage ? r.ResourceType.Name_FR : r.ResourceType.Name_EN
                               }).ToList();

                assignmentValues = (from r in results
                                    orderby r.FirstName, r.LastName
                                    select new ListItem
                                    {
                                        Id = r.ResourceId.ToString(),
                                        Description = string.Format("{0} - {1} {2} [{3}]", r.ResourceId, r.FirstName, r.LastName, r.ResourceTypeName)
                                    }).ToList();
            }

            return assignmentValues;
        }

        #endregion Resource Pool

        #region Get Maintenance Athletes

        public MaintenanceAthletesGeneralInfoViewModel GetMaintenanceAthletesGeneralInfoViewModel(int resourceId)
        {
            MaintenanceAthletesGeneralInfoViewModel model = null;

            var resource = (from r in this._repository.Resources
                            where r.ResourceID == resourceId
                            select r).SingleOrDefault();

            if (resource != null)
            {
                model = new MaintenanceAthletesGeneralInfoViewModel(resource);

                //SportID == Constant.SportId.All in ResourceSports is used as Athlete personal coach
                model.PersonalCoachId = (from s in this._repository.ResourceSports where s.ResourceID == model.ResourceId && s.SportID == Constant.SportId.All select s.CoachResourceID).FirstOrDefault();

                model.Countries = (from c in this._repository.Countries
                                    select new ListItem
                                    {
                                        Id = c.CountryID,
                                        Description = _frenchLanguage ? c.Name_FR : c.Name_EN
                                    }).ToList();

                model.CSCs = new List<ListItem> { null };
                model.CSCs.AddRange((from o in this._repository.Organizations
                                where o.OrganizationTypeID == Constant.OrganizationTypeId.CSC
                                select new ListItem
                                {
                                    Id = o.OrganizationID,
                                    Description = _frenchLanguage ? o.Name_FR : o.Name_EN
                                }).ToList());

                model.Genders = this._decodeBL.GetHumanGenders(true);

                var resources = (from r in this._repository.Resources
                                 join s in this._repository.ResourceSpecialties on r.ResourceSpecialtyID equals s.ResourceSpecialtyID
                                 where s.CoachIndicator
                                 select r).ToList();
                model.CoachResources = new List<ListItem> { null };
                model.CoachResources.AddRange((from r in resources
                                               select new ListItem 
                                               {
                                                Id = r.ResourceID.ToString(),
                                                Description = r.FirstName + " " + r.LastName + " - " + r.ResourceID.ToString()
                                               }).ToList());
            }

            return model;
        }

        public MaintenanceAthletesContactInfoViewModel GetMaintenanceAthletesContactInfoViewModel(int resourceId)
        {
            MaintenanceAthletesContactInfoViewModel model = null;

            var resource = (from r in this._repository.Resources
                            where r.ResourceID == resourceId
                            && r.Active
                            select r).SingleOrDefault();

            if (resource != null)
            {
                model = new MaintenanceAthletesContactInfoViewModel(resource);

                model.Countries = new List<ListItem> { null };
                model.Countries.AddRange((from c in this._repository.Countries
                                    select new ListItem
                                    {
                                        Id = c.CountryID,
                                        Description = _frenchLanguage ? c.Name_FR : c.Name_EN
                                    }).ToList());

                model.Provinces = new List<ListItem> { null };
                model.Provinces.AddRange((from p in this._repository.Provinces
                                    select new ListItem
                                    {
                                        Id = p.ProvinceID,
                                        Description = _frenchLanguage ? p.Name_FR : p.Name_EN
                                    }).ToList());
            }

            return model;
        }

        public List<MaintenanceAthletesAthleteGridViewModel> GetMaintenanceAthletesAthleteGridViewModel(string selectedSportId, string yearId, GridCommand command, out int total)
        {
            total = 0;

            if (string.IsNullOrEmpty(selectedSportId))
            {
                selectedSportId = Constant.SportId.All;
            }

            //NOTE: Using "ResourceSports.SportID.StartsWith" to determine sport parent/child relationship is not reliable.
            //selectedSportId = Parse3rdLevelSportId(selectedSportId);

            SortDescriptor sort = new SortDescriptor()
            {
                Member = "",
                SortDirection = ListSortDirection.Ascending
            };

            if (command.SortDescriptors.Any())
            {
                sort = command.SortDescriptors[0];
            }

            string filterString = string.Empty;
            filterString = GetFilterString(filterString, command.FilterDescriptors);

            ObjectParameter parameter = new ObjectParameter("TotalCount", typeof(int));

            if (command.PageSize == 0)
            {
                command.PageSize = 10;
                command.Page = 1;
            }

            var query = this._repository.uspGetMaintenanceAthleteGridViewModel(
                                _frenchLanguage ? Constant.LanguageCode.French : Constant.LanguageCode.English,
                                Constant.ResourceType.Athletes,
                                selectedSportId,
                                yearId,
                                false,
                                command.PageSize,
                                command.Page,
                                command.SortDescriptors.Count,
                                sort.Member,
                                sort.SortDirection == ListSortDirection.Ascending ? "ASC" : "DESC",
                                command.FilterDescriptors.Count,
                                filterString,
                                parameter).ToList();

            List<MaintenanceAthletesAthleteGridViewModel> model = new List<MaintenanceAthletesAthleteGridViewModel>();

            foreach (var item in query)
            {
                MaintenanceAthletesAthleteGridViewModel modelItem = new MaintenanceAthletesAthleteGridViewModel();

                modelItem.LastName = item.LastName;
                modelItem.FirstName = item.FirstName;
                modelItem.CountryId = item.CountryId;
                modelItem.CountryName = item.CountryName;
                modelItem.BirthDate = item.BirthDate;
                modelItem.GenderId = item.GenderId;
                modelItem.GenderName = item.GenderName;
                modelItem.ResourceId = item.ResourceId;
                modelItem.Pooled = item.Pooled;
                modelItem.TBD = item.TBD;
                modelItem.IsNew = item.IsNew;
                modelItem.BenchmarksEntered = Convert.ToBoolean(item.BenchmarksEntered);
                if (item.Category != null)
                {
                    modelItem.Category = item.Category;
                }
                modelItem.PersonalCoachId = item.PersonalCoachId;
                if (item.PersonalCoachName != null)
                {
                    modelItem.PersonalCoachName = item.PersonalCoachName;
                }

                model.Add(modelItem);
            }

            total = (int)parameter.Value;

            return model;
        }

        //public List<MaintenanceAthletesAthleteGridViewModel> GetMaintenanceAthletesAthleteGridViewModel(string selectedSportId, string yearId, GridCommand command, out int total)
        //{
        //    total = 0;

        //    if (string.IsNullOrEmpty(selectedSportId))
        //    {
        //        selectedSportId = Constant.SportId.All;
        //    }

        //    // NOTE: Using "ResourceSports.SportID.StartsWith" to determine sport parent/child relationship is not reliable.
        //    selectedSportId = Parse3rdLevelSportId(selectedSportId);

        //    var modelQuery = (from r in this._repository.Resources
        //                      where r.ResourceTypeID == Constant.ResourceType.Athletes
        //                      && (selectedSportId == Constant.SportId.All ||
        //                      (from s in r.ResourceSports
        //                       where s.SportID.StartsWith(selectedSportId)
        //                       select s).Any())      // Only Athletes that have sports under the selected sport id
        //                      && r.Active
        //                      select new MaintenanceAthletesAthleteGridViewModel
        //                      {
        //                          GenderId = r.Gender,
        //                          GenderName = (from g in this._repository.Decodes
        //                                        where g.DecodeCategoryID == Constant.DecodeCategoryId.Gender
        //                                        && g.DecodeID == r.Gender
        //                                        select _frenchLanguage ? g.DecodeID_FR : g.DecodeID_EN).FirstOrDefault(),
        //                          FirstName = r.FirstName,
        //                          LastName = r.LastName,
        //                          ResourceId = r.ResourceID,
        //                          DateOfBirth = r.DateOfBirth,
        //                          ResourceSpecialtyId = r.ResourceSpecialty.ResourceSpecialtyID,
        //                          CountryId = r.CountryID,
        //                          CountryName = _frenchLanguage ? r.Country.Name_FR : r.Country.Name_EN,
        //                          Pooled = r.Pooled,
        //                          TBD = r.TBD,
        //                          IsNew = r.New,
        //                          ResourceSpecialtyName = _frenchLanguage ? r.ResourceSpecialty.Name_FR : r.ResourceSpecialty.Name_EN,
        //                          BenchmarksEntered = (from s in r.ResourceSports
        //                                               where selectedSportId == Constant.SportId.All
        //                                               || s.SportID.StartsWith(selectedSportId)
        //                                               from b in s.BenchmarkResources
        //                                               where b.Active
        //                                               && (yearId == null || b.YearID == yearId)
        //                                               select b).Any(),
        //                          Category = (from s in r.ResourceSports
        //                                      where (selectedSportId == Constant.SportId.All ||
        //                                      s.SportID.StartsWith(selectedSportId))
        //                                      && s.ResourceClass != null
        //                                      && (yearId == null || s.YearID == yearId)
        //                                      orderby s.ResourceClass.SortOrder
        //                                      select _frenchLanguage ? s.ResourceClass.Name_FR : s.ResourceClass.Name_EN).FirstOrDefault(),
        //                          PersonalCoachId = (from s in this._repository.ResourceSports
        //                                             where s.ResourceID == r.ResourceID && s.SportID == Constant.SportId.All && s.CoachResourceID != null
        //                                             select s.CoachResourceID).FirstOrDefault(),
        //                          PersonalCoachName = "",
        //                          CoachLastName = (from rc in this._repository.Resources
        //                                           where rc.ResourceID == (from s in this._repository.ResourceSports
        //                                                                   where s.ResourceID == r.ResourceID && s.SportID == Constant.SportId.All && s.CoachResourceID != null
        //                                                                   select s.CoachResourceID).FirstOrDefault()
        //                                           select rc.LastName).FirstOrDefault(),
        //                          CoachFirstName = (from rc in this._repository.Resources
        //                                            where rc.ResourceID == (from s in this._repository.ResourceSports
        //                                                                    where s.ResourceID == r.ResourceID && s.SportID == Constant.SportId.All && s.CoachResourceID != null
        //                                                                    select s.CoachResourceID).FirstOrDefault()
        //                                            select rc.FirstName).FirstOrDefault()
        //                      });

        //    // Apply filtering from the UI
        //    if (command.FilterDescriptors.Any())
        //    {
        //        modelQuery = FilterAthletes(modelQuery, command.FilterDescriptors);
        //    }

        //    // Default sorting
        //    if (command.SortDescriptors.Count == 0)
        //    {
        //        modelQuery = modelQuery.OrderBy(x => x.LastName).ThenBy(x => x.FirstName);
        //    }

        //    // Apply sorting from the UI
        //    foreach (SortDescriptor sortDescriptor in command.SortDescriptors)
        //    {
        //        if (sortDescriptor.SortDirection == ListSortDirection.Ascending)
        //        {
        //            switch (sortDescriptor.Member)
        //            {
        //                case "ResourceId":
        //                    modelQuery = modelQuery.OrderBy(x => x.ResourceId);
        //                    break;
        //                case "LastName":
        //                    modelQuery = modelQuery.OrderBy(x => x.LastName);
        //                    break;
        //                case "FirstName":
        //                    modelQuery = modelQuery.OrderBy(x => x.FirstName);
        //                    break;
        //                case "CountryName":
        //                    modelQuery = modelQuery.OrderBy(x => x.CountryId);
        //                    break;
        //                case "GenderName":
        //                    modelQuery = modelQuery.OrderBy(x => x.GenderName);
        //                    break;
        //                case "BirthDate":
        //                    modelQuery = modelQuery.OrderBy(x => x.DateOfBirth);
        //                    break;
        //                case "IsNew":
        //                    modelQuery = modelQuery.OrderBy(x => x.IsNew);
        //                    break;
        //                case "BenchmarksEntered":
        //                    modelQuery = modelQuery.OrderBy(x => x.BenchmarksEntered);
        //                    break;
        //                case "Category":
        //                    modelQuery = modelQuery.OrderBy(x => x.Category);
        //                    break;
        //                case "PersonalCoachName":
        //                    modelQuery = modelQuery.OrderBy(x => x.CoachFirstName);
        //                    break;
        //            }
        //        }
        //        else
        //        {
        //            switch (sortDescriptor.Member)
        //            {
        //                case "ResourceId":
        //                    modelQuery = modelQuery.OrderByDescending(x => x.ResourceId);
        //                    break;
        //                case "LastName":
        //                    modelQuery = modelQuery.OrderByDescending(x => x.LastName);
        //                    break;
        //                case "FirstName":
        //                    modelQuery = modelQuery.OrderByDescending(x => x.FirstName);
        //                    break;
        //                case "CountryName":
        //                    modelQuery = modelQuery.OrderByDescending(x => x.CountryId);
        //                    break;
        //                case "GenderName":
        //                    modelQuery = modelQuery.OrderByDescending(x => x.GenderName);
        //                    break;
        //                case "BirthDate":
        //                    modelQuery = modelQuery.OrderByDescending(x => x.DateOfBirth);
        //                    break;
        //                case "IsNew":
        //                    modelQuery = modelQuery.OrderByDescending(x => x.IsNew);
        //                    break;
        //                case "BenchmarksEntered":
        //                    modelQuery = modelQuery.OrderByDescending(x => x.BenchmarksEntered);
        //                    break;
        //                case "Category":
        //                    modelQuery = modelQuery.OrderByDescending(x => x.Category);
        //                    break;
        //                case "PersonalCoachName":
        //                    modelQuery = modelQuery.OrderByDescending(x => x.CoachFirstName);
        //                    break;
        //            }
        //        }
        //    }

        //    // Get the count for UI paging
        //    total = modelQuery.Count();

        //    // Set the paging size
        //    if (command.PageSize > 0)
        //    {
        //        //Set start index to take records from
        //        modelQuery = modelQuery.Skip((command.Page - 1) * command.PageSize);
        //    }

        //    // Return only the records needed to fit on the current page
        //    modelQuery = modelQuery.Take(command.PageSize);

        //    // Now set the model with the query elements
        //    var model = modelQuery.ToList();

        //    // Set the birth date string
        //    foreach (var item in model)
        //    {
        //        if (item.DateOfBirth != null)
        //        {
        //            item.BirthDate = string.Format("{0:yyyy/MM/dd}", item.DateOfBirth);
        //        }
        //        if (item.PersonalCoachId != null)
        //        {
        //            item.PersonalCoachName = string.Format("{0} {1} - {2}", item.CoachFirstName, item.CoachLastName, item.PersonalCoachId);
        //        }
        //    }

        //    return model;
        //}

        public List<MaintenanceAthletesAthleteGridViewModel> GetMaintenanceAthletesAthleteGridViewModelParameter(string selectedSportId, string yearId, GridCommand command)
        {
            if (string.IsNullOrEmpty(selectedSportId))
            {
                selectedSportId = Constant.SportId.All;
            }

            //NOTE: Using "ResourceSports.SportID.StartsWith" to determine sport parent/child relationship is not reliable.
            //selectedSportId = Parse3rdLevelSportId(selectedSportId);

            SortDescriptor sort = new SortDescriptor()
            {
                Member = "",
                SortDirection = ListSortDirection.Ascending
            };

            if (command.SortDescriptors.Any())
            {
                sort = command.SortDescriptors[0];
            }

            string filterString = string.Empty;
            filterString = GetFilterString(filterString, command.FilterDescriptors);

            ObjectParameter parameter = new ObjectParameter("TotalCount", typeof(int));

            var query = this._repository.uspGetMaintenanceAthleteGridViewModel(
                                _frenchLanguage ? Constant.LanguageCode.French : Constant.LanguageCode.English,
                                Constant.ResourceType.Athletes,
                                selectedSportId,
                                yearId,
                                true,
                                command.PageSize,
                                command.Page,
                                command.SortDescriptors.Count,
                                sort.Member,
                                sort.SortDirection == ListSortDirection.Ascending ? "ASC" : "DESC",
                                command.FilterDescriptors.Count,
                                filterString,
                                parameter).ToList();

            List<MaintenanceAthletesAthleteGridViewModel> model = new List<MaintenanceAthletesAthleteGridViewModel>();

            foreach (var item in query)
            {
                MaintenanceAthletesAthleteGridViewModel modelItem = new MaintenanceAthletesAthleteGridViewModel();

                modelItem.LastName = item.LastName;
                modelItem.FirstName = item.FirstName;
                modelItem.CountryId = item.CountryId;
                modelItem.CountryName = item.CountryName;
                modelItem.BirthDate = item.BirthDate;
                modelItem.GenderId = item.GenderId;
                modelItem.GenderName = item.GenderName;
                modelItem.ResourceId = item.ResourceId;
                modelItem.Pooled = item.Pooled;
                modelItem.TBD = item.TBD;
                modelItem.IsNew = item.IsNew;
                modelItem.BenchmarksEntered = Convert.ToBoolean(item.BenchmarksEntered);
                if (item.Category != null)
                {
                    modelItem.Category = item.Category;
                }
                modelItem.PersonalCoachId = item.PersonalCoachId;
                if (item.PersonalCoachName != null)
                {
                    modelItem.PersonalCoachName = item.PersonalCoachName;
                }

                model.Add(modelItem);
            }

            return model;
        }

        //public List<MaintenanceAthletesAthleteGridViewModel> GetMaintenanceAthletesAthleteGridViewModelParameter(string selectedSportId, string yearId, GridCommand command)
        //{
        //    if (string.IsNullOrEmpty(selectedSportId))
        //    {
        //        selectedSportId = Constant.SportId.All;
        //    }

        //    // NOTE: Using "ResourceSports.SportID.StartsWith" to determine sport parent/child relationship is not reliable.
        //    selectedSportId = Parse3rdLevelSportId(selectedSportId);

        //    var query = (from r in this._repository.Resources
        //                 where r.ResourceTypeID == Constant.ResourceType.Athletes
        //                 && (selectedSportId == Constant.SportId.All ||
        //                 (from s in r.ResourceSports
        //                  where s.SportID.StartsWith(selectedSportId)
        //                  select s).Any())      // Only Athletes that have sports under the selected sport id
        //                 && r.Active
        //                 select new MaintenanceAthletesAthleteGridViewModel
        //                 {
        //                     GenderId = r.Gender,
        //                     GenderName = (from g in this._repository.Decodes
        //                                   where g.DecodeCategoryID == Constant.DecodeCategoryId.Gender
        //                                   && g.DecodeID == r.Gender
        //                                   select _frenchLanguage ? g.DecodeID_FR : g.DecodeID_EN).FirstOrDefault(),
        //                     FirstName = r.FirstName,
        //                     LastName = r.LastName,
        //                     ResourceId = r.ResourceID,
        //                     DateOfBirth = r.DateOfBirth,
        //                     ResourceSpecialtyId = r.ResourceSpecialty.ResourceSpecialtyID,
        //                     CountryId = r.CountryID,
        //                     CountryName = _frenchLanguage ? r.Country.Name_FR : r.Country.Name_EN,
        //                     Pooled = r.Pooled,
        //                     TBD = r.TBD,
        //                     IsNew = r.New,
        //                     ResourceSpecialtyName = _frenchLanguage ? r.ResourceSpecialty.Name_FR : r.ResourceSpecialty.Name_EN,
        //                     BenchmarksEntered = (from s in r.ResourceSports
        //                                          where selectedSportId == Constant.SportId.All
        //                                          || s.SportID.StartsWith(selectedSportId)
        //                                          from b in s.BenchmarkResources
        //                                          where b.Active
        //                                          && (yearId == null || b.YearID == yearId)
        //                                          select b).Any(),
        //                     Category = (from s in r.ResourceSports
        //                                 where (selectedSportId == Constant.SportId.All ||
        //                                 s.SportID.StartsWith(selectedSportId))
        //                                 && s.ResourceClass != null
        //                                 && (yearId == null || s.YearID == yearId)
        //                                 orderby s.ResourceClass.SortOrder
        //                                 select _frenchLanguage ? s.ResourceClass.Name_FR : s.ResourceClass.Name_EN).FirstOrDefault(),
        //                     PersonalCoachId = (from s in this._repository.ResourceSports
        //                                        where s.ResourceID == r.ResourceID && s.SportID == Constant.SportId.All && s.CoachResourceID != null
        //                                        select s.CoachResourceID).FirstOrDefault(),
        //                     CoachLastName = (from rc in this._repository.Resources
        //                                      where rc.ResourceID == (from s in this._repository.ResourceSports
        //                                                              where s.ResourceID == r.ResourceID && s.SportID == Constant.SportId.All && s.CoachResourceID != null
        //                                                              select s.CoachResourceID).FirstOrDefault()
        //                                      select rc.LastName).FirstOrDefault(),
        //                     CoachFirstName = (from rc in this._repository.Resources
        //                                       where rc.ResourceID == (from s in this._repository.ResourceSports
        //                                                               where s.ResourceID == r.ResourceID && s.SportID == Constant.SportId.All && s.CoachResourceID != null
        //                                                               select s.CoachResourceID).FirstOrDefault()
        //                                       select rc.FirstName).FirstOrDefault()
        //                 }).ToList();


        //    // Now set the model with the query elements
        //    foreach (var item in query)
        //    {
        //        if (item.DateOfBirth != null)
        //        {
        //            item.BirthDate = string.Format("{0:yyyy/MM/dd}", item.DateOfBirth);
        //        }

        //        if (item.PersonalCoachId != null)
        //        {
        //            item.PersonalCoachName = string.Format("{0} {1} - {2}", item.CoachFirstName, item.CoachLastName, item.PersonalCoachId);
        //        }
        //    }

        //    IQueryable<MaintenanceAthletesAthleteGridViewModel> modelQuery = query.AsQueryable();

        //    // Apply filtering from the UI
        //    if (command.FilterDescriptors.Any())
        //    {
        //        //modelQuery = FilterAthletes(modelQuery, command.FilterDescriptors);
        //        modelQuery = FilterMembers<MaintenanceAthletesAthleteGridViewModel>(modelQuery, command.FilterDescriptors);
        //    }

        //    // Apply sorting from the UI
        //    if (command.SortDescriptors.Any())
        //    {
        //        modelQuery = SortMembers<MaintenanceAthletesAthleteGridViewModel>(modelQuery, command.SortDescriptors);
        //    }
        //    else
        //    {
        //        // Apply the default sorting
        //        modelQuery = modelQuery.OrderBy(x => x.LastName).ThenBy(x => x.FirstName);
        //    }

        //    return modelQuery.ToList();
        //}

        public List<MaintenanceAthletesSportsInfoViewModel> GetMaintenanceAthletesSportsInfoViewModel(int resourceId, string yearId)
        {
            List<MaintenanceAthletesSportsInfoViewModel> model = null;

            var resource = (from r in this._repository.Resources
                            where r.ResourceID == resourceId
                            && r.Active
                            select r).SingleOrDefault();

            if (resource != null)
            {
                //Constant.SportId.All is used as Athlete personal coach
                model = (from s in this._repository.ResourceSports
                         where s.ResourceID == resourceId && s.SportID != Constant.SportId.All
                         //&& s.YearID == yearId
                         select new MaintenanceAthletesSportsInfoViewModel
                         {
                             ResourceId = s.ResourceID,
                             ResourceSportId = s.ResourceSportID,
                             ResourceClassId = s.ResourceClassID,
                             ResourceClassName = (from rl in this._repository.ResourceClasses
                                                  where rl.ResourceClassID == s.ResourceClassID
                                                  select _frenchLanguage ? rl.Name_FR : rl.Name_EN).FirstOrDefault(),
                             SportsId = s.Sport.ParentSport.ParentSportID,
                             SportName = _frenchLanguage ? s.Sport.ParentSport.ParentSport.Name_FR : s.Sport.ParentSport.ParentSport.Name_EN,
                             DisciplineId = s.Sport.ParentSportID,
                             DisciplineName = _frenchLanguage ? s.Sport.ParentSport.Name_FR : s.Sport.ParentSport.Name_EN,
                             EventId = s.SportID,
                             EventName = _frenchLanguage ? s.Sport.Name_FR : s.Sport.Name_EN,
                             Benchmark = (from b in s.BenchmarkResources
                                          where b.Active
                                          && b.YearID == yearId
                                          select b).Any(),
                             YearId = s.YearID,
                             HasComments = !string.IsNullOrEmpty(s.Comments),
                             SportTrackingRef = s.SportTrackingRef,
                             EventCoachId = s.CoachResourceID,
                             CoachLastName = (from r in this._repository.Resources
                                              where r.ResourceID == s.CoachResourceID
                                              select r.LastName).FirstOrDefault(),
                             CoachFirstName = (from r in this._repository.Resources
                                               where r.ResourceID == s.CoachResourceID
                                               select r.FirstName).FirstOrDefault()
                         }).OrderByDescending(x => x.YearId).ToList();

                foreach (MaintenanceAthletesSportsInfoViewModel item in model)
                {
                    if (item.EventCoachId != null)
                    {
                        item.EventCoachName = string.Format("{0} {1} - {2}", item.CoachFirstName, item.CoachLastName, item.EventCoachId);
                    }
                }
            }

            return model;
        }

        public List<MaintenanceAthletesSportsInfoViewModel> GetMaintenanceAthletesSportsInfoViewModelParameter(int resourceId, string yearId, GridCommand command)
        {
            List<MaintenanceAthletesSportsInfoViewModel> modelQuery = GetMaintenanceAthletesSportsInfoViewModel(resourceId, yearId);

            if (modelQuery == null)
                return modelQuery;

            IQueryable<MaintenanceAthletesSportsInfoViewModel> query = modelQuery.AsQueryable();
            if (command.FilterDescriptors.Any())
            {
                //query = FilterAthletesSportsInfo(query, command.FilterDescriptors);
                query = FilterMembers<MaintenanceAthletesSportsInfoViewModel>(query, command.FilterDescriptors);
            }

            // Sort the list
            if (command.SortDescriptors.Any())
            {
                // Apply sorting from the UI
                query = SortMembers<MaintenanceAthletesSportsInfoViewModel>(query, command.SortDescriptors);
            }
            //else
            //{
            //    // Sort the list by the default sorting
            //    query = query.OrderByDescending(x => x.YearId);
            //}

            //// Apply sorting from the UI
            //foreach (SortDescriptor sortDescriptor in command.SortDescriptors)
            //{
            //    if (sortDescriptor.SortDirection == ListSortDirection.Ascending)
            //    {
            //        switch (sortDescriptor.Member)
            //        {
            //            case "SportName":
            //                query = query.OrderBy(x => x.SportName);
            //                break;
            //            case "DisciplineName":
            //                query = query.OrderBy(x => x.DisciplineName);
            //                break;
            //            case "EventName":
            //                query = query.OrderBy(x => x.EventName);
            //                break;
            //            case "YearId":
            //                query = query.OrderBy(x => x.YearId);
            //                break;
            //            case "ResourceClassId":
            //                query = query.OrderBy(x => x.ResourceClassId);
            //                break;
            //            case "EventCoachId":
            //                query = query.OrderBy(x => x.EventCoachId);
            //                break;
            //            case "SportTrackingRef":
            //                query = query.OrderBy(x => x.SportTrackingRef);
            //                break;
            //            case "Benchmark":
            //                query = query.OrderBy(x => x.Benchmark);
            //                break;
            //            case "HasComments":
            //                query = query.OrderBy(x => x.HasComments);
            //                break;
            //        }
            //    }
            //    else
            //    {
            //        switch (sortDescriptor.Member)
            //        {
            //            case "SportName":
            //                query = query.OrderByDescending(x => x.SportName);
            //                break;
            //            case "DisciplineName":
            //                query = query.OrderByDescending(x => x.DisciplineName);
            //                break;
            //            case "EventName":
            //                query = query.OrderByDescending(x => x.EventName);
            //                break;
            //            case "YearId":
            //                query = query.OrderByDescending(x => x.YearId);
            //                break;
            //            case "ResourceClassId":
            //                query = query.OrderByDescending(x => x.ResourceClassId);
            //                break;
            //            case "EventCoachId":
            //                query = query.OrderByDescending(x => x.EventCoachId);
            //                break;
            //            case "SportTrackingRef":
            //                query = query.OrderByDescending(x => x.SportTrackingRef);
            //                break;
            //            case "Benchmark":
            //                query = query.OrderByDescending(x => x.Benchmark);
            //                break;
            //            case "HasComments":
            //                query = query.OrderByDescending(x => x.HasComments);
            //                break;
            //        }
            //    }
            //}

            return query.ToList<MaintenanceAthletesSportsInfoViewModel>();
        }

        public List<MaintenanceAthletesDirectAthleteSupportViewModel> GetMaintenanceAthletesDirectAthleteSupportViewModel(int resourceId)
        {
            List<MaintenanceAthletesDirectAthleteSupportViewModel> model = null;

            var resource = (from r in this._repository.Resources
                            where r.ResourceID == resourceId
                            && r.Active
                            select r).SingleOrDefault();

            if (resource != null)
            {
                model = new List<MaintenanceAthletesDirectAthleteSupportViewModel>();

                foreach (var support in resource.ResourceDirectAthleteSupports)
                {
                    var resourceSupport = new MaintenanceAthletesDirectAthleteSupportViewModel();

                    resourceSupport.Amount = support.Amount;
                    resourceSupport.Amount = resourceSupport.Amount ?? 0m;
                    resourceSupport.HasComments = !string.IsNullOrEmpty(support.Comments);
                    resourceSupport.Description = string.IsNullOrEmpty(support.Description) ? string.Empty : support.Description;
                    resourceSupport.DirectAthleteSupportCode = support.DirectAthleteSupportID;
                    resourceSupport.DirectAthleteSupportName = _frenchLanguage ? support.DirectAthleteSupport.Name_FR : support.DirectAthleteSupport.Name_EN;
                    resourceSupport.DirectAthleteSupportTypeId = support.DirectAthleteSupport.DirectAthleteSupportTypeID;
                    resourceSupport.DirectAthleteSupportTypeName = _frenchLanguage ? support.DirectAthleteSupport.DirectAthleteSupportType.Name_FR : support.DirectAthleteSupport.DirectAthleteSupportType.Name_EN;
                    resourceSupport.ResourceDirectAthleteSupportId = support.ResourceDirectAthleteSupportID;
                    resourceSupport.ResourceId = support.ResourceID;
                    resourceSupport.SupportContact = string.IsNullOrEmpty(support.SupportContact) ? string.Empty : support.SupportContact;
                    resourceSupport.SupportEmail = string.IsNullOrEmpty(support.SupportEmail) ? string.Empty : support.SupportEmail;
                    resourceSupport.SupportName = support.SupportName;
                    resourceSupport.YearId = support.YearID;

                    model.Add(resourceSupport);
                }
            }

            return model;
        }

        public List<MaintenanceAthletesDirectAthleteSupportViewModel> GetMaintenanceAthletesDirectAthleteSupportViewModelParameter(int resourceId, GridCommand command)
        {
            List<MaintenanceAthletesDirectAthleteSupportViewModel> model = GetMaintenanceAthletesDirectAthleteSupportViewModel(resourceId);

            IQueryable<MaintenanceAthletesDirectAthleteSupportViewModel> modelQuery = model.AsQueryable();

            // Apply filtering from the UI
            if (command.FilterDescriptors.Any())
            {
                modelQuery = FilterMembers<MaintenanceAthletesDirectAthleteSupportViewModel>(modelQuery, command.FilterDescriptors);
            }

            // Default sorting
            if (command.SortDescriptors.Any())
            {
                modelQuery = SortMembers<MaintenanceAthletesDirectAthleteSupportViewModel>(modelQuery, command.SortDescriptors);
            }
            else
            {
                // Apply the default sorting
                modelQuery = modelQuery.OrderBy(x => x.DirectAthleteSupportTypeName).ThenBy(x => x.YearId);
            }

            return modelQuery.ToList();
        }

        public List<MaintenanceAthletesAffiliatesInfoViewModel> GetMaintenanceAthletesAffiliatesInfoViewModel(int resourceId)
        {
            List<MaintenanceAthletesAffiliatesInfoViewModel> model = null;

            var resource = (from r in this._repository.Resources
                            where r.ResourceID == resourceId
                            && r.Active
                            select r).SingleOrDefault();

            if (resource != null)
            {
                model = new List<MaintenanceAthletesAffiliatesInfoViewModel>();

                foreach (var affiliate in resource.ResourceAffiliates)
                {
                    var resourceAffiliate = new MaintenanceAthletesAffiliatesInfoViewModel();

                    resourceAffiliate.AffiliateContact = string.IsNullOrEmpty(affiliate.AffiliateContact) ? string.Empty : affiliate.AffiliateContact;
                    resourceAffiliate.AffiliateEmail = string.IsNullOrEmpty(affiliate.AffiliateEmail) ? string.Empty : affiliate.AffiliateEmail;
                    resourceAffiliate.AffiliateName = affiliate.AffiliateName;
                    resourceAffiliate.AffiliateTypeId = affiliate.AffiliateTypeID;
                    resourceAffiliate.AffiliateTypeName = _frenchLanguage ? affiliate.AffiliateType.Name_FR : affiliate.AffiliateType.Name_EN;
                    resourceAffiliate.HasComments = !string.IsNullOrEmpty(affiliate.Comments);
                    resourceAffiliate.Description = string.IsNullOrEmpty(affiliate.Description) ? string.Empty : affiliate.Description;
                    resourceAffiliate.ResourceId = affiliate.ResourceID;
                    resourceAffiliate.YearFromId = affiliate.YearFromID;
                    resourceAffiliate.YearToId = affiliate.YearToID;
                    resourceAffiliate.ResourceAffiliateId = affiliate.ResourceAffiliateID;

                    model.Add(resourceAffiliate);
                }
            }

            return model;
        }

        public List<MaintenanceAthletesAffiliatesInfoViewModel> GetMaintenanceAthletesAffiliatesInfoViewModelParameter(int resourceId, GridCommand command)
        {
            List<MaintenanceAthletesAffiliatesInfoViewModel> model = GetMaintenanceAthletesAffiliatesInfoViewModel(resourceId);

            IQueryable<MaintenanceAthletesAffiliatesInfoViewModel> modelQuery = model.AsQueryable();

            // Apply filtering from the UI
            if (command.FilterDescriptors.Any())
            {
                modelQuery = FilterMembers<MaintenanceAthletesAffiliatesInfoViewModel>(modelQuery, command.FilterDescriptors);
            }

            // Default sorting
            if (command.SortDescriptors.Any())
            {
                modelQuery = SortMembers<MaintenanceAthletesAffiliatesInfoViewModel>(modelQuery, command.SortDescriptors);
            }
            else
            {
                // Apply the default sorting
                modelQuery = modelQuery.OrderBy(x => x.AffiliateTypeName).ThenBy(x => x.AffiliateName);
            }

            return modelQuery.ToList();
        }

        //public List<MaintenanceAthletesResultsViewModel> GetMaintenanceAthletesResultsViewModel(int resourceId)
        //{
        //    ObjectParameter parameter = new ObjectParameter("TotalCount", typeof(int));

        //    var query = this._repository.uspGetMaintenanceAthletesResultsViewModel(
        //                        _frenchLanguage ? Constant.LanguageCode.French : Constant.LanguageCode.English,
        //                        resourceId,
        //                        true,
        //                        10,
        //                        1,
        //                        0,
        //                        "",
        //                        "ASC",
        //                        0,
        //                        "",
        //                        parameter).ToList();

        //    List<MaintenanceAthletesResultsViewModel> model = new List<MaintenanceAthletesResultsViewModel>();

        //    foreach (var item in query)
        //    {
        //        MaintenanceAthletesResultsViewModel modelItem = new MaintenanceAthletesResultsViewModel();

        //        modelItem.ResourceID = item.ResourceID;
        //        modelItem.ResultID = item.ResultID;
        //        modelItem.CompetitionID = item.CompetitionID;
        //        modelItem.CompetitionName = item.CompetitionName;
        //        modelItem.CompetitionDate = item.CompetitionDate;
        //        modelItem.Location = item.Location;
        //        modelItem.CountryID = item.CountryID;
        //        modelItem.CountryName = item.CountryName;
        //        modelItem.SportID = item.SportID;
        //        modelItem.SportName = item.SportName;
        //        modelItem.DisciplineID = item.DisciplineID;
        //        modelItem.DisciplineName = item.DisciplineName;
        //        modelItem.EventDate = item.EventDate;
        //        modelItem.EventID = item.EventID;
        //        modelItem.EventName = item.EventName;
        //        modelItem.EventDate = item.EventDate;
        //        modelItem.Comments = item.Comments;
        //        modelItem.Position = item.Position;
        //        modelItem.PerfTime = item.PerfTime;
        //        modelItem.PerfValue = item.PerfValue;

        //        model.Add(modelItem);
        //    }

        //    return model;
        //}

        public List<MaintenanceAthletesResultsViewModel> GetMaintenanceAthletesResultsViewModel(int resourceId)
        {
            var resource = (from r in this._repository.Resources
                            where r.ResourceID == resourceId
                            && r.Active
                            select r).SingleOrDefault();

            if (resource == null)
                return null;

            var resultsQuery = (from r in this._repository.Results
                                //join c in this._repository.Competitions
                                //on r.CompetitionID equals c.CompetitionID
                                join rr in this._repository.ResultResources
                                on r.ResultID equals rr.ResultID
                                where rr.ResourceID == (int?)resourceId
                                select new
                                {
                                    ResourceID = resourceId,
                                    ResultID = r.ResultID,
                                    Position = r.Position,
                                    EventDate = r.EventDate,
                                    PerfValue = r.PerfValue,
                                    PerfTime = r.PerfTime,
                                    Comments = r.Comments,
                                    EventID = r.SportID,
                                    EventName = (from s in this._repository.Sports
                                                 where s.SportID == r.SportID
                                                 select _frenchLanguage ? s.Name_FR : s.Name_EN).FirstOrDefault(),
                                    DisciplineID = r.Sport.ParentSportID,
                                    DisciplineName = (from s in this._repository.Sports
                                                      where s.SportID == r.Sport.ParentSportID
                                                      select _frenchLanguage ? s.Name_FR : s.Name_EN).FirstOrDefault(),
                                    SportID = r.Sport.ParentSport.ParentSportID,
                                    SportName = (from s in this._repository.Sports
                                                 where s.SportID == r.Sport.ParentSport.ParentSportID
                                                 select _frenchLanguage ? s.Name_FR : s.Name_EN).FirstOrDefault(),
                                    Gender = (from s in this._repository.Sports
                                              where s.SportID == r.Sport.ParentSportID
                                              select s.Gender).FirstOrDefault(),
                                    CompetitionID = r.CompetitionID,
                                    CompetitionName = (from ct in _repository.CompetitionTypes
                                                       join c in _repository.Competitions
                                                       on ct.CompetitionTypeID equals c.CompetitionTypeID
                                                       where c.CompetitionID == r.CompetitionID
                                                       select _frenchLanguage ? ct.Name_FR : ct.Name_EN).FirstOrDefault(),
                                    CompetitionDate = (from c in _repository.Competitions
                                                       where c.CompetitionID == r.CompetitionID
                                                       select c.CompetitionDate).FirstOrDefault(),
                                    Location = (from c in _repository.Competitions
                                                where c.CompetitionID == r.CompetitionID
                                                select c.Location).FirstOrDefault(),
                                    CountryID = (from c in _repository.Countries
                                                 join ct in _repository.Competitions
                                                 on c.CountryID equals ct.CountryID
                                                 where ct.CompetitionID == r.CompetitionID
                                                 select c.CountryID).FirstOrDefault(),
                                    CountryName = (from c in _repository.Countries
                                                   join ct in _repository.Competitions
                                                   on c.CountryID equals ct.CountryID
                                                   where ct.CompetitionID == r.CompetitionID
                                                   select _frenchLanguage ? c.Name_FR : c.Name_EN).FirstOrDefault()
                                });

            // Sort by Date is the default
            resultsQuery = resultsQuery.OrderByDescending(x => x.CompetitionDate).ThenByDescending(y => y.EventDate).ThenByDescending(z => z.Position);

            List<MaintenanceAthletesResultsViewModel> modelQuery = new List<MaintenanceAthletesResultsViewModel>();

            foreach (var result in resultsQuery)
            {
                var resourceResult = new MaintenanceAthletesResultsViewModel();

                resourceResult.ResourceID = resourceId;
                resourceResult.ResultID = result.ResultID;
                resourceResult.CompetitionID = result.CompetitionID;
                resourceResult.CompetitionName = result.CompetitionName;
                if (result.CompetitionDate != new DateTime(0001, 1, 1))
                {
                    resourceResult.CompetitionDate = string.Format("{0:yyyy/MM/dd}", result.CompetitionDate);
                }
                resourceResult.CountryID = result.CountryID;
                resourceResult.CountryName = result.CountryName;
                resourceResult.Location = result.Location;
                resourceResult.SportID = result.SportID;
                resourceResult.SportName = result.SportName;
                resourceResult.DisciplineID = result.DisciplineID;
                resourceResult.DisciplineName = this._decodeBL.GetDiscipline(result.DisciplineName);
                resourceResult.EventID = result.EventID;
                resourceResult.EventName = result.EventName;
                resourceResult.Gender = result.Gender;
                resourceResult.EventDate = string.Format("{0:yyyy/MM/dd}", result.EventDate);
                resourceResult.Position = result.Position;
                resourceResult.PerfValue = (result.PerfValue == null) ? string.Empty : String.Format("{0:0.00}", result.PerfValue);
                resourceResult.PerfTime = (result.PerfTime == null) ? string.Empty : ((TimeSpan)result.PerfTime).ConvertTimeSpanToString();
                resourceResult.Comments = result.Comments;

                modelQuery.Add(resourceResult);
            }

            return modelQuery;
        }

        public List<MaintenanceAthletesResultsViewModel> GetMaintenanceAthletesResultsViewModelParameter(int resourceId, GridCommand command)
        {
            SortDescriptor sort = new SortDescriptor()
            {
                Member = "",
                SortDirection = ListSortDirection.Ascending
            };

            if (command.SortDescriptors.Any())
            {
                sort = command.SortDescriptors[0];
            }

            string filterString = string.Empty;
            filterString = GetFilterString(filterString, command.FilterDescriptors);

            ObjectParameter parameter = new ObjectParameter("TotalCount", typeof(int));

            var query = this._repository.uspGetMaintenanceAthletesResultsViewModel(
                                _frenchLanguage ? Constant.LanguageCode.French : Constant.LanguageCode.English,
                                resourceId,
                                true,
                                command.PageSize,
                                command.Page,
                                command.SortDescriptors.Count,
                                sort.Member,
                                sort.SortDirection == ListSortDirection.Ascending ? "ASC" : "DESC",
                                command.FilterDescriptors.Count,
                                filterString,
                                parameter).ToList();

            List<MaintenanceAthletesResultsViewModel> model = new List<MaintenanceAthletesResultsViewModel>();

            foreach (var item in query)
            {
                MaintenanceAthletesResultsViewModel modelItem = new MaintenanceAthletesResultsViewModel();

                modelItem.ResourceID = item.ResourceID;
                modelItem.ResultID = item.ResultID;
                modelItem.CompetitionID = item.CompetitionID;
                modelItem.CompetitionName = item.CompetitionName;
                modelItem.CompetitionDate = item.CompetitionDate;
                modelItem.Location = item.Location;
                modelItem.CountryID = item.CountryID;
                modelItem.CountryName = item.CountryName;
                modelItem.SportID = item.SportID;
                modelItem.SportName = item.SportName;
                modelItem.DisciplineID = item.DisciplineID;
                modelItem.DisciplineName = item.DisciplineName;
                modelItem.EventDate = item.EventDate;
                modelItem.EventID = item.EventID;
                modelItem.EventName = item.EventName;
                modelItem.EventDate = item.EventDate;
                modelItem.Comments = item.Comments;
                modelItem.Position = item.Position;
                modelItem.PerfTime = item.PerfTime;
                modelItem.PerfValue = item.PerfValue;

                model.Add(modelItem);
            }

            return model;
        }

        //public List<MaintenanceAthletesResultsViewModel> GetMaintenanceAthletesResultsViewModelParameter(int resourceId, GridCommand command)
        //{
        //    List<MaintenanceAthletesResultsViewModel> modelQuery = GetMaintenanceAthletesResultsViewModel(resourceId);

        //    if (modelQuery == null)
        //        return modelQuery;

        //    IQueryable<MaintenanceAthletesResultsViewModel> query = modelQuery.AsQueryable();

        //    if (command.FilterDescriptors.Any())
        //    {
        //        //query = FilterAthletesResults(query, command.FilterDescriptors);
        //        query = FilterMembers<MaintenanceAthletesResultsViewModel>(query, command.FilterDescriptors);
        //    }

        //    // Sort the list
        //    if (command.SortDescriptors.Any())
        //    {
        //        // Apply sorting from the UI
        //        query = SortMembers<MaintenanceAthletesResultsViewModel>(query, command.SortDescriptors);
        //    }
        //    //else
        //    //{
        //    //    // Sort the list by the default sorting
        //    //    query = query.OrderByDescending(x => x.CompetitionDate).ThenByDescending(y => y.EventDate).ThenByDescending(z => z.Position);
        //    //}

        //    //// Sort the list by the default sorting
        //    //if (!command.SortDescriptors.Any())
        //    //{
        //    //    query = query.OrderByDescending(x => x.CompetitionDate).ThenByDescending(y => y.EventDate).ThenByDescending(z => z.Position);
        //    //}
        //    //else
        //    //{
        //    //    // Apply sorting from the UI
        //    //    foreach (SortDescriptor sortDescriptor in command.SortDescriptors)
        //    //    {
        //    //        if (sortDescriptor.SortDirection == ListSortDirection.Ascending)
        //    //        {
        //    //            switch (sortDescriptor.Member)
        //    //            {
        //    //                case "CompetitionName":
        //    //                    query = query.OrderBy(x => x.CompetitionName);
        //    //                    break;
        //    //                case "CompetitionDate":
        //    //                    query = query.OrderBy(x => x.CompetitionDate);
        //    //                    break;
        //    //                case "Location":
        //    //                    query = query.OrderBy(x => x.Location);
        //    //                    break;
        //    //                case "CountryID":
        //    //                    query = query.OrderBy(x => x.CountryID);
        //    //                    break;
        //    //                case "CountryName":
        //    //                    query = query.OrderBy(x => x.CountryName);
        //    //                    break;
        //    //                case "SportID":
        //    //                    query = query.OrderBy(x => x.SportID);
        //    //                    break;
        //    //                case "SportName":
        //    //                    query = query.OrderBy(x => x.SportName);
        //    //                    break;
        //    //                case "DisciplineID":
        //    //                    query = query.OrderBy(x => x.DisciplineID);
        //    //                    break;
        //    //                case "DisciplineName":
        //    //                    query = query.OrderBy(x => x.DisciplineName);
        //    //                    break;
        //    //                case "EventID":
        //    //                    query = query.OrderBy(x => x.EventID);
        //    //                    break;
        //    //                case "EventName":
        //    //                    query = query.OrderBy(x => x.EventName);
        //    //                    break;
        //    //                case "Gender":
        //    //                    query = query.OrderBy(x => x.Gender);
        //    //                    break;
        //    //                case "Position":
        //    //                    query = query.OrderBy(x => x.Position);
        //    //                    break;
        //    //                case "EventDate":
        //    //                    query = query.OrderBy(x => x.EventDate);
        //    //                    break;
        //    //                case "PerfTime":
        //    //                    query = query.OrderBy(x => x.PerfTime);
        //    //                    break;
        //    //                case "PerfValue":
        //    //                    query = query.OrderBy(x => x.PerfValue);
        //    //                    break;
        //    //            }
        //    //        }
        //    //        else
        //    //        {
        //    //            switch (sortDescriptor.Member)
        //    //            {
        //    //                case "CompetitionName":
        //    //                    query = query.OrderByDescending(x => x.CompetitionName);
        //    //                    break;
        //    //                case "CompetitionDate":
        //    //                    query = query.OrderByDescending(x => x.CompetitionDate);
        //    //                    break;
        //    //                case "Location":
        //    //                    query = query.OrderByDescending(x => x.Location);
        //    //                    break;
        //    //                case "CountryID":
        //    //                    query = query.OrderByDescending(x => x.CountryID);
        //    //                    break;
        //    //                case "CountryName":
        //    //                    query = query.OrderByDescending(x => x.CountryName);
        //    //                    break;
        //    //                case "SportID":
        //    //                    query = query.OrderByDescending(x => x.SportID);
        //    //                    break;
        //    //                case "SportName":
        //    //                    query = query.OrderByDescending(x => x.SportName);
        //    //                    break;
        //    //                case "DisciplineID":
        //    //                    query = query.OrderByDescending(x => x.DisciplineID);
        //    //                    break;
        //    //                case "DisciplineName":
        //    //                    query = query.OrderByDescending(x => x.DisciplineName);
        //    //                    break;
        //    //                case "EventID":
        //    //                    query = query.OrderByDescending(x => x.EventID);
        //    //                    break;
        //    //                case "EventName":
        //    //                    query = query.OrderByDescending(x => x.EventName);
        //    //                    break;
        //    //                case "Gender":
        //    //                    query = query.OrderByDescending(x => x.Gender);
        //    //                    break;
        //    //                case "Position":
        //    //                    query = query.OrderByDescending(x => x.Position);
        //    //                    break;
        //    //                case "EventDate":
        //    //                    query = query.OrderByDescending(x => x.EventDate);
        //    //                    break;
        //    //                case "PerfTime":
        //    //                    query = query.OrderByDescending(x => x.PerfTime);
        //    //                    break;
        //    //                case "PerfValue":
        //    //                    query = query.OrderByDescending(x => x.PerfValue);
        //    //                    break;
        //    //            }
        //    //        }
        //    //    }
        //    //}

        //    return query.ToList();
        //}

        #endregion Get Maintenance Athletes

        #region Get Maintenance IST

        public List<MaintenanceISTMembersGridViewModel> GetMaintenanceISTMembersGridViewModel(string selectedSportId, GridCommand command, out int total)
        {
            total = 0;

            if (string.IsNullOrEmpty(selectedSportId))
            {
                selectedSportId = Constant.SportId.All;
            }

            SortDescriptor sort = new SortDescriptor()
            {
                Member = "",
                SortDirection = ListSortDirection.Ascending
            };

            if (command.SortDescriptors.Any())
            {
                sort = command.SortDescriptors[0];
            }

            string filterString = string.Empty;
            filterString = GetFilterString(filterString, command.FilterDescriptors);

            ObjectParameter parameter = new ObjectParameter("TotalCount", typeof(int));

            if (command.PageSize == 0)
            {
                command.PageSize = 10;
                command.Page = 1;
            }

            var query = this._repository.uspGetMaintenanceISTMembersGridViewModel(
                                _frenchLanguage ? Constant.LanguageCode.French : Constant.LanguageCode.English,
                                Constant.ResourceType.ISTMembers,
                                selectedSportId,
                                false,
                                command.PageSize,
                                command.Page,
                                command.SortDescriptors.Count,
                                sort.Member,
                                sort.SortDirection == ListSortDirection.Ascending ? "ASC" : "DESC",
                                command.FilterDescriptors.Count,
                                filterString,
                                parameter).ToList();

            List<MaintenanceISTMembersGridViewModel> model = new List<MaintenanceISTMembersGridViewModel>();

            foreach (var item in query)
            {
                MaintenanceISTMembersGridViewModel modelItem = new MaintenanceISTMembersGridViewModel();

                modelItem.LastName = item.LastName;
                modelItem.FirstName = item.FirstName;
                modelItem.CountryId = item.CountryId;
                modelItem.CountryName = item.CountryName;
                modelItem.BirthDate = item.BirthDate;
                modelItem.GenderId = item.GenderId;
                modelItem.GenderName = item.GenderName;
                modelItem.ResourceId = item.ResourceId;
                modelItem.Pooled = item.Pooled;
                modelItem.TBD = item.TBD;
                modelItem.PrimaryCscId = item.PrimaryCscId;
                modelItem.PrimaryCscName = item.PrimaryCscName;
                modelItem.ResourceSpecialtyId = item.ResourceSpecialtyId;
                modelItem.ResourceSpecialtyName = item.ResourceSpecialtyName;

                model.Add(modelItem);
            }

            total = (int)parameter.Value;

            return model;
        }

        //public List<MaintenanceISTMembersGridViewModel> GetMaintenanceISTMembersGridViewModel(string selectedSportId, GridCommand command, out int total)
        //{
        //    total = 0;

        //    if (string.IsNullOrEmpty(selectedSportId))
        //    {
        //        selectedSportId = Constant.SportId.All;
        //    }

        //    List<MaintenanceISTMembersGridViewModel> query = (from r in this._repository.Resources
        //                                                      where r.ResourceTypeID == Constant.ResourceType.ISTMembers
        //                                                      && (selectedSportId == Constant.SportId.All ||
        //                                                      (r.ResourceSports.Any(s => s.Sport.SportLevelID == Constant.SportLevelId.Sport && s.SportID == selectedSportId)
        //                                                         || r.ResourceSports.Any(s => s.Sport.SportLevelID == Constant.SportLevelId.Discipline && s.Sport.ParentSport.SportID == selectedSportId)
        //                                                         || r.ResourceSports.Any(s => s.Sport.SportLevelID == Constant.SportLevelId.Event && s.Sport.ParentSport.ParentSport.SportID == selectedSportId)
        //                                                      )) // Only IST Members that have sports under the selected sport id
        //                                                      && r.Active
        //                                                      select new MaintenanceISTMembersGridViewModel
        //                                                      {
        //                                                          GenderId = r.Gender,
        //                                                          GenderName = (from g in this._repository.Decodes
        //                                                                        where g.DecodeCategoryID == Constant.DecodeCategoryId.Gender
        //                                                                        && g.DecodeID == r.Gender
        //                                                                        select _frenchLanguage ? g.DecodeID_FR : g.DecodeID_EN).FirstOrDefault(),
        //                                                          FirstName = r.FirstName,
        //                                                          LastName = r.LastName,
        //                                                          DateOfBirth = r.DateOfBirth,
        //                                                          ResourceId = r.ResourceID,
        //                                                          ResourceSpecialtyId = r.ResourceSpecialty.ResourceSpecialtyID,
        //                                                          CountryId = r.CountryID,
        //                                                          CountryName = _frenchLanguage ? r.Country.Name_FR : r.Country.Name_EN,
        //                                                          Pooled = r.Pooled,
        //                                                          TBD = r.TBD,
        //                                                          ResourceSpecialtyName = _frenchLanguage ? r.ResourceSpecialty.Name_FR : r.ResourceSpecialty.Name_EN,
        //                                                          PrimaryCscId = r.PrimaryCSCID,
        //                                                          PrimaryCscName = _frenchLanguage ? r.Organization.Name_FR : r.Organization.Name_EN
        //                                                      }).ToList();

        //    IQueryable<MaintenanceISTMembersGridViewModel> modelQuery = query.AsQueryable();

        //    //Apply filtering from the UI
        //    if (command.FilterDescriptors.Any())
        //    {
        //        //modelQuery = FilterISTMembers(modelQuery, command.FilterDescriptors);
        //        modelQuery = FilterMembers<MaintenanceISTMembersGridViewModel>(modelQuery, command.FilterDescriptors);
        //    }

        //    //Default sorting
        //    if (command.SortDescriptors.Count == 0)
        //    {
        //        modelQuery = modelQuery.OrderBy(x => x.LastName).ThenBy(x => x.FirstName);
        //    }

        //    // Apply sorting from the UI
        //    foreach (SortDescriptor sortDescriptor in command.SortDescriptors)
        //    {
        //        if (sortDescriptor.SortDirection == ListSortDirection.Ascending)
        //        {
        //            switch (sortDescriptor.Member)
        //            {
        //                case "ResourceId":
        //                    modelQuery = modelQuery.OrderBy(x => x.ResourceId);
        //                    break;
        //                case "LastName":
        //                    modelQuery = modelQuery.OrderBy(x => x.LastName);
        //                    break;
        //                case "FirstName":
        //                    modelQuery = modelQuery.OrderBy(x => x.FirstName);
        //                    break;
        //                case "CountryName":
        //                    modelQuery = modelQuery.OrderBy(x => x.CountryName);
        //                    break;
        //                case "BirthDate":
        //                    modelQuery = modelQuery.OrderBy(x => x.DateOfBirth);
        //                    break;
        //                case "Pooled":
        //                    modelQuery = modelQuery.OrderBy(x => x.Pooled);
        //                    break;
        //                case "TBD":
        //                    modelQuery = modelQuery.OrderBy(x => x.TBD);
        //                    break;
        //                case "PrimaryCscName":
        //                    modelQuery = modelQuery.OrderBy(x => x.PrimaryCscName);
        //                    break;
        //                case "ResourceSpecialtyName":
        //                    modelQuery = modelQuery.OrderBy(x => x.ResourceSpecialtyName);
        //                    break;
        //            }
        //        }
        //        else
        //        {
        //            switch (sortDescriptor.Member)
        //            {
        //                case "ResourceId":
        //                    modelQuery = modelQuery.OrderByDescending(x => x.ResourceId);
        //                    break;
        //                case "LastName":
        //                    modelQuery = modelQuery.OrderByDescending(x => x.LastName);
        //                    break;
        //                case "FirstName":
        //                    modelQuery = modelQuery.OrderByDescending(x => x.FirstName);
        //                    break;
        //                case "CountryName":
        //                    modelQuery = modelQuery.OrderByDescending(x => x.CountryName);
        //                    break;
        //                case "BirthDate":
        //                    modelQuery = modelQuery.OrderByDescending(x => x.DateOfBirth);
        //                    break;
        //                case "Pooled":
        //                    modelQuery = modelQuery.OrderByDescending(x => x.Pooled);
        //                    break;
        //                case "TBD":
        //                    modelQuery = modelQuery.OrderByDescending(x => x.TBD);
        //                    break;
        //                case "PrimaryCscName":
        //                    modelQuery = modelQuery.OrderByDescending(x => x.PrimaryCscName);
        //                    break;
        //                case "ResourceSpecialtyName":
        //                    modelQuery = modelQuery.OrderByDescending(x => x.ResourceSpecialtyName);
        //                    break;
        //            }
        //        }
        //    }

        //    //Get the count for UI paging
        //    total = modelQuery.Count();

        //    //Set the paging size
        //    if (command.PageSize > 0)
        //    {
        //        //Set start index to take records from
        //        modelQuery = modelQuery.Skip((command.Page - 1) * command.PageSize);
        //    }

        //    //Return only the records needed to fit on the current page
        //    modelQuery = modelQuery.Take(command.PageSize);

        //    //Now set the model with the query elements
        //    var model = modelQuery.ToList();

        //    foreach (var item in model)
        //    {
        //        if (item.DateOfBirth != null)
        //        {
        //            item.BirthDate = string.Format("{0:yyyy-MM-dd}", item.DateOfBirth);
        //        }
        //    }

        //    return model;
        //}

        public List<MaintenanceISTMembersGridViewModel> GetMaintenanceISTMembersGridViewModelParameter(string selectedSportId, GridCommand command)
        {
            if (string.IsNullOrEmpty(selectedSportId))
            {
                selectedSportId = Constant.SportId.All;
            }

            SortDescriptor sort = new SortDescriptor()
            {
                Member = "",
                SortDirection = ListSortDirection.Ascending
            };

            if (command.SortDescriptors.Any())
            {
                sort = command.SortDescriptors[0];
            }

            string filterString = string.Empty;
            filterString = GetFilterString(filterString, command.FilterDescriptors);

            ObjectParameter parameter = new ObjectParameter("TotalCount", typeof(int));

            var query = this._repository.uspGetMaintenanceISTMembersGridViewModel(
                                _frenchLanguage ? Constant.LanguageCode.French : Constant.LanguageCode.English,
                                Constant.ResourceType.ISTMembers,
                                selectedSportId,
                                true,
                                command.PageSize,
                                command.Page,
                                command.SortDescriptors.Count,
                                sort.Member,
                                sort.SortDirection == ListSortDirection.Ascending ? "ASC" : "DESC",
                                command.FilterDescriptors.Count,
                                filterString,
                                parameter).ToList();

            List<MaintenanceISTMembersGridViewModel> model = new List<MaintenanceISTMembersGridViewModel>();

            foreach (var item in query)
            {
                MaintenanceISTMembersGridViewModel modelItem = new MaintenanceISTMembersGridViewModel();

                modelItem.LastName = item.LastName;
                modelItem.FirstName = item.FirstName;
                modelItem.CountryId = item.CountryId;
                modelItem.CountryName = item.CountryName;
                modelItem.BirthDate = item.BirthDate;
                modelItem.GenderId = item.GenderId;
                modelItem.GenderName = item.GenderName;
                modelItem.ResourceId = item.ResourceId;
                modelItem.Pooled = item.Pooled;
                modelItem.TBD = item.TBD;
                modelItem.PrimaryCscId = item.PrimaryCscId;
                modelItem.PrimaryCscName = item.PrimaryCscName;
                modelItem.ResourceSpecialtyId = item.ResourceSpecialtyId;
                modelItem.ResourceSpecialtyName = item.ResourceSpecialtyName;

                model.Add(modelItem);
            }

            return model;
        }

        //public List<MaintenanceISTMembersGridViewModel> GetMaintenanceISTMembersGridViewModelParameter(string selectedSportId, GridCommand command)
        //{
        //    if (string.IsNullOrEmpty(selectedSportId))
        //    {
        //        selectedSportId = Constant.SportId.All;
        //    }

        //    List<MaintenanceISTMembersGridViewModel> query = (from r in this._repository.Resources
        //                                                      where r.ResourceTypeID == Constant.ResourceType.ISTMembers
        //                                                      && (selectedSportId == Constant.SportId.All ||
        //                                                      (r.ResourceSports.Any(s => s.Sport.SportLevelID == Constant.SportLevelId.Sport && s.SportID == selectedSportId)
        //                                                         || r.ResourceSports.Any(s => s.Sport.SportLevelID == Constant.SportLevelId.Discipline && s.Sport.ParentSport.SportID == selectedSportId)
        //                                                         || r.ResourceSports.Any(s => s.Sport.SportLevelID == Constant.SportLevelId.Event && s.Sport.ParentSport.ParentSport.SportID == selectedSportId)
        //                                                      )) // Only IST Members that have sports under the selected sport id
        //                                                      && r.Active
        //                                                      select new MaintenanceISTMembersGridViewModel
        //                                                      {
        //                                                          GenderId = r.Gender,
        //                                                          GenderName = (from g in this._repository.Decodes
        //                                                                        where g.DecodeCategoryID == Constant.DecodeCategoryId.Gender
        //                                                                        && g.DecodeID == r.Gender
        //                                                                        select _frenchLanguage ? g.DecodeID_FR : g.DecodeID_EN).FirstOrDefault(),
        //                                                          FirstName = r.FirstName,
        //                                                          LastName = r.LastName,
        //                                                          DateOfBirth = r.DateOfBirth,
        //                                                          ResourceId = r.ResourceID,
        //                                                          ResourceSpecialtyId = r.ResourceSpecialty.ResourceSpecialtyID,
        //                                                          CountryId = r.CountryID,
        //                                                          CountryName = _frenchLanguage ? r.Country.Name_FR : r.Country.Name_EN,
        //                                                          Pooled = r.Pooled,
        //                                                          TBD = r.TBD,
        //                                                          ResourceSpecialtyName = _frenchLanguage ? r.ResourceSpecialty.Name_FR : r.ResourceSpecialty.Name_EN,
        //                                                          PrimaryCscId = r.PrimaryCSCID,
        //                                                          PrimaryCscName = _frenchLanguage ? r.Organization.Name_FR : r.Organization.Name_EN
        //                                                      }).ToList();

        //    foreach (var item in query)
        //    {
        //        if (item.DateOfBirth != null)
        //        {
        //            item.BirthDate = string.Format("{0:yyyy-MM-dd}", item.DateOfBirth);
        //        }
        //    }

        //    IQueryable<MaintenanceISTMembersGridViewModel> modelQuery = query.AsQueryable();

        //    //Apply filtering from the UI
        //    if (command.FilterDescriptors.Any())
        //    {
        //        modelQuery = FilterMembers<MaintenanceISTMembersGridViewModel>(modelQuery, command.FilterDescriptors);
        //    }

        //    //Default sorting
        //    if (command.SortDescriptors.Any())
        //    {
        //        modelQuery = SortMembers<MaintenanceISTMembersGridViewModel>(modelQuery, command.SortDescriptors);
        //    }
        //    else
        //    {
        //        // Apply the default sorting
        //        modelQuery = modelQuery.OrderBy(x => x.LastName).ThenBy(x => x.FirstName);
        //    }

        //    return modelQuery.ToList();
        //}

        public MaintenanceISTMembersGeneralInfoViewModel GetMaintenanceISTMembersGeneralInfoViewModel(int resourceId)
        {
            MaintenanceISTMembersGeneralInfoViewModel model = null;

            var resource = (from r in this._repository.Resources
                            where r.ResourceID == resourceId
                            select r).SingleOrDefault();

            if (resource != null)
            {
                model = new MaintenanceISTMembersGeneralInfoViewModel(resource);

                model.Countries = (from c in this._repository.Countries
                                   select new ListItem
                                   {
                                       Id = c.CountryID,
                                       Description = _frenchLanguage ? c.Name_FR : c.Name_EN
                                   }).ToList();

                model.CSCs = new List<ListItem> { null };
                model.CSCs.AddRange((from o in this._repository.Organizations
                                     where o.OrganizationTypeID == Constant.OrganizationTypeId.CSC
                                     select new ListItem
                                     {
                                         Id = o.OrganizationID,
                                         Description = _frenchLanguage ? o.Name_FR : o.Name_EN
                                     }).ToList());

                model.Genders = this._decodeBL.GetHumanGenders(true);

                model.Specialties = new List<ListItem> { null };
                model.Specialties.AddRange((from s in this._repository.ResourceSpecialties
                                            where s.ResourceTypeID == Constant.ResourceType.ISTMembers
                                            select new ListItem
                                            {
                                                Id = s.ResourceSpecialtyID,
                                                Description = _frenchLanguage ? s.Name_FR : s.Name_EN
                                            }).OrderBy(s => s.Description).ToList());
            }

            return model;
        }

        public MaintenanceISTMembersContactInfoViewModel GetMaintenanceISTMembersContactInfoViewModel(int resourceId)
        {
            MaintenanceISTMembersContactInfoViewModel model = null;

            var resource = (from r in this._repository.Resources
                            where r.ResourceID == resourceId
                            && r.Active
                            select r).SingleOrDefault();

            if (resource != null)
            {
                model = new MaintenanceISTMembersContactInfoViewModel(resource);

                model.Countries = new List<ListItem> { null };
                model.Countries.AddRange((from c in this._repository.Countries
                                          select new ListItem
                                          {
                                              Id = c.CountryID,
                                              Description = _frenchLanguage ? c.Name_FR : c.Name_EN
                                          }).ToList());

                model.Provinces = new List<ListItem> { null };
                model.Provinces.AddRange((from p in this._repository.Provinces
                                          select new ListItem
                                          {
                                              Id = p.ProvinceID,
                                              Description = _frenchLanguage ? p.Name_FR : p.Name_EN
                                          }).ToList());
            }

            return model;
        }

        public List<MaintenanceISTMembersSportsInfoViewModel> GetMaintenanceISTMembersSportsInfoViewModel(int resourceId)
        {
            List<MaintenanceISTMembersSportsInfoViewModel> model = null;

            var resource = (from r in this._repository.Resources
                            where r.ResourceID == resourceId
                            && r.Active
                            select r).SingleOrDefault();

            if (resource != null)
            {
                model = new List<MaintenanceISTMembersSportsInfoViewModel>();

                //Constant.SportId.All is used as Athlete personal coach
                var sports = (from s in resource.ResourceSports where s.SportID != Constant.SportId.All select s).OrderBy(x => x.SportID).ToList();

                foreach (var sport in sports)
                {
                    var resourceSport = new MaintenanceISTMembersSportsInfoViewModel();

                    resourceSport.ResourceId = sport.ResourceID;
                    resourceSport.ResourceSportId = sport.ResourceSportID;
                    resourceSport.SportsId = sport.SportID;
                    resourceSport.SportName = SportBL.FormatSportName(sport.Sport, _frenchLanguage);
                    resourceSport.YearId = sport.YearID;
                    resourceSport.HasComments = !string.IsNullOrEmpty(sport.Comments);
                    resourceSport.CrossCompetency = sport.ISTCrossCompetency;
                    resourceSport.Primary = sport.ISTPrimary;

                    model.Add(resourceSport);
                }
            }

            return model;
        }

        public List<MaintenanceISTMembersSportsInfoViewModel> GetMaintenanceISTMembersSportsInfoViewModelParameter(int resourceId, GridCommand command)
        {
            List<MaintenanceISTMembersSportsInfoViewModel> model = GetMaintenanceISTMembersSportsInfoViewModel(resourceId);

            IQueryable<MaintenanceISTMembersSportsInfoViewModel> modelQuery = model.AsQueryable();

            // Apply filtering from the UI
            if (command.FilterDescriptors.Any())
            {
                modelQuery = FilterMembers<MaintenanceISTMembersSportsInfoViewModel>(modelQuery, command.FilterDescriptors);
            }

            // Default sorting
            if (command.SortDescriptors.Any())
            {
                modelQuery = SortMembers<MaintenanceISTMembersSportsInfoViewModel>(modelQuery, command.SortDescriptors);
            }
            else
            {
                // Apply the default sorting
                modelQuery = modelQuery.OrderBy(x => x.SportName);
            }

            model = modelQuery.ToList();

            return model;
        }

        public List<MaintenanceISTMembersQualificationInfoViewModel> GetMaintenanceISTMembersQualificationInfoViewModel(int resourceId)
        {
            List<MaintenanceISTMembersQualificationInfoViewModel> model = null;

            var resource = (from r in this._repository.Resources
                            where r.ResourceID == resourceId
                            && r.Active
                            select r).SingleOrDefault();

            if (resource != null)
            {
                model = new List<MaintenanceISTMembersQualificationInfoViewModel>();

                foreach (var qualification in resource.ResourceQualifications)
                {
                    var resourceQualification = new MaintenanceISTMembersQualificationInfoViewModel();

                    resourceQualification.ResourceId = qualification.ResourceID;
                    resourceQualification.ResourceQualificationId = qualification.ResourceQualificationID;
                    resourceQualification.QualificationTypeId = qualification.QualificationTypeID;
                    resourceQualification.QualificationTypeName = _frenchLanguage ? qualification.QualificationType.Name_FR : qualification.QualificationType.Name_EN;
                    resourceQualification.Qualified = qualification.Qualified;
                    resourceQualification.HasComments = !string.IsNullOrEmpty(qualification.Comments);
                    resourceQualification.Description = qualification.Description;

                    model.Add(resourceQualification);
                }
            }

            return model;
        }

        public List<MaintenanceISTMembersPoolInfoViewModel> GetMaintenanceISTMembersPoolInfoViewModel(int resourceId)
        {
            List<MaintenanceISTMembersPoolInfoViewModel> model = null;

            var resource = (from r in this._repository.Resources
                            where r.ResourceID == resourceId
                            && r.Active
                            select r).SingleOrDefault();

            if (resource != null)
            {
                var results = (from p in this._repository.ResourcePoolAssignments
                               where (resource.Pooled && p.ResourceIDPool == resource.ResourceID)
                               || (p.ResourceIDAssignment == resource.ResourceID)
                               select new
                               {
                                   HasComments = !string.IsNullOrEmpty(p.Comments),
                                   ResourceIdAssignment = p.ResourceIDAssignment,
                                   ResourceIdAssignmentFirstName = p.Resource.FirstName,
                                   ResourceIdAssignmentLastName = p.Resource.LastName,
                                   ResourceIdAssignmentResourceId = p.Resource.ResourceID,
                                   ResourceIdAssignmentResourceTypeName = _frenchLanguage ? p.Resource.ResourceType.Name_FR : p.Resource.ResourceType.Name_EN,
                                   ResourceIdPool = p.ResourceIDPool,
                                   ResourceIdPoolFirstName = p.Resource1.FirstName,
                                   ResourceIdPoolLastName = p.Resource1.LastName,
                                   ResourceIdPoolResourceId = p.Resource1.ResourceID,
                                   ResourceIdPoolResourceTypeName = _frenchLanguage ? p.Resource1.ResourceType.Name_FR : p.Resource1.ResourceType.Name_EN,
                                   ResourcePoolAssignmentId = p.ResourcePoolAssignmentID
                               }).ToList();

                model = (from r in results
                         select new MaintenanceISTMembersPoolInfoViewModel
                         {
                             HasComments = r.HasComments,
                             ResourceIdAssignment = r.ResourceIdAssignment,
                             ResourceIdAssignmentName = string.Format("{0} - {1} {2} [{3}]", r.ResourceIdAssignmentResourceId, r.ResourceIdAssignmentFirstName, r.ResourceIdAssignmentLastName, r.ResourceIdAssignmentResourceTypeName),
                             ResourceIdPool = r.ResourceIdPool,
                             ResourceIdPoolName = string.Format("{0} - {1} {2} [{3}]", r.ResourceIdPoolResourceId, r.ResourceIdPoolFirstName, r.ResourceIdPoolLastName, r.ResourceIdPoolResourceTypeName),
                             ResourcePoolAssignmentId = r.ResourcePoolAssignmentId
                         }).ToList();
            }

            return model;
        }

        #endregion Get Maintenance IST

        #region Get Maintenance NSO

        public List<MaintenanceNSOMembersGridViewModel> GetMaintenanceNSOMembersGridViewModel(string selectedSportId, GridCommand command, out int total)
        {
            total = 0;

            if (string.IsNullOrEmpty(selectedSportId))
            {
                selectedSportId = Constant.SportId.All;
            }

            SortDescriptor sort = new SortDescriptor()
            {
                Member = "",
                SortDirection = ListSortDirection.Ascending
            };

            if (command.SortDescriptors.Any())
            {
                sort = command.SortDescriptors[0];
            }

            string filterString = string.Empty;
            filterString = GetFilterString(filterString, command.FilterDescriptors);

            ObjectParameter parameter = new ObjectParameter("TotalCount", typeof(int));

            if (command.PageSize == 0)
            {
                command.PageSize = 10;
                command.Page = 1;
            }

            var query = this._repository.uspGetMaintenanceNSOMembersGridViewModel(
                                _frenchLanguage ? Constant.LanguageCode.French : Constant.LanguageCode.English,
                                Constant.ResourceType.NSOMembers,
                                selectedSportId,
                                false,
                                command.PageSize,
                                command.Page,
                                command.SortDescriptors.Count,
                                sort.Member,
                                sort.SortDirection == ListSortDirection.Ascending ? "ASC" : "DESC",
                                command.FilterDescriptors.Count,
                                filterString,
                                parameter).ToList();

            List<MaintenanceNSOMembersGridViewModel> model = new List<MaintenanceNSOMembersGridViewModel>();

            foreach (var item in query)
            {
                MaintenanceNSOMembersGridViewModel modelItem = new MaintenanceNSOMembersGridViewModel();

                modelItem.LastName = item.LastName;
                modelItem.FirstName = item.FirstName;
                modelItem.CountryId = item.CountryId;
                modelItem.CountryName = item.CountryName;
                modelItem.BirthDate = item.BirthDate;
                modelItem.GenderId = item.GenderId;
                modelItem.GenderName = item.GenderName;
                modelItem.ResourceId = item.ResourceId;
                modelItem.Pooled = item.Pooled;
                modelItem.TBD = item.TBD;
                modelItem.ResourceSpecialtyId = item.ResourceSpecialtyId;
                modelItem.ResourceSpecialtyName = item.ResourceSpecialtyName;

                model.Add(modelItem);
            }

            total = (int)parameter.Value;

            return model;
        }

        //public List<MaintenanceNSOMembersGridViewModel> GetMaintenanceNSOMembersGridViewModel(string selectedSportId, GridCommand command, out int total)
        //{
        //    total = 0;

        //    if (string.IsNullOrEmpty(selectedSportId))
        //    {
        //        selectedSportId = Constant.SportId.All;
        //    }

        //    List<MaintenanceNSOMembersGridViewModel> query = (from r in this._repository.Resources
        //                                                      where r.ResourceTypeID == Constant.ResourceType.NSOMembers
        //                                                      && (selectedSportId == Constant.SportId.All ||
        //                                                      (r.ResourceSports.Any(s => s.Sport.SportLevelID == Constant.SportLevelId.Sport && s.SportID == selectedSportId)
        //                                                         || r.ResourceSports.Any(s => s.Sport.SportLevelID == Constant.SportLevelId.Discipline && s.Sport.ParentSport.SportID == selectedSportId)
        //                                                         || r.ResourceSports.Any(s => s.Sport.SportLevelID == Constant.SportLevelId.Event && s.Sport.ParentSport.ParentSport.SportID == selectedSportId)
        //                                                      )) // Only NSO Members that have sports under the selected sport id
        //                                                      && r.Active
        //                                                      select new MaintenanceNSOMembersGridViewModel
        //                                                      {
        //                                                          GenderId = r.Gender,
        //                                                          GenderName = (from g in this._repository.Decodes
        //                                                                        where g.DecodeCategoryID == Constant.DecodeCategoryId.Gender
        //                                                                        && g.DecodeID == r.Gender
        //                                                                        select _frenchLanguage ? g.DecodeID_FR : g.DecodeID_EN).FirstOrDefault(),
        //                                                          FirstName = r.FirstName,
        //                                                          LastName = r.LastName,
        //                                                          DateOfBirth = r.DateOfBirth,
        //                                                          ResourceId = r.ResourceID,
        //                                                          ResourceSpecialtyId = r.ResourceSpecialty.ResourceSpecialtyID,
        //                                                          CountryId = r.CountryID,
        //                                                          CountryName = _frenchLanguage ? r.Country.Name_FR : r.Country.Name_EN,
        //                                                          Pooled = r.Pooled,
        //                                                          TBD = r.TBD,
        //                                                          ResourceSpecialtyName = _frenchLanguage ? r.ResourceSpecialty.Name_FR : r.ResourceSpecialty.Name_EN
        //                                                      }).ToList();

        //    IQueryable<MaintenanceNSOMembersGridViewModel> modelQuery = query.AsQueryable();

        //    //Apply filtering from the UI
        //    if (command.FilterDescriptors.Any())
        //    {
        //        //modelQuery = FilterNSOMembers(modelQuery, command.FilterDescriptors);
        //        modelQuery = FilterMembers<MaintenanceNSOMembersGridViewModel>(modelQuery, command.FilterDescriptors);
        //    }

        //    //Default sorting
        //    if (command.SortDescriptors.Count == 0)
        //    {
        //        modelQuery = modelQuery.OrderBy(x => x.LastName).ThenBy(x => x.FirstName);
        //    }

        //    // Apply sorting from the UI
        //    foreach (SortDescriptor sortDescriptor in command.SortDescriptors)
        //    {
        //        if (sortDescriptor.SortDirection == ListSortDirection.Ascending)
        //        {
        //            switch (sortDescriptor.Member)
        //            {
        //                case "ResourceId":
        //                    modelQuery = modelQuery.OrderBy(x => x.ResourceId);
        //                    break;
        //                case "LastName":
        //                    modelQuery = modelQuery.OrderBy(x => x.LastName);
        //                    break;
        //                case "FirstName":
        //                    modelQuery = modelQuery.OrderBy(x => x.FirstName);
        //                    break;
        //                case "CountryName":
        //                    modelQuery = modelQuery.OrderBy(x => x.CountryName);
        //                    break;
        //                case "BirthDate":
        //                    modelQuery = modelQuery.OrderBy(x => x.DateOfBirth);
        //                    break;
        //                case "Pooled":
        //                    modelQuery = modelQuery.OrderBy(x => x.Pooled);
        //                    break;
        //                case "TBD":
        //                    modelQuery = modelQuery.OrderBy(x => x.TBD);
        //                    break;
        //                case "GenderName":
        //                    modelQuery = modelQuery.OrderBy(x => x.GenderName);
        //                    break;
        //                case "ResourceSpecialtyName":
        //                    modelQuery = modelQuery.OrderBy(x => x.ResourceSpecialtyName);
        //                    break;
        //            }
        //        }
        //        else
        //        {
        //            switch (sortDescriptor.Member)
        //            {
        //                case "ResourceId":
        //                    modelQuery = modelQuery.OrderByDescending(x => x.ResourceId);
        //                    break;
        //                case "LastName":
        //                    modelQuery = modelQuery.OrderByDescending(x => x.LastName);
        //                    break;
        //                case "FirstName":
        //                    modelQuery = modelQuery.OrderByDescending(x => x.FirstName);
        //                    break;
        //                case "CountryName":
        //                    modelQuery = modelQuery.OrderByDescending(x => x.CountryName);
        //                    break;
        //                case "BirthDate":
        //                    modelQuery = modelQuery.OrderByDescending(x => x.DateOfBirth);
        //                    break;
        //                case "Pooled":
        //                    modelQuery = modelQuery.OrderByDescending(x => x.Pooled);
        //                    break;
        //                case "TBD":
        //                    modelQuery = modelQuery.OrderByDescending(x => x.TBD);
        //                    break;
        //                case "GenderName":
        //                    modelQuery = modelQuery.OrderByDescending(x => x.GenderName);
        //                    break;
        //                case "ResourceSpecialtyName":
        //                    modelQuery = modelQuery.OrderByDescending(x => x.ResourceSpecialtyName);
        //                    break;
        //            }
        //        }
        //    }

        //    //Get the count for UI paging
        //    total = modelQuery.Count();

        //    //Set the paging size
        //    if (command.PageSize > 0)
        //    {
        //        //Set start index to take records from
        //        modelQuery = modelQuery.Skip((command.Page - 1) * command.PageSize);
        //    }

        //    //Return only the records needed to fit on the current page
        //    modelQuery = modelQuery.Take(command.PageSize);

        //    //Now set the model with the query elements
        //    var model = modelQuery.ToList();

        //    foreach (var item in model)
        //    {
        //        if (item.DateOfBirth != null)
        //        {
        //            item.BirthDate = string.Format("{0:yyyy-MM-dd}", item.DateOfBirth);
        //        }
        //    }

        //    return model;
        //}

        public List<MaintenanceNSOMembersGridViewModel> GetMaintenanceNSOMembersGridViewModelParameter(string selectedSportId, GridCommand command)
        {
            if (string.IsNullOrEmpty(selectedSportId))
            {
                selectedSportId = Constant.SportId.All;
            }

            SortDescriptor sort = new SortDescriptor()
            {
                Member = "",
                SortDirection = ListSortDirection.Ascending
            };

            if (command.SortDescriptors.Any())
            {
                sort = command.SortDescriptors[0];
            }

            string filterString = string.Empty;
            filterString = GetFilterString(filterString, command.FilterDescriptors);

            ObjectParameter parameter = new ObjectParameter("TotalCount", typeof(int));

            var query = this._repository.uspGetMaintenanceNSOMembersGridViewModel(
                                _frenchLanguage ? Constant.LanguageCode.French : Constant.LanguageCode.English,
                                Constant.ResourceType.NSOMembers,
                                selectedSportId,
                                true,
                                command.PageSize,
                                command.Page,
                                command.SortDescriptors.Count,
                                sort.Member,
                                sort.SortDirection == ListSortDirection.Ascending ? "ASC" : "DESC",
                                command.FilterDescriptors.Count,
                                filterString,
                                parameter).ToList();

            List<MaintenanceNSOMembersGridViewModel> model = new List<MaintenanceNSOMembersGridViewModel>();

            foreach (var item in query)
            {
                MaintenanceNSOMembersGridViewModel modelItem = new MaintenanceNSOMembersGridViewModel();

                modelItem.LastName = item.LastName;
                modelItem.FirstName = item.FirstName;
                modelItem.CountryId = item.CountryId;
                modelItem.CountryName = item.CountryName;
                modelItem.BirthDate = item.BirthDate;
                modelItem.GenderId = item.GenderId;
                modelItem.GenderName = item.GenderName;
                modelItem.ResourceId = item.ResourceId;
                modelItem.Pooled = item.Pooled;
                modelItem.TBD = item.TBD;
                modelItem.ResourceSpecialtyId = item.ResourceSpecialtyId;
                modelItem.ResourceSpecialtyName = item.ResourceSpecialtyName;

                model.Add(modelItem);
            }

            return model;
        }

        //public List<MaintenanceNSOMembersGridViewModel> GetMaintenanceNSOMembersGridViewModelParameter(string selectedSportId, GridCommand command)
        //{
        //    if (string.IsNullOrEmpty(selectedSportId))
        //    {
        //        selectedSportId = Constant.SportId.All;
        //    }

        //    List<MaintenanceNSOMembersGridViewModel> query = (from r in this._repository.Resources
        //                                                      where r.ResourceTypeID == Constant.ResourceType.NSOMembers
        //                                                      && (selectedSportId == Constant.SportId.All ||
        //                                                      (r.ResourceSports.Any(s => s.Sport.SportLevelID == Constant.SportLevelId.Sport && s.SportID == selectedSportId)
        //                                                         || r.ResourceSports.Any(s => s.Sport.SportLevelID == Constant.SportLevelId.Discipline && s.Sport.ParentSport.SportID == selectedSportId)
        //                                                         || r.ResourceSports.Any(s => s.Sport.SportLevelID == Constant.SportLevelId.Event && s.Sport.ParentSport.ParentSport.SportID == selectedSportId)
        //                                                      )) // Only NSO Members that have sports under the selected sport id
        //                                                      && r.Active
        //                                                      select new MaintenanceNSOMembersGridViewModel
        //                                                      {
        //                                                          FirstName = r.FirstName,
        //                                                          LastName = r.LastName,
        //                                                          DateOfBirth = r.DateOfBirth,
        //                                                          ResourceId = r.ResourceID,
        //                                                          CountryId = r.CountryID,
        //                                                          CountryName = _frenchLanguage ? r.Country.Name_FR : r.Country.Name_EN,
        //                                                          GenderId = r.Gender,
        //                                                          GenderName = (from g in this._repository.Decodes
        //                                                                        where g.DecodeCategoryID == Constant.DecodeCategoryId.Gender
        //                                                                        && g.DecodeID == r.Gender
        //                                                                        select _frenchLanguage ? g.DecodeID_FR : g.DecodeID_EN).FirstOrDefault(),
        //                                                          Pooled = r.Pooled,
        //                                                          TBD = r.TBD,
        //                                                          ResourceSpecialtyId = r.ResourceSpecialty.ResourceSpecialtyID,
        //                                                          ResourceSpecialtyName = _frenchLanguage ? r.ResourceSpecialty.Name_FR : r.ResourceSpecialty.Name_EN
        //                                                      }).ToList();

        //    foreach (var item in query)
        //    {
        //        if (item.DateOfBirth != null)
        //        {
        //            item.BirthDate = string.Format("{0:yyyy-MM-dd}", item.DateOfBirth);
        //        }
        //    }

        //    IQueryable<MaintenanceNSOMembersGridViewModel> modelQuery = query.AsQueryable();

        //    //Apply filtering from the UI
        //    if (command.FilterDescriptors.Any())
        //    {
        //        modelQuery = FilterMembers<MaintenanceNSOMembersGridViewModel>(modelQuery, command.FilterDescriptors);
        //    }

        //    //Default sorting
        //    if (command.SortDescriptors.Any())
        //    {
        //        modelQuery = SortMembers<MaintenanceNSOMembersGridViewModel>(modelQuery, command.SortDescriptors);
        //    }
        //    else
        //    {
        //        modelQuery = modelQuery.OrderBy(x => x.LastName).ThenBy(x => x.FirstName);
        //    }

        //    return modelQuery.ToList();
        //}

        public MaintenanceNSOMembersGeneralInfoViewModel GetMaintenanceNSOMembersGeneralInfoViewModel(int resourceId)
        {
            MaintenanceNSOMembersGeneralInfoViewModel model = null;

            var resource = (from r in this._repository.Resources
                            where r.ResourceID == resourceId
                            select r).SingleOrDefault();

            if (resource != null)
            {
                model = new MaintenanceNSOMembersGeneralInfoViewModel(resource);

                model.Countries = (from c in this._repository.Countries
                                   select new ListItem
                                   {
                                       Id = c.CountryID,
                                       Description = _frenchLanguage ? c.Name_FR : c.Name_EN
                                   }).ToList();

                model.CSCs = new List<ListItem> { null };
                model.CSCs.AddRange((from o in this._repository.Organizations
                                     where o.OrganizationTypeID == Constant.OrganizationTypeId.CSC
                                     select new ListItem
                                     {
                                         Id = o.OrganizationID,
                                         Description = _frenchLanguage ? o.Name_FR : o.Name_EN
                                     }).ToList());

                model.Genders = this._decodeBL.GetHumanGenders(true);

                model.Specialties = new List<ListItem> { null };
                model.Specialties.AddRange((from s in this._repository.ResourceSpecialties
                                            where s.ResourceTypeID == Constant.ResourceType.NSOMembers
                                            select new ListItem
                                            {
                                                Id = s.ResourceSpecialtyID,
                                                Description = _frenchLanguage ? s.Name_FR : s.Name_EN
                                            }).OrderBy(s => s.Description).ToList());
            }

            return model;
        }

        public MaintenanceNSOMembersContactInfoViewModel GetMaintenanceNSOMembersContactInfoViewModel(int resourceId)
        {
            MaintenanceNSOMembersContactInfoViewModel model = null;

            var resource = (from r in this._repository.Resources
                            where r.ResourceID == resourceId
                            && r.Active
                            select r).SingleOrDefault();

            if (resource != null)
            {
                model = new MaintenanceNSOMembersContactInfoViewModel(resource);

                model.Countries = new List<ListItem> { null };
                model.Countries.AddRange((from c in this._repository.Countries
                                          select new ListItem
                                          {
                                              Id = c.CountryID,
                                              Description = _frenchLanguage ? c.Name_FR : c.Name_EN
                                          }).ToList());

                model.Provinces = new List<ListItem> { null };
                model.Provinces.AddRange((from p in this._repository.Provinces
                                          select new ListItem
                                          {
                                              Id = p.ProvinceID,
                                              Description = _frenchLanguage ? p.Name_FR : p.Name_EN
                                          }).ToList());
            }

            return model;
        }

        public List<MaintenanceNSOMembersSportsInfoViewModel> GetMaintenanceNSOMembersSportsInfoViewModel(int resourceId)
        {
            List<MaintenanceNSOMembersSportsInfoViewModel> model = null;

            var resource = (from r in this._repository.Resources
                            where r.ResourceID == resourceId
                            && r.Active
                            select r).SingleOrDefault();

            if (resource != null)
            {
                model = new List<MaintenanceNSOMembersSportsInfoViewModel>();

                //Constant.SportId.All is used as Athlete personal coach
                var sports = (from s in resource.ResourceSports where s.SportID != Constant.SportId.All select s).ToList();

                foreach (var sport in sports)
                {
                    var resourceSport = new MaintenanceNSOMembersSportsInfoViewModel();

                    resourceSport.ResourceId = sport.ResourceID;
                    resourceSport.ResourceSportId = sport.ResourceSportID;
                    resourceSport.SportsId = sport.SportID;
                    resourceSport.SportName = SportBL.FormatSportName(sport.Sport, _frenchLanguage);
                    resourceSport.YearId = sport.YearID;
                    resourceSport.HasComments = !string.IsNullOrEmpty(sport.Comments);
                    resourceSport.CrossCompetency = sport.ISTCrossCompetency;
                    resourceSport.Primary = sport.ISTPrimary;

                    model.Add(resourceSport);
                }
            }

            return model;
        }

        public List<MaintenanceNSOMembersSportsInfoViewModel> GetMaintenanceNSOMembersSportsInfoViewModelParameter(int resourceId, GridCommand command)
        {
            List<MaintenanceNSOMembersSportsInfoViewModel> model = GetMaintenanceNSOMembersSportsInfoViewModel(resourceId);

            IQueryable<MaintenanceNSOMembersSportsInfoViewModel> modelQuery = model.AsQueryable();

            // Apply filtering from the UI
            if (command.FilterDescriptors.Any())
            {
                modelQuery = FilterMembers<MaintenanceNSOMembersSportsInfoViewModel>(modelQuery, command.FilterDescriptors);
            }

            // Default sorting
            if (command.SortDescriptors.Any())
            {
                modelQuery = SortMembers<MaintenanceNSOMembersSportsInfoViewModel>(modelQuery, command.SortDescriptors);
            }
            else
            {
                // Apply the default sorting
                modelQuery = modelQuery.OrderBy(x => x.SportName);
            }

            model = modelQuery.ToList();

            return model;
        }
        
        public List<MaintenanceNSOMembersQualificationInfoViewModel> GetMaintenanceNSOMembersQualificationInfoViewModel(int resourceId)
        {
            List<MaintenanceNSOMembersQualificationInfoViewModel> model = null;

            var resource = (from r in this._repository.Resources
                            where r.ResourceID == resourceId
                            && r.Active
                            select r).SingleOrDefault();

            if (resource != null)
            {
                model = new List<MaintenanceNSOMembersQualificationInfoViewModel>();

                foreach (var qualification in resource.ResourceQualifications)
                {
                    var resourceQualification = new MaintenanceNSOMembersQualificationInfoViewModel();

                    resourceQualification.ResourceId = qualification.ResourceID;
                    resourceQualification.ResourceQualificationId = qualification.ResourceQualificationID;
                    resourceQualification.QualificationTypeId = qualification.QualificationTypeID;
                    resourceQualification.QualificationTypeName = _frenchLanguage ? qualification.QualificationType.Name_FR : qualification.QualificationType.Name_EN;
                    resourceQualification.Qualified = qualification.Qualified;
                    resourceQualification.HasComments = !string.IsNullOrEmpty(qualification.Comments);
                    resourceQualification.Description = qualification.Description;

                    model.Add(resourceQualification);
                }
            }

            return model;
        }

        public List<MaintenanceNSOMembersPoolInfoViewModel> GetMaintenanceNSOMembersPoolInfoViewModel(int resourceId)
        {
            List<MaintenanceNSOMembersPoolInfoViewModel> model = null;

            var resource = (from r in this._repository.Resources
                            where r.ResourceID == resourceId
                            && r.Active
                            select r).SingleOrDefault();

            if (resource != null)
            {
                var results = (from p in this._repository.ResourcePoolAssignments
                               where (resource.Pooled && p.ResourceIDPool == resource.ResourceID)
                               || (p.ResourceIDAssignment == resource.ResourceID)
                               select new
                               {
                                   HasComments = !string.IsNullOrEmpty(p.Comments),
                                   ResourceIdAssignment = p.ResourceIDAssignment,
                                   ResourceIdAssignmentFirstName = p.Resource.FirstName,
                                   ResourceIdAssignmentLastName = p.Resource.LastName,
                                   ResourceIdAssignmentResourceId = p.Resource.ResourceID,
                                   ResourceIdAssignmentResourceTypeName = _frenchLanguage ? p.Resource.ResourceType.Name_FR : p.Resource.ResourceType.Name_EN,
                                   ResourceIdPool = p.ResourceIDPool,
                                   ResourceIdPoolFirstName = p.Resource1.FirstName,
                                   ResourceIdPoolLastName = p.Resource1.LastName,
                                   ResourceIdPoolResourceId = p.Resource1.ResourceID,
                                   ResourceIdPoolResourceTypeName = _frenchLanguage ? p.Resource1.ResourceType.Name_FR : p.Resource1.ResourceType.Name_EN,
                                   ResourcePoolAssignmentId = p.ResourcePoolAssignmentID
                               }).ToList();

                model = (from r in results
                         select new MaintenanceNSOMembersPoolInfoViewModel
                         {
                             HasComments = r.HasComments,
                             ResourceIdAssignment = r.ResourceIdAssignment,
                             ResourceIdAssignmentName = string.Format("{0} - {1} {2} [{3}]", r.ResourceIdAssignmentResourceId, r.ResourceIdAssignmentFirstName, r.ResourceIdAssignmentLastName, r.ResourceIdAssignmentResourceTypeName),
                             ResourceIdPool = r.ResourceIdPool,
                             ResourceIdPoolName = string.Format("{0} - {1} {2} [{3}]", r.ResourceIdPoolResourceId, r.ResourceIdPoolFirstName, r.ResourceIdPoolLastName, r.ResourceIdPoolResourceTypeName),
                             ResourcePoolAssignmentId = r.ResourcePoolAssignmentId
                         }).ToList();
            }

            return model;
        }

        #endregion Get Maintenance NSO

        #region Populate Maintenance

        public void PopulateMaintenanceResourcesViewData(ViewDataDictionary viewData, string resourceId, string selectedSportId)
        {
            var id = 0;

            if (!string.IsNullOrEmpty(resourceId))
            {
                id = int.Parse(resourceId);
            }

            //viewData["genders"] = this._decodeBL.GetHumanGenders(true);

            //var countries = from c in this._repository.Countries
            //                select new ListItem
            //                {
            //                    Id = c.CountryID,
            //                    Description = _frenchLanguage ? c.Name_FR : c.Name_EN
            //                };

            //viewData["countries"] = countries.OrderBy(c => c.Description).ToList();

            viewData["resourceId"] = resourceId;

            if (id == 0)
            {
                viewData["resourceName"] = BusinessLocalization.SelectResource;
            }
            else
            {
                viewData["resourceName"] = (from r in this._repository.Resources
                                            where r.ResourceID == id
                                            && r.Active
                                            select r.FirstName + " " + r.LastName).SingleOrDefault();
            }

            viewData["sportName"] = (from s in this._repository.Sports
                                        where s.SportID == selectedSportId
                                        && s.Active
                                        select _frenchLanguage ? s.Name_FR : s.Name_EN).SingleOrDefault();

            var selectedSportLevel = (from s in this._repository.Sports
                                      where s.SportID == selectedSportId
                                      select s.SportLevelID).SingleOrDefault();

            if (selectedSportLevel == Constant.SportLevelId.Sport)
            {
                viewData["selectedSport"] = selectedSportId;
            }
            else if (selectedSportLevel == Constant.SportLevelId.Season)
            {
                viewData["selectedSport"] = (from s in this._repository.Sports
                                             where s.SportID == selectedSportId
                                             from c in s.ChildSports
                                             where c.Active
                                             orderby c.Name_EN
                                             select c.SportID).FirstOrDefault();
            }
            else if (selectedSportLevel == Constant.SportLevelId.Olympic)
            {
                viewData["selectedSport"] = (from s in this._repository.Sports
                                             where s.SportID == selectedSportId
                                             from c in s.ChildSports
                                             where c.Active
                                             from c2 in c.ChildSports
                                             where c2.Active
                                             orderby c2.Name_EN
                                             select c2.SportID).FirstOrDefault();
            }
            else if (selectedSportLevel == Constant.SportLevelId.All)
            {
                viewData["selectedSport"] = (from s in this._repository.Sports
                                             where s.SportLevelID == Constant.SportLevelId.Sport
                                             && s.Active
                                             orderby s.Name_EN
                                             select s.SportID).FirstOrDefault();
            }

            //viewData["years"] = (from y in this._repository.Years
            //                     where y.Active == null || (bool)y.Active
            //                        orderby y.YearID
            //                        select new ListItem
            //                        {
            //                            Id = y.YearID,
            //                            Description = y.YearID
            //                        }).ToList();

            //viewData["affiliate_types"] = (from t in this._repository.AffiliateTypes
            //                                select new ListItem
            //                                {
            //                                    Id = t.AffiliateTypeID,
            //                                    Description = _frenchLanguage ? t.Name_FR : t.Name_EN
            //                                }).ToList();
        }

        public void PopulateMaintenanceResourcesTabsExceptResultViewData(ViewDataDictionary viewData, string resourceId)
        {
            var id = 0;

            if (!string.IsNullOrEmpty(resourceId))
            {
                id = int.Parse(resourceId);
            }

            viewData["resourceId"] = resourceId;

            if (id == 0)
            {
                viewData["resourceName"] = BusinessLocalization.SelectResource;
            }
            else
            {
                viewData["resourceName"] = (from r in this._repository.Resources
                                            where r.ResourceID == id
                                            && r.Active
                                            select r.FirstName + " " + r.LastName).SingleOrDefault();
            }
        }

        public void PopulateMaintenanceResourcesResultTabViewData(ViewDataDictionary viewData, string resourceId)
        {
            var id = 0;

            if (!string.IsNullOrEmpty(resourceId))
            {
                id = int.Parse(resourceId);
            }

            viewData["resourceId"] = resourceId;

            if (id == 0)
            {
                viewData["resourceName"] = BusinessLocalization.SelectResource;
            }
            else
            {
                viewData["resourceName"] = (from r in this._repository.Resources
                                            where r.ResourceID == id
                                            && r.Active
                                            select r.FirstName + " " + r.LastName).SingleOrDefault();
            }

            if (!string.IsNullOrEmpty(resourceId))
            {
                viewData["AthleteResultsCompetitionDate"] = this._decodeBL.GetMostRecentOlympicGameInfo(resourceId).CompetitionDate;
            }
        }

        public void PopulateMaintenanceResourcesBenchmarkViewData(ViewDataDictionary viewData, string resourceId)
        {
            var id = 0;

            if (!string.IsNullOrEmpty(resourceId))
            {
                id = int.Parse(resourceId);
            }

            var benchmarkTypes = (from b in this._repository.BenchmarkTypes
                                    where b.BenchmarkTypeID > 0 //Remove Non-Benchmark
                                    select new
                                    {
                                        Id = b.BenchmarkTypeID,
                                        Description = _frenchLanguage ? b.Name_FR : b.Name_EN
                                    }).ToList();

            //Workaround for Linq to Sql limitation using toString()
            viewData["benchmark_types"] = (from b in benchmarkTypes
                                            orderby b.Description
                                            select new ListItem
                                            {
                                                Id = b.Id.ToString(),
                                                Description = b.Description
                                            }).ToList();

            var benchmarkTiers = (from t in this._repository.BenchmarkTiers
                                    select new
                                    {
                                        Id = t.BenchmarkTierID,
                                        Description = _frenchLanguage ? t.Name_FR : t.Name_EN,
                                        FilterOnId = t.SportID
                                    }).ToList();

            //Workaround for Linq to Sql limitation using toString()
            viewData["benchmark_tiers"] = (from t in benchmarkTiers
                                            orderby t.Description
                                            select new FilteredListItem
                                            {
                                                Id = t.Id.ToString(),
                                                Description = t.Description,
                                                FilterOnId = t.FilterOnId
                                            }).ToList();
        }

        public void PopulateMaintenanceISTMembersViewData(ViewDataDictionary viewData, string resourceId, string selectedSportId)
        {
            var id = 0;

            if (!string.IsNullOrEmpty(resourceId))
            {
                id = int.Parse(resourceId);
            }

            //viewData["genders"] = this._decodeBL.GetHumanGenders(true);

            //var countries = from c in this._repository.Countries
            //                select new ListItem
            //                {
            //                    Id = c.CountryID,
            //                    Description = _frenchLanguage ? c.Name_FR : c.Name_EN
            //                };

            //viewData["countries"] = countries.OrderBy(c => c.Description).ToList();

            viewData["resourceId"] = resourceId;

            if (id == 0)
            {
                viewData["resourceName"] = BusinessLocalization.SelectResource;
                //viewData["resourcePooled"] = false;
            }
            else
            {
                var resource = (from r in this._repository.Resources
                                where r.ResourceID == id
                                && r.Active
                                select r).SingleOrDefault();

                viewData["resourceName"] = resource.FirstName + " " + resource.LastName;
                //viewData["resourcePooled"] = resource.Pooled;
            }

            //viewData["years"] = (from y in this._repository.Years
            //                     where y.Active == null || (bool)y.Active
            //                     orderby y.YearID
            //                     select new ListItem
            //                     {
            //                         Id = y.YearID,
            //                         Description = y.YearID
            //                     }).ToList();

            viewData["sportName"] = (from s in this._repository.Sports
                                     where s.SportID == selectedSportId
                                     && s.Active
                                     select _frenchLanguage ? s.Name_FR : s.Name_EN).SingleOrDefault();

            var selectedSportLevel = (from s in this._repository.Sports
                                      where s.SportID == selectedSportId
                                      select s.SportLevelID).SingleOrDefault();

            if (selectedSportLevel == Constant.SportLevelId.Sport)
            {
                viewData["selectedSport"] = selectedSportId;
            }
            else if (selectedSportLevel == Constant.SportLevelId.Season)
            {
                viewData["selectedSport"] = (from s in this._repository.Sports
                                             where s.SportID == selectedSportId
                                             from c in s.ChildSports
                                             where c.Active
                                             orderby c.Name_EN
                                             select c.SportID).FirstOrDefault();
            }
            else if (selectedSportLevel == Constant.SportLevelId.Olympic)
            {
                viewData["selectedSport"] = (from s in this._repository.Sports
                                             where s.SportID == selectedSportId
                                             from c in s.ChildSports
                                             where c.Active
                                             from c2 in c.ChildSports
                                             where c2.Active
                                             orderby c2.Name_EN
                                             select c2.SportID).FirstOrDefault();
            }
            else if (selectedSportLevel == Constant.SportLevelId.All)
            {
                viewData["selectedSport"] = (from s in this._repository.Sports
                                             where s.SportLevelID == Constant.SportLevelId.Sport
                                             && s.Active
                                             orderby s.Name_EN
                                             select s.SportID).FirstOrDefault();
            }
        }

        public void PopulateMaintenanceISTMembersTabViewData(ViewDataDictionary viewData, string resourceId)
        {
            var id = 0;

            if (!string.IsNullOrEmpty(resourceId))
            {
                id = int.Parse(resourceId);
            }

            viewData["resourceId"] = resourceId;

            if (id == 0)
            {
                viewData["resourceName"] = BusinessLocalization.SelectResource;
                viewData["resourcePooled"] = false;
            }
            else
            {
                var resource = (from r in this._repository.Resources
                                where r.ResourceID == id
                                && r.Active
                                select r).SingleOrDefault();

                viewData["resourceName"] = resource.FirstName + " " + resource.LastName;
                viewData["resourcePooled"] = resource.Pooled;
            }
        }

        public void PopulateMaintenanceNSOMembersViewData(ViewDataDictionary viewData, string resourceId, string selectedSportId)
        {
            var id = 0;

            if (!string.IsNullOrEmpty(resourceId))
            {
                id = int.Parse(resourceId);
            }

            //viewData["genders"] = this._decodeBL.GetHumanGenders(true);

            //var countries = from c in this._repository.Countries
            //                select new ListItem
            //                {
            //                    Id = c.CountryID,
            //                    Description = _frenchLanguage ? c.Name_FR : c.Name_EN
            //                };

            //viewData["countries"] = countries.OrderBy(c => c.Description).ToList();

            viewData["resourceId"] = resourceId;

            if (id == 0)
            {
                viewData["resourceName"] = BusinessLocalization.SelectResource;
                //viewData["resourcePooled"] = false;
            }
            else
            {
                var resource = (from r in this._repository.Resources
                                where r.ResourceID == id
                                && r.Active
                                select r).SingleOrDefault();

                viewData["resourceName"] = resource.FirstName + " " + resource.LastName;
                //viewData["resourcePooled"] = resource.Pooled;
            }

            //viewData["years"] = (from y in this._repository.Years
            //                     where y.Active == null || (bool)y.Active
            //                        orderby y.YearID
            //                        select new ListItem
            //                        {
            //                            Id = y.YearID,
            //                            Description = y.YearID
            //                        }).ToList();

            viewData["sportName"] = (from s in this._repository.Sports
                                        where s.SportID == selectedSportId
                                        && s.Active
                                        select _frenchLanguage ? s.Name_FR : s.Name_EN).SingleOrDefault();

            var selectedSportLevel = (from s in this._repository.Sports
                                      where s.SportID == selectedSportId
                                      select s.SportLevelID).SingleOrDefault();

            if (selectedSportLevel == Constant.SportLevelId.Sport)
            {
                viewData["selectedSport"] = selectedSportId;
            }
            else if (selectedSportLevel == Constant.SportLevelId.Season)
            {
                viewData["selectedSport"] = (from s in this._repository.Sports
                                             where s.SportID == selectedSportId
                                             from c in s.ChildSports
                                             where c.Active
                                             orderby c.Name_EN
                                             select c.SportID).FirstOrDefault();
            }
            else if (selectedSportLevel == Constant.SportLevelId.Olympic)
            {
                viewData["selectedSport"] = (from s in this._repository.Sports
                                             where s.SportID == selectedSportId
                                             from c in s.ChildSports
                                             where c.Active
                                             from c2 in c.ChildSports
                                             where c2.Active
                                             orderby c2.Name_EN
                                             select c2.SportID).FirstOrDefault();
            }
            else if (selectedSportLevel == Constant.SportLevelId.All)
            {
                viewData["selectedSport"] = (from s in this._repository.Sports
                                             where s.SportLevelID == Constant.SportLevelId.Sport
                                             && s.Active
                                             orderby s.Name_EN
                                             select s.SportID).FirstOrDefault();
            }
        }

        public void PopulateMaintenanceNSOMembersTabViewData(ViewDataDictionary viewData, string resourceId)
        {
            var id = 0;

            if (!string.IsNullOrEmpty(resourceId))
            {
                id = int.Parse(resourceId);
            }

            viewData["resourceId"] = resourceId;

            if (id == 0)
            {
                viewData["resourceName"] = BusinessLocalization.SelectResource;
                viewData["resourcePooled"] = false;
            }
            else
            {
                var resource = (from r in this._repository.Resources
                                where r.ResourceID == id
                                && r.Active
                                select r).SingleOrDefault();

                viewData["resourceName"] = resource.FirstName + " " + resource.LastName;
                viewData["resourcePooled"] = resource.Pooled;
            }
        }

        #endregion Populate Maintenance

        #region Filter

        private IQueryable<MaintenanceAthletesAthleteGridViewModel> FilterAthletes(IQueryable<MaintenanceAthletesAthleteGridViewModel> modelQuery, IFilterDescriptor iFilter)
        {
            var filter = (FilterDescriptor)iFilter;

            var valueObjectString = filter.Value.ToString();
            var filterValue = valueObjectString.ToLower();

            if (filter.Member == "FirstName")
            {
                switch (filter.Operator)
                {
                    case FilterOperator.Contains:
                        modelQuery = modelQuery.Where(r => r.FirstName.ToLower().Contains(filterValue));
                        break;
                    case FilterOperator.EndsWith:
                        modelQuery = modelQuery.Where(r => r.FirstName.ToLower().EndsWith(filterValue));
                        break;
                    case FilterOperator.IsEqualTo:
                        modelQuery = modelQuery.Where(r => r.FirstName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.IsNotEqualTo:
                        modelQuery = modelQuery.Where(r => !r.FirstName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.StartsWith:
                        modelQuery = modelQuery.Where(r => r.FirstName.ToLower().StartsWith(filterValue));
                        break;
                }
            }

            if (filter.Member == "LastName")
            {
                switch (filter.Operator)
                {
                    case FilterOperator.Contains:
                        modelQuery = modelQuery.Where(r => r.LastName.ToLower().Contains(filterValue));
                        break;
                    case FilterOperator.EndsWith:
                        modelQuery = modelQuery.Where(r => r.LastName.ToLower().EndsWith(filterValue));
                        break;
                    case FilterOperator.IsEqualTo:
                        modelQuery = modelQuery.Where(r => r.LastName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.IsNotEqualTo:
                        modelQuery = modelQuery.Where(r => !r.LastName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.StartsWith:
                        modelQuery = modelQuery.Where(r => r.LastName.ToLower().StartsWith(filterValue));
                        break;
                }
            }

            if (filter.Member == "CountryName")
            {
                switch (filter.Operator)
                {
                    case FilterOperator.Contains:
                        modelQuery = modelQuery.Where(r => r.CountryName.ToLower().Contains(filterValue));
                        break;
                    case FilterOperator.EndsWith:
                        modelQuery = modelQuery.Where(r => r.CountryName.ToLower().EndsWith(filterValue));
                        break;
                    case FilterOperator.IsEqualTo:
                        modelQuery = modelQuery.Where(r => r.CountryName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.IsNotEqualTo:
                        modelQuery = modelQuery.Where(r => !r.CountryName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.StartsWith:
                        modelQuery = modelQuery.Where(r => r.CountryName.ToLower().StartsWith(filterValue));
                        break;
                }
            }

            if (filter.Member == "GenderName")
            {
                switch (filter.Operator)
                {
                    case FilterOperator.Contains:
                        modelQuery = modelQuery.Where(r => r.GenderName.ToLower().Contains(filterValue));
                        break;
                    case FilterOperator.EndsWith:
                        modelQuery = modelQuery.Where(r => r.GenderName.ToLower().EndsWith(filterValue));
                        break;
                    case FilterOperator.IsEqualTo:
                        modelQuery = modelQuery.Where(r => r.GenderName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.IsNotEqualTo:
                        modelQuery = modelQuery.Where(r => !r.GenderName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.StartsWith:
                        modelQuery = modelQuery.Where(r => r.GenderName.ToLower().StartsWith(filterValue));
                        break;
                }
            }

            if (filter.Member == "BirthDate")
            {
                string propertyName = "DateOfBirth";

                switch (filter.Operator)
                {
                    case FilterOperator.Contains:
                        modelQuery = modelQuery.Where(r => r.ConvertDateTimeToString(propertyName).Contains(filterValue));
                        break;
                    case FilterOperator.EndsWith:
                        modelQuery = modelQuery.Where(r => r.ConvertDateTimeToString(propertyName).EndsWith(filterValue));
                        break;
                    case FilterOperator.IsEqualTo:
                        modelQuery = modelQuery.Where(r => r.ConvertDateTimeToString(propertyName).Equals(filterValue));
                        break;
                    case FilterOperator.IsNotEqualTo:
                        modelQuery = modelQuery.Where(r => !r.ConvertDateTimeToString(propertyName).Equals(filterValue));
                        break;
                    case FilterOperator.StartsWith:
                        modelQuery = modelQuery.Where(r => r.ConvertDateTimeToString(propertyName).StartsWith(filterValue));
                        break;
                }
            }

            if (filter.Member == "Category")
            {
                switch (filter.Operator)
                {
                    case FilterOperator.Contains:
                        modelQuery = modelQuery.Where(r => r.Category.ToLower().Contains(filterValue));
                        break;
                    case FilterOperator.EndsWith:
                        modelQuery = modelQuery.Where(r => r.Category.ToLower().EndsWith(filterValue));
                        break;
                    case FilterOperator.IsEqualTo:
                        modelQuery = modelQuery.Where(r => r.Category.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.IsNotEqualTo:
                        modelQuery = modelQuery.Where(r => !r.Category.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.StartsWith:
                        modelQuery = modelQuery.Where(r => r.Category.ToLower().StartsWith(filterValue));
                        break;
                }
            }

            return modelQuery;
        }
        
        private IQueryable<MaintenanceAthletesAthleteGridViewModel> FilterAthletes(IQueryable<MaintenanceAthletesAthleteGridViewModel> modelQuery, IList<IFilterDescriptor> filters)
        {
            foreach (var ifilter in filters)
            {
                if (ifilter.GetType() == typeof(CompositeFilterDescriptor))
                {
                    modelQuery = FilterAthletes(modelQuery, ((CompositeFilterDescriptor)ifilter).FilterDescriptors);
                }
                else
                {
                    modelQuery = FilterAthletes(modelQuery, ifilter);
                }
            }

            return modelQuery;
        }

        private IQueryable<MaintenanceISTMembersGridViewModel> FilterISTMembers(IQueryable<MaintenanceISTMembersGridViewModel> modelQuery, IFilterDescriptor iFilter)
        {
            var filter = (FilterDescriptor)iFilter;

            var valueObjectString = filter.Value.ToString();
            var filterValue = valueObjectString.ToLower();

            if (filter.Member == "FirstName")
            {
                switch (filter.Operator)
                {
                    case FilterOperator.Contains:
                        modelQuery = modelQuery.Where(r => r.FirstName.ToLower().Contains(filterValue));
                        break;
                    case FilterOperator.EndsWith:
                        modelQuery = modelQuery.Where(r => r.FirstName.ToLower().EndsWith(filterValue));
                        break;
                    case FilterOperator.IsEqualTo:
                        modelQuery = modelQuery.Where(r => r.FirstName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.IsNotEqualTo:
                        modelQuery = modelQuery.Where(r => !r.FirstName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.StartsWith:
                        modelQuery = modelQuery.Where(r => r.FirstName.ToLower().StartsWith(filterValue));
                        break;
                }
            }

            if (filter.Member == "LastName")
            {
                switch (filter.Operator)
                {
                    case FilterOperator.Contains:
                        modelQuery = modelQuery.Where(r => r.LastName.ToLower().Contains(filterValue));
                        break;
                    case FilterOperator.EndsWith:
                        modelQuery = modelQuery.Where(r => r.LastName.ToLower().EndsWith(filterValue));
                        break;
                    case FilterOperator.IsEqualTo:
                        modelQuery = modelQuery.Where(r => r.LastName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.IsNotEqualTo:
                        modelQuery = modelQuery.Where(r => !r.LastName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.StartsWith:
                        modelQuery = modelQuery.Where(r => r.LastName.ToLower().StartsWith(filterValue));
                        break;
                }
            }

            if (filter.Member == "CountryName")
            {
                switch (filter.Operator)
                {
                    case FilterOperator.Contains:
                        modelQuery = modelQuery.Where(r => r.CountryName.ToLower().Contains(filterValue));
                        break;
                    case FilterOperator.EndsWith:
                        modelQuery = modelQuery.Where(r => r.CountryName.ToLower().EndsWith(filterValue));
                        break;
                    case FilterOperator.IsEqualTo:
                        modelQuery = modelQuery.Where(r => r.CountryName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.IsNotEqualTo:
                        modelQuery = modelQuery.Where(r => !r.CountryName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.StartsWith:
                        modelQuery = modelQuery.Where(r => r.CountryName.ToLower().StartsWith(filterValue));
                        break;
                }
            }

            if (filter.Member == "BirthDate")
            {
                string propertyName = "DateOfBirth";

                switch (filter.Operator)
                {
                    case FilterOperator.Contains:
                        modelQuery = modelQuery.Where(r => r.ConvertDateTimeToString(propertyName).Contains(filterValue));
                        break;
                    case FilterOperator.EndsWith:
                        modelQuery = modelQuery.Where(r => r.ConvertDateTimeToString(propertyName).EndsWith(filterValue));
                        break;
                    case FilterOperator.IsEqualTo:
                        modelQuery = modelQuery.Where(r => r.ConvertDateTimeToString(propertyName).Equals(filterValue));
                        break;
                    case FilterOperator.IsNotEqualTo:
                        modelQuery = modelQuery.Where(r => !r.ConvertDateTimeToString(propertyName).Equals(filterValue));
                        break;
                    case FilterOperator.StartsWith:
                        modelQuery = modelQuery.Where(r => r.ConvertDateTimeToString(propertyName).StartsWith(filterValue));
                        break;
                }
            }

            if (filter.Member == "PrimaryCscName")
            {
                switch (filter.Operator)
                {
                    case FilterOperator.Contains:
                        modelQuery = modelQuery.Where(r => r.PrimaryCscName.ToLower().Contains(filterValue));
                        break;
                    case FilterOperator.EndsWith:
                        modelQuery = modelQuery.Where(r => r.PrimaryCscName.ToLower().EndsWith(filterValue));
                        break;
                    case FilterOperator.IsEqualTo:
                        modelQuery = modelQuery.Where(r => r.PrimaryCscName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.IsNotEqualTo:
                        modelQuery = modelQuery.Where(r => !r.PrimaryCscName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.StartsWith:
                        modelQuery = modelQuery.Where(r => r.PrimaryCscName.ToLower().StartsWith(filterValue));
                        break;
                }
            }

            if (filter.Member == "ResourceSpecialtyName")
            {
                switch (filter.Operator)
                {
                    case FilterOperator.Contains:
                        modelQuery = modelQuery.Where(r => r.ResourceSpecialtyName.ToLower().Contains(filterValue));
                        break;
                    case FilterOperator.EndsWith:
                        modelQuery = modelQuery.Where(r => r.ResourceSpecialtyName.ToLower().EndsWith(filterValue));
                        break;
                    case FilterOperator.IsEqualTo:
                        modelQuery = modelQuery.Where(r => r.ResourceSpecialtyName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.IsNotEqualTo:
                        modelQuery = modelQuery.Where(r => !r.ResourceSpecialtyName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.StartsWith:
                        modelQuery = modelQuery.Where(r => r.ResourceSpecialtyName.ToLower().StartsWith(filterValue));
                        break;
                }
            }

            return modelQuery;
        }

        private IQueryable<MaintenanceISTMembersGridViewModel> FilterISTMembers(IQueryable<MaintenanceISTMembersGridViewModel> modelQuery, IList<IFilterDescriptor> filters)
        {
            foreach (var ifilter in filters)
            {
                if (ifilter.GetType() == typeof(CompositeFilterDescriptor))
                {
                    modelQuery = FilterISTMembers(modelQuery, ((CompositeFilterDescriptor)ifilter).FilterDescriptors);
                }
                else
                {
                    modelQuery = FilterISTMembers(modelQuery, ifilter);
                }
            }

            return modelQuery;
        }

        private IQueryable<MaintenanceNSOMembersGridViewModel> FilterNSOMembers(IQueryable<MaintenanceNSOMembersGridViewModel> modelQuery, IFilterDescriptor iFilter)
        {
            var filter = (FilterDescriptor)iFilter;

            var valueObjectString = filter.Value.ToString();
            var filterValue = valueObjectString.ToLower();

            if (filter.Member == "FirstName")
            {
                switch (filter.Operator)
                {
                    case FilterOperator.Contains:
                        modelQuery = modelQuery.Where(r => r.FirstName.ToLower().Contains(filterValue));
                        break;
                    case FilterOperator.EndsWith:
                        modelQuery = modelQuery.Where(r => r.FirstName.ToLower().EndsWith(filterValue));
                        break;
                    case FilterOperator.IsEqualTo:
                        modelQuery = modelQuery.Where(r => r.FirstName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.IsNotEqualTo:
                        modelQuery = modelQuery.Where(r => !r.FirstName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.StartsWith:
                        modelQuery = modelQuery.Where(r => r.FirstName.ToLower().StartsWith(filterValue));
                        break;
                }
            }

            if (filter.Member == "LastName")
            {
                switch (filter.Operator)
                {
                    case FilterOperator.Contains:
                        modelQuery = modelQuery.Where(r => r.LastName.ToLower().Contains(filterValue));
                        break;
                    case FilterOperator.EndsWith:
                        modelQuery = modelQuery.Where(r => r.LastName.ToLower().EndsWith(filterValue));
                        break;
                    case FilterOperator.IsEqualTo:
                        modelQuery = modelQuery.Where(r => r.LastName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.IsNotEqualTo:
                        modelQuery = modelQuery.Where(r => !r.LastName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.StartsWith:
                        modelQuery = modelQuery.Where(r => r.LastName.ToLower().StartsWith(filterValue));
                        break;
                }
            }

            if (filter.Member == "CountryName")
            {
                switch (filter.Operator)
                {
                    case FilterOperator.Contains:
                        modelQuery = modelQuery.Where(r => r.CountryName.ToLower().Contains(filterValue));
                        break;
                    case FilterOperator.EndsWith:
                        modelQuery = modelQuery.Where(r => r.CountryName.ToLower().EndsWith(filterValue));
                        break;
                    case FilterOperator.IsEqualTo:
                        modelQuery = modelQuery.Where(r => r.CountryName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.IsNotEqualTo:
                        modelQuery = modelQuery.Where(r => !r.CountryName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.StartsWith:
                        modelQuery = modelQuery.Where(r => r.CountryName.ToLower().StartsWith(filterValue));
                        break;
                }
            }

            if (filter.Member == "BirthDate")
            {
                string propertyName = "DateOfBirth";

                switch (filter.Operator)
                {
                    case FilterOperator.Contains:
                        modelQuery = modelQuery.Where(r => r.ConvertDateTimeToString(propertyName).Contains(filterValue));
                        break;
                    case FilterOperator.EndsWith:
                        modelQuery = modelQuery.Where(r => r.ConvertDateTimeToString(propertyName).EndsWith(filterValue));
                        break;
                    case FilterOperator.IsEqualTo:
                        modelQuery = modelQuery.Where(r => r.ConvertDateTimeToString(propertyName).Equals(filterValue));
                        break;
                    case FilterOperator.IsNotEqualTo:
                        modelQuery = modelQuery.Where(r => !r.ConvertDateTimeToString(propertyName).Equals(filterValue));
                        break;
                    case FilterOperator.StartsWith:
                        modelQuery = modelQuery.Where(r => r.ConvertDateTimeToString(propertyName).StartsWith(filterValue));
                        break;
                }
            }

            if (filter.Member == "GenderName")
            {
                switch (filter.Operator)
                {
                    case FilterOperator.Contains:
                        modelQuery = modelQuery.Where(r => r.GenderName.ToLower().Contains(filterValue));
                        break;
                    case FilterOperator.EndsWith:
                        modelQuery = modelQuery.Where(r => r.GenderName.ToLower().EndsWith(filterValue));
                        break;
                    case FilterOperator.IsEqualTo:
                        modelQuery = modelQuery.Where(r => r.GenderName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.IsNotEqualTo:
                        modelQuery = modelQuery.Where(r => !r.GenderName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.StartsWith:
                        modelQuery = modelQuery.Where(r => r.GenderName.ToLower().StartsWith(filterValue));
                        break;
                }
            }

            if (filter.Member == "ResourceSpecialtyName")
            {
                switch (filter.Operator)
                {
                    case FilterOperator.Contains:
                        modelQuery = modelQuery.Where(r => r.ResourceSpecialtyName.ToLower().Contains(filterValue));
                        break;
                    case FilterOperator.EndsWith:
                        modelQuery = modelQuery.Where(r => r.ResourceSpecialtyName.ToLower().EndsWith(filterValue));
                        break;
                    case FilterOperator.IsEqualTo:
                        modelQuery = modelQuery.Where(r => r.ResourceSpecialtyName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.IsNotEqualTo:
                        modelQuery = modelQuery.Where(r => !r.ResourceSpecialtyName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.StartsWith:
                        modelQuery = modelQuery.Where(r => r.ResourceSpecialtyName.ToLower().StartsWith(filterValue));
                        break;
                }
            } 
            
            return modelQuery;
        }

        private IQueryable<MaintenanceNSOMembersGridViewModel> FilterNSOMembers(IQueryable<MaintenanceNSOMembersGridViewModel> modelQuery, IList<IFilterDescriptor> filters)
        {
            foreach (var ifilter in filters)
            {
                if (ifilter.GetType() == typeof(CompositeFilterDescriptor))
                {
                    modelQuery = FilterNSOMembers(modelQuery, ((CompositeFilterDescriptor)ifilter).FilterDescriptors);
                }
                else
                {
                    modelQuery = FilterNSOMembers(modelQuery, ifilter);
                }
            }

            return modelQuery;
        }

        private IQueryable<MaintenanceAthletesResultsViewModel> FilterAthletesResults(IQueryable<MaintenanceAthletesResultsViewModel> modelQuery, IFilterDescriptor iFilter)
        {
            var filter = (FilterDescriptor)iFilter;

            var valueObjectString = filter.Value.ToString();
            var filterValue = valueObjectString.ToLower();

            if (filter.Member == "CompetitionName")
            {
                switch (filter.Operator)
                {
                    case FilterOperator.Contains:
                        modelQuery = modelQuery.Where(r => r.CompetitionName.ToLower().Contains(filterValue));
                        break;
                    case FilterOperator.EndsWith:
                        modelQuery = modelQuery.Where(r => r.CompetitionName.ToLower().EndsWith(filterValue));
                        break;
                    case FilterOperator.IsEqualTo:
                        modelQuery = modelQuery.Where(r => r.CompetitionName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.IsNotEqualTo:
                        modelQuery = modelQuery.Where(r => !r.CompetitionName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.StartsWith:
                        modelQuery = modelQuery.Where(r => r.CompetitionName.ToLower().StartsWith(filterValue));
                        break;
                }
            }

            if (filter.Member == "CompetitionDate")
            {
                switch (filter.Operator)
                {
                    case FilterOperator.Contains:
                        modelQuery = modelQuery.Where(r => r.CompetitionDate.ToLower().Contains(filterValue));
                        break;
                    case FilterOperator.EndsWith:
                        modelQuery = modelQuery.Where(r => r.CompetitionDate.ToLower().EndsWith(filterValue));
                        break;
                    case FilterOperator.IsEqualTo:
                        modelQuery = modelQuery.Where(r => r.CompetitionDate.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.IsNotEqualTo:
                        modelQuery = modelQuery.Where(r => !r.CompetitionDate.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.StartsWith:
                        modelQuery = modelQuery.Where(r => r.CompetitionDate.ToLower().StartsWith(filterValue));
                        break;
                }
            }

            if (filter.Member == "Location")
            {
                switch (filter.Operator)
                {
                    case FilterOperator.Contains:
                        modelQuery = modelQuery.Where(r => r.Location.ToLower().Contains(filterValue));
                        break;
                    case FilterOperator.EndsWith:
                        modelQuery = modelQuery.Where(r => r.Location.ToLower().EndsWith(filterValue));
                        break;
                    case FilterOperator.IsEqualTo:
                        modelQuery = modelQuery.Where(r => r.Location.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.IsNotEqualTo:
                        modelQuery = modelQuery.Where(r => !r.Location.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.StartsWith:
                        modelQuery = modelQuery.Where(r => r.Location.ToLower().StartsWith(filterValue));
                        break;
                }
            }

            if (filter.Member == "CountryName")
            {
                switch (filter.Operator)
                {
                    case FilterOperator.Contains:
                        modelQuery = modelQuery.Where(r => r.CountryName.ToLower().Contains(filterValue));
                        break;
                    case FilterOperator.EndsWith:
                        modelQuery = modelQuery.Where(r => r.CountryName.ToLower().EndsWith(filterValue));
                        break;
                    case FilterOperator.IsEqualTo:
                        modelQuery = modelQuery.Where(r => r.CountryName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.IsNotEqualTo:
                        modelQuery = modelQuery.Where(r => !r.CountryName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.StartsWith:
                        modelQuery = modelQuery.Where(r => r.CountryName.ToLower().StartsWith(filterValue));
                        break;
                }
            }

            if (filter.Member == "SportName")
            {
                switch (filter.Operator)
                {
                    case FilterOperator.Contains:
                        modelQuery = modelQuery.Where(r => r.SportName.ToLower().Contains(filterValue));
                        break;
                    case FilterOperator.EndsWith:
                        modelQuery = modelQuery.Where(r => r.SportName.ToLower().EndsWith(filterValue));
                        break;
                    case FilterOperator.IsEqualTo:
                        modelQuery = modelQuery.Where(r => r.SportName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.IsNotEqualTo:
                        modelQuery = modelQuery.Where(r => !r.SportName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.StartsWith:
                        modelQuery = modelQuery.Where(r => r.SportName.ToLower().StartsWith(filterValue));
                        break;
                }
            }

            if (filter.Member == "DisciplineName")
            {
                switch (filter.Operator)
                {
                    case FilterOperator.Contains:
                        modelQuery = modelQuery.Where(r => r.DisciplineName.ToLower().Contains(filterValue));
                        break;
                    case FilterOperator.EndsWith:
                        modelQuery = modelQuery.Where(r => r.DisciplineName.ToLower().EndsWith(filterValue));
                        break;
                    case FilterOperator.IsEqualTo:
                        modelQuery = modelQuery.Where(r => r.DisciplineName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.IsNotEqualTo:
                        modelQuery = modelQuery.Where(r => !r.DisciplineName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.StartsWith:
                        modelQuery = modelQuery.Where(r => r.DisciplineName.ToLower().StartsWith(filterValue));
                        break;
                }
            }

            if (filter.Member == "EventName")
            {
                switch (filter.Operator)
                {
                    case FilterOperator.Contains:
                        modelQuery = modelQuery.Where(r => r.EventName.ToLower().Contains(filterValue));
                        break;
                    case FilterOperator.EndsWith:
                        modelQuery = modelQuery.Where(r => r.EventName.ToLower().EndsWith(filterValue));
                        break;
                    case FilterOperator.IsEqualTo:
                        modelQuery = modelQuery.Where(r => r.EventName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.IsNotEqualTo:
                        modelQuery = modelQuery.Where(r => !r.EventName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.StartsWith:
                        modelQuery = modelQuery.Where(r => r.EventName.ToLower().StartsWith(filterValue));
                        break;
                }
            }

            if (filter.Member == "EventDate")
            {
                switch (filter.Operator)
                {
                    case FilterOperator.Contains:
                        modelQuery = modelQuery.Where(r => r.EventDate.ToLower().Contains(filterValue));
                        break;
                    case FilterOperator.EndsWith:
                        modelQuery = modelQuery.Where(r => r.EventDate.ToLower().EndsWith(filterValue));
                        break;
                    case FilterOperator.IsEqualTo:
                        modelQuery = modelQuery.Where(r => r.EventDate.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.IsNotEqualTo:
                        modelQuery = modelQuery.Where(r => !r.EventDate.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.StartsWith:
                        modelQuery = modelQuery.Where(r => r.EventDate.ToLower().StartsWith(filterValue));
                        break;
                }
            }

            if (filter.Member == "Position")
            {
                switch (filter.Operator)
                {
                    case FilterOperator.IsLessThan:
                        modelQuery = modelQuery.Where(r => r.Position < Convert.ToInt32(filterValue));
                        break;
                    case FilterOperator.IsLessThanOrEqualTo:
                        modelQuery = modelQuery.Where(r => r.Position <= Convert.ToInt32(filterValue));
                        break;
                    case FilterOperator.IsEqualTo:
                        modelQuery = modelQuery.Where(r => r.Position == Convert.ToInt32(filterValue));
                        break;
                    case FilterOperator.IsNotEqualTo:
                        modelQuery = modelQuery.Where(r => r.Position != Convert.ToInt32(filterValue));
                        break;
                    case FilterOperator.IsGreaterThanOrEqualTo:
                        modelQuery = modelQuery.Where(r => r.Position >= Convert.ToInt32(filterValue));
                        break;
                    case FilterOperator.IsGreaterThan:
                        modelQuery = modelQuery.Where(r => r.Position > Convert.ToInt32(filterValue));
                        break;
                }
            }

            if (filter.Member == "PerfTime")
            {
                switch (filter.Operator)
                {
                    case FilterOperator.Contains:
                        modelQuery = modelQuery.Where(r => r.PerfTime.ToLower().Contains(filterValue));
                        break;
                    case FilterOperator.EndsWith:
                        modelQuery = modelQuery.Where(r => r.PerfTime.ToLower().EndsWith(filterValue));
                        break;
                    case FilterOperator.IsEqualTo:
                        modelQuery = modelQuery.Where(r => r.PerfTime.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.IsNotEqualTo:
                        modelQuery = modelQuery.Where(r => !r.PerfTime.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.StartsWith:
                        modelQuery = modelQuery.Where(r => r.PerfTime.ToLower().StartsWith(filterValue));
                        break;
                }
            }

            if (filter.Member == "PerfValue")
            {
                switch (filter.Operator)
                {
                    case FilterOperator.Contains:
                        modelQuery = modelQuery.Where(r => r.PerfValue.ToLower().Contains(filterValue));
                        break;
                    case FilterOperator.EndsWith:
                        modelQuery = modelQuery.Where(r => r.PerfValue.ToLower().EndsWith(filterValue));
                        break;
                    case FilterOperator.IsEqualTo:
                        modelQuery = modelQuery.Where(r => r.PerfValue.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.IsNotEqualTo:
                        modelQuery = modelQuery.Where(r => !r.PerfValue.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.StartsWith:
                        modelQuery = modelQuery.Where(r => r.PerfValue.ToLower().StartsWith(filterValue));
                        break;
                }
            }

            return modelQuery;
        }

        private IQueryable<MaintenanceAthletesResultsViewModel> FilterAthletesResults(IQueryable<MaintenanceAthletesResultsViewModel> modelQuery, IList<IFilterDescriptor> filters)
        {
            foreach (var ifilter in filters)
            {
                if (ifilter.GetType() == typeof(CompositeFilterDescriptor))
                {
                    modelQuery = FilterAthletesResults(modelQuery, ((CompositeFilterDescriptor)ifilter).FilterDescriptors);
                }
                else
                {
                    modelQuery = FilterAthletesResults(modelQuery, ifilter);
                }
            }

            return modelQuery;
        }

        private IQueryable<MaintenanceAthletesSportsInfoViewModel> FilterAthletesSportsInfo(IQueryable<MaintenanceAthletesSportsInfoViewModel> modelQuery, IFilterDescriptor iFilter)
        {
            var filter = (FilterDescriptor)iFilter;

            var valueObjectString = filter.Value.ToString();
            var filterValue = valueObjectString.ToLower();

            if (filter.Member == "SportName")
            {
                switch (filter.Operator)
                {
                    case FilterOperator.Contains:
                        modelQuery = modelQuery.Where(r => r.SportName.ToLower().Contains(filterValue));
                        break;
                    case FilterOperator.EndsWith:
                        modelQuery = modelQuery.Where(r => r.SportName.ToLower().EndsWith(filterValue));
                        break;
                    case FilterOperator.IsEqualTo:
                        modelQuery = modelQuery.Where(r => r.SportName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.IsNotEqualTo:
                        modelQuery = modelQuery.Where(r => !r.SportName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.StartsWith:
                        modelQuery = modelQuery.Where(r => r.SportName.ToLower().StartsWith(filterValue));
                        break;
                }
            }

            if (filter.Member == "DisciplineName")
            {
                switch (filter.Operator)
                {
                    case FilterOperator.Contains:
                        modelQuery = modelQuery.Where(r => r.DisciplineName.ToLower().Contains(filterValue));
                        break;
                    case FilterOperator.EndsWith:
                        modelQuery = modelQuery.Where(r => r.DisciplineName.ToLower().EndsWith(filterValue));
                        break;
                    case FilterOperator.IsEqualTo:
                        modelQuery = modelQuery.Where(r => r.DisciplineName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.IsNotEqualTo:
                        modelQuery = modelQuery.Where(r => !r.DisciplineName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.StartsWith:
                        modelQuery = modelQuery.Where(r => r.DisciplineName.ToLower().StartsWith(filterValue));
                        break;
                }
            }

            if (filter.Member == "EventName")
            {
                switch (filter.Operator)
                {
                    case FilterOperator.Contains:
                        modelQuery = modelQuery.Where(r => r.EventName.ToLower().Contains(filterValue));
                        break;
                    case FilterOperator.EndsWith:
                        modelQuery = modelQuery.Where(r => r.EventName.ToLower().EndsWith(filterValue));
                        break;
                    case FilterOperator.IsEqualTo:
                        modelQuery = modelQuery.Where(r => r.EventName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.IsNotEqualTo:
                        modelQuery = modelQuery.Where(r => !r.EventName.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.StartsWith:
                        modelQuery = modelQuery.Where(r => r.EventName.ToLower().StartsWith(filterValue));
                        break;
                }
            }

            if (filter.Member == "YearId")
            {
                switch (filter.Operator)
                {
                    case FilterOperator.Contains:
                        modelQuery = modelQuery.Where(r => r.YearId.ToLower().Contains(filterValue));
                        break;
                    case FilterOperator.EndsWith:
                        modelQuery = modelQuery.Where(r => r.YearId.ToLower().EndsWith(filterValue));
                        break;
                    case FilterOperator.IsEqualTo:
                        modelQuery = modelQuery.Where(r => r.YearId.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.IsNotEqualTo:
                        modelQuery = modelQuery.Where(r => !r.YearId.ToLower().Equals(filterValue));
                        break;
                    case FilterOperator.StartsWith:
                        modelQuery = modelQuery.Where(r => r.YearId.ToLower().StartsWith(filterValue));
                        break;
                }
            }

            return modelQuery;
        }

        private IQueryable<MaintenanceAthletesSportsInfoViewModel> FilterAthletesSportsInfo(IQueryable<MaintenanceAthletesSportsInfoViewModel> modelQuery, IList<IFilterDescriptor> filters)
        {
            foreach (var ifilter in filters)
            {
                if (ifilter.GetType() == typeof(CompositeFilterDescriptor))
                {
                    modelQuery = FilterAthletesSportsInfo(modelQuery, ((CompositeFilterDescriptor)ifilter).FilterDescriptors);
                }
                else
                {
                    modelQuery = FilterAthletesSportsInfo(modelQuery, ifilter);
                }
            }

            return modelQuery;
        }

        private IQueryable<T> FilterMembers<T>(IQueryable<T> modelQuery, IList<IFilterDescriptor> filters)
        {
            foreach (var ifilter in filters)
            {
                if (ifilter.GetType() == typeof(CompositeFilterDescriptor))
                {
                    modelQuery = FilterMembers<T>(modelQuery, ((CompositeFilterDescriptor)ifilter).FilterDescriptors);
                }
                else
                {
                    modelQuery = FilterProperties<T>(modelQuery, ifilter);
                }
            }

            return modelQuery;
        }

        private IQueryable<T> FilterProperties<T>(IQueryable<T> modelQuery, IFilterDescriptor iFilter)
        {
            var filter = (FilterDescriptor)iFilter;

            var valueObjectString = filter.Value.ToString();
            var filterValue = valueObjectString.ToLower();

            // Get the property type of the filter
            Type propertyType = typeof(T).GetProperty(filter.Member).PropertyType;

            // Apply filter setting for each filter type
            if (propertyType == typeof(int))
            {
                switch (filter.Operator)
                {
                    case FilterOperator.IsLessThan:
                        modelQuery = modelQuery.Where(r => (int)r.GetReflectedPropertyValue(filter.Member) < Convert.ToInt32(filterValue));
                        break;
                    case FilterOperator.IsLessThanOrEqualTo:
                        modelQuery = modelQuery.Where(r => (int)r.GetReflectedPropertyValue(filter.Member) <= Convert.ToInt32(filterValue));
                        break;
                    case FilterOperator.IsEqualTo:
                        modelQuery = modelQuery.Where(r => (int)r.GetReflectedPropertyValue(filter.Member) == Convert.ToInt32(filterValue));
                        break;
                    case FilterOperator.IsNotEqualTo:
                        modelQuery = modelQuery.Where(r => (int)r.GetReflectedPropertyValue(filter.Member) != Convert.ToInt32(filterValue));
                        break;
                    case FilterOperator.IsGreaterThanOrEqualTo:
                        modelQuery = modelQuery.Where(r => (int)r.GetReflectedPropertyValue(filter.Member) >= Convert.ToInt32(filterValue));
                        break;
                    case FilterOperator.IsGreaterThan:
                        modelQuery = modelQuery.Where(r => (int)r.GetReflectedPropertyValue(filter.Member) > Convert.ToInt32(filterValue));
                        break;
                }
            }
            else
            {
                switch (filter.Operator)
                {
                    case FilterOperator.Contains:
                        modelQuery = modelQuery.Where(r => ((string)r.GetReflectedPropertyValue(filter.Member)).Contains(filterValue));
                        break;
                    case FilterOperator.EndsWith:
                        modelQuery = modelQuery.Where(r => ((string)r.GetReflectedPropertyValue(filter.Member)).EndsWith(filterValue));
                        break;
                    case FilterOperator.IsEqualTo:
                        modelQuery = modelQuery.Where(r => ((string)r.GetReflectedPropertyValue(filter.Member)).Equals(filterValue));
                        break;
                    case FilterOperator.IsNotEqualTo:
                        modelQuery = modelQuery.Where(r => !((string)r.GetReflectedPropertyValue(filter.Member)).Equals(filterValue));
                        break;
                    case FilterOperator.StartsWith:
                        modelQuery = modelQuery.Where(r => ((string)r.GetReflectedPropertyValue(filter.Member)).StartsWith(filterValue));
                        break;
                }
            }

            return modelQuery;
        }

        private IQueryable<T> SortMembers<T>(IQueryable<T> modelQuery, IList<SortDescriptor> sortDescriptors)
        {
            // Apply sorting from the UI if there is any
            if (sortDescriptors.Any())
            {
                foreach (SortDescriptor sortDescriptor in sortDescriptors)
                {
                    if (sortDescriptor.SortDirection == ListSortDirection.Ascending)
                    {
                        // Sort the field that is specified by sortDescriptor.Member
                        modelQuery = modelQuery.OrderBy(r => r.GetReflectedPropertyValue(sortDescriptor.Member));
                    }
                    else
                    {
                        // Sort the field that is specified by sortDescriptor.Member
                        modelQuery = modelQuery.OrderByDescending(r => r.GetReflectedPropertyValue(sortDescriptor.Member));
                    }
                }
            }

            return modelQuery;
        }

            //else
            //{
            //    // Get default sorting
            //    List<SortAttribute> defaultSortDescriptors = GetDefaultSortDescriptors<T>();

            //    // Sort the fields that are in defaultSortDescriptors
            //    foreach (SortAttribute sort in defaultSortDescriptors)
            //    {
            //        if (sort.SortOrder == ListSortDirection.Ascending)
            //        {
            //            if (sort.SortPriority == 1)
            //            {
            //                modelQuery = modelQuery.OrderBy(x => x.GetReflectedPropertyValue(sort.PropertyName));
            //            }
            //            else
            //            {
            //                modelQuery = ((IOrderedQueryable<T>)modelQuery).ThenBy(x => x.GetReflectedPropertyValue(sort.PropertyName));
            //            }
            //        }
            //        else
            //        {
            //            if (sort.SortPriority == 1)
            //            {
            //                modelQuery = modelQuery.OrderByDescending(x => x.GetReflectedPropertyValue(sort.PropertyName));
            //            }
            //            else
            //            {
            //                modelQuery = ((IOrderedQueryable<T>)modelQuery).ThenByDescending(x => x.GetReflectedPropertyValue(sort.PropertyName));
            //            }
            //        }
            //    }
            //}

        //private List<SortAttribute> GetDefaultSortDescriptors<T>()
        //{
        //    List<SortAttribute> sortDescriptors = new List<SortAttribute>();

        //    // Get properties info in the class
        //    MemberInfo[] members = typeof(T).GetMembers();
        //    foreach (MemberInfo member in members)
        //    {
        //        // Get the attributes for each property
        //        object[] attributes = member.GetCustomAttributes(true);

        //        if (attributes.Length == 0)
        //            continue;

        //        foreach (object attribute in attributes)
        //        {
        //            // Find SortAttribute
        //            SortAttribute sort = attribute as SortAttribute;
        //            if (sort != null)
        //            {
        //                // Copy attribute info for sort to a list
        //                sortDescriptors.Add(new SortAttribute
        //                {
        //                    PropertyName = member.Name,                 // Get the property name
        //                    SortOrder = sort.SortOrder,                 // Get the sort order
        //                    SortPriority = sort.SortPriority            // Get the Sort priority
        //                });
        //            }
        //        }
        //    }

        //    return sortDescriptors.OrderBy(s => s.SortPriority).ToList();
        //}

        private string GetFilterString(string filterString, IList<IFilterDescriptor> filters)
        {
            foreach (var ifilter in filters)
            {
                if (ifilter.GetType() == typeof(CompositeFilterDescriptor))
                {
                    filterString = GetFilterString(filterString, ((CompositeFilterDescriptor)ifilter).FilterDescriptors);
                }
                else
                {
                    filterString = GetFilterString(filterString, ifilter);
                }
            }

            return filterString;
        }

        private string GetFilterString(string filterString, IFilterDescriptor iFilter)
        {
            var filter = (FilterDescriptor)iFilter;

            var filterValue = filter.Value.ToString().ToLower();

            var type = filter.Member.GetType();

            filterString += "Field:" + filter.Member + ","
                          + "Operator:" + filter.Operator.ToString() + ","
                          + "Type:" + type.ToString().Replace("System.", "") + ","
                          + "Content:" + filterValue + "; ";

            return filterString;
        }

        #endregion Filter

    }
}
