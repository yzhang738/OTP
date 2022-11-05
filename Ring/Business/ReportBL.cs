﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OTP.Ring.Data.Models;
using OTP.Ring.Common;
using OTP.Ring.Models;
using Microsoft.Reporting.WebForms;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using OTP.Ring.Models.ViewModel;
using System.Web.Mvc;

namespace OTP.Ring.Business
{
    public class ReportBL : IReportBL
    {
        private bool _frenchLanguage = false;
        private Data.Models.RingEntities _repository;
        private string _reportConnectionString;

        public ReportBL(string connectionString, string reportConnectionString)
        {
            _frenchLanguage = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToUpper() == Constant.LanguageCode.French;
            _repository = new Data.Models.RingEntities(connectionString);
            this._reportConnectionString = reportConnectionString;
        }

        public List<ListItem> GetReportList(string reportType, string securityGroupProfileId, string reportCategory = null)
        {
            var reportList = new List<ListItem>();

            if (reportType == Constant.ReportType.StandardReporting)
            {
                var reports = (from r in this._repository.Reports
                               where (r.ReportType == Constant.ReportType.StandardReporting || (r.ReportType == Constant.ReportType.AdHocReporting)
                               && (from s in r.ReportSecurityGroupProfiles
                                   where s.SecurityGroupProfileID == securityGroupProfileId
                                   select s).Any())
                               && (string.IsNullOrEmpty(reportCategory) || r.ReportCategory == reportCategory)
                               select new
                               {
                                   Id = r.ReportID,
                                   Description = _frenchLanguage ? r.Name_FR : r.Name_EN
                               }).ToList();

                reportList = (from r in reports
                              orderby r.Description
                              select new ListItem
                              {
                                  Id = r.Id.ToString(),
                                  Description = r.Description
                              }).ToList();
            }
            else if (reportType == Constant.ReportType.AdHocReporting)
            {
                var reports = (from r in this._repository.Reports
                               where r.ReportType == reportType
                               select new
                               {
                                   Id = r.ReportID,
                                   Description = _frenchLanguage ? r.Name_FR : r.Name_EN
                               }).ToList();

                reportList = (from r in reports
                              orderby r.Description
                              select new ListItem
                              {
                                  Id = r.Id.ToString(),
                                  Description = r.Description
                              }).ToList();
            }

            return reportList;
        }

        public List<ListItem> GetReportList(int reportId)
        {
            var reportList = new List<ListItem>();

            var reports = (from r in this._repository.Reports
                           where r.ReportID == reportId
                           select new
                           {
                               Id = r.ReportID,
                               Description = _frenchLanguage ? r.Name_FR : r.Name_EN
                           }).ToList();

            reportList = (from r in reports
                          orderby r.Description
                          select new ListItem
                          {
                              Id = r.Id.ToString(),
                              Description = r.Description
                          }).ToList();

            return reportList;
        }

        public ReportingReportParametersViewModel GetReportingReportParametersViewModel(int reportId)
        {
            var model = new ReportingReportParametersViewModel();

            var report = this._repository.Reports.Where(r => r.ReportID == reportId).SingleOrDefault();

            model.Comments = report.Comments;
            model.Description = _frenchLanguage ? report.Description_FR : report.Description_EN;
            model.Locked = report.Locked;
            model.Name = _frenchLanguage ? report.Name_FR : report.Name_EN;
            model.ReportCategory = report.ReportCategory;
            model.ReportId = model.ReportId;
            model.SecurityUserSport = report.SecurityUserSport;
            model.SportId = report.SportID;
            model.ReportViewId = report.ReportViewID;
            model.SecurityUserProfileID = report.SecurityUserProfileID;
            model.ReportType = report.ReportType;

            model.Filters = new List<ReportFilters>();

            if (report.ReportType == Constant.ReportType.StandardReporting)
            {
                var procedures = (from p in report.ReportProcedures
                                  orderby p.SortOrder
                                  where p.ProcedureType == Constant.ReportProcedureType.Filter
                                  select p).ToList();

                foreach (var proc in procedures)
                {
                    ReportFilters filter = new ReportFilters();

                    filter.ReportProcedureId = proc.ReportProcedureID;
                    filter.Label = _frenchLanguage ? proc.Name_FR : proc.Name_EN;
                    filter.SortOrder = proc.SortOrder;
                    filter.StoredProcedureName = proc.ProcedureName;
                    filter.ParentReportProcedureId = (from p in proc.ReportParameters
                                                      orderby p.SortOrder
                                                      select p.ReportProcedureIDParameter).ToList();
                    filter.ChildReportProcedureId = (from p in proc.ReportParameter0
                                                     orderby p.SortOrder
                                                     select p.ReportProcedureIDParameter).ToList();
                    //filter.ChildReportProcedureId = (from p in proc.ReportParameters1
                    //                                 select p.ReportProcedureID).FirstOrDefault();
                    SetDefaultIDValue(report.Name_EN, filter);
                    filter.Display = true;

                    model.Filters.Add(filter);
                }
            }
            else if (report.ReportType == Constant.ReportType.AdHocReporting)
            {
                var templateReport = this._repository.Reports.Where(r => r.ReportType == Constant.ReportType.Template).SingleOrDefault();

                var adhocFilters = (from f in this._repository.ReportFilters
                                    where f.ReportID == report.ReportID
                                    orderby f.SortOrder
                                    select f).ToList();

                foreach (var adhocFilter in adhocFilters)
                {
                    ReportFilters filter = new ReportFilters();

                    bool isLanguageFilter = adhocFilter.SortOrder == 0;

                    filter.ReportProcedureId = adhocFilter.SortOrder;
                    filter.Label = (_frenchLanguage ? adhocFilter.FilterName_FR : adhocFilter.FilterName_EN) + " " + adhocFilter.ReportOperand;
                    filter.SortOrder = adhocFilter.SortOrder;
                    filter.StoredProcedureName = (from p in templateReport.ReportProcedures
                                                  where p.ProcedureType == Constant.ReportProcedureType.Filter
                                                  && p.SortOrder == (isLanguageFilter ? 0 : 1) //HACK: Dirty hack.  Sort Order 0 = Language Procedure, Sort Order 1 = Adhoc Filter Procedure
                                                  select p.ProcedureName).SingleOrDefault();
                    filter.ParentReportProcedureId = isLanguageFilter ? new List<int>() : new List<int>(new int[] { 0 }); // Make every other filter a child of the language filter
                    filter.ChildReportProcedureId = isLanguageFilter ? new List<int>() : new List<int>(new int[] { 0 }); 
                    //filter.ChildReportProcedureId = 999; // Make sure Language Filter isn't a child of itself
                    filter.Display = adhocFilter.Display;
                    filter.DefaultValue = adhocFilter.Default_EN;

                    model.Filters.Add(filter);
                }
            }

            return model;
        }

