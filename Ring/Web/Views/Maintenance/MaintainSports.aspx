<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MaintenanceSportsViewModel>" %>

<asp:Content ID="Content7" ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.RenderPartial("~/Views/Shared/JavaScriptCheck.ascx"); %>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	<%: Shared.MaintenanceStrings.MaintainSports_Title %> <%: ViewData["sportName"] %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MenuContent" runat="server">
    <% Html.RenderPartial("~/Views/Shared/NavMenu.ascx", Model.Menu); %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="InfoBar" runat="server">
    <% Html.RenderPartial("~/Views/Shared/LanguageSelector.ascx", Model.CurrentLanguageCode); %> &nbsp;|&nbsp;
    <% Html.RenderPartial("~/Views/Shared/CurrentUserName.ascx", Model.CurrentUserName); %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="YearSelectionContent" runat="server">
    <% Html.RenderPartial("~/Views/Shared/YearSelector.ascx", Model.CurrentOlympicYears); %>
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="HeaderImage" runat="server">
    <%= RingHTMLHelper.GetRandomHeaderImage() %>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">

<div id="container">

    <div style="float:left; width:100%;">
        <% Html.RenderPartial("~/Views/Shared/CommentEditor.ascx"); %>

        <%= Html.ValidationMessage("Errors") %>
        <%= Html.Telerik().Grid<MaintenanceSportsGridSportViewModel>()
            .Name("Sports")
            .TableHtmlAttributes(new { style = "font-size:8pt;" })
            .HtmlAttributes(new { style = "height:415px;width:1120px" })
            .DataKeys(keys => keys.Add(s => s.SportId))
            .Editable(editing => editing.Mode(GridEditMode.InLine))
            .ClientEvents(e => e.OnRowDataBound("gridSports_OnRowDataBound")
                                .OnError("grid_OnError"))
            .DataBinding(dataBinding => dataBinding.Ajax()
                .Select("_SportsMaintenanceAjax", "Maintenance")
                .Update("UpdateSport", "Maintenance")
            )
            .Pageable(paging => paging.PageSize(14))
            .Scrollable(scrolling => scrolling.Height(388))
            .Resizable(resize => resize.Columns(true))
            .Sortable()
            .Filterable()
            .Columns(columns =>
            {
                columns.Bound(s => s.SportCode).Width(70);
                columns.Bound(s => s.Name).Width(100);
                columns.Bound(s => s.Gender)
                    .Width(70)
                    .HtmlAttributes(new { style = "text-align:center" });
               columns.Bound(s => s.NSO)
                    .ClientTemplate("<# if (NSO) { #><#= NSO.Description #> <#} else { #> none <# } #>")
                    .Width(200);
                columns.Bound(s => s.HPA)
                    .ClientTemplate("<# if (HPA) { #><#= HPA.Description #> <#} else { #> none <# } #>")
                    .Width(125);
                columns.Bound(s => s.Benchmark)
                    .ClientTemplate("<span <#= Benchmark ? \"class='t-icon t-update'\" : \"\" #>></span>")
                    .Width(80)
                    .HtmlAttributes(new { style = "text-align:center" });
                columns.Bound(s => s.MedalEvent)
                    .ClientTemplate("<span <#= MedalEvent ? \"class='t-icon t-update'\" : \"\" #>></span>")
                    .Width(80)
                    .HtmlAttributes(new { style = "text-align:center" });
                columns.Bound(s => s.Ranking)
                    //.ClientTemplate("<# if (Ranking) { #><#= Ranking.Description #> <#} else { #> none <# } #>")
                    .Width(70);
                columns.Bound(s => s.HasComments)
                    .ClientTemplate("<span onclick=\"openCommentWindow('Sport', '<#= SportId #>', '<#= Name #>', 'true', '<#= Editable #>');\" class=\"comment sprite sprite-<#= HasComments ? \"note\" : \"note_none\" #>\"></span>")
                    .Width(40)
                    .Title("");
                columns.Command(commands =>
                {
                    commands.Edit().ButtonType(GridButtonType.ImageAndText);
                })
                .Width(100);
            })
            .DetailView(details => details.ClientTemplate(
                Html.Telerik().Grid<MaintenanceSportsGridDisciplineViewModel>()
                .Name("Disciplines_<#= SportId #>")
                .HtmlAttributes(new { Style = "width:1050px" })
                .DataKeys(keys => keys.Add(d => d.DisciplineId))
                    .ToolBar(toolbar => toolbar.Insert().ButtonType(GridButtonType.ImageAndText))
                .ClientEvents(e => e.OnRowDataBound("gridDisciplines_OnRowDataBound")
                                    .OnError("grid_OnError"))
                .DataBinding(dataBinding => dataBinding.Ajax()
                    .Select("_DisciplinesForSportsMaintenanceAjax", "Maintenance", new { sportId = "<#= SportId #>" })
                    .Update("UpdateDiscipline", "Maintenance")
                    .Delete("DeleteDiscipline", "Maintenance")
                    .Insert("InsertDiscipline", "Maintenance", new { parentSportId = "<#= SportId #>" })
                )
                .Pageable()
                .Sortable()
                .Filterable()
                .Resizable(resize => resize.Columns(true))
                .Columns(columns =>
                {
                    columns.Bound(d => d.SportCode).Width(70);
                    columns.Bound(d => d.Name_EN).Width(200);
                    columns.Bound(d => d.Name_FR).Width(200);
                    columns.Bound(d => d.Gender)
                        .Width(70)
                        .HtmlAttributes(new { style = "text-align:center" });
                    columns.Bound(d => d.Benchmark)
                        .ClientTemplate("<span <#= Benchmark ? \"class='t-icon t-update'\" : \"\" #>></span>")
                        .Width(85)
                        .HtmlAttributes(new { style = "text-align:center" });
                    columns.Bound(d => d.MedalEvent)
                        .ClientTemplate("<span <#= MedalEvent ? \"class='t-icon t-update'\" : \"\" #>></span>")
                        .Width(85)
                        .HtmlAttributes(new { style = "text-align:center" });
                    columns.Bound(d => d.HasComments)
                        .ClientTemplate("<span onclick=\"openCommentWindow('Sport', '<#= DisciplineId #>', '<#= Name_EN #>', 'true', 'true');\" class=\"comment sprite sprite-<#= HasComments ? \"note\" : \"note_none\" #>\"></span>")
                        .Width(37)
                        .HtmlAttributes(new { style = "text-align:center" })
                        .Title("");
                    columns.Command(commands =>
                    {
                        commands.Edit().ButtonType(GridButtonType.ImageAndText);
                        commands.Delete().ButtonType(GridButtonType.ImageAndText);
                    })
                    //.HtmlAttributes(new { style = "text-align:center" })
                    .Width(175)
                    .HtmlAttributes(new { style = "text-align:center;white-space:nowrap;float:right;" });
                })
                .DetailView(ordersDetailView => ordersDetailView.ClientTemplate(
                    Html.Telerik().Grid<MaintenanceSportsGridEventViewModel>()
                    .Name("Events_<#= DisciplineId #>")
                    .HtmlAttributes(new { Style = "width:1000px" })
                    .ToolBar(toolbar => toolbar.Insert().ButtonType(GridButtonType.ImageAndText))
                    .DataKeys(keys => keys.Add(e => e.EventId))
                    .ClientEvents(e => e.OnRowDataBound("gridEvents_OnRowDataBound")
                                    .OnEdit("gridEvents_OnEdit")
                                    .OnError("grid_OnError"))
                    .DataBinding(dataBinding => dataBinding.Ajax()
                        .Select("_EventsForDisciplinesMaintenanceAjax", "Maintenance", new { disciplineId = "<#= DisciplineId #>" })
                        .Update("UpdateEvent", "Maintenance")
                        .Delete("DeleteEvent", "Maintenance")
                        .Insert("InsertEvent", "Maintenance", new { parentSportId = "<#= DisciplineId #>" })
                    )
                    .Pageable()
                    .Sortable()
                    .Filterable()
                    .Resizable(resize => resize.Columns(true))
                    .Columns(columns =>
                    {
                        columns.Bound(e => e.SportCode).Width(60);
                        columns.Bound(e => e.Name_EN).Width(150);
                        columns.Bound(e => e.Name_FR).Width(150);
                        columns.Bound(e => e.Benchmark)
                            .ClientTemplate("<span <#= Benchmark ? \"class='t-icon t-update'\" : \"\" #>></span>")
                            .Width(60)
                            .HtmlAttributes(new { style = "text-align:center" });
                        columns.Bound(e => e.MedalEvent)
                            .ClientTemplate("<span <#= MedalEvent ? \"class='t-icon t-update'\" : \"\" #>></span>")
                            .Width(80)
                            .HtmlAttributes(new { style = "text-align:center" });
                        columns.Bound(e => e.TeamSport)
                            .ClientTemplate("<span <#= TeamSport ? \"class='t-icon t-update'\" : \"\" #>></span>")
                            .Width(60)
                            .HtmlAttributes(new { style = "text-align:center" });
                        columns.Bound(e => e.TeamSize)
                            .Width(70)
                            .HtmlAttributes(new { style = "text-align:center" });
                        columns.Bound(e => e.ResourceTracking)
                            .ClientTemplate("<span <#= ResourceTracking ? \"class='t-icon t-update'\" : \"\" #>></span>")
                            .Width(80)
                            .HtmlAttributes(new { style = "text-align:center" });
                        columns.Bound(e => e.HasComments)
                            .ClientTemplate("<span onclick=\"openCommentWindow('Sport', '<#= EventId #>', '<#= Name_EN #>', 'true', 'true');\" class=\"comment sprite sprite-<#= HasComments ? \"note\" : \"note_none\" #>\"></span>")
                            .Width(37)
                            .Title("");
                        columns.Command(commands =>
                        {
                            commands.Edit().ButtonType(GridButtonType.ImageAndText);
                            commands.Delete().ButtonType(GridButtonType.ImageAndText);
                        })
                            .Width(175)
                            .HtmlAttributes(new { style = "text-align:center;white-space:nowrap;float:right;" });
                    })
                    .ToHtmlString()
                ))
                .ToHtmlString()
            ))

        %>
  
        <br />
        <br />

    </div>
