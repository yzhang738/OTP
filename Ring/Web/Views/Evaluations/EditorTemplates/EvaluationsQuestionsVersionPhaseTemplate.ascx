<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EvaluationsQuestionsEvaluationVersionGridViewModel>" %>

<script type="text/javascript">
    function onVersionPhaseComboLoad()
    {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onVersionPhaseComboBound() 
    {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem) {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow) {
                editor.value(dataItem.VersionPhase);
            }
            else {
                editor.disable();
            }
        }
    }

    function onVersionPhaseDataBinding(e)
    {
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        var currentVal = "";

        if (dataItem)
        {
            currentVal = dataItem.VersionPhase;
        }

        e.data = $.extend({}, e.data, { versionPhase: currentVal });
    }

</script>

<%=         
    Html.Telerik().DropDownList()
        .Name("VersionPhase")
        .DataBinding(binding => binding.Ajax().Select("EvaluationVersionPhasesAjax", "Shared"))
        .HtmlAttributes(new { style = "font-size:8pt;width:250px" })
        .ClientEvents(events => events
            .OnLoad("onVersionPhaseComboLoad")
            .OnDataBound("onVersionPhaseComboBound")
            //.OnDataBinding("onVersionPhaseDataBinding")
          )
%>
