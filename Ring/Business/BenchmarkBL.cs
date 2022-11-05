using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OTP.Ring.Data.Models;
using OTP.Ring.Common;
using System.Transactions;
using OTP.Ring.Models.ViewModel;
using OTP.Ring.Models;
using System.Web.Mvc;

namespace OTP.Ring.Business
{
    public class BenchmarkBL : IBenchmarkBL
    {
        private bool _frenchLanguage = false;
        private Data.Models.RingEntities _repository;

        public BenchmarkBL(string connectionString)
        {
            _frenchLanguage = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToUpper() == Constant.LanguageCode.French;
            _repository = new Data.Models.RingEntities(connectionString);
        }

        public List<MaintenanceBenchmarksGridSportViewModel> GetMaintenanceBenchmarksGridSportViewModel(string year, string selectedSport)
        {
            List<MaintenanceBenchmarksGridSportViewModel> model = null;

            bool validSport = (from s in this._repository.Sports
                                where !string.IsNullOrEmpty(selectedSport)
                                && s.SportID == selectedSport
                                && s.SportLevelID == Constant.SportLevelId.Sport
                                select s).Any();

            if (!validSport)
            {
                throw new ArgumentException("A valid sport must be provided with SportLevel = Sport", "selectedSport");
            }

            var sport = (from s in this._repository.Sports
                            where s.SportID == selectedSport
                            select s).SingleOrDefault();

            model = new List<MaintenanceBenchmarksGridSportViewModel>();

            #region Flatten the sport hierarchy

            //Sport | All | All
            if (sport.Benchmark)
            {
                model.Add(new MaintenanceBenchmarksGridSportViewModel
                {
                    Sport = new ListItem { Id = sport.SportID, Description = _frenchLanguage ? sport.Name_FR : sport.Name_EN },
                    SportId = sport.SportID,
                    Discipline = null, //All
                    Event = null, //All
                    Gender = sport.Gender,
                    BenchmarksEntered = IsBenchmarkComplete(sport, year),
                    BenchmarkLevel = _frenchLanguage ? sport.SportLevel.Name_FR : sport.SportLevel.Name_EN
                });
            }

            foreach (var discipline in sport.ChildSports)
            {
                bool firstEvent = true;
                foreach (var sEvent in discipline.ChildSports)
                {
                    if (firstEvent)
                    {
                        //Sport | Discipline | ALL
                        if (discipline.Benchmark)
                        {
                            model.Add(new MaintenanceBenchmarksGridSportViewModel
                            {
                                Sport = new ListItem { Id = sport.SportID, Description = _frenchLanguage ? sport.Name_FR : sport.Name_EN },
                                Discipline = new ListItem { Id = discipline.SportID, Description = _frenchLanguage ? discipline.Name_FR : discipline.Name_EN },
                                SportId = discipline.SportID,
                                Event = null, //All
                                Gender = string.IsNullOrEmpty(discipline.Gender) ? sport.Gender : discipline.Gender,
                                BenchmarksEntered = IsBenchmarkComplete(discipline, year),
                                BenchmarkLevel = _frenchLanguage ? discipline.SportLevel.Name_FR : discipline.SportLevel.Name_EN
                            });
                        }

                        firstEvent = false;
                    }
                    else
                    {
                        //Sport | Discipline | Event
                        if (sEvent.Benchmark)
                        {
                            model.Add(new MaintenanceBenchmarksGridSportViewModel
                            {
                                Sport = new ListItem { Id = sport.SportID, Description = _frenchLanguage ? sport.Name_FR : sport.Name_EN },
                                SportId = sEvent.SportID,
                                Discipline = new ListItem { Id = discipline.SportID, Description = _frenchLanguage ? discipline.Name_FR : discipline.Name_EN },
                                Event = new ListItem { Id = sEvent.SportID, Description = _frenchLanguage ? sEvent.Name_FR : sEvent.Name_EN },
                                Gender = string.IsNullOrEmpty(discipline.Gender) ? sport.Gender : discipline.Gender,
                                BenchmarksEntered = IsBenchmarkComplete(sEvent, year),
                                BenchmarkLevel = _frenchLanguage ? sEvent.SportLevel.Name_FR : sEvent.SportLevel.Name_EN
                            });
                        }
                    }
                }
            }

            #endregion Flatten the sport hierarchy

            return model;
        }

        private bool IsBenchmarkComplete(Sport sport, string year)
        {
            var count = (from b in sport.BenchmarkSports
                         where b.YearID == year
                         && b.Active
                         && b.NSOValue != null
                         && b.OTPValue != null
                         select b).Count();

            return count > 0;
        }

