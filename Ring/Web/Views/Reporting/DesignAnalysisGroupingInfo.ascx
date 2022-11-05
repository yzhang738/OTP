<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ReportingEditableViewModel>" %>

    <div class="groupbox_title_background gradientHToWhite">
        <div class="groupbox_title" >
            <%= ViewData["reportName"] %>
        </div>
    </div>

    <% if (ViewData["reportId"] != null)
        { %>

            <%= Html.Telerik().Grid<ReportingDesignAnalysisGroupingsGridViewModel>()
            .Name("Groups")
            .TableHtmlAttributes(new { style = "font-size:8pt;" })
            .HtmlAttributes(new { style = "height:445px;width:985px" })
            .DataKeys(keys => keys.Add(a => a.ReportGroupId))
            .DataBinding(dataBinding => dataBinding.Ajax()
                .Select("DesignAnalysisGroupingGridAjax", "Reporting")
                .Insert("InsertReportGroup", "Reporting", new { id = ViewData["reportId"] })
                .Update("UpdateReportGroup", "Reporting", new { reportId = ViewData["reportId"] })
                .Delete("DeleteReportGroup", "Reporting", new { reportId = ViewData["reportId"] })
            )
            .Scrollable(scrolling => scrolling.Height(390))
            .Sortable()
            .Selectable()
            .Resizable(resize => resize.Columns(true))
            .ClientEvents(events => events
                                            .OnError("grid_OnError"))
            .ToolBar(t =>
            {
                if (Model.Editable) t.Insert().ButtonType(GridButtonType.ImageAndText).HtmlAttributes(new { @class = "GroupInsertButton", id = "GroupInsertButton" });
            })
            .Columns(columns =>
            {
                columns.Bound(a => a.ReportGroupLevel)
                    .Width(95);
                columns.Bound(a => a.FieldName_EN)
                    .Width(340);
                columns.Bound(a => a.FieldName_FR)
                    .Width(340);
                columns.Bound(a => a.HasComments)
                    .ClientTemplate("<span onclick=\"openCommentWindow('ReportGroup', '<#= ReportGroupId #>', '<#= FieldName_EN #>', '" + Model.Editable + "', 'true');\" class=\"comment sprite sprite-<#= HasComments ? \"note\" : \"note_none\" #>\"></span>")
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
                columns.Template(c => string.Empty).Width(865);
                columns.Bound(a => a.ReportGroupId).Hidden(true);
            })
            .DetailView(v => v.ClientTemplate(
                Html.Telerik().Grid<ReportingDesignAnalysisGroupingColumnsGridViewModel>()
                .Name("Columns_<#= ReportGroupId #>")
                .DataKeys(keys => keys.Add(b => b.ReportColumnId))
                .ToolBar(toolbar =>
                    {
                        if (Model.Editable)
                        {
                            toolbar.Insert().ButtonType(GridButtonType.ImageAndText);
                        }
                    })
                .ClientEvents(e => e.OnError("grid_OnError"))
                .DataBinding(dataBinding => dataBinding.Ajax()
                    .Select("DesignAnalysisGroupColumnsGridAjax", "Reporting", new { reportGroupId = "<#= ReportGroupId #>" })
                    .Insert("InsertReportColumn", "Reporting", new { reportGroupId = "<#= ReportGroupId #>" })
                    .Update("UpdateReportColumn", "Reporting", new { reportGroupId = "<#= ReportGroupId #>" })
                    .Delete("DeleteReportColumn", "Reporting", new { reportGroupId = "<#= ReportGroupId #>" })
                )
                .Pageable()
                .Sortable()
                .Resizable(resize => resize.Columns(true))
                .Filterable()
                .Columns(columns =>
                {
                    columns.Bound(b => b.ReportColumnLevel)
                        .Width(75);
                    columns.Bound(b => b.ColumnFieldName_EN)
                        .Width(350);
                    columns.Bound(b => b.ColumnDisplayName_EN)
                        .Width(150);
                    columns.Bound(b => b.ColumnFieldName_FR)
                        .Width(350);
                    columns.Bound(b => b.ColumnDisplayName_FR)
                        .Width(150);
                    columns.Bound(b => b.FieldSize)
                        .ClientTemplate("<#= FieldSizeName #>")
                        .Width(125);
                    columns.Bound(b => b.ReportAggregation)
                        .ClientTemplate("<#= ReportAggregationName #>")
                        .Width(300);
                    columns.Bound(b => b.FormatString)
                        .ClientTemplate("<#= FormatStringName #>")
                        .Width(125);
                    columns.Bound(b => b.HasComments)
                        .ClientTemplate("<span onclick=\"openCommentWindow('ReportColumn', '<#= ReportColumnId #>', '<#= ColumnDisplayName_EN #>', '" + Model.Editable + "', 'true');\" class=\"comment sprite sprite-<#= HasComments ? \"note\" : \"note_none\" #>\"></span>")
                        .Width(37)
                        .Title("")
                        .Filterable(false);
                    columns.Command(commands =>
                    {
                        if (Model.Editable)
                        {
                            commands.Edit().ButtonType(GridButtonType.ImageAndText);
                            commands.Delete().ButtonType(GridButtonType.ImageAndText);
                        }
                    })
                        .Width(95);
                    columns.Bound(b => b.ReportColumnId).Hidden(true);
                    columns.Bound(b => b.ReportGroupId).Hidden(true);
                })
                .ToHtmlString()
            ))    
            %>

            <br />
            <br />
            <div class="clear"></div>

    <% } %>
       
