using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using OTP.Ring.Common;

namespace OTP.Ring.Models.ViewModel
{
    [Serializable]
    [MetadataType(typeof(MaintenanceSportsGridDisciplineViewModel))]
    [KnownType(typeof(MaintenanceSportsGridDisciplineViewModel))]
    public class MaintenanceSportsGridDisciplineViewModel
    {
        [LocalizedDisplayName(typeof(MaintenanceSportsLocalization), "Title_DisciplineId")]
        public string DisciplineId { get; set; }

        [Required(ErrorMessageResourceType = typeof(MaintenanceSportsLocalization), ErrorMessageResourceName = "Error_Required_SportCode")]
        [StringLength(6, ErrorMessageResourceType = typeof(MaintenanceSportsLocalization), ErrorMessageResourceName = "Error_StringLength_SportCode")]
        [LocalizedDisplayName(typeof(MaintenanceSportsLocalization), "Title_SportCode")]
        public string SportCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(MaintenanceSportsLocalization), ErrorMessageResourceName = "Error_Required_DisciplineName_EN")]
        [LocalizedDisplayName(typeof(MaintenanceSportsLocalization), "Title_DisciplineName_EN")]
        public string Name_EN { get; set; }

        [Required(ErrorMessageResourceType = typeof(MaintenanceSportsLocalization), ErrorMessageResourceName = "Error_Required_DisciplineName_FR")]
        [LocalizedDisplayName(typeof(MaintenanceSportsLocalization), "Title_DisciplineName_FR")]
        public string Name_FR { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(MaintenanceSportsLocalization), "Title_HasComments")]
        public bool HasComments { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceSportsLocalization), "Title_Benchmark")]
        public bool Benchmark { get; set; }

        [UIHint("SportGenderPlusNoneTemplate")]
        [LocalizedDisplayName(typeof(MaintenanceSportsLocalization), "Title_Gender")]
        public string Gender { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(MaintenanceSportsLocalization), "Title_MedalEvent")]
        public bool MedalEvent { get; set; }
    }
}