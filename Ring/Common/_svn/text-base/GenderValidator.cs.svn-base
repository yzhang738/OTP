using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OTP.Ring.Common
{
    [Serializable]
    public class GenderValidator : DataAnnotationsModelValidator<GenderAttribute>
    {
        string _message;

        public GenderValidator(ModelMetadata metadata, ControllerContext context, GenderAttribute attribute)
            : base(metadata, context, attribute)
        {
            _message = attribute.ErrorMessage;
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = _message,
                ValidationType = "Gender"
            };

            return new[] { rule };
        }
    }
}