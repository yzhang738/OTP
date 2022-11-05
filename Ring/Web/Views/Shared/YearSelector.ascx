<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<YearSelectionViewModel>" %>

    <div>

        <div style="float:left;width:49%;text-align:right;padding:0px 0px 0px 0px;">
            <%: Shared.SharedStrings.YearSelector_ForYear %>:
        </div>
        <div style="float:right;width:50%;text-align:left;margin:0px 0px 0px 0px;">

            <%= Html.Telerik().DropDownList()
                .Name("YearSelector")
                .HtmlAttributes(new { style = "width:120px;font-size:12px" })
                .SelectedIndex(Model.SelectedYearIndex)
                .Enable(ViewData["Enable"] == null ? true : (bool)ViewData["Enable"])
                .BindTo(new SelectList(Model.OlympicYears))
                        .ClientEvents(e => e.OnChange("changeYear"))
                      
            %>

        </div> 

    </div>