        public List<MaintenanceBenchmarksGridSportBenchmarkViewModel> GetMaintenanceBenchmarksGridSportBenchmarkViewModel(string year, string sportId)
        {
            List<MaintenanceBenchmarksGridSportBenchmarkViewModel> model = null;

            var sport = (from s in this._repository.Sports
                            where s.SportID == sportId
                            select s).SingleOrDefault();

            if (sport != null)
            {
                var benchmarks = from b in sport.BenchmarkSports
                                    where b.YearID == year
                                    && b.Active
                                    select new MaintenanceBenchmarksGridSportBenchmarkViewModel
                                    {
                                        BenchmarkSportId = b.BenchmarkSportID,
                                        BenchmarkTierDescription  = b.BenchmarkTier == null ? "?" : (_frenchLanguage ? b.BenchmarkTier.Name_FR : b.BenchmarkTier.Name_EN),
                                        BenchmarkTier = b.BenchmarkTierID,
                                        BenchmarkTypeDescription = b.BenchmarkType == null ? "?" : (_frenchLanguage ? b.BenchmarkType.Name_FR : b.BenchmarkType.Name_EN),
                                        BenchmarkType = b.BenchmarkTypeID,
                                        SportId = b.SportID,
                                        OTPValue = b.OTPValue.HasValue ? b.OTPValue.Value : 0,
                                        NSOValue = b.NSOValue.HasValue ? b.NSOValue.Value : 0,
                                        YearId = b.YearID,
                                        HasComments = !string.IsNullOrEmpty(b.Comments),
                                        Active = b.Active
                                    };

                model = (from b in benchmarks
                            orderby b.BenchmarkType, b.BenchmarkTier
                            select b).ToList();
            }

            return model;
        }

        public List<MaintenanceAthletesBenchmarkSportsViewModel> GetMaintenanceAthletesBenchmarkSportsViewModel(string year, string resourceSportId)
        {
            var id = 0;

            if (!string.IsNullOrEmpty(resourceSportId))
            {
                id = int.Parse(resourceSportId);
            }

            List<MaintenanceAthletesBenchmarkSportsViewModel> model = null;

            model = (from br in this._repository.BenchmarkResources
                        where br.ResourceSportID == id
                        && br.YearID == year
                        && br.Active
                        select new MaintenanceAthletesBenchmarkSportsViewModel
                                {
                                    Active = br.Active,
                                    BenchmarkResourceId = br.BenchmarkResourceID,
                                    BenchmarkTierId = br.BenchmarkTierID,
                                    BenchmarkTierName = _frenchLanguage ? br.BenchmarkTier.Name_FR : br.BenchmarkTier.Name_EN,
                                    BenchmarkTypeId = br.BenchmarkTypeID,
                                    BenchmarkTypeName = _frenchLanguage ? br.BenchmarkType.Name_FR : br.BenchmarkType.Name_EN,
                                    HasComments = !string.IsNullOrEmpty(br.Comments),
                                    NSOValue = br.NSOValue,
                                    OTPValue = br.OTPValue,
                                    ResourceSportId = br.ResourceSportID,
                                    YearId = br.YearID
                                }).ToList();

            return model;
        }

        public string MaintenanceBenchmarksGetValidSelectedSportId(string currentlySelectedSportId)
        {
            string validSelectedSport = null;

            if (currentlySelectedSportId != Constant.SportId.All)
            {
                validSelectedSport = (from s in this._repository.Sports
                                        where s.SportID == currentlySelectedSportId
                                        && s.SportLevelID == Constant.SportLevelId.Sport
                                        select s.SportID).FirstOrDefault();
            }

            return validSelectedSport;
        }

        public void PopulateMaintenanceBenchmarksViewData(ViewDataDictionary viewData, string selectedSportId)
        {
            var benchmarkTypes = (from b in this._repository.BenchmarkTypes
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
                                    where t.SportID == selectedSportId
                                    select new
                                    {
                                        Id = t.BenchmarkTierID,
                                        Description = _frenchLanguage ? t.Name_FR : t.Name_EN
                                    }).ToList();

            //Workaround for Linq to Sql limitation using toString()
            viewData["benchmark_tiers"] = (from t in benchmarkTiers
                                            orderby t.Description
                                            select new ListItem
                                            {
                                                Id = t.Id.ToString(),
                                                Description = t.Description
                                            }).ToList();

            viewData["sportName"] = (from s in this._repository.Sports
                                        where s.SportID == selectedSportId
                                        select ": " + (_frenchLanguage ? s.Name_FR : s.Name_EN)).SingleOrDefault();

        }

        public List<ListItem> GetBenchmarkTiersBySportId(string sportId)
        {
            List<ListItem> tiers = null;

            var benchmarkTiers = (from t in this._repository.BenchmarkTiers
                                    where t.SportID == sportId
                                    select new
                                    {
                                        Id = t.BenchmarkTierID,
                                        Description = _frenchLanguage ? t.Name_FR : t.Name_EN
                                    }).ToList();

            //Workaround for Linq to Sql limitation using toString()
            tiers = (from t in benchmarkTiers
                        orderby t.Description
                        select new ListItem
                        {
                            Id = t.Id.ToString(),
                            Description = t.Description
                        }).ToList();

            return tiers;
        }

