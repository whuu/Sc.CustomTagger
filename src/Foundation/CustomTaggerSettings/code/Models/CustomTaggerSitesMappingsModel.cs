using Sitecore;
using System.Collections.Generic;
using System.Xml;

namespace Sc.CustomTagger.Settings.Models
{
    /// <summary>
    /// List of settings for site definition from config file
    /// </summary>
    public class CustomTaggerSitesMappingsModel
    {
        private const string SiteNameAttribute = "name";

        private const string SettingsAttribute = "settingsItemPath";

        public IList<CustomTaggerSiteMappingModel> CustomTaggerSitesMappings { get; }

        public CustomTaggerSitesMappingsModel()
        {
            CustomTaggerSitesMappings = new List<CustomTaggerSiteMappingModel>();
        }

        public CustomTaggerSitesMappingsModel(IList<CustomTaggerSiteMappingModel> customTaggerSitesMappings)
        {
            CustomTaggerSitesMappings = customTaggerSitesMappings;
        }

        [UsedImplicitly]
        protected void AddCustomTaggerSitesMappings(XmlNode node)
        {
            if (node?.Attributes[SiteNameAttribute]?.Value == null || node?.Attributes[SettingsAttribute]?.Value == null)
                return;

            CustomTaggerSitesMappings.Add(new CustomTaggerSiteMappingModel()
            {
                Name = node.Attributes[SiteNameAttribute].Value,
                SettingsItemPath = node.Attributes[SettingsAttribute].Value
            });
        }
    }
}