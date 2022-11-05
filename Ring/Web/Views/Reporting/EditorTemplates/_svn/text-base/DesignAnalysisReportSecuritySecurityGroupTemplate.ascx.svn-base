<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<script type="text/javascript">
    function onSecurityGroupComboBinding(e)
    {
        var securityUserProfileId = $("#securityUserProfileId").val();
        var reportId = $("#reportId").val();
        e.data = $.extend({}, e.data, { reportId: reportId, securityUserProfileId: securityUserProfileId });
    }

    function onSecurityGroupComboLoad()
    {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onSecurityGroupComboBound()
    {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem)
        {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow)
            {
                editor.value(dataItem.SecurityGroupProfileID);
            }
        }
    }
</script>

<%= 
    Html.Telerik().DropDownList()
        .Name("SecurityGroupProfileID")
        .HtmlAttributes(new
        {
            style = "width:110px;font-size:8pt;"
        })
        .DataBinding(binding => binding.Ajax().Select("SecurityGroupProfileAjax", "Shared"))
        .ClientEvents(e => e
            .OnLoad("onSecurityGroupComboLoad")
            .OnDataBinding("onSecurityGroupComboBinding")
            .OnDataBound("onSecurityGroupComboBound"))
%>
