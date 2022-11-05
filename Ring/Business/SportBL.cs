using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OTP.Ring.Data.Models;
using OTP.Ring.Common;
using System.Transactions;
using System.Web.Mvc;
using OTP.Ring.Models;
using OTP.Ring.Models.ViewModel;
using System.Web;

namespace OTP.Ring.Business
{
    public class SportBL : ISportBL
    {
        private bool _frenchLanguage = false;
        private Data.Models.RingEntities _repository;

        public SportBL(string connectionString)
        {
            _frenchLanguage = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToUpper() == Constant.LanguageCode.French;
            _repository = new Data.Models.RingEntities(connectionString);
        }

        public Sport GetSport(string id)
        {
            return (from s in this._repository.Sports
                       where s.SportID == id
                       select s).FirstOrDefault();
        }

        private List<Sport> GetSports(int upTopAndIncludingSportLevelId)
        {
            var sports = from s in this._repository.Sports
                         join sl in this._repository.SportLevels on s.SportLevelID equals sl.SportLevelID
                         where sl.SportLevelID <= upTopAndIncludingSportLevelId
                         && s.Active
                         select s;

            return sports.ToList();
        }

        public List<Sport> GetAllSportsToSportLevel()
        {
            return GetSports(Constant.SportLevelId.Sport);
        }

        public List<Sport> GetAllSportsToDisciplineLevel()
        {
            return GetSports(Constant.SportLevelId.Discipline);
        }

        public List<Sport> GetAllSportsToEventLevel()
        {
            return GetSports(Constant.SportLevelId.Event);
        }

        public int UpdateSport(
            string sportId,
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
            bool? resourceTracking = null)
        {
            int changes = 0;

            var sport = (from s in this._repository.Sports
                            where s.SportID == sportId
                            select s).First();

            sport.OrganizationID = nsoId == Constant.UndefinedString ? sport.OrganizationID : nsoId;
            sport.HPAID = (hpaId == Constant.UndefinedString) ? sport.HPAID : (hpaId == "-1" ? null : hpaId);
            sport.Name_EN = name_EN == Constant.UndefinedString ? sport.Name_EN : name_EN;
            sport.Name_FR = name_FR == Constant.UndefinedString ? sport.Name_FR : name_FR;
            sport.SportCode = sportCode == Constant.UndefinedString ? sport.SportCode : sportCode;
            sport.Benchmark = benchmark ?? sport.Benchmark;
            sport.TeamSize = teamSize ?? sport.TeamSize;
            sport.MedalEvent = medalEvent ?? sport.MedalEvent;
            sport.Comments = comments == Constant.UndefinedString ? sport.Comments : HttpUtility.HtmlEncode(comments);
            sport.Gender = gender == Constant.UndefinedString ? sport.Gender : gender;
            sport.Active = active ?? sport.Active;
            sport.ResourceTracking = resourceTracking ?? sport.ResourceTracking;
            sport.TeamSport = teamSport ?? sport.TeamSport;

            using (TransactionScope scope = new TransactionScope())
            {
                //Update Sport
                var updateSportReturnValue = this._repository.uspUpdateSport(
                    sport.SportID,
                    sport.SportCode,
                    sport.Name_EN,
                    sport.Name_FR,
                    sport.OrganizationID,
                    sport.HPAID,
                    sport.Benchmark,
                    sport.Comments,
                    sport.TeamSize,
                    sport.MedalEvent,
                    sport.StylingCode,
                    sport.Gender,
                    sport.Active,
                    sport.TeamSport,
                    sport.ResourceTracking).SingleOrDefault();

                changes = updateSportReturnValue == null ? changes : changes + (int)updateSportReturnValue;

                //Update / Add / Delete Sport Ranking for the selected year
                if (rankingYear != null && rankingCategoryId != Constant.UndefinedString)
                {
                    var ranking = sport.SportRankings.Where(r => r.YearID == rankingYear).SingleOrDefault();

                    if (ranking == null)
                    {
                        if (rankingCategoryId != "-1")
                        {
                            //Add sport ranking for the selected year

                            var rankingId = this._repository.uspAddSportRanking(
                                sport.SportID,
                                rankingYear,
                                rankingCategoryId,
                                null).SingleOrDefault();

                            changes = rankingId == null ? changes : changes++;
                        }
                    }
                    else
                    {
                        if (rankingCategoryId == "-1")
                        {
                            //Remove the ranking for the selected year
                            var deleteSportRankingReturnValue = this._repository.uspDeleteSportRanking(ranking.SportRankingID).SingleOrDefault();
                            changes = deleteSportRankingReturnValue == null ? changes : changes + (int)deleteSportRankingReturnValue;
                        }
                        else
                        {
                            //Update sport ranking for the selected year
                            var updateSportRankingReturnValue = this._repository.uspUpdateSportRanking(
                                ranking.SportRankingID,
                                ranking.SportID,
                                rankingYear,
                                rankingCategoryId,
                                ranking.Comments).SingleOrDefault();

                            changes = updateSportRankingReturnValue == null ? changes : changes + (int)updateSportRankingReturnValue;
                        }
                    }
                }

                scope.Complete();
            }

            return changes;
        }

