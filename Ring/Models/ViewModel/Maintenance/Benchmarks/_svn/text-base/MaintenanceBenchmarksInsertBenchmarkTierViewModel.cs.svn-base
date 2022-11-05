using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using OTP.Ring.Data.Models;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using OTP.Ring.Common;

namespace OTP.Ring.Models.ViewModel
{
    [Serializable]
    [KnownType(typeof(MaintenanceBenchmarksInsertBenchmarkTierViewModel))]
    public class MaintenanceBenchmarksInsertBenchmarkTierViewModel
    {
        [Required(ErrorMessageResourceType = typeof(MaintenanceBenchmarksLocalization), ErrorMessageResourceName = "Error_Required_Name_EN")]
        [LocalizedDisplayName(typeof(MaintenanceBenchmarksLocalization), "Title_Name_EN")]
        public string Name_EN { get; set; }

        [Required(ErrorMessageResourceType = typeof(MaintenanceBenchmarksLocalization), ErrorMessageResourceName = "Error_Required_Name_FR")]
        [LocalizedDisplayName(typeof(MaintenanceBenchmarksLocalization), "Title_Name_FR")]
        public string Name_FR { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceBenchmarksLocalization), "Title_PositionStart")]
        public int PositionStart { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceBenchmarksLocalization), "Title_PositionEnd")]
        public int PositionEnd { get; set; }
    }
}