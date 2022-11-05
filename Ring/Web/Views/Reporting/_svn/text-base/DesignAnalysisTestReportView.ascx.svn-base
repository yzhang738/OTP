<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ReportingDesignAnalysisTestReportViewModel>" %>

    <div class="groupbox_title_background gradientHToWhite">
        <div class="groupbox_title" >
            <%= ViewData["reportName"] %>
        </div>
    </div>

    <% if (ViewData["reportId"] != null)
        {   
           Html.RenderPartial("~/Views/Reporting/ReportRenderer.ascx", Model.ReportList);
       } %>
       