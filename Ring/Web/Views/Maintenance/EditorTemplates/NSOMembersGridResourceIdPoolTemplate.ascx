<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<int>" %>

 <script type="text/javascript">
     function onResourceIdPoolComboBinding(e)
     {
         var resourceId = $('#resourceId').val();
         e.data = $.extend({}, e.data, { resourceId: resourceId });
     }

    function onResourceIdPoolComboBound()
    {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem)
        {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow)
            {
                editor.value(dataItem.ResourceIdPool);
            }
        }
    }
</script>

<%= 
    Html.Telerik().DropDownList()
        .Name("ResourceIdPool")
        .DataBinding(binding => binding.Ajax().Select("ResourcePoolAssignmentAjax", "Shared"))
        .ClientEvents(events => events
                .OnDataBinding("onResourceIdPoolComboBinding")
                .OnDataBound("onResourceIdPoolComboBound"))
        .HtmlAttributes(new { style = "font-size:8pt;" })
%>
