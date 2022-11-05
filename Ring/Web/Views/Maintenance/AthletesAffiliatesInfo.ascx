<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MaintenanceEditableViewModel>" %>

    <script type="text/javascript">
        function onExportAthleteAffiliatesInfoToExcel() {
            var resourceId = $('#resourceId').val();

            var grid = $('#Affiliates').data('tGrid');
            var page = grid.currentPage;
            var pageSize = grid.pageSize;
            var orderBy = grid.orderBy
            var filter = grid.filterBy;

            var url = "/ExportExcelButton/ExportAthleteAffiliatesInfoToExcel";
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

        <input type="hidden" id="tabResourceId" value="<%= ViewData["resourceId"] %>" />
        <%= Html.Telerik().Grid<MaintenanceAthletesAffiliatesInfoViewModel>()
            .Name("Affiliates")
            .TableHtmlAttributes(new { style = "font-size:8pt;" })
            .HtmlAttributes(new { style = "height:335px" })
            .DataKeys(k => k.Add(k2 => k2.ResourceAffiliateId))
            .ToolBar(t =>
                {
                    if (Model.Editable) t.Insert().ButtonType(GridButtonType.ImageAndText);
                })
            .Editable(e => e.Mode(GridEditMode.InLine).Enabled(true))
            .ToolBar(commands => commands.Custom().Text(Shared.MaintenanceStrings.ExportToExcel).HtmlAttributes(new { onclick = "onExportAthleteAffiliatesInfoToExcel();return false;" }))
            .ClientEvents(e => e.OnError("grid_OnError")
                    .OnLoad("restoreDefaultDeleteConfirmation")
                    .OnDataBound("restoreDefaultDeleteConfirmation")
            )
            .DataBinding(dataBinding => dataBinding.Ajax()
                            .Select("AthletesAffiliatesInfoAjax", "Maintenance")
                            .Update("UpdateAthletesAffiliate", "Maintenance")
                            .Insert("InsertAthletesAffiliate", "Maintenance")
                            .Delete("DeleteAthletesAffiliate", "Maintenance")
            )
            .Pageable(paging => paging.PageSize(10))
                .Scrollable(scrolling => scrolling.Height(276))
            .Resizable(resize => resize.Columns(true))
            .Sortable()
            .Filterable()
            .Columns(columns =>
            {
                columns.Bound(a => a.AffiliateTypeId)
                    .ClientTemplate("<#= AffiliateTypeName #>")
                    .Width(200);
                columns.Bound(a => a.YearFromId)
                    .Width(120);
                columns.Bound(a => a.YearToId)
                    .Width(120);
                columns.Bound(a => a.AffiliateName)
                    .Width(150);
                columns.Bound(a => a.Description)
                    .Width(150);
                columns.Bound(a => a.AffiliateContact)
                    .Width(120);
                columns.Bound(a => a.AffiliateEmail)
                    .Width(130);
                columns.Bound(a => a.HasComments)
                    .ClientTemplate("<span onclick=\"openCommentWindow('ResourceAffiliate', '<#= ResourceAffiliateId #>', '<#= AffiliateName #>', '" + Model.Editable + "', 'true');\" class=\"comment sprite sprite-<#= HasComments ? \"note\" : \"note_none\" #>\"></span>")
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
                columns.Bound(a => a.ResourceAffiliateId).Hidden(true);
                columns.Bound(a => a.ResourceId).Hidden(true);
            })

                    %>

            <br />
            <br />
            <div class="clear"></div>

    <% } %>

    


           