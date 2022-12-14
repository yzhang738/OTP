<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<EvaluationsQuestionsViewModel>" %>

<asp:Content ID="Content7" ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.RenderPartial("~/Views/Shared/JavaScriptCheck.ascx"); %>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	<%: Shared.EvaluationsStrings.Questions_Title %>
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
  
                <%
                Html.Telerik().Window()
                .Name("CopyVersion")
                .Title(Shared.EvaluationsStrings.Questions_CopyVersionWindow_Title)
                .Content(() =>
                    { %>
                        <% using (Html.BeginForm("CopyEvaluationVersion", "Evaluations"))
                        { %>
                            <div class="groupbox_600" style="margin:0px 0px 0px 0px">
                                <div class="groupbox_title_background gradientHToWhite">
                                    <div class="groupbox_title" >
                                        <%= string.Format(Shared.EvaluationsStrings.Questions_CopyVersionWindow_GroupBoxTitle,
                                            "<span id='copyVersionId'>" + ViewData["versionId"] + "</span>") %>
                                    </div>
                                </div>
                                <div class="groupbox_content">
                                    <div>
                                        <label><%: Shared.EvaluationsStrings.Questions_CopyVersionWindow_CopyToYearLabel %></label>
                                        <%         
                                            Html.Telerik().DropDownList()
                                                .Name("copyToYear")
                                                .DataBinding(binding => binding.Ajax().Select("EvaluationCopyToYearAjax", "Shared"))
                                                .HtmlAttributes(new { style = "font-size:8pt;width:150px" })
                                                .ClientEvents(events => events
                                                                                .OnDataBinding("onCopyToYearComboBinding")).Render();
                                        %>
                                    </div>
                                </div>
                                <img src="../../Content/Images/sidenav_btm.gif" width="600" height="9" /> 
                            </div> 
                            <div class="clear"></div>
                            <div style="float:right; text-align:right; margin: 5px 10px 0px 0px" >
                                <input type="submit" value="<%: Shared.EvaluationsStrings.Questions_CopyVersionWindow_SubmitButton %>" onclick="return onCopyEvaluationSubmit()" />
                            </div>
                            <input type="hidden" id="copyEvaluationVersionId" name="copyEvaluationVersionId" value="<%= ViewData["evaluationVersionId"] %>" />
                       <% } %>
                    <%})
                .Draggable(true)
                .Scrollable(false)
                .Resizable()
                .Buttons(b => b.Close())
                .Width(605)
                .Height(150)
                .Modal(true)
                .Visible(false) /* window not visible on initial load */
                .Render();
                        
            %>

            <% Html.RenderPartial("~/Views/Shared/CommentEditor.ascx"); %>
            <input type="hidden" id="evaluationVersionId" value="<%= ViewData["evaluationVersionId"] %>" />
            
            <div>
                <% Html.Telerik().Grid<EvaluationsQuestionsEvaluationVersionGridViewModel>(Model.VersionsGridModel)
                    .Name("Versions")
                    .TableHtmlAttributes(new { style = "font-size:8pt" })
                    .HtmlAttributes(new { style = "height:340px" })
                    .DataKeys(keys => keys.Add(v => v.EvaluationVersionId))
                    .DataBinding(dataBinding => dataBinding.Ajax()
                        .Select("LoadEvaluationsQuestionsEvaluationVersionGridAjax", "Evaluations")
                        .Insert("InsertEvaluationVersion", "Evaluations")
                        .Update("UpdateEvaluationVersion", "Evaluations")
                    )
                    .ClientEvents(events => events.OnEdit("onEditEvaluationsVersion")
                    )
                    .Pageable(paging => paging.PageSize(10))
                    .Scrollable(scrolling => scrolling.Height(281))
                    .Sortable()
                    .Selectable()
                    .Filterable()
                    .ToolBar(t =>
                        {
                            if (Model.Editable)
                            {
                                t.Insert().ButtonType(GridButtonType.ImageAndText)
                                    .HtmlAttributes(new { @class = "AddButton", id = "AddButton" });
                            }
                        })
                    .Resizable(resize => resize.Columns(true))
                    .ClientEvents(events => events
                                            .OnRowSelect("onVersionGridRowSelect")
                                            .OnRowDataBound("onVersionGridRowBound")
                                            .OnLoad("onVersionGridLoad")
                                            .OnError("grid_OnError"))
                    .RowAction(row =>
                    {
                        row.Selected = row.DataItem.EvaluationVersionId.Equals(ViewData["evaluationVersionId"]);
                    })
                    .Columns(columns =>
                    {
                        columns.Bound(v => v.VersionId)
                            .Width(180);
                        columns.Bound(v => v.OrganizationTypeId)
                            .Width(200);
                        columns.Bound(v => v.Name_EN)
                            .Width(230);
                        columns.Bound(v => v.Name_FR)
                            .Width(230);
                        columns.Bound(v => v.VersionPhase)
                            .Width(80);
                        columns.Bound(v => v.HasComments)
                            .ClientTemplate("<span onclick=\"openCommentWindow('EvaluationVersion', '<#= EvaluationVersionId #>', '<#= Name #>', '" + Model.Editable + "', 'true');\" class=\"comment sprite sprite-<#= HasComments ? \"note\" : \"note_none\" #>\"></span>")
                            .Template(v =>
                                { %>
                                    <span onclick="openCommentWindow('EvaluationVersion', '<%= v.EvaluationVersionId %>', '<%= v.Name %>', '<%= Model.Editable %>', 'true');" class="comment sprite sprite-<%= v.HasComments ? "note" : "note_none" %>"></span>
                                <%
                                })
                            .Width(60)
                            .Title("")
                            .Filterable(false);
                        columns.Command(commands =>
                        {
                            if (Model.Editable)
                            {
                                //The select button is replaced as the copy button
                                commands.Select().ButtonType(GridButtonType.ImageAndText).HtmlAttributes(new { style = "float:left", @class = "SelectButton", id = "SelectButton" });
                                commands.Edit().ButtonType(GridButtonType.ImageAndText).HtmlAttributes(new { style = "float:left" });
                            }
                        })
                        .Width(95);
                        columns.Bound(v => v.EvaluationVersionId).Hidden(true);
                    }).Render();
            
                %>
            </div>
  
            <div class="clear"></div>

            <div style="margin-top:40px">
                <h2><%= string.Format(Shared.EvaluationsStrings.Questions_QuestionGridLabel,
                          "<span id='versionId'>" + ViewData["versionId"].ToString() + "</span>") %></h2>
            </div>

            <div class="clear"></div>

            <div>
                <input id='showDeletedQuestions' name='showDeletedQuestions' type='checkbox' style='margin-right: 3px' onclick="onShowDeletedQuestionsClick()" /><%: Shared.EvaluationsStrings.Questions_ShowDeleted %>
                <% Html.Telerik().Grid<EvaluationsQuestionsEvaluationPillarsGridViewModel>()
                    .Name("Pillars")
                    .TableHtmlAttributes(new { style = "font-size:8pt;height:30px" })
                    .HtmlAttributes(new { style = "height:560px" })
                    .DataBinding(dataBinding => dataBinding.Ajax()
                        .Select("LoadEvaluationsQuestionsEvaluationPillarsGridAjax", "Evaluations", new { showDeleted = false })
                    )
                    .Scrollable(scrolling => scrolling.Height(501))
                    .Resizable(resize => resize.Columns(true))
                    .ClientEvents(events => events
                        .OnError("grid_OnError")
                    )
                    .Columns(columns =>
                    {
                        columns.Bound(a => a.EvaluationMasterPillarName)
                            .Width(100);
                        columns.Bound(a => a.Name)
                            .Width(250);
                        columns.Bound(a => a.Description);
                        columns.Bound(a => a.NumberOfQuestions)
                            .Width(140);
                        columns.Bound(a => a.EvaluationMasterPillarId).Hidden(true);
                        columns.Bound(a => a.EvaluationVersionId).Hidden(true);
                    })
                    .DetailView(d => d.ClientTemplate(
                        Html.Telerik().Grid<EvaluationsQuestionsEvaluationPillarQuestionsGridViewModel>()
                        .Name("Questions_<#= EvaluationMasterPillarId #>")
                        .DataKeys(keys => keys.Add(b => b.EvaluationVersionQuestionId))
                        .ToolBar(toolbar => 
                            {
                                if (Model.Editable)
                                {
                                    toolbar.Insert().ButtonType(GridButtonType.ImageAndText)
                                        .HtmlAttributes(new { @class = "AddButton_<#= EvaluationMasterPillarId #>", id = "AddButton_<#= EvaluationMasterPillarId #>" });
                                }
                            })
                        .ClientEvents(e => e.OnError("grid_OnError")
                                            .OnEdit("onQuestionGridEdit")
                                            .OnDataBinding("onQuestionGridDataBinding")
                                            .OnRowDataBound("onQuestionGridRowBinding"))
                        .DataBinding(dataBinding => dataBinding.Ajax()
                            .Select("LoadEvaluationsQuestionsEvaluationPillarQuestionsGridAjax", "Evaluations", new { evaluationVersionId = "<#= EvaluationVersionId #>", evaluationMasterPillarId = "<#= EvaluationMasterPillarId #>" })
                            .Insert("InsertEvaluationVersionQuestion", "Evaluations", new { evaluationVersionId = "<#= EvaluationVersionId #>", evaluationMasterPillarId = "<#= EvaluationMasterPillarId #>" })
                            .Update("UpdateEvaluationVersionQuestion", "Evaluations", new { evaluationVersionId = "<#= EvaluationVersionId #>", evaluationMasterPillarId = "<#= EvaluationMasterPillarId #>" })
                            .Delete("DeleteEvaluationVersionQuestion", "Evaluations", new { evaluationVersionId = "<#= EvaluationVersionId #>", evaluationMasterPillarId = "<#= EvaluationMasterPillarId #>" })
                        )
                        .Pageable()
                        .Sortable()
                        .Resizable(resize => resize.Columns(true))
                        .Filterable()
                        .Columns(columns =>
                        {
                            columns.Bound(b => b.SortOrder)
                                .Width(85);
                            columns.Bound(b => b.QuestionPhase)
                                .Width(130);
                            columns.Bound(b => b.SportId)
                                .ClientTemplate("<#= SportName  #>")
                                .Width(250);
                            columns.Bound(b => b.VersionQuestion);
                            columns.Bound(b => b.HasComments)
                                .ClientTemplate("<span onclick=\"openCommentWindow('EvaluationVersionQuestion', '<#= EvaluationVersionQuestionId #>', '" + Shared.EvaluationsStrings.Questions_Question + " <#= SortOrder #>', '" + Model.Editable + "', 'true');\" class=\"comment sprite sprite-<#= HasComments ? \"note\" : \"note_none\" #>\"></span>")
                                .Width(37)
                                .Title("")
                                .Filterable(false);
                            columns.Command(commands =>
                            {
                                if (Model.Editable)
                                {
                                    commands.Edit().ButtonType(GridButtonType.ImageAndText)
                                        .HtmlAttributes(new { style = "float:left", @class = "EditButton_<#= EvaluationMasterPillarId #>", id = "EditButton_<#= EvaluationMasterPillarId #>" });
                                    commands.Delete().ButtonType(GridButtonType.ImageAndText)
                                      .HtmlAttributes(new { style = "float:left", @class = "DeleteButton_<#= EvaluationMasterPillarId #>", id = "DeleteButton_<#= EvaluationMasterPillarId #>" });
                                }
                            })
                                .Width(95);
                            columns.Bound(b => b.EvaluationVersionQuestionId).Hidden(true);
                        }).ToHtmlString()
                    )).Render();
                
                %>
            </div>
        </div>
    </div>

    <script type="text/javascript">

        function onVersionGridLoad(e)
        {
            CreateCopyButton($(".SelectButton"))
        }

        function onVersionGridRowBound(e)
        {
            CreateCopyButton($("#SelectButton"));
        }

        function CreateCopyButton(e)
        {
            e.replaceWith("<div class=\"t-grid-action t-button t-state-default t-grid-edit\" onmouseover=\"$(this).addClass('t-button-hover');\" onmouseleave=\"$(this).removeClass('t-button-hover');\" onclick=\"copyEvaluationVersion();\"><span class=\"t-add t-icon\"></span><span class=\"button-span\"><%: Shared.EvaluationsStrings.Questions_CopyButton %></span></div>");
        }

        function copyEvaluationVersion()
        {
            openCopyVersionWindow();
        }

        function onVersionGridRowSelect(e)
        {
            var evaluationVersionId = e.row.cells[e.row.cells.length - 1].innerHTML;

            $("#evaluationVersionId").val(evaluationVersionId);
            $('#versionId').text(e.row.cells[0].innerHTML);

            $("#copyEvaluationVersionId").val(evaluationVersionId);
            $('#copyVersionId').text(e.row.cells[0].innerHTML);
            
            RebindPillarsGrid();
        }

        function onEditEvaluationsVersion(e) 
        {
            if (e.mode == 'insert') {
                var versionName = 'Version 1';
                $('#Name_EN').val(versionName);
                $('#Name_FR').val(versionName);
            }
        }

        function RebindPillarsGrid()
        {
            var pillarsGrid = $('#Pillars').data('tGrid');

            // rebind the related grid
            pillarsGrid.rebind({
                evaluationVersionId: $("#evaluationVersionId").val(), showDeleted: $('#showDeletedQuestions').is(':checked')
            });
        }

        function onQuestionGridEdit(e)
        {
            var mode = e.mode;

            if (mode == 'edit')
            {
                var dataItem = e.dataItem;

                if (dataItem.SortOrder == 0)
                {
                    $(e.form).find('#SortOrder').attr("disabled", true);
                    $(e.form).find('#QuestionPhase').data('tDropDownList').disable();
                    $(e.form).find('#SportId').data('tDropDownList').disable();
                }
            }
            else
            {
                var largestSortOrder = 0;

                $(e.target).find('tbody tr:not(.t-grid-new-row)').slice(1).each(function ()
                {
                    if (parseInt(this.cells[0].innerHTML) > largestSortOrder)
                    {
                        largestSortOrder = parseInt(this.cells[0].innerHTML);
                    }
                })

                $('#SortOrder').val(largestSortOrder + 1);
            }
        }

        function onCopyToYearComboBinding(e)
        {
            var evaluationVersionId = $("#copyEvaluationVersionId").val();
            e.data = $.extend({}, e.data, { evaluationVersionId: evaluationVersionId });
        }

        function openCopyVersionWindow()
        {
            $("#CopyVersion").data("tWindow").center().open();
            $("#copyToYear").data('tDropDownList').reload();
        }

        function onShowDeletedQuestionsClick()
        {
            RebindPillarsGrid();
        }

        function onQuestionGridDataBinding(e)
        {
            e.data = $.extend({}, e.data, { showDeleted: $('#showDeletedQuestions').is(':checked') });
        }

        function onCopyEvaluationSubmit()
        {
            var yearCombo = $("#copyToYear").data('tDropDownList');
            SetRingCookie('ck_year', yearCombo.text(), { path: '/' });
            return true;
        }

        function onQuestionGridRowBinding(e)
        {
            if (!e.dataItem.Editable)
            {
                $("#EditButton_" + e.dataItem.EvaluationMasterPillarId).replaceWith('<div></div>');
                $("#AddButton_" + e.dataItem.EvaluationMasterPillarId).replaceWith('<div></div>');
                $("#DeleteButton_" + e.dataItem.EvaluationMasterPillarId).replaceWith('<div></div>');
            }
            else if (e.dataItem.QuestionPhase != "NEW")
            {
                $("#DeleteButton_" + e.dataItem.EvaluationMasterPillarId).replaceWith('<div></div>');
            }
        }

    </script>

</asp:Content>

