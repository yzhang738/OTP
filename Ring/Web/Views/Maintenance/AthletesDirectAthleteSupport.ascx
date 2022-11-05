<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MaintenanceEditableViewModel>" %>

    <input type="hidden" id="tabResourceId" value="<%= ViewData["resourceId"] %>" />

    <script type="text/javascript">
        function onExportAthleteDirectAthleteSupportToExcel() {
            var resourceId = $('#resourceId').val();

            var grid = $('#DirectAthleteSupport').data('tGrid');
            var page = grid.currentPage;
            var pageSize = grid.pageSize;
            var orderBy = grid.orderBy
            var filter = grid.filterBy;

            var url = "/ExportExcelButton/ExportAthleteDirectAthleteSupportToExcel";
            window.location.href = url + "?resourceId=" + resourceId + "&page=" + page + "&pageSize=" + pageSize + "&orderBy=" + orderBy + "&filter=" + filter;
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
       
        <%= Html.Telerik().Grid<MaintenanceAthletesDirectAthleteSupportViewModel>()
            .Name("DirectAthleteSupport")
            .TableHtmlAttributes(new { style = "font-size:8pt;" })
            .HtmlAttributes(new { style = "height:335px" })
            .DataKeys(k => k.Add(k2 => k2.ResourceDirectAthleteSupportId))
            .ToolBar(t =>
                {
                    if (Model.Editable) t.Insert().ButtonType(GridButtonType.ImageAndText);
                })
            .Editable(e => e.Mode(GridEditMode.InLine).Enabled(true))
            .ToolBar(commands => commands.Custom().Text(Shared.MaintenanceStrings.ExportToExcel).HtmlAttributes(new { onclick = "onExportAthleteDirectAthleteSupportToExcel();return false;" }))
            .ClientEvents(e => e.OnError("grid_OnError")
                    .OnLoad("restoreDefaultDeleteConfirmation")
                    .OnDataBound("restoreDefaultDeleteConfirmation")
            )
            .DataBinding(dataBinding => dataBinding.Ajax()
                .Select("AthletesDirectAthleteSupportAjax", "Maintenance")
                .Update("UpdateAthletesDirectAthleteSupport", "Maintenance")
                .Insert("InsertAthletesDirectAthleteSupport", "Maintenance")
                .Delete("DeleteAthletesDirectAthleteSupport", "Maintenance", new { ResourceId = "<#= ResourceId #>" })
            )
            .Pageable(paging => paging.PageSize(10))
                .Scrollable(scrolling => scrolling.Height(276))
            .Resizable(resize => resize.Columns(true))
            .Sortable()
            .Filterable()
            .Columns(columns =>
            {
                columns.Bound(s => s.DirectAthleteSupportTypeId)
                    .ClientTemplate("<#= DirectAthleteSupportTypeName #>")
                    .Width(130);
                columns.Bound(s => s.DirectAthleteSupportCode)
                    .ClientTemplate("<#= DirectAthleteSupportName #>")
                    .Width(150);
                columns.Bound(s => s.YearId)
                    .Width(120);
                columns.Bound(s => s.SupportName)
                    .Width(150);
                columns.Bound(s => s.Amount)
                    .Format("{0:C}")
                    .Width(75);
                columns.Bound(s => s.Description)
                    .Width(150);
                columns.Bound(s => s.SupportContact)
                    .Width(150);
                columns.Bound(s => s.SupportEmail)
                    .Width(120);
                columns.Bound(s => s.HasComments)
                        .ClientTemplate("<span onclick=\"openCommentWindow('ResourceDirectAthleteSupport', '<#= ResourceDirectAthleteSupportId #>', '<#= DirectAthleteSupportCode #>', '" + Model.Editable + "', 'true');\" class=\"comment sprite sprite-<#= HasComments ? \"note\" : \"note_none\" #>\"></span>")
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
                .Width(175)
                .Title(Shared.SharedStrings.GridColumn_Command)
                .HtmlAttributes(new { style = "white-space:nowrap;float:right;" });
                columns.Bound(s => s.ResourceDirectAthleteSupportId).Hidden(true);
                columns.Bound(s => s.ResourceId).Hidden(true);
            })

                    %>

            <br />
            <br />
            <div class="clear"></div>

    <% } %>

    


           