        private void SetDefaultIDValue(string reportName, ReportFilters filter)
        {
            switch (reportName)
            {
                case "Athletes":
                case "Athlètes":
                    switch (filter.Label)
                    {
                        case "Language":
                            filter.DefaultID = "EN";
                            filter.DefaultValue = (from l in _repository.Languages where l.LanguageID == filter.DefaultID select l.Name).SingleOrDefault();
                            break;
                        case "Langue":
                            filter.DefaultID = "FR";
                            filter.DefaultValue = (from l in _repository.Languages where l.LanguageID == filter.DefaultID select l.Name).SingleOrDefault();
                            break;
                        case "Sport Level 1":
                        case "Niveau du Sport 1":
                            filter.DefaultID = "S";
                            filter.DefaultValue = (from s in _repository.Sports where s.SportID == filter.DefaultID select _frenchLanguage ? s.Name_FR : s.Name_EN).SingleOrDefault();
                            break;  
                        case "Sport Level 2":
                        case "Niveau du Sport 2":
                            filter.DefaultID = "S_O";
                            filter.DefaultValue = (from s in _repository.Sports where s.SportID == filter.DefaultID select _frenchLanguage ? s.Name_FR : s.Name_EN).SingleOrDefault();
                            break;
                        case "Sport Level 3":
                        case "Niveau du Sport 3":
                            filter.DefaultID = "S_O_AC_C";
                            filter.DefaultValue = (from s in _repository.Sports where s.SportID == filter.DefaultID select _frenchLanguage ? s.Name_FR : s.Name_EN).SingleOrDefault();
                            break;
                        case "Sport Level 4":
                        case "Niveau du Sport 4":
                            filter.DefaultID = "S_O_AC_M_TRA";
                            filter.DefaultValue = (from s in _repository.Sports where s.SportID == filter.DefaultID select _frenchLanguage ? s.Name_FR : s.Name_EN).SingleOrDefault();
                            break;
                    }
                    break;
                case "CSC Summary Report":
                case "Rapport Sommaire du CCM":
                    switch (filter.Label)
                    {
                        case "Language":
                            filter.DefaultID = "EN";
                            filter.DefaultValue = (from l in _repository.Languages where l.LanguageID == filter.DefaultID select l.Name).SingleOrDefault();
                            break;
                        case "Langue":
                            filter.DefaultID = "FR";
                            filter.DefaultValue = (from l in _repository.Languages where l.LanguageID == filter.DefaultID select l.Name).SingleOrDefault();
                            break;
                        case "Season":
                        case "Saison":
                            filter.DefaultID = DateTime.Now.AddYears(-1).Year.ToString() + "/" + DateTime.Now.Year.ToString();
                            filter.DefaultValue = filter.DefaultID;
                            break;
                        case "Funding Version":
                        case "Version du Financement":
                            filter.DefaultID = "Current";
                            filter.DefaultValue = (from d in _repository.Decodes where d.DecodeID == filter.DefaultID && d.DecodeCategoryID == "CSCFundingVersions" select _frenchLanguage ? d.DecodeID_FR : d.DecodeID_EN).FirstOrDefault();
                            break;
                    }
                    break;
                case "Evaluation Report By Sport":
                case "Rapport d'Évaluation Par Sport":
                    switch (filter.Label)
                    {
                        case "Language":
                            filter.DefaultID = "EN";
                            filter.DefaultValue = (from l in _repository.Languages where l.LanguageID == filter.DefaultID select l.Name).SingleOrDefault();
                            break;
                        case "Langue":
                            filter.DefaultID = "FR";
                            filter.DefaultValue = (from l in _repository.Languages where l.LanguageID == filter.DefaultID select l.Name).SingleOrDefault();
                            break;
                        case "Season":
                        case "Saison":
                            filter.DefaultID = DateTime.Now.AddYears(-1).Year.ToString() + "/" + DateTime.Now.Year.ToString();
                            filter.DefaultValue = filter.DefaultID;
                            break;
                        case "Version":
                            string yearID = DateTime.Now.AddYears(-1).Year.ToString() + "/" + DateTime.Now.Year.ToString();
                            filter.DefaultID = (from e in _repository.EvaluationVersions where e.YearID == yearID 
                                                select e.EvaluationVersionID).FirstOrDefault().ToString();
                            filter.DefaultValue = _frenchLanguage ? "ONS - Version1 (OUVERT)" : "NSO - Version1 (OPEN)";
                            break;
                        case "Sport":
                            filter.DefaultID = "S_O_CKC_C";
                            filter.DefaultValue = (from s in _repository.Sports where s.SportID == filter.DefaultID select _frenchLanguage ? s.Name_FR : s.Name_EN).SingleOrDefault();
                            break;
                        case "Detail":
                        case "Détail":
                            filter.DefaultID = "1";
                            filter.DefaultValue = (from d in _repository.Decodes where d.DecodeID == filter.DefaultID && d.DecodeCategoryID == "YESNO" select _frenchLanguage ? d.DecodeID_FR : d.DecodeID_EN).FirstOrDefault();
                            break;
                    }
                    break;
                case "Funding Recommendation Report by Funding Partner":
                case "Rapport de Recommandation du Financement Par Associé du Financement":
                    switch (filter.Label)
                    {
                        case "Language":
                            filter.DefaultID = "EN";
                            filter.DefaultValue = (from l in _repository.Languages where l.LanguageID == filter.DefaultID select l.Name).SingleOrDefault();
                            break;
                        case "Langue":
                            filter.DefaultID = "FR";
                            filter.DefaultValue = (from l in _repository.Languages where l.LanguageID == filter.DefaultID select l.Name).SingleOrDefault();
                            break;
                        case "Season":
                        case "Saison":
                            filter.DefaultID = DateTime.Now.AddYears(-1).Year.ToString() + "/" + DateTime.Now.Year.ToString();
                            filter.DefaultValue = filter.DefaultID;
                            break;
                        case "Sport":
                            filter.DefaultID = "S_O";
                            filter.DefaultValue = (from s in _repository.Sports where s.SportID == filter.DefaultID select _frenchLanguage ? s.Name_FR : s.Name_EN).SingleOrDefault();
                            break;
                        case "Funding Version":
                        case "Version du Financement":
                            filter.DefaultID = "Current";
                            filter.DefaultValue = (from d in _repository.Decodes where d.DecodeID == filter.DefaultID && d.DecodeCategoryID == "FundSummaryVersions" select _frenchLanguage ? d.DecodeID_FR : d.DecodeID_EN).FirstOrDefault();
                            break;
                        case "Team Sport":
                        case "Sport d'Équipe":
                            filter.DefaultID = "2";
                            filter.DefaultValue = (from d in _repository.Decodes where d.DecodeID == filter.DefaultID && d.DecodeCategoryID == "TeamSportSelect" select _frenchLanguage ? d.DecodeID_FR : d.DecodeID_EN).SingleOrDefault();
                            break;
                    }                    
                    break;
                case "Funding Recommendation Summary":
                case "Sommaire de Recommandation du Financement":
                    switch (filter.Label)
                    {
                        case "Language":
                            filter.DefaultID = "EN";
                            filter.DefaultValue = (from l in _repository.Languages where l.LanguageID == filter.DefaultID select l.Name).SingleOrDefault();
                            break;
                        case "Langue":
                            filter.DefaultID = "FR";
                            filter.DefaultValue = (from l in _repository.Languages where l.LanguageID == filter.DefaultID select l.Name).SingleOrDefault();
                            break;
                        case "Season":
                        case "Saison":
                            filter.DefaultID = DateTime.Now.AddYears(-1).Year.ToString() + "/" + DateTime.Now.Year.ToString();
                            filter.DefaultValue = filter.DefaultID;
                            break;
                        case "Sport":
                            filter.DefaultID = "S";
                            filter.DefaultValue = (from s in _repository.Sports where s.SportID == filter.DefaultID select _frenchLanguage ? s.Name_FR : s.Name_EN).SingleOrDefault();
                            break;
                    }      
                    break;
                case "Funding Summary for Website":
                case "Sommaire du Financement Pour Website":
                    switch (filter.Label)
                    {
                        case "Language":
                            filter.DefaultID = "EN";
                            filter.DefaultValue = (from l in _repository.Languages where l.LanguageID == filter.DefaultID select l.Name).SingleOrDefault();
                            break;
                        case "Langue":
                            filter.DefaultID = "FR";
                            filter.DefaultValue = (from l in _repository.Languages where l.LanguageID == filter.DefaultID select l.Name).SingleOrDefault();
                            break;
                        case "Season":
                        case "Saison":
                            filter.DefaultID = DateTime.Now.AddYears(-1).Year.ToString() + "/" + DateTime.Now.Year.ToString();
                            filter.DefaultValue = filter.DefaultID;
                            break;
                        case "Sport":
                            filter.DefaultID = "S";
                            filter.DefaultValue = (from s in _repository.Sports where s.SportID == filter.DefaultID select _frenchLanguage ? s.Name_FR : s.Name_EN).SingleOrDefault();
                            break;
                    }
                    break;
                case "Gold Rank Report":
                case "Rapport du Grade d'Or":
                    switch (filter.Label)
                    {
                        case "Language":
                            filter.DefaultID = "EN";
                            filter.DefaultValue = (from l in _repository.Languages where l.LanguageID == filter.DefaultID select l.Name).SingleOrDefault();
                            break;
                        case "Langue":
                            filter.DefaultID = "FR";
                            filter.DefaultValue = (from l in _repository.Languages where l.LanguageID == filter.DefaultID select l.Name).SingleOrDefault();
                            break;
                        case "Competition Year":
                        case "Année de la Compétition":
                            filter.DefaultID = DateTime.Now.AddYears(-2).Year.ToString() + "/" + DateTime.Now.AddYears(-1).Year.ToString();
                            filter.DefaultValue = filter.DefaultID;
                            break;
                        case "Competition":
                        case "Compétition":
                            filter.DefaultID = "WC";
                            filter.DefaultValue = (from c in _repository.CompetitionTypes where c.CompetitionTypeID == filter.DefaultID select _frenchLanguage ? c.Name_FR : c.Name_EN).SingleOrDefault();
                            break;
                        case "Sport":
                            filter.DefaultID = "ALL";
                            filter.DefaultValue = (from s in _repository.Sports where s.SportID == filter.DefaultID select _frenchLanguage ? s.Name_FR : s.Name_EN).SingleOrDefault();
                            break;
                        case "Previous Competition Year":
                        case "Année de la Compétition Précédent":
                            filter.DefaultID = DateTime.Now.AddYears(-3).Year.ToString() + "/" + DateTime.Now.AddYears(-2).Year.ToString();
                            filter.DefaultValue = filter.DefaultID;
                            break;
                        case "Previous Competition":
                        case "Compétition Précédent":
                            filter.DefaultID = "WC";
                            filter.DefaultValue = (from c in _repository.CompetitionTypes where c.CompetitionTypeID == filter.DefaultID select _frenchLanguage ? c.Name_FR : c.Name_EN).SingleOrDefault();
                            break;
                    }      
                    break;
                case "Results":
                case "Résultats":
                    switch (filter.Label)
                    {
                        case "Language":
                            filter.DefaultID = "EN";
                            filter.DefaultValue = (from l in _repository.Languages where l.LanguageID == filter.DefaultID select l.Name).SingleOrDefault();
                            break;
                        case "Langue":
                            filter.DefaultID = "FR";
                            filter.DefaultValue = (from l in _repository.Languages where l.LanguageID == filter.DefaultID select l.Name).SingleOrDefault();
                            break;
                        case "Year":
                        case "Année":
                            // Get the year of most recent winter olympic game
                            filter.DefaultID = (((DateTime.Now.Year - 3) / 4) * 4 + 2).ToString();
                            filter.DefaultValue = filter.DefaultID;
                            break;
                        case "Month":
                        case "Mois":
                            filter.DefaultID = "0";
                            int defaultID = Convert.ToInt32(filter.DefaultID);
                            filter.DefaultValue = (from m in _repository.Months where m.Sequence == defaultID select _frenchLanguage ? m.Name_FR : m.Name_EN).SingleOrDefault();
                            break;
                        case "Location":
                        case "Lieu":
                            filter.DefaultID = "ALL";
                            filter.DefaultValue = (from c in _repository.Countries where c.CountryID == filter.DefaultID select _frenchLanguage ? c.Name_FR : c.Name_EN).SingleOrDefault();
                            break;
                        case "Competition Type":
                        case "Genre de la Compétition":
                            filter.DefaultID = ExtensionMethods.GetConfigurationSetting("defaultCompetitionTypeID");
                            filter.DefaultValue = (from c in _repository.CompetitionTypes where c.CompetitionTypeID == filter.DefaultID select _frenchLanguage ? c.Name_FR : c.Name_EN).SingleOrDefault();
                            break;
                        case "Competition Field":
                        case "Domaine de la Compétition":
                            filter.DefaultID = "W_O_ACA_C";
                            filter.DefaultValue = (from s in _repository.Sports where s.SportID == filter.DefaultID select _frenchLanguage ? s.Name_FR : s.Name_EN).SingleOrDefault();
                            break;
                        case "Competition":
                        case "Compétition":
                            // Get the year of most recent winter olympic game
                            string winterGameYear = (((DateTime.Now.Year - 3) / 4) * 4 + 2).ToString();

                            // Get the competition Id for the game
                            string competitionType = ExtensionMethods.GetConfigurationSetting("defaultCompetitionTypeID");
                            int competitionID = (from c in _repository.Competitions where c.YearID.Contains(winterGameYear) && c.CompetitionTypeID == competitionType select c.CompetitionID).FirstOrDefault();

                            filter.DefaultID = competitionID.ToString();
                            filter.DefaultValue = (from c in _repository.Competitions where c.CompetitionID == competitionID select c.Name).FirstOrDefault();
                            break;
                        case "Discipline":
                        case "Discipliné":
                            filter.DefaultID = "W_O_ACA_M_SPD";
                            filter.DefaultValue = (from s in _repository.Sports where s.SportID == filter.DefaultID select _frenchLanguage ? s.Name_FR : s.Name_EN).SingleOrDefault();
                            break;
                        case "Event":
                        case "Épreuve":
                            filter.DefaultID = "W_O_ACA_M_SPD_DH";
                            filter.DefaultValue = (from s in _repository.Sports where s.SportID == filter.DefaultID select _frenchLanguage ? s.Name_FR : s.Name_EN).SingleOrDefault();
                            break;
                    }
                    break;
                case "Resource Funding By Sport":
                case "Financement de la Ressource Par Sport":
                    switch (filter.Label)
                    {
                        case "Language":
                            filter.DefaultID = "EN";
                            filter.DefaultValue = (from l in _repository.Languages where l.LanguageID == filter.DefaultID select l.Name).SingleOrDefault();
                            break;
                        case "Langue":
                            filter.DefaultID = "FR";
                            filter.DefaultValue = (from l in _repository.Languages where l.LanguageID == filter.DefaultID select l.Name).SingleOrDefault();
                            break;
                        case "Season":
                        case "Saison":
                            filter.DefaultID = DateTime.Now.AddYears(-1).Year.ToString() + "/" + DateTime.Now.Year.ToString();
                            filter.DefaultValue = filter.DefaultID;
                            break;
                        case "Organization":
                        case "Organisation":
                            filter.DefaultID = "NSO";
                            filter.DefaultValue = (from o in _repository.OrganizationTypes where o.OrganizationTypeID == filter.DefaultID select _frenchLanguage ? o.Name_FR : o.Name_EN).SingleOrDefault();
                            break;
                        case "Funding Summary":
                            filter.DefaultID = "374";
                            filter.DefaultValue = "Archery";
                            break;
                        case "Sommaire du Financement":
                            filter.DefaultID = "374";
                            filter.DefaultValue = "Tir à l'arc";
                            break;
                        case "Select Version":
                        case "Sélectionner Version":
                            filter.DefaultID = "Current";
                            filter.DefaultValue = (from d in _repository.Decodes where d.DecodeID == filter.DefaultID && d.DecodeCategoryID == "FundingVersions" select _frenchLanguage ? d.DecodeID_FR : d.DecodeID_EN).SingleOrDefault();
                            break;
                        case "Resource Only":
                        case "Ressource Seulement":
                            filter.DefaultID = "0";
                            filter.DefaultValue = (from d in _repository.Decodes where d.DecodeID == filter.DefaultID && d.DecodeCategoryID == "YESNO" select _frenchLanguage ? d.DecodeID_FR : d.DecodeID_EN).SingleOrDefault();
                            break;
                    }
                    break;
                case "Schedule A":
                case "Annexe A":
                    switch (filter.Label)
                    {
                        case "Language":
                            filter.DefaultID = "EN";
                            filter.DefaultValue = (from l in _repository.Languages where l.LanguageID == filter.DefaultID select l.Name).SingleOrDefault();
                            break;
                        case "Langue":
                            filter.DefaultID = "FR";
                            filter.DefaultValue = (from l in _repository.Languages where l.LanguageID == filter.DefaultID select l.Name).SingleOrDefault();
                            break;
                        case "Season":
                        case "Saison":
                            filter.DefaultID = DateTime.Now.AddYears(-1).Year.ToString() + "/" + DateTime.Now.Year.ToString();
                            filter.DefaultValue = filter.DefaultID;
                            break;
                        case "Organization": 
                        case "Organisation":
                            filter.DefaultID = "NSO";
                            filter.DefaultValue = (from o in _repository.OrganizationTypes where o.OrganizationTypeID == filter.DefaultID select _frenchLanguage ? o.Name_FR : o.Name_EN).SingleOrDefault();
                            break;
                        case "Funding Summary":
                            filter.DefaultID = "374";
                            filter.DefaultValue = "Archery";
                            break;
                        case "Sommaire du Financement":
                            filter.DefaultID = "374";
                            filter.DefaultValue = "Tir à l'arc";
                            break;
                        case "Select Version":
                        case "Sélectionner Version":
                            filter.DefaultID = "Current";
                            filter.DefaultValue = (from d in _repository.Decodes where d.DecodeID == filter.DefaultID && d.DecodeCategoryID == "FundingVersions" select _frenchLanguage ? d.DecodeID_FR : d.DecodeID_EN).SingleOrDefault();
                            break;
                        case "Recurring Expenses":
                        case "Dépenses Récurrent":
                            filter.DefaultID = "0";
                            filter.DefaultValue = (from d in _repository.Decodes where d.DecodeID == filter.DefaultID && d.DecodeCategoryID == "Recurring" select _frenchLanguage ? d.DecodeID_FR : d.DecodeID_EN).SingleOrDefault();
                            break;
                        case "Show Allocations":
                        case "Exposer Allocations":
                            filter.DefaultID = "1";
                            filter.DefaultValue = (from d in _repository.Decodes where d.DecodeID == filter.DefaultID && d.DecodeCategoryID == "YESNO" select _frenchLanguage ? d.DecodeID_FR : d.DecodeID_EN).SingleOrDefault();
                            break;
                        case "Show Indirect Funding":
                        case "Exposer Financement Indirect":
                            filter.DefaultID = "1";
                            filter.DefaultValue = (from d in _repository.Decodes where d.DecodeID == filter.DefaultID && d.DecodeCategoryID == "YESNO" select _frenchLanguage ? d.DecodeID_FR : d.DecodeID_EN).SingleOrDefault();
                            break;
                    }      
                    break;
                case "SSSM Plan":
                case "SMS Moyen":
                    switch (filter.Label)
                    {
                        case "Language":
                            filter.DefaultID = "EN";
                            filter.DefaultValue = (from l in _repository.Languages where l.LanguageID == filter.DefaultID select l.Name).SingleOrDefault();
                            break;
                        case "Langue":
                            filter.DefaultID = "FR";
                            filter.DefaultValue = (from l in _repository.Languages where l.LanguageID == filter.DefaultID select l.Name).SingleOrDefault();
                            break;
                        case "Season":
                        case "Saison":
                            filter.DefaultID = DateTime.Now.AddYears(-1).Year.ToString() + "/" + DateTime.Now.Year.ToString();
                            filter.DefaultValue = filter.DefaultID;
                            break;
                        case "Organization":
                        case "Organisation":
                            filter.DefaultID = "NSO";
                            filter.DefaultValue = (from o in _repository.OrganizationTypes where o.OrganizationTypeID == filter.DefaultID select _frenchLanguage ? o.Name_FR : o.Name_EN).SingleOrDefault();
                            break;
                        case "Funding Summary":
                            filter.DefaultID = "374";
                            filter.DefaultValue = "Archery";
                            break;
                        case "Sommaire du Financement":
                            filter.DefaultID = "374";
                            filter.DefaultValue = "Tir à l'arc";
                            break;
                        case "Select Version":
                        case "Sélectionner Version":
                            filter.DefaultID = "Current";
                            filter.DefaultValue = (from d in _repository.Decodes where d.DecodeID == filter.DefaultID && d.DecodeCategoryID == "FundingVersions" select _frenchLanguage ? d.DecodeID_FR : d.DecodeID_EN).SingleOrDefault();
                            break;
                        case "Recurring Expenses":
                        case "Dépenses Récurrent":
                            filter.DefaultID = "0";
                            filter.DefaultValue = (from d in _repository.Decodes where d.DecodeID == filter.DefaultID && d.DecodeCategoryID == "Recurring" select _frenchLanguage ? d.DecodeID_FR : d.DecodeID_EN).SingleOrDefault();
                            break;
                    }
                    break;
                case "Total Medal Rank Report":
                case "Rapport du Grade des Médailles Total":
                    switch (filter.Label)
                    {
                        case "Language":
                            filter.DefaultID = "EN";
                            filter.DefaultValue = (from l in _repository.Languages where l.LanguageID == filter.DefaultID select l.Name).SingleOrDefault();
                            break;
                        case "Langue":
                            filter.DefaultID = "FR";
                            filter.DefaultValue = (from l in _repository.Languages where l.LanguageID == filter.DefaultID select l.Name).SingleOrDefault();
                            break;
                        case "Competition Year":
                        case "Année de la Compétition":
                            filter.DefaultID = DateTime.Now.AddYears(-2).Year.ToString() + "/" + DateTime.Now.AddYears(-1).Year.ToString();
                            filter.DefaultValue = filter.DefaultID;
                            break;
                        case "Competition":
                        case "Compétition":
                            filter.DefaultID = "WC";
                            filter.DefaultValue = (from c in _repository.CompetitionTypes where c.CompetitionTypeID == filter.DefaultID select _frenchLanguage ? c.Name_FR : c.Name_EN).SingleOrDefault();
                            break;
                        case "Sport":
                            filter.DefaultID = "ALL";
                            filter.DefaultValue = (from s in _repository.Sports where s.SportID == filter.DefaultID select _frenchLanguage ? s.Name_FR : s.Name_EN).SingleOrDefault();
                            break;
                        case "Previous Competition Year":
                        case "Année de la Compétition Précédent":
                            filter.DefaultID = DateTime.Now.AddYears(-3).Year.ToString() + "/" + DateTime.Now.AddYears(-2).Year.ToString();
                            filter.DefaultValue = filter.DefaultID;
                            break;
                        case "Previous Competition":
                        case "Compétition Précédent":
                            filter.DefaultID = "WC";
                            filter.DefaultValue = (from c in _repository.CompetitionTypes where c.CompetitionTypeID == filter.DefaultID select _frenchLanguage ? c.Name_FR : c.Name_EN).SingleOrDefault();
                            break;
                    } 
                    break;
                default:
                    break;
            }
        }

