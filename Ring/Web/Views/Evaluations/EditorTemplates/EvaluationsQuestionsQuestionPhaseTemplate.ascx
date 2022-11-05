<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EvaluationsQuestionsEvaluationVersionGridViewModel>" %>

<script type="text/javascript">
    function onQuestionPhaseComboLoad()
    {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onQuestionPhaseComboBound()
    {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);
        var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);

        if (isNewRow)
        {
            editor.disable();
        }
        else if (dataItem)
        {
            editor.value(dataItem.QuestionPhase);
            if (dataItem.SortOrder == 0)
            {
                editor.disable();
            }
        }
    }

    function onQuestionPhaseComboBinding(e)
    {
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);
        var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
        var currentVal = "";

        if (!isNewRow && dataItem)
        {
            currentVal = dataItem.QuestionPhase;
        }

        e.data = $.extend({}, e.data, { questionPhase: currentVal });
    }

</script>

<%=         
    Html.Telerik().DropDownList()
        .Name("QuestionPhase")
        .DataBinding(binding => binding.Ajax().Select("EvaluationVersionQuestionPhasesAjax", "Shared"))
        .HtmlAttributes(new { style = "font-size:8pt;width:250px" })
        .ClientEvents(events => events
            .OnLoad("onQuestionPhaseComboLoad")
            .OnDataBound("onQuestionPhaseComboBound")
            .OnDataBinding("onQuestionPhaseComboBinding"))
%>
