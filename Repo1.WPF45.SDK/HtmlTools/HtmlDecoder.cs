using System.Linq;
using System.Reflection;
using System.Web;
using Repo1.Core.ns11.Extensions.StringExtensions;

namespace Repo1.WPF45.SDK.HtmlTools
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
