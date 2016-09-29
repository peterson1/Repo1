using System.Linq;
using System.Reflection;
using Repo1.Core.ns12.Helpers.StringExtensions;
using System.Web;

namespace Repo1.WPF452.SDK.Helpers.HtmlTools
{
    public class HtmlDecoder
    {
        public static void ReplaceStrings<T>(T obj)
        {
            var props = typeof(T).GetRuntimeProperties()
                                 .Where(x => x.CanWrite &&
                                        x.PropertyType == typeof(string))
                                 .ToList();

            foreach (var prop in props)
            {
                var text = prop.GetValue(obj)?.ToString();
                if (text.IsBlank()) continue;
                var dcod = HttpUtility.HtmlDecode(text);
                prop.SetValue(obj, dcod);
            }
        }
    }
}
