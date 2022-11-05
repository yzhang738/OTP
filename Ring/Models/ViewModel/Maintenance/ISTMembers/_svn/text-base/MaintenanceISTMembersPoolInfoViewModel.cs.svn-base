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
    [KnownType(typeof(MaintenanceISTMembersPoolInfoViewModel))]
    public class MaintenanceISTMembersPoolInfoViewModel
    {
        [Editable(false)]
        public int ResourcePoolAssignmentId { get; set; }

        [UIHint("ISTMembersGridResourceIdPoolTemplate")]
        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_ResourceIdPool")]
        public int ResourceIdPool { get; set; }

        public string ResourceIdPoolName { get; set; }

        [UIHint("ISTMembersGridResourceIdAssignmentTemplate")]
        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_ResourceIdAssignment")]
        public int ResourceIdAssignment { get; set; }

        public string ResourceIdAssignmentName { get; set; }

        [ReadOnly(true)]
        public bool HasComments { get; set; }
    }
}