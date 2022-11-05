<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<SummaryListViewModel>>" %>

 <% foreach (var page in Model)
    { %>

    <div class="groupbox_home_page">
        <div class="groupbox_title_background gradientHToWhite">
            <div class="groupbox_title" >
                <span class="<%= Url.Content(page.IconClass) %>" ></span>
                <%: Html.ActionLink(
                    linkText: page.Name,
                    actionName: page.Action,
                    controllerName: page.Controller,
                    htmlAttributes: new { style = "font-weight: bold;" },
                    routeValues: null)%>  
            </div>
        </div>
        <div class="groupbox_content">
            <%: page.Description %>
            <%: Shared.SharedStrings.Pre_Summary_ClickHere %> <strong><%: page.Name %></strong>
            <%: Html.ActionLink(
                    linkText: Shared.SharedStrings.Summary_ClickHere,
                    actionName: page.Action,
                    controllerName: page.Controller,
                    htmlAttributes: new { style = "font-weight: bold;" },
                    routeValues: null)%>.
        </div>
        <img src="../../Content/Images/sidenav_btm.gif" width="1000" height="9" /> 
    </div> 

    <div class="clear"></div>

   <% } %>
