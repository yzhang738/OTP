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
    [MetadataType(typeof(MaintenanceBenchmarksGridSportViewModel))]
    [KnownType(typeof(MaintenanceBenchmarksGridSportViewModel))]
    public class MaintenanceBenchmarksGridSportViewModel
    {
        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(MaintenanceBenchmarksLocalization), "Title_Sport")]
        public ListItem Sport { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(MaintenanceBenchmarksLocalization), "Title_SportId")]
        public string SportId { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(MaintenanceBenchmarksLocalization), "Title_Discipline")]
        public ListItem Discipline { get; set; }
        
        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(MaintenanceBenchmarksLocalization), "Title_Event")]
        public ListItem Event { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceBenchmarksLocalization), "Title_BenchmarkLevel")]
        public string BenchmarkLevel { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(MaintenanceBenchmarksLocalization), "Title_Gender")]
        public string Gender { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceBenchmarksLocalization), "Title_BenchmarksEntered")]
        public bool BenchmarksEntered { get; set; }
    }
}