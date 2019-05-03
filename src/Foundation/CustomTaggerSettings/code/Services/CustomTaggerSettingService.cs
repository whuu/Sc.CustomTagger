using Sc.CustomTagger.Settings.Models;
using Sitecore.Data.Items;
using Sitecore.Data;
using System.Linq;
using Sitecore.Web;
using System;

namespace Sc.CustomTagger.Settings.Services
{
    /// <summary>
    /// Custom Tagger Settings item service
    /// </summary>
    public class CustomTaggerSettingService : ICustomTaggerSettingService
    {
        private readonly ID _defaultCustomTaggerSettingsItemId = new ID("{82239F2F-D096-4DB4-A6B5-776B210D47F9}");

        private Database Database
        {
            get
            {
                return Sitecore.Context.ContentDatabase ?? Sitecore.Context.Database;
            }
        }

        /// <summary>
        /// Get custom tagger settings from configuration or default item.
        /// It possbile to configure path to this item for each website in module configuration.
        /// Check Sc.CustomTagger.Settings.config for details
        /// </summary>
        /// <param name="contentItem"></param>
        /// <returns></returns>
        public CustomTaggerSettingModel GetCustomTaggerSettingModel(Item contentItem = null)
        {
            var settingsItem = GetSettingsForContentItem(contentItem);
            return new CustomTaggerSettingModel(settingsItem);
        }

        protected virtual Item GetSettingsForContentItem(Item contentItem)
        {
            Item customTaggerSettingsItem = null;
            var siteName = GetSite(contentItem)?.Name;

            if (!string.IsNullOrWhiteSpace(siteName))
            {
                var xmlNode = Sitecore.Configuration.Factory.GetConfigNode("customTagger");
                var sitesMappings = Sitecore.Configuration.Factory.CreateObject<CustomTaggerSitesMappingsModel>(xmlNode);
                var site = sitesMappings.CustomTaggerSitesMappings.FirstOrDefault(m => m.Name.Equals(siteName));

                if (site != null && !string.IsNullOrWhiteSpace(site.SettingsItemPath))
                {
                    customTaggerSettingsItem = Database.GetItem(site.SettingsItemPath);
                }
            }

            if (customTaggerSettingsItem == null)
            {
                customTaggerSettingsItem = Database.GetItem(_defaultCustomTaggerSettingsItemId);
            }
            return customTaggerSettingsItem;
        }

        protected virtual SiteInfo GetSite(Item item)
        {
            var siteInfoList = Sitecore.Configuration.Factory.GetSiteInfoList();
            SiteInfo currentSiteinfo = null;
            var matchLength = 0;
            foreach (var siteInfo in siteInfoList)
            {
                if (siteInfo.Database == "core" || siteInfo.Name == "modules_website")
                {
                    continue;
                }
                if (item.Paths.FullPath.StartsWith(siteInfo.RootPath, StringComparison.OrdinalIgnoreCase) && siteInfo.RootPath.Length > matchLength)
                {
                    matchLength = siteInfo.RootPath.Length;
                    currentSiteinfo = siteInfo;
                }
            }
            return currentSiteinfo;
        }
    }
}