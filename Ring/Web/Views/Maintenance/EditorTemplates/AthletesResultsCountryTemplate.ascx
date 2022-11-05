<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MaintenanceAthletesResultsViewModel>" %>

<script type="text/javascript">
    function onCountryComboLoad() {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onCountryComboBinding(e) {
        var resourceId = $('#resourceId').val();
        e.data = $.extend({}, e.data, { id: resourceId });
    }

    function onCountryComboBound() {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem) {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (isNewRow) {
                editor.value('CAN');
            }
            else {
                editor.value(dataItem.CountryName);
            }
        }
    }
</script>

<%=         
    Html.Telerik().DropDownList()
        .Name("CountryName")
        .DataBinding(binding => binding.Ajax().Select("AthleteResultsCountryAjax", "Shared"))
        .HtmlAttributes(new { style = "font-size:8pt;" })
        .ClientEvents(events => events
            .OnLoad("onCountryComboLoad")
            .OnDataBinding("onCountryComboBinding")
            .OnDataBound("onCountryComboBound")
            )
%>

