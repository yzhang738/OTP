<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<script type="text/javascript">
    function OTPAlert(text, windowTitle)
    {
        if (!windowTitle)
        {
            windowTitle = "<%: Shared.SharedStrings.Important %>";
        }

        $("#otp_alert_text_target").html(text);
        $("#OTPAlert").data("tWindow").title(windowTitle);
        $("#OTPAlert").data("tWindow").center().open();
    }
</script>

        <%
            Html.Telerik().Window()
            .Name("OTPAlert")
            .Title("set by javascript")
            .Content(() =>
                {%>
                    <div id="otp_alert_text_target" style="width:100%; height:80%; float:left"></div>
                    <div style="float:right;margin: 5px 0px 0px 0px;">
                        <input style="width:75px" type="button" value="<%: Shared.SharedStrings.Button_OK %>" onclick="$('#OTPAlert').data('tWindow').close();" />
                    </div>
                <%})
            .Draggable(false)
            .Scrollable(true)
            .Buttons(b => b.Close())
            .Width(400)
            .Height(150)
            .Modal(true)
            .Visible(false) /* window not visible on initial load */
            .Render();
                        
        %>

        