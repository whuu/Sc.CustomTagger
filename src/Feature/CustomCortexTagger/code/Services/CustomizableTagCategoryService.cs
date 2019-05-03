using Newtonsoft.Json.Linq;
using Sc.CustomTagger.Models;
using Sc.CustomTagger.Settings.Models;
using Sitecore.ContentTagging.Core.Models;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;
using System.Collections.Generic;
using System.Linq;

namespace Sc.CustomTagger.Services
{
    public class CustomizableTagCategoryService : TagItemEntryService, ICustomizableTagCategoryService
    {
        private const string EntryKey = "JToken";
        private const string TypeGroupKey = "_typeGroup";
        private const string TypeKey = "_type";

        /// <summary>
        /// Search for categories in Open Calais format 
        /// </summary>
        /// <param name="tagData"></param>
        /// <returns></returns>
        public virtual TagsCategoriesModel FindCategoriesInTags(IEnumerable<TagData> tagData)
        {
            List<TagData> tagsWithoutCategories = new List<TagData>();
            Dictionary<string, List<TagData>> tagsCategories = new Dictionary<string, List<TagData>>();

            foreach (var tag in tagData)
            {
                if (!tag.Properties.Any(p =>
                {
                    if (p.Key != EntryKey || p.Value == null)
                    {
                        return false;
                    }

                    var jObject = p.Value as JObject;
                    if (jObject == null)
                    {
                        return false;
                    }

                    if (!jObject.ContainsKey(TypeGroupKey))
                    {
                        return false;
                    }

                    var categoryName = jObject.GetValue(TypeGroupKey).ToString();
                    if (jObject.ContainsKey(TypeKey))
                    {
                        categoryName = jObject.GetValue(TypeKey).ToString();
                    }
                    categoryName = string.Concat(categoryName.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');

                    if (!tagsCategories.ContainsKey(categoryName))
                    {
                        tagsCategories.Add(categoryName, new List<TagData>());
                    }

                    tagsCategories[categoryName].Add(tag);

                    return true;
                }))
                {
                    tagsWithoutCategories.Add(tag);
                }
            }
            return new TagsCategoriesModel { TagsCategories = tagsCategories, TagsWithoutCategories = tagsWithoutCategories };
        }

        public virtual Item CreateNewCategory(string categoryName, CustomTaggerSettingModel settings)
        {
            categoryName = RemoveDiacritics(categoryName);
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                return null;
            }
            using (new SecurityDisabler())
            {
                var categoryItem = settings.TagsCollectionRootItem.Add(ItemUtil.ProposeValidItemName(categoryName), settings.TagCategoryTemplate);
                categoryItem.Editing.BeginEdit();
                categoryItem.Fields[Sitecore.FieldIDs.DisplayName].Value = categoryName;
                if (!settings.TagCategoryValueFieldId.IsNull)
                {
                    categoryItem.Fields[settings.TagCategoryValueFieldId].Value = categoryName;
                }
                categoryItem.Editing.EndEdit();
                return categoryItem;
            }
        }

        public virtual Item GetCategory(string categoryName, List<Item> allCategories)
        {
            categoryName = RemoveDiacritics(categoryName);
            return allCategories.FirstOrDefault(i => i.Name == ItemUtil.ProposeValidItemName(categoryName));
        }

        public List<Item> AllCategories(CustomTaggerSettingModel settings)
        {
            var query = $"fast:{settings.TagsCollectionRootItem.Paths.Path}//*[@@templateid='{settings.TagCategoryTemplate.ID}']";
            return settings.TagsCollectionRootItem.Database.SelectItems(query).ToList();
        }
    }
}