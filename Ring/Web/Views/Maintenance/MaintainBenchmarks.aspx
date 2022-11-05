<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MaintenanceBenchmarksViewModel>" %>

<asp:Content ID="Content7" ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.RenderPartial("~/Views/Shared/JavaScriptCheck.ascx"); %>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	<%: Shared.MaintenanceStrings.MaintainBenchmarks_Title %> <%: ViewData["sportName"] %>
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
    <h3 style="color:Red"><%: Html.ValidationSummary(true) %></h3>
    
    <% if (Model.SelectedSport == null)
       { %>
        <h3><%: Shared.MaintenanceStrings.MaintainBenchmarks_Error_NoSelectedSport%></h3>
       <%}
       else
       { %>

               <%
            Html.Telerik().Window()
            .Name("InsertBenchmarkTier")
            .Title(" ")
            .Content(() =>
                { %>
                    <% using (Html.BeginForm("InsertBenchmarkTier", "Maintenance"))
                    { %>
                        
                        <div class="groupbox_600" style="margin:0px 0px 0px 0px">
                            <div class="groupbox_title_background gradientHToWhite">
                                <div class="groupbox_title" >
                                    <%: Shared.MaintenanceStrings.MaintainBenchmarks_InsertBenchmark_GroupBoxTitle %> <%: ViewData["sportName"] %>
                                </div>
                            </div>
                            <div class="groupbox_content">
                                <div class="editor-label">
                                    <%: Html.LabelFor(model => model.InsertBenchmarkTier.Name_EN) %>
                                </div>
                                <div class="editor-field">
                                    <%: Html.TextBoxFor(model => model.InsertBenchmarkTier.Name_EN) %>
                                    <%: Html.ValidationMessageFor(model => model.InsertBenchmarkTier.Name_EN) %>
                                </div>
            
                                <div class="editor-label">
                                    <%: Html.LabelFor(model => model.InsertBenchmarkTier.Name_FR) %>
                                </div>
                                <div class="editor-field">
                                    <%: Html.TextBoxFor(model => model.InsertBenchmarkTier.Name_FR) %>
                                    <%: Html.ValidationMessageFor(model => model.InsertBenchmarkTier.Name_FR) %>
                                </div>
            
                                <div class="editor-label">
                                    <%: Html.LabelFor(model => model.InsertBenchmarkTier.PositionStart) %>
                                </div>
                                <div class="editor-field">
                                    <%: Html.TextBoxFor(model => model.InsertBenchmarkTier.PositionStart) %>
                                    <%: Html.ValidationMessageFor(model => model.InsertBenchmarkTier.PositionStart) %>
                                </div>
            
                                <div class="editor-label">
                                    <%: Html.LabelFor(model => model.InsertBenchmarkTier.PositionEnd) %>
                                </div>
                                <div class="editor-field">
                                    <%: Html.TextBoxFor(model => model.InsertBenchmarkTier.PositionEnd) %>
                                    <%: Html.ValidationMessageFor(model => model.InsertBenchmarkTier.PositionEnd) %>
                                </div>
                            </div>
                            <img src="../../Content/Images/sidenav_btm.gif" width="600" height="9" /> 
                        </div> 
                        <div class="clear"></div>
                        <div style="float:right; text-align:right; margin: 5px 10px 0px 0px" >
                            <input type="submit" value="<%: Shared.SharedStrings.Button_Save %>" />
                            
                        </div>
                        <%: Html.Hidden("sportId", Model.SelectedSport)  %>

                          <%
                    } %>
                <%})
            .Draggable(true)
            .Scrollable(false)
            .Resizable()
            .Buttons(b => b.Close())
            .Width(605)
            .Height(230)
            .Modal(true)
            .Visible(false) /* window not visible on initial load */
            .Render();
                        
        %>

        <% Html.RenderPartial("~/Views/Shared/CommentEditor.ascx"); %>

        <div style="float:left; margin:10px 10px 5px 0px;" class="t-button t-state-default" onmouseover="$(this).addClass('t-button-hover');" onmouseleave="$(this).removeClass('t-button-hover');" onclick="openTierWindow()">
            <span class="t-add t-icon"></span><span class="button-span"><%: Shared.MaintenanceStrings.MaintainBenchmarks_AddBenchmarkTierButton %></span>
        </div>
        <div class="clear"></div>

          <%= Html.Telerik().Grid<MaintenanceBenchmarksGridSportViewModel>()
            .Name("Sports")
                .TableHtmlAttributes(new { style = "font-size:8pt;" })
                    .HtmlAttributes(new { style = "height:535px" })
            .DataBinding(dataBinding => dataBinding.Ajax()
                .Select("BenchmarksSportsGridAjax", "Maintenance")
            )
            .Pageable(paging => paging.PageSize(14))
            .Scrollable(scrolling => scrolling.Height(510))
            .Resizable(resize => resize.Columns(true))
            .Sortable()
            .Filterable()
            .Columns(columns =>
            {
                columns.Bound(s => s.BenchmarkLevel);
                columns.Bound(s => s.Sport)
                    .ClientTemplate("<# if (Sport) { #><#= Sport.Description #> <#} else { #> none <# } #>");
                columns.Bound(s => s.Gender).Width(37);
                columns.Bound(s => s.Discipline)
                    .ClientTemplate("<# if (Discipline) { #><#= Discipline.Description #> <#} else { #> {All} <# } #>");
                columns.Bound(s => s.Event)
                    .ClientTemplate("<# if (Event) { #><#= Event.Description #> <#} else { #> {All} <# } #>");
                columns.Bound(s => s.BenchmarksEntered)
                    .ClientTemplate("<span class='<#= BenchmarksEntered ? \"t-update\" : \"t-delete\" #> t-icon'></span>")
                    .Width(100)
                    .HtmlAttributes(new { style = "text-align:center" });
                columns.Bound(s => s.SportId).Hidden(true);
            })
            .DetailView(ordersDetailView => ordersDetailView.ClientTemplate(
                Html.Telerik().Grid<MaintenanceBenchmarksGridSportBenchmarkViewModel>()
                .Name("Benchmarks_<#= SportId #>")
                .HtmlAttributes(new { Style = "width:697px" })
                .ToolBar(toolbar => toolbar.Insert().ButtonType(GridButtonType.ImageAndText))
                .DataKeys(keys => keys.Add(b => b.BenchmarkSportId))
                .ClientEvents(e => e.OnError("grid_OnError"))                
                .DataBinding(dataBinding => dataBinding.Ajax()
                    .Select("BenchmarksSportsBenchmarkGridAjax", "Maintenance", new { sportId = "<#= SportId #>" })
                    .Insert("InsertSportBenchmark", "Maintenance", new { sportId = "<#= SportId #>" })
                    .Update("UpdateSportBenchmark", "Maintenance", new { sportId = "<#= SportId #>" })
                    .Delete("DeleteSportBenchmark", "Maintenance", new { sportId = "<#= SportId #>" })
                )
                .Pageable()
                .Sortable()
                .Resizable(resize => resize.Columns(true))
                .Filterable()
                .Columns(columns =>
                {
                    columns.Bound(b => b.BenchmarkType)
                        .ClientTemplate("<#= BenchmarkTypeDescription  #>")
                        .Width(250);
                    columns.Bound(b => b.BenchmarkTier)
                        .ClientTemplate("<#= BenchmarkTierDescription  #>")
                        .Width(125);
                    columns.Bound(b => b.NSOValue).Width(80);
                    columns.Bound(b => b.OTPValue).Width(80);
                    columns.Bound(b => b.HasComments)
                        .ClientTemplate("<span onclick=\"openCommentWindow('BenchmarkSport', '<#= BenchmarkSportId #>', '<#= BenchmarkTier.Description #>', 'true', 'true');\" class=\"comment sprite sprite-<#= HasComments ? \"note\" : \"note_none\" #>\"></span>")
                        .Width(37)
                        .Title("");
                    columns.Command(commands =>
                    {
                        commands.Edit().ButtonType(GridButtonType.ImageAndText);
                        commands.Delete().ButtonType(GridButtonType.ImageAndText);
                    })
                        .Width(95);
                })
                .ToHtmlString()
            )) %>

        <% } %> 
  
        <br />
        <br />
    </div>
</div>

    <script type="text/javascript">
        function openTierWindow()
        {
            $("#InsertBenchmarkTier")
                .data("tWindow").center().open();
        }
    </script>
</asp:Content>