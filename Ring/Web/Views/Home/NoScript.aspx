<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<HomeIndexViewModel>" %>

<asp:Content ID="Content6" ContentPlaceHolderID="HeaderImage" runat="server">
    <%= RingHTMLHelper.GetRandomHeaderImage() %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContent" runat="server">
    <%: Shared.HomeStrings.NoScript_Title %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="InfoBar" runat="server">
    <% Html.RenderPartial("~/Views/Shared/LanguageSelector.ascx", Model.CurrentLanguageCode); %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">

<noscript>

         <table width="65%" style="HEIGHT: 100%" border="0">
			<tr>
                <td>
					<br/>
				</td>
			</tr>
			<tr>
				<td align="center"><%: Shared.HomeStrings.NoScript_Line1 %></td>
			</tr>
			<tr>
                <td>
					<br/>
				</td>
			</tr>
			<tr>
				<td align="center"><%= string.Format(Shared.HomeStrings.NoScript_Line2, "<a href='http://support.microsoft.com/gp/howtoscript'>", "</a>") %></td>
			</tr>
			<tr>
                <td>
					<br/>
				</td>
			</tr>
        </table>

</noscript>

    <script type="text/javascript">
        window.location.href = '/';
    </script>
</asp:Content>