        public string GetReportingReportDescription(int reportId)
        {
            var model = "";

            model = (from r in this._repository.Reports
                        where r.ReportID == reportId
                        select _frenchLanguage ? r.Description_FR : r.Description_EN).SingleOrDefault();

            return model;
        }

        public List<ListItem> GetReportFilterData(string storedProcedureName, string parameters)
        {
            var result = new List<ListItem>();
            DataSet ds = new DataSet();

            using (SqlConnection conn = new SqlConnection(this._reportConnectionString))
            {
                using (SqlCommand command = conn.CreateCommand())
                {
                    var commandText = "exec dbo." + storedProcedureName;

                    if (!string.IsNullOrEmpty(parameters))
                    {
                        List<string> paramList = parameters.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                        bool firstParam = true;
                        foreach (var param in paramList)
                        {
                            commandText += string.Format(" {0} '{1}'", firstParam ? "" : ",", param);
                            firstParam = false;
                        }
                    }

                    commandText += ";";

                    SqlDataAdapter da = new SqlDataAdapter(commandText, conn);
                    da.Fill(ds, "Table");
                }
            }

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var item = new ListItem();
                    item.Id = (String.IsNullOrEmpty(row["Value"].ToString())) ? "0" : row["Value"].ToString();
                    item.Description = row["Label"].ToString();

                    result.Add(item);
                }
            }
            else
            {
                string name = "No Selection"; 
                if (parameters.Contains("FR,"))
                {
                    name = (from d in _repository.Decodes where d.DecodeCategoryID == "NoFilterValue" select d.DecodeID_FR).SingleOrDefault().ToString();
                }
                else if (parameters.Contains("EN,"))
                {
                    name = (from d in _repository.Decodes where d.DecodeCategoryID == "NoFilterValue" select d.DecodeID_EN).SingleOrDefault().ToString();
                }

                var item = new ListItem
                {
                    Id = "0",
                    Description = name
                };

                result.Add(item);
            }

