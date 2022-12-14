<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<OTP.Ring.Models.ListItem>>" %>

<script type="text/javascript">
    var supressMessages = false;

//    function onDropDownClosed(e) {
//        var reportProcedureId = e.currentTarget.id.replace('ddl_', '');
//        var childReportProcedureId = $("#childId_" + reportProcedureId).val();

//        var childCombo = $('#ddl_' + childReportProcedureId).data('tDropDownList');

//        if (childCombo) {
//            supressMessages = true;
//            childCombo.text('');
//            childCombo.reload();
//        }

//        if (reportProcedureId != '126') {
//            return;
//        }

//        childReportProcedureId = 128;
//        childCombo = $('#ddl_' + childReportProcedureId).data('tDropDownList');

//        if (childCombo) {
//            supressMessages = true;
//            childCombo.text('');
//            childCombo.reload();
//        }
//    }

    function onDropDownClosed(e) {
        var reportProcedureId = e.currentTarget.id.replace('ddl_', '');
        var childCount = $("#childId_count_" + reportProcedureId).val();

        if (childCount > 0) {
            $("#childId_ids_" + reportProcedureId + " input").each(
                function (index) {
                    var childId = $(this).val();
                    var childCombo = $("#ddl_" + childId).data('tDropDownList');

                    if (childCombo) {
                        supressMessages = true;
                        childCombo.text('');
                        childCombo.reload();
                    }
                }
            );

        }      
    }

    function onDropDownBinding(e)
    {
        var isValid = true;
        var reportProcedureId = e.currentTarget.id.replace('ddl_', '');
        var parentCount = $("#parent_count_" + reportProcedureId).val();
        
        if (parentCount > 0)
        {
            var parameters = "";
            var errorMessage = ""; 
            $("#parent_ids_" + reportProcedureId + " input").each(
                function (index)
                {
                    var parentId = $(this).val();
                    var parentValue = $("#ddl_" + parentId).data('tDropDownList').value();
                    parameters = parameters + "," + parentValue;

                    if (parentValue.length == 0)
                    {
                        e.preventDefault();
                        isValid = false;
                        var labelName = $("#label_" + parentId).text();
                        errorMessage = errorMessage + "<%: Shared.ReportingStrings.StandardReports_Error_MissingValue %>".format(labelName) + "<br>";
                    }
                }
            );

            if (isValid)
            {
                var isAdhocReport = $('#reportTypeAdhoc').val() == 'true';

                if (isAdhocReport)
                {
                    var reportId = $("#ReportList").data('tDropDownList').value();
                    var adhocReportSortOrder = reportProcedureId;

                    parameters = parameters + "," + reportId + "," + adhocReportSortOrder;
                }

                //add parameters to e.data if validation passes
                e.data = $.extend({}, e.data, { procedureParams: parameters });
            }
            else
            {
                if (!supressMessages)
                {
                    OTPAlert(errorMessage, "<%: Shared.ReportingStrings.StandardReports_Error_Title %>");
                }
            }
        }

        supressMessages = false;
    }

    function onReportChange()
    {
        var reportId = $("#ReportList").data('tDropDownList').value();

        if (reportId)
        {
            //Force ajax to retrieve new data each call
            var d = new Date();
            var refreshSeed = d.getTime();

            $("#ReportDescriptionTarget").html('');
            $('#ParameterTarget').html("<img src='../../Content/Images/ajax-loader.gif' style='display:block;margin:auto;' />");

            frame = window.frames["ReportViewFrame"];
            if (frame.document.body)
            {
                frame.document.body.innerHTML = "";
            }

            $.getJSON("/Reporting/GetReportDescription", { reportId: reportId, refresh: refreshSeed }, function (description)
            {
                $("#ReportDescriptionTarget").html(description);
            });

            $.getJSON("/Reporting/ReportParameters/", { reportId: reportId, refresh: refreshSeed }, function (data)
            {
                $("#ParameterTarget").html(data);
            });
        }
        else
        {
            $('#ParameterTarget').html('');
            $("#ReportDescriptionTarget").html('');
        }
    }

    function ProcessReport()
    {
        var isValid = true;
        var reportId = $("#ReportList").data('tDropDownList').value();
        if (reportId)
        {
            var parameters = "";
            var errorMessage = "";
            $("#parameterDropDowns .t-dropdown").each(
                function (index)
                {
                    var comboId = this.id.replace('ddl_', '');
                    var comboValue = $(this).data('tDropDownList').value();
                    parameters = parameters + "," + comboId + "," + comboValue;

                    if (comboValue.length == 0)
                    {
                        var labelName = $("#label_" + comboId).text();
                        errorMessage = errorMessage + "<%: Shared.ReportingStrings.StandardReports_Error_MissingValue %>".format(labelName) + "<br>";
                        isValid = false;
                    }
                }
            );

            if (isValid)
            {
                var securityUserId = $("#securityUserId").val();
                var form = $('<form method="post" target="ReportViewFrame" ' +
                    'action="../../Reports/ReportViewer.aspx?r=' + reportId + '&p=' + parameters + '&u=' + securityUserId + '"></form>');

                $("#HiddenFormTarget").append(form);
                form.submit();
                $("#HiddenFormTarget").empty();
            }
            else
            {
                OTPAlert(errorMessage, "<%: Shared.ReportingStrings.StandardReports_Error_Title %>");
            }
        }
        else
        {
            OTPAlert("<%: Shared.ReportingStrings.StandardReports_Error_MissingReport %>", "<%: Shared.ReportingStrings.StandardReports_Error_Title %>");
        }
    }

</script>

<div class="groupbox_600" style="margin-top: 0px">
    <div class="groupbox_title_background gradientHToWhite">
        <div class="groupbox_title">
            <div style="float:left; margin:5px 5px 5px 0px">
                <span class="sprite sprite-chart_bar" style="margin:0px 5px 0px 0px"></span>
            </div>
            <div style="float:left; margin:5px 5px 5px 0px">
                <%: Shared.ReportingStrings.StandardReports_ReportName %>:
            </div>
            <div style="float:right; margin: 5px 22px 5px 0px">
                <%= Html.Telerik().DropDownList()
                    .Name("ReportList")
                    .HtmlAttributes(new { style = "width:300px;font-size:12px;margin-left:5px" })
                    .BindTo(new SelectList(Model, "Id", "Description"))
                    .ClientEvents(events => events
                        .OnChange("onReportChange")
                        .OnLoad("onReportChange"))
                %>
            </div>
            <div class="clear"></div>
        </div>
    </div>
    <div class="groupbox_content">
        <div id="ReportDescriptionTarget"></div>
    </div>
    <img src="../../Content/Images/sidenav_btm.gif" width="600" height="9" /> 
</div> 

<div style="margin:15px 0px 10px 20px; width:600px">
    &nbsp;<div id="ParameterTarget"></div>
</div>
<div id="HiddenFormTarget" style="display:none;"></div>
<input type="hidden" id="securityUserId" value="<%= ViewData["securityUserId"] %>" />

<iframe frameborder="no" style="border-style:solid; border-width:thin; border-color:#6b6b6b;width: 100%;height: 600px; overflow: auto;" id="ReportViewFrame" name="ReportViewFrame" ></iframe>

