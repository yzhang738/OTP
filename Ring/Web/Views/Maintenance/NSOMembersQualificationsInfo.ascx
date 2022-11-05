<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MaintenanceEditableViewModel>" %>

<input type="hidden" id="tabResourceId" value="<%= ViewData["resourceId"] %>" />

     <script type="text/javascript">
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

        <%= Html.Telerik().Grid<MaintenanceNSOMembersQualificationInfoViewModel>()
            .Name("Qualifications")
                .TableHtmlAttributes(new { style = "font-size:8pt;" })
                .HtmlAttributes(new { style = "height:335px" })
            .DataKeys(k => k.Add(k2 => k2.ResourceQualificationId))
            .ToolBar(t =>
                {
                    if (Model.Editable) t.Insert().ButtonType(GridButtonType.ImageAndText);
                })
            .Editable(e => e.Mode(GridEditMode.InLine).Enabled(true))
            .ClientEvents(e => e.OnError("grid_OnError")
                                .OnLoad("restoreDefaultDeleteConfirmation")
                                .OnDataBound("restoreDefaultDeleteConfirmation")
                                )
            .DataBinding(dataBinding => dataBinding.Ajax()
                .Select("NSOMembersQualificationsAjax", "Maintenance")
                .Update("UpdateNSOResourceQualification", "Maintenance")
                .Insert("InsertNSOResourceQualification", "Maintenance")
                .Delete("DeleteNSOResourceQualification", "Maintenance")
            )
            .Pageable(paging => paging.PageSize(10))
                .Scrollable(scrolling => scrolling.Height(276))
            .Resizable(resize => resize.Columns(true))
            .Sortable()
            .Filterable()
            .Columns(columns =>
            {
                columns.Bound(s => s.QualificationTypeId)
                    .ClientTemplate("<#= QualificationTypeName #>")
                    .Width(200);
                columns.Bound(s => s.Description).Width(300);
                columns.Bound(s => s.Qualified)
                    .ClientTemplate("<span class='<#= Qualified ? \"t-update\" : \"t-delete\" #> t-icon'></span>")
                    .Width(85)
                    .HtmlAttributes(new { style = "text-align:center" });
                columns.Bound(s => s.HasComments)
                        .ClientTemplate("<span onclick=\"openCommentWindow('ResourceQualification', '<#= ResourceQualificationId #>', '<#= Description #>', '" + Model.Editable + "', 'true');\" class=\"comment sprite sprite-<#= HasComments ? \"note\" : \"note_none\" #>\"></span>")
                        .Width(37)
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
               columns.Bound(s => s.ResourceQualificationId).Hidden(true);
                columns.Bound(s => s.ResourceId).Hidden(true);

            })

                    %>

            <br />
            <br />
            <div class="clear"></div>

    <% } %>

    


           