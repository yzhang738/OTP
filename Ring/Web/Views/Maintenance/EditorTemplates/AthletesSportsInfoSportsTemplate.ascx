<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>

<script type="text/javascript">
    function onAllSportsComboClosed(e)
    {
        var disciplineDropDownList = $('#DisciplineName').data('tDropDownList');
        disciplineDropDownList.text('');
        disciplineDropDownList.reload();

        var coachDropDownList = $('#EventCoachId').data('tDropDownList');
        coachDropDownList.text('');
        coachDropDownList.reload();

    }

    function onAllSportsComboLoad()
    {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onAllSportsComboBinding(e) {
        var resourceId = $('#resourceId').val();
        e.data = $.extend({}, e.data, { id: resourceId });
    }

    function onAllSportsComboBound() {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem) {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow) editor.value(dataItem.SportsId);
        }

        onAllSportsComboClosed();
    }
</script>

<%= 
    Html.Telerik().DropDownList()
        .Name("SportName")
        .DataBinding(binding => binding.Ajax().Select("AllSportsOnSportLevelAjax", "Shared"))
        .ClientEvents(e => e
            .OnClose("onAllSportsComboClosed")
            .OnLoad("onAllSportsComboLoad")
            .OnDataBinding("onAllSportsComboBinding")
            .OnDataBound("onAllSportsComboBound"))
        .HtmlAttributes(new { style = "font-size:8pt;" })
%>
