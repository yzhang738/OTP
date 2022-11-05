<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>

 <script type="text/javascript">

     function onFundingPriorityComboLoad() {
         var combo = $(this).data('tDropDownList');
         combo.reload();
     }

     function onFundingPriorityComboBound() {
         var editor = $(this).data('tDropDownList');
         var tr = $(this).closest('tr:has(form)');
         var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

         if (dataItem) {
             var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
             if (!isNewRow) editor.value(dataItem.PriorityCode);
         }
     }

</script>


<%= 
    Html.Telerik().DropDownList()
        .Name("PriorityCode")
        .DataBinding(binding => binding.Ajax().Select("GetFundingPriorityAjax", "Funding"))
        .ClientEvents(events => events
                .OnLoad("onFundingPriorityComboLoad")
                .OnDataBound("onFundingPriorityComboBound")
            )
        .HtmlAttributes(new { style = "font-size:8pt;" })
%>
