using Sitecore.ContentTagging.Core.Models;
using Sitecore.ContentTagging.Core.Providers;
using System;
using Sitecore.Data.Items;
using Sitecore.Data.Fields;
using System.Text;
using Sc.CustomTagger.Settings.Services;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;

namespace Sc.CustomTagger.Providers
{
    /// <summary>
    /// Content provider for customizable tagger, which ignores custom tags field
    /// </summary>
    public class CustomizableContentProvider : IContentProvider<Item>
    {
        private readonly ICustomTaggerSettingService _tagsSettingService;

        public CustomizableContentProvider()
        {
            _tagsSettingService = ServiceProviderServiceExtensions.GetService<ICustomTaggerSettingService>(ServiceLocator.ServiceProvider);
        }

        public TaggableContent GetContent(Item source)
        {
            var stringContent = new StringContent();
            var stringBuilder = new StringBuilder();
            var settings = _tagsSettingService.GetCustomTaggerSettingModel(source);

            foreach (Field field in source.Fields)
            {
                if (!field.Name.StartsWith("__", StringComparison.InvariantCulture) && field.ID != settings.TagsFieldTargetId)
                {
                    stringBuilder.Append(field.Value);
                    if (stringBuilder.Length > 0)
                        stringBuilder.Append(" ");
                }
            }
            stringContent.Content = stringBuilder.ToString();
            return stringContent;
        }
    }
}