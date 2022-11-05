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
    [KnownType(typeof(MaintenanceAthletesSportsInfoViewModel))]
    public class MaintenanceAthletesSportsInfoViewModel
    {
        [Editable(false)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_ResourceId")]
        public int ResourceId { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_ResourceSportId")]
        public int ResourceSportId { get; set; }

        //[Required(ErrorMessageResourceType = typeof(MaintenanceAthletesLocalization), ErrorMessageResourceName = "Error_Required_SportsId")]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_SportsId")]
        public string SportsId { get; set; }

        [UIHint("AthletesSportsInfoSportsTemplate")]
        [ExportToExcel(CellLength = 120)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_SportName")]
        public string SportName { get; set; }

        //[Required(ErrorMessageResourceType = typeof(MaintenanceAthletesLocalization), ErrorMessageResourceName = "Error_Required_DisciplineId")]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_DisciplineId")]
        public string DisciplineId { get; set; }

        [UIHint("AthletesSportsInfoDisiplinesTemplate")]
        [ExportToExcel(CellLength = 130)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_DisciplineName")]
        public string DisciplineName { get; set; }

        //[Required(ErrorMessageResourceType = typeof(MaintenanceAthletesLocalization), ErrorMessageResourceName = "Error_Required_EventId")]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_EventId")]
        public string EventId { get; set; }

        [UIHint("AthletesSportsInfoEventsTemplate")]
        [ExportToExcel(CellLength = 130)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_EventName")]
        public string EventName { get; set; }

        [UIHint("AthletesSportsInfoYearIDTemplate")]
        [ExportToExcel(CellLength = 60)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_YearId")]
        public string YearId { get; set; }

        [UIHint("AthletesSportsInfoResourceClassTempate")]
        //[Required(ErrorMessageResourceType = typeof(MaintenanceAthletesLocalization), ErrorMessageResourceName = "Error_Required_ResourceClassId")]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_ResourceClassId")]
        public string ResourceClassId { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_ResourceClassName")]
        [ExportToExcel(CellLength = 80)]
        public string ResourceClassName { get; set; }

        [UIHint("AthletesSportsInfoCoachResourcesTemplate")]
        [ExportToExcel(CellLength = 100)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_EventCoachId")]
        public int? EventCoachId { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_EventCoachName")]
        public string EventCoachName { get; set; }

        [ExportToExcel(CellLength = 70)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_SportTrackingRef")]
        public string SportTrackingRef { get; set; }

        [ReadOnly(true)]
        [ExportToExcel(CellLength = 50, CellAlignment = "CENTER")]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_Benchmark")]
        public bool Benchmark { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_HasComments")]
        public bool HasComments { get; set; }

        public string CoachLastName { get; set; }

        public string CoachFirstName { get; set; }

    }
}