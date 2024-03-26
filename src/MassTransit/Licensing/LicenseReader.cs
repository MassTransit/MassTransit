namespace MassTransit.Licensing
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text.Json;


    public class LicenseReader
    {
        const string _ =
            @"MIGbMBAGByqGSM49AgEGBSuBBAAjA4GGAAQA7sH7I9oVjLeDpPaHZCGBQi3Je/NlYYu96gkRipnrplCuox3pMDR0NljeGA35xjsJg7mm6r67/zZAt9GeAIftOnYAc4oaWRGbpaC6O3j/2i+v96gzk21xDh68OTEHLS4J720x/0pd6yvXlZPvGEeyHQgIKoQE11WmPYAP5nXZTJn6KwM=";

        public static LicenseInfo LoadFromFile(string path)
        {
            using var stream = File.OpenText(path);
            var license = stream.ReadToEnd();

            return Load(license);
        }

        public static LicenseInfo Load(string license)
        {
            var payload = ExtractPayload(license);

            var file = JsonSerializer.Deserialize<LicenseFile>(payload, LicenseSettings.SerializerOptions);
            if (file == null)
                throw new InvalidLicenseFormatException();

            if (string.IsNullOrWhiteSpace(file.Kind) || !string.Equals(file.Kind, "License"))
                throw new InvalidLicenseFormatException();
            if (string.IsNullOrWhiteSpace(file.Version) || !string.Equals(file.Version, "v1"))
                throw new InvalidLicenseFormatException();
            if (string.IsNullOrWhiteSpace(file.Data))
                throw new InvalidLicenseFormatException();
            if (string.IsNullOrWhiteSpace(file.Signature))
                throw new InvalidLicenseFormatException();

            var signature = Convert.FromBase64String(file.Signature);

            var kb = Convert.FromBase64String(_);

            using var verify = ECDsa.Create(new ECParameters
            {
                Curve = ECCurve.NamedCurves.nistP521,
                Q = new ECPoint
                {
                    X = kb.Skip(26).Take(66).ToArray(),
                    Y = kb.Skip(92).Take(66).ToArray()
                }
            });

            file.Signature = null;
            var serialize = JsonSerializer.SerializeToUtf8Bytes(file, LicenseSettings.SerializerOptions);

            if (verify.VerifyData(serialize, signature, HashAlgorithmName.SHA256) == false)
                throw new InvalidLicenseException("Invalid signature");

            var bytes = Convert.FromBase64String(file.Data);

            var licenseInfo = JsonSerializer.Deserialize<LicenseInfo>(bytes, LicenseSettings.SerializerOptions);
            if (licenseInfo == null)
                throw new InvalidLicenseException();

            return licenseInfo;
        }

        static byte[] ExtractPayload(string text)
        {
            return Convert.FromBase64String(string.Concat(text.Split('\n')
                .Select(x => x.Trim().Trim('\r'))
                .Where(x => !x.StartsWith("-----"))));
        }
    }
}