        public int AddEventOrDiscipline(
            string parentSportId,
            string sportCode,
            string name_en,
            string name_fr,
            string comments,
            EventOrDiscipline sportLevel,
            bool benchmark,
            int? teamSize,
            bool? medalEvent,
            string gender,
            bool active,
            bool teamSport,
            bool resourceTracking)
        {
            int changes = 0;

            string sportId = string.Empty;
            if (sportLevel == EventOrDiscipline.Discipline)
            {
                sportId = parentSportId.Substring(0, parentSportId.Length - 1) + gender + "_" + sportCode;
            }
            else
            {
                sportId = parentSportId + "_" + sportCode;
            }

            var nameExists = (from s in this._repository.Sports
                          where s.Active
                          && s.SportID == sportId
                          && s.SportCode == sportCode
                          //&& (s.Name_EN == name_en
                          //|| s.Name_FR == name_fr)
                          select s).Any();

            if (nameExists)
            {
                //TODO: Localize
                throw new ArgumentException("The supplied Sport Id or Code is already in use.");
            }

            var returnValue = this._repository.uspAddSport(
                sportId,
                sportLevel == EventOrDiscipline.Event ? Constant.SportLevelId.Event : Constant.SportLevelId.Discipline,
                sportCode,
                name_en,
                name_fr,
                parentSportId,
                null,
                null,
                gender,
                benchmark,
                HttpUtility.HtmlEncode(comments),
                teamSize,
                medalEvent,
                0,
                active,
                sportLevel == EventOrDiscipline.Event ? teamSport : false,
                sportLevel == EventOrDiscipline.Event ? resourceTracking : false).SingleOrDefault();

            changes = returnValue == null ? changes : (int)returnValue;

            return changes;
        }

        public int DeleteEventOrDiscipline(string sportId)
        {
            int changes = 0;

            changes = this._repository.uspDeleteSport(sportId);

            return changes;
        }

        public enum EventOrDiscipline
        {
            Event,
            Discipline
        }

        public void PopulateMaintenanceSportsViewData(ViewDataDictionary viewData, string selectedSportId)
        {
            viewData["hpas"] = (from h in this._repository.HPAs
                                orderby h.LastName, h.FirstName
                                select new ListItem
                                    {
                                        Id = h.HPAID,
                                        Description = h.FirstName + " " + h.LastName
                                    }).ToList(); ;

            var nsos = (from o in this._repository.Organizations
                                where o.OrganizationTypeID == Constant.OrganizationTypeId.NSO
                                select new ListItem
                                {
                                    Id = o.OrganizationID,
                                    Description = _frenchLanguage ? o.Name_FR : o.Name_EN
                                });

            viewData["nsos"] = nsos.OrderBy(o => o.Description).ToList();

            var rankings = (from r in this._repository.SportRankingCategories
                            select new ListItem
                            {
                                Id = r.SportRankingCategoryID,
                                Description = r.OTPRank + r.OTPRankClass
                            });

            viewData["rankings"] = rankings.OrderBy(o => o.Description).ToList();

            viewData["sportName"] = (from s in this._repository.Sports
                                        where s.SportID == selectedSportId
                                        && s.Active
                                        select ": " + (_frenchLanguage ? s.Name_FR : s.Name_EN)).SingleOrDefault();

            viewData["genders"] = (from g in this._repository.Decodes
                                    where g.DecodeCategoryID == Constant.DecodeCategoryId.Gender
                                    orderby g.SortOrder
                                    select new ListItem
                                    {
                                        Id = g.DecodeID,
                                        Description = _frenchLanguage ? g.DecodeID_FR : g.DecodeID_EN
                                    }).ToList();

        }

