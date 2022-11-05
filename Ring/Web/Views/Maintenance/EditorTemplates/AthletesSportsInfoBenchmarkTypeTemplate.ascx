<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<script type="text/javascript">
    function onBenchmarkTypeLoad()
    {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onBenchmarkTypeBound()
    {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem) {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow) editor.value(dataItem.BenchmarkTypeId);
        }
    }
</script>

<%=         
    Html.Telerik().DropDownList()
        .Name("BenchmarkTypeId")
        .DataBinding(binding => binding.Ajax().Select("BenchmarkTypeAjax", "Shared"))
        .HtmlAttributes(new { style = "font-size:8pt;" })
        .ClientEvents(events => events
            .OnLoad("onBenchmarkTypeLoad")
            .OnDataBound("onBenchmarkTypeBound"))
%>