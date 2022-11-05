<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ReportingStandardReportsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.RenderPartial("~/Views/Shared/JavaScriptCheck.ascx"); %>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="TitleContent" runat="server">
	<%: Shared.ReportingStrings.StandardReports_Title %>
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

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div "viewcontent">
        <% Html.RenderPartial("~/Views/Reporting/ReportRenderer.ascx", Model.ReportList); %>
    </div>

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="YearSelectionContent" runat="server">
    <div style="float:left;width:49%;text-align:right;margin:0px 0px 5px 0px;">
        <%: Shared.ReportingStrings.StandardReports_ReportCategory %>
    </div>
    <div style="float:right;width:50%;text-align:left;margin:0px 0px 5px 0px;">

        <%= Html.Telerik().DropDownList()
            .Name("CategorySelector")
            .HtmlAttributes(new { style = "width:200px;font-size:12px" })
            .SelectedIndex(Model.ReportCategoryListSelectedIndex)
            .BindTo(new SelectList(Model.ReportCategoryList, "Id", "Description"))
                    .ClientEvents(e => e.OnClose("changeCategory"))
                                  
        %>

    </div> 
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContent" runat="server">
    <script type="text/javascript">
        function changeCategory()
        {
            var editor = $(this).data('tDropDownList');
            window.location.href = '/Reporting/StandardReports/' + editor.value();
        }
    </script>
</asp:Content>