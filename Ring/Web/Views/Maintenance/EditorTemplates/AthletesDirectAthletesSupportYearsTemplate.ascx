<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>

<script type="text/javascript">
    function onYearsComboLoad()
    {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onYearsComboBound()
    {
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
        .HtmlAttributes(new
        {
            style = "width:100px;font-size:8pt;"
        })
        .DataBinding(binding => binding.Ajax().Select("AllYearsAjax", "Shared"))
        .ClientEvents(events => events
            .OnLoad("onYearsComboLoad")
            .OnDataBound("onYearsComboBound"))
%>
