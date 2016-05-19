using System.Globalization;
using System.Text.RegularExpressions;

namespace MusicSorter
{
    public class SterilizeStringFactory : ISterilizeStringFactory
    {
        public string CleanString(string property)
        {
            if (string.IsNullOrEmpty(property)) return string.Empty;

            var regex = new Regex("[^a-zA-Z0-9 ]").Replace(property, string.Empty).Trim();
            regex = regex.Replace(".", string.Empty);
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(regex);
        }
    }
}