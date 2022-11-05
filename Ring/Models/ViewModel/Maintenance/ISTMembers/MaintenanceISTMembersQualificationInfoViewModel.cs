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
    [KnownType(typeof(MaintenanceISTMembersQualificationInfoViewModel))]
    public class MaintenanceISTMembersQualificationInfoViewModel
    {
        [Editable(false)]
        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_ResourceId")]
        public int ResourceId { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_ResourceQualificationId")]
        public int ResourceQualificationId { get; set; }

        [Required(ErrorMessageResourceType = typeof(MaintenanceISTMembersLocalization), ErrorMessageResourceName = "Error_Required_QualificationTypeId")]
        [UIHint("ISTMembersQualificationsInfoResourceQualificationTypeTemplate")]
        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_QualificationTypeId")]
        public string QualificationTypeId { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_QualificationTypeName")]
        public string QualificationTypeName { get; set; }

        [Required(ErrorMessageResourceType = typeof(MaintenanceISTMembersLocalization), ErrorMessageResourceName = "Error_Required_Description")]
        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_Description")]
        public string Description { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_Qualified")]
        public bool Qualified { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_HasComments")]
        public bool HasComments { get; set; }
    }
}