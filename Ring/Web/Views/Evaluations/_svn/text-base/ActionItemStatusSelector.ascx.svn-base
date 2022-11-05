<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EvaluationActionItemViewModel>" %>

<table BORDER="0" CELLPADDING="3" CELLSPACING="3" align="center">
    <tr>
        <td align="center">
            <%: Shared.EvaluationsStrings.ActionItemStatusSelector_Label %>
        </td>

        <td >
            <%= Html.Telerik().DropDownList()
                .Name("ActionItemStatusSelector")
                                    .HtmlAttributes(new { style = "width:100px;font-size:12px;text-align:left" })
                .BindTo(new SelectList(Model.SelectorItems, "Id", "Description", Model.SelectorValue))
                .ClientEvents(e => e.OnChange("changeActionItemStatus"))
                      
            %>
        </td>
    </tr>
</table>