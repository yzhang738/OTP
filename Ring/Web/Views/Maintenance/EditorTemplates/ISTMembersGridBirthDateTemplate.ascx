<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MaintenanceAthletesAthleteGridViewModel>" %>

<%=         
    Html.Telerik().DatePicker()
        .Name("BirthDate")
        .HtmlAttributes(new { style = "font-size:8pt;" })
        .ShowButton(true)
        .Format("yyyy/MM/dd")
%>

