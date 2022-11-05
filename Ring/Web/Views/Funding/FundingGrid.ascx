<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<FundingViewModel>" %>

<% string actionName = (string)ViewContext.RouteData.Values["action"]; %>

<%  if (Model.CanAccessAvailableFund)
    {
        Html.RenderAction("FundingAllocation", new { actionName = actionName, fundingId = Model.Funding.Id, type = Model.Funding.Type, yearId = Model.Funding.YearId, sportId = Model.Funding.SportId });
    }
%>

<input type="hidden" id="hidFundingId" value="<%= Model.Funding.Id %>" />

<div style="float:left;width:90%;">
    <h3 style="color:Red"><%: Html.ValidationSummary(true) %></h3>
    <%= Html.Telerik().Grid<FundingTotal>()
        .Name("Totals")
        .TableHtmlAttributes(new { style = "font-size:8pt;" })
        .Columns(columns =>
            {
                columns.Bound(b => b.Type).ClientTemplate("<#= Type  #>").Width(120);

                if (actionName == "Requests")
                {
                    columns.Bound(b => b.OriginalBudget).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                    columns.Bound(b => b.RequestOtp).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                    columns.Bound(b => b.RequestNso).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                    columns.Bound(b => b.RequestSC).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                    columns.Bound(b => b.RequestOther).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                }

                if (actionName == "Recommendation")
                {
                    columns.Bound(b => b.RequestOtp).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                    columns.Bound(b => b.HpaRecommend).Format("{0:,#}").Visible(!Model.ShrinkMode).HtmlAttributes(new { style = "text-align:right" });
                    columns.Bound(b => b.OtpRecommend).Format("{0:,#}").Visible(!Model.ShrinkMode).HtmlAttributes(new { style = "text-align:right" });
                    columns.Bound(b => b.FinalRecommend).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                    columns.Bound(b => b.RecommendSC).Format("{0:,#}").Visible(Model.CanAccessAllocation).HtmlAttributes(new { style = "text-align:right" });
                    columns.Bound(b => b.RecommendCoc).Format("{0:,#}").Visible(Model.CanAccessAllocation).HtmlAttributes(new { style = "text-align:right" });
                    columns.Bound(b => b.RecommendOther).Format("{0:,#}").Visible(Model.CanAccessAllocation).HtmlAttributes(new { style = "text-align:right" });
                }

                if (actionName == "RevisionActual")
                {
                    columns.Bound(b => b.RevisedBudget).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                    columns.Bound(b => b.Actual).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                    columns.Bound(b => b.Variance).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                    columns.Bound(b => b.FinalRecommend).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                    columns.Bound(b => b.Revised).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                    columns.Bound(b => b.PendingRevise).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                    columns.Bound(b => b.PendingReviseSC).Format("{0:,#}").Visible(Model.CanAccessAllocation).HtmlAttributes(new { style = "text-align:right" });
                    columns.Bound(b => b.PendingReviseCoc).Format("{0:,#}").Visible(Model.CanAccessAllocation).HtmlAttributes(new { style = "text-align:right" });
                    columns.Bound(b => b.PendingReviseOther).Format("{0:,#}").Visible(Model.CanAccessAllocation).HtmlAttributes(new { style = "text-align:right" });
                }
            })
        .DataBinding(dataBinding => dataBinding.Ajax()
            .Select("GetFundingTotalsAjax", "Funding", new { id = Model.Funding.Id }))
        .Scrollable(scrolling => scrolling.Height(95))
        .Resizable(resize => resize.Columns(true))
        .Sortable()
        %>
</div>

<% Html.RenderPartial("~/Views/Funding/FundingStatus.ascx"); %>

<% Html.RenderPartial("~/Views/Shared/CommentEditor.ascx"); %>

