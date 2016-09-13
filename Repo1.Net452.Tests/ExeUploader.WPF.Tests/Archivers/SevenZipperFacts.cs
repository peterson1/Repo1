using System;
using System.IO;
using FluentAssertions;
using Repo1.Net452.Tests.ExeUploader.WPF.Tests.Archivers.SampleFiles;
using Repo1.Net452.Tests.Helpers;
using Repo1.WPF452.SDK.Archivers;
using Repo1.WPF452.SDK.Helpers.FileInfoExtensions;
using Xunit;

namespace Repo1.Net452.Tests.ExeUploader.WPF.Tests.Archivers
{
    public class SevenZipperFacts
    {
        [Fact(DisplayName = "Single Volume: 4.6MB")]
        public async void Case1()
        {
            var origF   = SampleFinder.Get("sample_4.6mb");
            var oldHash = origF.SHA1ForFile();
            var outDir  = TempDir.New();

            var archives = await SevenZipper1.Compress(origF, outDir);
            archives.Should().HaveCount(1, "expect 1 part");
            File.Exists(archives[0]).Should().BeTrue();

            var list = await SevenZipper1.Decompress(archives[0], outDir);
            list.Should().HaveCount(1);

            var newF = Path.Combine(outDir, list[0]);
            var newHash = newF.SHA1ForFile();
            newHash.Should().Be(oldHash, "Hashes should match");

            Directory.Delete(outDir, true);
        }


        [Fact(DisplayName = "Multi-volume: 4 parts")]
        public async void Case2()
        {
            var origF   = SampleFinder.Get("sample_4.6mb");
            var oldHash = origF.SHA1ForFile();
            var outDir  = TempDir.New();

            var archives = await SevenZipper1.Compress(origF, outDir, 1);
            archives.Should().HaveCount(4, "expect 4 parts");
            File.Exists(archives[0]).Should().BeTrue();
            File.Exists(archives[1]).Should().BeTrue();
            File.Exists(archives[2]).Should().BeTrue();
            File.Exists(archives[3]).Should().BeTrue();

            var list    = await SevenZipper1.Decompress(archives[0], outDir);
            list.Should().HaveCount(1);

            var newF    = Path.Combine(outDir, list[0]);
            var newHash = newF.SHA1ForFile();
            newHash.Should().Be(oldHash, "Hashes should match");

            Directory.Delete(outDir, true);
        }


        [Fact(DisplayName = "Multi-volume: 1 big chunk")]
        public async void Case3()
        {
            var origF = SampleFinder.Get("sample_4.6mb");
            var oldHash = origF.SHA1ForFile();
            var outDir = TempDir.New();

            var archives = await SevenZipper1.Compress(origF, outDir, 5);
            archives.Should().HaveCount(1, "expect 1 part");
            File.Exists(archives[0]).Should().BeTrue();

            var list = await SevenZipper1.Decompress(archives[0], outDir);
            list.Should().HaveCount(1);

            var newF = Path.Combine(outDir, list[0]);
            var newHash = newF.SHA1ForFile();
            newHash.Should().Be(oldHash, "Hashes should match");

            Directory.Delete(outDir, true);
        }


        [Fact(DisplayName = "Multi-volume: 7 small chunks")]
        public async void Case4()
        {
            var origF = SampleFinder.Get("sample_4.6mb");
            var oldHash = origF.SHA1ForFile();
            var outDir = TempDir.New();

            var archives = await SevenZipper1.Compress(origF, outDir, 0.5);
            archives.Should().HaveCount(7, "expect 7 parts");
            File.Exists(archives[0]).Should().BeTrue();
            File.Exists(archives[1]).Should().BeTrue();
            File.Exists(archives[2]).Should().BeTrue();
            File.Exists(archives[3]).Should().BeTrue();
            File.Exists(archives[4]).Should().BeTrue();
            File.Exists(archives[5]).Should().BeTrue();
            File.Exists(archives[6]).Should().BeTrue();

            var list = await SevenZipper1.Decompress(archives[0], outDir);
            list.Should().HaveCount(1);

            var newF = Path.Combine(outDir, list[0]);
            var newHash = newF.SHA1ForFile();
            newHash.Should().Be(oldHash, "Hashes should match");

            Directory.Delete(outDir, true);
        }

    }
}
