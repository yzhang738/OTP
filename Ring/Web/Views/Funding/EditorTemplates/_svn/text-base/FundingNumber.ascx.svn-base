<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<decimal?>" %>

<%= Html.Telerik().NumericTextBox<decimal>()
        .Name(ViewData.TemplateInfo.GetFullHtmlFieldName(string.Empty))
        .InputHtmlAttributes(new { style = "font-size:9pt;width:95%;text-align:right;" })
        .Spinners(false)
        .DecimalDigits(0)
        .MinValue(0)
        .MaxValue(decimal.MaxValue)
        .Value(Model ?? 0m)
%>
