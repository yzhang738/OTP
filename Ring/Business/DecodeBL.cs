using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Runtime.Caching;
using OTP.Ring.Common;
using OTP.Ring.Models;
using OTP.Ring.Models.ViewModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.HSSF.Util;
using NPOI.HSSF.Record;

namespace OTP.Ring.Business
{
    public class DecodeBL : IDecodeBL
    {
        private CacheItemPolicy oneHourSlidingPolicy = new CacheItemPolicy() { SlidingExpiration = new TimeSpan(1, 0, 0) };
        private bool _frenchLanguage = false;
        private Data.Models.RingEntities _repository;

        public DecodeBL(string connectionString)
        {
            _repository = new Data.Models.RingEntities(connectionString);

            //NOTE: 1. Language should be ideally handled in presentation layer.
            //      2. Deciding language code in BL constructor determines that all BL instances can only be PerCall instances.
            //      3. Caching needs to be language specific
            _frenchLanguage = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToUpper() == Constant.LanguageCode.French;
        }

        #region PUBLIC

        public IEnumerable<Decode> GetDecodesByCategory(string categoryId)
        {
            string cacheKey = string.Format(Constant.Cache.KeyTemplate, Constant.Cache.Decode, _frenchLanguage, categoryId);
            ObjectCache cache = MemoryCache.Default;

            if (cache.Contains(cacheKey))
            {
                return (IEnumerable<Decode>)cache.Get(cacheKey);
            }
            else
            {
                var decodes = (from d in this._repository.Decodes
                        where d.DecodeCategoryID == categoryId
                        orderby d.SortOrder
                        select new Decode
                        {
                            Id = d.DecodeID,
                            CategoryId = d.DecodeCategoryID,
                            Name = _frenchLanguage ? d.DecodeID_FR : d.DecodeID_EN,
                            Comment = d.Comments,
                            SortOrder = d.SortOrder
                        }).ToList();

                cache.Add(cacheKey, decodes, oneHourSlidingPolicy);

                return decodes;
            }
        }

        public IEnumerable<Decode> GetOrganizationTypes()
        {
            string cacheKey = string.Format(Constant.Cache.KeyTemplate, Constant.Cache.DecodeOrganizationType, _frenchLanguage, null);
            ObjectCache cache = MemoryCache.Default;

            if (cache.Contains(cacheKey))
            {
                return (IEnumerable<Decode>)cache.Get(cacheKey);
            }
            else
            {
                var decodes = (from o in this._repository.OrganizationTypes
                        select new Decode
                        {
                            Id = o.OrganizationTypeID,
                            Name = _frenchLanguage ? o.Name_FR : o.Name_EN,
                            Comment = o.Comments,
                            CategoryId = "OrganizationType",
                            SortOrder = 0
                        }).ToList();

                cache.Add(cacheKey, decodes, oneHourSlidingPolicy);

                return decodes;
            }
        }

        public IEnumerable<Decode> GetActionItemTypes()
        {
            string cacheKey = string.Format(Constant.Cache.KeyTemplate, Constant.Cache.DecodeActionItemType, _frenchLanguage, null);
            ObjectCache cache = MemoryCache.Default;

            if (cache.Contains(cacheKey))
            {
                return (IEnumerable<Decode>)cache.Get(cacheKey);
            }
            else
            {
                var decodes = (from a in this._repository.ActionItemTypes
                        select new Decode
                        {
                            Id = a.ActionItemTypeID,
                            Name = _frenchLanguage ? a.CategroyName_FR : a.CategoryName_EN,
                            Comment = a.Comments,
                            CategoryId = a.SourceName_EN,
                            SortOrder = 0
                        }).ToList();

                cache.Add(cacheKey, decodes, oneHourSlidingPolicy);

                return decodes;
            }
        }

        public IEnumerable<Decode> GetBenchmarkTypes()
        {
            string cacheKey = string.Format(Constant.Cache.KeyTemplate, Constant.Cache.DecodeBenchmarkType, _frenchLanguage, null);
            ObjectCache cache = MemoryCache.Default;

            if (cache.Contains(cacheKey))
            {
                return (IEnumerable<Decode>)cache.Get(cacheKey);
            }
            else
            {
                var decodes = (from a in this._repository.BenchmarkTypes
                               select new Decode
                               {
                                   IntId = a.BenchmarkTypeID,
                                   Name = _frenchLanguage ? a.Name_FR : a.Name_EN,
                                   Comment = a.Comments,
                                   CategoryId = Constant.Cache.DecodeBenchmarkType,
                                   SortOrder = 0
                               }).ToList();

                foreach (var decode in decodes)
                {
                    decode.Id = decode.IntId.ToString();    
                }

                cache.Add(cacheKey, decodes, oneHourSlidingPolicy);

                return decodes;
            }
        }

        public IEnumerable<Decode> GetCompetitionTypes()
        {
            string cacheKey = string.Format(Constant.Cache.KeyTemplate, Constant.Cache.DecodeCompetitionType , _frenchLanguage, null);
            ObjectCache cache = MemoryCache.Default;

            if (cache.Contains(cacheKey))
            {
                return (IEnumerable<Decode>)cache.Get(cacheKey);
            }
            else
            {
                var decodes = (from a in this._repository.CompetitionTypes
                               select new Decode
                               {
                                   Id = a.CompetitionTypeID,
                                   Name = _frenchLanguage ? a.Name_FR : a.Name_EN,
                                   Comment = a.Comments,
                                   CategoryId = Constant.Cache.DecodeCompetitionType,
                                   SortOrder = 0
                               }).ToList();

                cache.Add(cacheKey, decodes, oneHourSlidingPolicy);

                return decodes;
            }
        }

        public IEnumerable<Decode> GetTeamTypes()
        {
            string cacheKey = string.Format(Constant.Cache.KeyTemplate, Constant.Cache.DecodeTeamType, _frenchLanguage, null);
            ObjectCache cache = MemoryCache.Default;

            if (cache.Contains(cacheKey))
            {
                return (IEnumerable<Decode>)cache.Get(cacheKey);
            }
            else
            {
                var decodes = (from a in this._repository.TeamTypes
                               select new Decode
                               {
                                   Id = a.TeamTypeID ,
                                   Name = _frenchLanguage ? a.Name_FR : a.Name_EN,
                                   Comment = a.Comments,
                                   CategoryId = Constant.Cache.DecodeTeamType,
                                   SortOrder = 0
                               }).ToList();

                cache.Add(cacheKey, decodes, oneHourSlidingPolicy);

                return decodes;
            }
        }

