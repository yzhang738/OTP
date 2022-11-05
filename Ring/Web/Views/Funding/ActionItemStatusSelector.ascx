<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<FundingActionItemViewModel>" %>

<table BORDER="0" CELLPADDING="3" CELLSPACING="3" align="center">
    <tr>
        <td align="center">
            <%: Shared.FundingStrings.ActionItemStatusSelector_Status %>
        </td>

        <td align="center">
            <%= Html.Telerik().DropDownList()
                .Name("ActionItemStatusSelector")
                    .HtmlAttributes(new { style = "width:100px;text-align:left;font-size:12px" })
                .BindTo(new SelectList(Model.SelectorItems, "Id", "Description", Model.SelectorValue))
                .ClientEvents(e => e.OnChange("changeActionItemStatus"))
                      
            %>
        </td>
    </tr>
</table>