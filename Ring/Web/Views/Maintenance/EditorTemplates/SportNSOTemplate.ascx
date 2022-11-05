﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MaintenanceSportsGridSportViewModel>" %>
<%= Html.DropDownList(null, new SelectList((IEnumerable)ViewData["nsos"], "Id", "Description"))%>
<script type="text/javascript">
    // find the dropdown first
    var $dropdown = $('#NSO');
//    $('<option/>', { text: '<none>', value: -1 })
//          .prependTo($dropdown);
    // find the table row (tr) which is being edited
    var $tr = $dropdown.closest('tr:has(form)');

    // get the grid client object
    var grid = $tr.closest('.t-grid').data('tGrid');

    // get the data item bound to this table row
    var dataItem = grid.dataItem($tr);
    // set the value of the dropdown to select the proper item
    //$dropdown.val(dataItem.Employee ? dataItem.Employee.EmployeeID : -1);
    $dropdown.val(dataItem.NSO.Id);
</script>