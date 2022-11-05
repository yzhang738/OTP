<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<decimal?>" %>

<%= Html.Telerik().NumericTextBox<decimal>()
        .Name(ViewData.TemplateInfo.GetFullHtmlFieldName(string.Empty))
        .InputHtmlAttributes(new { style = "width:100%" })
        .Spinners(false)
        .DecimalDigits(4)
        .MinValue(0)
        .MaxValue(decimal.MaxValue)
        .Value(Model)
%>
