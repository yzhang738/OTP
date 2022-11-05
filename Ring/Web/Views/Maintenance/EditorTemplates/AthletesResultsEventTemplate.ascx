<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MaintenanceAthletesResultsViewModel>" %>

<script type="text/javascript">
    function onEventComboBinding(e) {
        //Get the previous drop down list's value
        var disciplineId = $(e.currentTarget).closest('.t-edit-form tr td').eq(0).prev().find('input,select').val();
        e.data = $.extend({}, e.data, { disciplineId: disciplineId });
    }

    function onEventComboBound() {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem) {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow) editor.value(dataItem.EventID);
        }
    }
</script>

<%= 
    Html.Telerik().DropDownList()
        .Name("EventName")
        .DataBinding(binding => binding.Ajax().Select("ResultsEventsByDisciplineAjax", "Shared"))
        .ClientEvents(events => events
            .OnDataBinding("onEventComboBinding")
            .OnDataBound("onEventComboBound"))
        .HtmlAttributes(new { style = "font-size:8pt;" })
%>
