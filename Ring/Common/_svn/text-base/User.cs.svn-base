using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OTP.Ring.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace OTP.Ring.Common
{
    [Serializable]
    public class User
    {
        [LocalizedDisplayName(typeof(CommonLocalization), "User_Title_Id")]
        public int Id { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "User_Title_LiveId")]
        public string LiveId { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "User_Title_AuthenticatedLiveId")]
        public string AuthenticatedLiveId { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "User_Title_FirstName")]
        [Required(ErrorMessageResourceType = typeof(CommonLocalization), ErrorMessageResourceName = "User_Required_FirstName")]
        public string FirstName { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "User_Title_LastName")]
        [Required(ErrorMessageResourceType = typeof(CommonLocalization), ErrorMessageResourceName = "User_Required_LastName")]
        public string LastName { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "User_Title_UserProfileId")]
        public string UserProfileId { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "User_Title_UserProfileName")]
        public string UserProfileName { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "User_Title_GroupProfileId")]
        public string GroupProfileId { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "User_Title_GroupProfileName")]
        public string GroupProfileName { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "User_Title_HomePageId")]
        public string HomePageId { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "User_Title_Comments")]
        //[Required(ErrorMessageResourceType = typeof(CommonLocalization), ErrorMessageResourceName = "User_Required_Comments")]
        public string Comments { get; set; }

        public List<Page> Pages { get; set; }
        //TODO: Remove association to Ring.Data
        public List<Sport> Sports { get; set; }

    }
}
