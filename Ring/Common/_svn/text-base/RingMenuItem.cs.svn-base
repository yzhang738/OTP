using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTP.Ring.Data.Models;

namespace OTP.Ring.Common
{
    [Serializable]
    public class MenuItemComparer : IComparer<RingMenuItem>
    {
        public int Compare(RingMenuItem x, RingMenuItem y)
        {
            return x.SortOrder.CompareTo(y.SortOrder);
        }
    }

    [Serializable]
    public class RingMenuItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public object SecurityInfo { get; set; }
        //public MenuItem ParentItem { get; set; }
        public List<RingMenuItem> ChildItems { get; set; }
        public string SortOrder { get; set; }
        public string ImageUrl { get; set; }
        public HtmlStyleAttribute.Styles HtmlStyle { get; set; }
        public bool ExpandOnLoad { get; set; }
        public bool Selected { get; set; }
        public bool Visible { get; set; }
        public string Url { get; set; }

        public bool? Recursive { get; set; }
        public int? ReadAccess { get; set; }
        public int? WriteAccess { get; set; }

        public string IconClass
        {
            get
            {
                var iconClass = "";

                if (!string.IsNullOrEmpty(ImageUrl))
                {
                    iconClass = "sprite sprite-" + ImageUrl.Substring(ImageUrl.LastIndexOf('/') + 1, ImageUrl.LastIndexOf('.') - ImageUrl.LastIndexOf('/') - 1);
                }
                return iconClass;
            }
        }

        public string HtmlAttributes
        {
            get
            {
                return HtmlStyle.ToHtmlStyleString();
            }
        }

        public RingMenuItem(string id = null, string name = null, object securityInfo = null, RingMenuItem parentItem = null, List<RingMenuItem> childItems = null, string sortOrder = null, string imageUrl = "", HtmlStyleAttribute.Styles styles = HtmlStyleAttribute.Styles.Normal, bool expandOnLoad = false, string url = "", bool selected = false, bool visible = true, bool? recursive = null, int? readAccess = null, int? writeAccess = null)
        {
            Id = id;
            Name = name;
            SecurityInfo = securityInfo;
            //ParentItem = parentItem;
            ChildItems = childItems ?? new List<RingMenuItem>();
            SortOrder = sortOrder;
            ImageUrl = imageUrl;
            HtmlStyle = styles;
            ExpandOnLoad = expandOnLoad;
            Selected = selected;
            Visible = visible;
            Url = url;

            Recursive = recursive;
            ReadAccess = readAccess;
            WriteAccess = writeAccess;
        }

        public RingMenuItem(SecurityPage page, string languageCode, List<RingMenuItem> childItems = null, bool? recursive = null, int? readAccess = null, int? writeAccess = null)
        {
            Id = page.SecurityPageID;
            Name = languageCode == Ring.Common.Constant.LanguageCode.French ? page.Name_FR : page.Name_EN;
            SecurityInfo = null;
            ChildItems = childItems ?? new List<RingMenuItem>();
            SortOrder = page.Order;
            ImageUrl = page.IconURL;
            HtmlStyle = (HtmlStyleAttribute.Styles)page.StylingCode;
            ExpandOnLoad = false;
            Selected = false;
            Visible = page.Visible;
            Url = page.NodeLeafCode == Ring.Common.Constant.NodeLeafCode.Leaf ? page.URL : string.Empty;

            Recursive = recursive;
            ReadAccess = readAccess;
            WriteAccess = writeAccess;
        }

        public RingMenuItem(Sport sport, string languageCode, List<RingMenuItem> childItems = null)
        {
            Id = sport.SportID;
            Name = languageCode == Ring.Common.Constant.LanguageCode.French ? sport.Name_FR : sport.Name_EN;
            SecurityInfo = null;
            ChildItems = childItems ?? new List<RingMenuItem>();
            SortOrder = Name;
            ImageUrl = string.Empty;
            HtmlStyle = (HtmlStyleAttribute.Styles)sport.StylingCode;
            ExpandOnLoad = false;
            Selected = false;
            Visible = true;
            Url = string.Empty;
        }

        public void AddChildItem(RingMenuItem item)
        {
            if (this.ChildItems == null)
            {
                this.ChildItems = new List<RingMenuItem>();
            }

            var child = (from c in this.ChildItems where c.Id == item.Id select c).SingleOrDefault();

            if (child == null)
            {
                this.ChildItems.Add(item);
            }
            else
            {
                if (child.Recursive == null && item.Recursive != null)
                {
                    child.Recursive = item.Recursive;
                    child.ReadAccess = item.ReadAccess;
                    child.WriteAccess = item.WriteAccess;
                }
            }
        }

        public RingMenuItem Clone()
        {
            return new RingMenuItem 
                { 
                    Id = this.Id,
                    Name = this.Name,
                    SecurityInfo = this.SecurityInfo,
                    ChildItems = new List<RingMenuItem>(),
                    SortOrder = this.SortOrder,
                    ImageUrl = this.ImageUrl,
                    HtmlStyle = this.HtmlStyle,
                    ExpandOnLoad = this.ExpandOnLoad,
                    Selected = this.Selected,
                    Visible = this.Visible,
                    Url = this.Url,
                    Recursive = this.Recursive,
                    ReadAccess = this.ReadAccess,
                    WriteAccess = this.WriteAccess 
                };
        }
    }
}