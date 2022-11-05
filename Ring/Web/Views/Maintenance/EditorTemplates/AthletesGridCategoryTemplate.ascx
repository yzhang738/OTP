<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MaintenanceAthletesAthleteGridViewModel>" %>

<script type="text/javascript">
    function onCategoryComboLoad() {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onCategoryComboBound() {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem) {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow) editor.value(dataItem.Category);
        }
    }
</script>

<%=         
    Html.Telerik().DropDownList()
        .Name("Category")
        .DataBinding(binding => binding.Ajax().Select("CategoryAjax", "Shared"))
        .HtmlAttributes(new { style = "font-size:8pt;width:100px" })
        .ClientEvents(events => events
        .OnLoad("onCategoryComboLoad")
        .OnDataBound("onCategoryComboBound"))
%>
