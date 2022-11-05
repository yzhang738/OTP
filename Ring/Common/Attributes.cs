using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace OTP.Ring.Common
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ExportToExcelAttribute : Attribute
    {
        public string PropertyName { get; set; }
        public string PropertyDisplayName { get; set; }
        public int CellLength { get; set; }
        public string CellAlignment { get; set; }

        public ExportToExcelAttribute()
        {
            PropertyName = string.Empty;
            PropertyDisplayName = string.Empty;
            CellLength = 1;
            CellAlignment = Constant.ExcelAlignment.Left;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class SortAttribute : Attribute
    {
        public string PropertyName { get; set; }
        public ListSortDirection SortOrder { get; set; }
        public int SortPriority { get; set; }

        public SortAttribute()
        {
            SortOrder = ListSortDirection.Ascending;
            SortPriority = 0;
        }
    }
}
