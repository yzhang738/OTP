<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<bool?>" %>
<%: Html.CheckBox(null, Model.HasValue ? Model.Value : false)%>
