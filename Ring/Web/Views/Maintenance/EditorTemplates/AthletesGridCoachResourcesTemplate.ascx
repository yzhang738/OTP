<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<int?>" %>

<script type="text/javascript">
    function onPersonalCoachLoad() {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onPersonalCoachBinding(e) {
        // Get the resource Id
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem) {
            var resourceId = dataItem.ResourceId;
        }

        var sportId = $(e.currentTarget).closest('.t-edit-form tr td').eq(0).prev().prev().prev().prev().prev().find('input,select').val();
        
        e.data = $.extend({}, e.data, { sportId: sportId }, { id: resourceId });
    }

    function onPersonalCoachBound() {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);
        
        if (dataItem) {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow) editor.value(dataItem.PersonalCoachName);
        }
    }
</script>

<%=         
    Html.Telerik().DropDownList()
        .Name("PersonalCoachName")
        .DataBinding(binding => binding.Ajax().Select("CoachResourcesAjax", "Shared"))
        .HtmlAttributes(new { style = "font-size:8pt;" })
        .ClientEvents(events => events
            .OnLoad("onPersonalCoachLoad")
            .OnDataBinding("onPersonalCoachBinding")
            .OnDataBound("onPersonalCoachBound"))
%>
