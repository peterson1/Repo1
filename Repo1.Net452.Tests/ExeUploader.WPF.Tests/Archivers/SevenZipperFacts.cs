using System;
using System.Collections.Generic;
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
            archives.MustHaveFiles(1);

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
            archives.MustHaveFiles(4);

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
            archives.MustHaveFiles(1);

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
            archives.MustHaveFiles(7);

            var list = await SevenZipper1.Decompress(archives[0], outDir);
            list.Should().HaveCount(1);

            var newF = Path.Combine(outDir, list[0]);
            var newHash = newF.SHA1ForFile();
            newHash.Should().Be(oldHash, "Hashes should match");

            Directory.Delete(outDir, true);
        }


        [Fact(DisplayName = "Multi-volume: Pre-ordered list")]
        public async void Case5()
        {
            var origF = SampleFinder.Get("sample_4.6mb");
            var oldHash = origF.SHA1ForFile();
            var outDir = TempDir.New();

            var archives = await SevenZipper1.Compress(origF, outDir, 0.5);
            archives.MustHaveFiles(7);

            var list = await SevenZipper1.DecompressMultiPart(archives, outDir);
            list.Should().HaveCount(1);

            var newF = Path.Combine(outDir, list[0]);
            var newHash = newF.SHA1ForFile();
            newHash.Should().Be(oldHash, "Hashes should match");

            Directory.Delete(outDir, true);
        }

    }


    internal static class SevenZipperFactsExtensions
    {
        internal static void MustHaveFiles(this List<string> list, int expctdCount)
        {
            list.Should().HaveCount(expctdCount, $"expect {expctdCount} parts");

            foreach (var file in list)
                File.Exists(file).Should().BeTrue();
        }
    }
}
