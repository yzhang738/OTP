<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MaintenanceAthletesResultsViewModel>" %>

<script type="text/javascript">
    function onLoad(e) {
        var datestring = '<%= ViewData["AthleteResultsCompetitionDate"] %>';

        var datepicker = $(this).data('tDatePicker');
        datepicker.value(datestring);
    }

</script>

<%=         
    Html.Telerik().DatePicker()
        .Name("EventDate")
        .HtmlAttributes(new { style = "font-size:8pt;" })
        .ShowButton(true)
        .Value("2012/07/27")
        .Format("yyyy/MM/dd")
        .ClientEvents(events => events.OnLoad("onLoad")
        ) 
%>

