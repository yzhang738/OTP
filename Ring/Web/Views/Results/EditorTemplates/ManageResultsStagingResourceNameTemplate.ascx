<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ResultsLoadTemplateDownloadTemplateMappingsGridViewModel>" %>

<script type="text/javascript">
    function onResourceIdComboLoad()
    {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onResourceIdComboBinding(e)
    {
        var masterRow = $(e.currentTarget).closest('.t-detail-row').prev();
        var parentDataItem = masterRow.closest('.t-grid').data('tGrid').dataItem(masterRow);

        var resultStagingId = "";

        if (parentDataItem)
        {
            resultStagingId = parentDataItem.ResultStagingId;
        }

        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        var curval = "";

        if (dataItem)
        {
            curval = dataItem.ResourceId;
        }

        e.data = $.extend({}, e.data, { resultStagingId: resultStagingId, currentResourceId: curval });
    }

    function onResourceIdComboBound()
    {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem)
        {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow)
            {
                editor.value(dataItem.ResourceId);
            }
        }
    }

</script>

<%=         
    Html.Telerik().DropDownList()
        .Name("ResourceId")
        .DataBinding(binding => binding.Ajax().Select("ResultStagingResourceNameAjax", "Shared"))
        .HtmlAttributes(new { style = "font-size:8pt" })
        .ClientEvents(events => events
                .OnDataBound("onResourceIdComboBound")
                .OnLoad("onResourceIdComboLoad")
                .OnDataBinding("onResourceIdComboBinding")
                )
%>
