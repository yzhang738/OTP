<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<FundingAllocationViewModel>" %>

<div style="float:left;width:90%;margin-bottom:5px;">

<%
    using (Html.BeginForm("UpdateAvailableFunds", "Funding", FormMethod.Post, new { name = "AvailableFundsForm" }))
    {
    %>
    <fieldset>
        <legend><%: Shared.FundingStrings.FundingAllocation_SectionName %></legend>
        <div>
            <div style="font-size:8pt;float:left;width:19%;text-align:right;"><%: Shared.FundingStrings.FundingAllocation_Total %></div>
            <div style="float:left;width:22%;">
                <div style="font-size:8pt;float:left;width:35%;text-align:right;"><%: Html.LabelFor(model => model.FundingAllocation.AvailableFundsSC)%></div>
                <div style="float:right;width:60%;"><%: Html.EditorFor(model => model.FundingAllocation.AvailableFundsSC)%></div>
            </div>
            <div style="float:left;width:22%;">
                <div style="font-size:8pt;float:left;width:35%;text-align:right;"><%: Html.LabelFor(model => model.FundingAllocation.AvailableFundsCoc)%></div>
                <div style="float:right;width:60%;"><%: Html.EditorFor(model => model.FundingAllocation.AvailableFundsCoc)%></div>
            </div>
            <div style="float:left;width:22%;">
                <div style="font-size:8pt;float:left;width:35%;text-align:right;"><%: Html.LabelFor(model => model.FundingAllocation.AvailableFundsOther)%></div>
                <div style="float:right;width:60%;"><%: Html.EditorFor(model => model.FundingAllocation.AvailableFundsOther)%></div>
            </div>
            <div style="float:left;width:15%;">
            </div>
        </div>
        <div>
            <div style="font-size:8pt;float:left;width:19%;text-align:right;"><%: Shared.FundingStrings.FundingAllocation_Unallocated %></div>
            <div style="float:left;width:22%;">
                <div style="font-size:8pt;float:left;width:35%;text-align:right;"><%: Html.LabelFor(model => model.FundingAllocation.UnallocatedFundsSC)%></div>
                <div style="font-size:8pt;float:right;width:60%;text-align:right;"><%: Model.FundingAllocation.UnallocatedFundsSC.Value.ToString("n0") %></div>
            </div>
            <div style="float:left;width:22%;">
                <div style="font-size:8pt;float:left;width:35%;text-align:right;"><%: Html.LabelFor(model => model.FundingAllocation.UnallocatedFundsCoc)%></div>
                <div style="font-size:8pt;float:right;width:60%;text-align:right;"><%: Model.FundingAllocation.UnallocatedFundsCoc.Value.ToString("n0")%></div>
            </div>
            <div style="float:left;width:22%;">
                <div style="font-size:8pt;float:left;width:35%;text-align:right;"><%: Html.LabelFor(model => model.FundingAllocation.UnallocatedFundsOther)%></div>
                <div style="font-size:8pt;float:right;width:60%;text-align:right;"><%: Model.FundingAllocation.UnallocatedFundsOther.Value.ToString("n0")%></div>
            </div>
            <div style="float:left;width:15%;">
                <div style="float:right;" class="t-button t-state-default" onmouseover="$(this).addClass('t-button-hover');" onmouseleave="$(this).removeClass('t-button-hover');" onclick="document.AvailableFundsForm.submit();">
                    <span class="t-update t-icon"></span><span class="button-span"><%: Shared.FundingStrings.FundingAllocation_UpdateButton %></span>
                </div>
                <div class="clear"></div>
            </div>
        </div>
        <%: Html.HiddenFor(model => model.FundingAllocation.Id)%>
        <%: Html.HiddenFor(model => model.FundingAllocation.Type)%>
        <%: Html.HiddenFor(model => model.FundingAllocation.YearId)%>
        <%: Html.HiddenFor(model => model.FundingAllocation.SportId)%>
        <%: Html.HiddenFor(model => model.actionName)%>
        <%: Html.HiddenFor(model => model.fundingId)%>
    </fieldset>
    <%
    }
%>
</div>
