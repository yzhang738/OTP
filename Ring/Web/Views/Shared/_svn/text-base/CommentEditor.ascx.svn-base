<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<script type="text/javascript">

    function openCommentWindow(tableName, primaryKey, windowTitle, editableGrid, editableRow) {

        var editable = (editableGrid.toLowerCase() == 'true' && editableRow.toLowerCase() == 'true');

        if (windowTitle === "")
        {
            windowTitle = "<%: Shared.SharedStrings.Comments %>";
        }
        else
        {
            windowTitle = "<%: Shared.SharedStrings.Comments %> - " + windowTitle;
        }

        if (editable) 
        {
            $("#saveCommentsButton").removeAttr('disabled');
        }
        else
        {
            $("#saveCommentsButton").attr('disabled', 'disabled');
        }

        //Force ajax to retrieve new data each call
        var d = new Date();
        var refreshSeed = d.getTime();

        $.getJSON("/Shared/GetComments", { primaryKey: primaryKey, tableName: tableName, refresh: refreshSeed }, function (comments)
        {
            var cleanComments = unescape(comments);
            $("#CommentWindow").data("tWindow").title(windowTitle);

            $("#CommentWindow")
                    .find('input[name=primaryKey]').val(primaryKey).end()
                    .find('input[name=tableName]').val(tableName).end()
                    .data("tWindow").center().open();

            $("#editorWindowComments").data("tEditor").value(cleanComments);
        });
    }

    function updateComments() {

        var primaryKey = $("#CommentWindow")
                    .find('input[name=primaryKey]').val();

        var tableName = $("#CommentWindow")
                    .find('input[name=tableName]').val();

        var comments = $("#editorWindowComments").data("tEditor").value();
        var cleanComments = escape(comments);

        $.post("/Shared/UpdateComments", { tableName: tableName, primaryKey: primaryKey, comments: cleanComments }, null, "json");
        $("#CommentWindow").data("tWindow").close();
    }
</script>

        <%
            Html.Telerik().Window()
            .Name("CommentWindow")
            .Title("set by javascript")
            .Content(() =>
                { 
                        Html.Telerik().Editor()
                            .Name("editorWindowComments")
                            .Tools(t => t.Clear())
                            .Value("")
                            .HtmlAttributes(new { style = "height:300px" })
                            .Render();

                          %> 
                          <input type="hidden" name="primaryKey" value="" />
                          <input type="hidden" name="tableName" value="" />

                          <div style="text-align:right;margin: 5px 0px 0px 0px;">
                                <input id="saveCommentsButton" type="button" value="<%: Shared.SharedStrings.SaveComments %>" onclick="updateComments()" />
                          </div>

                <%})
            .Draggable(true)
            .Scrollable(false)
            .Resizable()
            .Buttons(b => b.Close())
            .Width(900) /* editor toolbar gets on one line */
            .Height(350) /* accomodates editor height */
            .Modal(true)
            .Visible(false) /* window not visible on initial load */
            .Render();
                        
        %>

        