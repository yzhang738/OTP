<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EvaluationsQuestionsEvaluationVersionGridViewModel>" %>

<script type="text/javascript">
    function onQuestionSportComboLoad()
    {
        var combo = $(this).data('tDropDownList');
        combo.reload();
    }

    function onQuestionSportComboBound()
    {
        var editor = $(this).data('tDropDownList');
        var tr = $(this).closest('tr:has(form)');
        var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

        if (dataItem)
        {
            var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
            if (!isNewRow)
            {
                editor.value(dataItem.SportId);
            }
        }
    }

</script>

<%=         
    Html.Telerik().DropDownList()
        .Name("SportId")
        .DataBinding(binding => binding.Ajax().Select("EvaluationQuestionSportAjax", "Shared"))
        .HtmlAttributes(new { style = "font-size:8pt;width:1000px" })
        .ClientEvents(events => events
            .OnLoad("onQuestionSportComboLoad")
            .OnDataBound("onQuestionSportComboBound"))
%>
