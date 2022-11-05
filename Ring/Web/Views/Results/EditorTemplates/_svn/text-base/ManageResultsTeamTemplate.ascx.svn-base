<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<int?>" %>

 <script type="text/javascript">
     function onTeamComboLoad() {
         var combo = $(this).data('tDropDownList');
         combo.reload();
     }

     function onTeamComboBinding(e) {

         var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);

         if (isNewRow) {
             var countryId = $(e.currentTarget).closest('.t-edit-form tr td').eq(0).prev().find('input,select').val();
             var eventId = $(e.currentTarget).closest('.t-edit-form tr td').eq(0).prev().prev().prev().find('input,select').val();

             e.data = $.extend({}, e.data, { countryCode: countryId, sportId: eventId });

             var currentPosition = $(e.currentTarget).closest('.t-edit-form tr td').eq(0).prev().prev().find('input').val();
             if (currentPosition.length == 0 || currentPosition == 0) {
                 var refreshSeed = new Date().getTime();
                 var competitionId = $("#TeamManagementWindow").find('input[name=hiddenCompetitionId]').val();

                 $.getJSON("/Results/GetNextPositionByEvent", { competitionId: competitionId, eventId: eventId, refresh: refreshSeed }, function (nextPosition) {
                     $(e.currentTarget).closest('.t-edit-form tr td').eq(0).prev().prev().find('input').val(nextPosition);
                 });
             }
         }
         else {
             var tr = $(this).closest('tr:has(form)');
             var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

             if (dataItem) {
                 e.data = $.extend({}, e.data, { teamType: dataItem.TeamType, countryCode: dataItem.CountryCode, sportId: dataItem.EventId });
             }
         }
     }

     function onTeamComboBound() {
         var editor = $(this).data('tDropDownList');
         var tr = $(this).closest('tr:has(form)');
         var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

         if (dataItem) {
             var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
             if (!isNewRow) editor.value(dataItem.TeamId);
         }
     }

</script>
<%= 
    Html.Telerik().DropDownList()
            .Name("TeamId")
            .DataBinding(binding => binding.Ajax().Select("GetTeamsAjax", "Results"))
            .ClientEvents(events => events
                .OnDataBinding("onTeamComboBinding")
                .OnDataBound("onTeamComboBound")
                .OnLoad("onTeamComboLoad"))
        .HtmlAttributes(new { style = "font-size:8pt;" })
%>
