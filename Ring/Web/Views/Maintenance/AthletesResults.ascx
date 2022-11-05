<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MaintenanceEditableViewModel>" %>      
     
    <script type="text/javascript">
        function onExportAthleteResultsToExcel() {
            var resourceId = $('#resourceId').val();

            var grid = $('#Results').data('tGrid');
            var page = grid.currentPage;
            var pageSize = grid.pageSize;
            var orderBy = grid.orderBy
            var filter = grid.filterBy;

            var url = "/ExportExcelButton/ExportAthleteResultsToExcel";
            window.location.href = url + "?resourceId=" + resourceId + "&page=" + page + "&pageSize=" + pageSize + "&orderBy=" + orderBy + "&filter=" + filter;
        }

        function restoreDefaultDeleteConfirmation() {
            var grid = $(this).data('tGrid');

            $(this).find('.t-grid-delete').click(function (e) {
                //var tr = $(this).closest('tr');

                grid.localization.deleteConfirmation = '<%: Shared.MaintenanceStrings.Maintenance_DefaultDeleteConfirmation %>';
            });
        }

        function onInsertResults(e) 
        {
            var resourceId = $('#resourceId').val();
            var values = e.values;
            
            var datepicker = $("#CompetitionDate").data('tDatePicker');
            var datestring = datepicker.inputValue.toString();
            var perftime = $('#PerfTime').val();

            $.ajax({
                type: 'POST',
                url: '/Maintenance/IncorrectPerfTime',
                dataType: 'json',
                data: { perfTime: perftime },
                async: false,
                success: function (perftime_valid) 
                {
                    if (!perftime_valid) 
                    {
                        OTPAlert("<%: Shared.SharedStrings.Error_PerfTimeInvalid %>", "Status");
                        e.preventDefault();
                    }
                }
            });
        }
        
    </script>

    <div class="groupbox_title_background gradientHToWhite">
        <table style="width: 100%">
            <tr>
                <td class="groupbox_title_results_tab1">
                    <%= ViewData["resourceName"] %>
                </td>
                <td class="groupbox_title_results_tab4"> 
<%--                    <input type="image" src='<%: Url.Content("~/Content/Images/ExportExcelButton.gif") %>'  id="ExportExcel" />                                    
--%> 
<%--                    <a href="#" id="ExportExcel"> <img src='<%: Url.Content("~/Content/Images/ExportExcelButton.gif") %>'/></a>
--%>                    
                </td>
            </tr>
        </table>
    </div>

    <% if (ViewData["resourceId"] != null) 
       { %>

        <input type="hidden" id="tabResourceId" value="<%= ViewData["resourceId"] %>" />
        <%= Html.Telerik().Grid<MaintenanceAthletesResultsViewModel>()
            .Name("Results")
            .TableHtmlAttributes(new { style = "font-size:8pt;" })
            .HtmlAttributes(new { style = "height:350px;" })
            .DataKeys(k => k.Add(k2 => k2.ResultID))
            .ToolBar(t =>
                {
                    if (Model.Editable) t.Insert().ButtonType(GridButtonType.ImageAndText);
                })
            .Editable(e => e.Mode(GridEditMode.InLine).Enabled(true))
            .ToolBar(commands => commands.Custom().Text(Shared.MaintenanceStrings.ExportToExcel).HtmlAttributes(new { onclick = "onExportAthleteResultsToExcel();return false;" }))
            .ClientEvents(e => e.OnError("grid_OnError") 
                                .OnSave("onInsertResults")
                                .OnLoad("restoreDefaultDeleteConfirmation")
                                .OnDataBound("restoreDefaultDeleteConfirmation")
                )
            .DataBinding(dataBinding => dataBinding.Ajax()
                                        .Select("AthletesResultsAjax", "Maintenance")
                                        .Update("UpdateAthletesResults", "Maintenance")
                                        .Insert("InsertAthletesResults", "Maintenance")
                                        .Delete("DeleteAthletesResults", "Maintenance")
            )
            .Pageable(paging => paging
                .PageSize(10)
                .Style(GridPagerStyles.NextPreviousAndInput | GridPagerStyles.Numeric)
                )
            .Scrollable(scrolling => scrolling.Height(290))
            .Resizable(resize => resize.Columns(true))
            .EnableCustomBinding(true)
            .Sortable()
            .Filterable()
            .Columns(columns =>
            {
                columns.Bound(a => a.CompetitionName)
                    .Width(160);
                columns.Bound(a => a.CompetitionDate)
                    .Width(100);
                columns.Bound(a => a.Location)
                    .Width(120);
                columns.Bound(a => a.CountryName)
                    .Width(110);
                columns.Bound(a => a.SportName)
                    .Width(140);
                columns.Bound(a => a.DisciplineName)
                    .Width(120);
                columns.Bound(a => a.EventName)
                    .Width(120);
                columns.Bound(a => a.Position)
                    .Width(60);
                columns.Bound(a => a.EventDate)
                    .Width(100);
                columns.Bound(a => a.PerfTime)
                    .Width(120);
                columns.Bound(a => a.PerfValue)
                    .Width(120);
                columns.Bound(a => a.Comments)
                    .Width(60);
                columns.Command(a =>
                {
                    if (Model.Editable)
                    {
                        a.Edit().ButtonType(GridButtonType.ImageAndText);
                        a.Delete().ButtonType(GridButtonType.ImageAndText);
                    }
                })
                .Width(175)
                .Title(Shared.SharedStrings.GridColumn_Command)
                .HtmlAttributes(new { style = "white-space:nowrap;float:right;" });
                columns.Bound(a => a.ResultID).Hidden(true);
                columns.Bound(a => a.ResourceID).Hidden(true);
            })

        %>

            <br />
            <br />
            <div class="clear"></div>

    <% } %>
