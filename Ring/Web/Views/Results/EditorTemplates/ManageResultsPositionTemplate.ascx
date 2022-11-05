<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<int>" %>

<%= Html.Telerik().IntegerTextBox()
        .Name(ViewData.TemplateInfo.GetFullHtmlFieldName(string.Empty))
        .InputHtmlAttributes(new { style = "width:100%" })
        .Spinners(false)
        .MinValue(0)
        .MaxValue(int.MaxValue)
        .Value(Model) 
%>
