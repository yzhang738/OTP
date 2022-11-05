<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<FundingViewModel>" %>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.RenderPartial("~/Views/Shared/JavaScriptCheck.ascx"); %>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%: Shared.FundingStrings.Recommendation_Title %> <br /> <%: Model.Title %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="container">
        <% 
        if (Model.Funding == null)
        {%>
            <h3><%: Shared.FundingStrings.Recommendation_Error_NoSelectedSport %></h3>
        <%}
        else
        {
            Html.RenderPartial("~/Views/Funding/FundingGrid.ascx");
        } %>
    </div>
</asp:Content>


<asp:Content ID="Content4" ContentPlaceHolderID="MenuContent" runat="server">
    <% Html.RenderPartial("~/Views/Shared/NavMenu.ascx", Model.Menu); %>
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="HeaderImage" runat="server">
    <%= RingHTMLHelper.GetRandomHeaderImage() %>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="YearSelectionContent" runat="server">
    <%  var viewData = new ViewDataDictionary();
        viewData.Add("Enable", false);
        Html.RenderPartial("~/Views/Shared/YearSelector.ascx", Model.CurrentOlympicYears, viewData); %>
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="InfoBar" runat="server">
    <% Html.RenderPartial("~/Views/Shared/LanguageSelector.ascx", Model.CurrentLanguageCode); %> &nbsp;|&nbsp;
    <% Html.RenderPartial("~/Views/Shared/CurrentUserName.ascx", Model.CurrentUserName); %>
</asp:Content>