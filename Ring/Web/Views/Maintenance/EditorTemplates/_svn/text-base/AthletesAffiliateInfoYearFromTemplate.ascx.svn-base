<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<script type="text/javascript">
    function onYearFromLoad()
    {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onYearFromBound()
    {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem) {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow) editor.value(dataItem.YearFromId);
        }
    }
</script>

<%=         
    Html.Telerik().DropDownList()
        .Name("YearFromId")
        .DataBinding(binding => binding.Ajax().Select("AllYearsAjax", "Shared"))
        .HtmlAttributes(new { style = "width:100px;font-size:8pt;" })
        .ClientEvents(events => events
            .OnLoad("onYearFromLoad")
            .OnDataBound("onYearFromBound"))
%>