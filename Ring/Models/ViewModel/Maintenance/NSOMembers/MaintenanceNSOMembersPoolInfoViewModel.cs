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
    [KnownType(typeof(MaintenanceNSOMembersPoolInfoViewModel))]
    public class MaintenanceNSOMembersPoolInfoViewModel
    {
        [Editable(false)]
        public int ResourcePoolAssignmentId { get; set; }

        [UIHint("NSOMembersGridResourceIdPoolTemplate")]
        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_ResourceIdPool")]
        public int ResourceIdPool { get; set; }

        public string ResourceIdPoolName { get; set; }

        [UIHint("NSOMembersGridResourceIdAssignmentTemplate")]
        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_ResourceIdAssignment")]
        public int ResourceIdAssignment { get; set; }

        public string ResourceIdAssignmentName { get; set; }

        [ReadOnly(true)]
        public bool HasComments { get; set; }
    }
}