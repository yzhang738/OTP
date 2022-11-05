<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ResultsLoadResultsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.RenderPartial("~/Views/Shared/JavaScriptCheck.ascx"); %>
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="TitleContent" runat="server">
    <%: Shared.ResultsStrings.LoadResults_Title %> - <%: Model.Competition.Name %>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="MenuContent" runat="server">
    <% Html.RenderPartial("~/Views/Shared/NavMenu.ascx", Model.Menu); %>
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="InfoBar" runat="server">
    <% Html.RenderPartial("~/Views/Shared/LanguageSelector.ascx", Model.CurrentLanguageCode); %> &nbsp;|&nbsp;
    <% Html.RenderPartial("~/Views/Shared/CurrentUserName.ascx", Model.CurrentUserName); %>
</asp:Content>

<asp:Content ID="Content9" ContentPlaceHolderID="HeaderImage" runat="server">
    <%= RingHTMLHelper.GetRandomHeaderImage() %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <% if (Model.ErrorGridViewModel != null && Model.ErrorGridViewModel.Count == 0)
       { %>

            <div class="groupbox_600" style="margin-top: 20px">
                <div class="groupbox_title_background gradientHToWhite">
                    <div class="groupbox_title">
                        <div style="display:inline-block; margin-top:6px"><%: Shared.ResultsStrings.LoadResults_Step5Title %></div>
                    </div>
                </div>
                <div class="groupbox_content">
                    <div style="float:left; margin: 5px 22px 5px 0px">
                        <h3><%: Shared.ResultsStrings.LoadResults_ProcessingSuccesful %></h3><br />
                        <%: Html.ActionLink(Shared.ResultsStrings.ManageResults_Title, "ManageResults", new { id = Model.Competition.Id } ) %> 
                    </div>
                </div>
                <img src="../../Content/Images/sidenav_btm.gif" width="600" height="9" /> 
            </div> 
            <div class="clear"></div>

       <%}
       else if (Model.ErrorGridViewModel != null && Model.ErrorGridViewModel.Count > 0)
       { %>

       
            <div class="groupbox_1000" style="margin:0px 0px 0px 0px">
                <div class="groupbox_title_background gradientHToWhite">
                    <div class="groupbox_title" >
                        <div style="display:inline-block; margin-top:6px"><%: Shared.ResultsStrings.LoadResults_ErrorsOccured %></div>
                    </div>
                </div>
                <div class="groupbox_content">
                    <div style="float: left;">
                        <h3>
                            <%= string.Format(Shared.ResultsStrings.LoadResults_ErrorsInstructions,
                                    "<a href='" + ResolveUrl("~/Results/LoadResults/" + Model.Competition.Id.ToString()) + "'>",
                                    "</a>") %>
                        </h3>
                    </div>
                    <div class="clear"></div>
                    <% Html.Telerik().Grid(Model.ErrorGridViewModel)
                        .Name("ErrorsGrid")
                        .TableHtmlAttributes(new { style = "font-size:8pt" })
                        .HtmlAttributes(new { style = "height:320px" })
                        .Scrollable(scrolling => scrolling.Height(281))
                        .Resizable(resize => resize.Columns(true))
                        .Footer(false)
                        .ClientEvents(events => events.OnError("grid_OnError"))
                        .Columns(columns =>
                        {
                            columns.Bound(a => a.SourceRow)
                                .Width(70);
                            columns.Bound(a => a.SourceColumn)
                                .Width(70);
                            columns.Bound(a => a.SourceColumnName)
                                .Width(100);
                            columns.Bound(a => a.DataType)
                                .Width(100);
                            columns.Bound(a => a.FieldLength)
                                .Width(95);
                            columns.Bound(a => a.DownloadValue)
                                .Width(125);
                            columns.Bound(a => a.ErrorMessage);
                        }).Render();
                    %>

                </div>
                <img src="../../Content/Images/sidenav_btm.gif" width="1000" height="9" /> 
            </div> 

       <% }
       else
       { %>

                   <%
        if (!string.IsNullOrEmpty(Model.Errors))
        { %>
                    <p style="color:Red"><%= Model.Errors%></p>
            <%  } %>

       <%--STEP 1--%>

    <div class="groupbox_600" style="margin-top: 0px">
        <div class="groupbox_title_background gradientHToWhite">
            <div class="groupbox_title">
                <div style="margin-top:6px;display:inline-block">
                   <%: Shared.ResultsStrings.LoadResults_Step1Title %>
                </div>
            </div>
        </div>
        <div class="groupbox_content">
            <div style="float:left; margin:5px 5px 5px 0px; width:85px">
                <%: Shared.ResultsStrings.LoadResults_Template %>:
            </div>
            <div style="float:left; margin: 5px 22px 5px 0px">
                <%= Html.Telerik().DropDownList()
                    .Name("TemplateList")
                    .HtmlAttributes(new { style = "width:400px;font-size:12px;margin-left:5px" })
                    .BindTo(new SelectList(Model.DownloadTemplateList, "Id", "Description"))
                    .ClientEvents(events => events
                        .OnChange("onTemplateChange"))
                %>
            </div>

            <div style="float:left;" id="TemplateDownloadTarget"></div>

        </div>
        <img src="../../Content/Images/sidenav_btm.gif" width="600" height="9" /> 
    </div> 
    <div class="clear"></div>
        
        <%--STEP 2--%>

    <div class="groupbox_600" style="margin-top: 20px">
        <div class="groupbox_title_background gradientHToWhite">
            <div class="groupbox_title">
                <div style="display:inline-block; margin-top:6px">
                    <%: Shared.ResultsStrings.LoadResults_Step2Title %>
                </div>
            </div>
        </div>
        <div class="groupbox_content">
            <div id="TemplateDescriptionTarget"></div><br />
            <div class="clear"></div>
            <div id="TemplateURLTarget"></div>

        </div>
        <img src="../../Content/Images/sidenav_btm.gif" width="600" height="9" /> 
    </div> 
    <div class="clear"></div>

    <%--STEP 3--%>

        <div class="groupbox_600" style="margin-top: 20px">
        <div class="groupbox_title_background gradientHToWhite">
            <div class="groupbox_title">
                <div style="display:inline-block; margin-top:6px">
                    <%: Shared.ResultsStrings.LoadResults_Step3Title %>
                </div>
            </div>
        </div>
        <div class="groupbox_content">
            <div style="float:left; margin:5px 5px 5px 0px; width:85px">
                <%: Shared.ResultsStrings.LoadResults_EventLabel %>:
            </div>
            <div style="float:left; margin: 5px 22px 5px 0px">
                <%= Html.Telerik().DropDownList()
                    .Name("EventList")
                    .BindTo(new SelectList(Model.EventList, "Id", "Description"))
                    .HtmlAttributes(new { style = "width:400px;font-size:12px;margin-left:5px" })
                %>
            </div>

            <div style="float:left; margin:5px 5px 5px 0px; width:85px">
                <%: Shared.ResultsStrings.LoadResults_EventDateLabel %>:
            </div>
            <div style="float:left; margin: 5px 22px 5px 0px">
                <%= Html.Telerik().DatePicker()
                    .Name("EventDate")
                    .Value(Model.Competition.Date)
                    .HtmlAttributes(new { style = "width:200px;font-size:12px;margin-left:5px" })
                %>
            </div>
            
            <div class="clear"></div>

            <div style="float:left; margin:5px 5px 5px 0px; width:85px">
                <%: Shared.ResultsStrings.LoadResults_DescriptionLabel %>:
            </div>
            <div style="float:left; margin: 5px 22px 5px 0px">
                <%= Html.TextBox("EventDescription", "ALL", new { style = "width:395px;font-size:12px;margin-left:5px" }) %>
            </div>

            <div style="float:left; margin:5px 5px 5px 0px; width:85px">
                 <%: Shared.ResultsStrings.LoadResults_BenchmarkLabel %>:
            </div>
            <div style="float:left; margin: 5px 22px 5px 0px">
                <%= Html.Telerik().DropDownList()
                    .Name("BenchmarkType")
                    .BindTo(new SelectList(Model.BenchmarkList, "Id", "Description"))
                    .SelectedIndex(Model.BenchmarkSelectedIndex)
                    .HtmlAttributes(new { style = "width:400px;font-size:12px;margin-left:5px" })
                %>
            </div>
        </div>
        <img src="../../Content/Images/sidenav_btm.gif" width="600" height="9" /> 
    </div> 
    <div class="clear"></div>

    <%--STEP 4--%>

    <div class="groupbox_600" style="margin-top: 20px">
        <div class="groupbox_title_background gradientHToWhite">
            <div class="groupbox_title">
                <div style="display:inline-block; margin-top:6px"><%: Shared.ResultsStrings.LoadResults_Step4Title %></div>
            </div>
        </div>
        <div class="groupbox_content">
            <div id="inputFormTarget"></div>
        </div>
        <img src="../../Content/Images/sidenav_btm.gif" width="600" height="9" /> 
    </div> 
    <div class="clear"></div>

    <% } %> 

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContent" runat="server">
    <script type="text/javascript">
        function onTemplateChange()
        {
            var templateId = $("#TemplateList").data('tDropDownList').value();

            if (templateId)
            {
                var competitionId = <%: Model.Competition.Id %>;
                var eventDateValue = $('#EventDate').data("tDatePicker").value();
                var eventDate = $.telerik.formatString('{0:d}', eventDateValue);
                var eventDescription = $('#EventDescription').val();
                var sportId = $("#EventList").data('tDropDownList').value();
                var benchmarkType = $("#BenchmarkType").data('tDropDownList').value();

                //Force ajax to retrieve new data each call
                var d = new Date();
                var refreshSeed = d.getTime();

                $.getJSON("/Results/GetTemplateDescription", { downloadTemplateId: templateId, refresh: refreshSeed }, function (description)
                {
                    $("#TemplateDescriptionTarget").html(description);
                });

                $.getJSON("/Results/GetTemplateURL", { downloadTemplateId: templateId, refresh: refreshSeed }, function (url)
                {
                    $("#TemplateURLTarget").html("<a href='" + url + "' target='_blank'>" + url + "</a>");
                });

                $("#TemplateDownloadTarget").html("<a href='/Results/GetTemplateFile/" + templateId + "?competitionId=" + competitionId + "'><%: Shared.ResultsStrings.LoadResults_DownloadTemplate %></a>");
                $("#inputFormTarget").html(
                    "<form id='upload_form' name='upload_form' action='/Results/Upload?competitionId=" + competitionId + 
                    "&amp;downloadTemplateId=" + templateId +
                    "&amp;benchmarkTypeId=" + benchmarkType +
                    "&amp;eventDate=" + eventDate +
                    "&amp;eventDescription=" + eventDescription +
                    "&amp;sportId=" + sportId +
                    "' enctype='multipart/form-data' method='post' onSubmit='showLoader();'>" +
                    "<p><input type='file' id='fileUpload' name='fileUpload' size='75' value='test' /></p>" +
                    "<p align='right'><input type='button' name='submitform' value='<%: Shared.ResultsStrings.LoadResults_ButtonUploadFile %>' onClick='submitForm();' /></p>" +
                    "</form>");
            }
            else
            {
                $("#inputFormTarget").html('');
                $("#TemplateDownloadTarget").html('');
                $("#TemplateURLTarget").html('');
                $("#TemplateDescriptionTarget").html('');
            }
        }

        function submitForm()
        {
            var templateId = $("#TemplateList").data('tDropDownList').value();

            if (templateId && $('#fileUpload').val())
            {
                var competitionId = <%: Model.Competition.Id %>;
                var eventDateValue = $('#EventDate').data("tDatePicker").value();
                var eventDate = $.telerik.formatString('{0:d}', eventDateValue);
                var eventDescription = $('#EventDescription').val();
                var sportId = $("#EventList").data('tDropDownList').value();
                var benchmarkType = $("#BenchmarkType").data('tDropDownList').value();

                $("#upload_form").attr("action", "/Results/Upload?competitionId=" + competitionId + "&downloadTemplateId=" + templateId + "&benchmarkTypeId=" + benchmarkType + "&eventDate=" + eventDate + "&eventDescription=" + eventDescription + "&sportId=" + sportId);
                document.upload_form.submit();
                ShowPleaseWait();
            }
        }
    </script>
</asp:Content>
