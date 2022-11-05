using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTP.Ring.Common;
using OTP.Ring.Data.Models;

namespace OTP.Ring.Common
{
    [Serializable]
    public class MenuHierarchy
    {
        private string _selectedPage;

        public List<RingMenuItem> SportMenuItems { get; set; }
        public List<RingMenuItem> PageMenuItems { get; set; }
        
        public MenuHierarchy(List<Sport> allSports, List<Sport> allowedSports, List<SecurityPage> allPages, List<Page> allowedPages, string languageCode, bool isGuest)
        {
            if (allPages != null && allowedPages != null)
            {
                //instantiate Page Menu Items
                PageMenuItems = new List<RingMenuItem>();

                List<string> subRootPageIds = new List<string> { Constant.PageId.ResultsIndex, Constant.PageId.EvaluationsIndex, Constant.PageId.FundingIndex, Constant.PageId.ReportingIndex, Constant.PageId.MaintenanceIndex };

                //Build Page Menu Items
                RingMenuItem allPageMenuItems = BuildAllPageMenuItems(allowedPages, allPages, languageCode, subRootPageIds);
                allPageMenuItems = SortMenuItem(allPageMenuItems);
                allPageMenuItems = InheritMenuItemAccess(allPageMenuItems);

                foreach (var subRootPageId in subRootPageIds)
                {
                    RingMenuItem group = ParseSubRootFromAllMenuItems(allPageMenuItems, subRootPageId);
                    if (group != null)
                    {
                        if (subRootPageId == Constant.PageId.ReportingIndex)
                        {
                            group.ExpandOnLoad = isGuest;
                        }

                        PageMenuItems.Add(group);
                    }
                }
            }

            if (allSports != null && allowedSports != null)
            {
                //instantiate Sport Menu Items
                SportMenuItems = new List<RingMenuItem>();

                if (!isGuest)
                {
                    //Build Sport Group
                    var topLevelSportSports = from s in allSports
                                              where s.SportID == Constant.SportId.All
                                              select s;
                    if (topLevelSportSports.Count() == 1)
                    {
                        RingMenuItem SportsGroup = new RingMenuItem(topLevelSportSports.First(), languageCode);
                        SportsGroup.ExpandOnLoad = true;
                        SportsGroup.ImageUrl = "~/Content/Images/sport_running_man.png";
                        SportsGroup.Url = string.Empty;

                        SportsGroup = BuildAllSportMenuItem(allowedSports, allSports, SportsGroup, languageCode);
                        SportMenuItems.Add(SportsGroup);
                    }
                }
            }
        }

        public string SelectedSport
        {
            set 
            {
                if (SportMenuItems != null)
                {
                    foreach (var item in SportMenuItems)
                    {
                        if (SetSelectedMenuItem(item, value)) break;
                    }
                }
            }
        }

        public string SelectedPage
        {
            set
            {
                this._selectedPage = value;

                if (PageMenuItems != null)
                {
                    foreach (var item in PageMenuItems)
                    {
                        if (SetSelectedMenuItem(item, this._selectedPage)) break;
                    }
                }
            }
        }

        public int ReadAccess
        {
            get 
            {
                if (string.IsNullOrEmpty(this._selectedPage)) return Constant.AccessLevel.No;

                var page = (from r in this.FlatPageMenuItems where r.Id == this._selectedPage select r).FirstOrDefault();

                if (page == null) return Constant.AccessLevel.No;

                if (page.ReadAccess == null) return Constant.AccessLevel.No;

                return page.ReadAccess.Value;
            }
        }

        public int WriteAccess
        {
            get
            {
                if (string.IsNullOrEmpty(this._selectedPage)) return Constant.AccessLevel.No;

                var page = (from r in this.FlatPageMenuItems where r.Id == this._selectedPage select r).FirstOrDefault();

                if (page == null) return Constant.AccessLevel.No;

                if (page.WriteAccess == null) return Constant.AccessLevel.No;

                return page.WriteAccess.Value;
            }
        }

        public List<RingMenuItem> FlatSportMenuItems
        {
            get
            {
                List<RingMenuItem> items = new List<RingMenuItem>();

                FlatMenuItems(this.SportMenuItems, items);
                items.Sort(new MenuItemComparer());

                return items;
            }
        }

        public List<RingMenuItem> FlatPageMenuItems
        {
            get
            {
                List<RingMenuItem> items = new List<RingMenuItem>();

                FlatMenuItems(this.PageMenuItems, items);
                items.Sort(new MenuItemComparer());

                return items;
            }
        }

        private void FlatMenuItems(List<RingMenuItem> inputs, List<RingMenuItem> outputs)
        {
            if (inputs != null)
            {
                outputs.AddRange((from s in inputs select s.Clone()).ToList());

                foreach (var input in inputs)
                {
                    FlatMenuItems(input.ChildItems, outputs);
                }
            }
        }

