using System;
using Sc.CustomTagger.Settings.Exceptions;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace Sc.CustomTagger.Settings.Models
{
    public class CustomTaggerSettingModel
    {
        private Database _contextDatabase;

        private readonly ID _tagsCollectionRootItemFieldId = new ID("{EECFD4D2-D64C-4214-8150-CFE9C491D779}");
        private readonly Guid _tagsCollectionRootItemId;

        private readonly ID _tagEntryTemplateFieldId = new ID("{5A4DE458-E557-4DE1-B185-FC5EF282E3C8}");
        private readonly Guid _tagEntryTemplateId;

        private readonly ID _tagEntryValueFieldFieldId = new ID("{6D18469E-2395-4735-8EB1-84C06E3CDCCA}");
        private readonly Guid _tagEntryValueFieldId;

        private readonly ID _tagsFieldTargetFieldId = new ID("{55DD19B5-9143-4CA1-9ABB-AA4826F60031}");
        private readonly Guid _tagsFieldTargetId;

        private readonly ID _tagCategoryTemplateFieldId = new ID("{FBA071CF-5C10-4E4B-9AC1-F57F7E63F2AB}");
        private readonly Guid _tagCategoryTemplateId;

        private readonly ID _tagCategoryValueFieldFieldlId = new ID("{CBCF7B8F-B928-4410-B014-0D28B45152D1}");
        private readonly Guid _tagCategoryValueFieldId;

        public Item TagsCollectionRootItem
        {
            get {
                var item = _contextDatabase.GetItem(new ID(_tagsCollectionRootItemId));
                if (item == null) {
                    throw new CustomTaggerSettingsException($"CustomTagger: tags root item with ID {_tagsCollectionRootItemId} not found");
                }
                return item;
            }
        }

        public ID TagCategoryValueFieldId
        {
            get
            {
                return new ID(_tagCategoryValueFieldId);
            }
        }

        public TemplateItem TagCategoryTemplate
        {
            get
            {
                var template = new TemplateItem(_contextDatabase.GetItem(new ID(_tagCategoryTemplateId)));
                if (template == null)
                {
                    throw new CustomTaggerSettingsException($"CustomTagger: category template item with ID {_tagCategoryTemplateId} for tags not found");
                }
                return template;
            }
        }

        public ID TagEntryValueFieldId
        {
            get
            {
                return new ID(_tagEntryValueFieldId);
            }
        }

        public TemplateItem TagEntryTemplate
        {
            get
            {
                var template = new TemplateItem(_contextDatabase.GetItem(new ID(_tagEntryTemplateId)));
                if (template == null)
                {
                    throw new CustomTaggerSettingsException($"CustomTagger: template item with ID {_tagEntryTemplateId} for tags not found");
                }
                return template;
            }
        }

        public ID TagsFieldTargetId
        {
            get
            {
                return new ID(_tagsFieldTargetId); 
            }
        }

        public CustomTaggerSettingModel(Item customTaggerSettingsItem)
        {
            Assert.IsNotNull(customTaggerSettingsItem, "customTaggerSettingsItem is null");
            _contextDatabase = customTaggerSettingsItem.Database;

            var tagsCollectionRootItemField = customTaggerSettingsItem.Fields[_tagsCollectionRootItemFieldId]?.GetValue(true);
            var tagEntryTemplateField = customTaggerSettingsItem.Fields[_tagEntryTemplateFieldId]?.GetValue(true);
            var tagEntryValueFieldField = customTaggerSettingsItem.Fields[_tagEntryValueFieldFieldId]?.GetValue(true);
            var tagsFieldTargetField = customTaggerSettingsItem.Fields[_tagsFieldTargetFieldId]?.GetValue(true);
            var tagCategoryTemplateField = customTaggerSettingsItem.Fields[_tagCategoryTemplateFieldId]?.GetValue(true);
            var tagCategoryValueFieldField = customTaggerSettingsItem.Fields[_tagCategoryValueFieldFieldlId]?.GetValue(true);
            
            Guid.TryParse(tagsCollectionRootItemField, out _tagsCollectionRootItemId);
            Guid.TryParse(tagEntryTemplateField, out _tagEntryTemplateId);
            Guid.TryParse(tagEntryValueFieldField, out _tagEntryValueFieldId);
            Guid.TryParse(tagsFieldTargetField, out _tagsFieldTargetId);
            Guid.TryParse(tagCategoryTemplateField, out _tagCategoryTemplateId);
            Guid.TryParse(tagCategoryValueFieldField, out _tagCategoryValueFieldId);
        }
    }
}