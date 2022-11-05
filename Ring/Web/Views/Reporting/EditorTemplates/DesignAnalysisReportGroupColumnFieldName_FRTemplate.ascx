<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<script type="text/javascript">
    function onColumnFieldName_FRBinding(e)
    {
        var reportId = $("#reportId").val();
        e.data = $.extend({}, e.data, { reportId: reportId, includeNull: true });
    }

    function onColumnFieldName_FRLoad()
    {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onColumnFieldName_FRBound()
    {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem)
        {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow)
            {
                editor.value(dataItem.ColumnFieldName_FR);
            }
        }
    }
</script>

<%= 
    Html.Telerik().DropDownList()
        .Name("ColumnFieldName_FR")
        .HtmlAttributes(new
        {
            style = "width:110px;font-size:8pt;"
        })
            .DataBinding(binding => binding.Ajax().Select("ReportViewFieldsAjax", "Shared"))
        .ClientEvents(e => e
            .OnLoad("onColumnFieldName_FRLoad")
            .OnDataBinding("onColumnFieldName_FRBinding")
            .OnDataBound("onColumnFieldName_FRBound"))
%>
