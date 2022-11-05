using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace OTP.Ring.Common
{
    [Serializable]
    public class TeamMember
    {
        [LocalizedDisplayName(typeof(CommonLocalization), "TeamMember_Title_Id")]
        public int Id { get; set; }

        [UIHint("ManageTeamResourceTemplate")]
        [LocalizedDisplayName(typeof(CommonLocalization), "TeamMember_Title_ResourceId")]
        public int ResourceId { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "TeamMember_Title_ResourceDesc")]
        public string ResourceDesc { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "TeamMember_Title_Comment")]
        public string Comment { get; set; }
    }
}
