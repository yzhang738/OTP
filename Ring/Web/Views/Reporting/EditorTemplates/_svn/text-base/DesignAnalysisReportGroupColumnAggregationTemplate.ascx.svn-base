<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<script type="text/javascript">
    function onReportAggregationComboBinding(e)
    {
        var reportId = $("#reportId").val();
        var columnFieldName_EN = $('#ColumnFieldName_EN').data('tDropDownList').value();
        e.data = $.extend({}, e.data, { reportId: reportId, columnFieldName_EN: columnFieldName_EN });
    }

    function onReportAggregationComboBound()
    {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem)
        {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow)
            {
                editor.value(dataItem.ReportAggregation);
            }
        }
    }
</script>

<%= 
    Html.Telerik().DropDownList()
        .Name("ReportAggregation")
        .HtmlAttributes(new
        {
            style = "width:110px;font-size:8pt;"
        })
        .DataBinding(binding => binding.Ajax().Select("ReportAggregationAjax", "Shared"))
        .ClientEvents(e => e
            .OnDataBound("onReportAggregationComboBound")
            .OnDataBinding("onReportAggregationComboBinding"))
%>
