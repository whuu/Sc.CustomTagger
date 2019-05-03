using Sc.CustomTagger.Settings.Models;
using Sitecore.Data.Items;
using Sitecore.ContentTagging.Core.Models;
using System.Collections.Generic;

namespace Sc.CustomTagger.Services
{
    public interface ICustomizableTagEntryService
    {
        Item AddTag(TagData data, Item parentItem, CustomTaggerSettingModel settings, List<Item> allTags);

        List<Item> AllTags(CustomTaggerSettingModel settings);

        Item GetTag(string tagName, CustomTaggerSettingModel settings, List<Item> allTags, Item parentItem = null);
    }
}