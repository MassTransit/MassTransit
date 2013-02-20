// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.NewIdParsers
{
    using System;


    public class Base32Parser :
        INewIdParser
    {
        const string ConvertChars = "abcdefghijklmnopqrstuvwxyz234567ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        const string HexChars = "0123456789ABCDEF";

        readonly string _chars;

        public Base32Parser(string chars)
        {
            if (chars.Length % 32 != 0)
                throw new ArgumentException("The characters must be a multiple of 32");

            _chars = chars;
        }

        public Base32Parser()
        {
            _chars = ConvertChars;
        }

        public NewId Parse(string text)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("The string was null or empty", "text");

            var buffer = new char[32];

            int bufferOffset = 0;
            int offset = 0;
            long number;
            for (int i = 0; i < 6; ++i)
            {
                number = 0;
                for (int j = 0; j < 4; j++)
                {
                    int index = _chars.IndexOf(text[offset + j]);
                    if (index < 0)
                        throw new ArgumentException("Tracking number contains invalid characters");

                    number = number * 32 + (index % 32);
                }

                ConvertLongToBase16(buffer, bufferOffset, number, 5);

                offset += 4;
                bufferOffset += 5;
            }

            number = 0;
            for (int j = 0; j < 2; j++)
            {
                int index = _chars.IndexOf(text[offset + j]);
                if (index < 0)
                    throw new ArgumentException("Tracking number contains invalid characters");

                number = number * 32 + (index % 32);
            }
            ConvertLongToBase16(buffer, bufferOffset, number, 2);

            return new NewId(new string(buffer, 0, 32));
        }

        static void ConvertLongToBase16(char[] buffer, int offset, long value, int count)
        {
            for (int i = count - 1; i >= 0; i--)
            {
                var index = (int)(value % 16);
                buffer[offset + i] = HexChars[index];
                value /= 16;
            }
        }
    }
}