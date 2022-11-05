<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<WhoAmIViewModel>" %>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.RenderPartial("~/Views/Shared/JavaScriptCheck.ascx"); %>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	<%: Shared.MaintenanceStrings.WhoAmI_Title %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <% Html.EnableClientValidation(); %>

    <% using (Html.BeginForm("WhoAmI", "Maintenance"))
    { %>

        <div class="groupbox_600" style="margin:0px 0px 0px 0px">
            <div class="groupbox_title_background gradientHToWhite">
                <div class="groupbox_title"><%: Shared.MaintenanceStrings.WhoAmI_Info %></div>
            </div>
            <div class="groupbox_content">

                <%: Html.ValidationSummary(true) %>

                <div style="float:left; width:98%; margin:0px 0px 15px 0px;">
                    <div style="float:left;width:40%;text-align:right;"><%: Html.LabelFor(model => model.User.FirstName)%></div>
                    <div style="float:right;width:50%;text-align:left;"><%: Html.TextBoxFor(model => model.User.FirstName) %></div> 
                </div>

                <div style="float:left; width:98%; margin:0px 0px 15px 0px;">
                    <div style="float:left;width:40%;text-align:right;"><%: Html.LabelFor(model => model.User.LastName)%></div>
                    <div style="float:right;width:50%;text-align:left;"><%: Html.TextBoxFor(model => model.User.LastName) %></div> 
                </div>

                <div style="float:left; width:98%; margin:0px 0px 15px 0px;">
                    <div style="float:left;width:40%;text-align:right;"><%: Html.LabelFor(model => model.User.Comments)%></div>
                    <div style="float:right;width:50%;text-align:left;"><%: Html.TextAreaFor(model => model.User.Comments, 4, 32, null) %></div> 
                </div>

                <div style="float:left; width:98%; margin:0px 0px 15px 0px;">
                    <div style="float:left;width:40%;text-align:right;"><%: Html.LabelFor(model => model.User.AuthenticatedLiveId)%></div>
                    <div style="float:right;width:50%;text-align:left;"><%: Model.User.AuthenticatedLiveId %></div> 
                </div>

                <div style="float:right; text-align:right; margin: 5px 10px 0px 0px" >
                    <input type="button" onclick="window.location.href = '/';" value="<%: Shared.SharedStrings.Button_Cancel %>" />
                    <input type="submit" value="<%: Shared.SharedStrings.Button_Save %>" />
                </div>
            </div>
            <img alt="" src="../../Content/Images/sidenav_btm.gif" width="600" height="9" /> 
        </div> 
        <div class="clear"></div>


            <%
    } %>

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="MenuContent" runat="server">
    <% Html.RenderPartial("~/Views/Shared/NavMenu.ascx", Model.Menu); %>
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="HeaderImage" runat="server">
    <%= RingHTMLHelper.GetRandomHeaderImage() %>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="InfoBar" runat="server">
    <% Html.RenderPartial("~/Views/Shared/LanguageSelector.ascx", Model.CurrentLanguageCode); %> 
</asp:Content>