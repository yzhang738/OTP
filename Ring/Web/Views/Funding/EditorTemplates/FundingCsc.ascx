<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>

 <script type="text/javascript">

     function onFundingCscComboLoad() {
         var combo = $(this).data('tDropDownList');
         combo.reload();
     }

     function onFundingCscComboClosed(e) {
         var dropDownList = $('#ResourceId').data('tComboBox');
         dropDownList.text('');
         dropDownList.reload();
     }
     
     function onFundingCscComboBound() {
         var editor = $(this).data('tDropDownList');
         var tr = $(this).closest('tr:has(form)');
         var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

         if (dataItem) {
             var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
             if (!isNewRow) editor.value(dataItem.CscId);
         }
         onFundingCscComboClosed();
     }

</script>


<%= 
    Html.Telerik().DropDownList()
        .Name("CscId")
        .DataBinding(binding => binding.Ajax().Select("GetAllOrganizationsAjax", "Funding", new { organizationId = (string)ViewData["organizationId"]}))
        .ClientEvents(events => events
                .OnLoad("onFundingCscComboLoad")
                .OnDataBound("onFundingCscComboBound")
                .OnClose("onFundingCscComboClosed")
            )
        .HtmlAttributes(new { style = "font-size:8pt;" })
%>
