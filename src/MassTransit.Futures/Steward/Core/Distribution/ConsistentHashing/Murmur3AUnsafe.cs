// Copyright 2007-2013 Chris Patterson
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
namespace MassTransit.Steward.Core.Distribution.ConsistentHashing
{
    using System;


    public class Murmur3AUnsafe :
        HashGenerator
    {
        const uint Seed = 0xc58f1a7b;

        const UInt32 c1 = 0xcc9e2d51;
        const UInt32 c2 = 0x1b873593;

        public unsafe uint Hash(string s)
        {
            char[] data = s.ToCharArray();
            fixed (char* input = &data[0])
            {
                return Hash((byte*)input, (uint)data.Length * sizeof(char), Seed);
            }
        }

        public unsafe uint Hash(byte[] data)
        {
            fixed (byte* input = &data[0])
            {
                return Hash(input, (uint)data.Length, Seed);
            }
        }

        public unsafe uint Hash(byte[] data, int offset, uint count, uint seed)
        {
            fixed (byte* input = &data[offset])
            {
                return Hash(input, count, seed);
            }
        }

        static unsafe uint Hash(byte* data, uint len, uint seed)
        {
            UInt32 nblocks = len / 4;
            UInt32 h1 = seed;

            //----------
            // body

            UInt32 k1;
            var block = (UInt32*)data;
            for (UInt32 i = nblocks; i > 0; --i, ++block)
            {
                k1 = *block;

                k1 *= c1;
                k1 = Rotl32(k1, 15);
                k1 *= c2;

                h1 ^= k1;
                h1 = Rotl32(h1, 13);
                h1 = h1 * 5 + 0xe6546b64;
            }

            //----------
            // tail


            k1 = 0;
            uint rem = len & 3;
            var tail = (byte*)block;
            if (rem >= 3)
                k1 ^= (uint)(tail[2] << 16);
            if (rem >= 2)
                k1 ^= (uint)(tail[1] << 8);
            if (rem > 0)
            {
                k1 ^= tail[0];
                k1 *= c1;
                k1 = Rotl32(k1, 15);
                k1 *= c2;
                h1 ^= k1;
            }

            //----------
            // finalization

            h1 ^= len;

            h1 ^= h1 >> 16;
            h1 *= 0x85ebca6b;
            h1 ^= h1 >> 13;
            h1 *= 0xc2b2ae35;
            h1 ^= h1 >> 16;

            return h1;
        }

        static UInt32 Rotl32(UInt32 x, int r)
        {
            return (x << r) | (x >> (32 - r));
        }
    }
}