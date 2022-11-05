<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<script type="text/javascript">
    function onFilterFieldName_ENBinding(e)
    {
        var reportId = $("#reportId").val();
        e.data = $.extend({}, e.data, { reportId: reportId, includeNull: false });
    }

    function onFilterFieldName_ENLoad()
    {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onFilterFieldName_ENBound()
    {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem)
        {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow)
            {
                editor.value(dataItem.FilterFieldName_EN);
            }
        }
    }
</script>

<%= 
    Html.Telerik().DropDownList()
        .Name("FilterFieldName_EN")
        .HtmlAttributes(new
        {
            style = "width:110px;font-size:8pt;"
        })
            .DataBinding(binding => binding.Ajax().Select("ReportViewFieldsAjax", "Shared"))
        .ClientEvents(e => e
            .OnLoad("onFilterFieldName_ENLoad")
            .OnDataBinding("onFilterFieldName_ENBinding")
            .OnDataBound("onFilterFieldName_ENBound"))
%>
