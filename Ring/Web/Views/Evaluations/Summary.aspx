<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<EvaluationsSummaryViewModel>" %>

<asp:Content ID="Content7" ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.RenderPartial("~/Views/Shared/JavaScriptCheck.ascx"); %>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	<%: Shared.EvaluationsStrings.Summary_Title %>
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
    <div>
        <div style="float:left;width:49%;text-align:right;margin:5px 0px 5px 0px;">
            <%: Shared.EvaluationsStrings.Summary_Version %>
        </div>
        <div style="float:right;width:50%;text-align:left;margin:5px 0px 5px 0px;">

            <%= Html.Telerik().DropDownList()
                .Name("VersionSelector")
                .HtmlAttributes(new { style = "width:200px;font-size:12px" })
                .SelectedIndex(Model.SelectedEvaluationVersionIndex)
                .BindTo(new SelectList(Model.EvaluationVersions, "Id", "Description"))
                      .ClientEvents(e => e.OnClose("changeVersion"))
                                  
            %>

        </div> 
    </div>
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="HeaderImage" runat="server">
    <%= RingHTMLHelper.GetRandomHeaderImage() %>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">
    <div id="container">
        <div style="float:left; width:100%;">
            <% Html.RenderPartial("~/Views/Shared/CommentEditor.ascx"); %>

            <%
                Html.Telerik().Window()
                .Name("AddEvaluationWindow")
                .Title(Shared.EvaluationsStrings.Summary_AddEvaluationWindow_Title)
                .Content(() =>
                    { %>
                        <% using (Html.BeginForm("AddEvaluation", "Evaluations"))
                        { %>
                            <div class="groupbox_600" style="margin:0px 0px 0px 0px">
                                <div class="groupbox_title_background gradientHToWhite">
                                    <div class="groupbox_title" >
                                        <%= string.Format(Shared.EvaluationsStrings.Summary_AddEvaluationWindow_GroupBoxTitle,
                                                "<span id='versionId'></span>") %>
                                    </div>
                                </div>
                                <div class="groupbox_content">
<%--                                        <label><%: Shared.EvaluationsStrings.Summary_AddEvaluationWindow_SportLabel %></label>
                                        <%         
                                            Html.Telerik().DropDownList()
                                                .Name("sportId")
                                                .DataBinding(binding => binding.Ajax().Select("EvaluationAddSportIdAjax", "Shared", new { writeAccess = Model.Menu.Menu.WriteAccess }))
                                                .HtmlAttributes(new { style = "font-size:8pt;width:200px;margin-top:5px" })
                                                .ClientEvents(events => events
                                                    .OnDataBinding("onSportIdComboBinding")).Render();
                                        %>
--%>                                
                                    <table style="width: 100%; margin-top:10px">        
                                        <tr>
                                            <td style="width:20px"><label><%: Shared.EvaluationsStrings.Summary_AddEvaluationWindow_SportLabel %></label></td>
                                            <td ><%         
                                                Html.Telerik().DropDownList()
                                                    .Name("sportId")
                                                    .DataBinding(binding => binding.Ajax().Select("EvaluationAddSportIdAjax", "Shared", new { writeAccess = Model.Menu.Menu.WriteAccess }))
                                                    .HtmlAttributes(new { style = "font-size:8pt;width:200px;" })
                                                    .ClientEvents(events => events
                                                        .OnDataBinding("onSportIdComboBinding")).Render();
                                                %>  
                                            </td>
                                        </tr>
                                    </table>

                                </div>
                                <img src="../../Content/Images/sidenav_btm.gif" width="600" height="9" /> 
                            </div> 
                            <div class="clear"></div>
                            <div style="float:right; text-align:right; margin: 15px 10px 0px 0px" >
                                <input type="submit" value="<%: Shared.EvaluationsStrings.Summary_AddEvaluationWindow_SubmitButton %>" />
                            </div>
                            <input type="hidden" id="evaluationVersionId" name="evaluationVersionId" value="" />
                        <% } %>
                    <%})
                .Draggable(true)
                .Scrollable(false)
                .Resizable()
                .Buttons(b => b.Close())
                .Width(605)
                .Height(130)
                .Modal(true)
                .Visible(false) /* window not visible on initial load */
                .Render();
            %>


             <%
                Html.Telerik().Window()
                .Name("CopyEvaluation")
                .Title(Shared.EvaluationsStrings.Questions_CopyEvaluationWindow_Title)
                .Content(() =>
                    { %>
                        <% using (Html.BeginForm("CopyEvaluation", "Evaluations", FormMethod.Post, new { name = "copyEvalutionForm" }))
                           { %>
                            <div class="groupbox_600" style="margin:0px 0px 0px 0px">
                                <div class="groupbox_title_background gradientHToWhite">
                                    <div class="groupbox_title" >
                                        <%= string.Format(Shared.EvaluationsStrings.Questions_CopyEvaluationWindow_Heading,
                                                "<span id='sportName'></span>",
                                                "<span id='versionIdCopy'></span>")%>
                                    </div>
                                </div>
                                <div class="groupbox_content">
                                    <div>
                                        <h4><%: Shared.EvaluationsStrings.Questions_CopyEvaluationsWindow_Warning%></h4>
<%--                                        <label><%: Shared.EvaluationsStrings.Questions_CopyEvaluationsWindow_Label%></label>

                                        <% Html.Telerik().DropDownList()
                                                .Name("copyToEvaluationVersionId")
                                                .DataBinding(binding => binding.Ajax().Select("EvaluationCopyToVersionAjax", "Shared"))
                                                .HtmlAttributes(new { style = "font-size:8pt;width:300px" })
                                                .ClientEvents(events => events
                                                    .OnDataBinding("oncopyToEvaluationVersionComboBinding")).Render(); 
                                        %>
--%>                                    
                                    <table style="width: 100%; margin-top:10px">        
                                        <tr>
                                            <td ><label><%: Shared.EvaluationsStrings.Questions_CopyEvaluationsWindow_Label%></label></td>
                                            <td >         
                                                <% Html.Telerik().DropDownList()
                                                        .Name("copyToEvaluationVersionId")
                                                        .DataBinding(binding => binding.Ajax().Select("EvaluationCopyToVersionAjax", "Shared"))
                                                        .HtmlAttributes(new { style = "font-size:8pt;width:320px" })
                                                        .ClientEvents(events => events
                                                            .OnDataBinding("oncopyToEvaluationVersionComboBinding")).Render(); 
                                                %>
                                            </td>
                                        </tr>
                                    </table>
                                    </div>
                                </div>
                                <img src="../../Content/Images/sidenav_btm.gif" width="600" height="9" /> 
                            </div> 
                            <div class="clear"></div>
                            <div style="float:right; text-align:right; margin: 20px 10px 0px 0px" >
                                <input type="button" value="<%: Shared.EvaluationsStrings.Questions_CopyVersionWindow_SubmitButton %>" onClick="submitCopyEvaluations()" />
                            </div>
                            <input type="hidden" id="evaluationId" name="evaluationId" />
                            <input type="hidden" id="copyFromEvaluationVersionId" name="copyFromEvaluationVersionId" />
                       <% } %>
                    <%})
                .Draggable(true)
                .Scrollable(false)
                .Resizable()
                .Buttons(b => b.Close())
                .Width(605)
                .Height(200)
                .Modal(true)
                .Visible((bool)ViewData["ShowCopyWindow"]) /* window not visible on initial load */
                .Render();
                        
            %>
            
            <h3 style="color:Red"><%: Html.ValidationSummary(true) %></h3>

            <% if (Model.Menu.Menu.WriteAccess != Constant.AccessLevel.No && Model.EvaluationVersionEditable)
               { %>
            <div style="float:left; margin:10px 10px 5px 0px;" class="t-button t-state-default" onmouseover="$(this).addClass('t-button-hover');" onmouseleave="$(this).removeClass('t-button-hover');" onclick="openAddEvaluationWindow()">
                <span class="t-add t-icon"></span><span class="button-span"><%: Shared.EvaluationsStrings.Summary_AddEvaluationButton %></span>
            </div>
            <% } %>

            <div class="clear"></div>

            <% Html.Telerik().Grid<EvaluationsSummaryGridViewModel>(Model.Evaluations)
                .Name("Evaluations")
                .TableHtmlAttributes(new { style = "font-size:8pt;" })
                .HtmlAttributes(new { style = "height:415px;width:1100px" })
                .ClientEvents(e => e.OnError("grid_OnError")
                                    .OnLoad("onEvaluationGridLoad")
                                    .OnRowDataBound("onEvaluationGridRowBound")
                                    .OnRowSelect("onEvaluationGridRowSelect"))
                .DataBinding(dataBinding => dataBinding.Server())
                .DataKeys(keys => keys.Add(e => e.EvaluationId))
                .Pageable(paging => paging.PageSize(14))
                .Scrollable(scrolling => scrolling.Height(388))
                .Resizable(resize => resize.Columns(true))
                .Selectable()
                .Sortable()
                .Filterable()
                .Columns(columns =>
                {
                    int evalIndex = 0;
                    columns.Bound(e => e.SportName)
                        .Template(e =>
                        {
                            %>
                                <%: Html.ActionLink(e.SportName, "EvaluationForm", new { id = e.EvaluationId.ToString() } ) %>
                            <%  
                        })
                        .Width(100);
                    columns.Bound(e => e.Rank)
                        .Width(55);
                    columns.Bound(e => e.EvaluationPhase)
                        .Width(75);

                    if (Model.Evaluations.Count > 0
                        && Model.Evaluations[evalIndex].Pillars.Count() > 0)
                    {
                        Action<int> addPillarColumn = (i) => {
                            columns.Bound(e => e.Pillars.ElementAt(i).Description)
                            .Template(e =>
                            {
                                %>
                                    <span class="sprite sprite-StopLight_<%= e.Pillars.ElementAt(i).Description %>"></span>
                                <%  
                            })
                            .Title(Model.Evaluations[evalIndex].Pillars.ElementAt(i).Id)
                            .Filterable(false)
                            .Sortable(false)
                            .HtmlAttributes(new { style = "text-align:center" })
                            .Width(75);
                        };
                
                        for (int i = 0; i < Model.Evaluations[evalIndex].Pillars.Count(); i++)
                        {
                            addPillarColumn(i);
                        }
                    }

                    columns.Template(e =>
                    {
                        %>
                            <%: Html.ActionLink(string.Format("{0}/{1}", e.OpenActionItems, e.TotalActionItems), "ActionItems", new { referenceId = e.EvaluationId.ToString() })%>
                        <%  
                    })
                        .Title(Shared.EvaluationsStrings.Summary_ActionItemsColumnTitle)
                        .Width(150);
                    columns.Command(commands =>
                        {
                            if (Model.Menu.Menu.WriteAccess != Constant.AccessLevel.No)
                            {
                                //The select button is replaced as the copy button
                                commands.Select().ButtonType(GridButtonType.ImageAndText).HtmlAttributes(new { style = "float:left", @class = "SelectButton", id = "SelectButton" });
                            }
                        }).Width(95);
                    columns.Bound(e => e.EvaluationId).Hidden(true);
                
                    evalIndex++;
                })
                .DetailView(detailView => detailView.Template(x =>
               {
                    %>
                        <% Html.Telerik().Grid(x.EvaluationOverallQuestions)
                                .ClientEvents(e => e.OnError("grid_OnError"))
                                .Name("Questions_" + x.EvaluationId)
                                .HtmlAttributes(new { Style = "width:952px" })
                                .Sortable()
                                .Resizable(resize => resize.Columns(true))
                                .Filterable()
                                .Columns(columns =>
                                {
                                    columns.Bound(b => b.EvalutionMasterPillarId)
                                        .Width(80);
                                    columns.Bound(b => b.VersionQuestion)
                                        .Width(321);
                                    columns.Bound(b => b.HPAAssessment)
                                        .Width(321);
                                    columns.Bound(b => b.EvaluationStatusInitial)
                                        .Template(b =>
                                            {                                            
                                                %> <span class="sprite sprite-StopLight_<%= b.EvaluationStatusInitial %>"></span> <%
                                            })
                                        .Filterable(false)
                                        .Sortable(false)
                                        .Width(100)
                                        .HtmlAttributes(new { style = "text-align:center" });
                                    columns.Bound(b => b.EvaluationStatusCurrent)
                                        .Template(b =>
                                            {                                            
                                                %> <span class="sprite sprite-StopLight_<%= b.EvaluationStatusCurrent %>"></span> <%
                                            })
                                        .Filterable(false)
                                        .Sortable(false)
                                        .Width(100)
                                        .HtmlAttributes(new { style = "text-align:center" });
                                    columns.Bound(b => b.EvaluationQuestionId).Hidden(true);
                                }).Render();
                        %>
                      <%
               })).Render();
            
            %>
  
            <br />
            <br />

        </div>
    </div>
