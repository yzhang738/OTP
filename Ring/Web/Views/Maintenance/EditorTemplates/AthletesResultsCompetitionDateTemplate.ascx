<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MaintenanceAthletesResultsViewModel>"  %>

<script type="text/javascript">
    function onLoad(e) {
        var datestring = '<%= ViewData["AthleteResultsCompetitionDate"] %>';
        
        var datepicker = $(this).data('tDatePicker');
        datepicker.value(datestring);
    }

    function onChange() {
        var datepicker = $(this).data('tDatePicker');
        var datestring = datepicker.inputValue.toString();

        $('#EventDate').data('tDatePicker').value(datestring);
    }    
</script>

<%=         
    Html.Telerik().DatePicker()
        .Name("CompetitionDate")
        .HtmlAttributes(new { style = "font-size:8pt;" })
        .ShowButton(true)
        .Value("2012/07/27")
        .Format("yyyy/MM/dd")
        .ClientEvents(events => events.OnClose("onChange")
                                      .OnLoad("onLoad")
        ) 
%>

