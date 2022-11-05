using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
}
