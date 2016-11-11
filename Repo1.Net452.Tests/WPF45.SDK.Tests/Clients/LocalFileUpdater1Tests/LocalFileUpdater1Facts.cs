using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Repo1.Core.ns11.Configuration;
using Repo1.Core.ns11.R1Models;
using Repo1.WPF45.SDK.Clients;
using Xunit;

namespace Repo1.Net452.Tests.WPF45.SDK.Tests.Clients.LocalFileUpdater1Tests
{
    public class LocalFileUpdater1Facts
    {
        [Fact(DisplayName = "Can Update Ping Node")]
        public async void Case1()
        {
            var sut = new LocalFileUpdater1(new RestServerCredentials
            {
                Username   = "Justin D. Livingstone",
                Password   = "KcGPFvSylL30AipGj6sd",
                ApiBaseURL = "https://repo1.nfshost.com/api1"
            }, "");

            sut.PingNode = new R1Ping
            {

            };

            var r1Exe = await sut.GetLatestVersions();

            r1Exe.Should().NotBeNull();
        }
    }
}