        public IEnumerable<Decode> GetSpecialResultPositionTypes()
        {
            string cacheKey = string.Format(Constant.Cache.KeyTemplate, Constant.Cache.DecodePositionType, _frenchLanguage, null);
            ObjectCache cache = MemoryCache.Default;
            
            if (cache.Contains(cacheKey))
            {
                return (IEnumerable<Decode>)cache.Get(cacheKey);
            }
            else
            {
                var decodes = (from a in this._repository.ResultPositionMappings
                               select new Decode
                               {
                                   IntId = a.ResultPositionDecode,
                                   Id = a.ResultPositionValue,
                                   Name = a.ResultPositionValue,
                                   Comment = a.Comments,
                                   CategoryId = Constant.Cache.DecodePositionType,
                                   SortOrder = a.ResultPositionMappingID
                               }).ToList();

                cache.Add(cacheKey, decodes, oneHourSlidingPolicy);

                return decodes;
            }

        }

        public List<ListItem> GetAllYearsList()
        {
            string cacheKey = string.Format(Constant.Cache.KeyTemplate, Constant.Cache.DecodeYear, null, null);
            ObjectCache cache = MemoryCache.Default;

            if (cache.Contains(cacheKey))
            {
                return (List<ListItem>)cache.Get(cacheKey);
            }
            else
            {
                List<ListItem> years = null;

                years = (from y in this._repository.Years
                         where y.Active == null || (bool)y.Active
                         orderby y.YearID descending
                            select new ListItem
                            {
                                Id = y.YearID,
                                Description = y.YearID
                            }).ToList();

                cache.Add(cacheKey, years, oneHourSlidingPolicy);

                return years;
            }
        }

        public List<ListItem> GetAllCountriesList()
        {
            string cacheKey = string.Format(Constant.Cache.KeyTemplate, Constant.Cache.DecodeCountry, _frenchLanguage, null);
            ObjectCache cache = MemoryCache.Default;

            if (cache.Contains(cacheKey))
            {
                return (List<ListItem>)cache.Get(cacheKey);
            }
            else
            {
                List<ListItem> countries = null;

                countries = (from c in this._repository.Countries
                             select new ListItem
                             {
                                 Id = c.CountryID,
                                 Description = _frenchLanguage ? c.Name_FR : c.Name_EN
                             }).OrderBy(c => c.Description).ToList();

                cache.Add(cacheKey, countries, oneHourSlidingPolicy);

                return countries;
            }
        }

        public MaintenanceAthletesResultsViewModel GetMostRecentOlympicGameInfo(string id)
        {
            int resourceId = Convert.ToInt32(id);

            // Get the Olympic game type based on athlete's sport
            string competitionTypeID = (from rs in this._repository.ResourceSports
                                          where rs.ResourceID == resourceId
                                          select rs.Sport.ParentSport.ParentSport.ParentSportID.Equals("S_O") ? "OSG" :
                                                  rs.Sport.ParentSport.ParentSport.ParentSportID.Equals("W_O") ? "OWG" :
                                                  rs.Sport.ParentSport.ParentSport.ParentSportID.Equals("S_P") ? "PSG" :
                                                  rs.Sport.ParentSport.ParentSport.ParentSportID.Equals("W_P") ? "PWG" :
                                                  rs.Sport.ParentSport.ParentSport.ParentSportID.Equals("ALL") ? "OSG" : ""
                                        ).FirstOrDefault();

            MaintenanceAthletesResultsViewModel olympicInfo = new MaintenanceAthletesResultsViewModel();

            // Get the year of most recent Olympic/Paralympic game
            string summerGameYear = (((DateTime.Now.Year - 1) / 4) * 4).ToString();
            string winterGameYear = (((DateTime.Now.Year - 3) / 4) * 4 + 2).ToString();

            // Get the competition type from ResourceSport table first. If nothing there, then get from Result table
            if (!string.IsNullOrEmpty(competitionTypeID))
            {
                // Get the Olympic game info
                var competitionDetail = (from c in this._repository.Competitions
                                         where (c.YearID.Contains(summerGameYear) || c.YearID.Contains(winterGameYear)) && c.CompetitionTypeID == competitionTypeID
                                         select new
                                         {
                                             CompetitionTypeID = c.CompetitionTypeID,
                                             CountryID = c.CountryID,
                                             Location = c.Location,
                                             Date = c.CompetitionDate
                                         }).FirstOrDefault();

                olympicInfo.CompetitionName = competitionDetail.CompetitionTypeID;
                olympicInfo.CountryID = competitionDetail.CountryID;
                olympicInfo.Location = competitionDetail.Location;
                olympicInfo.CompetitionDate = String.Format("{0:yyyy/MM/dd}", competitionDetail.Date);
            }
            else
            {
                // Check the Result table for sport Ids.
                var resultSports = (from r in this._repository.Results
                                    join rr in this._repository.ResultResources
                                    on r.ResultID equals rr.ResultID
                                    where rr.ResourceID == resourceId
                                    select r).FirstOrDefault();

                // If no result for the resource, then assume summer game.
                if (resultSports == null)
                {
                    // Get the Olympic game info
                    var competitionDetail = (from c in this._repository.Competitions
                                             where c.YearID.Contains(summerGameYear) && c.CompetitionTypeID == "OSG"
                                             select new
                                             {
                                                 CompetitionTypeID = c.CompetitionTypeID,
                                                 CountryID = c.CountryID,
                                                 Location = c.Location,
                                                 Date = c.CompetitionDate
                                             }).FirstOrDefault();

                    olympicInfo.CompetitionName = competitionDetail.CompetitionTypeID;
                    olympicInfo.CountryID = competitionDetail.CountryID;
                    olympicInfo.Location = competitionDetail.Location;
                    olympicInfo.CompetitionDate = String.Format("{0:yyyy/MM/dd}", competitionDetail.Date);
                }
                else
                {
                    // Get competition type based on Result table.
                    competitionTypeID = (from s in this._repository.Sports
                                        join r in this._repository.Results
                                        on s.SportID equals r.SportID
                                        join rr in this._repository.ResultResources
                                        on r.ResultID equals rr.ResultID
                                        where rr.ResourceID == resourceId
                                        select s.ParentSport.ParentSport.ParentSportID.Equals("S_O") ? "OSG" :
                                                s.ParentSport.ParentSport.ParentSportID.Equals("W_O") ? "OWG" :
                                                s.ParentSport.ParentSport.ParentSportID.Equals("S_P") ? "PSG" :
                                                s.ParentSport.ParentSport.ParentSportID.Equals("W_P") ? "PWG" :
                                                s.ParentSport.ParentSport.ParentSportID.Equals("ALL") ? "OSG" : ""
                                       ).FirstOrDefault();

                    // Get the Olympic game info
                    var competitionDetail = (from c in this._repository.Competitions
                                             where (c.YearID.Contains(summerGameYear) || c.YearID.Contains(winterGameYear)) && c.CompetitionTypeID == competitionTypeID
                                             select new
                                             {
                                                 CompetitionTypeID = c.CompetitionTypeID,
                                                 CountryID = c.CountryID,
                                                 Location = c.Location,
                                                 Date = c.CompetitionDate
                                             }).FirstOrDefault();

                    olympicInfo.CompetitionName = competitionDetail.CompetitionTypeID;
                    olympicInfo.CountryID = competitionDetail.CountryID;
                    olympicInfo.Location = competitionDetail.Location;
                    olympicInfo.CompetitionDate = String.Format("{0:yyyy/MM/dd}", competitionDetail.Date);
                }
            }

            return olympicInfo;
        }

