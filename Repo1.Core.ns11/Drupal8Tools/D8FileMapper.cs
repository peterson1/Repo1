using System.Collections.Generic;

namespace Repo1.Core.ns11.Drupal8Tools
{
    public class D8FileMapper
    {
        public static Dictionary<string, object> Cast (string fileName, long fileSize, string base64Content, string baseUrl)
        {
            var dict = new Dictionary<string, object>();

            dict.Add("_links"  , D8HALJson.GetFileLinks(baseUrl));
            dict.Add("filename", Value(fileName));
            dict.Add("filemime", Value("application/octet-stream"));
            dict.Add("data"    , Value(base64Content));
            //dict.Add("type"    , D8HALJson.TargetIdField("file"));
            //dict.Add("filesize", Value(fileSize));
            //dict.Add("uri"    , Value($"public://test/{fileName}"));

            return dict;
        }


        private static List<Dictionary<string, object>> Value<T>(T value)
            => D8HALJson.ValueField(value);
    }
}
