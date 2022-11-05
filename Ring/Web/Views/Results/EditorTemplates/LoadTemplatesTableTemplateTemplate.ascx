<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ResultsLoadTemplateDownloadTemplateGridViewModel>" %>

<script type="text/javascript">
    function onTableTemplateComboLoad()
    {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onTableTemplateComboBound()
    {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem)
        {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow)
            {
                editor.value(dataItem.TableTemplateId);
                editor.disable();
            }
        }
    }
</script>

<%=         
    Html.Telerik().DropDownList()
        .Name("TableTemplateId")
        .DataBinding(binding => binding.Ajax().Select("TableTemplateAjax", "Shared", new { resultsOnly = true }))
        .HtmlAttributes(new { style = "font-size:8pt;width:90px" })
        .ClientEvents(events => events
            .OnDataBound("onTableTemplateComboBound")
            .OnLoad("onTableTemplateComboLoad"))
%>