        public List<ListItem> GetAllCompetitionTypeList()
        {
            string cacheKey = string.Format(Constant.Cache.KeyTemplate, Constant.Cache.DecodeCompetitionName, _frenchLanguage, null);
            ObjectCache cache = MemoryCache.Default;

            if (cache.Contains(cacheKey))
            {
                return (List<ListItem>)cache.Get(cacheKey);
            }
            else
            {
                List<ListItem> types = null;

                types = (from c in this._repository.CompetitionTypes
                         select new ListItem
                         {
                             Id = c.CompetitionTypeID,
                             Description = _frenchLanguage ? c.Name_FR : c.Name_EN
                         }).OrderBy(c => c.Description).ToList();

                cache.Add(cacheKey, types, oneHourSlidingPolicy);

                return types;
            }
        }

        public List<ListItem> GetAllCompetitionTypeList(string id)
        {
            int resourceId = Convert.ToInt32(id);
            var resourceSport = (from rs in this._repository.ResourceSports
                                  where rs.ResourceID == resourceId
                                  select rs).FirstOrDefault();

            List<ListItem> types = null;

            // Get the competition type from ResourceSport table first. If nothing there, then get from Result table
            if (resourceSport != null)
            {
                // Get the sports Ids from ResourceSport
                var sportIds = (from s in this._repository.Sports
                                join rs in this._repository.ResourceSports
                                on s.SportID equals rs.SportID
                                where rs.ResourceID == resourceId
                                select s.ParentSport.ParentSport.ParentSportID
                                ).Distinct().ToList();

                // Add the sport Id from Result table
                sportIds = sportIds.Union(GetSportIdsFromResults(resourceId)).ToList();

                // Get the competition type list
                types = GetCompetitionTypes(GetCompetitionTypesFromSportID(sportIds));

                // Get distinct elements and order the list
                types = types.GroupBy(c => c.Description).Select(g => g.First()).OrderBy(c => c.Description).ToList();
            }
            else
            {
                // Check the Result table for sport Ids.
                var resultSports = (from r in this._repository.Results
                                    join rr in this._repository.ResultResources
                                    on r.ResultID equals rr.ResultID
                                    where rr.ResourceID == resourceId
                                    select r).FirstOrDefault();

                // If no result for the Result table, then get all competition types.
                if (resultSports == null)
                {
                    types = GetAllCompetitionTypeList();
                }
                else
                {
                    // Get competition type list based on those from Result table and 3 defaults.
                    types = GetCompetitionTypesFromResults(resourceId);
                }
            }

            return types;
        }

        public string GetCurrentYearID()
        {
            // If year Id is separated by June.
            if (DateTime.Now.Month <= 6)
            {
                return DateTime.Now.AddYears(-1).Year.ToString() + "/" + DateTime.Now.AddYears(0).Year.ToString();
            }
            else
            {
                return DateTime.Now.AddYears(0).Year.ToString() + "/" + DateTime.Now.AddYears(1).Year.ToString();
            }
        }

        //public string GetSportIdFromSportInfos(string id)
        //{
        //    int resourceId = Convert.ToInt32(id);

        //    // Get sport Ids from ResourceSport table.
        //    return (from s in this._repository.Sports
        //            join r in this._repository.ResourceSports
        //            on s.SportID equals r.SportID
        //            where r.ResourceID == resourceId && r.SportID != "ALL"
        //            select s.ParentSport.ParentSportID
        //            ).FirstOrDefault();
        //}

        public string GetSportIdFromSportInfos(string id)
        {
            int resourceId = Convert.ToInt32(id);

            // Get sport Ids from ResourceSport table.
            var sports = (from s in this._repository.Sports
                          join r in this._repository.ResourceSports
                          on s.SportID equals r.SportID
                          where r.ResourceID == resourceId && r.SportID != "ALL"
                          select new
                          {
                              YearID  = r.YearID,
                              SportID = s.ParentSport.ParentSportID
                          }).OrderByDescending(x => x.YearID).ToList();

            return (sports.Count == 0) ? null : sports[0].SportID;
        }

        public string GetDisciplineIdFromSportInfos(string id)
        {
            int resourceId = Convert.ToInt32(id);

            // Get discipline Ids from ResourceSport table.
            var sports = (from s in this._repository.Sports
                          join r in this._repository.ResourceSports
                          on s.SportID equals r.SportID
                          where r.ResourceID == resourceId && r.SportID != "ALL"
                          select new
                          {
                              YearID = r.YearID,
                              SportID = s.ParentSport.SportID
                          }).OrderByDescending(x => x.YearID).ToList();

            return (sports.Count == 0) ? null : sports[0].SportID;
        }

        public string GetEventIdFromSportInfos(string id)
        {
            int resourceId = Convert.ToInt32(id);

            // Get event Ids from ResourceSport table.
            var sports = (from s in this._repository.Sports
                          join r in this._repository.ResourceSports
                          on s.SportID equals r.SportID
                          where r.ResourceID == resourceId && r.SportID != "ALL"
                          select new
                          {
                              YearID = r.YearID,
                              SportID = s.SportID
                          }).OrderByDescending(x => x.YearID).ToList();

            return (sports.Count == 0) ? null : sports[0].SportID;
        }

        public string GetCategoryFromSportInfos(string id)
        {
            int resourceId = Convert.ToInt32(id);

            // Get sport Ids from ResourceSport table.
            var category = (from r in this._repository.ResourceSports
                            where r.ResourceID == resourceId && r.SportID != "ALL"
                            select new
                            {
                                YearID = r.YearID,
                                ResourceClassID = r.ResourceClassID
                            }
                            ).OrderByDescending(x => x.YearID).ToList();

            return (category.Count == 0) ? null : category[0].ResourceClassID;
        }

        public string GetCoachIdFromResources(string id)
        {
            int? resourceId = string.IsNullOrEmpty(id) ? (int?)null : Convert.ToInt32(id); 

            // Get sport Ids from ResourceSport table.
            var coaches = (from r in this._repository.ResourceSports
                           where r.ResourceID == resourceId && r.SportID == "ALL"
                           select new
                           {
                               YearID = r.YearID,
                               CoachResourceID = r.CoachResourceID
                           }
                            ).FirstOrDefault();

            return (coaches == null) ? null : coaches.CoachResourceID.ToString();
        }

