using Microsoft.Extensions.DependencyInjection;
using Sc.CustomTagger.Settings.Exceptions;
using Sc.CustomTagger.Settings.Services;
using Sitecore.ContentTagging.Core.Models;
using Sitecore.ContentTagging.Core.Providers;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace Sc.CustomTagger.Providers
{
    /// <summary>
    /// Content Tagger store tags incustomizable item's field
    /// </summary>
    public class CustomizableTagger : ITagger<Item>
    {
        private readonly ICustomTaggerSettingService _settingsService;

        public CustomizableTagger()
        {
            _settingsService = ServiceProviderServiceExtensions.GetService<ICustomTaggerSettingService>(ServiceLocator.ServiceProvider);
        }

        /// <summary>
        /// Store tags in the content item
        /// </summary>
        /// <param name="contentItem"></param>
        /// <param name="tags"></param>
        public void TagContent(Item contentItem, IEnumerable<Tag> tags)
        {
            var tagsFieldId = _settingsService.GetCustomTaggerSettingModel(contentItem).TagsFieldTargetId;
            var tagsField = contentItem.Fields[tagsFieldId];
            if (tagsField == null || string.IsNullOrEmpty(tagsField.Type))
            {
                throw new CustomTaggerSettingsException($"CustomTagger: Field {tagsFieldId} not found or wrong type in item {contentItem.ID}");
            }

            var tagsEditField = (MultilistField)tagsField;
            contentItem.Editing.BeginEdit();
            foreach (var tag in tags)
            {
                if (ID.TryParse(tag.ID, out ID id) && !tagsEditField.TargetIDs.Contains(id))
                {
                    tagsEditField.Add(tag.ID);
                }
            }
            contentItem.Editing.EndEdit();
        }
    }
}