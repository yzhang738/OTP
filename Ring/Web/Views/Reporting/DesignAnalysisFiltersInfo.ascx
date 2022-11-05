<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ReportingEditableViewModel>" %>

    <div class="groupbox_title_background gradientHToWhite">
        <div class="groupbox_title" >
            <%= ViewData["reportName"] %>
        </div>
    </div>

    <% if (ViewData["reportId"] != null)
        { %>

            <%= Html.Telerik().Grid<ReportingDesignAnalysisFiltersGridViewModel>()
            .Name("Filters")
            .TableHtmlAttributes(new { style = "font-size:8pt;" })
            .HtmlAttributes(new { style = "height:445px" })
            .DataKeys(keys => keys.Add(a => a.ReportFilterId))
            .DataBinding(dataBinding => dataBinding.Ajax()
                .Select("DesignAnalysisFiltersGridAjax", "Reporting")
                .Insert("InsertReportFilter", "Reporting", new { reportId = ViewData["reportId"] })
                .Update("UpdateReportFilter", "Reporting", new { reportId = ViewData["reportId"] })
                .Delete("DeleteReportFilter", "Reporting", new { ReportId = ViewData["reportId"] })
            )
            .Scrollable(scrolling => scrolling.Height(390))
            .Sortable()
            .Selectable()
            .Resizable(resize => resize.Columns(true))
            .ClientEvents(events => events
                                            .OnError("grid_OnError"))
            .ToolBar(t =>
            {
                if (Model.Editable)
                {
                    t.Insert().ButtonType(GridButtonType.ImageAndText).HtmlAttributes(new { @class = "FilterInsertButton", id = "FilterInsertButton" });
                }
            })
            .Columns(columns =>
            {
                columns.Bound(a => a.SortOrder)
                    .Width(100);
                columns.Bound(a => a.FilterName_EN)
                    .Width(150);
                columns.Bound(a => a.FilterFieldName_EN)
                    .Width(350);
                columns.Bound(a => a.FilterName_FR)
                    .Width(150);
                columns.Bound(a => a.FilterFieldName_FR)
                    .Width(350);
                columns.Bound(a => a.ReportOperand)
                    .Width(80);
                columns.Bound(a => a.Default_EN)
                    .Width(100);
                columns.Bound(a => a.Display)
                    .ClientTemplate("<span <#= Display ? \"class='t-icon t-update'\" : \"class='t-icon t-delete'\" #>></span>")
                    .Width(80)
                    .HtmlAttributes(new { style = "text-align:center" });
                columns.Bound(a => a.HasComments)
                    .ClientTemplate("<span onclick=\"openCommentWindow('ReportFilter', '<#= ReportFilterId #>', '<#= FilterName_EN #>', '" + Model.Editable + "', 'true');\" class=\"comment sprite sprite-<#= HasComments ? \"note\" : \"note_none\" #>\"></span>")
                    .Width(37)
                    .Title("")
                    .Filterable(false);
                columns.Command(a =>
                {
                    if (Model.Editable)
                    {
                        a.Edit().ButtonType(GridButtonType.ImageAndText);
                        a.Delete().ButtonType(GridButtonType.ImageAndText);
                    }
                })
                    .Width(95);
                columns.Bound(a => a.ReportFilterId).Hidden(true);
            })
            %>

            <br />
            <br />
            <div class="clear"></div>

    <% } %>
       
