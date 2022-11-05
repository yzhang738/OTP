using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using OTP.Ring.Data.Models;
using System.Collections.Generic;
using OTP.Ring.Common;

namespace OTP.Ring.Models.ViewModel
{
    [Serializable]
    [MetadataType(typeof(MaintenanceBenchmarksGridSportBenchmarkViewModel))]
    [KnownType(typeof(MaintenanceBenchmarksGridSportBenchmarkViewModel))]
    public class MaintenanceBenchmarksGridSportBenchmarkViewModel
    {
        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(MaintenanceBenchmarksLocalization), "Title_BenchmarkSportId")]
        public int BenchmarkSportId { get; set; }
        
        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(MaintenanceBenchmarksLocalization), "Title_SportId")]
        public string SportId { get; set; }

        [UIHint("BenchmarksBenchmarkTypeTemplate")]
        [LocalizedDisplayName(typeof(MaintenanceBenchmarksLocalization), "Title_BenchmarkType")]
        public int BenchmarkType { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceBenchmarksLocalization), "Title_BenchmarkTypeDescription")]
        public string BenchmarkTypeDescription { get; set; }

        [UIHint("BenchmarksBenchmarkTierTemplate")]
        [LocalizedDisplayName(typeof(MaintenanceBenchmarksLocalization), "Title_BenchmarkTier")]
        public int BenchmarkTier { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceBenchmarksLocalization), "Title_BenchmarkTierDescription")]
        public string BenchmarkTierDescription { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(MaintenanceBenchmarksLocalization), "Title_YearId")]
        public string YearId { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceBenchmarksLocalization), "Title_OTPValue")]
        public int? OTPValue { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceBenchmarksLocalization), "Title_NSOValue")]
        public int? NSOValue { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(MaintenanceBenchmarksLocalization), "Title_HasComments")]
        public bool HasComments { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(MaintenanceBenchmarksLocalization), "Title_Active")]
        public bool Active { get; set; }
    }
}