        public string GetCoachIdFromSportInfos(string id)
        {
            int resourceId = Convert.ToInt32(id);

            // Get sport Ids from ResourceSport table.
            var coaches = (from r in this._repository.ResourceSports
                           where r.ResourceID == resourceId && r.SportID != "ALL"
                           select new
                           {
                               YearID = r.YearID,
                               CoachResourceID = r.CoachResourceID
                           }
                            ).OrderByDescending(x => x.YearID).ToList();

            return (coaches.Count == 0) ? null : coaches[0].CoachResourceID.ToString();
        }

        private List<string> GetSportIdsFromResults(int resourceId)
        {
            // Get sport Ids from Result table.
            return (from s in this._repository.Sports
                    join r in this._repository.Results
                    on s.SportID equals r.SportID
                    join rr in this._repository.ResultResources
                    on r.ResultID equals rr.ResultID
                    where rr.ResourceID == resourceId
                    select s.ParentSport.ParentSport.ParentSportID
                    ).Distinct().ToList();
        }

        private List<ListItem> GetCompetitionTypesFromResults(int resourceId)
        {
            // Get sport Ids from Result table.
            var sportIds = (from s in this._repository.Sports
                            join r in this._repository.Results
                            on s.SportID equals r.SportID
                            join rr in this._repository.ResultResources
                            on r.ResultID equals rr.ResultID
                            where rr.ResourceID == resourceId
                            select s.ParentSport.ParentSport.ParentSportID
                            ).Distinct().ToList();

            // Get competition type list based on those from Result table and 3 defaults.
            return GetCompetitionTypes(GetCompetitionTypesFromSportID(sportIds));
        }

        private List<string> GetCompetitionTypesFromSportID(List<string> sportIds)
        {
            List<string> types = new List<string>();

            foreach (string s in sportIds)
            {
                string compType = s.Equals("S_O") ? "OSG" :
                                s.Equals("W_O") ? "OWG" :
                                s.Equals("S_P") ? "PSG" :
                                s.Equals("W_P") ? "PWG" :
                                s.Equals("ALL") ? "OSG" : "";
                types.Add(compType);
            }
            return types;
        }

        private List<ListItem> GetCompetitionTypes(List<string> competitionTypes)
        {
            // Form competition type list based on those from Result table and 3 defaults.
            List<ListItem> types = (from c in this._repository.CompetitionTypes
                                     where c.CompetitionTypeID.Equals("OTHER") || c.CompetitionTypeID.Equals("WC") || c.CompetitionTypeID.Equals("WCGP")
                                     select new ListItem
                                     {
                                         Id = c.CompetitionTypeID,
                                         Description = _frenchLanguage ? c.Name_FR : c.Name_EN
                                     }).ToList();

            foreach (var compType in competitionTypes)
            {
                var addTypes = (from c in this._repository.CompetitionTypes
                                where c.CompetitionTypeID == compType
                                select new ListItem
                                {
                                    Id = compType,
                                    Description = _frenchLanguage ? c.Name_FR : c.Name_EN
                                }).FirstOrDefault();

                types.Add(addTypes);
            }

            return types.OrderBy(a => a.Description).ToList();
        }

        public List<ListItem> GetAllCompetitionNameList()
        {
            string cacheKey = string.Format(Constant.Cache.KeyTemplate, Constant.Cache.DecodeCompetitionName, _frenchLanguage, null);
            ObjectCache cache = MemoryCache.Default;

            if (cache.Contains(cacheKey))
            {
                return (List<ListItem>)cache.Get(cacheKey);
            }
            else
            {
                var temp = (from c in this._repository.Competitions
                            select new
                            {
                                Id = c.Name,
                                Description = c.Name,
                                Date = c.CompetitionDate
                            }).ToList();

                List<ListItem> competitions = new List<ListItem>();
                for (int i = 0; i < temp.Count; i++)
                {
                    ListItem competition = new ListItem();
                    competition.Id = temp[i].Id.ToString();
                    competition.Description = temp[i].Description;
                    //competition.Description = temp[i].Description + "(" + temp[i].Date.Year.ToString() + "/" + temp[i].Date.Month.ToString() + ")";
                    competitions.Add(competition);
                }

                competitions = competitions.GroupBy(c => c.Description).Select(g => g.First()).OrderBy(c => c.Description).ToList();

                cache.Add(cacheKey, competitions, oneHourSlidingPolicy);

                return competitions;
            }
        }

        public List<ListItem> GetAllHostCityList()
        {
            string cacheKey = string.Format(Constant.Cache.KeyTemplate, Constant.Cache.DecodeCompetitionHostCity, _frenchLanguage, null);
            ObjectCache cache = MemoryCache.Default;

            if (cache.Contains(cacheKey))
            {
                return (List<ListItem>)cache.Get(cacheKey);
            }
            else
            {
                var temp = (from c in this._repository.Competitions
                            select new 
                            {
                                Id = c.Location,
                                Description = c.Location,
                            }).ToList();

                List<ListItem> locations = new List<ListItem>();
                for (int i = 0; i < temp.Count; i++)
                {
                    ListItem location = new ListItem();
                    location.Id = temp[i].Id.ToString();
                    location.Description = temp[i].Description;
                    locations.Add(location);
                }

                locations = locations.GroupBy(c => c.Description).Select(g => g.First()).OrderBy(c => c.Description).ToList();
                locations.RemoveAll(x => x.Description.Contains("/"));

                cache.Add(cacheKey, locations, oneHourSlidingPolicy);

                return locations;
            }
        }

        public string GetCompetitionDateByCompetitionID(string id)
        {
            if (string.IsNullOrEmpty(id))
                return string.Format("{0:dd/MM/yyyy}", DateTime.Today);

            int competitionId = int.Parse(id); 

            string cacheKey = string.Format(Constant.Cache.KeyTemplate, Constant.Cache.DecodeCompetitionDate, _frenchLanguage, null);
            ObjectCache cache = MemoryCache.Default;

            if (cache.Contains(cacheKey))
            {
                return (string)cache.Get(cacheKey);
            }
            else
            {
                DateTime competitionDate = (from c in this._repository.Competitions
                                       where c.CompetitionID == competitionId
                                       select c.CompetitionDate
                                       ).FirstOrDefault();

                string date = string.Format("{0:dd/MM/yyyy}", competitionDate);
                cache.Add(cacheKey, date, oneHourSlidingPolicy);

                return date;
            }
        }

