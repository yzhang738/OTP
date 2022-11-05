<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>

<script type="text/javascript">
    function onResourceQualificationLoad()
    {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onResourceQualificationBound()
    {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem) {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow) editor.value(dataItem.QualificationTypeId);
        }
    }
</script>

<%=         
    Html.Telerik().DropDownList()
        .Name("QualificationTypeId")
        .DataBinding(binding => binding.Ajax().Select("ResourceQualificationTypeAjax", "Shared"))
        .HtmlAttributes(new { style = "font-size:8pt;" })
        .ClientEvents(events => events
            .OnLoad("onResourceQualificationLoad")
            .OnDataBound("onResourceQualificationBound"))
%>
