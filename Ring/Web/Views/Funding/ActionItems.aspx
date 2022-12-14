<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<FundingActionItemViewModel>" %>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <% Html.RenderPartial("~/Views/Shared/JavaScriptCheck.ascx"); %>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%: Shared.FundingStrings.ActionItems_Title %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<div id="container">

    <div style="float:left; width:100%;">
    
    <% Html.RenderPartial("~/Views/Shared/CommentEditor.ascx"); %>

    
        <%= Html.Telerik().Grid<ActionItem>()
            .Name("ActionItem")
            .DataKeys(keys => keys.Add(d => d.Id))
            .Editable(editing => editing.Mode(GridEditMode.InLine))                    
            .TableHtmlAttributes(new { style = "font-size:8pt;" })
            .Columns(columns =>
                {
                    columns.Bound(b => b.OrganizationDesc).ClientTemplate("<#= OrganizationDesc  #>").ReadOnly();
                    columns.Bound(b => b.SportDesc).ClientTemplate("<#= SportDesc  #>").ReadOnly();
                    columns.Bound(b => b.TypeDesc).ClientTemplate(Html.ActionLink("<#= TypeDesc  #>", "<#= ActionName  #>", "<#= ControllerName  #>", new { id = "<#= ReferenceId  #>" }, null).ToHtmlString()).ReadOnly();

                    columns.Bound(b => b.Description).Width(150);
                    columns.Bound(b => b.DueDate)
                        .ClientTemplate("<#= DueDateLiteral  #>");
                    columns.Bound(b => b.Status).ClientTemplate("<#= StatusDesc  #>");
                    columns.Bound(b => b.Priority).ClientTemplate("<#= PriorityDesc  #>");
                    
                    columns.Bound(b => b.Comment)
                        .ClientTemplate("<span onclick=\"openCommentWindow('ActionItem', '<#= Id #>', '<#= TypeDesc #>', 'true', '<#= Editable #>');\" class=\"comment sprite sprite-<#= HasComment ? \"note\" : \"note_none\" #>\"></span>")
                        .Width(37)
                        .Title("")
                        .ReadOnly();

                    columns.Command(commands => commands.Edit().ButtonType(GridButtonType.ImageAndText)).Width(90);
                })
                .ClientEvents(e => e.OnError("grid_OnError").OnRowDataBound("gridActionItem_OnRowDataBound")) 
            .DataBinding(dataBinding => dataBinding.Ajax()
                            .Select("GetActionItemsAjax", "Funding")
                            .Update("UpdateActionItemAjax", "Funding"))
            .Scrollable(scrolling => scrolling.Height(380))
            .Resizable(resize => resize.Columns(true))
            .Sortable()
            .Pageable(paging => paging.PageSize(10))
            .Filterable()
            .ToHtmlString()
            %>

    
    </div>

</div>

    <script type="text/javascript">

        function gridActionItem_OnRowDataBound(e) {

            if (!e.dataItem.Editable) {
                e.row.cells[8].innerHTML = '';
            }

            return;
        }

    </script>

</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="HeaderImage" runat="server">
    <%= RingHTMLHelper.GetRandomHeaderImage() %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="MenuContent" runat="server">
    <% Html.RenderPartial("~/Views/Shared/NavMenu.ascx", Model.Menu); %>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="InfoBar" runat="server">
    <% Html.RenderPartial("~/Views/Shared/LanguageSelector.ascx", Model.CurrentLanguageCode); %> &nbsp;|&nbsp;
    <% Html.RenderPartial("~/Views/Shared/CurrentUserName.ascx", Model.CurrentUserName); %>
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="YearSelectionContent" runat="server">
    <% Html.RenderPartial("~/Views/Funding/ActionItemStatusSelector.ascx", Model); %>
</asp:Content>
