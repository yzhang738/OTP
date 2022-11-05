﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MaintenanceEditableViewModel>" %>

<input type="hidden" id="tabResourceId" value="<%= ViewData["resourceId"] %>" />

    <script type="text/javascript">
        function onPooledGridEdit(e) {
            var currentLanguage = '<%= Model.CurrentLanguageCode %>';
            var languageCode = '<%: Constant.LanguageCode.French %>';
                        
            if (e.mode == 'insert') {
                if (currentLanguage == languageCode) {
                    // Add buttons in French
                    $(e.form).find('td:last').prev()
                            .append('<a href="#" class="t-grid-action t-button t-state-default t-grid-insert"><span class="t-icon t-insert"/>Insérer</a>')
                            .append('<a href="#" class="t-grid-action t-button t-state-default t-grid-cancel"><span class="t-icon t-cancel"/>Annuler</a>')
                            .find('.t-grid-delete')
                            .remove();
                }
                else {
                    // Add buttons in English                    
                    $(e.form).find('td:last').prev()
                            .append('<a href="#" class="t-grid-action t-button t-state-default t-grid-insert"><span class="t-icon t-insert"/>Insert</a>')
                            .append('<a href="#" class="t-grid-action t-button t-state-default t-grid-cancel"><span class="t-icon t-cancel"/>Cancel</a>')
                            .find('.t-grid-delete')
                            .remove();
                }
            }
        }

        function restoreDefaultDeleteConfirmation() {
            var grid = $(this).data('tGrid');

            $(this).find('.t-grid-delete').click(function (e) {
                //var tr = $(this).closest('tr');

                grid.localization.deleteConfirmation = '<%: Shared.MaintenanceStrings.Maintenance_DefaultDeleteConfirmation %>';
            });
        }

        </script>

    <div class="groupbox_title_background gradientHToWhite">
        <div class="groupbox_title" >
            <%= ViewData["resourceName"] %>
        </div>
    </div>

    <% if (ViewData["resourceId"] != null)
       { %>

        <%= Html.Telerik().Grid<MaintenanceISTMembersPoolInfoViewModel>()
            .Name("Pools")
                .TableHtmlAttributes(new { style = "font-size:8pt;" })
                .HtmlAttributes(new { style = "height:335px" })
            .DataKeys(keys => keys.Add(s => s.ResourcePoolAssignmentId))
            .ToolBar(toolbar =>
                {
                    if (Model.Editable) toolbar.Insert().ButtonType(GridButtonType.ImageAndText);
                })
            .ClientEvents(e => e.OnError("grid_OnError")
                                //.OnEdit("onPooledGridEdit")
                                .OnLoad("restoreDefaultDeleteConfirmation")
                                .OnDataBound("restoreDefaultDeleteConfirmation")
                                )
            .DataBinding(dataBinding => dataBinding.Ajax()
                .Select("ISTMembersPoolInfoAjax", "Maintenance")
                .Insert("InsertISTMembersPoolAssignment", "Maintenance", new { pooled = ViewData["resourcePooled"] })
                .Update("UpdateISTMembersPoolAssignment", "Maintenance", new { resourceId = ViewData["resourceId"] })
                .Delete("DeleteISTMembersPoolAssignment", "Maintenance", new { resourceId = ViewData["resourceId"] })
            )
            .Pageable(paging => paging.PageSize(10))
                .Scrollable(scrolling => scrolling.Height(276))
            .Resizable(resize => resize.Columns(true))
            .Sortable()
            .Filterable()
            .Columns(columns =>
            {
                if ((bool)ViewData["resourcePooled"])
                {
                    columns.Bound(s => s.ResourceIdAssignment)
                        .ClientTemplate("<#= ResourceIdAssignmentName #>").Width(550);
                }
                else
                {
                    columns.Bound(s => s.ResourceIdPool)
                        .ClientTemplate("<#= ResourceIdPoolName #>").Width(550);
                }
                columns.Bound(s => s.HasComments)
                    .ClientTemplate("<span onclick=\"openCommentWindow('ResourcePoolAssignment', '<#= ResourcePoolAssignmentId #>', '<#= ResourceIdPoolName #>', '" + Model.Editable + "', 'true');\" class=\"comment sprite sprite-<#= HasComments ? \"note\" : \"note_none\" #>\"></span>")
                    .Width(50)
                    .Title("")
                    .Filterable(false);
                columns.Command(commands =>
                {
                    if (Model.Editable)
                    {
                        commands.Edit().ButtonType(GridButtonType.ImageAndText);
                        commands.Delete().ButtonType(GridButtonType.ImageAndText);
                    }
                })
                .Width(175)
                .Title(Shared.SharedStrings.GridColumn_Command)
                .HtmlAttributes(new { style = "white-space:nowrap;float:right;" });
                columns.Bound(s => s.ResourcePoolAssignmentId).Hidden(true);
            })
            %>

            <br />
            <br />
            <div class="clear"></div>

    <% } %>
    


           