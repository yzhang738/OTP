using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTP.Ring.Common
{
    [Serializable]
    public class Page
    {
        public string Id { get; set; }
        public string Name_EN { get; set; }
        public string Name_FR { get; set; }
        public string Url { get; set; }
        public string ParentId { get; set; }
        public string Order { get; set; }

        public int ReadAccess { get; set; }
        public int WriteAccess { get; set; }
        public bool Recursive { get; set; }
    }
}
