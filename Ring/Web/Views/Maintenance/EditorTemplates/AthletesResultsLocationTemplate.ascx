<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MaintenanceAthletesResultsViewModel>" %>

<script type="text/javascript">
    function onLocationComboLoad() {
        var combo = $(this).data('tComboBox');
        combo.reload();

    }

    function onLocationComboBinding(e) {
        var resourceId = $('#resourceId').val();
        e.data = $.extend({}, e.data, { id: resourceId });
    }

    function onLocationComboBound() {
        var editor = $(this).data('tComboBox');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem) {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow) {
                editor.value(dataItem.Location);
            }
        }
    }
</script>

<%=         
    Html.Telerik().ComboBox()
        .Name("Location")
            .DataBinding(binding => binding.Ajax().Select("AthleteResultsLocationAjax", "Shared"))
        .HtmlAttributes(new { style = "font-size:8pt;" })
        .ClientEvents(events => events
            .OnLoad("onLocationComboLoad")
            .OnDataBinding("onLocationComboBinding")
            .OnDataBound("onLocationComboBound")
            )
%>
