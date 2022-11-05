<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MaintenanceAthletesAthleteGridViewModel>" %>

<script type="text/javascript">
    function onCountryComboLoad()
    {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onCountryComboBound()
    {
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
        .DataBinding(binding => binding.Ajax().Select("CountryAjax", "Shared"))
        .HtmlAttributes(new { style = "font-size:8pt;width:110px" })
        .ClientEvents(events => events
            .OnLoad("onCountryComboLoad")
            .OnDataBound("onCountryComboBound"))
%>
