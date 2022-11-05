using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace OTP.Ring.Models
{
    [Serializable]
    [TypeConverter(typeof(ListItemConverter))]
    public class ListItem : IComparable
    {
        public string Id { get; set; }
        public string Description { get; set; }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj is ListItem)
            {
                var otherListItem = (ListItem)obj;
                return Description.CompareTo(otherListItem.Description);
            }
            else
            {
                throw new ArgumentException("Object is not a ListItem.");
            }
        }

        #endregion
    }

    public class ListItemConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string)) ? true : base.CanConvertFrom(context, sourceType);
        }
    }
}