        public List<MaintenanceSportsGridSportViewModel> GetMaintenanceSportsGridSportViewModel(string year, IList<ListItem> selectedSports)
        {
            List<MaintenanceSportsGridSportViewModel> model = null;

            IEnumerable<string> selectedSportIds = (from a in selectedSports select a.Id);

            model = (from s in this._repository.Sports
                     where s.SportLevelID == Constant.SportLevelId.Sport
                     && s.Active
                     && selectedSportIds.Contains(s.SportID)
                     let medalEvents = (from discipline in s.ChildSports
                                        where discipline.Active
                                        from sEvent in discipline.ChildSports
                                        where sEvent.Active
                                        && sEvent.MedalEvent != null
                                        && (bool)sEvent.MedalEvent
                                        select sEvent).Any()
                     select new MaintenanceSportsGridSportViewModel
                     {
                         SportId = s.SportID,
                         Name = _frenchLanguage ? s.Name_FR : s.Name_EN,
                         Gender = s.Gender,
                         HPA = new ListItem { Id = s.HPAID, Description = s.HPA == null ? "none" : s.HPA.FirstName + " " + s.HPA.LastName },
                         NSO = new ListItem { Id = s.OrganizationID, Description = s.OrganizationID },
                         NSOName = (from o in this._repository.Organizations
                                    where o.OrganizationID == s.OrganizationID
                                    select _frenchLanguage ? o.Name_FR : o.Name_EN).FirstOrDefault(),
                         Benchmark = s.Benchmark,
                         HasComments = !string.IsNullOrEmpty(s.Comments),
                         SportCode = s.SportCode,
                         Ranking = (from r in s.SportRankings
                                    where r.YearID == year
                                    select new ListItem
                                    {
                                        Id = r.SportRankingCategoryID,
                                        Description = r.SportRankingCategoryID
                                    }).FirstOrDefault(),
                         MedalEvent = medalEvents
                     }).OrderBy(s => s.SportCode).ToList();

            foreach (var item in model)
            {
                if (item.NSO == null)
                    continue;

                item.NSO.Description = item.NSOName;
            }

            return model;
        }

        public List<MaintenanceSportsGridDisciplineViewModel> GetMaintenanceSportsGridDisciplineViewModel(string parentSportId)
        {
            List<MaintenanceSportsGridDisciplineViewModel> model = null;

            model = (from s in this._repository.Sports
                        where s.ParentSportID == parentSportId
                        && s.Active
                        select new MaintenanceSportsGridDisciplineViewModel
                        {
                            DisciplineId = s.SportID,
                            Name_EN = s.Name_EN,
                            Name_FR = s.Name_FR,
                            Benchmark = s.Benchmark,
                            HasComments = !string.IsNullOrEmpty(s.Comments),
                            SportCode = s.SportCode,
                            Gender = s.Gender,
                            MedalEvent = (from e in s.ChildSports
                                          where e.Active
                                        && e.MedalEvent != null
                                        && (bool)e.MedalEvent
                                        select e).Any()
                        }).ToList();

            return model;
        }

        public List<MaintenanceSportsGridEventViewModel> GetMaintenanceSportsGridEventViewModel(string parentSportId)
        {
            List<MaintenanceSportsGridEventViewModel> model = null;

            model = (from s in this._repository.Sports
                        where s.ParentSportID == parentSportId
                        && s.Active
                        select new MaintenanceSportsGridEventViewModel
                        {
                            EventId = s.SportID,
                            Name_EN = s.Name_EN,
                            Name_FR = s.Name_FR,
                            SportCode = s.SportCode,
                            Benchmark = s.Benchmark,
                            TeamSize = s.TeamSize,
                            HasComments = !string.IsNullOrEmpty(s.Comments),
                            MedalEvent = s.MedalEvent == null ? false : (bool)s.MedalEvent,
                            TeamSport = s.TeamSport == null ? false : (bool)s.TeamSport,
                            ResourceTracking = s.ResourceTracking == null ? false : (bool)s.ResourceTracking
                        }).ToList();

            return model;
        }

