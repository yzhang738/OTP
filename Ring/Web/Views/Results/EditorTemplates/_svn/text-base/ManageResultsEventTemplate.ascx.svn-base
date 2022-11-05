<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>

<script type="text/javascript">
    function onEventComboLoad()
    {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onEventComboBound()
    {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem)
        {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow)
            {
                editor.value(dataItem.EventId);
                editor.disable();
            }
        }
    }
</script>

<%= 
    Html.Telerik().DropDownList()
        .Name("EventId")
        .DataBinding(binding => binding.Ajax().Select("GetEventsByCompetitionAjax", "Results", new { competitionId = (int)ViewData["competitionId"] }))
        .ClientEvents(events => events
            .OnDataBound("onEventComboBound")
            .OnLoad("onEventComboLoad"))
        .HtmlAttributes(new { style = "font-size:8pt;" })
%>

