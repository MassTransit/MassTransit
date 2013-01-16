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
namespace MassTransit.NewIdFormatters
{
    using System;


    public class Base32Formatter :
        INewIdFormatter
    {
        const string LowerCaseChars = "abcdefghijklmnopqrstuvwxyz234567";
        const string UpperCaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        string _chars;

        public Base32Formatter(bool upperCase = false)
        {
            _chars = upperCase ? UpperCaseChars : LowerCaseChars;
        }

        public Base32Formatter(string chars)
        {
            if (chars.Length != 32)
                throw new ArgumentException("The character string must be exactly 32 characters");

            _chars = chars;
        }

        public string Format(byte[] bytes)
        {
            var result = new char[26];

            int offset = 0;
            long number;
            for (int i = 0; i < 3; i++)
            {
                int indexed = i * 5;
                number = bytes[indexed] << 12 | bytes[indexed + 1] << 4 | bytes[indexed + 2] >> 4;
                ConvertLongToBase32(result, offset, number, 4, _chars);

                offset += 4;

                number = (bytes[indexed + 2] & 0xf) << 16 | bytes[indexed + 3] << 8 | bytes[indexed + 4];
                ConvertLongToBase32(result, offset, number, 4, _chars);

                offset += 4;
            }

            ConvertLongToBase32(result, offset, bytes[15], 2, _chars);

            return new string(result, 0, 26);
        }

        static void ConvertLongToBase32(char[] buffer, int offset, long value, int count, string chars)
        {
            for (int i = count - 1; i >= 0; i--)
            {
                var index = (int)(value % 32);
                buffer[offset + i] = chars[index];
                value /= 32;
            }
        }
    }
}