        public List<ListItem> GetAffiliateTypes()
        {
            string cacheKey = string.Format(Constant.Cache.KeyTemplate, Constant.Cache.DecodeAffiliate, _frenchLanguage, null);
            ObjectCache cache = MemoryCache.Default;

            if (cache.Contains(cacheKey))
            {
                return (List<ListItem>)cache.Get(cacheKey);
            }
            else
            {
                List<ListItem> types = null;

                types = (from t in this._repository.AffiliateTypes
                            select new ListItem
                            {
                                Id = t.AffiliateTypeID,
                                Description = _frenchLanguage ? t.Name_FR : t.Name_EN
                            }).ToList();

                cache.Add(cacheKey, types, oneHourSlidingPolicy);

                return types;
            }
        }

        public List<ListItem> GetDirectAthleteSupportTypes()
        {
            string cacheKey = string.Format(Constant.Cache.KeyTemplate, Constant.Cache.DecodeDirectAthleteSupportType, _frenchLanguage, null);
            ObjectCache cache = MemoryCache.Default;

            if (cache.Contains(cacheKey))
            {
                return (List<ListItem>)cache.Get(cacheKey);
            }
            else
            {
                List<ListItem> types = new List<ListItem>();
                types.Add(new ListItem { Id = null, Description = BusinessLocalization.SelectType });

                types.AddRange((from t in this._repository.DirectAthleteSupportTypes
                            select new ListItem
                            {
                                Id = t.DirectAthleteSupportTypeID,
                                Description = _frenchLanguage ? t.Name_FR : t.Name_EN
                            }).OrderBy(t => t.Description).ToList());

                cache.Add(cacheKey, types, oneHourSlidingPolicy);

                return types;
            }
        }

        public List<ListItem> GetDirectAthleteSupportCategoriesByType(string directAthleteSupportTypeId)
        {
            string cacheKey = string.Format(Constant.Cache.KeyTemplate, Constant.Cache.DecodDirectAthleteSupportCategory, _frenchLanguage, directAthleteSupportTypeId);
            ObjectCache cache = MemoryCache.Default;

            if (cache.Contains(cacheKey))
            {
                return (List<ListItem>)cache.Get(cacheKey);
            }
            else
            {
                List<ListItem> categories = new List<ListItem>();
                categories.Add(new ListItem { Id = null, Description = BusinessLocalization.SelectType });

                categories.AddRange((from c in this._repository.DirectAthleteSupports
                                where c.DirectAthleteSupportTypeID == directAthleteSupportTypeId
                                select new ListItem
                                {
                                    Id = c.DirectAthleteSupportID,
                                    Description = _frenchLanguage ? c.Name_FR : c.Name_EN
                                }).OrderBy(t => t.Description).ToList());

                cache.Add(cacheKey, categories, oneHourSlidingPolicy);

                return categories;
            }
        }

        public List<ListItem> GetGenders(bool includeNull)
        {
            string cacheKey = string.Format(Constant.Cache.KeyTemplate, Constant.Cache.DecodeGender, _frenchLanguage, includeNull);
            ObjectCache cache = MemoryCache.Default;

            var genders = new List<ListItem>();

            if (cache.Contains(cacheKey))
            {
                genders = (List<ListItem>)cache.Get(cacheKey);
            }
            else
            {
                genders = (from g in this._repository.Decodes
                            where g.DecodeCategoryID == Constant.DecodeCategoryId.Gender
                            orderby g.SortOrder
                            select new ListItem
                            {
                                Id = g.DecodeID,
                                Description = _frenchLanguage ? g.DecodeID_FR : g.DecodeID_EN
                            }).ToList();

                if (includeNull)
                {
                    genders.Insert(0, new ListItem { Id = null, Description = CommonLocalization.ListItem_NotSet });
                }

                cache.Add(cacheKey, genders, oneHourSlidingPolicy);
            }

            return genders;
        }

        public List<ListItem> GetHumanGenders(bool includeNull)
        {
            string cacheKey = string.Format(Constant.Cache.KeyTemplate, Constant.Cache.DecodeHumanGender, _frenchLanguage, includeNull);
            ObjectCache cache = MemoryCache.Default;

            var genders = new List<ListItem>();

            if (cache.Contains(cacheKey))
            {
                genders = (List<ListItem>)cache.Get(cacheKey);
            }
            else
            {
                genders = (from g in this._repository.Decodes
                           where g.DecodeCategoryID == Constant.DecodeCategoryId.Gender && (g.DecodeID == Constant.Gender.Male || g.DecodeID == Constant.Gender.Female)
                           orderby g.SortOrder
                           select new ListItem
                           {
                               Id = g.DecodeID,
                               Description = _frenchLanguage ? g.DecodeID_FR : g.DecodeID_EN
                           }).ToList();

                if (includeNull)
                {
                    genders.InsertNullListItem(_frenchLanguage);
                }

                cache.Add(cacheKey, genders, oneHourSlidingPolicy);
            }

            return genders;
        }

        public List<Organization> GetAllOrganizations()
        {
            string cacheKey = string.Format(Constant.Cache.KeyTemplate, Constant.Cache.DecodeOrganization, _frenchLanguage, null);
            ObjectCache cache = MemoryCache.Default;

            if (cache.Contains(cacheKey))
            {
                return (List<Organization>)cache.Get(cacheKey);
            }
            else
            {
                var organizations = (from o in this._repository.Organizations
                                     select new Organization
                                     {
                                         Id = o.OrganizationID,
                                         Type = o.OrganizationTypeID,
                                         Name = _frenchLanguage ? o.Name_FR : o.Name_EN
                                     }).ToList();

                cache.Add(cacheKey, organizations, oneHourSlidingPolicy);

                return organizations;
            }
        }

        public List<Organization> GetAllOrganizationsByType(string organizationTypeId, bool includeNullItem = false)
        {
            string cacheKey = string.Format(Constant.Cache.KeyTemplate, Constant.Cache.DecodeOrganizationByType, _frenchLanguage, organizationTypeId, includeNullItem);
            ObjectCache cache = MemoryCache.Default;

            if (cache.Contains(cacheKey))
            {
                return (List<Organization>)cache.Get(cacheKey);
            }
            else
            {
                var organizations = (from o in this._repository.Organizations
                                    where o.OrganizationTypeID == organizationTypeId
                                    select new Organization
                                    {
                                        Id = o.OrganizationID,
                                        Type = o.OrganizationTypeID,
                                        Name = _frenchLanguage ? o.Name_FR : o.Name_EN
                                    }).ToList();

                if (includeNullItem)
                {
                    organizations.Insert(0, new Organization { Id = null, Name = CommonLocalization.ListItem_NotSet });
                }

                cache.Add(cacheKey, organizations, oneHourSlidingPolicy);

                return organizations;
            }
        }

        public List<OTP.Ring.Data.Models.SecurityPage> GetAllPages()
        {
            return this._repository.SecurityPages.ToList();
        }