        public int AddBenchmarkSport(
            string sportId,
            int benchmarkTypeId,
            int benchmarkTierId,
            string yearId,
            int? nsoValue,
            int? otpValue,
            string comments,
            bool active)
        {
            int changes = 0;

            var returnValue = this._repository.uspAddBenchmarkSport(
                sportId,
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

        public int UpdateBenchmarkSport(
            int benchmarkSportId,
            string sportId = Constant.UndefinedString,
            int? benchmarkTypeId = null,
            int? benchmarkTierId = null,
            string yearId = Constant.UndefinedString,
            int? nsoValue = null,
            int? otpValue = null,
            string comments = Constant.UndefinedString,
            bool? active = null)
        {
            int changes = 0;

            var benchmarkSport = (from b in this._repository.BenchmarkSports
                                    where b.BenchmarkSportID == benchmarkSportId
                                    select b).First();

            benchmarkSport.SportID = sportId == Constant.UndefinedString ? benchmarkSport.SportID : sportId;
            benchmarkSport.BenchmarkTypeID = benchmarkTypeId ?? benchmarkSport.BenchmarkTypeID;
            benchmarkSport.BenchmarkTierID = benchmarkTierId ?? benchmarkSport.BenchmarkTierID;
            benchmarkSport.YearID = yearId == Constant.UndefinedString ? benchmarkSport.YearID : yearId;
            benchmarkSport.NSOValue = nsoValue ?? benchmarkSport.NSOValue;
            benchmarkSport.OTPValue = otpValue ?? benchmarkSport.OTPValue;
            benchmarkSport.Comments = comments == Constant.UndefinedString ? benchmarkSport.Comments : comments;
            benchmarkSport.Active = active ?? benchmarkSport.Active;

            //Update Benchmark Sport
            var updateBenchmarkSportReturnValue = this._repository.uspUpdateBenchmarkSport(
                benchmarkSport.BenchmarkSportID,
                benchmarkSport.SportID,
                benchmarkSport.BenchmarkTypeID,
                benchmarkSport.BenchmarkTierID,
                benchmarkSport.YearID,
                benchmarkSport.NSOValue,
                benchmarkSport.OTPValue,
                benchmarkSport.Comments,
                benchmarkSport.Active).SingleOrDefault();

            changes = updateBenchmarkSportReturnValue == null ? changes : changes + (int)updateBenchmarkSportReturnValue;

            return changes;
        }

        public int DeleteBenchmarkSport(int benchmarkSportId)
        {
            int result = 0;

            var output = this._repository.uspDeleteBenchmarkSport(benchmarkSportId).SingleOrDefault();
            result = output == null ? result : (int)output;

            return result;
        }

        public int AddBenchmarkTier(
            string sportId,
            string name_en,
            string name_fr,
            int positionStart,
            int positionEnd,
            string comments)
        {
            int changes = 0;

            var alreadyExists = (from b in this._repository.BenchmarkTiers
                                 where b.SportID == sportId
                                 && (b.Name_EN == name_en
                                 || b.Name_FR == name_fr)
                                 select b).Any();

            if (alreadyExists)
            {
                //TODO: Localize
                throw new Exception("A benchmark for the selected Sport already exists with the name: " + name_en + " (" + name_fr + ")");
            }

            var returnValue = this._repository.uspAddBenchmarkTier(
                sportId,
                name_en,
                name_fr,
                positionStart,
                positionEnd,
                comments).SingleOrDefault();

            changes = returnValue == null ? changes : (int)returnValue;

            return changes;
        }

        public List<ListItem> GetBenchmarkTypes()
        {
            List<ListItem> benchmarkTypes = null;

            var benchmarks = (from t in this._repository.BenchmarkTypes
                                select new
                                {
                                    Id = t.BenchmarkTypeID,
                                    Description = _frenchLanguage ? t.Name_FR : t.Name_EN
                                }).ToList();

            //Workaround for Linq to Sql limitation using toString()
            benchmarkTypes = (from t in benchmarks
                                orderby t.Description
                                select new ListItem
                                {
                                    Id = t.Id.ToString(),
                                    Description = t.Description
                                }).ToList();

            return benchmarkTypes;
        }

        public List<int> GetBenchmarkSportIdsBySportYearTypeTier(string sportId, string yearId, int benchmarkTypeId, int benchmarkTierId)
        {
            return (from s in this._repository.BenchmarkSports
                    where s.SportID == sportId && s.YearID == yearId && s.BenchmarkTypeID == benchmarkTypeId && s.BenchmarkTierID == benchmarkTierId
                    select s.BenchmarkSportID).ToList();
        }
    }
}
