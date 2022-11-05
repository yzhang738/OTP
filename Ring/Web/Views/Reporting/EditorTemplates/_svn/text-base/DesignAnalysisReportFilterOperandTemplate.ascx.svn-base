<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<script type="text/javascript">

    function onReportOperandLoad()
    {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onReportOperandBound()
    {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem)
        {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow)
            {
                editor.value(dataItem.ReportOperand);
            }
        }
    }
</script>

<%= 
    Html.Telerik().DropDownList()
        .Name("ReportOperand")
        .HtmlAttributes(new
        {
            style = "width:110px;font-size:8pt;"
        })
            .DataBinding(binding => binding.Ajax().Select("DecodeListAjax", "Shared",
                        new { decodeCategoryId = Constant.DecodeCategoryId.ReportOperand }))
        .ClientEvents(e => e
            .OnLoad("onReportOperandLoad")
            .OnDataBound("onReportOperandBound"))
%>
