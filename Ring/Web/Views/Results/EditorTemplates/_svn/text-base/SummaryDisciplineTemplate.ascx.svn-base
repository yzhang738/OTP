<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>

 <script type="text/javascript">

     function onDisciplineComboLoad() {
         var combo = $(this).data('tDropDownList');
         combo.reload();
     }

     function onDisciplineComboBinding(e) {
         var competitionRow = $(e.currentTarget).closest('.t-detail-row').prev();
         var competitionDataItem = competitionRow.closest('.t-grid').data('tGrid').dataItem(competitionRow);
         e.data = $.extend({}, e.data, { sportId: competitionDataItem.SportId });
     }


     function onDisciplineComboClosed(e) {
         var dropDownList = $('#EventId').data('tDropDownList');
         dropDownList.text('');
         dropDownList.reload();
     }

     function onDisciplineComboBound() {
         var editor = $(this).data('tDropDownList');
         var tr = $(this).closest('tr:has(form)');
         var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

         if (dataItem) {
             var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
             if (!isNewRow) editor.value(dataItem.DisciplineId);
         }
         onDisciplineComboClosed();
     }

</script>
<%= 
    Html.Telerik().DropDownList()
        .Name("DisciplineId")
        .DataBinding(binding => binding.Ajax().Select("GetDisciplinesBySportAjax", "Results"))
            .ClientEvents(events => events
                .OnLoad("onDisciplineComboLoad")
                .OnDataBinding("onDisciplineComboBinding")
                .OnClose("onDisciplineComboClosed")
                .OnDataBound("onDisciplineComboBound")
                )
        .HtmlAttributes(new { style = "font-size:8pt;" })
%>