        private bool SetSelectedMenuItem(RingMenuItem input, string selectedItem)
        {
            if (input.Id == selectedItem)
            {
                input.ExpandOnLoad = true;
                input.Selected = true;
                return true;
            }

            bool matchFound = false;
            foreach (var item in input.ChildItems)
            {
                matchFound = matchFound || SetSelectedMenuItem(item, selectedItem);
                if (matchFound)
                {
                    input.ExpandOnLoad = true;
                    return matchFound;
                }
            }

            return matchFound;
        }

        private RingMenuItem ParseSubRootFromAllMenuItems(RingMenuItem allMenuItems, string subRootGroupName)
        {
            var groups = from g in allMenuItems.ChildItems
                    where g.Id == subRootGroupName
                    select g;

            if (groups.Count() == 1)
            {
                return groups.Single();
            }

            return null;
        }

        private RingMenuItem BuildAllPageMenuItems(List<Page> allowedPages, List<SecurityPage> allPages, string lauguageCode, List<string> subRootPageIds)
        {
            RingMenuItem rootGroup = new RingMenuItem();

            allowedPages = ParseIllogicAllowedPages(allowedPages, allPages, subRootPageIds);

            foreach (var allowedPage in allowedPages)
            {
                if (allowedPage.Id == Constant.PageId.HomeIndex) continue;

                RingMenuItem subRootMenuItem = BuildSubRootPageMenuItem(allowedPage, allPages, lauguageCode);
                MergeMenuItemsOnSameLevel(new List<RingMenuItem>() { subRootMenuItem }, rootGroup);
            }

            return rootGroup;
        }

        private List<Page> ParseIllogicAllowedPages(List<Page> allowedPages, List<SecurityPage> allPages, List<string> subRootPageIds)
        {
            if (allowedPages == null) return allowedPages;

            //if there is Home page with Recursive, and has ReadAccess, carry it to sub-root level
            Page home = (from p in allowedPages where p.Id == Constant.PageId.HomeIndex && p.Recursive && p.ReadAccess != Constant.AccessLevel.No select p).SingleOrDefault();

            if (home != null)
            {
                foreach (var subRootPageId in subRootPageIds)
                {
                    var exist = (from p in allowedPages where p.Id == subRootPageId select p).Any();

                    if (!exist)
                    {
                        var page = (from p in allPages where p.SecurityPageID == subRootPageId 
                                    select new Page()
                                    {
                                        Id = p.SecurityPageID,
                                        Name_EN = p.Name_EN,
                                        Name_FR = p.Name_FR,
                                        Url = p.URL,
                                        ParentId = p.ParentSecurityPageID,
                                        Order = p.Order,
                                        ReadAccess = home.ReadAccess,
                                        WriteAccess = home.WriteAccess,
                                        Recursive = home.Recursive
                                    }).SingleOrDefault();

                        if (page != null) allowedPages.Add(page);
                    }
                }
            }

            //remove all home pages
            allowedPages = (from p in allowedPages where p.Id != Constant.PageId.HomeIndex select p).ToList();

            //remove all pages without readaccess, SecurityGroupPage is for grant access, not deny access
            allowedPages = (from p in allowedPages where p.ReadAccess != Constant.AccessLevel.No select p).ToList();

            return allowedPages;
        }

        private RingMenuItem BuildAllSportMenuItem(List<Sport> allowedSports, List<Sport> allSports, RingMenuItem rootSportGroup, string lauguageCode)
        {
            //(SportID = ALL) is at root level, needs to find the sub root level sports
            if ((from s in allowedSports
                 where s.SportID == Constant.SportId.All
                 select s).Count() > 0)
            {
                allowedSports = (from s in allSports
                                 where s.ParentSportID == Constant.SportId.All && s.SportID != Constant.SportId.All
                                 select s).ToList();
            }

            foreach (var allowedSport in allowedSports)
            {
                if (allowedSport.SportID == Constant.SportId.All) continue;

                RingMenuItem subRootMenuItem = BuildSubRootSportMenuItem(allowedSport.SportID, allSports, lauguageCode);
                MergeMenuItemsOnSameLevel(new List<RingMenuItem>() { subRootMenuItem }, rootSportGroup);
            }

            return SortMenuItem(rootSportGroup);
        }

        private RingMenuItem BuildSubRootPageMenuItem(Page allowedPage, List<SecurityPage> allPages, string languageCode)
        {
            RingMenuItem menuItem = null;

            //find the allowed page itself
            var pages = from p in allPages
                        where p.SecurityPageID == allowedPage.Id
                        select p;

            SecurityPage page = null;

            if (pages.Count() == 1)
            {
                page = pages.First();

                menuItem = new RingMenuItem(page, languageCode, recursive: allowedPage.Recursive, readAccess: allowedPage.ReadAccess, writeAccess: allowedPage.WriteAccess);
            }
            else
            {
                return menuItem;
            }

            //find and link up its child pages, if recursive
            if (allowedPage.Recursive)
            {
                menuItem = FindLinkChildrenPageMenuItem(menuItem, allPages, languageCode);
            }

            //find and link up its parent pages
            string currentPageId = page.SecurityPageID;
            string parentPageId = page.ParentSecurityPageID;

            while (currentPageId != parentPageId && parentPageId != Constant.PageId.HomeIndex)
            {
                var parentPages = from p in allPages
                                  where p.SecurityPageID == parentPageId
                                  select p;

                if (parentPages.Count() == 1)
                {
                    var matchedParentPage = parentPages.First();

                    menuItem = new RingMenuItem(matchedParentPage, languageCode, new List<RingMenuItem>() { menuItem });

                    currentPageId = matchedParentPage.SecurityPageID;
                    parentPageId = matchedParentPage.ParentSecurityPageID;
                }
                else
                {
                    break;
                }
            }

            return menuItem;
        
        }

