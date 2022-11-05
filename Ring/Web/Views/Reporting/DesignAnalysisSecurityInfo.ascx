<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ReportingEditableViewModel>" %>

    <div class="groupbox_title_background gradientHToWhite">
        <div class="groupbox_title" >
            <%= ViewData["reportName"] %>
        </div>
    </div>

    <% if (ViewData["reportId"] != null)
        { %>

            <script type="text/javascript">
                function onSecurityGridEdit(e)
                {
                    if (e.mode == 'insert')
                    {
                        $(e.form).find('td:last').prev()
                             .append('<a href="#" class="t-grid-action t-button t-state-default t-grid-insert"><span class="t-icon t-insert"/>Insert</a>')
                             .append('<a href="#" class="t-grid-action t-button t-state-default t-grid-cancel"><span class="t-icon t-cancel"/>Cancel</a>')
                             .find('.t-grid-delete')
                             .remove();
                    }
                }
            </script>

            <input type="hidden" id="securityUserProfileId" value="<%= ViewData["securityUserProfileId"] %>" />

            <% if (!(bool)ViewData["reportLocked"])
               { %>
                    <h3 style="color:Red">Lock report to add security.</h3>
            <% } %>

            <% Html.Telerik().Grid<ReportingDesignAnalysisSecurityGridViewModel>()
            .Name("Security")
            .TableHtmlAttributes(new { style = "font-size:8pt;" })
            .HtmlAttributes(new { style = "height:445px" })
            .DataKeys(keys => keys.Add(a => a.ReportSecurityGroupProfileID))
            .DataBinding(dataBinding => dataBinding.Ajax()
                .Select("DesignAnalysisSecurityGridAjax", "Reporting")
                .Insert("InsertReportSecurityGroupProfile", "Reporting", new { reportId = ViewData["reportId"] })
                .Delete("DeleteReportSecurityGroupProfile", "Reporting", new { ReportId = ViewData["reportId"] })
            )
            .Scrollable(scrolling => scrolling.Height(390))
            .Sortable()
            .Selectable()
            .Resizable(resize => resize.Columns(true))
            .ClientEvents(events => events.OnEdit("onSecurityGridEdit")
                                            .OnError("grid_OnError"))
            .ToolBar(t =>
            {
                if (Model.Editable && (bool)ViewData["reportLocked"])
                {
                    t.Insert().ButtonType(GridButtonType.ImageAndText).HtmlAttributes(new { @class = "SecurityInsertButton", id = "SecurityInsertButton" });
                }
            })
            .Columns(columns =>
            {
                columns.Bound(a => a.SecurityGroupProfileID)
                    .ClientTemplate("<#= SecurityGroupProfileName  #>");
                columns.Bound(a => a.HasComments)
                    .ClientTemplate("<span onclick=\"openCommentWindow('ReportSecurityGroupProfile', '<#= ReportSecurityGroupProfileID #>', '<#= SecurityGroupProfileName #>', '" + Model.Editable + "', 'true');\" class=\"comment sprite sprite-<#= HasComments ? \"note\" : \"note_none\" #>\"></span>")
                    .Width(37)
                    .Title("")
                    .Filterable(false);
                columns.Command(a =>
                {
                    if (Model.Editable)
                    {
                        a.Delete().ButtonType(GridButtonType.ImageAndText);
                    }
                })
                    .Width(95);
                columns.Bound(a => a.ReportSecurityGroupProfileID).Hidden(true);
            }).Render();
            %>

            <br />
            <br />
            <div class="clear"></div>

    <% } %>
       
