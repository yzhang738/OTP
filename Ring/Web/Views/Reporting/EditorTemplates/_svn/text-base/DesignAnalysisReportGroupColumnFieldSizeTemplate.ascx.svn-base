<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<script type="text/javascript">
    function onFieldSizeComboLoad()
    {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onFieldSizeComboBound()
    {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem)
        {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow)
            {
                editor.value(dataItem.FieldSize);
            }
        }
    }
</script>

<%= 
    Html.Telerik().DropDownList()
        .Name("FieldSize")
        .HtmlAttributes(new
        {
            style = "width:110px;font-size:8pt;"
        })
            .DataBinding(binding => binding.Ajax().Select("DecodeListAjax", "Shared",
                        new { decodeCategoryId = Constant.DecodeCategoryId.ReportFieldSize } ))
        .ClientEvents(e => e
            .OnLoad("onFieldSizeComboLoad")
            .OnDataBound("onFieldSizeComboBound"))
%>
