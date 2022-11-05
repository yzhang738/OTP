<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MaintenanceEditableViewModel>" %>

    <input type="hidden" id="tabResourceId" value="<%= ViewData["resourceId"] %>" />

    <script type="text/javascript">
        function onExportAthleteResourceSportToExcel() {
            var resourceId = $('#resourceId').val();

            var grid = $('#Sports').data('tGrid');
            var page = grid.currentPage;
            var pageSize = grid.pageSize;
            var orderBy = grid.orderBy
            var filter = grid.filterBy;

            var currentYear = '<%= Model.CurrentOlympicYears %>';

            var url = "/ExportExcelButton/ExportAthleteResourceSportToExcel";
            window.location.href = url + "?resourceId=" + resourceId + "&yearId=" + currentYear + "&page=" + page + "&pageSize=" + pageSize + "&orderBy=" + orderBy + "&filter=" + filter;
        }

        function restoreDefaultDeleteConfirmation() {
            var grid = $(this).data('tGrid');

            $(this).find('.t-grid-delete').click(function (e) {
                //var tr = $(this).closest('tr');

                grid.localization.deleteConfirmation = '<%: Shared.MaintenanceStrings.Maintenance_DefaultDeleteConfirmation %>';
            });
        }

    </script>

    <div class="groupbox_title_background gradientHToWhite">
        <div class="groupbox_title" >
            <%= ViewData["resourceName"] %>
        </div>
    </div>

    <% if (ViewData["resourceId"] != null)
       { %>
        <%= Html.Telerik().Grid<MaintenanceAthletesSportsInfoViewModel>()
            .Name("Sports")
            .TableHtmlAttributes(new { style = "font-size:8pt;" })
            .HtmlAttributes(new { style = "height:350px;" })
            .DataKeys(keys => keys.Add(s => s.ResourceSportId))
            .ToolBar(toolbar =>
                {
                    if (Model.Editable) toolbar.Insert().ButtonType(GridButtonType.ImageAndText);
                })
            .ToolBar(commands => commands.Custom().Text(Shared.MaintenanceStrings.ExportToExcel).HtmlAttributes(new { onclick = "onExportAthleteResourceSportToExcel();return false;" }))
            .DataBinding(dataBinding => dataBinding.Ajax()
                .Select("AthletesSportsInfoAjax", "Maintenance")
                .Insert("InsertAthletesSport", "Maintenance")
                .Update("UpdateAthletesSport", "Maintenance")
                .Delete("DeleteAthletesSport", "Maintenance")
            )
            .Pageable(paging => paging.PageSize(10))
            .Scrollable(scrolling => scrolling.Height(290))
            .Resizable(resize => resize.Columns(true))
            .Sortable()
            .Filterable()
            .Columns(columns =>
            {
                columns.Bound(s => s.SportName)
                    .Width(130);
                columns.Bound(s => s.DisciplineName)
                    .Width(140);
                columns.Bound(s => s.EventName)
                    .Width(130);
                columns.Bound(s => s.YearId)
                    .Width(100);
                columns.Bound(s => s.ResourceClassId)
                    .ClientTemplate("<#= ResourceClassName #>")
                    .Width(100);
                columns.Bound(s => s.EventCoachId)
                    .ClientTemplate("<#= EventCoachName #>")
                    .Width(110);
                columns.Bound(s => s.SportTrackingRef)
                    .Width(80);
                columns.Bound(s => s.Benchmark)
                    .ClientTemplate("<span class='<#= Benchmark ? \"t-update\" : \"t-delete\" #> t-icon'></span>")
                    .Width(80)
                    .HtmlAttributes(new { style = "text-align:center" });
                columns.Bound(s => s.HasComments)
                    .ClientTemplate("<span onclick=\"openCommentWindow('ResourceSport', '<#= ResourceSportId #>', '<#= EventName #>', '" + Model.Editable + "', 'true');\" class=\"comment sprite sprite-<#= HasComments ? \"note\" : \"note_none\" #>\"></span>")
                    .Width(40)
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
                .Width(175)
                .Title(Shared.SharedStrings.GridColumn_Command)
                .HtmlAttributes(new { style = "white-space:nowrap;float:right;" });
                columns.Bound(s => s.ResourceSportId).Hidden(true);
                columns.Bound(s => s.ResourceId).Hidden(true);
            })
            .ClientEvents(events => events.OnRowSelect("onAthleteGridRowSelect")
                                            .OnError("grid_OnError")
                                            .OnLoad("restoreDefaultDeleteConfirmation")
                                            .OnDataBound("restoreDefaultDeleteConfirmation")
                                            )
            .DetailView(ordersDetailView => ordersDetailView.ClientTemplate(
                Html.Telerik().Grid<MaintenanceAthletesBenchmarkSportsViewModel>()
                .Name("Benchmarks_<#= ResourceSportId #>")
                .HtmlAttributes(new { Style = "width:882px" })
                .DataKeys(keys => keys.Add(b => b.BenchmarkResourceId))
                .ToolBar(toolbar =>
                    {
                        if (Model.Editable) toolbar.Insert().ButtonType(GridButtonType.ImageAndText);
                    })
                .ClientEvents(e => e.OnError("grid_OnError"))
                .DataBinding(dataBinding => dataBinding.Ajax()
                    .Select("AthletesBenchmarkSportsAjax", "Maintenance", new { resourceSportId = "<#= ResourceSportId #>" })
                    .Insert("InsertAthletesBenchmark", "Maintenance", new { resourceSportId = "<#= ResourceSportId #>" })
                    .Update("UpdateAthletesBenchmark", "Maintenance", new { resourceSportId = "<#= ResourceSportId #>" })
                    .Delete("DeleteAthletesBenchmark", "Maintenance", new { resourceSportId = "<#= ResourceSportId #>" })
                )
                .Pageable()
                .Sortable()
                .Resizable(resize => resize.Columns(true))
                .Filterable()
                .Columns(columns =>
                {
                    columns.Bound(b => b.BenchmarkTypeId)
                        .ClientTemplate("<#= BenchmarkTypeName #>")
                        .Width(180);
                    columns.Bound(b => b.BenchmarkTierId)
                        .ClientTemplate("<#= BenchmarkTierName #>")
                        .Width(180);
                    columns.Bound(b => b.NSOValue)
                        .Width(180);
                    columns.Bound(b => b.OTPValue)
                        .Width(180);
                    columns.Bound(b => b.HasComments)
                        .ClientTemplate("<span onclick=\"openCommentWindow('BenchmarkResource', '<#= BenchmarkResourceId #>', '<#= BenchmarkTierName #>', '" + Model.Editable + "', 'true');\" class=\"comment sprite sprite-<#= HasComments ? \"note\" : \"note_none\" #>\"></span>")
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
                })
                .ToHtmlString()
            )
            )

                    %>

            <br />
            <br />
            <div class="clear"></div>

    <% } %>
    


           