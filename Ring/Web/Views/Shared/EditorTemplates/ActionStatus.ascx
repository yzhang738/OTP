﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<%= Html.DropDownList(null, new SelectList((IEnumerable)ViewData["actionStatus"], "Id", "Name"))%>