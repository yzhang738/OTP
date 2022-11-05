<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<EvaluationFormViewModel>" %>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.RenderPartial("~/Views/Shared/JavaScriptCheck.ascx"); %>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%: Shared.EvaluationsStrings.EvaluationForm_Title %> <%: Model.SportTitle %><br /><%: Model.EvaluationTitle %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div>

    <%-- Best Practices Window --%>
    <%
        Html.Telerik().Window()
        .Name("BestPracticesWindow")
        .Title(Shared.EvaluationsStrings.EvalutionForm_BestPractices)
        .Content(() =>
            { %>
            
                <div class="groupbox_1000_1" style="margin:0px 0px 0px 0px">
                    <div class="groupbox_title_background gradientHToWhite">
                        <div class="groupbox_title" id="_title"></div>
                    </div>
                    <div class="groupbox_content">

                        <%= Html.Telerik().Grid<EvaluationBestPractice>()
                            .Name("BestPracticesGrid")
                            .DataKeys(keys => keys.Add(r => r.Id))
                            .Editable(editing => editing.Mode(GridEditMode.InLine))
                            .ToolBar(commands =>
                            {
                                if (Model.Editable) commands.Insert().ButtonType(GridButtonType.ImageAndText).HtmlAttributes(new { style = "font-size:8pt;" });
                            })
                            .TableHtmlAttributes(new { style = "font-size:8pt;" })
                            .Columns(columns =>
                                {
                                    columns.Bound(r => r.YearId).ReadOnly().Width(100);
                                    columns.Bound(r => r.SportDesc).ReadOnly().Width(100);
                                    columns.Bound(r => r.Description);
                                    //columns.Bound(r => r.DescriptionFR);
                                    
                                    columns.Bound(r => r.Comment)
                                        .ClientTemplate("<span onclick=\"openCommentWindow('EvaluationBestPractice', '<#= Id #>', '<#= SportDesc #>', '" + Model.Editable + "', '<#= Editable #>');\" class=\"comment sprite sprite-<#= HasComment ? \"note\" : \"note_none\" #>\"></span>")
                                        .Width(37)
                                        .Title("")
                                        .ReadOnly();

                                    columns.Command(commands =>
                                    {
                                        if (Model.Editable)
                                        {
                                            commands.Edit().ButtonType(GridButtonType.ImageAndText);
                                            commands.Delete().ButtonType(GridButtonType.ImageAndText);
                                        }
                                    }).Width(100);

                                    columns.Bound(r => r.EvaluationId).Hidden(true);
                                    columns.Bound(r => r.MasterPillarId).Hidden(true);
                                    columns.Bound(r => r.SportId).Hidden(true);
                                })
                            .ClientEvents(e => e.OnError("grid_OnError")
                                                .OnSave("gridBestPracticesGrid_onSave")
                                                .OnRowDataBound("gridBestPracticesGrid_OnRowDataBound"))
                            .DataBinding(dataBinding => dataBinding.Ajax()
                                            .Select("GetBestPracticesAjax", "Evaluations")
                                            .Update("UpdateBestPracticesAjax", "Evaluations")
                                            .Insert("InsertBestPracticesAjax", "Evaluations")
                                            .Delete("DeleteBestPracticesAjax", "Evaluations"))
                            .Scrollable(scrolling => scrolling.Enabled(true).Height(300))
                            .Resizable(resize => resize.Columns(true))
                            .Sortable()
                            .Filterable()
                            .ToHtmlString()
                        %>


                    <input type="hidden" name="hiddenEvaluationId" value="" />
                    <input type="hidden" name="hiddenMasterPillarId" value="" />
                    <input type="hidden" name="hiddenSportId" value="" />

                    </div>
                    <img alt="" src="../../Content/Images/sidenav_btm.gif" width="1000" height="9" /> 
                </div> 
                <div class="clear"></div>


                <% Html.RenderPartial("~/Views/Shared/CommentEditor.ascx"); %>

            <% })
        .Draggable(true)
        .Scrollable(false)
        .Resizable()
        .Buttons(b => b.Close())
        .Width(1005)
        .Modal(true)
        .Visible(false)
        .Render();
                        
    %>
    
    </div>


    <div id="container">
        <% 
        if (Model.Evaluation == null)
        {%>
            <h3><%: Shared.EvaluationsStrings.EvaluationForm_Error_NoEvaluation %></h3>
        <%}
        else
        { %>

            <h3 style="color:Red"><%: Html.ValidationSummary(true) %></h3>

            <div style="float:right;">
 
            <%
                if (Model.Editable)
                {
                    foreach (var item in Model.ChangeToStatus)
                    {
                        using (Html.BeginForm("ChangeToPhase", "Evaluations", new { id = Model.Evaluation.Id, changeFromPhase = item.FromStatus, changeToPhase = item.ToStatus }, FormMethod.Post, new { name = "statusForm" + item.ToStatus }))
                        {
                        string buttonText = Shared.EvaluationsStrings.EvalutionForm_ChangeToStatusButton_NA;
                        if (item.ToStatus == OTP.Ring.Common.Constant.EvaluationPhase.Edit)
                            buttonText = Shared.EvaluationsStrings.EvalutionForm_ChangeToStatusButton_Edit;
                        else if (item.ToStatus == OTP.Ring.Common.Constant.EvaluationPhase.Submit)
                            buttonText = Shared.EvaluationsStrings.EvalutionForm_ChangeToStatusButton_Submit;
                        else if (item.ToStatus == OTP.Ring.Common.Constant.EvaluationPhase.Closed)
                            buttonText = Shared.EvaluationsStrings.EvalutionForm_ChangeToStatusButton_Close;
                        else
                            buttonText = item.ToStatus;
            
                        %>
                            <div style="float:right; margin: 5px 0px 5px 10px;" class="t-button t-state-default" onmouseover="$(this).addClass('t-button-hover');" onmouseleave="$(this).removeClass('t-button-hover');" onclick="document.statusForm<%= item.ToStatus %>.submit();">
                                <span class="t-expand t-icon"></span><span class="button-span"><%: buttonText%></span>
                            </div>
                        <%
                        }
                    }
                }
                 %>

            </div>            
            
            <div class="clear"></div>

        <%    
            Html.Telerik().TabStrip()
                        .Name("TabStrip")
                        .Items(tabstrip =>
                        {
                            foreach (var pillar in Model.Evaluation.Pillars)
                            {
                                tabstrip.Add()
                                    .Text(pillar.Id_Name)
                                    .LoadContentFrom("EvaluationFormPillar", "Evaluations", new { id = Model.Evaluation.Id, pillarId = pillar.Id, 
                                        hasActionItem = Model.ToolbarButtons.Contains(OTP.Ring.Common.Constant.PageId.EvaluationAction),
                                        editable = Model.Editable 
                                    });
                            }
                        })
                        .SelectedIndex(Model.SelectedTabIndex)
                        .Render();            

            %>          
            <%
        } %>

    </div>

    <% Html.RenderPartial("~/Views/Shared/ActionItemEditor.ascx"); %>

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ScriptContent" runat="server">
    <script type="text/javascript">
        function OpenBestPracticesWindow(evaluationId, pillarId, sportId)
        {
            var bestPracticesGrid = $('#BestPracticesGrid').data('tGrid');

            bestPracticesGrid.rebind({
                pillarId: pillarId
            });

            $("#BestPracticesWindow")
                .find('input[name=hiddenEvaluationId]').val(evaluationId).end()
                .find('input[name=hiddenMasterPillarId]').val(pillarId).end()
                .find('input[name=hiddenSportId]').val(sportId).end()
                .data("tWindow").center().open();
        }


        function gridBestPracticesGrid_onSave(e)
        {
            var evaluationId = $("#BestPracticesWindow").find('input[name=hiddenEvaluationId]').val();
            var pillarId = $("#BestPracticesWindow").find('input[name=hiddenMasterPillarId]').val();
            var sportId = $("#BestPracticesWindow").find('input[name=hiddenSportId]').val();

            e.values["EvaluationId"] = evaluationId;
            e.values["MasterPillarId"] = pillarId;
            e.values["SportId"] = sportId;
        }


        function gridBestPracticesGrid_OnRowDataBound(e)
        {

            if (!e.dataItem.Editable)
            {
                e.row.cells[4].innerHTML = '';
            }

            return;
        }

        function gridEvaluationQuestionGrid_OnEdit(e)
        {
            var sportId = '<%: Model.Evaluation.SportId %>';
            var canEditSportQuestions = <%: Model.CanEditSportQuestions.ToString().ToLower() %>;

            if (e.mode == 'edit')
            {
                var dataItem = e.dataItem;

                if (!canEditSportQuestions || dataItem.SportId != sportId)
                {
                    $('#Question').attr("disabled", true);
                }
            }
        }

    </script>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="MenuContent" runat="server">
    <% Html.RenderPartial("~/Views/Shared/NavMenu.ascx", Model.Menu); %>
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="HeaderImage" runat="server">
    <%= RingHTMLHelper.GetRandomHeaderImage() %>
</asp:Content>

<asp:Content ID="Content9" ContentPlaceHolderID="InfoBar" runat="server">
    <% Html.RenderPartial("~/Views/Shared/LanguageSelector.ascx", Model.CurrentLanguageCode); %> &nbsp;|&nbsp;
    <% Html.RenderPartial("~/Views/Shared/CurrentUserName.ascx", Model.CurrentUserName); %>
</asp:Content>