<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<script type="text/javascript">
    function onReportFormatStringBinding(e)
    {
        var reportId = $("#reportId").val();
        var columnFieldName_EN = $('#ColumnFieldName_EN').data('tDropDownList').value();

        e.data = $.extend({}, e.data, { reportId: reportId, columnFieldName_EN: columnFieldName_EN });
    }

    function onReportFormatStringBound()
    {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem)
        {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow)
            {
                editor.value(dataItem.FormatString);
            }
        }
    }
</script>

<%= 
    Html.Telerik().DropDownList()
        .Name("FormatString")
        .HtmlAttributes(new
        {
            style = "width:110px;font-size:8pt;"
        })
        .DataBinding(binding => binding.Ajax().Select("FormatStringAjax", "Shared"))
        .ClientEvents(e => e
            .OnDataBound("onReportFormatStringBound")
            .OnDataBinding("onReportFormatStringBinding"))
%>
