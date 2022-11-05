using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTP.Ring.Common
{
    [Serializable]
    public class Organization
    {
        public class Comparer : IComparer<Organization>
        {
            private string _currentOrganization;

            public Comparer(string organizationId)
            {
                this._currentOrganization = organizationId;
            }

            public int Compare(Organization x, Organization y)
            {
                if (x.Id == this._currentOrganization && y.Id == this._currentOrganization) return 0;
                if (x.Id == this._currentOrganization) return -1;
                if (y.Id == this._currentOrganization) return 1;

                return (x.Type + x.Name).CompareTo(y.Type + y.Name);
            }
        }

        [LocalizedDisplayName(typeof(CommonLocalization), "Organization_Title_Id")]
        public string Id { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Organization_Title_Type")]
        public string Type { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Organization_Title_Name")]
        public string Name { get; set; }
    }
}
