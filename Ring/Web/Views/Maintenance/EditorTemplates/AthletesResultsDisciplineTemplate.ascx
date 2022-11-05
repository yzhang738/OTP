<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MaintenanceAthletesResultsViewModel>" %>

<script type="text/javascript">
    function onDisciplineComboBinding(e) {
        //Get the previous drop down list's value
        var sportId = $(e.currentTarget).closest('.t-edit-form tr td').eq(0).prev().find('input,select').val();

        // Get the resource Id
        var resourceId = $('#resourceId').val();

        e.data = $.extend({}, e.data, { sportId: sportId }, { id: resourceId });
    }

    function onDisciplineComboClosed(e) {
        var dropDownList = $('#EventName').data('tDropDownList');
        dropDownList.text('');
        dropDownList.reload();
    }

    function onDisciplineComboBound() {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem) {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow) editor.value(dataItem.DisciplineID);
        }

        onDisciplineComboClosed();
    }
</script>

<%=         
    Html.Telerik().DropDownList()
        .Name("DisciplineName")
        .DataBinding(binding => binding.Ajax().Select("DisciplinesBySportAjax", "Shared"))
        .HtmlAttributes(new { style = "font-size:8pt;" })
        .ClientEvents(events => events
            .OnDataBinding("onDisciplineComboBinding")
            .OnClose("onDisciplineComboClosed")
            .OnDataBound("onDisciplineComboBound"))
%>

