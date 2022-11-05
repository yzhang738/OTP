<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ReportingDesignAnalysisViewModel>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.RenderPartial("~/Views/Shared/JavaScriptCheck.ascx"); %>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	<%:Shared.ReportingStrings.DesignAnalysis_Title %> - <%: ViewData["reportName"] %>
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="MenuContent" runat="server">
    <% Html.RenderPartial("~/Views/Shared/NavMenu.ascx", Model.Menu); %>
</asp:Content>

<asp:Content ID="Content8" ContentPlaceHolderID="InfoBar" runat="server">
    <% Html.RenderPartial("~/Views/Shared/LanguageSelector.ascx", Model.CurrentLanguageCode); %> &nbsp;|&nbsp;
    <% Html.RenderPartial("~/Views/Shared/CurrentUserName.ascx", Model.CurrentUserName); %>
</asp:Content>

<asp:Content ID="Content10" ContentPlaceHolderID="HeaderImage" runat="server">
    <%= RingHTMLHelper.GetRandomHeaderImage() %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

    <div "viewcontent">
        <input type="hidden" id="reportId" value="<%= ViewData["reportId"] %>" />
        <% Html.RenderPartial("~/Views/Shared/CommentEditor.ascx"); %>
        
        <%
            Html.Telerik().Window()
            .Name("CopyReport")
            .Title(Shared.ReportingStrings.DesignAnalysis_CopyReport)
            .Content(() =>
                { %>
                    <% using (Html.BeginForm("CopyReport", "Reporting"))
                    { %>
                        <div class="groupbox_600" style="margin:0px 0px 0px 0px">
                            <div class="groupbox_title_background gradientHToWhite">
                                <div class="groupbox_title" >
                            
                                </div>
                            </div>
                            <div class="groupbox_content">
                                <div>
                                    <%: Shared.ReportingStrings.DesignAnalysis_CopyReport_Prompt %>
                                </div>
                            </div>
                            <img src="../../Content/Images/sidenav_btm.gif" width="600" height="9" /> 
                        </div> 
                        <div class="clear"></div>
                        <div style="float:right; text-align:right; margin: 5px 10px 0px 0px" >
                            <input type="submit" value="<%: Shared.SharedStrings.Button_Copy %>" />
                            <input style="width:75px" type="button" value="<%: Shared.SharedStrings.Button_Cancel %>" onclick="$('#CopyReport').data('tWindow').close();" />
                        </div>
                        <input type="hidden" id="copyReportId" name="copyReportId" />
                    <% } %>
                <%})
            .Draggable(true)
            .Scrollable(false)
            .Resizable()
            .Buttons(b => b.Close())
            .Width(605)
            .Height(100)
            .Modal(true)
            .Visible(false) /* window not visible on initial load */
            .Render();
                        
        %>

        <div>
            <% Html.Telerik().Grid<ReportingDesignAnalysisReportGridViewModel>()
            .Name("Reports")
            .TableHtmlAttributes(new { style = "font-size:8pt;" })
            .HtmlAttributes(new { style = "height:345px" })
            .DataKeys(keys => keys.Add(a => a.ReportId))
            .DataBinding(dataBinding => dataBinding.Ajax()
                .Select("DesignAnalysisReportGridAjax", "Reporting")
                .Insert("InsertReport", "Reporting")
                .Update("UpdateReport", "Reporting")
                .Delete("DeleteReport", "Reporting")
            )
            .Pageable(paging => paging.PageSize(10))
            .Scrollable(scrolling => scrolling.Height(286))
            .Sortable()
            .Selectable()
            .Filterable()
            .Resizable(resize => resize.Columns(true))
            .ToolBar(t => 
                {
                    if (Model.Editable) t.Insert().ButtonType(GridButtonType.ImageAndText);
                })
            .ClientEvents(events => events.OnEdit("onReportsGridEdit")
                                          .OnRowSelect("onReportsGridRowSelect")
                                          .OnRowDataBound("onReportsGridRowBound")
                                          .OnError("grid_OnError")
                                          .OnSave("onReportsGridSave"))
            .Columns(columns =>
            {
                columns.Bound(a => a.ReportCategoryId)
                    .ClientTemplate("<#= ReportCategoryName #>")
                    .Width(170);
                columns.Bound(a => a.ReportViewId)
                    .ClientTemplate("<#= ReportViewName #>")
                    .Width(150);
                columns.Bound(a => a.Name_EN)
                    .Width(175);
                columns.Bound(a => a.Name_FR)
                    .Width(175);
                columns.Bound(a => a.SecurityUserSport)
                    .ClientTemplate("<span <#= SecurityUserSport ? \"class='t-icon t-update'\" : \"\" #>></span>")
                    .Width(105)
                    .HtmlAttributes(new { style = "text-align:center" });
                columns.Bound(a => a.SportId)
                    .ClientTemplate("<#= SportName #>")
                    .Width(220);
                columns.Bound(a => a.Description_EN)
                    .Width(250);
                columns.Bound(a => a.Description_FR)
                    .Width(250);
                columns.Bound(a => a.Locked)
                    .ClientTemplate("<span <#= Locked ? \"class='t-icon t-update'\" : \"\" #>></span>")
                    .Width(75)
                    .HtmlAttributes(new { style = "text-align:center" });
                columns.Command(a =>
                    {
                        if (Model.Editable)
                        {
                            a.Edit().ButtonType(GridButtonType.ImageAndText);
                            a.Delete().ButtonType(GridButtonType.ImageAndText);
                            a.Select().ButtonType(GridButtonType.ImageAndText).HtmlAttributes(new { style = "float:left", id = "SelectButton" });
                        }
                    })
                    .Width(95);
                columns.Bound(a => a.ReportId).Hidden(true);
            }).Render();
                
            %>
        </div>

        <div class="clear"></div>

        <div style="margin:40px 0px 0px 0px">
        <% Html.Telerik().TabStrip()
            .Name("TabStrip")
            .ClientEvents(e => e.OnSelect("onTabSelect"))
            .Items(tabstrip =>
            {
                tabstrip.Add()
                    .Text(Shared.ReportingStrings.DesignAnalysis_ReportGroupings)
                    .SpriteCssClasses("sprite sprite-layout_content")
                    .LoadContentFrom("DesignAnalysisGroupingInfo", "Reporting", new { reportId = ViewData["reportId"], editable = Model.Editable && !(bool)ViewData["reportLocked"] });
                tabstrip.Add()
                    .Text(Shared.ReportingStrings.DesignAnalysis_ReportFilters)
                    .SpriteCssClasses("sprite sprite-layout_header")
                    .LoadContentFrom("DesignAnalysisFiltersInfo", "Reporting", new { ReportId = ViewData["reportId"], editable = Model.Editable && !(bool)ViewData["reportLocked"] });
                tabstrip.Add()
                    .Text(Shared.ReportingStrings.DesignAnalysis_ReportSecurity)
                    .SpriteCssClasses("sprite sprite-report_user")
                    .LoadContentFrom("DesignAnalysisSecurityInfo", "Reporting", new { ReportId = ViewData["reportId"], editable = Model.Editable });
                tabstrip.Add()
                    .Text(Shared.ReportingStrings.DesignAnalysis_TestReport)
                    .SpriteCssClasses("sprite sprite-report_go")
                    .LinkHtmlAttributes(new { @class = "TestReportTab" })
                    .LoadContentFrom("DesignAnalysisTestReportView", "Reporting", new { ReportId = ViewData["reportId"] });
            })
            .SelectedIndex(0)
            .Render();
        %>
        </div>

       <div class="clear"></div>
        <br />

    </div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ScriptContent" runat="server">
    <script type="text/javascript">

        var refreshReport = true;
        var insertingNewRow = false;
        var updatingExistingRow = false;

        function onReportsGridEdit(e)
        {
            var mode = e.mode;

            if (mode == 'edit')
            {
                var dataItem = e.dataItem;
                $(e.form).find('#ReportViewId').data('tDropDownList').disable();
            }
        }

        function onReportsGridSave(e)
        {
            if (e.dataItem)
            {
                //updating
                updatingExistingRow = true;
                $("#reportId").val(e.dataItem.ReportId.toString())
            }
            else
            {
                //inserting
                insertingNewRow = true;
            }
        }

        function onReportsGridRowSelect(e)
        {
            var currentReportId = $("#reportId").val();
            var selectedReportId = e.row.cells[e.row.cells.length - 1].innerHTML;

            if (currentReportId != selectedReportId)
            {
                window.location.pathname = "Reporting/DesignAnalysis/" + selectedReportId.toString();
            }
        }

        function onReportsGridRowBound(e)
        {
            if (insertingNewRow)
            {
                var grid = $('#Reports').data('tGrid');
                var lastRow = grid.$tbody.find(' > tr:last');
                var lastItem = grid.dataItem(lastRow);

                $("#reportId").val(lastItem.ReportId.toString())
            }

            if (insertingNewRow || updatingExistingRow)
            {
                insertingNewRow = false;
                updatingExistingRow = false;
                window.location.pathname = "Reporting/DesignAnalysis/" + $("#reportId").val();
            }
            else
            {
                CreateCopyButton($("#SelectButton"), e.row.cells[e.row.cells.length - 1].innerHTML);

                var reportId = $("#reportId").val();

                var row = $(e.row);

                if (e.row.cells[e.row.cells.length - 1].innerHTML == reportId)
                {
                    $(e.row).addClass('t-state-selected');
                }
            }
        }

        function onTabSelect(e)
        {
            //When Test Report tab is clicked after the initial load, reload the values
            if ($(e.item).find('.TestReportTab').length)
            {
                var $item = $(e.item);
                var $link = $item.find('.t-link');
                var contentUrl = $link.data('ContentUrl');
                if (contentUrl)
                {
                    onReportChange();
                }
            }
        }

        function CreateCopyButton(e, reportId)
        {
            e.replaceWith("<a class='t-grid-action t-button t-state-default' href='#' onclick='copyReport(" + reportId + ");'><span class='t-icon t-add'></span><%: Shared.EvaluationsStrings.Questions_CopyButton %></a>");
        }

        function copyReport(reportId)
        {
            $("#copyReportId").val(reportId);
            $("#CopyReport").data("tWindow").center().open();
        }
    </script>

</asp:Content>