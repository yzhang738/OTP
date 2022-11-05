<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<FundingViewModel>" %>

<div>


<% 
    
    string actionName = (string)ViewContext.RouteData.Values["action"];

    string actionItemType = string.Empty;

    if (actionName == "Requests")
    {
        actionItemType = OTP.Ring.Common.Constant.ActionItemType.FundingRequest;
    }
    else if (actionName == "Recommendation")
    {
        actionItemType = OTP.Ring.Common.Constant.ActionItemType.FundingRecommendation;
    }
    else if (actionName == "RevisionActual")
    {
        actionItemType = OTP.Ring.Common.Constant.ActionItemType.FundingRevision;
    }
    
    
%>

    <div style="float:left;">
    <% if (Model.ToolbarButtons != null && Model.ToolbarButtons.Count > 0)
       {
           if (Model.ToolbarButtons.Contains(OTP.Ring.Common.Constant.PageId.FundingAction))
           {%>

           <% if (Model.Editable)
              { %>
                <div style="float:left; margin:10px 10px 5px 0px;" class="t-button t-state-default" onmouseover="$(this).addClass('t-button-hover');" onmouseleave="$(this).removeClass('t-button-hover');" onclick="openActionItemWindow('<%: actionName %>', 'Funding', '<%: actionItemType %>', '<%: Model.Funding.Id %>', '<%: Model.Funding.OrganizationId %>', '<%: Model.Funding.SportId %>')">
                    <span class="t-add t-icon"></span><span class="button-span"><%: Shared.FundingStrings.FundingStatus_Button_AddActionItem%></span>
                </div>
            <% } %>

            <div style="float:left; margin:10px 10px 5px 0px;" class="t-button t-state-default" onmouseover="$(this).addClass('t-button-hover');" onmouseleave="$(this).removeClass('t-button-hover');">
                <span class="t-add t-icon"></span><span class="button-span"><a href="<%: Url.Action("ActionItems", "Funding", new {referenceId = Model.Funding.Id }) %>"><%: Shared.FundingStrings.FundingStatus_Button_ActionItem %> <%: Model.Funding.ActionItem %></a></span>
            </div>


        <% }

           if (Model.ToolbarButtons.Contains(OTP.Ring.Common.Constant.PageId.MaintenanceResources))
           {
            %>
            <div style="float:left; margin:10px 10px 5px 0px;" class="t-button t-state-default" onmouseover="$(this).addClass('t-button-hover');" onmouseleave="$(this).removeClass('t-button-hover');">
                <span class="t-add t-icon"></span><span class="button-span"><a href="<%: Url.Action("Resources", "Maintenance") %>" target="_newResources"><%: Shared.FundingStrings.FundingStatus_Button_Resource %></a></span>
            </div>
        <% } %>
           
           <div class="clear"></div>
           <%
       }
       else
       { 
       %>
        <div style="float:left; margin:10px 10px 5px 0px;">&nbsp;</div>
       <%
       } %>
    </div>



    <div style="float:right;">
 
    <%
        foreach (var item in Model.ChangeToStatus)
        {
            using (Html.BeginForm("ChangeToStatus", "Funding", new { id = Model.Funding.Id, changeFromStatus = item.FromStatus, changeToStatus = item.ToStatus }, FormMethod.Post, new { name = "statusForm" + item.ToStatus }))
            {
                string buttonText = Shared.FundingStrings.FundingStatus_Button_NA;
                if (item.ToStatus == OTP.Ring.Common.Constant.FundingStatus.New)
                    buttonText = Shared.FundingStrings.FundingStatus_Button_New;
                else if (item.ToStatus == OTP.Ring.Common.Constant.FundingStatus.Requested)
                    buttonText = Shared.FundingStrings.FundingStatus_Button_Request;
                else if (item.ToStatus == OTP.Ring.Common.Constant.FundingStatus.HpaRecommended)
                    buttonText = Shared.FundingStrings.FundingStatus_Button_Hpa;
                else if (item.ToStatus == OTP.Ring.Common.Constant.FundingStatus.OtpRecommended)
                    buttonText = Shared.FundingStrings.FundingStatus_Button_Otp;
                else if (item.ToStatus == OTP.Ring.Common.Constant.FundingStatus.Approved)
                    buttonText = Shared.FundingStrings.FundingStatus_Button_Final;
                else if (item.ToStatus == OTP.Ring.Common.Constant.FundingStatus.Allocated)
                    buttonText = Shared.FundingStrings.FundingStatus_Button_Lock;
                else if (item.ToStatus == OTP.Ring.Common.Constant.FundingStatus.Revise)
                    buttonText = Shared.FundingStrings.FundingStatus_Button_Revision;
                else if (item.ToStatus == OTP.Ring.Common.Constant.FundingStatus.Actualize)
                    buttonText = Shared.FundingStrings.FundingStatus_Button_Actual;
                else if (item.ToStatus == OTP.Ring.Common.Constant.FundingStatus.Closed)
                    buttonText = Shared.FundingStrings.FundingStatus_Button_Close;
                else
                    buttonText = item.ToStatus;
            
                %>
                    <div style="float:right; margin:10px 0px 5px 10px;" class="t-button t-state-default" onmouseover="$(this).addClass('t-button-hover');" onmouseleave="$(this).removeClass('t-button-hover');" onclick="document.statusForm<%= item.ToStatus %>.submit();">
                        <span class="t-expand t-icon"></span><span class="button-span"><%: buttonText %></span>
                    </div>
                <%
            }
        }
    
         %>

    </div>


    
    <% Html.RenderPartial("~/Views/Shared/ActionItemEditor.ascx"); %>

</div>
