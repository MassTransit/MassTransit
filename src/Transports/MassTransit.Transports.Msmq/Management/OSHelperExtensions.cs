// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports.Msmq.Management
{
	using System;
	using System.Diagnostics;
	using System.Runtime.InteropServices;

	/// <summary>
	/// Provides extensions to <see cref="OperatingSystem"/> and <see cref="Process"/>
	/// that detects whether the process and the OS are 64 bit.
	/// </summary>
	public static class OSHelperExtensions
	{
		/// <summary>
		/// The function determines whether the current operating system is a
		/// 64-bit operating system.
		/// </summary>
		/// <returns>
		/// The function returns true if the operating system is 64-bit;
		/// otherwise, it returns false.
		/// </returns>
		public static bool IsWin64BitOS(this OperatingSystem os)
		{
			if (IntPtr.Size == 8)
				// 64-bit programs run only on Win64
				return true;
			else// 32-bit programs run on both 32-bit and 64-bit Windows
			{   // Detect whether the current process is a 32-bit process    
				// running on a 64-bit system.
				return Process.GetCurrentProcess().Is64BitProc();
			}
		}

		/// <summary>
		/// Checks if the process is 64 bit
		/// </summary>
		/// <returns>
		/// The function returns true if the process is 64-bit;
		/// otherwise, it returns false.
		/// </returns>
		public static bool Is64BitProc(this Process p)
		{
			// 32-bit programs run on both 32-bit and 64-bit Windows
			// Detect whether the current process is a 32-bit process
			// running on a 64-bit system.
			bool result;
			return ((DoesWin32MethodExist("kernel32.dll", "IsWow64Process") && IsWow64Process(p.Handle, out result)) && result);
		}

		/// <summary>
		/// The function determins whether a method exists in the export
		/// table of a certain module.
		/// </summary>
		/// <param name="moduleName">The name of the module</param>
		/// <param name="methodName">The name of the method</param>
		/// <returns>
		/// The function returns true if the method specified by methodName
		/// exists in the export table of the module specified by moduleName.
		/// </returns>
		static bool DoesWin32MethodExist(string moduleName, string methodName)
		{
			IntPtr moduleHandle = GetModuleHandle(moduleName);
			if (moduleHandle == IntPtr.Zero)
				return false;
			return (GetProcAddress(moduleHandle, methodName) != IntPtr.Zero);
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		static extern IntPtr GetModuleHandle(string moduleName);

		[DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
		static extern IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)]string procName);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool IsWow64Process(IntPtr hProcess, out bool wow64Process);
	}
}