<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EvaluationsQuestionsEvaluationVersionGridViewModel>" %>

<script type="text/javascript">
    function onVersionIdComboLoad() {
        var combo = $(this).data('tComboBox');        
        combo.reload();
    }

    function onVersionIdClosed() {
        var editor = $(this).data('tComboBox');
        var index = editor.selectedIndex;

//        var versionName = editor.data[index].Text;
//        $('#Name_EN').val(versionName);

        var versionId = editor.data[index].Value;

        $.ajax({
            type: 'POST',
            url: '/Shared/SetVersionNameEnAjax',
            dataType: 'json',
            data: { versionID: versionId },
            async: false,
            success: function (versionName) {
                $('#Name_EN').val(versionName);
            }
        });

        $.ajax({
            type: 'POST',
            url: '/Shared/SetVersionNameFrAjax',
            dataType: 'json',
            data: { versionID: versionId },
            async: false,
            success: function (versionName) {
                $('#Name_FR').val(versionName);
            }
        });

//        var versionNameEN = $('#VersionNameEn').data('tComboBox');
//        versionNameEN.text('');
//        versionNameEN.reload();
//        versionNameEN.select(index);

//        var versionNameFR = $('#VersionNameFr').data('tComboBox');
//        versionNameFR.text('');
//        versionNameFR.reload();
//        versionNameFR.select(index);

    }

    function onVersionIdComboBound() {
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

</script>

<%=         
    Html.Telerik().ComboBox()
        .Name("VersionId")
        .DataBinding(binding => binding.Ajax().Select("VersionIdAjax", "Shared"))
        .HtmlAttributes(new { style = "font-size:8pt;width:250px" })
        .ClientEvents(events => events
            .OnLoad("onVersionIdComboLoad")           
            .OnClose("onVersionIdClosed")
            .OnDataBound("onVersionIdComboBound"))
%>
