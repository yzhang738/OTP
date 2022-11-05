<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EvaluationsQuestionsEvaluationVersionGridViewModel>" %>

<script type="text/javascript">
    function onVersionNameFrLoad() {
        var combo = $(this).data('tComboBox');
        combo.reload();
    }

    function onVersionNameFrBinding(e) {
        // Get the versionId combobox's value
        var versionId = $(e.currentTarget).closest('.t-edit-form tr td').eq(0).prev().prev().find('input,select').val();
        e.data = $.extend({}, e.data, { versionID: versionId });
    }

    var firstFRBound = true;

    function onVersionNameFrComboBound() {
        if (firstFRBound) {
            firstFRBound = false;

            var editor = $(this).data('tComboBox');
            var tr = $(this).closest('tr:has(form)');
            var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

            if (dataItem) {
                var isNewRow = ($(this).closest('.t-grid-new-row').length == 1);
                if (!isNewRow) {
                    editor.value(dataItem.VersionId);
                }
            }
        }
    }

</script>

<%=         
    Html.Telerik().ComboBox()
        .Name("VersionNameFr")
        .DataBinding(binding => binding.Ajax().Select("VersionNameFRAjax", "Shared"))
        .HtmlAttributes(new { style = "font-size:8pt;width:210px" })
        .ClientEvents(events => events
                .OnLoad("onVersionNameFrLoad")
                //.OnDataBinding("onVersionNameFrBinding")
                .OnDataBound("onVersionNameFrComboBound"))
%>
