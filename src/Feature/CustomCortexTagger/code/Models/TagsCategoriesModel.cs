using Sitecore.ContentTagging.Core.Models;
using System.Collections.Generic;

namespace Sc.CustomTagger.Models
{
    public class TagsCategoriesModel
    {
        public List<TagData> TagsWithoutCategories { get; set; }

        public Dictionary<string, List<TagData>> TagsCategories { get; set; }
    }
}