        public List<OTP.Ring.Models.ViewModel.SummaryListViewModel> GetSummaryPageList(string currentPageId, User user)
        {
            var securityPages = from groupPages in this._repository.SecurityGroupPages
                                where groupPages.SecurityGroupProfileID == user.GroupProfileId
                                    && groupPages.ReadAccess != Constant.AccessLevel.No
                                select groupPages;

            var allowedPages = (from pages in this._repository.SecurityPages
                                where pages.Visible
                                from securityPage in securityPages
                                where (pages.SecurityPageID == securityPage.SecurityPageID
                                    || (securityPage.SecurityPageID == Constant.PageId.HomeIndex && pages.SecurityPageID == pages.ParentSecurityPageID))
                                    && securityPage.Recursive
                                join children in this._repository.SecurityPages on pages.SecurityPageID equals children.ParentSecurityPageID
                                join childrenChildren in this._repository.SecurityPages on children.SecurityPageID equals childrenChildren.ParentSecurityPageID
                                select childrenChildren)
                                .Union(
                                    from pages in this._repository.SecurityPages
                                    where pages.Visible
                                    from securityPage in securityPages
                                    where pages.SecurityPageID == securityPage.SecurityPageID
                                        && !securityPage.Recursive
                                    select pages
                                );

            var summaryPages = (from p in this._repository.SecurityPages
                                where (p.ParentSecurityPageID == currentPageId
                                    || (currentPageId == Constant.PageId.HomeIndex && p.SecurityPageID == p.ParentSecurityPageID))
                                    && p.SecurityPageID != Constant.PageId.HomeIndex
                                    && p.SecurityPageID != currentPageId
                                    && p.Visible
                                from a in allowedPages
                                where p.SecurityPageID == a.SecurityPageID
                                select new OTP.Ring.Models.ViewModel.SummaryListViewModel
                                {
                                    SecurityPageId = a.SecurityPageID,
                                    Description = _frenchLanguage ? a.Description_FR : a.Description_EN,
                                    HtmlStyleCode = a.StylingCode,
                                    IconUrl = a.IconURL,
                                    Name = _frenchLanguage ? a.Name_FR : a.Name_EN,
                                    Order = a.Order,
                                    URL = a.URL
                                }).OrderBy(p => p.Order).ToList();

            foreach (var page in summaryPages)
            {
                if (page.IconUrl == null)
                {
                    var parentPageId = page.SecurityPageId[0].ToString();
                    page.IconUrl = (from p in this._repository.SecurityPages
                                    where p.SecurityPageID == parentPageId
                                    select p.IconURL).FirstOrDefault();
                }
            }

            return summaryPages;
        }
        
        public char[] GetColumnLetterArray(int numberofColumns)
        {
            var columnLetters = new char[numberofColumns];
            //Build array of letters from A - ?
            for (int i = 0; i < numberofColumns; i++)
            {
                columnLetters[i] = (char)(i + (int)('A'));
            }

            return columnLetters;
        }

        public List<ListItem> GetColumnFormatList(int downloadTemplateId, string fieldName)
        {
            var formats = new List<ListItem>();

            var formatType = (from f in this._repository.TableTemplateFields
                              from d in this._repository.DownloadTemplates
                              where d.DownloadTemplateID == downloadTemplateId
                              && f.TableTemplateID == d.TableTemplateID
                              && f.FieldName == fieldName
                              select f.FormatType).SingleOrDefault();

            if (formatType != null)
            {
                formats = (from f in this._repository.Decodes
                           where f.DecodeCategoryID == formatType
                           select new ListItem
                           {
                               Id = f.DecodeID,
                               Description = f.DecodeID
                           }).ToList();
            }

            formats.InsertNullListItem(_frenchLanguage);

            return formats;
        }

        public string GetGenderCode(string sportid)
        {
            if (sportid.Contains("_M_"))
            {
                return "M";
            }
            else if (sportid.Contains("_F_"))
            {
                return "F";
            }
            else if (sportid.Contains("_C_"))
            {
                return "C";
            }
            else
            {
                return "";
            }
        }

        public string GetDiscipline(string discipline)
        {
            if (string.IsNullOrEmpty(discipline))
                return string.Empty;

            if (_frenchLanguage)
            {
                discipline = discipline.Replace("(F)", "(Femmes)").Replace("(M)", "(Hommes)");
            }
            else
            {
                discipline = discipline.Replace("(F)", "(Women)").Replace("(M)", "(Men)");
            }

            return discipline;
        }

        public MemoryStream CreateExcelFile<T>(dynamic query)
        {
            // Get properties info in the class
            List<ExportToExcelAttribute> exportList = GetExportToExcelList<T>();

            // Create new Excel workbook
            var workbook = new HSSFWorkbook();

            // Create new Excel sheet
            var sheet = workbook.CreateSheet();

            // Set the width of the columns
            for (int i = 0; i < exportList.Count; i++)
            {
                sheet.SetColumnWidth(i, exportList[i].CellLength * Constant.ExportExcelCellUnit);
            }

            // Create a header row
            var headerRow = sheet.CreateRow(0);

            // Create header style 
            HSSFFont font = (HSSFFont)workbook.CreateFont();
            font.Boldweight = (short)FontBoldWeight.BOLD;
            HSSFCellStyle header_style = (HSSFCellStyle)workbook.CreateCellStyle();
            header_style.SetFont(font);
            header_style.Alignment = HorizontalAlignment.CENTER;

            // Set the column names and style in the header row
            for (int i = 0; i < exportList.Count; i++)
            {
                headerRow.CreateCell(i).SetCellValue(exportList[i].PropertyDisplayName);
                headerRow.GetCell(i).CellStyle = header_style;
            }

            //(Optional) freeze the header row so it is not scrolled
            sheet.CreateFreezePane(0, 1, 0, 1);

            int rowNumber = 1;

            // Create cell style 
            HSSFCellStyle cell_style = (HSSFCellStyle)workbook.CreateCellStyle();

            // Populate the sheet with values from the grid data
            foreach (var item in query)
            {
                // Create a new row
                var row = sheet.CreateRow(rowNumber++);

                for (int i = 0; i < exportList.Count; i++)
                {
                    // Get item value for each property
                    object objectValue = item.GetType().GetProperty(exportList[i].PropertyName).GetValue(item, null);

                    // Set value for the cell if it is not null
                    if (objectValue == null)
                        continue;

                    // Set value for the cell if it is not empty or null
                    string itemValue = objectValue.ToString();
                    if (string.IsNullOrEmpty(itemValue))
                        continue;

                    // Take the lower case for bool type
                    if (item.GetType().GetProperty(exportList[i].PropertyName).PropertyType == typeof(bool))
                    {
                        itemValue = _frenchLanguage ? ((itemValue == "True") ? "vrai" : "faux") : ((itemValue == "True") ? "true" : "false");
                    }

                    row.CreateCell(i).SetCellValue(itemValue);

                    // Set style for the cell if the style is not left-aligned which is default
                    if (exportList[i].CellAlignment != Constant.ExcelAlignment.Left)
                    {
                        cell_style.Alignment = GetNPOIAlignment(exportList[i].CellAlignment);
                        row.GetCell(i).CellStyle = cell_style;
                    }
                }
            }

            // Write the workbook to a memory stream
            MemoryStream output = new MemoryStream();
            workbook.Write(output);

            return output;
        }