        public List<ListItem> GetAllSportsOnSportLevel()
        {
            List<ListItem> sports = null;

            sports = (from s in this._repository.Sports
                      where s.Active
                      && s.SportLevelID == Constant.SportLevelId.Sport
                      select new ListItem
                      {
                          Id = s.SportID,
                          Description = _frenchLanguage ? s.Name_FR : s.Name_EN
                      }).OrderBy(s => s.Description).ToList();

            return sports;
        }

        public List<ListItem> GetAllSportsOnSportLevel(string id)
        {
            int resourceId = Convert.ToInt32(id);
            List<ListItem> sports = null;

            var rsp = (from rs in this._repository.ResourceSports
                        where rs.ResourceID == resourceId
                        select rs).FirstOrDefault();

            // Get the sport Id from ResourceSport table first. If nothing there, then get from Result table
            if (rsp != null)
            {
                var resourceSports = (from rs in this._repository.ResourceSports
                                        where rs.ResourceID == resourceId
                                        select rs.Sport.ParentSport.ParentSportID);

                // Get the sport Id from ResourceSport table
                sports = (from s in this._repository.Sports
                          where s.Active
                          && s.SportLevelID == Constant.SportLevelId.Sport
                          && resourceSports.Contains(s.SportID)
                          select new ListItem
                          {
                              Id = s.SportID,
                              Description = _frenchLanguage ? s.Name_FR : s.Name_EN
                          }).OrderBy(s => s.Description).ToList();

                // Add the sport Id from Result table
                var resultSports = GetSportsFromResults(resourceId);

                if (resultSports != null)
                {
                    sports.AddRange(resultSports);
                }

                // Get distinct elements and order the list
                sports = sports.GroupBy(c => c.Description).Select(g => g.First()).OrderBy(c => c.Description).ToList();
            }
            else
            {
                // Get the sport Id from Result table
                sports = GetSportsFromResults(resourceId);

                // If no result is found, then get all sport Ids.
                if (sports == null)
                {
                    sports = GetAllSportsOnSportLevel();
                }
            }

            return sports;
        }

        private List<ListItem> GetSportsFromResults(int resourceId)
        {
            List<ListItem> sports = null;

            // Check the Result table for sport Ids.
            var resultSports = (from r in this._repository.Results
                                join rr in this._repository.ResultResources
                                on r.ResultID equals rr.ResultID
                                where rr.ResourceID == resourceId
                                select r).FirstOrDefault();
                
            // If no result is found, then return null list.
            if (resultSports == null)
            {
                return sports;
            }
            else
            {
                // Get sport id level 3 from Result table.
                sports = (from s in this._repository.Sports
                            join r in this._repository.Results
                            on s.SportID equals r.SportID
                            join rr in this._repository.ResultResources
                            on r.ResultID equals rr.ResultID
                            where rr.ResourceID == resourceId
                            select new ListItem
                            {
                                Id = s.ParentSport.ParentSportID,
                                Description = _frenchLanguage ? s.ParentSport.ParentSport.Name_FR : s.ParentSport.ParentSport.Name_EN
                            }).OrderBy(a => a.Description).ToList();
            }

            return sports;
        }

        public List<ListItem> GetDisciplinesBySport(string sportId)
        {
            List<ListItem> sports = null;

            sports = (from s in this._repository.Sports
                        where s.Active
                        && s.SportLevelID == Constant.SportLevelId.Discipline
                        && s.ParentSportID == sportId
                        select new ListItem
                        {
                            Id = s.SportID,
                            Description = _frenchLanguage ? s.Name_FR : s.Name_EN
                        }).OrderBy(s => s.Description).ToList();

            if (sports == null || sports.Count == 0)
            {
                sports = new List<ListItem>();
                sports.Add(new ListItem { Id = null, Description = "Select a Sport" });
            }

            return sports;
        }

