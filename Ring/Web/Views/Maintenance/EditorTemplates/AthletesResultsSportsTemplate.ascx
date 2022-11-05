<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MaintenanceAthletesResultsViewModel>" %>

<script type="text/javascript">
    function onSportComboClosed(e) {
        var disciplineDropDownList = $('#DisciplineName').data('tDropDownList');
        disciplineDropDownList.text('');
        disciplineDropDownList.reload();
    }

    function onSportComboLoad() {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onSportComboBinding(e) {
        var resourceId = $('#resourceId').val();
        e.data = $.extend({}, e.data, { id: resourceId });
    }

    function onSportComboBound() {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem) {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow) editor.value(dataItem.SportID);
        }

        onSportComboClosed();
    }
</script>

<%=         
    Html.Telerik().DropDownList()
        .Name("SportName")
        .DataBinding(binding => binding.Ajax().Select("AllSportsOnAddResultAjax", "Shared"))
        .HtmlAttributes(new { style = "font-size:8pt;" })
        .ClientEvents(events => events
            .OnClose("onSportComboClosed")
            .OnLoad("onSportComboLoad")
            .OnDataBinding("onSportComboBinding")
            .OnDataBound("onSportComboBound")
            )
%>

