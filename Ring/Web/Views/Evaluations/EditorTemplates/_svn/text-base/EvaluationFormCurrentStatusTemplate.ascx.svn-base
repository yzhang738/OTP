<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>

<script type="text/javascript">
    function onCurrentStatusComboLoad() {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onCurrentStatusComboBound() {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem) {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow) editor.value(dataItem.CurrentStatus);
        }
    }
</script>

<%=         
    Html.Telerik().DropDownList()
        .Name("CurrentStatus")
        .DataBinding(binding => binding.Ajax().Select("EvaluationStatusAjax", "Shared"))
        .HtmlAttributes(new { style = "font-size:8pt;width:100px" })
        .ClientEvents(events => events
            .OnLoad("onCurrentStatusComboLoad")
            .OnDataBound("onCurrentStatusComboBound"))
%>

