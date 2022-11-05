<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<NavMenuViewModel>" %>

    <% if (Model.ShowPageMenu)
       { %>
        <%: 
         Html.Telerik().PanelBar()
        .Name("PageNav")
        .HtmlAttributes(new { style = "width: 200px; float: top; margin-bottom: 30px;" })
        .ExpandMode(PanelBarExpandMode.Single)
        .BindTo(Model.Menu.PageMenuItems, mappings =>
        {
            mappings.For<RingMenuItem>(binding => binding
                    .ItemDataBound((item, pages) =>
                    {
                        var urlHelper = new UrlHelper(this.ViewContext.RequestContext);

                        item.Text = pages.Name;
                        
                        if(!string.IsNullOrEmpty(pages.IconClass))
                        {
                            item.SpriteCssClasses = pages.IconClass;
                        }
                        
                        item.Expanded = pages.ExpandOnLoad;
                        item.HtmlAttributes.Add("style", pages.HtmlAttributes);
                        item.Selected = pages.Selected;
                        item.Visible = pages.Visible;
                        
                        if (!string.IsNullOrEmpty(pages.Url))
                        {
                            item.Url = urlHelper.Content("~/" + pages.Url);
                        }
                    })
                    .Children(pages => pages.ChildItems));
        })
        %>
    <% } %>

    <% if (Model.ShowSportMenu) 
       { %>
        <%: 
         Html.Telerik().PanelBar()
        .Name("SportNav")
        .HtmlAttributes(new { style = "width: 200px; float: top; margin-bottom: 30px;" })
        .ExpandMode(PanelBarExpandMode.Single)
        .BindTo(Model.Menu.SportMenuItems, mappings => 
        {
            mappings.For<RingMenuItem>(binding => binding
                    .ItemDataBound((item, sports) =>
                    {
                        item.Text = sports.Name;

                        if (!string.IsNullOrEmpty(sports.IconClass))
                        {
                            item.SpriteCssClasses = sports.IconClass;
                        }
                        
                        item.Expanded = sports.ExpandOnLoad;
                        item.Selected = sports.Selected;
                        item.Visible = sports.Visible;
                        item.HtmlAttributes.Add("style", sports.HtmlAttributes);
                        item.LinkHtmlAttributes.Add("onclick", string.Format("changeSport('{0}', '{1}')", sports.Id, Url.RouteUrl(new { controller = (string)this.Url.RequestContext.RouteData.Values["controller"], action = (string)this.Url.RequestContext.RouteData.Values["action"], id = (string)this.Url.RequestContext.RouteData.Values["id"] })));
                    })
                    .Children(sports => sports.ChildItems));
        })
        %>
    <%} %>