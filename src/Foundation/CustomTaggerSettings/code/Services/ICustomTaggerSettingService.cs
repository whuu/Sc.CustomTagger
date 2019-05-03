using Sc.CustomTagger.Settings.Models;
using Sitecore.Data.Items;

namespace Sc.CustomTagger.Settings.Services
{
    public interface ICustomTaggerSettingService
    {
        CustomTaggerSettingModel GetCustomTaggerSettingModel(Item contentItem = null);
    }
}