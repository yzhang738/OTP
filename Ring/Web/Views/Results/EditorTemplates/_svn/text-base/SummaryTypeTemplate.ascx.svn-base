<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>

<script type="text/javascript">

    function onSummaryTypesComboLoad() {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onSummaryTypesComboBound() {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem) {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow) editor.value(dataItem.Type);
        }
    }
</script>

<%= 
    Html.Telerik().DropDownList()
        .Name("Type")
        .DataBinding(binding => binding.Ajax().Select("GetSummaryTypesAjax", "Results"))
        .ClientEvents(e => e
                .OnLoad("onSummaryTypesComboLoad")
                .OnDataBound("onSummaryTypesComboBound"))
        .HtmlAttributes(new { style = "font-size:8pt;" })
%>
