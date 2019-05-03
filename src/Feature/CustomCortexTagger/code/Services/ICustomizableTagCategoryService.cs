using Sc.CustomTagger.Models;
using Sc.CustomTagger.Settings.Models;
using Sitecore.ContentTagging.Core.Models;
using Sitecore.Data.Items;
using System.Collections.Generic;

namespace Sc.CustomTagger.Services
{
    public interface ICustomizableTagCategoryService
    {
        Item CreateNewCategory(string categoryName, CustomTaggerSettingModel settings);

        TagsCategoriesModel FindCategoriesInTags(IEnumerable<TagData> tagData);

        List<Item> AllCategories(CustomTaggerSettingModel settings);

        Item GetCategory(string categoryName, List<Item> allCategories);
    }
}