<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>

<script type="text/javascript">

    function onAllSportsComboBinding(e) {
        var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
        if (isNewRow) e.data = $.extend({}, e.data, { filterByRole: true });
    }

    function onAllSportsComboClose(e) {
        var selectedSportId = $('#SportId option:selected').val();

        // send an ajax request to get the data and post it to the datepicker
        $.ajax(
        {
            type: "POST",
            url: "/Maintenance/LoadCompetitionDate",
            data: { id: selectedSportId },
            success: function (data) {
            },
            complete: function (data) {

            }
        });
    }

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
                editor.value(dataItem.SportId);
                editor.disable();
            }
        }
    }
</script>

<%= 
    Html.Telerik().DropDownList()
        .Name("SportId")
        .DataBinding(binding => binding.Ajax().Select("GetSportsOnSportLevelAjax", "Results"))
        .ClientEvents(e => e
            .OnDataBinding("onAllSportsComboBinding")
            .OnLoad("onAllSportsComboLoad")
            .OnDataBound("onAllSportsComboBound"))
        .HtmlAttributes(new { style = "font-size:8pt;" })
%>
