<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EvaluationsQuestionsEvaluationVersionGridViewModel>" %>

<script type="text/javascript">
    function onVersionNameEnComboLoad() {
        var combo = $(this).data('tComboBox');
        combo.reload();
    }

    function onVersionNameEnBinding(e) {
        // Get the versionId combobox's value
        var versionId = $(e.currentTarget).closest('.t-edit-form tr td').eq(0).prev().prev().find('input,select').val();
        e.data = $.extend({}, e.data, { versionID: versionId });
    }

    var firstENBound = true;

    function onVersionNameEnComboBound() {
        if (firstENBound) {
            firstENBound = false;

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
        .Name("VersionNameEn")
        .HtmlAttributes(new { style = "font-size:8pt;width:210px" })
        .DataBinding(binding => binding.Ajax().Select("VersionNameENAjax", "Shared"))
        .ClientEvents(events => events
                    .OnLoad("onVersionNameEnComboLoad")
                    //.OnDataBinding("onVersionNameEnBinding")
                    .OnDataBound("onVersionNameEnComboBound"))
%>
