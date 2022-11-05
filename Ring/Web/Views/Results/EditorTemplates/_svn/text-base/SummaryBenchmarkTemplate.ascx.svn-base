<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<int>" %>

<script type="text/javascript">

    function onSummaryBenchmarksComboLoad() {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onSummaryBenchmarksComboBound() {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem) {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow) editor.value(dataItem.BenchmarkCode.toString());
        }
    }
</script>

<%= 
    Html.Telerik().DropDownList()
        .Name("BenchmarkCode")
        .DataBinding(binding => binding.Ajax().Select("GetSummaryBenchmarksAjax", "Results"))
        .ClientEvents(e => e
                .OnLoad("onSummaryBenchmarksComboLoad")
                .OnDataBound("onSummaryBenchmarksComboBound"))
        .HtmlAttributes(new { style = "font-size:8pt;" })
%>
