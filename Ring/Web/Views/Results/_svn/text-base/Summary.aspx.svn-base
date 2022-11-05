<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ResultsSummaryViewModel>" %>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.RenderPartial("~/Views/Shared/JavaScriptCheck.ascx"); %>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%: Shared.ResultsStrings.Summary_Title %> : <%: Model.VerifiedSelectedSportDesc%>
</asp:Content>

<asp:Content ID="Content10" ContentPlaceHolderID="InfoBar" runat="server">
    <% Html.RenderPartial("~/Views/Shared/LanguageSelector.ascx", Model.CurrentLanguageCode); %> &nbsp;|&nbsp;
    <% Html.RenderPartial("~/Views/Shared/CurrentUserName.ascx", Model.CurrentUserName); %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div style="float:left; width:100%;">
    <% 
    if (string.IsNullOrEmpty(Model.VerifiedSelectedSport))
    {%>
        <h3><%: Shared.ResultsStrings.Summary_Error_NoSelectedSport %></h3>
    <%}
    else
    {
    %>
     <%--// TODO: Localize --%>
        <%
        Html.Telerik().Window()
        .Name("ConfirmRepost")
        .Title("Download New or Re-Post Existing Results")
        .Content(() =>
            { %>
                <div class="groupbox_600" style="margin:0px 0px 0px 0px">
                    <div class="groupbox_title_background gradientHToWhite">
                        <div class="groupbox_title" >
                        </div>
                    </div>
                    <div class="groupbox_content">
                        <div>
                            <h4>Would you like to manage the existing results of this competition, or download a new set of results?</h4>
                        </div>
                    </div>
                    <img src="../../Content/Images/sidenav_btm.gif" width="600" height="9" /> 
                </div> 
                <div class="clear"></div>
                <div style="float:right; text-align:right; margin: 5px 10px 0px 0px" >
                    <input type="button" value="Manage Existing Results" onClick="manageExistingResults(true);" />
                    <input type="button" value="Download New Results" onClick="downloadNewResults();" />
                    <input type="button" value="<%: Shared.SharedStrings.Button_Cancel %>" onClick="$('#ConfirmRepost').data('tWindow').close();" />
                </div>
                <input type="hidden" id="competitionId" name="competitionId" />
            <%})
        .Draggable(true)
        .Scrollable(false)
        .Resizable()
        .Buttons(b => b.Close())
        .Width(605)
        .Height(120)
        .Modal(true)
        .Visible(false)
        .Render();
                        
        %>
        
        <%-- Building Grid --%>
        <%  Html.Telerik().Grid<Competition>()
            .Name("Competitions")
            .DataKeys(keys => keys.Add(c => c.Id))
            .Editable(editing => editing.Mode(GridEditMode.InLine))
            .ToolBar(commands =>
            {
                if (Model.Editable) commands.Insert().ButtonType(GridButtonType.ImageAndText).HtmlAttributes(new { style = "font-size:8pt;" });
            })
            .TableHtmlAttributes(new { style = "font-size:8pt;" })
            .Pageable(paging => paging
                .PageSize(7))
            .EnableCustomBinding(true)
            .Filterable()
            .Columns(columns =>
                {
                    columns.Bound(c => c.SportId)
                        .ClientTemplate("<#= SportDesc  #>")
                        .Width(150)
                        .Filterable(false);
                    columns.Bound(c => c.Name)
                        .Width(150)
                        .Filterable(false);
                    columns.Bound(c => c.Date)
                        .ClientTemplate("<#= DateLiteral #>")
                        .Width(100)
                        .Filterable(false);
                    columns.Bound(c => c.Location)
                        .Width(100)
                        .Filterable(false);
                    columns.Bound(c => c.CountryCode)
                        .ClientTemplate("<#= CountryDesc  #>")
                        .Width(150)
                        .Filterable(false);
                    columns.Bound(c => c.DownloadStatus)
                        .ClientTemplate("<#= ShowDownloadStatus ? \"<a onclick='return confirmShowApproval(\" + ShowApproveConfirmation + \", \" + Id + \", 1)' href='#'>\" + DownloadStatus + \"</a>\" : DownloadStatus #>")
                        .Width(85)
                        .Filterable(false);
                    columns.Bound(c => c.ApproveStatus)
                        .ClientTemplate("<#= ShowApproveStatus ? \"<a onclick='return confirmShowApproval(\" + ShowApproveConfirmation + \", \" + Id + \", 2)' href='#'>\" + ApproveStatus + \"</a>\" : ApproveStatus #>")
                        .Width(80)
                        .Filterable(false);
                    columns.Bound(c => c.Ratio)
                        .Filterable(false)
                        .Sortable(false)
                        .Width(155);
                    columns.Bound(c => c.Type)
                        .ClientTemplate("<#= TypeDesc  #>")
                        .Width(150)
                        .Filterable(false);
                    columns.Bound(c => c.BenchmarkCode)
                        .ClientTemplate("<#= BenchmarkDesc  #>")
                        .Width(150)
                        .Filterable(false);


                    columns.Command(commands =>
                    {
                        commands.Edit().ButtonType(GridButtonType.ImageAndText);
                        commands.Delete().ButtonType(GridButtonType.ImageAndText);
                    }).Width(90);
                })
            .ClientEvents(e => e.OnError("grid_OnError")
                    .OnRowDataBound("gridCompetitions_OnRowDataBound")
                    .OnSave("gridCompetitions_OnSave"))
            .DataBinding(dataBinding => dataBinding.Ajax()
                        .Select("GetCompetitionsAjax", "Results")
                        .Update("UpdateCompetitionsAjax", "Results")
                        .Insert("InsertCompetitionsAjax", "Results")
                        .Delete("DeleteCompetitionsAjax", "Results"))
            .Scrollable(scrolling => scrolling.Enabled(true).Height(370))
            .Resizable(resize => resize.Columns(true))
            .Sortable()
            .DetailView(events => events.ClientTemplate(
                    Html.Telerik().Grid<CompetitionEvent>()
                    .Name("Events_<#= Id #>")
                    .DataKeys(keys => keys.Add(e => e.Id))
                    .Editable(editing => editing.Mode(GridEditMode.InLine))
                    .ToolBar(commands =>
                    {
                        if (Model.Editable) commands.Insert().ButtonType(GridButtonType.ImageAndText);
                    })                    
                    .Columns(columns =>
                    {
                        columns.Bound(e => e.DisciplineId).ClientTemplate("<#= DisciplineDesc  #>");
                        columns.Bound(e => e.EventId).ClientTemplate("<#= EventDesc  #>");

                        columns.Bound(e => e.Cancelled)
                            .ClientTemplate("<span class='<#= Cancelled ? \"t-update\" : \"\" #> t-icon'></span>")
                            .HtmlAttributes(new { style = "text-align:center" }).Width(75);
                        
                        columns.Bound(e => e.FieldSize);
                        
                        columns.Command(commands =>
                        {
                            commands.Edit().ButtonType(GridButtonType.ImageAndText);
                            commands.Delete().ButtonType(GridButtonType.ImageAndText);
                        }).Width(90);                        
                    })
                    .ClientEvents(e => e.OnError("grid_OnError")
                                .OnRowDataBound("gridCompetitionEvents_OnRowDataBound")
                                .OnSave("gridCompetitions_OnSave")
                                .OnDelete("gridCompetitions_OnSave"))
                    .DataBinding(dataBinding => dataBinding.Ajax()
                                .Select("GetCompetitionEventsAjax", "Results", new { competitionId = "<#= Id #>" })
                                .Update("UpdateCompetitionEventsAjax", "Results", new { competitionId = "<#= Id #>" })
                                .Insert("InsertCompetitionEventsAjax", "Results", new { competitionId = "<#= Id #>" })
                                .Delete("DeleteCompetitionEventsAjax", "Results", new { competitionId = "<#= Id #>" }))
                    .Resizable(resize => resize.Columns(true))
                    .ToHtmlString()
                )
            ).Render();
        %>

    <% } %>

    </div>
    
    <script type="text/javascript">

        function confirmShowApproval(showConfirmation, id, linkType)
        {
            $('#competitionId').val(id);

            if (showConfirmation)
            {
                $("#ConfirmRepost").data("tWindow").center().open();
            }
            else
            {
                if (linkType == 1)
                {
                    downloadNewResults();
                }
                else
                {
                    manageExistingResults(false);
                }
            }

            return true;
        }

        function manageExistingResults(posted)
        {
            var id = $('#competitionId').val();
            var path = "/Results/ManageResults/" + id;
            if (posted)
            {
                path = "/Results/UnloadResultStagingResults/" + id;
            }

            window.location.pathname = path;
        }

        function downloadNewResults()
        {
            var id = $('#competitionId').val();
            window.location.pathname = "Results/LoadResults/" + id;
        }

        function gridCompetitions_OnRowDataBound(e)
        {
            if (!e.dataItem.Editable)
            {
                e.row.cells[11].innerHTML = '';
            }
        }

        function gridCompetitionEvents_OnRowDataBound(e)
        {
            var competitionRow = $(e.currentTarget).closest('.t-detail-row').prev();
            var competitionDataItem = competitionRow.closest('.t-grid').data('tGrid').dataItem(competitionRow);

            if (!competitionDataItem.Editable)
            {
                e.row.cells[4].innerHTML = '';
                $(e.currentTarget).find('.t-grid-toolbar').hide();
            }
        }

        function gridCompetitions_OnSave(e)
        {
            var grid = $('#Competitions').data('tGrid');
            grid.ajaxRequest();
        }

    </script>

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="MenuContent" runat="server">
    <% Html.RenderPartial("~/Views/Shared/NavMenu.ascx", Model.Menu); %>
</asp:Content>

<asp:Content ID="Content11" ContentPlaceHolderID="HeaderImage" runat="server">
    <%= RingHTMLHelper.GetRandomHeaderImage() %>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="YearSelectionContent" runat="server">
    <% Html.RenderPartial("~/Views/Shared/YearSelector.ascx", Model.CurrentOlympicYears); %>
    <% Html.RenderPartial("~/Views/Results/SummaryStatusSelector.ascx", Model); %>
</asp:Content>
