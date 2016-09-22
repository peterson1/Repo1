using ServiceStack.Text;

namespace Repo1.WPF452.SDK.Helpers.Serialization
{
    public class Json
    {
        public static T Deserialize<T>(string json)
            => JsonSerializer.DeserializeFromString<T>(json);


        public static string Serialize<T>(T obj)
            => JsonSerializer.SerializeToString(obj);
    }
}
