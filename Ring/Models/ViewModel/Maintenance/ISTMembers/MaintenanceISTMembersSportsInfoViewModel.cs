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
    [KnownType(typeof(MaintenanceISTMembersSportsInfoViewModel))]
    public class MaintenanceISTMembersSportsInfoViewModel
    {
        [Editable(false)]
        [ExportToExcel(CellLength = 60, CellAlignment = "CENTER")]
        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_ResourceId")]
        public int ResourceId { get; set; }

        [ExportToExcel(CellLength = 60, CellAlignment = "CENTER")]
        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_ResourceSportId")]
        public int ResourceSportId { get; set; }

        [UIHint("ISTMembersSportsInfoAllSportsTemplate")]
        [Required(ErrorMessageResourceType = typeof(MaintenanceISTMembersLocalization), ErrorMessageResourceName = "Error_Required_SportsId")]
        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_SportsId")]
        public string SportsId { get; set; }

        [ExportToExcel(CellLength = 100)]
        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_SportName")]
        public string SportName { get; set; }

        [ReadOnly(true)]
        [ExportToExcel(CellLength = 60)]
        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_YearId")]
        public string YearId { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_HasComments")]
        public bool HasComments { get; set; }

        [UIHint("NullableBoolean")]
        [ExportToExcel(CellLength = 80, CellAlignment = "CENTER")]
        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_CrossCompetency")]
        public bool? CrossCompetency { get; set; }

        [UIHint("NullableBoolean")]
        [ExportToExcel(CellLength = 80, CellAlignment = "CENTER")]
        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_Primary")]
        public bool? Primary { get; set; }
    }
}