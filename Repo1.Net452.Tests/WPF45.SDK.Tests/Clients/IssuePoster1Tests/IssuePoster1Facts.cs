using System;
using FluentAssertions;
using Repo1.Core.ns11.Configuration;
using Repo1.WPF45.SDK.Clients;
using Xunit;

namespace Repo1.Net452.Tests.WPF45.SDK.Tests.Clients.IssuePoster1Tests
{
    public class IssuePoster1Facts
    {
        [Fact(DisplayName = "Can Post Issue")]
        public async void Case1()
        {
            var sut = new IssuePoster1(new RestServerCredentials
            {
                Username = "Justin D. Livingstone",
                Password = "KcGPFvSylL30AipGj6sd",
                ApiBaseURL = "https://repo1.nfshost.com/api1"
            });

            await sut.PostError("sample error msg", "");

            var last = await sut.LastPostedIssue();
            last.Should().NotBeNull();
            last.nid.Should().BeGreaterThan(0);
        }
    }
}
