<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<int>" %>

<script type="text/javascript">

    function onBenchmarksComboLoad() {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onBenchmarksComboBound()
    {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem)
        {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow)
            {
                editor.value(dataItem.BenchmarkTypeId);
            }
        }
    }
</script>

<%= 
    Html.Telerik().DropDownList()
        .Name("BenchmarkTypeId")
        .DataBinding(binding => binding.Ajax().Select("BenchmarkTypeAjax", "Shared"))
        .ClientEvents(e => e
                .OnLoad("onBenchmarksComboLoad")
                .OnDataBound("onBenchmarksComboBound"))
        .HtmlAttributes(new { style = "font-size:8pt;" })
%>
