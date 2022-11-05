<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MaintenanceAthletesAthleteGridViewModel>" %>

<script type="text/javascript">
    function onGenderComboLoad()
    {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onGenderComboBound()
    {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem)
        {
            editor.value(dataItem.GenderName);
        }
    }
</script>

<%=         
    Html.Telerik().DropDownList()
        .Name("GenderName")
        .DataBinding(binding => binding.Ajax().Select("GenderAjax", "Shared", new { includeNull = true }))
        .HtmlAttributes(new { style = "font-size:8pt;width:75px" })
        .ClientEvents(events => events
            .OnLoad("onGenderComboLoad")
            .OnDataBound("onGenderComboBound"))
%>
