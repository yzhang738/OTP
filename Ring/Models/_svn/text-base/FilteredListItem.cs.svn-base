using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace OTP.Ring.Models
{
    [Serializable]
    [TypeConverter(typeof(FilteredListItemConverter))]
    public class FilteredListItem : IComparable
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string FilterOnId { get; set; }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj is FilteredListItem)
            {
                var otherFilteredListItem = (FilteredListItem)obj;
                return Description.CompareTo(otherFilteredListItem.Description);
            }
            else
            {
                throw new ArgumentException("Object is not a FilteredListItem.");
            }
        }

        #endregion
    }

    public class FilteredListItemConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string)) ? true : base.CanConvertFrom(context, sourceType);
        }
    }
}
