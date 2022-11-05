<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<script type="text/javascript">

    function PleaseWait_OnClose(e)
    {
        // cancelling the OnClose event
        e.preventDefault();
    }

    function ShowPleaseWait()
    {
        frame = window.frames["LoaderFrame"];
        if (frame.document.body)
        {
            frame.document.body.innerHTML = "<div style='text-align:center; margin-top: -15px'><img src='../../Content/Images/ajax-loader.gif' /></div>";
        }
        $("#PleaseWait").data("tWindow").center().open();
    }

    function CancelPleaseWait()
    {
        $("#PleaseWait").data("tWindow").close();
    }
</script>

<%
    Html.Telerik().Window()
    .Name("PleaseWait")
    .Title(Shared.ResultsStrings.LoadResults_PleaseWait)
    .Buttons(b =>{}) // clear close button
    .Content(() =>
        {%>
            <iframe scrolling="no" frameborder="no" style="width: 100%;height: 100%; overflow: hidden;" id="LoaderFrame" name="LoaderFrame" ></iframe>
        <%})
    .Draggable(false)
    .Scrollable(false)
    .ClientEvents(e => e.OnClose("PleaseWait_OnClose"))
    .Width(200)
    .Height(100)
    .Modal(true)
    .Visible(false) /* window not visible on initial load */
    .Render();
                        
%>

        