﻿using Repo1.Core.ns11.R1Clients;
using Xunit.Abstractions;

namespace Repo1.D8Tests.Lib45.TestTools
{
    static class RestClientExtensions
    {
        public static void MakeTestable(this IRestClient client, ITestOutputHelper testOutputHelper)
        {
            client.OnWarning  = s => testOutputHelper.WriteLine(s);
            client.OnError    = e => { throw e; };
            client.MaxRetries = 0;
        }
    }
}