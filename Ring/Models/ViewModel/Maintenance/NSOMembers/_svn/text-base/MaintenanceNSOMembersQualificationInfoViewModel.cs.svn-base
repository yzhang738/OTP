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
    [KnownType(typeof(MaintenanceNSOMembersQualificationInfoViewModel))]
    public class MaintenanceNSOMembersQualificationInfoViewModel
    {
        [Editable(false)]
        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_ResourceId")]
        public int ResourceId { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_ResourceQualificationId")]
        public int ResourceQualificationId { get; set; }

        [Required(ErrorMessageResourceType = typeof(MaintenanceNSOMembersLocalization), ErrorMessageResourceName = "Error_Required_QualificationTypeId")]
        [UIHint("NSOMembersQualificationsInfoResourceQualificationTypeTemplate")]
        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_QualificationTypeId")]
        public string QualificationTypeId { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_QualificationTypeName")]
        public string QualificationTypeName { get; set; }

        [Required(ErrorMessageResourceType = typeof(MaintenanceNSOMembersLocalization), ErrorMessageResourceName = "Error_Required_Description")]
        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_Description")]
        public string Description { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_Qualified")]
        public bool Qualified { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_HasComments")]
        public bool HasComments { get; set; }
    }
}