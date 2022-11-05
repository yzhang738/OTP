<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MaintenanceAthletesViewModel>" %>

<asp:Content ID="Content7" ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.RenderPartial("~/Views/Shared/JavaScriptCheck.ascx"); %>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%: Shared.MaintenanceStrings.Athletes_PageTitle %>: <%: ViewData["sportName"] %> - <%: Shared.MaintenanceStrings.Athletes_ResourceTitle %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MenuContent" runat="server">
    <% Html.RenderPartial("~/Views/Shared/NavMenu.ascx", Model.Menu); %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="InfoBar" runat="server">
    <% Html.RenderPartial("~/Views/Shared/LanguageSelector.ascx", Model.CurrentLanguageCode); %> &nbsp;|&nbsp;
    <% Html.RenderPartial("~/Views/Shared/CurrentUserName.ascx", Model.CurrentUserName); %>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="YearSelectionContent" runat="server">
    <% Html.RenderPartial("~/Views/Shared/YearSelector.ascx", Model.CurrentOlympicYears); %>
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="HeaderImage" runat="server">
    <%= RingHTMLHelper.GetRandomHeaderImage() %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">

    <div id="container">
        <input type="hidden" id="resourceId" value="<%= ViewData["resourceId"] %>" />
        <input type="hidden" id="command" value="<%= ViewData["command"] %>" />
        <% Html.RenderPartial("~/Views/Shared/CommentEditor.ascx"); %>

        <% //TODO: Localize  %>
        <% Html.Telerik().Window()
            .Name("AssignSport")
            .Title(Shared.MaintenanceStrings.Maintenance_AssignSportTitle)
            .Content(() =>
                { %>
                    <div class="groupbox_600_athlete" style="margin:0px 0px 0px 0px">
                        <div class="groupbox_title_background gradientHToWhite">
                            <div class="groupbox_title" >
                                <%:Shared.MaintenanceStrings.Maintenance_AssignSportCaption %>
                            </div>
                        </div>
                        <div class="groupbox_content">
                            <div>
                                <table>
                                    <tr>
                                        <td>
                                            <label>Sport:</label>
                                        </td>
                                        <td>
                                            <%= 
                                                Html.Telerik().DropDownList()
                                                    .Name("AssignSportsId")
                                                    .DataBinding(binding => binding.Ajax().Select("AllSportsOnSportLevelAjax", "Shared"))
                                                    .ClientEvents(e => e
                                                        .OnClose("onAssignSportsComboClosed")
                                                        .OnLoad("onAssignSportsComboLoad")
                                                        .OnDataBound("onAssignSportsComboBound"))
                                                    .HtmlAttributes(new { style = "font-size:8pt;" })
                                            %>
                                        </td>
                                    </tr>

                                    <tr>
                                        <td>
                                            <label>Discipline:</label>
                                        </td>
                                        <td>
                                            <%= 
                                                Html.Telerik().DropDownList()
                                                    .Name("AssignDisciplineId")
                                                    .DataBinding(binding => binding.Ajax().Select("DisciplinesBySportAjax", "Shared"))
                                                    .ClientEvents(events => events
                                                        .OnDataBinding("onAssignDisciplineComboBinding")
                                                        .OnClose("onAssignDisciplineComboClosed")
                                                        .OnDataBound("onAssignDisciplineComboBound"))
                                                    .HtmlAttributes(new { style = "font-size:8pt;" })
                                            %>
                                        </td>
                                    </tr>

                                    <tr>
                                        <td>
                                            <label><%: Shared.MaintenanceStrings.Maintenance_AssignSportEvent %></label>
                                        </td>
                                        <td>
                                            <%= 
                                                Html.Telerik().DropDownList()
                                                    .Name("AssignEventId")
                                                    .DataBinding(binding => binding.Ajax().Select("EventsByDisciplineAjax", "Shared"))
                                                    .ClientEvents(events => events
                                                        .OnDataBinding("onAssignEventComboBinding"))
                                                    .HtmlAttributes(new { style = "font-size:8pt;" })
                                            %>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                        <img src="../../Content/Images/sidenav_btm.gif" width="600" height="9" /> 
                    </div> 
                    <div class="clear"></div>
                    <div style="text-align:center; margin: 8px 10px 0px 10px" >
                        <input type="submit" value="<%: Shared.MaintenanceStrings.Maintenance_AssignSportAndSave %>" onclick="SaveResource(true);$('#AssignSport').data('tWindow').close();" />
                        <input type="submit" value="<%: Shared.MaintenanceStrings.Maintenance_NotAssignSportAndSave %>" onclick="SaveResource(false);$('#AssignSport').data('tWindow').close();" />
                        <input type="submit" value="<%: Shared.SharedStrings.Button_Cancel %>" onclick="$('#AssignSport').data('tWindow').close();" />
                    </div>
                <%})
            .Draggable(true)
            .Scrollable(false)
            .Resizable()
            .Buttons(b => b.Close())
            .ClientEvents(e => e.OnClose("onAssignSportWindowClose"))
            .Width(675)
            .Height(155)
            .Modal(true)
            .Visible(false) /* window not visible on initial load */
            .Render();
        %>

        <div>
        <% Html.Telerik().Grid<MaintenanceAthletesAthleteGridViewModel>()
        .Name("Athletes")
        .TableHtmlAttributes(new { style = "font-size:8pt;" })
        .HtmlAttributes(new { style = "height:345px" })
        .DataKeys(keys => keys.Add(a => a.ResourceId))
        .DataBinding(dataBinding => dataBinding.Ajax()
            .Select("AthletesMaintenanceAthleteGridAjax", "Maintenance")
            .Insert("InsertAthlete", "Maintenance")
            .Update("UpdateAthlete", "Maintenance")
            .Delete("DeleteAthlete", "Maintenance")
        )
        .Pageable(paging => paging
            .PageSize(10)
            .PageTo((int)ViewData["athletesCurrentPage"])
            .Style(GridPagerStyles.NextPreviousAndInput | GridPagerStyles.Numeric)
            )
        .EnableCustomBinding(true)
        .Scrollable(scrolling => scrolling.Height(285))
        .Sortable()
        .Selectable()
        .Filterable()
        .Resizable(resize => resize.Columns(true))
        .ToolBar(t =>
            {
                if (Model.Editable) t.Insert().ButtonType(GridButtonType.ImageAndText);
            })
        .Editable(e => e.Mode(GridEditMode.InLine).Enabled(true))
        .ToolBar(commands => commands.Custom().Text(Shared.MaintenanceStrings.ExportToExcel).HtmlAttributes(new { onclick = "onExportAthletesPageToExcel();return false;" }))
        .ClientEvents(events => events.OnSave("onAthleteGridSave")
                                      .OnRowSelect("onAthleteGridRowSelect")
                                      .OnRowDataBound("onRowDataBound")
                                      .OnEdit("onAthleteGridEdit")
                                      .OnError("grid_OnError")
                                      //.OnLoad("replaceDeleteConfirmation")
                                      //.OnDataBound("replaceDeleteConfirmation")
                                      )
        .Columns(columns =>
        {
            columns.Bound(a => a.ResourceId)
                .Width(50)
                .Filterable(true);
            columns.Bound(a => a.LastName)
                .Width(120);
            columns.Bound(a => a.FirstName)
                .Width(120);
            columns.Bound(a => a.CountryName)
                //.ClientTemplate("<#= CountryName #>")
                .Width(130);
            columns.Bound(a => a.GenderName)
                //.ClientTemplate("<#= GenderName #>")
                .Width(70);
            columns.Bound(a => a.BirthDate)
                .Format("{0:dd/MM/yyyy}")
                .Width(95)
                .Filterable(true);
            columns.Bound(a => a.IsNew)
                 .ClientTemplate("<span <#= IsNew ? \"class='t-icon t-update'\" : \"\" #>></span>")
                 .Width(60)
                 .HtmlAttributes(new { style = "text-align:center" })
                 .Filterable(false);
            columns.Bound(a => a.BenchmarksEntered)
                .ClientTemplate("<span class='<#= BenchmarksEntered ? \"t-update\" : \"t-delete\" #> t-icon'></span>")
                .Width(75)
                .HtmlAttributes(new { style = "text-align:center" })
                .Filterable(false);
            columns.Bound(a => a.Category).Width(90);
            columns.Bound(a => a.PersonalCoachName)
                //.ClientTemplate("<#= PersonalCoachName #>")
                .Width(120)
                .Filterable(true);
            columns.Command(a =>
                {
                    a.Edit().ButtonType(GridButtonType.ImageAndText);
                    a.Delete().ButtonType(GridButtonType.ImageAndText);
                })
                .Width(175)
                .Title(Shared.SharedStrings.GridColumn_Command)
                .HtmlAttributes(new { style = "white-space:nowrap;float:right;" });
            columns.Bound(a => a.AssignSportId).Hidden(true);
        }).Render();
                
        %>
        </div>

        <div style="margin:40px 0px 0px 0px; height: 470px">
        <% Html.Telerik().TabStrip()
            .Name("TabStrip")
            .Items(tabstrip =>
            {
                tabstrip.Add()
                    .Text(Shared.MaintenanceStrings.Tab_General)
                    .HtmlAttributes(new { @class = "tab", url = "/Maintenance/AthletesGeneralInfo/", tabNumber = "1" })
                    .SpriteCssClasses("sprite sprite-vcard")
                    .Content(() =>
                        {
                          %>
                                <div class="groupbox_title_background gradientHToWhite">
                                    <div class="groupbox_title" >
                                        <%= ViewData["resourceName"] %>
                                    </div>
                                </div>
                          <%  
                        });
                tabstrip.Add()
                    .Text(Shared.MaintenanceStrings.Tab_Contact)
                    .HtmlAttributes(new { @class = "tab", url = "/Maintenance/AthletesContactInfo/", tabNumber = "2" })
                    .SpriteCssClasses("sprite sprite-phone")
                    .Content(() =>
                        {
                          %>
                            <div class="groupbox_title_background gradientHToWhite">
                                <div class="groupbox_title" >
                                    <%= ViewData["resourceName"] %>
                                </div>
                            </div>
                          <%  
                        });
                tabstrip.Add()
                    .Text(Shared.MaintenanceStrings.Tab_Sports)
                    .HtmlAttributes(new { @class = "tab", url = "/Maintenance/AthletesSportsInfo/", tabNumber = "3" })
                    .SpriteCssClasses("sprite sprite-sport_running_man")
                    .Content(() =>
                        {
                          %>
                            <div style="height: 400px">
                            <div class="groupbox_title_background gradientHToWhite">
                                <div class="groupbox_title" >
                                    <%= ViewData["resourceName"] %>
                                </div>
                            </div>
                            </div>
                          <%  
                        });
                tabstrip.Add()
                    .Text(Shared.MaintenanceStrings.Tab_DAS)
                    .HtmlAttributes(new { @class = "tab", url = "/Maintenance/AthletesDirectAthleteSupport/", tabNumber = "4" })
                    .SpriteCssClasses("sprite sprite-money")
                    .Content(() =>
                        {
                          %>
                            <div style="height: 400px">
                            <div class="groupbox_title_background gradientHToWhite">
                                <div class="groupbox_title" >
                                    <%= ViewData["resourceName"] %>
                                </div>
                            </div>
                            </div>
                          <%  
                        });
                tabstrip.Add()
                    .Text(Shared.MaintenanceStrings.Tab_Affiliates)
                    .HtmlAttributes(new { @class = "tab", url = "/Maintenance/AthletesAffiliatesInfo/", tabNumber = "5" })
                    .SpriteCssClasses("sprite sprite-sitemap_color")
                    .Content(() =>
                        {
                          %>
                            <div style="height: 400px">
                            <div class="groupbox_title_background gradientHToWhite">
                                <div class="groupbox_title" >
                                    <%= ViewData["resourceName"] %>
                                </div>
                            </div>
                            </div>
                          <%  
                        });
                tabstrip.Add()
                    .Text(Shared.MaintenanceStrings.Tab_Results)
                    .HtmlAttributes(new { @class = "tab", url = "/Maintenance/AthletesResults/", tabNumber = "6" })
                    .SpriteCssClasses("sprite sprite-medal_gold_3")
                    .Content(() =>
                        {
                          %>
                            <div style="height: 400px">
                            <div class="groupbox_title_background gradientHToWhite">
                                <div class="groupbox_title" >
                                    <%= ViewData["resourceName"] %>
                                </div>
                            </div>
                            </div>
                          <%  
                        });
            })
            .ClientEvents(e => e.OnSelect("onTabSelect"))
            .SelectedIndex(0)
            .Render();
        %>
        </div>
        
    </div>
    
    <script type="text/javascript">
        function onExportAthletesPageToExcel()
        {
            var grid = $('#Athletes').data('tGrid');
            var exportLimit = parseInt('<%: Constant.ExportExcelLimit %>');
            
            if (grid.total >= exportLimit) {
                var r = confirm("<%: Shared.MaintenanceStrings.ExportExcelTimeLimit %>");
                if (r == false) {
                    return;
                }
            }
            
            var page = grid.currentPage;
            var pageSize = grid.pageSize;
            var orderBy = grid.orderBy;
            var filter = grid.filterBy;

            var selectSport = '<%= Model.CurrentSport %>';
            var currentYear = '<%= Model.CurrentYear %>';
            
            var url = "/ExportExcelButton/ExportAthletePageToExcel";
            window.location.href = url + "?page=" + page + "&pageSize=" + pageSize + "&orderBy=" + orderBy + "&filter=" + filter + "&selectedSportId=" + selectSport + "&yearId=" + currentYear;
        }

        function replaceDeleteConfirmation()
        {
            var grid = $(this).data('tGrid');
            
            $(this).find('.t-grid-delete').click(function(e) {
                var tr = $(this).closest('tr');
                
                grid.localization.deleteConfirmation = '<%: Shared.MaintenanceStrings.Athlete_DeleteConfirmation %>';
            });
        }

        function onAthleteGridRowSelect(e)
        {
            var resourceId = e.row.cells[0].innerHTML;
            $('#resourceId').val(resourceId);
            
            if (resourceId.length > 0 && resourceId != "&nbsp;")
            {
                var activeTab = $('li.t-state-active.tab');
                refreshTab(activeTab.attr("tabNumber"), activeTab.attr("url"));
            }
        }

        function onRowDataBound(e)
        {
            // Get the Athletes grid
            var grid = $("#Athletes").data("tGrid");

            // Set the resourceID by the 1st row's resourceId
            var resourceID = grid.data[0].ResourceId;

            if (e.dataItem.ResourceId == resourceID) {
                $('#resourceId').val(resourceID);
                
                // Get the active tab and reload the content of the active tab
                var activeTab = $('li.t-state-active.tab');
                refreshTab(activeTab.attr("tabNumber"), activeTab.attr("url"));
            }
        }

        function onTabSelect(e)
        {
            var resourceId = $('#resourceId').val();
            refreshTab($(e.item).attr("tabNumber"), $(e.item).attr("url"));
        }

        function refreshTab(tabNumber, url)
        {
            var resourceId = $('#resourceId').val();
            var tabResourceId = $("#TabStrip-" + tabNumber).find('#tabResourceId').val();
            
            if(resourceId && resourceId != tabResourceId)
            {
                var editable = <%: Model.Editable.ToString().ToLower() %>;
                var tabStrip = $("#TabStrip").data("tTabStrip");

                // get the content url for the first tab strip item
                var tabContentUrl = $(tabStrip.element).parent().find(".t-link:eq(" + (tabNumber - 1) + ")").data("ContentUrl");
                if (tabContentUrl)
                {
                    //do nothing
                } 
                else
                {
                    tabContentUrl = url + resourceId + "?editable=" + editable;
                }

                $('li.tab[tabNumber=' + tabNumber + ']').find('.sprite').addClass('t-loading');

                // send an ajax request to get the data and post it to the first tab strip item
                $.ajax(
                {
                    type: "POST",
                    url: tabContentUrl,
                    data: { id: resourceId, editable: editable },
                    success: function (data)
                    {
                        //reload the details content tab
                        $("#TabStrip-" + tabNumber).html(data);
                    },
                    complete: function (data)
                    {
                        $('li.tab[tabNumber=' + tabNumber + ']').find('.sprite').removeClass('t-loading'); 
                    }
                });
            }
        }

        function onAthleteGridEdit(e)
        {
            var editable = '<%= Model.Editable %>'.toLowerCase();
            var currentLanguage = '<%= Model.CurrentLanguageCode %>';
            var languageCode = '<%: Constant.LanguageCode.French %>';
 
            var newRow = $("#CountryName").closest('.t-grid-new-row');
           
//            if (e.mode == 'insert') {
//                if (editable == 'true') {
//                    if (currentLanguage == languageCode) {
//                        // Add buttons in French
//                        $(e.form).find('td:last').prev()
//                                .append('<a href="#" class="t-grid-action t-button t-state-default t-grid-insert"><span class="t-icon t-insert"/>Insérer</a>')
//                                .append('<a href="#" class="t-grid-action t-button t-state-default t-grid-cancel"><span class="t-icon t-cancel"/>Annuler</a>')
//                                .find('.t-grid-delete')
//                                .remove();
//                    }
//                    else {
//                        // Add buttons in English
//                        $(e.form).find('td:last').prev()
//                            .append('<a href="#" class="t-grid-action t-button t-state-default t-grid-insert"><span class="t-icon t-insert"/>Insert</a>')
//                            .append('<a href="#" class="t-grid-action t-button t-state-default t-grid-cancel"><span class="t-icon t-cancel"/>Cancel</a>')
//                            .find('.t-grid-delete')
//                            .remove();
//                    }
//                }
//            }

//           if (e.mode == 'edit'){
//                //Get the resourceID
//                var resourceID = e.dataItem.ResourceId;
//                
//                // Set the resourceID 
//                if (resourceID != null) {
//                    $('#resourceId').val(resourceID);
//                }
//           }
        }

        function onAthleteGridSave(e)
        {
            var d = new Date();
            var refreshSeed = d.getTime();
            var values = e.values;
            var resourceType = "<%: Constant.ResourceType.Athletes %>";
            
            if (e.mode == 'insert') {
                $.ajax({
                    type: 'POST',
                    url: '/Maintenance/ShouldPrompResourceExists',
                    dataType: 'json',
                    data: { resourceTypeId: resourceType, firstName: values.FirstName, lastName: values.LastName, countryId: values.CountryId, refresh: refreshSeed },
                    async: false,
                    success: function(promptUser)
                    {
                        if(promptUser)
                        {
                            //TODO: Localize
                            var reply = confirm("<%: Shared.MaintenanceStrings.Athlete_SaveConfirmation %>");

                            if(reply == false)
                            {
                                e.preventDefault();
                            }
                        }
                    }
                });
            
                if(!e.isDefaultPrevented() && !e.dataItem)
                {
                   e.preventDefault();

                   $("#AssignSport").data("tWindow").center().open();
                   saveResourceValues = values;
                }
            }
        }

        var saveResourceValues;

        function SaveResource(assignSport)
        {
            if(assignSport)
            {
                saveResourceValues.AssignSportId = $('#AssignEventId').data('tDropDownList').value();
            }
            else
            {
                saveResourceValues.AssignSportId = "";
            }

            $.ajax({
                type: 'POST',
                url: '/Maintenance/InsertAthlete',
                dataType: 'json',
                data: saveResourceValues,
                async: false,
                success: function(result)
                {
                    $('div[class~="t-grid"]').data('tGrid').ajaxRequest();
                    OTPAlert("<%: Shared.MaintenanceStrings.Athlete_AddConfirmation %>");
                }
            });
        }

        function onAssignSportsComboClosed(e)
        {
            var disciplineDropDownList = $('#AssignDisciplineId').data('tDropDownList');
            var eventDownList = $('#AssignEventId').data('tDropDownList');
            eventDownList.text('');
            disciplineDropDownList.text('');
            disciplineDropDownList.reload();
        }

        function onAssignSportsComboLoad()
        {
            var combo = $(this).data('tDropDownList');
            combo.reload();
        }

        function onAssignSportsComboBound()
        {
            var selectedSport = '<%: ViewData["selectedSport"].ToString() %>';
            var editor = $(this).data('tDropDownList');
            editor.value(selectedSport);

            onAssignSportsComboClosed();
        }

        function onAssignSportWindowClose()
        {
            $('#AssignSportsId').data('tDropDownList').close();
            $('#AssignDisciplineId').data('tDropDownList').close();
            $('#AssignEventId').data('tDropDownList').close();
        }

        function onAssignDisciplineComboBinding(e)
        {
            //Get the previous drop down list's value
            var sportId = $('#AssignSportsId').data('tDropDownList').value();
            e.data = $.extend({}, e.data, { sportId: sportId });
        }

        function onAssignDisciplineComboClosed(e)
        {
            var dropDownList = $('#AssignEventId').data('tDropDownList');
            dropDownList.text('');
            dropDownList.reload();
        }

        function onAssignDisciplineComboBound()
        {
            onAssignDisciplineComboClosed();
        }

        function onAssignEventComboBinding(e)
        {
            //Get the previous drop down list's value
            var sportId = $('#AssignDisciplineId').data('tDropDownList').value();
            e.data = $.extend({}, e.data, { sportId: sportId });
        }

    </script>

</asp:Content>


