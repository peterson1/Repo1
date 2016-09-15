using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Repo1.Core.ns12.Helpers.StringExtensions;
using Repo1.WPF452.SDK.FileChunkers;
using Repo1.WPF452.SDK.Helpers;
using SevenZip;

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
            return ExtractSoloArchive(oneBigF, targetDir);
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






        //private static List<string> GetPartsList(string outputFilePath)
        //{
        //    var list  = new List<string>();
        //    var dir   = Path.GetDirectoryName(outputFilePath);
        //    var fName = Path.GetFileName(outputFilePath);

        //    foreach (var file in Directory.GetFiles(dir, fName + "*"))
        //    {
        //        list.Add(file);
        //    }
        //    return list;
        //}


        private static SevenZipCompressor GetUltra7z2Compressor()
        {
            if (!SetLibraryPath(COMPRESSOR_LIB)) return null;

            var zpr               = new SevenZipCompressor();
            zpr.ArchiveFormat     = OutArchiveFormat.SevenZip;
            zpr.CompressionLevel  = CompressionLevel.Ultra;
            zpr.CompressionMethod = CompressionMethod.Lzma2;
            zpr.CompressionMode   = CompressionMode.Create;

            //if (maxVolumeSizeMB.HasValue)
            //    zpr.VolumeSize = Convert.ToInt32(1024 * 1024 * maxVolumeSizeMB);

            return zpr;
        }


        private static bool SetLibraryPath(string libFilename)
        {
            var bDir = AppDomain.CurrentDomain.BaseDirectory;
            var libF = Path.Combine(bDir, libFilename);

            if (!File.Exists(libF))
                throw new FileNotFoundException("Missing 7-zip library file.", libF);
            
            SevenZipCompressor.SetLibraryPath(libF);
            return true;
        }
    }
}
