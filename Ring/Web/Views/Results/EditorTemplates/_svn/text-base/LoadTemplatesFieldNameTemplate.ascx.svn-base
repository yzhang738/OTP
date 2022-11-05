<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ResultsLoadTemplateDownloadTemplateMappingsGridViewModel>" %>

<script type="text/javascript">
    function onFieldNameComboLoad()
    {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onFieldNameComboClosed(e)
    {
        var selectedValue = $('#FieldName').data('tDropDownList').value();

        if (selectedValue.indexOf("(Detail)") != -1)
        {
            $('#TeamMember').val('1').attr('disabled', 'disabled');
        }
        else
        {
            $('#TeamMember').removeAttr('disabled');
        }

        var dropDownList = $('#ColumnFormat').data('tDropDownList');
        dropDownList.text('');
        dropDownList.reload();
    }

    function onFieldNameComboBound()
    {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem)
        {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow)
            {
                editor.value(dataItem.FieldName);
                editor.disable();
            }
        }

        onFieldNameComboClosed();
    }

    function onFieldNameComboBinding(e)
    {
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        var curval = "";

        if (dataItem)
        {
            curval = dataItem.FieldName;
        }

        var downloadTemplateId = $("#downloadTemplateId").val();
        e.data = $.extend({}, e.data, { downloadTemplateId: downloadTemplateId, currentFieldName: curval });
    }
</script>

<%=         
    Html.Telerik().DropDownList()
        .Name("FieldName")
        .DataBinding(binding => binding.Ajax().Select("TemplateFieldNameAjax", "Shared"))
        .HtmlAttributes(new { style = "font-size:8pt" })
        .ClientEvents(events => events
                .OnDataBound("onFieldNameComboBound")
                .OnDataBinding("onFieldNameComboBinding")
                .OnClose("onFieldNameComboClosed")
                .OnLoad("onFieldNameComboLoad"))
%>
