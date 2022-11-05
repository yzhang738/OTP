<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

        <%
            Html.Telerik().Window()
            .Name("CreateActionItemWindow")
            .Title(Shared.SharedStrings.ActionItemEditor_CreateActionItemWindow_Title)
            .Content(() =>
                { %>


                    <% using (Html.BeginForm("CreateActionItem", "Shared"))
                    { %>

                        <div class="groupbox_600" style="margin:0px 0px 0px 0px">
                            <div class="groupbox_title_background gradientHToWhite">
                                <div class="groupbox_title"><%: Shared.SharedStrings.ActionItemEditor_ActionItem %></div>
                            </div>
                            <div class="groupbox_content">


                                <div style="float:left; width:98%; margin:0px 0px 15px 0px;">
                                    <div style="float:left;width:40%;text-align:right;"><%: Shared.SharedStrings.ActionItemEditor_Description %></div>
                                    <div style="float:right;width:50%;text-align:left;"><%: Html.TextBox("Description", null, new { style = "width:90%" })%></div> 
                                </div>

                                <div style="float:left; width:98%; margin:0px 0px 15px 0px;">
                                    <div style="float:left;width:40%;text-align:right;"><%: Shared.SharedStrings.ActionItemEditor_DueDate %></div>
                                    <div style="float:right;width:50%;text-align:left;">

                                        <%= Html.Telerik().DatePicker()
                                                .Name("DueDate")
                                                .Min(DateTime.Now)
                                                .Max(DateTime.Now.AddYears(1))
                                                .Value(DateTime.Now.AddDays(7))
                                                .ShowButton(true)
                                        %>

                                    </div> 
                                </div>

                                <div style="float:left; width:98%; margin:0px 0px 15px 0px;">
                                    <div style="float:left;width:40%;text-align:right;"><%: Shared.SharedStrings.ActionItemEditor_Priority %></div>
                                    <div style="float:right;width:50%;text-align:left;"><%= Html.DropDownList("Priority", new SelectList((IEnumerable)ViewData["actionPriorities"], "Id", "Name"))%></div> 
                                </div>

                                <div style="float:left; width:98%; margin:0px 0px 15px 0px;">
                                    <div style="float:left;width:40%;text-align:right;"><%: Shared.SharedStrings.ActionItemEditor_Comments %></div>
                                    <div style="float:right;width:50%;text-align:left;">
                                    
                                    <%--
                                    <%: Html.TextArea("fieldComments")  %>
                                    --%>
                                    <% Html.Telerik().Editor()
                                            .Name("Comments")
                                            .Tools(t => t.Clear())
                                            .HtmlAttributes(new { style = "height:300px" })
                                            .Render();
                           
                                    %>
                                    </div> 
                                </div>

                                <input type="hidden" name="actionName" value="" />
                                <input type="hidden" name="controllerName" value="" />
                                <input type="hidden" name="typeId" value="" />
                                <input type="hidden" name="referenceId" value="" />
                                <input type="hidden" name="organizationId" value="" />
                                <input type="hidden" name="sportId" value="" />
                            </div>
                            <img alt="" src="../../Content/Images/sidenav_btm.gif" width="600" height="9" /> 
                        </div> 
                        <div class="clear"></div>
                        <div style="float:right; text-align:right; margin: 5px 10px 0px 0px" >
                            <input type="submit" value="<%: Shared.SharedStrings.Button_Save %>" />
                        </div>

                          <%
                    } %>



             <% })
            .Draggable(true)
            .Scrollable(false)
            .Resizable()
            .Buttons(b => b.Close())
            .Width(605)
            .Modal(true)
            .Visible(false)
            .Render();
                        
        %>

<script type="text/javascript">

    function openEvaluationActionItemWindow(actionName, controllerName, typeId, referenceId, sportId) {

        $("#CreateActionItemWindow")
                .find('input[name=actionName]').val(actionName).end()
                .find('input[name=controllerName]').val(controllerName).end()
                .find('input[name=typeId]').val(typeId).end()
                .find('input[name=referenceId]').val(referenceId).end()
                .find('input[name=sportId]').val(sportId).end()
                .data("tWindow").center().open();
    }

    function openActionItemWindow(actionName, controllerName, typeId, referenceId, organizationId, sportId) {

        $("#CreateActionItemWindow")
                .find('input[name=actionName]').val(actionName).end()
                .find('input[name=controllerName]').val(controllerName).end()
                .find('input[name=typeId]').val(typeId).end()
                .find('input[name=referenceId]').val(referenceId).end()
                .find('input[name=organizationId]').val(organizationId).end()
                .find('input[name=sportId]').val(sportId).end()
                .data("tWindow").center().open();
    }
    
</script>

