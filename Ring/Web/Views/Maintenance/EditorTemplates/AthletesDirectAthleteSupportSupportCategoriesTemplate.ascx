<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>

<script type="text/javascript">
    function onDirectAthleteSupportComboBound()
    {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem) {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow) editor.value(dataItem.DirectAthleteSupportCode);
        }
    }

    function onDirectAthleteSupportComboDataBinding(e)
    {
        //Get the previous drop down list's value
        var directAthleteSupportTypeId = $(e.currentTarget).closest('.t-edit-form tr td').eq(0).prev().find('input,select').val();

        e.data = $.extend({}, e.data, { directAthleteSupportTypeId: directAthleteSupportTypeId });
    }
</script>

<%= 
    Html.Telerik().DropDownList()
        .Name("DirectAthleteSupportCode")
        .HtmlAttributes(new
        {
            style = "width:150px;font-size:8pt;"
        })
        .DataBinding(binding => binding.Ajax().Select("GetDirectAthleteSupportCategoriesByTypeAjax", "Shared"))
        .ClientEvents(events => events
            .OnDataBinding("onDirectAthleteSupportComboDataBinding")
            .OnDataBound("onDirectAthleteSupportComboBound"))
                                
%>