</asp:Content>

<asp:Content ID="Content8" ContentPlaceHolderID="ScriptContent" runat="server">
    <script type="text/javascript">
        function onEvaluationGridLoad(e)
        {
            CreateCopyButton($(".SelectButton"))
        }

        function onEvaluationGridRowBound(e)
        {
            CreateCopyButton($("#SelectButton"));
        }

        function CreateCopyButton(e)
        {
            e.replaceWith("<div class=\"t-grid-action t-button t-state-default t-grid-edit\" onmouseover=\"$(this).addClass('t-button-hover');\" onmouseleave=\"$(this).removeClass('t-button-hover');\" onclick=\"copyEvaluation();\"><span class=\"t-add t-icon\"></span><span class=\"button-span\"><%: Shared.EvaluationsStrings.Questions_CopyButton %></span></div>");
        }

        function copyEvaluation()
        {
            $("#CopyEvaluation").data("tWindow").center().open();
        }

        function onEvaluationGridRowSelect(e)
        {
            var sportName = e.row.cells[1].innerText;
            var evaluationId = e.row.cells[e.row.cells.length - 1].innerHTML;
            var versionCombo = $('#VersionSelector').data('tDropDownList');

            $("#sportName").text(sportName);
            $('#versionIdCopy').text(versionCombo.text());
            $('#evaluationId').val(evaluationId);
        }

        function changeVersion()
        {
            var editor = $(this).data('tDropDownList');
            window.location.href = '/Evaluations/Summary/' + editor.value();
        }

        function openAddEvaluationWindow()
        {
            var versionCombo = $('#VersionSelector').data('tDropDownList');
            var versionId = versionCombo.value();

            if (versionCombo.value())
            {
                $('#versionId').text(versionCombo.text());
                $('#evaluationVersionId').val(versionCombo.value());
                $("#AddEvaluationWindow").data("tWindow").center().open();
                $("#sportId").data('tDropDownList').reload();
            }
            else
            {
                OTPAlert("<%: Shared.EvaluationsStrings.Summary_Error_NoEvaluation %>");
            }
        }

        function onSportIdComboBinding(e)
        {
            var evaluationVersionId = $("#evaluationVersionId").val();
            e.data = $.extend({}, e.data, { evaluationVersionId: evaluationVersionId });
        }

        function oncopyToEvaluationVersionComboBinding(e)
        {
            var versionCombo = $('#VersionSelector').data('tDropDownList');
            var copyFromEvaluationVersionId = versionCombo.value();
            e.data = $.extend({}, e.data, { copyFromEvaluationVersionId: copyFromEvaluationVersionId });
        }

        function submitCopyEvaluations()
        {
            if ($('#copyToEvaluationVersionId').data('tDropDownList').value())
            {
                $('#copyFromEvaluationVersionId').val($('#VersionSelector').data('tDropDownList').value());
                document.copyEvalutionForm.submit();
            }
            else
            {
                alert("<%: Shared.EvaluationsStrings.Summary_Error_NoCopyToVersionSelected %>");
            }
        }

    </script>
</asp:Content>