        private RingMenuItem BuildSubRootSportMenuItem(string allowedSportId, List<Sport> allSports, string languageCode)
        {
            RingMenuItem menuItem = null;

            //find the allowed sport itself
            var sports = from s in allSports
                         where s.SportID == allowedSportId
                         select s;

            Sport allowedSport = null;

            if (sports.Count() == 1)
            {
                allowedSport = sports.First();
                menuItem = new RingMenuItem(allowedSport, languageCode);
            }
            else
            {
                return menuItem;
            }

            //find and link up its child sports
            menuItem = FindLinkChildrenSportMenuItem(menuItem, allSports, languageCode);

            //find and link up its parent sports
            string currentSportId = allowedSport.SportID;
            string parentSportId = allowedSport.ParentSportID;

            while (currentSportId != parentSportId && parentSportId != Constant.SportId.All)
            {
                var parentSports = from s in allSports
                                   where s.SportID == parentSportId
                                   select s;

                if (parentSports.Count() == 1)
                {
                    var matchedParentSport = parentSports.First();

                    menuItem = new RingMenuItem(matchedParentSport, languageCode, new List<RingMenuItem>() { menuItem });

                    currentSportId = matchedParentSport.SportID;
                    parentSportId = matchedParentSport.ParentSportID;
                }
                else
                {
                    break;
                }

            }

            return menuItem;
        }

        private RingMenuItem FindLinkChildrenPageMenuItem(RingMenuItem input, List<SecurityPage> allPages, string languageCode)
        {
            RingMenuItem results = input;

            var menuItems = from p in allPages
                        where p.ParentSecurityPageID == input.Id && p.SecurityPageID != input.Id
                            select new RingMenuItem(p, languageCode);

            results.ChildItems = menuItems.ToList();

            for (int i = 0; i < results.ChildItems.Count; i++)
            {
                results.ChildItems[i] = FindLinkChildrenPageMenuItem(results.ChildItems[i], allPages, languageCode);
            }

            return results;
        }

        private RingMenuItem FindLinkChildrenSportMenuItem(RingMenuItem input, List<Sport> allSports, string languageCode)
        {
            RingMenuItem results = input;

            var menuItems = from s in allSports
                            where s.ParentSportID == input.Id && s.SportID != input.Id
                            select new RingMenuItem(s, languageCode);

            results.ChildItems = menuItems.ToList();

            for (int i = 0; i < results.ChildItems.Count; i++)
            {
                results.ChildItems[i] = FindLinkChildrenSportMenuItem(results.ChildItems[i], allSports, languageCode);
            }

            return results;
        }

        private RingMenuItem MergeMenuItemsOnSameLevel(List<RingMenuItem> input, RingMenuItem target)
        {
            foreach (var item in input)
            {
                target.AddChildItem(new RingMenuItem(
                    id: item.Id,
                    name: item.Name,
                    sortOrder: item.SortOrder,
                    imageUrl: item.ImageUrl,
                    styles: item.HtmlStyle,
                    expandOnLoad: item.ExpandOnLoad,
                    visible: item.Visible,
                    url: item.Url,
                    recursive: item.Recursive,
                    readAccess: item.ReadAccess,
                    writeAccess: item.WriteAccess));

                var i = from c in target.ChildItems
                        where c.Id == item.Id
                        select c;

                MergeMenuItemsOnSameLevel(item.ChildItems, i.Single());
            }

            return target;
        }

        private RingMenuItem SortMenuItem(RingMenuItem input)
        {
            input.ChildItems.Sort(new MenuItemComparer());

            foreach (var i in input.ChildItems)
            {
                SortMenuItem(i);
            }

            return input;
        }

        private RingMenuItem InheritMenuItemAccess(RingMenuItem input)
        {
            for (int i = 0; i < input.ChildItems.Count; i++)
            {
                var child = input.ChildItems[i];

                if (input.Recursive != null && input.Recursive.Value && child.Recursive == null)
                {
                    child.Recursive = input.Recursive;
                    child.ReadAccess = input.ReadAccess;
                    child.WriteAccess = input.WriteAccess;
                }

                child = InheritMenuItemAccess(child);
            }

            return input;
        }
    }

}