        public List<ListItem> GetEventsByDiscipline(string sportId)
        {
            List<ListItem> sports = null;

            sports = (from s in this._repository.Sports
                      where s.Active
                      && s.SportLevelID == Constant.SportLevelId.Event
                      && s.ParentSportID == sportId
                      select new ListItem
                      {
                          Id = s.SportID,
                          Description = _frenchLanguage ? s.Name_FR : s.Name_EN
                      }).OrderBy(s => s.Description).ToList();

            if (sports == null || sports.Count == 0)
            {
                sports = new List<ListItem>();
                sports.Add(new ListItem { Id = null, Description = "Select a Discipine" });
            }

            return sports;
        }

        public List<ListItem> GetResultsEventsByDiscipline(string disciplineId)
        {
            List<ListItem> sports = null;

            sports = (from s in this._repository.Sports
                      where s.Active
                      && s.SportLevelID == Constant.SportLevelId.Event
                      && s.ParentSportID == disciplineId
                      select new ListItem
                      {
                          Id = s.SportID,
                          Description = _frenchLanguage ? s.Name_FR : s.Name_EN
                      }).OrderBy(s => s.Description).ToList();

            if (sports == null || sports.Count == 0)
            {
                sports = new List<ListItem>();
                sports.Add(new ListItem { Id = null, Description = "Select a Discipine" });
            }

            return sports;
        }

        public List<int> GetResourceIdsBySport(string sportId)
        {
            return (from s in this._repository.ResourceSports
                      where s.SportID == sportId || s.SportID.StartsWith(sportId)
                      select s.ResourceID).ToList();
        }

        public SportDetail GetValidSportForDisplaySummary(string selectedSportId)
        {
            Data.Models.Sport sport = GetSport(selectedSportId);

            if (sport == null) return null;

            return new SportDetail
            {
                Id = sport.SportID,
                Name = _frenchLanguage ? sport.Name_FR : sport.Name_EN,
                Level = sport.SportLevelID
            };
        }

        public List<ListItem> GetEvaluationQuestionSportList()
        {
            List<ListItem> itemList = new List<ListItem>();

            var allSport = (from s in this._repository.Sports
                            where s.SportLevelID == Constant.SportLevelId.All
                            && s.Active
                            select s).FirstOrDefault();

            List<Sport> sportList = new List<Sport>();
            sportList.Add(allSport);

            foreach (var olympic in allSport.ChildSports.Where(s => s.SportLevelID == Constant.SportLevelId.Olympic))
            {
                sportList.Add(olympic);

                var olympicSeason = (from s in this._repository.Sports
                                     where s.SportLevelID == Constant.SportLevelId.Season
                                     && s.Active
                                     && s.ParentSportID == olympic.SportID
                                     select s).FirstOrDefault();

                sportList.Add(olympicSeason);

                foreach (var sport in olympicSeason.ChildSports.Where(s => s.SportLevelID == Constant.SportLevelId.Sport))
                {
                    sportList.Add(sport);
                }
            }

            foreach (var sport in sportList)
            {
                var suffix = "";

                for (int i = 0; i < sport.SportLevelID; i++)
                {
                    suffix += "-";
                }

                itemList.Add(new ListItem { Id = sport.SportID, Description = suffix + (_frenchLanguage ? sport.Name_FR : sport.Name_EN) });
            }

            return itemList;
        }

        public List<ListItem> GetAllSportsUnderSportLevel()
        {
            List<Sport> allSports = new List<Sport>();

            var repository = (from s in this._repository.Sports where s.Active select s).ToList();

            var sportList = (from s in repository
                                     where s.SportLevelID == Constant.SportLevelId.Sport
                                     orderby _frenchLanguage ? s.Name_FR : s.Name_EN
                                     select s).ToList();

            foreach (var sport in sportList)
            {
                allSports.Add(sport);

                var disciplineList = (from d in repository
                                      where d.SportLevelID == Constant.SportLevelId.Discipline && d.ParentSportID == sport.SportID
                                      orderby _frenchLanguage ? d.Name_FR : d.Name_EN
                                      select d).ToList();

                foreach (var discipline in disciplineList)
                {
                    allSports.Add(discipline);

                    var eventList = (from e in repository
                                     where e.SportLevelID == Constant.SportLevelId.Event && e.ParentSportID == discipline.SportID
                                     orderby _frenchLanguage ? e.Name_FR : e.Name_EN
                                     select e).ToList();

                    allSports.AddRange(eventList);
                }
            }


            List<ListItem> itemList = new List<ListItem>();

            foreach (var sport in allSports)
            {
                var suffix = "";

                for (int i = 0; i < (sport.SportLevelID - 3) * 3; i++)
                {
                    suffix += "-";
                }

                itemList.Add(new ListItem { Id = sport.SportID, Description = suffix + (_frenchLanguage ? sport.Name_FR : sport.Name_EN) });
            }

            return itemList;
        }

