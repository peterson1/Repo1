using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repo1.Core.ns11.Extensions.StringExtensions;
using Repo1.Core.ns11.Obfuscators;
using Repo1.Core.ns11.R1Models.D8Models;

namespace Repo1.D8Tests.Lib45.TestTools
{
    public static class FakeFactoryExtensions
    {


        public static R1Package D8Package(this FakeFactory fke)
            => new R1Package
            {
                FileName      = fke.FileName,
                FileSize      = fke.Int(1000, 10000),
                LatestHash    = fke.Text.SHA1ForUTF8(),
                LatestVersion = fke.FileVersion,
            };


        public static R1PackagePart D8PackagePart(this FakeFactory fke)
        {
            var part            = new R1PackagePart();
            part.Package        = fke.D8Package();
            part.PackageVersion = fke.FileVersion;
            part.PartHash       = fke.Text.SHA1ForUTF8();
            part.TotalParts     = fke.Int(1, 15);
            part.PartNumber     = fke.Int(1, part.TotalParts);
            return part;
        }
    }
}
