using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTP.Ring.Data.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using OTP.Ring.Common;

namespace OTP.Ring.Models.ViewModel
{
    [Serializable]
    [KnownType(typeof(MaintenanceAthletesResultsViewModel))]
    public class MaintenanceAthletesResultsViewModel
    {
        [Editable(false)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_ResourceId")]
        public int ResourceID { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_ResultId")]
        public int ResultID { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_CompetitionId")]
        public int CompetitionID { get; set; }

        [UIHint("AthletesResultsCompetitionTemplate")]
        [ExportToExcel(CellLength = 150)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_CompetitionName")]
        public string CompetitionName { get; set; }

        [UIHint("AthletesResultsCompetitionDateTemplate")]
        [ExportToExcel(CellLength = 60)]
        //[Sort(SortOrder = ListSortDirection.Descending, SortPriority = 1)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_CompetitionDate")]
        public string CompetitionDate { get; set; }

        [UIHint("AthletesResultsLocationTemplate")]
        [ExportToExcel(CellLength = 80)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_CompetitionLocation")]
        public string Location { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_CountryName")]
        public string CountryID { get; set; }

        [UIHint("AthletesResultsCountryTemplate")]
        [ExportToExcel(CellLength = 80)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_CountryName")]
        public string CountryName { get; set; }

        //[Required(ErrorMessageResourceType = typeof(MaintenanceAthletesLocalization), ErrorMessageResourceName = "Error_Required_SportsId")]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_SportsId")]
        public string SportID { get; set; }

        [UIHint("AthletesResultsSportsTemplate")]
        [ExportToExcel(CellLength = 120)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_SportName")]
        public string SportName { get; set; }

        //[Required(ErrorMessageResourceType = typeof(MaintenanceAthletesLocalization), ErrorMessageResourceName = "Error_Required_DisciplineId")]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_DisciplineId")]
        public string DisciplineID { get; set; }

        [UIHint("AthletesResultsDisciplineTemplate")]
        [ExportToExcel(CellLength = 80)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_DisciplineName")]
        public string DisciplineName { get; set; }

        //[Required(ErrorMessageResourceType = typeof(MaintenanceAthletesLocalization), ErrorMessageResourceName = "Error_Required_EventId")]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_EventId")]
        public string EventID { get; set; }

        [UIHint("AthletesResultsEventTemplate")]
        [ExportToExcel(CellLength = 100)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_EventName")]
        public string EventName { get; set; }

        [UIHint("AthletesResultsEventDateTemplate")]
        [ExportToExcel(CellLength = 60)]
        //[Sort(SortOrder = ListSortDirection.Descending, SortPriority = 2)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_EventDate")]
        public string EventDate { get; set; }

        [UIHint("AthletesResultsGenderTemplate")]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_Gender")]
        public string Gender { get; set; }

        [ExportToExcel(CellLength = 40, CellAlignment = "CENTER")]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_Position")]
        public int Position { get; set; }

        [ExportToExcel(CellLength = 80)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_PerfValue")]
        public string PerfValue { get; set; }

        [ExportToExcel(CellLength = 80)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_PerfTime")]
        public string PerfTime { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_Comments")]
        public string Comments { get; set; }

        [ReadOnly(true)]
        public bool HasComments { get; set; }

    }
}