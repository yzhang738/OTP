﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ReportingReportParametersViewModel>" %>

<fieldset>
    <legend><%: Shared.ReportingStrings.ReportParameters_Legend %></legend>
    <div id="parameterDropDowns">
    <input type="hidden" id="reportTypeAdhoc" value="<%: (Model.ReportType == Constant.ReportType.AdHocReporting).ToString().ToLower() %>" />

    <% foreach (var filter in Model.Filters)
    {
        %>
        
        <input type="hidden" id="parent_count_<%: filter.ReportProcedureId.ToString() %>" value="<%: filter.ParentReportProcedureId.Count.ToString() %>" />
        <input type="hidden" id="childId_count_<%: filter.ReportProcedureId.ToString() %>" value="<%: filter.ChildReportProcedureId.Count.ToString() %>" />
<%--        <input type="hidden" id="childId_<%: filter.ReportProcedureId.ToString() %>" value="<%: filter.ChildReportProcedureId.ToString() %>" />
--%>
        <% if(filter.ParentReportProcedureId.Count > 0)
            { %>
                <div id="parent_ids_<%: filter.ReportProcedureId.ToString() %>" style="display:none;">
                    <%  for (int i = 0; i < filter.ParentReportProcedureId.Count; i++)
			        { %>
                        <input type="hidden" name="parentId" value="<%: filter.ParentReportProcedureId[i].ToString() %>" />
			        <% } %>
                </div>
        <% } %>

        <% if (filter.ChildReportProcedureId.Count > 0)
            { %>
                <div id="childId_ids_<%: filter.ReportProcedureId.ToString() %>" style="display:none;">
                    <%  for (int i = 0; i < filter.ChildReportProcedureId.Count; i++)
			        { %>
                        <input type="hidden" name="childId" value="<%: filter.ChildReportProcedureId[i].ToString() %>" />
			        <% } %>
                </div>
        <% } %>

        <% if (filter.Display)
           { %>
                <div style="width: 560px; margin:10px 10px 10px 10px">
                    <div style="float: left; padding-top: 5px">
                        <label id="label_<%: filter.ReportProcedureId.ToString() %>"><%: filter.Label%></label>
                    </div>
                    <div style="float: right">
                        <% 
                            Html.Telerik().DropDownList()
                                .Name("ddl_" + filter.ReportProcedureId.ToString())
                                .Items(i => i.Add().Text(filter.DefaultValue).Value(filter.DefaultID).Selected(true))
                                .HtmlAttributes(new
                                {
                                    style = "width:300px;font-size:8pt;"
                                })
                                .DataBinding(binding => binding.Ajax().Select("ReportParameterAjax", "Shared",
                                    new { defaultID = filter.DefaultID, storedProcedureName = filter.StoredProcedureName }))
                                .ClientEvents(e => e
                                    .OnClose("onDropDownClosed")
                                    .OnDataBinding("onDropDownBinding")).Render();                               
                                
                        %>
                    </div>
                    <div style="clear: both"></div>
                </div>
        <% }
           else
           {
                Html.Telerik().DropDownList()
                .Name("ddl_" + filter.ReportProcedureId.ToString())
                .Items(i => i.Add().Text(filter.DefaultValue).Value(filter.DefaultValue))
                .SelectedIndex(0)
                .HtmlAttributes(new
                {
                    style = "display:none;"
                }).Render();
           } %>
    <% } %>
    </div>
</fieldset>

    <div style="float:right; margin: 5px 0px 5px 0px" class="t-button t-state-default" onmouseover="$(this).addClass('t-button-hover');" onmouseleave="$(this).removeClass('t-button-hover');" onclick="ProcessReport()">
        <span class="button-span"><%: Shared.ReportingStrings.ReportParameters_ButtonProcessReport %></span>
    </div>

