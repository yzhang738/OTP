<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ResultsLoadTemplateDownloadTemplateMappingsGridViewModel>" %>

<script type="text/javascript">
    function onColumnFormatComboLoad()
    {
//        var combo = $(this).data('tDropDownList');
//        combo.reload();
    }

    function onColumnFormatComboBound()
    {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem)
        {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow)
            {
                editor.value(dataItem.ColumnFormat);
            }
        }
    }

    function onColumnFormatComboBinding(e)
    {
        var downloadTemplateId = $("#downloadTemplateId").val();
        var fieldName = $('#FieldName').data('tDropDownList').value();

        e.data = $.extend({}, e.data, { downloadTemplateId: downloadTemplateId, fieldName: fieldName });
    }

</script>

<%=         
    Html.Telerik().DropDownList()
        .Name("ColumnFormat")
        .DataBinding(binding => binding.Ajax().Select("ColumnFormatAjax", "Shared"))
        .HtmlAttributes(new { style = "font-size:8pt" })
        .ClientEvents(events => events
            .OnLoad("onColumnFormatComboLoad")
            .OnDataBound("onColumnFormatComboBound")
            .OnDataBinding("onColumnFormatComboBinding"))
%>
