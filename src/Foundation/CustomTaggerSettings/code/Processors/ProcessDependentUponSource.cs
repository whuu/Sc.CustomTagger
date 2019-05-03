using Sitecore.Pipelines.GetLookupSourceItems;

namespace Sc.CustomTagger.Settings.Processors
{
    /// <summary>
    /// Dependent field handler
    /// </summary>
    public class ProcessDependentUponSource
    {
        private const string DependentUponTag = "@dependentupon";
        private const string TemplateFieldId = "{455A3E98-A627-4B40-8035-E683A0331AC7}";

        public void Process(GetLookupSourceItemsArgs args)
        {
            if (!args.Source.StartsWith(DependentUponTag))
            {
                return;
            }
            var fieldName = GetFieldName(args.Source);
            args.Source = GetDataSource(args.Item[fieldName]);
        }

        private string GetFieldName(string source)
        {
            var result = string.Empty;
            result = source.Replace(DependentUponTag, string.Empty);
            return result.Substring(2, result.Length - 4);
        }

        private string GetDataSource(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                return string.Empty;
            }
            return $"query://*[@@id='{itemId}']/*/*[@@templateid='{TemplateFieldId}']";
        }
    }
}