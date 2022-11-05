<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>

<script type="text/javascript">
    function onYearToLoad() {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onYearToBound() {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem) {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow) editor.value(dataItem.YearId);
        }
    }

</script>

<%= 
    Html.Telerik().DropDownList()
        .Name("YearId")
        .DataBinding(binding => binding.Ajax().Select("AllYearsAjax", "Shared"))
        .HtmlAttributes(new { style = "font-size:8pt;" })
        .ClientEvents(events => events
            .OnLoad("onYearToLoad")
            .OnDataBound("onYearToBound"))
%>
