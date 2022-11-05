<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<FundingSummaryViewModel>" %>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.RenderPartial("~/Views/Shared/JavaScriptCheck.ascx"); %>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%: Shared.FundingStrings.Summary_Title %> : <%: Model.VerifiedSelectedSportDesc%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="container">

    <% 
    if (string.IsNullOrEmpty(Model.VerifiedSelectedSport))
    {%>
        <h3><%: Shared.FundingStrings.Summary_Error_NoSelectedSport %></h3>
    <%}
    else
    {
    %>

        <%: Html.ValidationSummary(true) %>

        <%-- Building Create Funding Window --%>
        <%
            Html.Telerik().Window()
            .Name("CreateFundingWindow")
            .Title(Shared.FundingStrings.Summary_CreateFundingWindow_Title)
            .Content(() =>
                { %>

                    <% using (Html.BeginForm("CreateFundingRequest", "Funding"))
                    { %>

                        <div class="groupbox_600" style="margin:0px 0px 0px 0px">
                            <div class="groupbox_title_background gradientHToWhite">
                                <div class="groupbox_title" id="_title"></div>
                            </div>
                            <div class="groupbox_content">

                                <div id="_orgSelection" style="float:left; width:98%; margin:0px 0px 15px 0px;">
                                    <div style="float:left;width:40%;text-align:right;"><%: Shared.FundingStrings.Summary_NewFunding_CSC %></div>
                                    <div style="float:right;width:50%;text-align:left;"><%: Html.DropDownList("OrgId", new SelectList((IEnumerable)ViewData["cscs"], "Id", "Name"), new { style = "width:200px;" })%></div> 
                                </div>

                                <div id="_prjSelection" style="float:left; width:98%; margin:0px 0px 15px 0px;">
                                    <div style="float:left;width:40%;text-align:right;"><%: Shared.FundingStrings.Summary_NewFunding_OTP %></div>
                                    <div style="float:right;width:50%;text-align:left;"><%: Html.DropDownList("PrjId", new SelectList((IEnumerable)ViewData["prjs"], "Id", "Name"), new { style = "width:200px;" })%></div> 
                                </div>

                                <div id="_sportSelection" style="float:left; width:98%; margin:0px 0px 15px 0px;">
                                    <div style="float:left;width:40%;text-align:right;"><%: Shared.FundingStrings.Summary_NewFunding_Sport %></div>
                                    <div style="float:right;width:50%;text-align:left;"><%: Html.DropDownList("SportId", new SelectList((IEnumerable)ViewData["sports"], "Id", "Description"), new { style = "width:200px;" })%> </div>
                                </div>

                                <div id="_yearSelection" style="float:left; width:98%; margin:0px 0px 15px 0px;">
                                    <div style="float:left;width:40%;text-align:right;"><%: Shared.FundingStrings.Summary_NewFunding_Season %></div> 
                                    <div style="float:right;width:50%;text-align:left;"><%: Html.DropDownList("YearId", new SelectList((IEnumerable)ViewData["years"], "Id", "Description", "2010/2011"), new { style = "width:200px;" })%> </div>
                                </div>

                                <input type="hidden" name="actionName" value="" />
                                <input type="hidden" name="fundingType" value="" />
                                <input type="hidden" name="fundingId" value="" />
                            </div>
                            <img alt="" src="../../Content/Images/sidenav_btm.gif" width="600" height="9" /> 
                        </div> 
                        <div class="clear"></div>
                        <div style="float:right; text-align:right; margin: 5px 10px 0px 0px" >
                            <input type="submit" value="<%: Shared.FundingStrings.Summary_Create %>" />
                        </div>

                          <%
                    } %>



             <% })
            .Draggable(true)
            .Scrollable(false)
            .Resizable()
            .Buttons(b => b.Close())
            .Width(605)
            .Modal(true)
            .Visible(false)
            .Render();
                        
        %>

        <%-- Building Grid --%>
        <% Html.Telerik().Grid<FundingSummary>(Model.FundingSummaries)
            .Name("Summaries")
            .TableHtmlAttributes(new { style = "font-size:8pt;" })
            .Columns(columns =>
                {
                    columns.Bound(b => b.Type).Template(c => c.TypeDesc);
                    columns.Bound(b => b.RequestOtp).Width(110).Format("{0:n0}").HtmlAttributes(new { style = "text-align:right" });
                    columns.Bound(b => b.HpaRecommend).Width(110).Format("{0:n0}").HtmlAttributes(new { style = "text-align:right" });
                    columns.Bound(b => b.OtpRecommend).Width(110).Format("{0:n0}").HtmlAttributes(new { style = "text-align:right" });
                    columns.Bound(b => b.FinalRecommend).Width(110).Format("{0:n0}").HtmlAttributes(new { style = "text-align:right" });
                    columns.Bound(b => b.Revised).Width(110).Format("{0:n0}").HtmlAttributes(new { style = "text-align:right" });
                    columns.Template(c => string.Empty).Width(3);
                    columns.Bound(b => b.RevisedBudget).Width(97).Format("{0:n0}").HtmlAttributes(new { style = "text-align:right" });
                    columns.Bound(b => b.Actual).Width(97).Format("{0:n0}").HtmlAttributes(new { style = "text-align:right" });
                    columns.Template(c => {  %>
                        <div style="float:left; margin:10px 10px 5px 0px;" class="t-button t-state-default" onmouseover="$(this).addClass('t-button-hover');" onmouseleave="$(this).removeClass('t-button-hover');" onclick="OpenCreateFundingWindow('New', '<%: c.Type %>', null);">
                            <span class="t-add t-icon"></span><span class="button-span"><%: Shared.SharedStrings.Button_New %></span>
                        </div>
                        <div class="clear"></div>
                     <% }).Visible(Model.Editable).Width(90);
                })
            .DataBinding(dataBinding => dataBinding.Server())
            .Scrollable(scrolling => scrolling.Height(400))
            .Resizable(resize => resize.Columns(true))
            .Sortable()
            .DetailView(funding => funding.Template(e =>
                {
                    Html.Telerik().Grid<Funding>(e.Fundings)
                        .Name("Fundings_" + e.Type)
                        .TableHtmlAttributes(new { style = "font-size:8pt;" })
                        .Columns(columns =>
                        {
                            columns.Template(c => { %> <%: (c.Type == Constant.OrganizationTypeId.NSO) ? c.SportDesc : c.OrganizationDesc %> <% }).Width(100);
                            columns.Bound(f => f.StatusCode).Width(80).Template(c => c.StatusDesc);
                            columns.Bound(f => f.RequestOtp).Width(105).Template(c => { %> <%: Html.ActionLink(c.RequestOtp.Value.ToString("n0"), "Requests", new { id = c.Id }) %> <% } ).HtmlAttributes(new { style = "text-align:right" });
                            columns.Bound(f => f.HpaRecommend).Width(105).Template(c => { if (c.ShowHpaRecommendLink) { %> <%: Html.ActionLink(c.HpaRecommend.Value.ToString("n0"), "Recommendation", new { id = c.Id }) %> <% } }).HtmlAttributes(new { style = "text-align:right" });
                            columns.Bound(f => f.OtpRecommend).Width(105).Template(c => { if (c.ShowOtpRecommendLink) { %> <%: Html.ActionLink(c.OtpRecommend.Value.ToString("n0"), "Recommendation", new { id = c.Id }) %> <% } }).HtmlAttributes(new { style = "text-align:right" });
                            columns.Bound(f => f.FinalRecommend).Width(105).Template(c => { if (c.ShowFinalRecommendLink) { %> <%: Html.ActionLink(c.FinalRecommend.Value.ToString("n0"), "Recommendation", new { id = c.Id }) %> <% } }).HtmlAttributes(new { style = "text-align:right" });
                            columns.Bound(f => f.Revised).Width(105).Template(c => { if (c.ShowRevisedLink) { %> <%: Html.ActionLink(c.Revised.Value.ToString("n0"), "RevisionActual", new { id = c.Id }) %> <% } }).HtmlAttributes(new { style = "text-align:right" });
                            columns.Template(c => string.Empty).Width(3);
                            columns.Bound(f => f.RevisedBudget).Width(90).Format("{0:n0}").HtmlAttributes(new { style = "text-align:right" });
                            columns.Bound(f => f.Actual).Width(90).Format("{0:n0}").HtmlAttributes(new { style = "text-align:right" });
                            columns.Bound(f => f.ActionItem).Width(50).Template(c => { %> <%: Html.ActionLink(c.ActionItem, "ActionItems", new { referenceId = c.Id, type = string.Empty })%> <% });
                            columns.Template(c => {  %>
                                <div style="float:left; margin:10px 10px 5px 0px;" class="t-button t-state-default" onmouseover="$(this).addClass('t-button-hover');" onmouseleave="$(this).removeClass('t-button-hover');" onclick="OpenCreateFundingWindow('Copy', '<%: c.Type %>', '<%: c.Id %>');">
                                    <span class="t-add t-icon"></span><span class="button-span"><%: Shared.SharedStrings.Button_Copy %></span>
                                </div>
                                <div class="clear"></div>
                            <% }).Visible(Model.Editable).Width(90);
                        })
                        .DataBinding(dataBinding => dataBinding.Server())
                        .Scrollable()
                        .Sortable()
                        .Render();
                }))
            .Render();
            
        %>



    <% } %>

    </div>

    <div>
        <script type="text/javascript">

            function OpenCreateFundingWindow(actionName, fundingType, fundingId) {

                $("#CreateFundingWindow")
                    .find('input[name=actionName]').val(actionName).end()
                    .find('input[name=fundingType]').val(fundingType).end()
                    .find('input[name=fundingId]').val(fundingId).end()
                    .data("tWindow").center().open();
                
                //populate title bar
                $("#CreateFundingWindow").find('div[id=_title]').html(actionName + " <%: Shared.FundingStrings.Summary_FundingRequest %>");

                if (fundingType == "NSO") {
                    $("#CreateFundingWindow").find('div[id=_prjSelection]').hide();
                    $("#CreateFundingWindow").find('div[id=_orgSelection]').hide();
                    $("#CreateFundingWindow").find('div[id=_sportSelection]').show();
                }
                else if (fundingType == "CSC") {
                    $("#CreateFundingWindow").find('div[id=_prjSelection]').hide();
                    $("#CreateFundingWindow").find('div[id=_orgSelection]').show();
                    $("#CreateFundingWindow").find('div[id=_sportSelection]').hide();
                }
                else if (fundingType == "OTP") {
                    $("#CreateFundingWindow").find('div[id=_prjSelection]').show();
                    $("#CreateFundingWindow").find('div[id=_orgSelection]').hide();
                    $("#CreateFundingWindow").find('div[id=_sportSelection]').hide();
                }
            }

        </script>
    </div>

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="MenuContent" runat="server">
    <% Html.RenderPartial("~/Views/Shared/NavMenu.ascx", Model.Menu); %>
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="HeaderImage" runat="server">
    <%= RingHTMLHelper.GetRandomHeaderImage() %>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="YearSelectionContent" runat="server">
    <% Html.RenderPartial("~/Views/Shared/YearSelector.ascx", Model.CurrentOlympicYears); %>
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="InfoBar" runat="server">
    <% Html.RenderPartial("~/Views/Shared/LanguageSelector.ascx", Model.CurrentLanguageCode); %> &nbsp;|&nbsp;
    <% Html.RenderPartial("~/Views/Shared/CurrentUserName.ascx", Model.CurrentUserName); %>
</asp:Content>