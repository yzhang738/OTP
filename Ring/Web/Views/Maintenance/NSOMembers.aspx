<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MaintenanceNSOMembersViewModel>" %>

<asp:Content ID="Content7" ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.RenderPartial("~/Views/Shared/JavaScriptCheck.ascx"); %>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%: Shared.MaintenanceStrings.NSOMembers_PageTitle %>: <%: ViewData["sportName"] %> - <%: Shared.MaintenanceStrings.NSOMembers_ResourceTitle %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MenuContent" runat="server">
    <% Html.RenderPartial("~/Views/Shared/NavMenu.ascx", Model.Menu); %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="InfoBar" runat="server">
    <% Html.RenderPartial("~/Views/Shared/LanguageSelector.ascx", Model.CurrentLanguageCode); %> &nbsp;|&nbsp;
    <% Html.RenderPartial("~/Views/Shared/CurrentUserName.ascx", Model.CurrentUserName); %>
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="HeaderImage" runat="server">
    <%= RingHTMLHelper.GetRandomHeaderImage() %>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="YearSelectionContent" runat="server">
    <% Html.RenderPartial("~/Views/Shared/YearSelector.ascx", Model.CurrentOlympicYears); %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">

    <div id="container">
        <input type="hidden" id="resourceId" value="<%= ViewData["resourceId"] %>" />

        <% Html.RenderPartial("~/Views/Shared/CommentEditor.ascx"); %>
        <%//TODO: Localize %>
        <%
            Html.Telerik().Window()
            .Name("AssignSport")
            .Title(Shared.MaintenanceStrings.Maintenance_AssignSportTitle)
            .Content(() =>
                { %>
                    <div class="groupbox_600_member" style="margin:0px 0px 0px 0px">
                        <div class="groupbox_title_background gradientHToWhite">
                            <div class="groupbox_title" >
                                <%: Shared.MaintenanceStrings.MaintenanceNSO_AssignSportCaption %>
                            </div>
                        </div>
                        <div class="groupbox_content_member">
                            <div>
                                <table>
                                    <tr>
                                        <td><label>Sport:</label></td>
                                        <td style="width: 5px"></td>
                                        <td>
                                            <%= 
                                                Html.Telerik().DropDownList()
                                                    .Name("AssignSportsId")
                                                    .DataBinding(binding => binding.Ajax().Select("AllSportsUnderSportLevelAjax", "Shared"))
                                                    .HtmlAttributes(new { style = "font-size:8pt;width:450px" })
                                                    .ClientEvents(events => events
                                                    .OnLoad("onAssignSportsComboLoad")
                                                    .OnDataBound("onAssignSportsComboBound"))
                                            %>
                                         </td>
                                    </tr>
                                </table>
                           </div>
                        </div>
                        <img src="../../Content/Images/sidenav_btm.gif" width="600" height="9" /> 
                    </div> 
                    <div class="clear"></div>
                    <div style="text-align:center; margin: 20px 10px 0px 0px" >
                        <input type="submit" value="<%: Shared.MaintenanceStrings.MaintenanceNSO_AssignSportAndSave %>" onclick="SaveResource(true);$('#AssignSport').data('tWindow').close();" />
                        <input type="submit" value="<%: Shared.MaintenanceStrings.MaintenanceNSO_NotAssignSportAndSave %>" onclick="SaveResource(false);$('#AssignSport').data('tWindow').close();" />
                        <input type="submit" value="<%: Shared.SharedStrings.Button_Cancel %>" onclick="$('#AssignSport').data('tWindow').close();" />
                    </div>
                <%})
            .Draggable(true)
            .Scrollable(false)
            .Resizable()
            .Buttons(b => b.Close())
            .ClientEvents(e => e.OnClose("onAssignSportWindowClose"))
            .Width(780)
            .Height(150)
            .Modal(true)
            .Visible(false) /* window not visible on initial load */
            .Render();
        %>

        <div>
        <% Html.Telerik().Grid<MaintenanceNSOMembersGridViewModel>()
        .Name("NSOs")
        .TableHtmlAttributes(new { style = "font-size:8pt;" })
        .HtmlAttributes(new { style = "height:360px" })
        .DataKeys(keys => keys.Add(a => a.ResourceId))
        .DataBinding(dataBinding => dataBinding.Ajax()
            .Select("NSOMembersGridAjax", "Maintenance")
            .Insert("InsertNSOMember", "Maintenance")
            .Update("UpdateNSOMember", "Maintenance")
            .Delete("DeleteNSOMember", "Maintenance")
        )
        .Pageable(paging => paging
            .PageSize(10)
            .PageTo((int)ViewData["nsoCurrentPage"])
            .Style(GridPagerStyles.NextPreviousAndInput | GridPagerStyles.Numeric)
            )
        .EnableCustomBinding(true)
        .Scrollable(scrolling => scrolling.Height(300))
        .Sortable()
        .Selectable()
        .Filterable()
        .Resizable(resize => resize.Columns(true))
        .ToolBar(t =>
            {
                if (Model.Editable) t.Insert().ButtonType(GridButtonType.ImageAndText);
            })
        .ToolBar(commands => commands.Custom().Text(Shared.MaintenanceStrings.ExportToExcel).HtmlAttributes(new { onclick = "onExportNSOMembersPageToExcel();return false;" }))
        .ClientEvents(events => events.OnSave("onNSOGridSave")
                                      //.OnEdit("onNSOGridEdit")
                                      .OnRowSelect("onNSOGridRowSelect")
                                      .OnRowDataBound("onRowDataBound")
                                      .OnError("grid_OnError")
                                      //.OnLoad("replaceMemberDeleteConfirmation")
                                      //.OnDataBound("replaceMemberDeleteConfirmation")
                                     )
        .Columns(columns =>
        {
            columns.Bound(a => a.ResourceId)
                .Width(50)
                .Filterable(false);
            columns.Bound(a => a.LastName)
                .Width(120);
            columns.Bound(a => a.FirstName)
                .Width(120);
            columns.Bound(a => a.CountryName)
                //.ClientTemplate("<#= CountryName #>")
                .Width(130);
            columns.Bound(a => a.BirthDate)
                .Format("{0:yyyy/MM/dd}")
                .Width(100);
                //.Filterable(false);
            columns.Bound(a => a.Pooled)
                .ClientTemplate("<span <#= Pooled ? \"class='t-icon t-update'\" : \"\" #>></span>")
                .Width(60)
                .HtmlAttributes(new { style = "text-align:center" })
                .Filterable(false);
            columns.Bound(a => a.TBD)
                .ClientTemplate("<span <#= TBD ? \"class='t-icon t-update'\" : \"\" #>></span>")
                .Width(60)
                .HtmlAttributes(new { style = "text-align:center" })
                .Filterable(false);
            columns.Bound(a => a.GenderName)
                //.ClientTemplate("<#= GenderName #>")
                .Width(90);
            columns.Bound(a => a.ResourceSpecialtyName)
                //.ClientTemplate("<#= ResourceSpecialtyName #>")
                .Width(150);
            columns.Command(a =>
                {
                    if (Model.Editable)
                    {
                        a.Edit().ButtonType(GridButtonType.ImageAndText);
                        a.Delete().ButtonType(GridButtonType.ImageAndText);
                    }
                })
                .Width(175)
                .Title(Shared.SharedStrings.GridColumn_Command)
                .HtmlAttributes(new { style = "white-space:nowrap;float:right;" });
        }).Render();
                
        %>
        </div>

        <div style="margin:40px 0px 0px 0px; height: 450px">
        <% Html.Telerik().TabStrip()
            .Name("TabStrip")
            .Items(tabstrip =>
            {
                tabstrip.Add()
                    .Text(Shared.MaintenanceStrings.Tab_General)
                    .SpriteCssClasses("sprite sprite-vcard")
                    .HtmlAttributes(new { @class = "tab", url = "/Maintenance/NSOMembersGeneralInfo/", tabNumber = "1" })
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
                    .SpriteCssClasses("sprite sprite-phone")
                    .HtmlAttributes(new { @class = "tab", url = "/Maintenance/NSOMembersContactInfo/", tabNumber = "2" })
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
                    .SpriteCssClasses("sprite sprite-sport_running_man")
                    .HtmlAttributes(new { @class = "tab", url = "/Maintenance/NSOMembersSportsInfo/", tabNumber = "3" })
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
                    .Text(Shared.MaintenanceStrings.Tab_Qualifications)
                    .SpriteCssClasses("sprite sprite-script")
                    .HtmlAttributes(new { @class = "tab", url = "/Maintenance/NSOMembersQualificationsInfo/", tabNumber = "4" })
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
                    .Text(Shared.MaintenanceStrings.Tab_PoolAssignment)
                    .SpriteCssClasses("sprite sprite-group_link")
                    .HtmlAttributes(new { @class = "tab", url = "/Maintenance/NSOMembersPoolInfo/", tabNumber = "5" })
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
        function onExportNSOMembersPageToExcel()
        {
            var grid = $('#NSOs').data('tGrid');

            var page = grid.currentPage;
            var pageSize = grid.pageSize;
            var orderBy = grid.orderBy
            var filter = grid.filterBy;

            var selectSport = '<%= Model.CurrentSport %>';
            
            var url = "/ExportExcelButton/ExportNSOMembersPageToExcel";
            window.location.href = url + "?page=" + page + "&pageSize=" + pageSize + "&orderBy=" + orderBy + "&filter=" + filter + "&selectedSportId=" + selectSport;
        }

//        function replaceMemberDeleteConfirmation()
//        {
//            var grid = $(this).data('tGrid');
//            
//            $(this).find('.t-grid-delete').click(function(e) {
//                var tr = $(this).closest('tr');
//                
//                grid.localization.deleteConfirmation = '<%: Shared.MaintenanceStrings.Member_DeleteConfirmation %>';
//            });
//        }

        function onNSOGridRowSelect(e)
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
            var grid = $("#NSOs").data("tGrid");

            // Set the resourceID by the 1st row's resourceId
            var resourceID = grid.data[0].ResourceId;
            $('#resourceId').val(resourceID);

            // Get the active tab and reload the content of the active tab
            var activeTab = $('li.t-state-active.tab');
            refreshTab(activeTab.attr("tabNumber"), activeTab.attr("url"));

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

        function onNSOGridEdit(e) {
            if (e.mode == 'insert') {
                $(e.form).find('td:last')
                     .append('<a href="#" class="t-grid-action t-button t-state-default t-grid-insert"><span class="t-icon t-insert"/>Insert</a>')
                     .append('<a href="#" class="t-grid-action t-button t-state-default t-grid-cancel"><span class="t-icon t-cancel"/>Cancel</a>')
                     .find('.t-grid-delete')
                     .remove();
            }
        } 

        function onNSOGridSave(e)
        {
            var d = new Date();
            var refreshSeed = d.getTime();
            var values = e.values;
            var resourceType = "<%: Constant.ResourceType.NSOMembers %>";

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
                            var reply = confirm("<%: Shared.MaintenanceStrings.NSOMember_SaveConfirmation %>");

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
                saveResourceValues.AssignSportId = $('#AssignSportsId').data('tDropDownList').value();
            }
            else
            {
                saveResourceValues.AssignSportId = "";
            }

            $.ajax({
                type: 'POST',
                url: '/Maintenance/InsertNSOMember',
                dataType: 'json',
                data: saveResourceValues,
                async: false,
                success: function(result)
                {
                    $('div[class~="t-grid"]').data('tGrid').ajaxRequest();
                    OTPAlert("<%: Shared.MaintenanceStrings.NSOMember_AddConfirmation %>");
                }
            });
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
        }

        function onAssignSportWindowClose()
        {
            $('#AssignSportsId').data('tDropDownList').close();
        }

    </script>

</asp:Content>


