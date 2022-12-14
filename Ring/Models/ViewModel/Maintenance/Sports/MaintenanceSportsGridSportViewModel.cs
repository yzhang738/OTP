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
    [MetadataType(typeof(MaintenanceSportsGridSportViewModel))]
    [KnownType(typeof(MaintenanceSportsGridSportViewModel))]
    public class MaintenanceSportsGridSportViewModel
    {
        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(MaintenanceSportsLocalization), "Title_SportId")]
        public string SportId { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(MaintenanceSportsLocalization), "Title_SportName")]
        public string Name { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(MaintenanceSportsLocalization), "Title_SportCode")]
        public string SportCode { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(MaintenanceSportsLocalization), "Title_Gender")]
        public string Gender { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceSportsLocalization), "Title_Benchmark")]
        public bool Benchmark { get; set; }
        
        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(MaintenanceSportsLocalization), "Title_MedalEvent")]
        public bool MedalEvent { get; set; }

        [UIHint("SportHPATemplate")]
        [LocalizedDisplayName(typeof(MaintenanceSportsLocalization), "Title_HPA")]
        public ListItem HPA { get; set; }

        [UIHint("SportNSOTemplate")]
        [LocalizedDisplayName(typeof(MaintenanceSportsLocalization), "Title_NSO")]
        public ListItem NSO { get; set; }

        public string NSOName { get; set; }

        [UIHint("SportRankingTemplate")]
        [LocalizedDisplayName(typeof(MaintenanceSportsLocalization), "Title_Ranking")]
        public ListItem Ranking { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(MaintenanceSportsLocalization), "Title_HasComments")]
        public bool HasComments { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(MaintenanceSportsLocalization), "Title_Editable")]
        public bool Editable { get; set; }
    }
}