<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<script type="text/javascript">
    function onDirectAthleteSupportTypeComboClosed(e)
    {
        var categoryDropDownList = $('#DirectAthleteSupportCode').data('tDropDownList');
        categoryDropDownList.text('');
        categoryDropDownList.reload();
    }

    function onDirectAthleteSupportTypeComboLoad()
    {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onDirectAthleteSupportTypeComboBound()
    {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem) {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow) editor.value(dataItem.DirectAthleteSupportTypeId);
        }

        onDirectAthleteSupportTypeComboClosed();
    }
</script>

<%= 
    Html.Telerik().DropDownList()
        .Name("DirectAthleteSupportTypeId")
        .HtmlAttributes(new
        {
            style = "width:110px;font-size:8pt;"
        })
        .DataBinding(binding => binding.Ajax().Select("DirectAthleteSupportTypesAjax", "Shared"))
        .ClientEvents(e => e
            .OnClose("onDirectAthleteSupportTypeComboClosed")
            .OnLoad("onDirectAthleteSupportTypeComboLoad")
            .OnDataBound("onDirectAthleteSupportTypeComboBound"))
%>
