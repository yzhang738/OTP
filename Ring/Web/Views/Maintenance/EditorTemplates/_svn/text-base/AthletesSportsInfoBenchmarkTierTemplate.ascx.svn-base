<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<int>" %>

<script type="text/javascript">
    function onTierComboDataBinding(e)
    {
        var masterRow = $(e.currentTarget).closest('.t-detail-row').prev();
        var parentDataItem = masterRow.closest('.t-grid').data('tGrid').dataItem(masterRow);
        
        if (parentDataItem)
        {
            var sportId = parentDataItem.SportsId;
            e.data = $.extend({}, e.data, { sportId: sportId });
        }
    }

    function onTierComboBound()
    {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem) {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow) editor.value(dataItem.BenchmarkTierId);
        }
    }

    function onTierComboLoad()
    {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }
</script>

<%= 
    Html.Telerik().DropDownList()
        .Name("BenchmarkTierId")
        .DataBinding(binding => binding.Ajax().Select("AthletesBenchmarkSportsBenchmarkTierAjax", "Maintenance"))
        .ClientEvents(events => events
            .OnDataBinding("onTierComboDataBinding")
            .OnDataBound("onTierComboBound")
            .OnLoad("onTierComboLoad"))
%>
  