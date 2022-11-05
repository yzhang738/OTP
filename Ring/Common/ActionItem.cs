using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace OTP.Ring.Common
{
    [Serializable]
    public class ActionItem
    {
        [LocalizedDisplayName(typeof(CommonLocalization), "ActionItem_Title_Id")]
        public int Id { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "ActionItem_Title_Type")]
        public string TypeId { get; set; }        //ActionItemType.Type

        [LocalizedDisplayName(typeof(CommonLocalization), "ActionItem_Title_TypeDesc")]
        public string TypeDesc { get; set; }    //ActionItemType.Category

        [LocalizedDisplayName(typeof(CommonLocalization), "ActionItem_Title_ReferenceId")]
        public string ReferenceId { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "ActionItem_Title_OrganizationId")]
        public string OrganizationId { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "ActionItem_Title_OrganizationDesc")]
        public string OrganizationDesc { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "ActionItem_Title_SportId")]
        public string SportId { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "ActionItem_Title_SportDesc")]
        public string SportDesc { get; set; }

        [Required(ErrorMessageResourceType = typeof(CommonLocalization), ErrorMessageResourceName = "ActionItem_Required_Description")]
        [LocalizedDisplayName(typeof(CommonLocalization), "ActionItem_Title_Description")]
        public string Description { get; set; }

        // Rallen - Dec 21 2010 - Edited Date auto property to use old format
        private DateTime _dueDate;

        [DataType(DataType.Date)]
        [LocalizedDisplayName(typeof(CommonLocalization), "ActionItem_Title_DueDate")]
        public DateTime DueDate
        {
            get
            {
                return DateTime.SpecifyKind(new DateTime(_dueDate.Year, _dueDate.Month, _dueDate.Day), DateTimeKind.Utc);
            }
            set
            {
                _dueDate = DateTime.SpecifyKind(new DateTime(value.Year, value.Month, value.Day), DateTimeKind.Utc);
            }
        }

        public string DueDateLiteral
        {
            get
            {
                return DueDate.ToShortDateString();
            }
        }

        [UIHint("ActionStatus")]
        [LocalizedDisplayName(typeof(CommonLocalization), "ActionItem_Title_Status")]
        public string Status { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "ActionItem_Title_StatusDesc")]
        public string StatusDesc { get; set; }

        [UIHint("ActionPriority")]
        [LocalizedDisplayName(typeof(CommonLocalization), "ActionItem_Title_Priority")]
        public string Priority { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "ActionItem_Title_PriorityDesc")]
        public string PriorityDesc { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "ActionItem_Title_Comment")]
        public string Comment { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "ActionItem_Title_UserId")]
        public int UserId { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "ActionItem_Title_HasComment")]
        public bool HasComment { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "ActionItem_Title_ControllerName")]
        public string ControllerName { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "ActionItem_Title_ActionName")]
        public string ActionName { get; set; }

        public bool Editable { get; set; }
    }
}
