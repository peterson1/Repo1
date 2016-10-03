using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Repo1.Core.ns11.Extensions.StringExtensions;
using Repo1.WPF452.SDK.FileChunkers;
using Repo1.WPF452.SDK.Helpers;
using Repo1.WPF452.SDK.Helpers.EmbeddedResourceHelpers;
using SevenZip;
using static System.Environment;

namespace Repo1.WPF452.SDK.Archivers
{
    public class SevenZipper1
    {
        const string COMPRESSOR_LIB = "7za.dll";
        const string EXTRACTOR_LIB  = "7zxa.dll";



        public static Task<List<string>> DecompressMultiPart(IEnumerable<string> orderedParts, string targetDir)
        {
            var oneBigF = Path.Combine(targetDir, $"OneBigFile_{DateTime.Now.Ticks}.merged");
            FileChunker1.WriteOneBigFile(oneBigF, orderedParts);

            foreach (var part in orderedParts)
                SilentDelete(part);

            var list = ExtractSoloArchive(oneBigF, targetDir);

            SilentDelete(oneBigF);

            return list;
        }


        private static void SilentDelete(string filePath)
        {
            try { File.Delete(filePath); }
            catch { }
        }


        public static Task<List<string>> Decompress(string archivePath, string targetDir)
        {
            if (FileChunker1.IsPartOfMany(archivePath))
                archivePath = FileChunker1.Merge(archivePath);

            return ExtractSoloArchive(archivePath, targetDir);
        }


        private static async Task<List<string>> ExtractSoloArchive(string archivePath, string targetDir)
        {
            if (!SetLibraryPath(EXTRACTOR_LIB)) return null;

            var tcs = new TaskCompletionSource<List<string>>();
            var zpr = new SevenZipExtractor(archivePath);
            var list = new List<string>();

            zpr.FileExtractionFinished += (s, e)
                => list.Add(e.FileInfo.FileName);

            zpr.ExtractionFinished += (s, e) => tcs.SetResult(list);

            zpr.ExtractArchive(targetDir);

            var contents = await tcs.Task;

            if (contents == null   ) return Alerter.Warn("Content list is NULL.");
            if (contents.Count == 0) return Alerter.Warn("Archive did not contain any file.");

            return contents;
        }


        public static Task<List<string>> Compress(string  filePath, 
                                                  string  targetFDir = null, 
                                                  double? maxVolumeSizeMB = null, 
                                                  string  extension = ".7z")
        {
            var tcs  = new TaskCompletionSource<List<string>>();
            var zpr  = GetUltra7z2Compressor();
            var outF = "";

            if (targetFDir.IsBlank())
            {
                targetFDir = Path.GetDirectoryName(filePath);
                outF = filePath + extension;
            }
            else
            {
                Directory.CreateDirectory(targetFDir);
                outF = Path.Combine(targetFDir, Path.GetFileName(filePath) + extension);
            }

            zpr.CompressionFinished += (s, e) =>
            {
                if (maxVolumeSizeMB.HasValue)
                {
                    var parts = FileChunker1.Split(outF, targetFDir, maxVolumeSizeMB.Value);
                    tcs.SetResult(parts);
                }
                else
                    tcs.SetResult(new List<string> { outF });
            };

            zpr.BeginCompressFiles(outF, filePath);

            return tcs.Task;
        }




        private static SevenZipCompressor GetUltra7z2Compressor()
        {
            var bDir = GetLocalBinariesDir();
            var libF = Path.Combine(bDir, COMPRESSOR_LIB);
            SevenZipCompressor.SetLibraryPath(libF);

            var zpr               = new SevenZipCompressor();
            zpr.ArchiveFormat     = OutArchiveFormat.SevenZip;
            zpr.CompressionLevel  = CompressionLevel.Ultra;
            zpr.CompressionMethod = CompressionMethod.Lzma2;
            zpr.CompressionMode   = CompressionMode.Create;

            return zpr;
        }


        private static bool SetLibraryPath(string libFilename)
        {
            var bDir = GetLocalBinariesDir();
            var libF = Path.Combine(bDir, libFilename);

            if (!File.Exists(libF))
                EmbeddedResrc.ExtractToFile<SevenZipper1>
                    (libFilename, "Archivers", bDir);

            //  To fix "The path is not of a legal form" error:
            //    -  add tag in FodyWeavers.xml
            //    -  <Costura CreateTemporaryAssemblies='true' />


            //  To fix "Can not load 7-zip library or internal COM error! Message: failed to load library."
            //    = proj. properties > Build > Prefer 32-bit

            SevenZipCompressor.SetLibraryPath(libF);

            return true;
        }


        public static string GetLocalBinariesDir()
        {
            var appData = GetFolderPath(SpecialFolder.LocalApplicationData);
            return Path.Combine(appData, typeof(SevenZipper1).Name);
        }
    }
}
