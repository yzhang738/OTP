using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace OTP.Ring.Common
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event)]
    public class LocalizedDisplayNameAttribute : DisplayNameAttribute
    {
        private readonly string _resourceKey;
        private readonly Type _resourceManagerProvider;

        public LocalizedDisplayNameAttribute(Type resourceManagerProvider, string resourceKey)
            : base()
        {
            this._resourceKey = resourceKey;
            this._resourceManagerProvider = resourceManagerProvider;
        }

        public override string DisplayName
        {
            get
            {
                var property = this._resourceManagerProvider.GetProperty(this._resourceKey, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);

                if (property != null)
                {
                    return property.GetValue(this._resourceManagerProvider, null).ToString();
                }
                else
                {
                    // Fallback with the key name
                    return this._resourceKey;
                }
            }
        }
    }
}
