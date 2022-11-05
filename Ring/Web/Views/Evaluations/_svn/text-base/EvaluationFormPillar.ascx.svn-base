<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EvaluationPillarViewModel>" %>

<div class="groupbox_title_background gradientHToWhite">
    <div class="groupbox_title" >
        <%: Model.Pillar.Description %>
    </div>
</div>

<div style="float:right; margin:0px 0px 5px 10px;" class="t-button t-state-default" onmouseover="$(this).addClass('t-button-hover');" onmouseleave="$(this).removeClass('t-button-hover');" onclick="OpenBestPracticesWindow('<%: Model.EvaluationId %>', '<%: Model.Pillar.Id %>', '<%: Model.SportId %>');">
    <span class="button-span"><span class="t-add t-icon" style="margin: 0px 5px 3px 0px"></span><%: Shared.EvaluationsStrings.EvalutionFormPillar_Button_BestPractice %></span>
</div>



<% if (Model.Editable && Model.HasActionItem)
    {
        %>
        <div style="float:right; margin:0px 0px 5px 10px;" class="t-button t-state-default" onmouseover="$(this).addClass('t-button-hover');" onmouseleave="$(this).removeClass('t-button-hover');" onclick="openEvaluationActionItemWindow('EvaluationForm', 'Evaluations', '<%: Model.Pillar.Id %>', '<%: Model.EvaluationId %>', '<%: Model.SportId %>')">
            <span class="t-add t-icon"></span><span class="button-span"><%: Shared.EvaluationsStrings.EvalutionFormPillar_Button_AddActionItem %></span>
        </div>
<% } %>

<% if (Model.HasActionItem)
    { %>
        <div style="float:right; margin:0px 0px 5px 10px;" class="t-button t-state-default" onmouseover="$(this).addClass('t-button-hover');" onmouseleave="$(this).removeClass('t-button-hover');">
            <span class="sprite sprite-book_go" style="margin:0px 3px 0px 1px"></span><span class="button-span"><a href="<%: Url.Action("ActionItems", "Evaluations", new {referenceId = Model.EvaluationId, type = Model.Pillar.Id }) %>"><%: Shared.EvaluationsStrings.EvalutionFormPillar_Button_ActionItem %> <%: Model.ActionItemRatio %></a></span>
        </div>
<% } %>

<div class="clear"></div>

<div>

    <%= Html.Telerik().Grid<EvaluationQuestion>()
        .Name("QuestionsGrid" + Model.Pillar.Id)
        .DataKeys(keys => keys.Add(r => r.Id))
        .Editable(editing => editing.Mode(GridEditMode.InLine))
        .ToolBar(commands =>
        {
            if (Model.CanEditSportQuestions)
            {
                commands.Insert().ButtonType(GridButtonType.ImageAndText).HtmlAttributes(new { style = "font-size:8pt;" });
            }
        })
        .TableHtmlAttributes(new { style = "font-size:8pt;" })
        .Columns(columns =>
            {
                columns.Bound(r => r.SortOrder)
                    .Width(40);
                columns.Bound(r => r.Question);

                columns.Bound(r => r.NotApplicable)
                    .ClientTemplate("<span class='<#= NotApplicable ? \"t-update\" : \"\" #> t-icon'></span>")
                    .Width(75).HtmlAttributes(new { style = "text-align:center" }); 
                
                columns.Bound(r => r.InitialAssessment).Visible(!Model.CurrentStatusVisible);
                columns.Bound(r => r.CurrentAssessment).Visible(Model.CurrentStatusVisible);
                
                columns.Bound(r => r.InitialStatus).ReadOnly(!Model.InitialStatusEditable).Width(100)
                    .ClientTemplate("<span class='sprite sprite-StopLight_<#= InitialStatus #>'></span>")
                    .Filterable(false)
                    .Sortable(false)
                    .HtmlAttributes(new { style = "text-align:center" });
                columns.Bound(r => r.CurrentStatus).Visible(Model.CurrentStatusVisible).Width(100)
                    .ClientTemplate("<span class='sprite sprite-StopLight_<#= CurrentStatus #>'></span>")
                    .Filterable(false)
                    .Sortable(false)
                    .HtmlAttributes(new { style = "text-align:center" });
                columns.Bound(r => r.HasComments)
                    .ClientTemplate("<span onclick=\"openCommentWindow('EvaluationQuestion', '<#= Id #>', '<#= SortOrder #>', '" + Model.Editable + "', 'true');\" class=\"comment sprite sprite-<#= HasComments ? \"note\" : \"note_none\" #>\"></span>")
                    .Width(37)
                    .Title("")
                    .Filterable(false);
                columns.Command(commands =>
                {
                    if (Model.Editable) commands.Edit().ButtonType(GridButtonType.ImageAndText);
                    
                }).Width(100);

                columns.Bound(r => r.VersionQuestionId).Hidden(true);
                columns.Bound(r => r.SportId).Hidden(true);
            })
            .ClientEvents(e => e.OnEdit("gridEvaluationQuestionGrid_OnEdit")
                .OnError("grid_OnError"))
        .DataBinding(dataBinding => dataBinding.Ajax()
                    .Select("GetPillarQuestionsAjax", "Evaluations", new { id = Model.EvaluationId, pillarId = Model.Pillar.Id })
                    .Update("UpdatePillarQuestionsAjax", "Evaluations", new { evaluationId = Model.EvaluationId, pillarId = Model.Pillar.Id, evaluationSportId = Model.SportId })
                    .Insert("InsertPillarQuestionsAjax", "Evaluations", new { evaluationId = Model.EvaluationId, pillarId = Model.Pillar.Id, evaluationSportId = Model.SportId }))
        .Resizable(resize => resize.Columns(true))
        .Scrollable(scrolling => scrolling.Height(250))
        .Sortable()
        .Filterable()
        .ToHtmlString()
    %>

</div>