            return result;
        }

        public ReportingReportDataViewModel GetReportDocumentData(int reportId, string parameters, string securityUserId)
        {
            var result = new ReportingReportDataViewModel();
            Dictionary<int, string> parameterList = new Dictionary<int, string>();

            var isAdhocReport = (from r in this._repository.Reports
                                 where r.ReportID == reportId
                                 select r.ReportType == Constant.ReportType.AdHocReporting).SingleOrDefault();

            if (isAdhocReport)
            {
                //Add the reportId and SeurityUserID to the parameter list
                parameterList.Add(998, reportId.ToString());
                parameterList.Add(999, securityUserId);
            }

            if (!string.IsNullOrEmpty(parameters))
            {
                if (parameters.Contains("EN,"))
                {
                    _frenchLanguage = false;
                }
                else if (parameters.Contains("FR,"))
                {
                    _frenchLanguage = true;
                }

                List<string> paramList = parameters.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                for (int i = 0; i < paramList.Count; i += 2)
                {
                    parameterList.Add(int.Parse(paramList[i]), paramList[i + 1]);
                }
            }

            using (SqlConnection conn = new SqlConnection(this._reportConnectionString))
            {
                conn.Open();
                var report = new OTP.Ring.Data.Models.Report();

                if (isAdhocReport)
                {
                    report = (from r in this._repository.Reports
                              where r.ReportType == Constant.ReportType.Template
                              select r).SingleOrDefault();
                }
                else
                {
                    report = (from r in this._repository.Reports
                              where r.ReportID == reportId
                              select r).SingleOrDefault();
                }

                if (!isAdhocReport)
                {
                    var procedures = (from p in report.ReportProcedures
                                      where p.ProcedureType == Constant.ReportProcedureType.DataSet
                                      select p).ToList();

                    foreach (var proc in procedures)
                    {
                        //using (SqlCommand command = new SqlCommand(proc.ProcedureName, conn))
                        //{
                        //    command.CommandType = CommandType.StoredProcedure;
                        //    //command.CommandTimeout = 300;

                        //    foreach (var param in proc.ReportParameters.OrderBy(p => p.SortOrder))
                        //    {
                        //        command.Parameters.AddWithValue("@" + param.Comments, parameterList[param.ReportProcedureIDParameter]);
                        //    }

                        //    SqlDataAdapter da = new SqlDataAdapter();
                        //    da.SelectCommand = command;

                        //    DataSet ds = new DataSet();
                        //    da.Fill(ds, "Table");

                        //    result.DataSources.Add(new ReportDataSource(proc.DataSetRef, ds.Tables[0]));
                        //}

                        using (SqlCommand command = conn.CreateCommand())
                        {
                            var commandText = "exec dbo." + proc.ProcedureName;
                            command.CommandTimeout = 300;

                            bool firstParam = true;
                            foreach (var param in proc.ReportParameters.OrderBy(p => p.SortOrder))
                            {
                                commandText += string.Format("{0} '{1}'", firstParam ? "" : ",", parameterList[param.ReportProcedureIDParameter]);
                                firstParam = false;
                            }

                            //commandText += ";";

                            SqlDataAdapter da = new SqlDataAdapter(commandText, conn);
                            DataSet ds = new DataSet();

                            da.Fill(ds, "Table");

                            result.DataSources.Add(new ReportDataSource(proc.DataSetRef, ds.Tables[0]));
                        }
                    }
                }
                else
                {
                    var templateReport = this._repository.Reports.Where(r => r.ReportType == Constant.ReportType.Template).SingleOrDefault();

                    var procedure = (from p in templateReport.ReportProcedures
                                     where p.ProcedureType == Constant.ReportProcedureType.DataSet
                                     select p).SingleOrDefault();

                    using (SqlCommand command = conn.CreateCommand())
                    {
                        var commandText = "exec dbo." + procedure.ProcedureName;

                        bool firstParam = true;

                        foreach (var param in parameterList)
                        {
                            commandText += string.Format("{0} '{1}'", firstParam ? "" : ",", param.Value);
                            firstParam = false;
                        }

                        commandText += ";";

                        SqlDataAdapter da = new SqlDataAdapter(commandText, conn);
                        DataSet ds = new DataSet();

                        da.Fill(ds, "Table");

                        result.DataSources.Add(new ReportDataSource(procedure.DataSetRef, ds.Tables[0]));
                    }
                }

                result.DisplayName = _frenchLanguage ? report.Name_FR : report.Name_EN;
                result.ReportId = report.ReportID;
                result.ReportDefinition = report.ReportDefinition;
                //result.ReportDefinition = _frenchLanguage ? report.ReportDefinition_FR : report.ReportDefinition_EN;
            }

            return result;
        }

        public List<ListItem> GetReportViewList(int? reportId)
        {
            var reports = new List<ListItem>();

            if (reportId == null)
            {
                reports = (from r in this._repository.ReportViews
                           orderby r.Name_EN
                           select new ListItem
                           {
                               Id = r.ReportViewID,
                               Description = _frenchLanguage ? r.Name_FR : r.Name_EN
                           }).ToList();
            }
            else
            {
                reports = (from r in this._repository.Reports
                           where r.ReportID == reportId
                           select new ListItem
                           {
                               Id = r.ReportViewID,
                               Description = _frenchLanguage ? r.Description_FR : r.Description_EN
                           }).ToList();
            }

            return reports;
        }

        public List<ReportingDesignAnalysisReportGridViewModel> GetReportingDesignAnalysisReportGridViewModel(string securityUserProfileId, List<RingMenuItem> appliedSports)
        {
            var appliedSportIds = (from s in appliedSports
                                   select s.Id).ToList();

            var model = (from r in this._repository.Reports
                         where r.ReportType == Constant.ReportType.AdHocReporting
                         && (r.SportID == null || !r.SecurityUserSport || appliedSportIds.Contains(r.SportID))
                         select new ReportingDesignAnalysisReportGridViewModel
                         {
                             Editable = !r.Locked
                                && (r.SecurityUserProfileID == null
                                    || r.SecurityUserProfileID == securityUserProfileId
                                    || securityUserProfileId == Constant.SecurityUserProfile.Admin),
                             Description_EN = r.Description_EN,
                             Description_FR = r.Description_FR,
                             HasComments = !string.IsNullOrEmpty(r.Comments),
                             Locked = r.Locked,
                             Name_EN = r.Name_EN,
                             Name_FR = r.Name_FR,
                             ReportCategoryId = r.ReportCategory,
                             ReportCategoryName = (from d in this._repository.Decodes
                                                   where d.DecodeID == r.ReportCategory
                                                   select _frenchLanguage ? d.DecodeID_FR : d.DecodeID_EN).FirstOrDefault(),
                             ReportId = r.ReportID,
                             ReportViewId = r.ReportViewID,
                             ReportViewName = _frenchLanguage ? r.ReportView.Name_FR : r.ReportView.Name_EN,
                             SecurityUserProfileId = r.SecurityUserProfileID,
                             SecurityUserSport = r.SecurityUserSport,
                             SportId = r.SportID,
                             SportName = _frenchLanguage ? r.Sport.Name_FR : r.Sport.Name_EN
                         }).ToList();

            return model;
        }

        public int AddAdhocReport(
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
            string securityUserProfileId)
        {
            int changes = 0;

            var returnValue = this._repository.uspAddAdhocReport(
                name_EN,
                name_FR,
                description_EN,
                description_FR,
                locked,
                sportId,
                comments,
                reportCategory,
                securityUserSport,
                reportViewId,
                securityUserProfileId).SingleOrDefault();

            changes = returnValue == null ? changes : (int)returnValue;

            return changes;
        }

        public int UpdateAdhocReport(
            int reportId,
            string name_EN = Constant.UndefinedString,
            string name_FR = Constant.UndefinedString,
            string description_EN = Constant.UndefinedString,
            string description_FR = Constant.UndefinedString,
            bool? locked = null,
            string sportId = Constant.UndefinedString,
            string comments = Constant.UndefinedString,
            string reportCategory = Constant.UndefinedString,
            bool? securityUserSport = null)
        {
            int changes = 0;

            var report = (from r in this._repository.Reports
                          where r.ReportID == reportId
                          select r).First();

            report.Name_EN = name_EN == Constant.UndefinedString ? report.Name_EN : name_EN;
            report.Name_FR = name_FR == Constant.UndefinedString ? report.Name_FR : name_FR;
            report.Description_EN = description_EN == Constant.UndefinedString ? report.Description_EN : description_EN;
            report.Description_FR = description_FR == Constant.UndefinedString ? report.Description_FR : description_FR;
            report.Locked = locked ?? report.Locked;
            report.SportID = sportId == Constant.UndefinedString ? report.SportID : sportId;
            report.Comments = comments == Constant.UndefinedString ? report.Comments : comments;
            report.ReportCategory = reportCategory == Constant.UndefinedString ? report.ReportCategory : reportCategory;
            report.SecurityUserSport = securityUserSport ?? report.SecurityUserSport;

            var returnValue = this._repository.uspUpdateAdhocReport(
                report.ReportID,
                report.Name_EN,
                report.Name_FR,
                report.Description_EN,
                report.Description_FR,
                report.Locked,
                report.SportID,
                report.Comments,
                report.ReportCategory,
                report.SecurityUserSport).SingleOrDefault();

            changes = returnValue == null ? changes : changes + (int)returnValue;

            return changes;
        }

        public int DeleteAdhocReport(int reportId)
        {
            int changes = 0;

            var returnValue = this._repository.uspDeleteAdhocReport(reportId).SingleOrDefault();

            changes = returnValue == null ? 0 : (int)returnValue;

            return changes;
        }

        public void PopulateDesignAnalysisViewData(ViewDataDictionary viewData, string reportId)
        {
            var id = 0;

            if (!string.IsNullOrEmpty(reportId))
            {
                id = int.Parse(reportId);
            }

            viewData["reportId"] = reportId;
            
            if (id == 0)
            {
                //TODO: Localization
                viewData["reportName"] = "Select a Report";
                viewData["reportLocked"] = false;
            }
            else
            {
                var report = (from r in this._repository.Reports
                              where r.ReportID == id
                              select r).SingleOrDefault();

                viewData["reportName"] = _frenchLanguage ? report.Name_FR : report.Name_EN;
                viewData["reportLocked"] = report.Locked;
            }
        }

        public List<ReportingDesignAnalysisGroupingsGridViewModel> GetReportingDesignAnalysisGroupingsGridViewModel(int reportId, string securityUserProfileId)
        {
            var model = new List<ReportingDesignAnalysisGroupingsGridViewModel>();

            model = (from g in this._repository.ReportGroups
                     where g.ReportID == reportId
                     orderby g.ReportGroupLevel
                     select new ReportingDesignAnalysisGroupingsGridViewModel
                     {
                         Editable = !g.Report.Locked
                            && (g.Report.SecurityUserProfileID == null
                                || g.Report.SecurityUserProfileID == securityUserProfileId
                                || securityUserProfileId == Constant.SecurityUserProfile.Admin),
                         FieldName_EN = g.FieldName_EN,
                         FieldName_FR = g.FieldName_FR,
                         HasComments = !string.IsNullOrEmpty(g.Comments),
                         ReportGroupId = g.ReportGroupID,
                         ReportGroupLevel = g.ReportGroupLevel,
                         ReportId = g.ReportID
                     }).ToList();

            return model;
        }

        public int AddReportGroup(
            int reportId,
            string fieldName_EN,
            string fieldName_FR,
            string comments)
        {
            int changes = 0;

            var maxReportGroupLevel = (from g in this._repository.ReportGroups
                                       where g.ReportID == reportId
                                       select (int?)g.ReportGroupLevel).Max() ?? 0;

            //Limit to 3 groups
            if (maxReportGroupLevel == 3)
            {
                //TODO: Localization
                throw new Exception("Cannot add more than 3 Report Groups.");
            }

            var returnValue = this._repository.uspAddReportGroup(
                reportId,
                ++maxReportGroupLevel,
                fieldName_EN,
                fieldName_FR,
                comments).SingleOrDefault();

            changes = returnValue == null ? changes : (int)returnValue;

            return changes;
        }

        public int UpdateReportGroup(
            int reportGroupId,
            string fieldName_EN = Constant.UndefinedString,
            string fieldName_FR = Constant.UndefinedString,
            string comments = Constant.UndefinedString)
        {
            int changes = 0;

            var reportGroup = (from g in this._repository.ReportGroups
                               where g.ReportGroupID == reportGroupId
                               select g).First();

            reportGroup.FieldName_EN = fieldName_EN == Constant.UndefinedString ? reportGroup.FieldName_EN : fieldName_EN;
            reportGroup.FieldName_FR = fieldName_FR == Constant.UndefinedString ? reportGroup.FieldName_FR : fieldName_FR;
            reportGroup.Comments = comments == Constant.UndefinedString ? reportGroup.Comments : comments;

            var returnValue = this._repository.uspUpdateReportGroup(
                reportGroup.ReportGroupID,
                reportGroup.FieldName_EN,
                reportGroup.FieldName_FR,
                reportGroup.Comments).SingleOrDefault();

            changes = returnValue == null ? changes : changes + (int)returnValue;

            return changes;
        }

        public int DeleteReportGroup(int reportGroupId)
        {
            int changes = 0;

            var returnValue = this._repository.uspDeleteReportGroup(reportGroupId).SingleOrDefault();

            changes = returnValue == null ? 0 : (int)returnValue;

            return changes;
        }

        public bool IsReportGroupValidForDeleting(int reportGroupId)
        {
            var deletingReportGroupLevel = (from g1 in this._repository.ReportGroups
                                            where g1.ReportGroupID == reportGroupId
                                            select g1.ReportGroupLevel).SingleOrDefault();

            var maxReportGroupLevel = (from g1 in this._repository.ReportGroups
                                       from g2 in this._repository.ReportGroups
                                       where g1.ReportGroupID == reportGroupId
                                       && g1.ReportID == g2.ReportID
                                       select g2.ReportGroupLevel).Max();

            return deletingReportGroupLevel >= maxReportGroupLevel;
        }

        public List<ListItem> GetReportViewFieldsList(int reportId, bool includeNull)
        {
            var model = new List<ListItem>();

            var reportViewId = (from r in this._repository.Reports
                                where r.ReportID == reportId
                                select r.ReportViewID).SingleOrDefault();

            var fields = this._repository.uspGetReportFields(reportViewId).ToList();

            foreach (var field in fields)
            {
                model.Add(new ListItem
                    {
                        Id = field.FieldName,
                        Description = string.Format("{0} ({1})", field.FieldName, field.DataType)
                    });
            }

            if (includeNull)
            {
                model.InsertNullListItem(_frenchLanguage);
            }

            return model;
        }

        public List<ReportingDesignAnalysisGroupingColumnsGridViewModel> GetReportingDesignAnalysisGroupingColumnsGridViewModel(int reportGroupId, string securityUserProfileId)
        {
            var model = new List<ReportingDesignAnalysisGroupingColumnsGridViewModel>();

            model = (from c in this._repository.ReportColumns
                     where c.ReportGroupID == reportGroupId
                     orderby c.ReportColumnLevel
                     select new ReportingDesignAnalysisGroupingColumnsGridViewModel
                     {
                         ColumnDisplayName_EN = c.DisplayName_EN,
                         ColumnDisplayName_FR = c.DisplayName_FR,
                         Editable = !c.ReportGroup.Report.Locked
                            && (c.ReportGroup.Report.SecurityUserProfileID == null
                                || c.ReportGroup.Report.SecurityUserProfileID == securityUserProfileId
                                || securityUserProfileId == Constant.SecurityUserProfile.Admin),
                         ColumnFieldName_EN = c.FieldName_EN,
                         ColumnFieldName_FR = c.FieldName_FR,
                         FieldSize = c.FieldSize,
                         FieldSizeName = (from d in this._repository.Decodes
                                          where d.DecodeID == c.FieldSize
                                          && d.DecodeCategoryID == Constant.DecodeCategoryId.ReportFieldSize
                                          select _frenchLanguage ? d.DecodeID_FR : d.DecodeID_EN).FirstOrDefault(),
                         HasComments = !string.IsNullOrEmpty(c.Comments),
                         ReportAggregation = c.ReportAggregation,
                         ReportAggregationName = (from d in this._repository.Decodes
                                                  where d.DecodeID == c.ReportAggregation
                                                  && d.DecodeCategoryID == Constant.DecodeCategoryId.ReportAggregation
                                                  select _frenchLanguage ? d.DecodeID_FR : d.DecodeID_EN).FirstOrDefault(),
                         ReportColumnId = c.ReportColumnID,
                         ReportColumnLevel = c.ReportColumnLevel + 1, // 0 based, show 1 based to user
                         ReportGroupId = c.ReportGroupID,
                         FormatString = c.FormatString,
                         FormatStringName = (from d in this._repository.Decodes
                                             where d.DecodeID == c.FormatString
                                             && d.DecodeCategoryID == Constant.DecodeCategoryId.ReportFormatString
                                             select _frenchLanguage ? d.DecodeID_FR : d.DecodeID_EN).FirstOrDefault()
                     }).ToList();

            return model;
        }

        public int AddReportColumn(
            int reportGroupId,
            string displayName_EN,
            string displayName_FR,
            string fieldName_EN,
            string fieldName_FR,
            string reportAggregation,
            string fieldSize,
            string comments,
            string formatString)
        {
            int changes = 0;

            var maxReportColumnLevel = (from c in this._repository.ReportColumns
                                        where c.ReportGroupID == reportGroupId
                                       select (int?)c.ReportColumnLevel).Max() ?? -1;

            //Limit to 10 columns
            if (maxReportColumnLevel == 9)
            {
                //TODO: Localization
                throw new Exception("Cannot add more than 10 Report Columns.");
            }


            var returnValue = this._repository.uspAddReportColumn(
                reportGroupId,
                ++maxReportColumnLevel,
                displayName_EN,
                displayName_FR,
                fieldName_EN,
                fieldName_FR,
                reportAggregation,
                fieldSize,
                comments,
                formatString).SingleOrDefault();

            changes = returnValue == null ? changes : (int)returnValue;

            return changes;
        }

        public int UpdateReportColumn(
            int reportColumnId,
            string displayName_EN = Constant.UndefinedString,
            string displayName_FR = Constant.UndefinedString,
            string fieldName_EN = Constant.UndefinedString,
            string fieldName_FR = Constant.UndefinedString,
            string reportAggregation = Constant.UndefinedString,
            string fieldSize = Constant.UndefinedString,
            string comments = Constant.UndefinedString,
            string formatString = Constant.UndefinedString)
        {
            int changes = 0;

            var reportColumn = (from c in this._repository.ReportColumns
                               where c.ReportColumnID == reportColumnId
                               select c).First();

            reportColumn.DisplayName_EN = displayName_EN == Constant.UndefinedString ? reportColumn.DisplayName_EN : displayName_EN;
            reportColumn.DisplayName_FR = displayName_FR == Constant.UndefinedString ? reportColumn.DisplayName_FR : displayName_FR;
            reportColumn.FieldName_EN = fieldName_EN == Constant.UndefinedString ? reportColumn.FieldName_EN : fieldName_EN;
            reportColumn.FieldName_FR = fieldName_FR == Constant.UndefinedString ? reportColumn.FieldName_FR : fieldName_FR;
            reportColumn.ReportAggregation = reportAggregation == Constant.UndefinedString ? reportColumn.ReportAggregation : reportAggregation;
            reportColumn.FieldSize = fieldSize == Constant.UndefinedString ? reportColumn.FieldSize : fieldSize;
            reportColumn.Comments = comments == Constant.UndefinedString ? reportColumn.Comments : comments;
            reportColumn.FormatString = formatString == Constant.UndefinedString ? reportColumn.FormatString : formatString;

            var returnValue = this._repository.uspUpdateReportColumn(
                reportColumn.ReportColumnID,
                reportColumn.ReportColumnLevel,
                reportColumn.DisplayName_EN,
                reportColumn.DisplayName_FR,
                reportColumn.FieldName_EN,
                reportColumn.FieldName_FR,
                reportColumn.ReportAggregation,
                reportColumn.FieldSize,
                reportColumn.Comments,
                formatString).SingleOrDefault();

            changes = returnValue == null ? changes : changes + (int)returnValue;

            return changes;
        }

        public int DeleteReportColumn(int reportColumnId)
        {
            int changes = 0;

            var returnValue = this._repository.uspDeleteReportColumn(reportColumnId).SingleOrDefault();

            changes = returnValue == null ? 0 : (int)returnValue;

            return changes;
        }

        public List<ListItem> GetReportAggregationList(int reportId, string columnFieldName)
        {
            var aggregations = (from a in this._repository.uspGetValidAggregations(reportId, columnFieldName)
                                orderby a.SortOrder
                                select new ListItem
                                {
                                    Id = a.DecodeID,
                                    Description = a.DecodeID + " - " + (_frenchLanguage ? a.DecodeID_FR : a.DecodeID_EN)
                                }).ToList();

            return aggregations;
        }

        public List<ListItem> GetReportColumnFormatStringList(int reportId, string columnFieldName)
        {
            var formats = (from a in this._repository.uspGetValidFormatStrings(reportId, columnFieldName)
                                orderby a.SortOrder
                                select new ListItem
                                {
                                    Id = a.DecodeID,
                                    Description = _frenchLanguage ? a.DecodeID_FR : a.DecodeID_EN
                                }).ToList();

            formats.InsertNullListItem(_frenchLanguage);

            return formats;
        }

        public List<ReportingDesignAnalysisFiltersGridViewModel> GetReportingDesignAnalysisFiltersGridViewModel(int reportId, string securityUserProfileId)
        {
            var model = new List<ReportingDesignAnalysisFiltersGridViewModel>();

            model = (from f in this._repository.ReportFilters
                     where f.ReportID == reportId
                     && f.SortOrder != 0
                     orderby f.SortOrder
                     select new ReportingDesignAnalysisFiltersGridViewModel
                     {
                         Default_EN = f.Default_EN,
                         Display = f.Display,
                         Editable = !f.Report.Locked
                            && (f.Report.SecurityUserProfileID == null
                                || f.Report.SecurityUserProfileID == securityUserProfileId
                                || securityUserProfileId == Constant.SecurityUserProfile.Admin),
                         FilterFieldName_EN = f.FieldName_EN,
                         FilterFieldName_FR = f.FieldName_FR,
                         FilterName_EN = f.FilterName_EN,
                         FilterName_FR = f.FilterName_FR,
                         HasComments = !string.IsNullOrEmpty(f.Comments),
                         ReportFilterId = f.ReportFilterID,
                         ReportId = f.ReportID,
                         SortOrder = f.SortOrder,
                         ReportOperand = f.ReportOperand
                     }).ToList();

            return model;
        }

        public int AddReportFilter(
            int reportId,
            string filterName_EN,
            string filterName_FR,
            string fieldName_EN,
            string fieldName_FR,
            bool display,
            string default_EN,
            string reportOperand,
            string comments)
        {
            int changes = 0;

            var sortOrder = (from f in this._repository.ReportFilters
                                       where f.ReportID == reportId
                                       select (int?)f.SortOrder).Max() ?? 0;

            if (sortOrder >= 9)
            {
                throw new Exception("Cannot add more than 9 Report Filters");
            }

            var returnValue = this._repository.uspAddReportFilter(
                reportId,
                ++sortOrder,
                filterName_EN,
                filterName_FR,
                fieldName_EN,
                fieldName_FR,
                display,
                default_EN,
                comments, reportOperand).SingleOrDefault();

            changes = returnValue == null ? changes : (int)returnValue;

            return changes;
        }

        public int UpdateReportFilter(
            int reportFilterId,
            string filterName_EN = Constant.UndefinedString,
            string filterName_FR = Constant.UndefinedString,
            string fieldName_EN = Constant.UndefinedString,
            string fieldName_FR = Constant.UndefinedString,
            bool? display = null,
            string default_EN = Constant.UndefinedString,
            string reportOperand = Constant.UndefinedString,
            string comments = Constant.UndefinedString)
        {
            int changes = 0;

            var reportFilter = (from f in this._repository.ReportFilters
                                where f.ReportFilterID == reportFilterId
                                select f).First();

            reportFilter.FilterName_EN = filterName_EN == Constant.UndefinedString ? reportFilter.FilterName_EN : filterName_EN;
            reportFilter.FilterName_FR = filterName_FR == Constant.UndefinedString ? reportFilter.FilterName_FR : filterName_FR;
            reportFilter.FieldName_EN = fieldName_EN == Constant.UndefinedString ? reportFilter.FieldName_EN : fieldName_EN;
            reportFilter.FieldName_FR = fieldName_FR == Constant.UndefinedString ? reportFilter.FieldName_FR : fieldName_FR;
            reportFilter.Display = display ?? reportFilter.Display;
            reportFilter.Default_EN = default_EN == Constant.UndefinedString ? reportFilter.Default_EN : default_EN;
            reportFilter.ReportOperand = reportOperand == Constant.UndefinedString ? reportFilter.ReportOperand : reportOperand;
            reportFilter.Comments = comments == Constant.UndefinedString ? reportFilter.Comments : comments;


            var returnValue = this._repository.uspUpdateReportFilter(
                reportFilter.ReportFilterID,
                reportFilter.SortOrder,
                reportFilter.FilterName_EN,
                reportFilter.FilterName_FR,
                reportFilter.FieldName_EN,
                reportFilter.FieldName_FR,
                reportFilter.Display,
                reportFilter.Default_EN,
                reportFilter.Comments,
                reportFilter.ReportOperand).SingleOrDefault();

            changes = returnValue == null ? changes : changes + (int)returnValue;

            return changes;
        }

        public int DeleteReportFilter(int reportFilterId)
        {
            int changes = 0;

            var returnValue = this._repository.uspDeleteReportFilter(reportFilterId).SingleOrDefault();

            changes = returnValue == null ? 0 : (int)returnValue;

            return changes;
        }

        public List<ReportingDesignAnalysisSecurityGridViewModel> GetReportingDesignAnalysisSecurityGridViewModel(int reportId, string securityUserProfileId)
        {
            var model = new List<ReportingDesignAnalysisSecurityGridViewModel>();

            model = (from s in this._repository.ReportSecurityGroupProfiles
                     where s.ReportID == reportId
                     orderby s.SecurityGroupProfile.Name
                     select new ReportingDesignAnalysisSecurityGridViewModel
                     {
                         Editable = !s.Report.Locked
                             && (s.Report.SecurityUserProfileID == null
                                 || s.Report.SecurityUserProfileID == securityUserProfileId
                                 || securityUserProfileId == Constant.SecurityUserProfile.Admin),
                         HasComments = !string.IsNullOrEmpty(s.Comments),
                         ReportId = s.ReportID,
                         ReportSecurityGroupProfileID = s.ReportSecurityGroupProfileID,
                         SecurityGroupProfileID = s.SecurityGroupProfileID,
                         SecurityGroupProfileName = s.SecurityGroupProfile.Name + " (" + s.SecurityGroupProfileID + ")"
                     }).ToList();

            return model;
        }

        public int AddReportSecurityGroupProfile(
            int reportId,
            string securityGroupProfileId,
            string comments)
        {
            int changes = 0;

            var returnValue = this._repository.uspAddReportSecurityGroupProfile(
                reportId,
                securityGroupProfileId,
                comments).SingleOrDefault();

            changes = returnValue == null ? changes : (int)returnValue;

            return changes;
        }

        public int DeleteReportSecurityGroupProfile(int reportSecurityGroupProfileId)
        {
            int changes = 0;

            var returnValue = this._repository.uspDeleteReportSecurityGroupProfile(reportSecurityGroupProfileId).SingleOrDefault();

            changes = returnValue == null ? 0 : (int)returnValue;

            return changes;
        }

        public List<ListItem> GetSecurityGroupProfileList(int reportId, string securityUserProfileId)
        {
            var usedGroups = (from g in this._repository.ReportSecurityGroupProfiles
                              where g.ReportID == reportId
                              select g.SecurityGroupProfileID).ToList();

            var groupList = (from s in this._repository.SecurityGroupProfiles
                             where !usedGroups.Contains(s.SecurityGroupProfileID)
                             && (securityUserProfileId == Constant.SecurityUserProfile.Admin || s.SecurityGroupProfileID != Constant.SecurityGroupProfile.Public)
                             select new ListItem
                             {
                                 Id = s.SecurityGroupProfileID,
                                 Description = s.Name + " (" + s.SecurityGroupProfileID + ")"
                             }).ToList();

            return groupList;
        }

        public ReportingStandardReportsViewModel PopulateReportingStandardReportsViewModel(string reportCategory, string securityUserGroupProfileId)
        {
            var model = new ReportingStandardReportsViewModel();
            int selectedIndex = 0;

            model.ReportList = this.GetReportList(Constant.ReportType.StandardReporting, securityUserGroupProfileId, reportCategory);
            model.ReportCategoryList = this.GetReportCategoryList(reportCategory, out selectedIndex);
            model.ReportCategoryListSelectedIndex = selectedIndex < 0 ? 0 : selectedIndex;

            return model;
        }

        private List<ListItem> GetReportCategoryList(string reportCategory, out int selectedIndex)
        {
            selectedIndex = 0;

            var categoryList = (from d in this._repository.Decodes
                                where d.DecodeCategoryID == Constant.DecodeCategoryId.ReportCategory
                                && d.DecodeID != Constant.ReportCategory.Template
                                orderby d.SortOrder
                                select new ListItem
                                {
                                    Id = d.DecodeID,
                                    Description = _frenchLanguage ? d.DecodeID_FR : d.DecodeID_EN
                                }).ToList();

            categoryList.Insert(0, new ListItem { Id = null, Description = BusinessLocalization.ReportBL_ListItem_All });

            if (categoryList != null && categoryList.Count > 0 && !string.IsNullOrEmpty(reportCategory))
            {
                selectedIndex = categoryList.IndexOf(categoryList.Where(v => v.Id == reportCategory).SingleOrDefault());
            }

            return categoryList;
        }

        public int CopyReport(int reportId)
        {
            int changes = 0;

            var returnValue = this._repository.uspCopyReport(reportId).SingleOrDefault();
            changes = returnValue == null ? changes : (int)returnValue;

            return changes;
        }

        public bool CanUnlockReport(int reportId, out string message)
        {
            message = "";
            var canUnlock = (from r in this._repository.Reports
                             where r.ReportID == reportId
                             select !r.Locked ||
                             r.ReportSecurityGroupProfiles.Count == 0
                             ).FirstOrDefault();

            if (!canUnlock)
            {
                //TODO: Localize
                message = "The report is published.  Remove security to allow unlocking.";
            }

            return canUnlock;
        }
    }
}