</div>

    <script type="text/javascript">

        function gridSports_OnRowDataBound(e) {

            if (!e.dataItem.Editable) {
                e.row.cells[10].innerHTML = '';
            }

            return;
        }

        function gridDisciplines_OnRowDataBound(e) {

            var sportRow = $(e.currentTarget).closest('.t-detail-row').prev();
            var sportDataItem = sportRow.closest('.t-grid').data('tGrid').dataItem(sportRow);

            if (!sportDataItem.Editable) {
                e.row.cells[8].innerHTML = '';
            }

            return;
        }

        function gridEvents_OnRowDataBound(e) {

            var disciplineRow = $(e.currentTarget).closest('.t-detail-row').prev();
            var sportRow = disciplineRow.closest('.t-detail-row').prev();
            var sportDataItem = sportRow.closest('.t-grid').data('tGrid').dataItem(sportRow);

            if (!sportDataItem.Editable) {
                e.row.cells[8].innerHTML = '';
            }

            return;
        }

        function gridEvents_OnEdit(e)
        {
            // Initiallize the input boxes
            if (e.mode == 'insert') {
                var teamSize = '1';
                $('#TeamSize').val(teamSize);

                $('#MedalEvent').attr('checked', 'checked');
            }

            // find the checkbox first
            var $checkbox = $('#TeamSport');

            $checkbox.change(function ()
            {
                UpdateResourceTracking($checkbox.is(':checked'));
            });

            // find the table row (tr) which is being edited
            var $tr = $checkbox.closest('tr:has(form)');

            // get the grid client object
            var grid = $tr.closest('.t-grid').data('tGrid');

            // get the data item bound to this table row
            var dataItem = grid.dataItem($tr);
            var teamSport = false;
            
            if (dataItem)
            {
                teamSport = dataItem.TeamSport;
            }
            //set the value of the dropdown to select the proper item
            $checkbox.attr('checked', teamSport);
            UpdateResourceTracking(teamSport);

            return;
        }

        function UpdateResourceTracking(value)
        {
            var $trackingCheck = $('#ResourceTracking');

            if (value == true)
            {
                $trackingCheck.removeAttr("disabled");
            }
            else
            {
                $trackingCheck.attr("disabled", true).attr('checked', true);
            }
        }
    </script>

</asp:Content>

