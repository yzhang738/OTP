using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OTP.Ring.Common;

namespace OTP.Ring.Models
{
    public static class Extentions
    {
        public static void InsertNullListItem(this List<ListItem> list, bool frenchLanguage = false, bool insertAtBeginning = true)
        {
            var nullItem = new ListItem { Id = null, Description = CommonLocalization.ListItem_NotSet };
            if (insertAtBeginning)
            {
                list.Insert(0, nullItem);
            }
            else
            {
                list.Add(nullItem);
            }
        }
    }
}
