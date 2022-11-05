<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<script type="text/javascript">
    function onReportCategoryIdComboLoad()
    {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onReportCategoryIdComboBound()
    {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem)
        {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow)
            {
                editor.value(dataItem.ReportCategoryId);
            }
        }
    }
</script>

<%= 
    Html.Telerik().DropDownList()
        .Name("ReportCategoryId")
        .HtmlAttributes(new
        {
            style = "width:110px;font-size:8pt;"
        })
        .DataBinding(binding => binding.Ajax().Select("ReportCategoryAjax", "Shared"))
        .ClientEvents(e => e
                .OnLoad("onReportCategoryIdComboLoad")
                .OnDataBound("onReportCategoryIdComboBound"))
%>
