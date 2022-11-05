<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<int?>" %>

<script type="text/javascript">
    function onEventCoachBound() {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem) {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow) editor.value(dataItem.EventCoachId);
        }
    }

    function onEventCoachBinding(e)
    {
        //Get the previous drop down list's value
        var sportId = $(e.currentTarget).closest('.t-edit-form tr td').eq(0).prev().prev().prev().prev().prev().find('input,select').val();
        
        // Get the resource Id
        var resourceId = $('#resourceId').val();

        e.data = $.extend({}, e.data, { sportId: sportId }, { id: resourceId });
    }
</script>

<%=         
    Html.Telerik().DropDownList()
        .Name("EventCoachId")
        .DataBinding(binding => binding.Ajax().Select("CoachResourcesSportInfoAjax", "Shared"))
        .HtmlAttributes(new { style = "font-size:8pt;" })
        .ClientEvents(events => events
            .OnDataBound("onEventCoachBound")
            .OnDataBinding("onEventCoachBinding")
        )
%>
