<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>

<script type="text/javascript">
    function onAllSportsComboLoad() {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onAllSportsComboBound() {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem) {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow) {
                editor.value(dataItem.SportsId);
            }
        }
    }

</script>

<%=         
    Html.Telerik().DropDownList()
        .Name("SportsId")
        .DataBinding(binding => binding.Ajax().Select("AllSportsUnderSportLevelAjax", "Shared"))
        .HtmlAttributes(new { style = "font-size:8pt;width:1000px" })
        .ClientEvents(events => events
        .OnLoad("onAllSportsComboLoad")
        .OnDataBound("onAllSportsComboBound"))
%>

