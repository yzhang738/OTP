using System;
namespace OTP.Ring.Business
{
    public interface IActionItemBL
    {
        int CreateActionItem(OTP.Ring.Common.ActionItem postedActionItem);
        System.Collections.Generic.List<OTP.Ring.Common.ActionItem> GetActionItems(string selectedStatusGrouping, string referenceId, string source, string type, int userId, System.Collections.Generic.IList<Models.ListItem> selectedSports);
        System.Collections.Generic.IEnumerable<OTP.Ring.Common.Decode> GetActionItemTypes();
        int UpdateActionItem(OTP.Ring.Common.ActionItem postedActionItem);
    }
}