        //public MemoryStream CreateAthletePageExcelFile(dynamic query)
        //{
        //    // Create new Excel workbook
        //    var workbook = new HSSFWorkbook();

        //    // Create new Excel sheet
        //    var sheet = workbook.CreateSheet();

        //    // Set the width of the columns
        //    sheet.SetColumnWidth(0, 60 * 64);
        //    sheet.SetColumnWidth(1, 100 * 64);
        //    sheet.SetColumnWidth(2, 100 * 64);
        //    sheet.SetColumnWidth(3, 100 * 64);
        //    sheet.SetColumnWidth(4, 40 * 64);
        //    sheet.SetColumnWidth(5, 70 * 64);
        //    sheet.SetColumnWidth(6, 60 * 64);
        //    sheet.SetColumnWidth(7, 60 * 64);
        //    sheet.SetColumnWidth(8, 80 * 64);
        //    sheet.SetColumnWidth(9, 100 * 64);

        //    // Create a header row
        //    var headerRow = sheet.CreateRow(0);

        //    // Create header style 
        //    HSSFFont font = (HSSFFont)workbook.CreateFont();
        //    font.Boldweight = (short)FontBoldWeight.BOLD;
        //    HSSFCellStyle header_style = (HSSFCellStyle)workbook.CreateCellStyle();
        //    header_style.SetFont(font);
        //    header_style.Alignment = HorizontalAlignment.CENTER;

        //    // Set the column names and style in the header row
        //    headerRow.CreateCell(0).SetCellValue("ResourceId");
        //    headerRow.GetCell(0).CellStyle = header_style;
        //    headerRow.CreateCell(1).SetCellValue("LastName");
        //    headerRow.GetCell(1).CellStyle = header_style;
        //    headerRow.CreateCell(2).SetCellValue("FirstName");
        //    headerRow.GetCell(2).CellStyle = header_style;
        //    headerRow.CreateCell(3).SetCellValue("Country");
        //    headerRow.GetCell(3).CellStyle = header_style;
        //    headerRow.CreateCell(4).SetCellValue("Gender");
        //    headerRow.GetCell(4).CellStyle = header_style;
        //    headerRow.CreateCell(5).SetCellValue("BirthDate");
        //    headerRow.GetCell(5).CellStyle = header_style;
        //    headerRow.CreateCell(6).SetCellValue("IsNew");
        //    headerRow.GetCell(6).CellStyle = header_style;
        //    headerRow.CreateCell(7).SetCellValue("Benchmark");
        //    headerRow.GetCell(7).CellStyle = header_style;
        //    headerRow.CreateCell(8).SetCellValue("Category");
        //    headerRow.GetCell(8).CellStyle = header_style;
        //    headerRow.CreateCell(9).SetCellValue("PersonalCoach");
        //    headerRow.GetCell(9).CellStyle = header_style;

        //    //(Optional) freeze the header row so it is not scrolled
        //    sheet.CreateFreezePane(0, 1, 0, 1);

        //    int rowNumber = 1;

        //    // Create cell style 
        //    HSSFCellStyle cell_style = (HSSFCellStyle)workbook.CreateCellStyle();
        //    cell_style.Alignment = HorizontalAlignment.CENTER;

        //    //Populate the sheet with values from the grid data
        //    foreach (MaintenanceAthletesAthleteGridViewModel item in query)
        //    {
        //        //Create a new row
        //        var row = sheet.CreateRow(rowNumber++);

        //        //Set values for the cells
        //        row.CreateCell(0).SetCellValue(item.ResourceId);
        //        row.GetCell(0).CellStyle = cell_style;
        //        row.CreateCell(1).SetCellValue(item.LastName);
        //        row.CreateCell(2).SetCellValue(item.FirstName);
        //        row.CreateCell(3).SetCellValue(item.CountryName);
        //        row.CreateCell(4).SetCellValue(item.GenderId);
        //        row.GetCell(4).CellStyle = cell_style;
        //        row.CreateCell(5).SetCellValue(string.Format("{0:yyyy/MM/dd}", item.BirthDate));
        //        row.GetCell(5).CellStyle = cell_style;
        //        row.CreateCell(6).SetCellValue(item.IsNew.ToString().ToLower());
        //        row.GetCell(6).CellStyle = cell_style;
        //        row.CreateCell(7).SetCellValue(item.BenchmarksEntered.ToString().ToLower());
        //        row.GetCell(7).CellStyle = cell_style;
        //        row.CreateCell(8).SetCellValue((item.Category == null) ? string.Empty : item.Category);
        //        row.GetCell(8).CellStyle = cell_style;
        //        row.CreateCell(9).SetCellValue((item.PersonalCoachName == null) ? string.Empty : item.PersonalCoachName);
        //    }

        //    //Write the workbook to a memory stream
        //    MemoryStream output = new MemoryStream();
        //    workbook.Write(output);

        //    return output;
        //}

        //public MemoryStream CreateAthleteResultsExcelFile(dynamic query)
        //{
        //    //var properties = typeof(MaintenanceAthletesResultsViewModel).GetProperties();
        //    //List<string> propertyName = new List<string>();
        //    //foreach (var p in properties)
        //    //{
        //    //    propertyName.Add(p.Name);
        //    //}

        //    MemberInfo[] members = typeof(MaintenanceAthletesResultsViewModel).GetMembers();
        //    foreach (MemberInfo member in members)
        //    {
        //        object[] attributes = member.GetCustomAttributes(true);

        //        if (attributes.Length == 0)
        //            continue;

        //        string alignment = string.Empty;
        //        foreach (object attribute in attributes)
        //        {
        //            ExportToExcelAttribute export = attribute as ExportToExcelAttribute;
        //            LocalizedDisplayNameAttribute localDisplayName = attribute as LocalizedDisplayNameAttribute;
        //            if (export != null)
        //            {
        //                alignment = export.CellAlignment;
        //            }
        //        }
        //    }

        //    // Create new Excel workbook
        //    var workbook = new HSSFWorkbook();

        //    // Create new Excel sheet
        //    var sheet = workbook.CreateSheet();

