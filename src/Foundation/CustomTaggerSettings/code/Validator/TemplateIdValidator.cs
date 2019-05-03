using Sitecore.Data;
using Sitecore.Data.Validators;
using Sitecore.Diagnostics;
using System;
using System.Runtime.Serialization;

namespace Sc.CustomTagger.Settings.Validator
{
    [Serializable]
    public class TemplateIdValidator : StandardValidator
    {
        public override string Name
        {
            get
            {
                return "TemplateIdValidator";
            }
        }
        
        public TemplateIdValidator(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        protected override ValidatorResult Evaluate()
        {
            var templateId = Parameters["TemplateId"];
            if (string.IsNullOrWhiteSpace(templateId))
            {
                return ValidatorResult.Valid;
            }

            var item = GetItem();
            if (item == null)
            {
                return ValidatorResult.Valid;
            }

            var controlValidationValue = ControlValidationValue;
            if (string.IsNullOrEmpty(controlValidationValue))
            {
                return ValidatorResult.Valid;
            }

            var fieldName = GetField().Title;
            if (!Guid.TryParse(controlValidationValue, out Guid value))
            {
                Text = GetText("Field \"{0}\" contains incorrect value", new string[]
                {
                    GetFieldDisplayName()
                });
                return ValidatorResult.FatalError;
            }

            var selectedItem = GetItem()?.Database?.GetItem(new ID(value));

            if (selectedItem != null && selectedItem.TemplateID == new ID(templateId))
            {
                return ValidatorResult.Valid;
            }

            Text = GetText("Field \"{0}\" contains incorrect value", new string[]
            {
                    GetFieldDisplayName()
            });
            return GetFailedResult(ValidatorResult.CriticalError);
        }

        protected override ValidatorResult GetMaxValidatorResult()
        {
            return GetFailedResult(ValidatorResult.CriticalError);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Assert.ArgumentNotNull(info, "info");
            info.AddValue("Name", this.Name);
            base.GetObjectData(info, context);
        }
    }
}