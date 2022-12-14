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
    [KnownType(typeof(MaintenanceAthletesBenchmarkSportsViewModel))]
    public class MaintenanceAthletesBenchmarkSportsViewModel
    {
        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_BenchmarkResourceId")]
        public int BenchmarkResourceId { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_ResourceId")]
        public string ResourceId { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_ResourceSportId")]
        public int ResourceSportId { get; set; }

        [Required(ErrorMessageResourceType = typeof(MaintenanceAthletesLocalization), ErrorMessageResourceName = "Error_Required_BenchmarkTypeId")]
        [UIHint("AthletesSportsInfoBenchmarkTypeTemplate")]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_BenchmarkTypeId")]
        public int BenchmarkTypeId { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_BenchmarkTypeName")]
        public string BenchmarkTypeName { get; set; }

        [Required(ErrorMessageResourceType = typeof(MaintenanceAthletesLocalization), ErrorMessageResourceName = "Error_Required_BenchmarkTierId")]
        [UIHint("AthletesSportsInfoBenchmarkTierTemplate")]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_BenchmarkTierId")]
        public int BenchmarkTierId { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_BenchmarkTierName")]
        public string BenchmarkTierName { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_YearId")]
        public string YearId { get; set; }

        //[DisplayName("OTP Value")]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_OTPValue")]
        public int? OTPValue { get; set; }

        //[DisplayName("NSO Value")]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_NSOValue")]
        public int? NSOValue { get; set; }

        [ReadOnly(true)]
        public bool HasComments { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_Active")]
        public bool Active { get; set; }
    }
}