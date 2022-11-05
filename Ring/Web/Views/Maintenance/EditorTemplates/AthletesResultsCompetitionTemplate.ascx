<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MaintenanceAthletesResultsViewModel>" %>

<script type="text/javascript">
    function onCompetitionComboLoad() {
        var combo = $(this).data('tDropDownList');
        combo.reload();        
    }

    function onCompetitionComboBinding(e) {
        var resourceId = $('#resourceId').val();
        e.data = $.extend({}, e.data, { id: resourceId });
    }

    function onCompetitionComboBound() {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem) {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow) {
                editor.value(dataItem.CompetitionName);
            }
        }
    }
</script>

<%=         
    Html.Telerik().DropDownList()
        .Name("CompetitionName")
        .DataBinding(binding => binding.Ajax().Select("AthleteResultsCompetitionAjax", "Shared"))
        .HtmlAttributes(new { style = "font-size:8pt;" })
        .ClientEvents(events => events
            .OnLoad("onCompetitionComboLoad")
            .OnDataBinding("onCompetitionComboBinding")
            .OnDataBound("onCompetitionComboBound")
            )
%>

