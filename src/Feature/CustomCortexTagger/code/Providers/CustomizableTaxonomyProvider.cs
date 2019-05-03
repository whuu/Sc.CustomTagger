using Microsoft.Extensions.DependencyInjection;
using Sc.CustomTagger.Services;
using Sc.CustomTagger.Settings.Exceptions;
using Sc.CustomTagger.Settings.Services;
using Sitecore.ContentTagging.Core.Models;
using Sitecore.ContentTagging.Core.Providers;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace Sc.CustomTagger.Providers
{
    /// <summary>
    /// Taxonomy provider for tags with customizable structure
    /// </summary>
    public class CustomizableTaxonomyProvider : ITaxonomyProvider
    {
        private readonly ICustomTaggerSettingService _tagsSettingService;

        private readonly ICustomizableTagEntryService _tagsEntryService;

        private readonly ICustomizableTagCategoryService _tagsCategoryService;

        public string ProviderId => nameof(DefaultTaxonomyProvider);

        public CustomizableTaxonomyProvider()
        {
            _tagsSettingService = ServiceProviderServiceExtensions.GetService<ICustomTaggerSettingService>(ServiceLocator.ServiceProvider);
            _tagsCategoryService = ServiceProviderServiceExtensions.GetService<ICustomizableTagCategoryService>(ServiceLocator.ServiceProvider);
            _tagsEntryService = ServiceProviderServiceExtensions.GetService<ICustomizableTagEntryService>(ServiceLocator.ServiceProvider);
        }

        public IEnumerable<Tag> CreateTags(IEnumerable<TagData> tagData)
        {
            return CreateTags(null, tagData);
        }

        public Tag GetTag(string tagName)
        {
            var settings = _tagsSettingService.GetCustomTaggerSettingModel();
            var existingTags = _tagsEntryService.AllTags(settings);
            var item = _tagsEntryService.GetTag(tagName, settings, existingTags);
            return item == null ? null : new Tag
            {
                TagName = item.Name,
                ID = item.ID.ToString(),
                TaxonomyProviderId = ProviderId
            };
        }

        public IEnumerable<Tag> CreateTags(Item contentItem, IEnumerable<TagData> tagData)
        {
            var settings = _tagsSettingService.GetCustomTaggerSettingModel(contentItem);
            if (!settings.TagEntryValueFieldId.IsNull && !settings.TagEntryTemplate.Fields.Select(x => x.ID).Contains(settings.TagEntryValueFieldId))
            {
                throw new CustomTaggerSettingsException($"CustomTagger: template {settings.TagEntryTemplate.ID} doesn't contain field with ID {settings.TagEntryValueFieldId}");
            }
            if (!settings.TagCategoryValueFieldId.IsNull && !settings.TagCategoryTemplate.Fields.Select(x => x.ID).Contains(settings.TagCategoryValueFieldId))
            {
                throw new CustomTaggerSettingsException($"CustomTagger: category template {settings.TagCategoryTemplate.ID} doesn't contain field with ID {settings.TagCategoryValueFieldId}");
            }

            var tagsCategorized = _tagsCategoryService.FindCategoriesInTags(tagData);
            List<Tag> tagsToAssign = new List<Tag>();

            var existingCategories = _tagsCategoryService.AllCategories(settings);
            var existingTagItems = _tagsEntryService.AllTags(settings);

            foreach (var tagCategory in tagsCategorized.TagsCategories)
            {
                var categoryItem = _tagsCategoryService.GetCategory(tagCategory.Key, existingCategories);
                if (categoryItem == null)
                {
                    //add category
                    categoryItem = _tagsCategoryService.CreateNewCategory(tagCategory.Key, settings);
                    if (categoryItem == null)
                    {
                        tagsCategorized.TagsWithoutCategories.AddRange(tagCategory.Value);
                        continue;
                    }
                    else
                    {
                        existingCategories.Add(categoryItem);
                    }
                }
                foreach (var tagInCategory in tagCategory.Value)
                {
                    //add tags to category
                    var tag = _tagsEntryService.AddTag(tagInCategory, categoryItem, settings, existingTagItems);
                    if (tag != null)
                    {
                        existingTagItems.Add(tag);
                        tagsToAssign.Add(new Tag()
                        {
                            TagName = tag.Name,
                            ID = tag.ID.ToString(),
                            TaxonomyProviderId = ProviderId,
                            Data = tagInCategory
                        });
                    }
                }
            }
            foreach (var tagWithoutCategory in tagsCategorized.TagsWithoutCategories)
            {
                //add tags without category
                var tag = _tagsEntryService.AddTag(tagWithoutCategory, settings.TagsCollectionRootItem, settings, existingTagItems);
                if (tag != null)
                {
                    existingTagItems.Add(tag);
                    tagsToAssign.Add(new Tag()
                    {
                        TagName = tag.Name,
                        ID = tag.ID.ToString(),
                        TaxonomyProviderId = ProviderId,
                        Data = tagWithoutCategory
                    });
                }
            }
            return tagsToAssign;
        }
        
        public Tag GetParent(string tagId)
        {
            return null;
        }

        public IEnumerable<Tag> GetChildren(string tagId)
        {
            return null;
        }
    }
}