        //    // Set the width of the columns
        //    sheet.SetColumnWidth(0, 150 * Constant.ExportExcelCellUnit);
        //    sheet.SetColumnWidth(1, 60 * 64);
        //    sheet.SetColumnWidth(2, 80 * 64);
        //    sheet.SetColumnWidth(3, 80 * 64);
        //    sheet.SetColumnWidth(4, 120 * 64);
        //    sheet.SetColumnWidth(5, 80 * 64);
        //    sheet.SetColumnWidth(6, 100 * 64);
        //    sheet.SetColumnWidth(7, 60 * 64);
        //    sheet.SetColumnWidth(8, 40 * 64);
        //    sheet.SetColumnWidth(9, 50 * 64);
        //    sheet.SetColumnWidth(10, 40 * 64);

        //    // Create a header row
        //    var headerRow = sheet.CreateRow(0);

        //    // Create header style 
        //    HSSFFont font = (HSSFFont)workbook.CreateFont();
        //    font.Boldweight = (short)FontBoldWeight.BOLD;
        //    HSSFCellStyle header_style = (HSSFCellStyle)workbook.CreateCellStyle();
        //    header_style.SetFont(font);
        //    header_style.Alignment = HorizontalAlignment.CENTER;

        //    // Set the column names and style in the header row
        //    headerRow.CreateCell(0).SetCellValue("Competition");
        //    headerRow.GetCell(0).CellStyle = header_style;
        //    headerRow.CreateCell(1).SetCellValue("Date");
        //    headerRow.GetCell(1).CellStyle = header_style;
        //    headerRow.CreateCell(2).SetCellValue("Location");
        //    headerRow.GetCell(2).CellStyle = header_style;
        //    headerRow.CreateCell(3).SetCellValue("Country");
        //    headerRow.GetCell(3).CellStyle = header_style;
        //    headerRow.CreateCell(4).SetCellValue("Sport");
        //    headerRow.GetCell(4).CellStyle = header_style;
        //    headerRow.CreateCell(5).SetCellValue("Discipline");
        //    headerRow.GetCell(5).CellStyle = header_style;
        //    headerRow.CreateCell(6).SetCellValue("Event");
        //    headerRow.GetCell(6).CellStyle = header_style;
        //    headerRow.CreateCell(7).SetCellValue("EventDate");
        //    headerRow.GetCell(7).CellStyle = header_style;
        //    headerRow.CreateCell(8).SetCellValue("Position");
        //    headerRow.GetCell(8).CellStyle = header_style;
        //    headerRow.CreateCell(9).SetCellValue("PerfTime");
        //    headerRow.GetCell(9).CellStyle = header_style;
        //    headerRow.CreateCell(10).SetCellValue("PerfValue");
        //    headerRow.GetCell(10).CellStyle = header_style;

        //    //(Optional) freeze the header row so it is not scrolled
        //    sheet.CreateFreezePane(0, 1, 0, 1);

        //    int rowNumber = 1;

        //    // Create cell style 
        //    HSSFCellStyle cell_style = (HSSFCellStyle)workbook.CreateCellStyle();
        //    cell_style.Alignment = HorizontalAlignment.CENTER;

        //    //Populate the sheet with values from the grid data
        //    foreach (MaintenanceAthletesResultsViewModel item in query)
        //    {
        //        //Create a new row
        //        var row = sheet.CreateRow(rowNumber++);

        //        //Set values for the cells
        //        row.CreateCell(0).SetCellValue(item.CompetitionName);
        //        row.CreateCell(1).SetCellValue(item.CompetitionDate);
        //        row.GetCell(1).CellStyle = cell_style;
        //        row.CreateCell(2).SetCellValue(item.Location);
        //        row.CreateCell(3).SetCellValue(item.CountryName);
        //        row.CreateCell(4).SetCellValue(item.SportName);
        //        row.CreateCell(5).SetCellValue(item.DisciplineName);
        //        row.CreateCell(6).SetCellValue(item.EventName);
        //        row.CreateCell(7).SetCellValue(item.EventDate);
        //        row.GetCell(7).CellStyle = cell_style;
        //        row.CreateCell(8).SetCellValue(item.Position.ToString());
        //        row.GetCell(8).CellStyle = cell_style;
        //        row.CreateCell(9).SetCellValue(item.PerfTime);
        //        row.GetCell(9).CellStyle = cell_style;
        //        row.CreateCell(10).SetCellValue((item.PerfValue == null) ? string.Empty : item.PerfValue.ToString());
        //        row.GetCell(10).CellStyle = cell_style;
        //    }

        //    //Write the workbook to a memory stream
        //    MemoryStream output = new MemoryStream();
        //    workbook.Write(output);

        //    return output;
        //}

        #endregion

        private OTP.Ring.Data.Models.SecurityPage GetTopLevelPage(OTP.Ring.Data.Models.SecurityPage page)
        {
            if (page.NodeLeafCode == Constant.NodeLeafCode.Node)
            {
                return page;
            }
            else
            {
                return GetTopLevelPage(page.ParentSecurityPage);
            }
        }

        private List<ExportToExcelAttribute> GetExportToExcelList<T>()
        {
            List<ExportToExcelAttribute> exportList = new List<ExportToExcelAttribute>();

            // Get properties info in the class
            MemberInfo[] members = typeof(T).GetMembers();
            foreach (MemberInfo member in members)
            {
                // Get the attributes for each property
                object[] attributes = member.GetCustomAttributes(true);

                if (attributes.Length == 0)
                    continue;

                foreach (object attribute in attributes)
                {
                    // Find ExportToExcelAttribute
                    ExportToExcelAttribute export = attribute as ExportToExcelAttribute;
                    if (export != null)
                    {
                        LocalizedDisplayNameAttribute localDisplayName = null;

                        // Find the LocalizedDisplayNameAttribute
                        foreach (object attribute1 in attributes)
                        {
                            localDisplayName = attribute1 as LocalizedDisplayNameAttribute;
                            if (localDisplayName != null)
                                break;
                        }

                        // Copy attribute info for export to a list
                        exportList.Add(new ExportToExcelAttribute
                        {
                            PropertyName = member.Name,                         // Get the property name
                            PropertyDisplayName = localDisplayName.DisplayName, // Get the display name of the property
                            CellLength = export.CellLength,                     // Get the cell length
                            CellAlignment = export.CellAlignment                // Get the cell alignment
                        });
                    }
                }
            }
            return exportList;
        }

        private HorizontalAlignment GetNPOIAlignment(string alignment)
        {
            if (alignment == Constant.ExcelAlignment.Left)
            {
                return HorizontalAlignment.LEFT;
            }
            else if (alignment == Constant.ExcelAlignment.Center)
            {
                return HorizontalAlignment.CENTER;
            }
            else if (alignment == Constant.ExcelAlignment.Right)
            {
                return HorizontalAlignment.RIGHT;
            }
            else if (alignment == Constant.ExcelAlignment.Justify)
            {
                return HorizontalAlignment.JUSTIFY;
            }
            else
            {
                return HorizontalAlignment.LEFT;
            }
        }
    }
}
