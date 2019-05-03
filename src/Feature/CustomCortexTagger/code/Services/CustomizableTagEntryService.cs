using Sc.CustomTagger.Settings.Models;
using System.Linq;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;
using Sitecore.ContentTagging.Core.Models;
using System.Collections.Generic;

namespace Sc.CustomTagger.Services
{
    public class CustomizableTagEntryService : TagItemEntryService, ICustomizableTagEntryService
    {
        public virtual Item AddTag(TagData data, Item parentItem, CustomTaggerSettingModel settings, List<Item> allTags)
        {
            var existingTag = GetTag(data.TagName, settings, allTags, parentItem);
            if (existingTag != null)
            {
                return existingTag;
            }
            return CreateNewTag(data, parentItem, settings);
        }

        public virtual Item GetTag(string tagName, CustomTaggerSettingModel settings, List<Item> allTags, Item parentItem = null)
        {
            tagName = RemoveDiacritics(tagName);
            if (parentItem == null)
            {
                return allTags.FirstOrDefault(i => i.Name == ItemUtil.ProposeValidItemName(tagName));
            }
            return allTags.FirstOrDefault(i => i.Name == ItemUtil.ProposeValidItemName(tagName) && i.ParentID == parentItem.ID);
        }

        public List<Item> AllTags(CustomTaggerSettingModel settings)
        {
            var query = $"fast:{settings.TagsCollectionRootItem.Paths.Path}//*[@@templateid='{settings.TagEntryTemplate.ID}']";
            return settings.TagsCollectionRootItem.Database.SelectItems(query).ToList();
        }

        protected virtual Item CreateNewTag(TagData data, Item parentItem, CustomTaggerSettingModel settings)
        {
            string tagName = RemoveDiacritics(data.TagName);
            string itemName = ItemUtil.ProposeValidItemName(RemoveDiacritics(data.TagName), "tag");
            using (new SecurityDisabler())
            {
                var tagItem = parentItem.Add(itemName, settings.TagEntryTemplate);
                tagItem.Editing.BeginEdit();
                tagItem.Fields[Sitecore.FieldIDs.DisplayName].Value = tagName;
                if (!settings.TagEntryValueFieldId.IsNull)
                {
                    tagItem.Fields[settings.TagEntryValueFieldId].Value = tagName;
                }
                tagItem.Editing.EndEdit();
                return tagItem;
            }
        }
    }
}