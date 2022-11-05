<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ResultsManageViewModel>" %>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.RenderPartial("~/Views/Shared/JavaScriptCheck.ascx"); %>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%: Shared.ResultsStrings.ManageResults_Title %> <%: Model.TitleSport %><br /><%: Model.TitleCompetition %>
</asp:Content>

<asp:Content ID="Content10" ContentPlaceHolderID="InfoBar" runat="server">
    <% Html.RenderPartial("~/Views/Shared/LanguageSelector.ascx", Model.CurrentLanguageCode); %> &nbsp;|&nbsp;
    <% Html.RenderPartial("~/Views/Shared/CurrentUserName.ascx", Model.CurrentUserName); %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="MenuContent" runat="server">
    <% Html.RenderPartial("~/Views/Shared/NavMenu.ascx", Model.Menu); %>
</asp:Content>

<asp:Content ID="Content11" ContentPlaceHolderID="HeaderImage" runat="server">
    <%= RingHTMLHelper.GetRandomHeaderImage() %>
</asp:Content>


<asp:Content ID="Content7" ContentPlaceHolderID="YearSelectionContent" runat="server">
    <% 
        if (ViewData["competitionId"] != null)
        {
            Html.RenderPartial("~/Views/Results/ResultStatusSelector.ascx", Model);
        }%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div>

    <% 
    if (ViewData["competitionId"] == null)
    {%>
        <h3><%: Shared.ResultsStrings.ManageResults_Error_NoCompetition %></h3>
    <%}
    else
    {
    %>
        <h3 style="color:Red"><%: Html.ValidationSummary(true) %></h3>

        <% if (Model.Editable) { %>

            <div style="float:left; width:100%;">
                <%-- Building Action Buttons --%>
            
                <div style="float:right;">

                    <% using (Html.BeginForm("ManageResults", "Results", new { id = Model.CompetitionId, resultsFilter = Model.SelectorValue, downloadOption = "D" }, FormMethod.Post, new { name = "frmResultDiscard" }))
                    { %>
                        <div style="float:right; margin:10px 0px 5px 10px;" class="t-button t-state-default" onmouseover="$(this).addClass('t-button-hover');" onmouseleave="$(this).removeClass('t-button-hover');" onclick="deleteResults();">
                            <span class="button-span"><span class="sprite sprite-time_delete" style="margin: 0px 5px 3px 0px"></span><%: Shared.ResultsStrings.ManageResults_Button_Discard %></span>
                        </div>
                    <% } %>

                    <% using (Html.BeginForm("ManageResults", "Results", new { id = Model.CompetitionId, resultsFilter = Model.SelectorValue, downloadOption = "R" }, FormMethod.Post, new { name = "frmResultPostReplace" }))
                    { %>
                        <div style="float:right; margin:10px 0px 5px 10px;" class="t-button t-state-default" onmouseover="$(this).addClass('t-button-hover');" onmouseleave="$(this).removeClass('t-button-hover');" onclick="postResults();">
                            <span class="button-span"><span class="sprite sprite-time_add" style="margin: 0px 5px 3px 0px"></span><%: Shared.ResultsStrings.ManageResults_Button_PostReplace %></span>
                        </div>
                    <% } %>

                </div>

            </div>

        <% } %>


        <div style="float:left; width:100%;">

            <%-- Building Grid --%>
            <%= Html.Telerik().Grid<ResultsManageResultsStagingGridViewModel>()
                .Name("Results")
                .DataKeys(keys => keys.Add(r => r.ResultStagingId))
                .Editable(editing => editing.Mode(GridEditMode.InLine))
                .ToolBar(commands =>
                {
                    if (Model.Editable)
                    {
                        commands.Insert().ButtonType(GridButtonType.ImageAndText).HtmlAttributes(new { style = "font-size:8pt;" });
                        commands.Custom()
                            .Text("<span class='sprite sprite-thumb_up' style='margin-right: 3px'></span>" + Shared.ResultsStrings.ManageResults_Button_Approve)
                            .Action("ManageResults", "Results", new { id = Model.CompetitionId, resultsFilter = Model.SelectorValue, downloadOption = "Approve" })
                            .HtmlAttributes(new { style = "font-size:8pt;", id = "Approve" });
                        commands.Custom()
                            .Text("<span class='sprite sprite-thumb_down' style='margin-right: 3px'></span>" + Shared.ResultsStrings.ManageResults_Button_Unapprove)
                            .Action("ManageResults", "Results", new { id = Model.CompetitionId, resultsFilter = Model.SelectorValue, downloadOption = "Unapprove" })
                            .HtmlAttributes(new { style = "font-size:8pt;", id = "UnApprove" });
                    }
                })
                .TableHtmlAttributes(new { style = "font-size:8pt;" })
                .Columns(columns =>
                    {
                        columns.Bound(r => r.Approved)
                            .ClientTemplate("<span class='<#= Approved ? \"t-update\" : \"t-delete\" #> t-icon'></span>")
                            .HtmlAttributes(new { style = "text-align:center" })
                            .Width(60);
                        columns.Bound(r => r.TeamName)
                            .Width(150);
                        columns.Bound(r => r.EventId)
                            .ClientTemplate("<#= EventDesc  #>")
                            .Width(150);
                        columns.Bound(r => r.EventDescription)
                            .Width(125);
                        columns.Bound(r => r.EventDate)
                            .ClientTemplate("<#= EventDateLiteral #>")
                            .Width(100);
                        columns.Bound(r => r.BenchmarkTypeId)
                            .ClientTemplate("<#= BenchmarkTypeName #>")
                            .Width(110);
                        columns
                            .Bound(r => r.PositionDesc)
                            .Width(70);
                        columns.Bound(r => r.CountryCode).ClientTemplate("<#= CountryDesc  #>").Width(150);
                        columns.Bound(r => r.PerfValue).Width(80);
                        columns.Bound(r => r.PerfTimeDesc)
                            .Width(125);
                        columns.Command(commands =>
                        {
                            if (Model.Editable)
                            {
                                commands.Edit().ButtonType(GridButtonType.ImageAndText);
                                commands.Delete().ButtonType(GridButtonType.ImageAndText);
                            }
                        }).Width(90);
                        columns.Bound(r => r.ResultStagingId).Hidden(true);
                        columns.Bound(r => r.TeamSport).Hidden(true);
                    })
                .ClientEvents(e => e.OnError("grid_OnError")
                    .OnEdit("onResultsGridEdit")
                )
                .DataBinding(dataBinding => dataBinding.Ajax()
                    .Select("GetCompetitionResultsAjax", "Results", new { competitionId = Model.CompetitionId })
                    .Update("UpdateCompetitionResultsAjax", "Results", new { competitionId = Model.CompetitionId })
                    .Insert("InsertCompetitionResultsAjax", "Results", new { competitionId = Model.CompetitionId })
                    .Delete("DeleteCompetitionResultsAjax", "Results", new { competitionId = Model.CompetitionId }))
                .Scrollable(scrolling => scrolling.Enabled(true).Height(370))
                .Resizable(resize => resize.Columns(true))
                .Sortable()
                .Filterable()
            .DetailView(d => d.ClientTemplate(
            Html.Telerik().Grid<ResultManageResultsStagingResourceGridViewModel>()
                .Name("Resources_<#= ResultStagingId #>")
                .HtmlAttributes(new { Style = "width:962px" })
                .DataKeys(keys => keys.Add(b => b.ResultStagingResourceId))
                .ToolBar(toolbar =>
                    {
                        if (Model.Editable) toolbar.Insert().ButtonType(GridButtonType.ImageAndText);
                    })
                .ClientEvents(e => e.OnError("grid_OnError"))
                .DataBinding(dataBinding => dataBinding.Ajax()
                    .Select("GetCompetitionResultsResourcesAjax", "Results", new { resultStagingId = "<#= ResultStagingId #>" })
                    .Insert("InsertResultStagingResource", "Results", new { resultStagingId = "<#= ResultStagingId #>" })
                    .Update("UpdateResultStagingResource", "Results", new { resultStagingId = "<#= ResultStagingId #>" })
                    .Delete("DeleteResultStagingResource", "Results", new { resultStagingId = "<#= ResultStagingId #>" })
                )
                .Pageable()
                .Sortable()
                .Resizable(resize => resize.Columns(true))
                .Filterable()
                .Columns(columns =>
                {
                    columns.Bound(b => b.ResourceId)
                        .ClientTemplate("<#= ResourceName #>")
                        .Width(220);
                    columns.Bound(b => b.TeamMember)
                        .Width(85);
                    columns.Bound(b => b.DownloadLastName)
                        .Width(120);
                    columns.Bound(b => b.DownloadFirstName)
                        .Width(120);
                    columns.Bound(b => b.GenderCode)
                        .ClientTemplate("<#= GenderDesc #>")
                        .Width(80);
                    columns.Bound(b => b.BirthDate)
                        .ClientTemplate("<#= BirthDateLiteral #>")
                        .Width(100);
                    columns.Bound(b => b.SportTrackingRef)
                        .Width(100);
                    columns.Bound(b => b.HasComments)
                        .ClientTemplate("<span onclick=\"openCommentWindow('ResultStagingResource', '<#= ResultStagingResourceId #>', '<#= ResourceId #>', '" + Model.Editable + "', 'true');\" class=\"comment sprite sprite-<#= HasComments ? \"note\" : \"note_none\" #>\"></span>")
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

        </div>

    <% } %>

    </div>
    
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="ScriptContent" runat="server">
    <script type="text/javascript">
        function postResults()
        {
            if (confirm("<%: Shared.ResultsStrings.ManageResults_PostResultsConfirmation %>"))
            {
                document.frmResultPostReplace.submit();
                ShowPleaseWait();
            }
        }

        function deleteResults()
        {
            if (confirm("<%: Shared.ResultsStrings.ManageResults_DeleteResultsConfirmation %>"))
            {
                document.frmResultDiscard.submit();
                ShowPleaseWait();
            }
        }

        function onResultsGridEdit(e)
        {
            if (e.mode != 'edit')
            {
                $('#Approved').attr('disabled', 'disabled');
            }
            else
            {
                $('#Approved').removeAttr('disabled');
            }
        }

        $('#Approve').click(function ()
        {
            ShowPleaseWait();
        });

        $('#UnApprove').click(function ()
        {
            ShowPleaseWait();
        });

    </script>
</asp:Content>