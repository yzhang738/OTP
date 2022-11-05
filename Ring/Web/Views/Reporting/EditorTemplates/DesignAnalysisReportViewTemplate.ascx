<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<script type="text/javascript">
    function onReportViewIdComboLoad()
    {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onReportViewIdComboBound()
    {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem)
        {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow)
            {
                editor.value(dataItem.ReportViewId);
            }
        }
    }
</script>

<%= 
    Html.Telerik().DropDownList()
        .Name("ReportViewId")
        .HtmlAttributes(new
        {
            style = "width:110px;font-size:8pt;"
        })
            .DataBinding(binding => binding.Ajax().Select("ReportViewListAjax", "Shared"))
        .ClientEvents(e => e
                .OnLoad("onReportViewIdComboLoad")
                .OnDataBound("onReportViewIdComboBound"))
%>