<div style="float:left; width:100%;">

        <%-- Building Grid --%>
        <%= Html.Telerik().Grid<FundingBlock>()
            .Name("Blocks")
            .TableHtmlAttributes(new { style = "font-size:8pt;" })
            .Columns(columns =>
                {
                    columns.Bound(b => b.Type).ClientTemplate("<#= TypeDesc  #>").Width(150);

                    if (actionName == "Requests")
                    {
                        columns.Bound(b => b.OriginalBudget).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                        columns.Bound(b => b.RequestOtp).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                        columns.Bound(b => b.RequestNso).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                        columns.Bound(b => b.RequestSC).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                        columns.Bound(b => b.RequestOther).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                    }

                    if (actionName == "Recommendation")
                    {
                        columns.Bound(b => b.RequestOtp).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                        columns.Bound(b => b.HpaRecommend).Format("{0:,#}").Visible(!Model.ShrinkMode).HtmlAttributes(new { style = "text-align:right" });
                        columns.Bound(b => b.OtpRecommend).Format("{0:,#}").Visible(!Model.ShrinkMode).HtmlAttributes(new { style = "text-align:right" });
                        columns.Bound(b => b.FinalRecommend).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                        columns.Bound(b => b.RecommendSC).Format("{0:,#}").Visible(Model.CanAccessAllocation).HtmlAttributes(new { style = "text-align:right" });
                        columns.Bound(b => b.RecommendCoc).Format("{0:,#}").Visible(Model.CanAccessAllocation).HtmlAttributes(new { style = "text-align:right" });
                        columns.Bound(b => b.RecommendOther).Format("{0:,#}").Visible(Model.CanAccessAllocation).HtmlAttributes(new { style = "text-align:right" });
                    }

                    if (actionName == "RevisionActual")
                    {
                        columns.Bound(b => b.RevisedBudget).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                        columns.Bound(b => b.Actual).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                        columns.Bound(b => b.Variance).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                        columns.Bound(b => b.FinalRecommend).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                        columns.Bound(b => b.Revised).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                        columns.Bound(b => b.PendingRevise).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                        columns.Bound(b => b.PendingReviseSC).Format("{0:,#}").Visible(Model.CanAccessAllocation).HtmlAttributes(new { style = "text-align:right" });
                        columns.Bound(b => b.PendingReviseCoc).Format("{0:,#}").Visible(Model.CanAccessAllocation).HtmlAttributes(new { style = "text-align:right" });
                        columns.Bound(b => b.PendingReviseOther).Format("{0:,#}").Visible(Model.CanAccessAllocation).HtmlAttributes(new { style = "text-align:right" });
                    }

                    columns.Bound(b => b.Comment)
                        .ClientTemplate("<span onclick=\"openCommentWindow('FundingBlock', '<#= Id #>', '<#= TypeDesc #>', '" + Model.Editable + "', 'true');\" class=\"comment sprite sprite-<#= HasComment ? \"note\" : \"note_none\" #>\"></span>")
                        .Title(string.Empty)
                        .Width(30);
                })
            .DataBinding(dataBinding => dataBinding.Ajax()
                .Select("GetFundingBlocksAjax", "Funding", new { id = Model.Funding.Id }))
            .Scrollable(scrolling => scrolling.Enabled(true).Height(1000))
            .Resizable(resize => resize.Columns(true))
            .Sortable()
            .DetailView(categories => categories.ClientTemplate(
                    Html.Telerik().Grid<FundingCategory>()
                    .Name("Categories_<#= Id #>")
                    .Columns(columns =>
                    {
                        columns.Bound(c => c.Type).ClientTemplate("<#= TypeDesc  #>").Width(120);

                        if (actionName == "Requests")
                        {
                            columns.Bound(c => c.OriginalBudget).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                            columns.Bound(c => c.RequestOtp).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                            columns.Bound(c => c.RequestNso).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                            columns.Bound(c => c.RequestSC).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                            columns.Bound(c => c.RequestOther).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                        }

                        if (actionName == "Recommendation")
                        {
                            columns.Bound(c => c.RequestOtp).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                            columns.Bound(c => c.HpaRecommend).Format("{0:,#}").Visible(!Model.ShrinkMode).HtmlAttributes(new { style = "text-align:right" });
                            columns.Bound(c => c.OtpRecommend).Format("{0:,#}").Visible(!Model.ShrinkMode).HtmlAttributes(new { style = "text-align:right" });
                            columns.Bound(c => c.FinalRecommend).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                            columns.Bound(c => c.RecommendSC).Format("{0:,#}").Visible(Model.CanAccessAllocation).HtmlAttributes(new { style = "text-align:right" });
                            columns.Bound(c => c.RecommendCoc).Format("{0:,#}").Visible(Model.CanAccessAllocation).HtmlAttributes(new { style = "text-align:right" });
                            columns.Bound(c => c.RecommendOther).Format("{0:,#}").Visible(Model.CanAccessAllocation).HtmlAttributes(new { style = "text-align:right" });
                        }

                        if (actionName == "RevisionActual")
                        {
                            columns.Bound(c => c.RevisedBudget).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                            columns.Bound(c => c.Actual).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                            columns.Bound(c => c.Variance).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                            columns.Bound(c => c.FinalRecommend).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                            columns.Bound(c => c.Revised).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                            columns.Bound(c => c.PendingRevise).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                            columns.Bound(c => c.PendingReviseSC).Format("{0:,#}").Visible(Model.CanAccessAllocation).HtmlAttributes(new { style = "text-align:right" });
                            columns.Bound(c => c.PendingReviseCoc).Format("{0:,#}").Visible(Model.CanAccessAllocation).HtmlAttributes(new { style = "text-align:right" });
                            columns.Bound(c => c.PendingReviseOther).Format("{0:,#}").Visible(Model.CanAccessAllocation).HtmlAttributes(new { style = "text-align:right" });
                        }

                        columns.Bound(c => c.Comment)
                            .ClientTemplate("<span onclick=\"openCommentWindow('FundingCategory', '<#= Id #>', '<#= TypeDesc #>', '" + Model.Editable + "', 'true');\" class=\"comment sprite sprite-<#= HasComment ? \"note\" : \"note_none\" #>\"></span>")
                            .Title(string.Empty)
                            .Width(30);
                    })
                    .DataBinding(dataBinding => dataBinding.Ajax()
                        .Select("GetFundingCategoriesAjax", "Funding", new { id = Model.Funding.Id, blockId = "<#= Id #>" }))
                    .Resizable(resize => resize.Columns(true))
                    .Sortable()
                    .DetailView(details => details.ClientTemplate(
                                Html.Telerik().Grid<FundingDetail>()
                                .Name("Details_<#= Id #>")
                                .DataKeys(keys => keys.Add(d => d.Id))
                                .Editable(editing => editing.Mode(GridEditMode.InLine))
                                .ToolBar(commands => 
                                {
                                    if (Model.Editable)
                                        commands.Insert().ButtonType(GridButtonType.ImageAndText);
                                })
                                .Columns(columns =>
                                {

                                    columns.Bound(d => d.Description).Width(250);
                                    columns.Bound(d => d.CscId).ClientTemplate("<#= CscDesc  #>").Width(320); ;
                                    columns.Bound(d => d.ResourceId).ClientTemplate("<#= ResourceDesc  #>").Width(200); ;
                                    columns.Bound(d => d.CommittedPercent).Width(100); ;
                                    columns.Bound(d => d.PriorityCode).ClientTemplate("<#= PriorityDesc  #>").Width(80); ;
                                    columns.Bound(d => d.Recurring)
                                        .ClientTemplate("<span class='<#= Recurring ? \"t-update\" : \"t-delete\" #> t-icon'></span>")
                                        .Width(75).HtmlAttributes(new { style = "text-align:center" }); 
                                    
                                    if (actionName == "Requests")
                                    {
                                        columns.Bound(d => d.OriginalBudget).Width(120).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                                        columns.Bound(d => d.RequestOtp).Width(120).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                                        columns.Bound(d => d.RequestNso).Width(120).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                                        columns.Bound(d => d.RequestSC).Width(120).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                                        columns.Bound(d => d.RequestOther).Width(120).ReadOnly().Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                                    }

                                    if (actionName == "Recommendation")
                                    {
                                        columns.Bound(d => d.RequestOtp).Width(120).Format("{0:,#}").ReadOnly().HtmlAttributes(new { style = "text-align:right" });
                                        columns.Bound(d => d.HpaRecommend).Width(120).Format("{0:,#}").ReadOnly(!Model.CanEditHpaRecommend).Visible(!Model.ShrinkMode).HtmlAttributes(new { style = "text-align:right" });
                                        columns.Bound(d => d.OtpRecommend).Width(120).Format("{0:,#}").ReadOnly(!Model.CanEditOtpRecommend).Visible(!Model.ShrinkMode).HtmlAttributes(new { style = "text-align:right" });
                                        columns.Bound(d => d.FinalRecommend).Width(120).Format("{0:,#}").ReadOnly(!Model.CanEditFinalRecommend).HtmlAttributes(new { style = "text-align:right" });
                                        columns.Bound(d => d.RecommendSC).Width(120).Format("{0:,#}").ReadOnly(!Model.CanEditFinalRecommend).Visible(Model.CanAccessAllocation).HtmlAttributes(new { style = "text-align:right" });
                                        columns.Bound(d => d.RecommendCoc).Width(120).Format("{0:,#}").ReadOnly(!Model.CanEditFinalRecommend).Visible(Model.CanAccessAllocation).HtmlAttributes(new { style = "text-align:right" });
                                        columns.Bound(d => d.RecommendOther).Width(120).Format("{0:,#}").ReadOnly().Visible(Model.CanAccessAllocation).HtmlAttributes(new { style = "text-align:right" });
                                    }

                                    if (actionName == "RevisionActual")
                                    {
                                        columns.Bound(d => d.RevisedBudget).Width(120).ReadOnly().Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                                        columns.Bound(d => d.Actual).Width(120).ReadOnly(!Model.CanEditActual).Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                                        columns.Bound(d => d.Variance).Width(120).ReadOnly().Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                                        columns.Bound(d => d.FinalRecommend).Width(120).ReadOnly().Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                                        columns.Bound(d => d.Revised).Width(120).ReadOnly().Format("{0:,#}").HtmlAttributes(new { style = "text-align:right" });
                                        columns.Bound(d => d.PendingRevise).Width(120).Format("{0:,#}").ReadOnly(!Model.CanEditRevision).HtmlAttributes(new { style = "text-align:right" });
                                        columns.Bound(d => d.PendingReviseSC).Width(120).Format("{0:,#}").ReadOnly(!Model.CanEditRevision).Visible(Model.CanAccessAllocation).HtmlAttributes(new { style = "text-align:right" });
                                        columns.Bound(d => d.PendingReviseCoc).Width(120).Format("{0:,#}").ReadOnly(!Model.CanEditRevision).Visible(Model.CanAccessAllocation).HtmlAttributes(new { style = "text-align:right" });
                                        columns.Bound(d => d.PendingReviseOther).Width(120).Format("{0:,#}").ReadOnly().Visible(Model.CanAccessAllocation).HtmlAttributes(new { style = "text-align:right" });
                                    }

                                    columns.Bound(d => d.Comment)
                                        .ClientTemplate("<span onclick=\"openCommentWindow('FundingDetail', '<#= Id #>', '<#= Description #>', '" + Model.Editable + "', 'true');\" class=\"comment sprite sprite-<#= HasComment ? \"note\" : \"note_none\" #>\"></span>")
                                        .Title(string.Empty)
                                        .Width(30)
                                        .ReadOnly();

                                    columns.Command(commands =>
                                    {
                                        if (Model.Editable) commands.Edit().ButtonType(GridButtonType.ImageAndText);
                                        if (Model.Deleteable) commands.Delete().ButtonType(GridButtonType.ImageAndText);
                                    }).Width(90);
                                    
                                })
                                .ClientEvents(e => e.OnSave("gridFundingDetail_OnSave")
                                                    .OnError("grid_OnError"))
                                .DataBinding(dataBinding => dataBinding.Ajax()
                                    .Select("GetFundingDetailsAjax", "Funding", new { id = Model.Funding.Id, categoryId = "<#= Id #>" })
                                    .Update("UpdateFundingDetailsAjax", "Funding", new { fundingId = Model.Funding.Id, categoryId = "<#= Id #>", actionName = actionName })
                                    .Insert("InsertFundingDetailsAjax", "Funding", new { fundingId = Model.Funding.Id, categoryId = "<#= Id #>", actionName = actionName })
                                    .Delete("DeleteFundingDetailsAjax", "Funding", new { fundingId = Model.Funding.Id, categoryId = "<#= Id #>", actionName = actionName }))
                                .Sortable()
                                .Scrollable(scrolling => scrolling.Enabled(true))
                                .Resizable(resize => resize.Columns(true))
                                .ToHtmlString()
                        )
                    )
                    .ToHtmlString()
                )
            ).ToHtmlString()
        %>
