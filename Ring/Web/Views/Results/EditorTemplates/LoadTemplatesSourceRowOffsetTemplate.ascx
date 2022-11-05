<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<int>" %>

<script type="text/javascript">
    function onSourceRowOffsetComboClosed(e)
    {
        var columnLetterDropDownList = $('#SourceColumn').data('tDropDownList');
        columnLetterDropDownList.text('');
        columnLetterDropDownList.reload();
    }

    function onSourceRowOffsetComboLoad()
    {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onSourceRowOffsetComboBound()
    {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem)
        {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow)
            {
                editor.value(dataItem.SourceRowOffset);
            }
        }

        onSourceRowOffsetComboClosed();
    }
</script>

<%=         
    Html.Telerik().DropDownList()
        .Name("SourceRowOffset")
        .DataBinding(binding => binding.Ajax().Select("SourceRowNumberAjax", "Shared"))
        .HtmlAttributes(new { style = "font-size:8pt" })
        .ClientEvents(events => events
                .OnDataBound("onSourceRowOffsetComboBound")
                .OnLoad("onSourceRowOffsetComboLoad")
                .OnClose("onSourceRowOffsetComboClosed")
                )
%>
