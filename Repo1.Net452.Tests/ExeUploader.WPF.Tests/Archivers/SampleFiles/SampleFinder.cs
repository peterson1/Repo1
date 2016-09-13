using System;
using System.IO;
using FluentAssertions;

namespace Repo1.Net452.Tests.ExeUploader.WPF.Tests.Archivers.SampleFiles
{
    class SampleFinder
    {
        const string SUB_DIR = @"ExeUploader.WPF.Tests\Archivers\SampleFiles";

        internal static string Get(string fileName)
        {
            var path = Path.Combine(SamplesDir, fileName);
            File.Exists(path).Should().BeTrue("File should exist: {0}", path);
            return path;
        }

        private static string SamplesDir 
            => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SUB_DIR);
    }
}
