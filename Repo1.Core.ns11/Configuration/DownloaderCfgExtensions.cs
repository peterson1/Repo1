﻿using Repo1.Core.ns11.Extensions.StringExtensions;

namespace Repo1.Core.ns11.Configuration
{
    public static class DownloaderCfgExtensions
    {
        public static string GetLicenseKey(this DownloaderCfg config,
            string macAddress) => (macAddress
                                 + config.ActivationKey).SHA1ForUTF8();

    }
}
