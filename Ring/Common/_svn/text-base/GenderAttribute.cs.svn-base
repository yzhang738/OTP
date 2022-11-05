using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace OTP.Ring.Common
{
    [Serializable]
    public class GenderAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var isvalid = false;

            if (value == null
                || ((string)value).Equals("null"))
            {
                isvalid = true;
            }
            else if(Regex.IsMatch((string)value, @"\A[MFmf]\z"))
            {
                isvalid = true;
            }

            return isvalid;
        }
    }
}