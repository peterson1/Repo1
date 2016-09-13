using System;
using System.IO;

namespace Repo1.Net452.Tests.Helpers
{
    public class TempDir
    {
        const string DIR_PREFIX = "Test_";

        public static string New()
            => Path.Combine(Path.GetTempPath(), 
                typeof(TempDir).Namespace,
                DIR_PREFIX + DateTime.Now.Ticks);
    }
}
