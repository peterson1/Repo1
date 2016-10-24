using System.Collections.Generic;

namespace Repo1.WPF45.SDK.Cryptographers
{
    public class Certificator
    {
        private static List<string> _whiteList = new List<string>();

        public static void AllowFrom(params string[] serverThumbPrints)
        {
            _whiteList.AddRange(serverThumbPrints);

            //todo: continue here...
        }
    }
}
