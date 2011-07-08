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
namespace MassTransit.Util
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Indicates that marked element should be localized or not.
	/// </summary>
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	public sealed class LocalizationRequiredAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="LocalizationRequiredAttribute"/> class.
		/// </summary>
		/// <param name="required"><c>true</c> if a element should be localized; otherwise, <c>false</c>.</param>
		public LocalizationRequiredAttribute(bool required)
		{
			Required = required;
		}

		/// <summary>
		/// Gets a value indicating whether a element should be localized.
		/// <value><c>true</c> if a element should be localized; otherwise, <c>false</c>.</value>
		/// </summary>
		public bool Required { get; set; }

		/// <summary>
		/// Returns whether the value of the given object is equal to the current <see cref="LocalizationRequiredAttribute"/>.
		/// </summary>
		/// <param name="obj">The object to test the value equality of. </param>
		/// <returns>
		/// <c>true</c> if the value of the given object is equal to that of the current; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj)
		{
			var attribute = obj as LocalizationRequiredAttribute;
			return attribute != null && attribute.Required == Required;
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>A hash code for the current <see cref="LocalizationRequiredAttribute"/>.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}


	/// <summary>
	/// Indicates that marked method builds string by format pattern and (optional) arguments. 
	/// Parameter, which contains format string, should be given in constructor.
	/// The format string should be in <see cref="string.Format(IFormatProvider,string,object[])"/> -like form
	/// </summary>
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public sealed class StringFormatMethodAttribute : Attribute
	{
		private readonly string myFormatParameterName;

		/// <summary>
		/// Initializes new instance of StringFormatMethodAttribute
		/// </summary>
		/// <param name="formatParameterName">Specifies which parameter of an annotated method should be treated as format-string</param>
		public StringFormatMethodAttribute(string formatParameterName)
		{
			myFormatParameterName = formatParameterName;
		}

		/// <summary>
		/// Gets format parameter name
		/// </summary>
		public string FormatParameterName
		{
			get { return myFormatParameterName; }
		}
	}


	/// <summary>
	/// Indicates that the function argument should be string literal and match one  of the parameters of the caller function.
	/// For example, <see cref="ArgumentNullException"/> has such parameter.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
	public sealed class InvokerParameterNameAttribute : Attribute
	{
	}


	/// <summary>
	/// Indicates that the marked method is assertion method, i.e. it halts control flow if one of the conditions is satisfied. 
	/// To set the condition, mark one of the parameters with <see cref="AssertionConditionAttribute"/> attribute
	/// </summary>
	/// <seealso cref="AssertionConditionAttribute"/>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public sealed class AssertionMethodAttribute : Attribute
	{
	}


	/// <summary>
	/// Indicates the condition parameter of the assertion method. 
	/// The method itself should be marked by <see cref="AssertionMethodAttribute"/> attribute.
	/// The mandatory argument of the attribute is the assertion type.
	/// </summary>
	/// <seealso cref="AssertionConditionType"/>
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
	public sealed class AssertionConditionAttribute : Attribute
	{
		private readonly AssertionConditionType myConditionType;

		/// <summary>
		/// Initializes new instance of AssertionConditionAttribute
		/// </summary>
		/// <param name="conditionType">Specifies condition type</param>
		public AssertionConditionAttribute(AssertionConditionType conditionType)
		{
			myConditionType = conditionType;
		}

		/// <summary>
		/// Gets condition type
		/// </summary>
		public AssertionConditionType ConditionType
		{
			get { return myConditionType; }
		}
	}


	/// <summary>
	/// Specifies assertion type. If the assertion method argument satisifes the condition, then the execution continues. 
	/// Otherwise, execution is assumed to be halted
	/// </summary>
	public enum AssertionConditionType
	{
		/// <summary>
		/// Indicates that the marked parameter should be evaluated to true
		/// </summary>
		IS_TRUE = 0,

		/// <summary>
		/// Indicates that the marked parameter should be evaluated to false
		/// </summary>
		IS_FALSE = 1,

		/// <summary>
		/// Indicates that the marked parameter should be evaluated to null value
		/// </summary>
		IS_NULL = 2,

		/// <summary>
		/// Indicates that the marked parameter should be evaluated to not null value
		/// </summary>
		IS_NOT_NULL = 3,
	}


	/// <summary>
	/// Indicates that the marked method unconditionally terminates control flow execution.
	/// For example, it could unconditionally throw exception
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public sealed class TerminatesProgramAttribute : Attribute
	{
	}


	/// <summary>
	/// Indicates that the value of marked element could be <c>null</c> sometimes, so the check for <c>null</c> is necessary before its usage
	/// </summary>
	[AttributeUsage(
		AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Delegate
		| AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public sealed class CanBeNullAttribute : Attribute
	{
	}


	/// <summary>
	/// Indicates that the value of marked element could never be <c>null</c>
	/// </summary>
	[AttributeUsage(
		AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Delegate
		| AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public sealed class NotNullAttribute : Attribute
	{
	}


	/// <summary>
	/// Indicates that the value of marked type (or its derivatives) cannot be compared using '==' or '!=' operators.
	/// There is only exception to compare with <c>null</c>, it is permitted
	/// </summary>
	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false,
		Inherited = true)]
	public sealed class CannotApplyEqualityOperatorAttribute : Attribute
	{
	}


	/// <summary>
	/// When applied to target attribute, specifies a requirement for any type which is marked with 
	/// target attribute to implement or inherit specific type or types
	/// </summary>
	/// <example>
	/// <code>
	/// [BaseTypeRequired(typeof(IComponent)] // Specify requirement
	/// public class ComponentAttribute : Attribute 
	/// {}
	/// 
	/// [Component] // ComponentAttribute requires implementing IComponent interface
	/// public class MyComponent : IComponent
	/// {}
	/// </code>
	/// </example>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	[BaseTypeRequired(typeof (Attribute))]
	public sealed class BaseTypeRequiredAttribute : Attribute
	{
		private readonly Type[] myBaseTypes;

		/// <summary>
		/// Initializes new instance of BaseTypeRequiredAttribute
		/// </summary>
		/// <param name="baseTypes">Specifies which types are required</param>
		public BaseTypeRequiredAttribute(params Type[] baseTypes)
		{
			myBaseTypes = baseTypes;
		}

		/// <summary>
		/// Gets enumerations of specified base types
		/// </summary>
		public IEnumerable<Type> BaseTypes
		{
			get { return myBaseTypes; }
		}
	}


	/// <summary>
	/// Indicates that the marked symbol is used implicitly (e.g. via reflection, in external library),
	/// so this symbol will not be marked as unused (as well as by other usage inspections)
	/// </summary>
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	public sealed class UsedImplicitlyAttribute : Attribute
	{
		[UsedImplicitly]
		public UsedImplicitlyAttribute()
			: this(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Default)
		{
		}

		[UsedImplicitly]
		public UsedImplicitlyAttribute(ImplicitUseKindFlags useKindFlags, ImplicitUseTargetFlags targetFlags)
		{
			UseKindFlags = useKindFlags;
			TargetFlags = targetFlags;
		}

		[UsedImplicitly]
		public UsedImplicitlyAttribute(ImplicitUseKindFlags useKindFlags)
			: this(useKindFlags, ImplicitUseTargetFlags.Default)
		{
		}

		[UsedImplicitly]
		public UsedImplicitlyAttribute(ImplicitUseTargetFlags targetFlags)
			: this(ImplicitUseKindFlags.Default, targetFlags)
		{
		}

		[UsedImplicitly]
		public ImplicitUseKindFlags UseKindFlags { get; private set; }

		/// <summary>
		/// Gets value indicating what is meant to be used
		/// </summary>
		[UsedImplicitly]
		public ImplicitUseTargetFlags TargetFlags { get; private set; }
	}


	/// <summary>
	/// Should be used on attributes and causes ReSharper to not mark symbols marked with such attributes as unused (as well as by other usage inspections)
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class MeansImplicitUseAttribute : Attribute
	{
		[UsedImplicitly]
		public MeansImplicitUseAttribute()
			: this(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Default)
		{
		}

		[UsedImplicitly]
		public MeansImplicitUseAttribute(ImplicitUseKindFlags useKindFlags, ImplicitUseTargetFlags targetFlags)
		{
			UseKindFlags = useKindFlags;
			TargetFlags = targetFlags;
		}

		[UsedImplicitly]
		public MeansImplicitUseAttribute(ImplicitUseKindFlags useKindFlags)
			: this(useKindFlags, ImplicitUseTargetFlags.Default)
		{
		}

		[UsedImplicitly]
		public MeansImplicitUseAttribute(ImplicitUseTargetFlags targetFlags)
			: this(ImplicitUseKindFlags.Default, targetFlags)
		{
		}

		[UsedImplicitly]
		public ImplicitUseKindFlags UseKindFlags { get; private set; }

		/// <summary>
		/// Gets value indicating what is meant to be used
		/// </summary>
		[UsedImplicitly]
		public ImplicitUseTargetFlags TargetFlags { get; private set; }
	}


	[Flags]
	public enum ImplicitUseKindFlags
	{
		Default = Access | Assign | Instantiated,

		/// <summary>
		/// Only entity marked with attribute considered used
		/// </summary>
		Access = 1,

		/// <summary>
		/// Indicates implicit assignment to a member
		/// </summary>
		Assign = 2,

		/// <summary>
		/// Indicates implicit instantiation of a type
		/// </summary>
		Instantiated = 4,
	}


	/// <summary>
	/// Specify what is considered used implicitly when marked with <see cref="MeansImplicitUseAttribute"/> or <see cref="UsedImplicitlyAttribute"/>
	/// </summary>
	[Flags]
	public enum ImplicitUseTargetFlags
	{
		Default = Itself,

		Itself = 1,

		/// <summary>
		/// Members of entity marked with attribute are considered used
		/// </summary>
		Members = 2,

		/// <summary>
		/// Entity marked with attribute and all its members considered used
		/// </summary>
		WithMembers = Itself | Members
	}
}