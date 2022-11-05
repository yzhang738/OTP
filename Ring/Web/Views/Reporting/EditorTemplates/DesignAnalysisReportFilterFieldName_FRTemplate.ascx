<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<script type="text/javascript">
    function onFilterFieldName_FRBinding(e)
    {
        var reportId = $("#reportId").val();
        e.data = $.extend({}, e.data, { reportId: reportId, includeNull: true });
    }

    function onFilterFieldName_FRLoad()
    {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onFilterFieldName_FRBound()
    {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem)
        {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow)
            {
                editor.value(dataItem.FilterFieldName_FR);
            }
        }
    }
</script>

<%= 
    Html.Telerik().DropDownList()
        .Name("FilterFieldName_FR")
        .HtmlAttributes(new
        {
            style = "width:110px;font-size:8pt;"
        })
            .DataBinding(binding => binding.Ajax().Select("ReportViewFieldsAjax", "Shared"))
        .ClientEvents(e => e
            .OnLoad("onFilterFieldName_FRLoad")
            .OnDataBinding("onFilterFieldName_FRBinding")
            .OnDataBound("onFilterFieldName_FRBound"))
%>
