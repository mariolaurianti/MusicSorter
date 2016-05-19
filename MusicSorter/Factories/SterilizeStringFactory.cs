using System.Globalization;
using System.Text.RegularExpressions;
using MusicSorter.Factories.Interfaces;

namespace MusicSorter.Factories
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