</div>

    <script type="text/javascript">

        function gridFundingDetail_OnSave(e) {

            var errorMsg = '';

            //verify resource is required
            var categoryRow = $(e.currentTarget).closest('.t-detail-row').prev();
            var categoryDataItem = categoryRow.closest('.t-grid').data('tGrid').dataItem(categoryRow);

            if (categoryDataItem.ResourceRequired && e.values.ResourceId.length == 0) {
                errorMsg = "<%: Shared.FundingStrings.FundingGrid_Error_SaveDetailMissingResource %>";
            }
            
            //verify caculation of OTHER is not negative
            if (e.values.OriginalBudget != undefined)   //Requests
            {
                var total = Number(e.values.OriginalBudget); if (isNaN(total)) total = 0;
                var part1 = Number(e.values.RequestOtp); if (isNaN(part1)) part1 = 0;
                var part2 = Number(e.values.RequestNso); if (isNaN(part2)) part2 = 0;
                var part3 = Number(e.values.RequestSC); if (isNaN(part3)) part3 = 0;

                if (total < part1 + part2 + part3) {
                    errorMsg += "<%: Shared.FundingStrings.FundingGrid_Error_SaveDetailTotalFunds %>";
                }
            }
            else if (e.values.FinalRecommend != undefined)  //Recommendation
            {
                var total = Number(e.values.FinalRecommend); if (isNaN(total)) total = 0;
                var part1 = Number(e.values.RecommendSC); if (isNaN(part1)) part1 = 0;
                var part2 = Number(e.values.RecommendCoc); if (isNaN(part2)) part2 = 0;

                if (total < part1 + part2) {
                    errorMsg += "<%: Shared.FundingStrings.FundingGrid_Error_SaveDetailFinalRecmd %>";
                }
            }
            else if (e.values.PendingRevise != undefined)   //RevisionActual
            {
                var total = Number(e.values.PendingRevise); if (isNaN(total)) total = 0;
                var part1 = Number(e.values.PendingReviseSC); if (isNaN(part1)) part1 = 0;
                var part2 = Number(e.values.PendingReviseCoc); if (isNaN(part2)) part2 = 0;

                if (total < part1 + part2) {
                    errorMsg += "<%: Shared.FundingStrings.FundingGrid_Error_SaveDetailRevising %>";
                }
            }

            if (errorMsg.length > 0) {
                OTPAlert(errorMsg);
                e.preventDefault();
            }

            return;
        }

    </script>
