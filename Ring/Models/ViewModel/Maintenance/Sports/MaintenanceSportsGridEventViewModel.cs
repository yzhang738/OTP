﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using OTP.Ring.Common;

namespace OTP.Ring.Models.ViewModel
{
    [Serializable]
    public class MaintenanceSportsGridEventViewModel
    {
        [LocalizedDisplayName(typeof(MaintenanceSportsLocalization), "Title_EventId")]
        public string EventId { get; set; }

        [Required(ErrorMessageResourceType = typeof(MaintenanceSportsLocalization), ErrorMessageResourceName = "Error_Required_SportCode")]
        [StringLength(6, ErrorMessageResourceType = typeof(MaintenanceSportsLocalization), ErrorMessageResourceName = "Error_StringLength_SportCode")]
        [LocalizedDisplayName(typeof(MaintenanceSportsLocalization), "Title_SportCode")]
        public string SportCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(MaintenanceSportsLocalization), ErrorMessageResourceName = "Error_Required_EventName_EN")]
        [LocalizedDisplayName(typeof(MaintenanceSportsLocalization), "Title_EventName_EN")]
        public string Name_EN { get; set; }

        [Required(ErrorMessageResourceType = typeof(MaintenanceSportsLocalization), ErrorMessageResourceName = "Error_Required_EventName_FR")]
        [LocalizedDisplayName(typeof(MaintenanceSportsLocalization), "Title_EventName_FR")]
        public string Name_FR { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(MaintenanceSportsLocalization), "Title_HasComments")]
        public bool HasComments { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceSportsLocalization), "Title_Benchmark")]
        public bool Benchmark { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceSportsLocalization), "Title_TeamSport")]
        public bool TeamSport { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceSportsLocalization), "Title_TeamSize")]
        public int? TeamSize { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceSportsLocalization), "Title_ResourceTracking")]
        public bool ResourceTracking { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceSportsLocalization), "Title_MedalEvent")]
        public bool MedalEvent { get; set; }
    }
}