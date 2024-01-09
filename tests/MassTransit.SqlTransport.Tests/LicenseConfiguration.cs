#nullable enable
namespace MassTransit.DbTransport.Tests;

using System;
using NUnit.Framework;


static class LicenseConfiguration
{
    const string LicensePathKey = "MT_LICENSE_PATH";

    public static string? LicensePath =>
        TestContext.Parameters.Exists(LicensePathKey)
            ? TestContext.Parameters.Get(LicensePathKey)
            : Environment.GetEnvironmentVariable(LicensePathKey);
}