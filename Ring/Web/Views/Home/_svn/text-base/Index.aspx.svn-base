<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<HomeIndexViewModel>" %>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.RenderPartial("~/Views/Shared/JavaScriptCheck.ascx"); %>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MenuContent" runat="server">
    <% Html.RenderPartial("~/Views/Shared/NavMenu.ascx", Model.Menu); %>
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="HeaderImage" runat="server">
    <%= RingHTMLHelper.GetRandomHeaderImage() %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContent" runat="server">
    <%: Shared.HomeStrings.Index_Title %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="InfoBar" runat="server">
    <% Html.RenderPartial("~/Views/Shared/LanguageSelector.ascx", Model.CurrentLanguageCode); %> &nbsp;|&nbsp;
    <% Html.RenderPartial("~/Views/Shared/CurrentUserName.ascx", Model.CurrentUserName); %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">
<% //if (Model.HasAccess) 
    if (true) 
   {
       Html.RenderPartial("~/Views/Shared/Summary.ascx", Model.SummaryPageList);
   }
   else
   {
   %>
         <table width="65%" style="HEIGHT: 100%" border="0">
			<tr>
                <td>
					<br/>
				</td>
			</tr>
			<tr>
				<td align="center"><%: Shared.HomeStrings.Index_Statement %></td>
			</tr>
			<tr>
                <td>
					<br/>
				</td>
			</tr>
			<tr>
				<td align="center"><%: Shared.HomeStrings.Index_UnderConstruction %></td>
			</tr>
			<tr>
                <td>
					<br/>
				</td>
			</tr>
        </table>

   <%
   }
   %>
</asp:Content>


