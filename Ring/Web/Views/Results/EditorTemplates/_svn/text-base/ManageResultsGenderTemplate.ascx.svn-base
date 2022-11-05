<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>

<script type="text/javascript">
    function onGenderComboLoad() {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onGenderComboBound() {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem) {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow) editor.value(dataItem.GenderCode);
        }
    }
</script>

<%=         
    Html.Telerik().DropDownList()
        .Name("GenderCode")
        .DataBinding(binding => binding.Ajax().Select("GenderAjax", "Shared", new { includeNull = true }))
        .HtmlAttributes(new { style = "font-size:8pt;width:90px" })
        .ClientEvents(events => events
            .OnLoad("onGenderComboLoad")
            .OnDataBound("onGenderComboBound"))
%>

