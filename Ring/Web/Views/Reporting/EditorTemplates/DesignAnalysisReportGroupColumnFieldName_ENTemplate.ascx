<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<script type="text/javascript">
    function onColumnFieldName_ENComboClosed(e)
    {
        var aggregationDropDownList = $('#ReportAggregation').data('tDropDownList');
        var formatStringDropDownList = $('#FormatString').data('tDropDownList');
        aggregationDropDownList.text('');
        aggregationDropDownList.reload();
        formatStringDropDownList.text('');
        formatStringDropDownList.reload();
    }

    function onColumnFieldName_ENLoad()
    {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onColumnFieldName_ENBinding(e)
    {
        var reportId = $("#reportId").val();
        e.data = $.extend({}, e.data, { reportId: reportId, includeNull: false });
    }

    function onColumnFieldName_ENBound()
    {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem)
        {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow)
            {
                editor.value(dataItem.ColumnFieldName_EN);
            }
        }

        onColumnFieldName_ENComboClosed();
    }
</script>

<%= 
    Html.Telerik().DropDownList()
        .Name("ColumnFieldName_EN")
        .HtmlAttributes(new
        {
            style = "width:110px;font-size:8pt;"
        })
            .DataBinding(binding => binding.Ajax().Select("ReportViewFieldsAjax", "Shared"))
        .ClientEvents(e => e
            .OnLoad("onColumnFieldName_ENLoad")
            .OnDataBinding("onColumnFieldName_ENBinding")
            .OnDataBound("onColumnFieldName_ENBound")
            .OnClose("onColumnFieldName_ENComboClosed"))
%>
