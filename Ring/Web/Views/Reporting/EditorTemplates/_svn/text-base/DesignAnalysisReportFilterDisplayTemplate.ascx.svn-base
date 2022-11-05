<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<bool>" %>

<script type="text/javascript">

    var tr = $('#Display').closest('tr:has(form)');
    var dataItem = tr.closest('.t-grid').data('tGrid').dataItem(tr);

    SetDefaultEditable(!dataItem.Display);

    function onDisplayClicked()
    {
        var display = $('#Display').attr('checked');
        SetDefaultEditable(!display);
    }

    function SetDefaultEditable(editable)
    {
        if (editable)
        {
            $('#Default_EN').attr('disabled', '');
        }
        else
        {
            $('#Default_EN').attr('disabled', 'disabled').val('');
        }
    }

</script>

<input id='Display' name='Display' type='checkbox' onclick='onDisplayClicked();' <%: Model ? "checked='checked'" : "" %> />

