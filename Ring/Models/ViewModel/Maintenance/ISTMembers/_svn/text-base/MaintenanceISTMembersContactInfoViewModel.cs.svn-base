using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using OTP.Ring.Data.Models;
using OTP.Ring.Common;

namespace OTP.Ring.Models.ViewModel
{
    [Serializable]
    [KnownType(typeof(MaintenanceISTMembersContactInfoViewModel))]
    public class MaintenanceISTMembersContactInfoViewModel
    {
        public bool Editable { get; set; }

        public MaintenanceISTMembersContactInfoViewModel(Resource resource = null)
        {
            if (resource != null)
            {
                ResourceId = resource.ResourceID;
                MailingAddress = resource.MailingAddress;
                MailingCity = resource.MailingCity;
                MailingProvinceId = resource.MailingProvinceID;
                MailingCountryId = resource.MailingCountryID;
                MailingPostalCode = resource.MailingPostalCode;
                HomePhone = resource.HomePhone;
                CellPhone = resource.CellPhone;
                SkypeName = resource.SkypeName;
                EMailAddress = resource.EMailAddress;
                HometownCity = resource.HometownCity;
                HometownProvinceId = resource.HometownProvinceID;
                HometownCountryId = resource.HometownCountryID;
            }
        }

        [Editable(false)]
        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_ResourceId")]
        public int ResourceId { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_MailingAddress")]
        public string MailingAddress { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_MailingCity")]
        public string MailingCity { get; set; }

        [Editable(false)]
        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_MailingProvinceId")]
        public string MailingProvinceId { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_MailingCountryId")]
        public string MailingCountryId { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_MailingPostalCode")]
        public string MailingPostalCode { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_HomePhone")]
        public string HomePhone { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_CellPhone")]
        public string CellPhone { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_SkypeName")]
        public string SkypeName { get; set; }

        [DataType(DataType.EmailAddress)]
        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_EMailAddress")]
        public string EMailAddress { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_HometownCity")]
        public string HometownCity { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_HometownProvinceId")]
        public string HometownProvinceId { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_HometownCountryId")]
        public string HometownCountryId { get; set; }

        public List<ListItem> Countries { get; set; }

        public List<ListItem> Provinces { get; set; }
    }
}