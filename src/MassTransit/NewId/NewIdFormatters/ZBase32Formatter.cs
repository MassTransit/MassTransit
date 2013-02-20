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
    public class ZBase32Formatter :
        Base32Formatter
    {
        // taken from analysis done at http://philzimmermann.com/docs/human-oriented-base-32-encoding.txt
        const string LowerCaseChars = "ybndrfg8ejkmcpqxot1uwisza345h769";
        const string UpperCaseChars = "YBNDRFG8EJKMCPQXOT1UWISZA345H769";

        public ZBase32Formatter(bool upperCase = false)
            : base(upperCase ? UpperCaseChars : LowerCaseChars)
        {
        }
    }
}