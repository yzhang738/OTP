<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<int?>" %>

 <script type="text/javascript">

     function onFundingresourceComboLoad() {
         var combo = $(this).data('tComboBox');
         combo.reload();
     }

     function onFundingresourceComboBinding(e) {
         var categoryRow = $(e.currentTarget).closest('.t-detail-row').prev();
         var categoryDataItem = categoryRow.closest('.t-grid').data('tGrid').dataItem(categoryRow);
         var fundingId = $("#hidFundingId").val();
         var cscId = $(e.currentTarget).closest('.t-edit-form tr td').eq(0).prev().find('input,select').val();
         e.data = $.extend({}, e.data, { fundingCategoryType: categoryDataItem.Type, fundingId: fundingId, cscId: cscId });
     }

     function onFundingresourceComboBound() {
         var editor = $(this).data('tComboBox');
         var tr = $(this).closest('tr:has(form)');
         var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

         if (dataItem) {
             var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
             if (!isNewRow) editor.value(dataItem.ResourceId);
         }
     }

</script>


<%= 
    Html.Telerik().ComboBox()
        .Name("ResourceId")
        .DataBinding(binding => binding.Ajax().Select("GetFundingResourcesByCscAjax", "Funding"))
        .ClientEvents(events => events
            .OnLoad("onFundingresourceComboLoad")
            .OnDataBinding("onFundingresourceComboBinding")
            .OnDataBound("onFundingresourceComboBound")
            )
        .HtmlAttributes(new { style = "font-size:8pt;" })
%>
