using Sitecore.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Sc.CustomTagger.Services
{
    public abstract class TagItemEntryService
    {
        protected static Database Database => Sitecore.Context.ContentDatabase ?? Sitecore.Context.Database;

        protected virtual string RemoveDiacritics(string s)
        {
            var name = Encoding.ASCII.GetString(Encoding.GetEncoding(1251).GetBytes(s));
            name = Regex.Replace(name, "[^a-zA-Z0-9_ .]+", "", RegexOptions.Compiled).Replace("_", " ");
            return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(name.ToLower());
        }
    }
}