<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<int>" %>

<script type="text/javascript">
    function onResourceComboLoad() {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onResourceComboBinding(e) {
        var sportId = $("#TeamManagementWindow").find('input[name=hiddenSportId]').val();
        var countryId = $("#TeamManagementWindow").find('input[name=hiddenCountryCode]').val();

        e.data = $.extend({}, e.data, { sportId: sportId, countryId: countryId });
    }


    function onResourceComboBound() {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem) {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow) editor.value(dataItem.ResourceId);
        }
    }
</script>

<%= 
    Html.Telerik().DropDownList()
        .Name("ResourceId")
        .DataBinding(binding => binding.Ajax().Select("GetAthletesBySportAjax", "Results"))
        .ClientEvents(events => events
            .OnDataBinding("onResourceComboBinding")
            .OnDataBound("onResourceComboBound")
            .OnLoad("onResourceComboLoad"))
        .HtmlAttributes(new { style = "font-size:8pt;" })
%>

