using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTP.Ring.Common
{
    [Serializable]
    public class Decode
    {
        public int IntId { get; set; }
        public string Id { get; set; }
        public string CategoryId { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public int SortOrder { get; set; }
    }
}