        private List<ListItem> GetSportsHeirachyFlattened(List<ListItem> itemList, List<Sport> childSports, int flattenToSportLevel)
        {
            if(childSports != null && childSports.Count > 0)
            {
                for (int i = 0; i < childSports.Count; i++)
                {
                    if (childSports[i].SportLevelID > flattenToSportLevel)
                    {
                        break;
                    }

                    if (i == 0
                        || childSports[i].SportLevelID == flattenToSportLevel)
                    {
                        itemList.Add(new ListItem { Id = childSports[i].SportID, Description = childSports[i].Name_EN });
                    }
                    else
                    {
                        itemList = GetSportsHeirachyFlattened(itemList, childSports[i].ChildSports.ToList(), flattenToSportLevel);
                    }
                }
            }

            return itemList;
        }

        public List<ListItem> GetReportDesignSportList(List<RingMenuItem> appliedSports)
        {
            List<ListItem> itemList = new List<ListItem>();

            var allSport = (from s in this._repository.Sports
                            where s.SportLevelID == Constant.SportLevelId.All
                            && s.Active
                            select s).FirstOrDefault();

            List<Sport> sportList = new List<Sport>();
            sportList.Add(allSport);

            foreach (var olympic in allSport.ChildSports.Where(s => s.SportLevelID == Constant.SportLevelId.Olympic))
            {
                sportList.Add(olympic);

                var olympicSeason = (from s in this._repository.Sports
                                     where s.SportLevelID == Constant.SportLevelId.Season
                                     && s.Active
                                     && s.ParentSportID == olympic.SportID
                                     select s).FirstOrDefault();

                sportList.Add(olympicSeason);

                foreach (var sport in olympicSeason.ChildSports.Where(s => s.SportLevelID == Constant.SportLevelId.Sport))
                {
                    sportList.Add(sport);
                }
            }

            var appliedSportIds = (from s in appliedSports
                                   select s.Id).ToList();

            foreach (var sport in sportList)
            {
                if (!appliedSportIds.Contains(sport.SportID))
                {
                    continue;
                }

                var suffix = "";

                for (int i = 0; i < sport.SportLevelID; i++)
                {
                    suffix += "-";
                }

                itemList.Add(new ListItem { Id = sport.SportID, Description = suffix + (_frenchLanguage ? sport.Name_FR : sport.Name_EN) });
            }

            return itemList;
        }

        public static string FormatSportName(Sport sport, bool french)
        {
            string name = "";

            if (sport.SportLevelID == Constant.SportLevelId.Discipline)
            {
                name = french
                    ? sport.ParentSport.Name_FR + " - " + sport.Name_FR
                    : sport.ParentSport.Name_EN + " - " + sport.Name_EN;
            }
            else if (sport.SportLevelID == Constant.SportLevelId.Event)
            {
                name = french
                    ? sport.ParentSport.ParentSport.Name_FR + " - " + sport.ParentSport.Name_FR + " - " + sport.Name_FR
                    : sport.ParentSport.ParentSport.Name_EN + " - " + sport.ParentSport.Name_EN + " - " + sport.Name_EN;
            }
            else
            {
                name = french ? sport.Name_FR : sport.Name_EN;
            }

            return name;
        }

        public string GetSportNameBySportID(string sportId)
        {
            return (from s in this._repository.Sports where s.SportID == sportId select _frenchLanguage ? s.Name_FR : s.Name_EN).FirstOrDefault();
        }

        public bool IfExistDescendantSportIDs(string sportId)
        {
            return (from s in this._repository.Sports where s.ParentSportID == sportId select s.SportID).Any();
        }
    }
}
