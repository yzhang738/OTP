<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ResultsLoadTemplateDownloadTemplateMappingsGridViewModel>" %>

<script type="text/javascript">
    function onSourceColumnComboLoad()
    {
//        var combo = $(this).data('tDropDownList');
//        combo.reload();
    }



</script>

<%=         
    Html.Telerik().DropDownList()
        .Name("SourceColumn")
        .DataBinding(binding => binding.Ajax().Select("SourceColumnLetterAjax", "Shared"))
        .HtmlAttributes(new { style = "font-size:8pt" })
        .ClientEvents(events => events
            .OnDataBound("onSourceColumnComboBound")
            .OnLoad("onSourceColumnComboLoad")
            .OnChange("onSourceColumnComboChange")
            .OnDataBinding("onSourceColumnComboBinding")
                )
%>
