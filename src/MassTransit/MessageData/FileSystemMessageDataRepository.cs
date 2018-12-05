// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.MessageData
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Util;


    public class FileSystemMessageDataRepository :
        IMessageDataRepository
    {
        const int DefaultBufferSize = 4096;
        static readonly char[] _separator = {':'};
        readonly DirectoryInfo _dataDirectory;

        public FileSystemMessageDataRepository(DirectoryInfo dataDirectory)
        {
            _dataDirectory = dataDirectory;
        }

        async Task<Stream> IMessageDataRepository.Get(Uri address, CancellationToken cancellationToken)
        {
            string filePath = ParseFilePath(address);

            string fullPath = Path.Combine(_dataDirectory.FullName, filePath);
            if (!File.Exists(fullPath))
                throw new FileNotFoundException("The file was not found", fullPath);

            using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, DefaultBufferSize, FileOptions.Asynchronous))
            {
                var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream, DefaultBufferSize, cancellationToken).ConfigureAwait(false);
                
                memoryStream.Position = 0;
                return memoryStream;
            }
        }

        async Task<Uri> IMessageDataRepository.Put(Stream stream, TimeSpan? timeToLive, CancellationToken cancellationToken)
        {
            string filePath = GenerateFilePath(timeToLive);

            string fullPath = Path.Combine(_dataDirectory.FullName, filePath);

            VerifyDirectory(fullPath);

            using (var fileStream = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.Read, DefaultBufferSize, FileOptions.Asynchronous))
            {
                await stream.CopyToAsync(fileStream, DefaultBufferSize, cancellationToken).ConfigureAwait(false);
            }

            return new Uri($"urn:file:{filePath.Replace(Path.DirectorySeparatorChar, ':')}");
        }

        static void VerifyDirectory(string fullPath)
        {
            string directoryName = Path.GetDirectoryName(fullPath);
            if (string.IsNullOrEmpty(directoryName))
                throw new DirectoryNotFoundException("No directory was found for the file path: " + fullPath);

            Directory.CreateDirectory(directoryName);
        }

        static string GenerateFilePath(TimeSpan? timeToLive)
        {
            string fileId = FormatUtil.Formatter.Format(NewId.Next().ToSequentialGuid().ToByteArray());

            string expiration = timeToLive.HasValue && timeToLive.Value < TimeSpan.MaxValue && timeToLive >= TimeSpan.Zero
                ? (DateTime.UtcNow + timeToLive.Value).ToString("yyyy-MM-dd-HH").Replace('-', Path.DirectorySeparatorChar)
                : "none";

            return Path.Combine(expiration, fileId);
        }

        static string ParseFilePath(Uri address)
        {
            if (address.Scheme != "urn")
                throw new ArgumentException("The address must be a urn");

            string[] parts = address.Segments[0].Split(_separator);
            if (parts[0] != "file")
                throw new ArgumentException("The address must be a urn:file");

            int length = parts.Length - 1;
            var elements = new string[length];
            Array.Copy(parts, 1, elements, 0, length);

            return Path.Combine(elements);
        }
    }
}