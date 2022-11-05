<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MaintenanceEditableViewModel>" %>

<input type="hidden" id="tabResourceId" value="<%= ViewData["resourceId"] %>" />

    <script type="text/javascript">
        function onExportISTMembersSportInfoToExcel() {
            var resourceId = $('#resourceId').val();

            var grid = $('#Sports').data('tGrid');
            var page = grid.currentPage;
            var pageSize = grid.pageSize;
            var orderBy = grid.orderBy
            var filter = grid.filterBy;

            var url = "/ExportExcelButton/ExportISTMembersSportInfoToExcel";
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

        <%= Html.Telerik().Grid<MaintenanceISTMembersSportsInfoViewModel>()
            .Name("Sports")
            .TableHtmlAttributes(new { style = "font-size:8pt;" })
            .HtmlAttributes(new { style = "height:335px" })
            .DataKeys(keys => keys.Add(s => s.ResourceSportId))
            .ToolBar(toolbar =>
                {
                    if (Model.Editable) toolbar.Insert().ButtonType(GridButtonType.ImageAndText);
                })
            .ToolBar(commands => commands.Custom().Text(Shared.MaintenanceStrings.ExportToExcel).HtmlAttributes(new { onclick = "onExportISTMembersSportInfoToExcel();return false;" }))
            .ClientEvents(e => e.OnError("grid_OnError")
                    .OnLoad("restoreDefaultDeleteConfirmation")
                    .OnDataBound("restoreDefaultDeleteConfirmation")
                    )
            .DataBinding(dataBinding => dataBinding.Ajax()
                .Select("ISTMembersSportsInfoAjax", "Maintenance")
                .Insert("InsertISTMembersSport", "Maintenance")
                .Update("UpdateISTMembersSport", "Maintenance")
                .Delete("DeleteISTMembersSport", "Maintenance")
            )
            .Pageable(paging => paging.PageSize(10))
                .Scrollable(scrolling => scrolling.Height(276))
            .Resizable(resize => resize.Columns(true))
            .Sortable()
            .Filterable()
            .Columns(columns =>
            {
                columns.Bound(s => s.SportsId)
                    .ClientTemplate("<#= SportName #>").Width(300);
                columns.Bound(s => s.CrossCompetency)
                    .ClientTemplate("<span class='<#= CrossCompetency ? \"t-update\" : \"t-delete\" #> t-icon'></span>")
                    .Width(150).HtmlAttributes(new { style = "text-align:center" });
                columns.Bound(s => s.Primary)
                    .ClientTemplate("<span class='<#= Primary ? \"t-update\" : \"t-delete\" #> t-icon'></span>")
                    .Width(150).HtmlAttributes(new { style = "text-align:center" }); 
                columns.Bound(s => s.HasComments)
                    .ClientTemplate("<span onclick=\"openCommentWindow('ResourceSport', '<#= ResourceSportId #>', '<#= SportName #>', '" + Model.Editable + "', 'true');\" class=\"comment sprite sprite-<#= HasComments ? \"note\" : \"note_none\" #>\"></span>")
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
                columns.Bound(s => s.ResourceSportId).Hidden(true);
                columns.Bound(s => s.ResourceId).Hidden(true);
            })
            %>

            <br />
            <br />
            <div class="clear"></div>

    <% } %>
    


           