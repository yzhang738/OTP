<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<script type="text/javascript">
    function onAffiliateTypeLoad()
    {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onAffiliateTypeBound()
    {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem) {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow) editor.value(dataItem.AffiliateTypeId);
        }
    }
</script>

<%=         
    Html.Telerik().DropDownList()
        .Name("AffiliateTypeId")
        .DataBinding(binding => binding.Ajax().Select("AffiliateTypesAjax", "Shared"))
        .HtmlAttributes(new { style = "width:270px;font-size:8pt;" })
        .ClientEvents(events => events
            .OnLoad("onAffiliateTypeLoad")
            .OnDataBound("onAffiliateTypeBound"))
%>
