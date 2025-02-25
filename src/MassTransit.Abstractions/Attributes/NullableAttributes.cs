using System;

#if !NET6_0_OR_GREATER && !NETSTANDARD2_1
namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Parameter)]
    sealed class NotNullWhenAttribute :
        Attribute
    {
        /// <summary>Initializes the attribute with the specified return value condition.</summary>
        /// <param name="returnValue">
        /// The return value condition. If the method returns this value, the associated parameter will not be null.
        /// </param>
        public NotNullWhenAttribute(bool returnValue)
        {
            ReturnValue = returnValue;
        }

        /// <summary>Gets the return value condition.</summary>
        public bool ReturnValue { get; }
    }
}


[AttributeUsage(AttributeTargets.Parameter)]
sealed class MaybeNullWhenAttribute :
    Attribute
{
    /// <summary>Initializes the attribute with the specified return value condition.</summary>
    /// <param name="returnValue">
    /// The return value condition. If the method returns this value, the associated parameter will not be null.
    /// </param>
    public MaybeNullWhenAttribute(bool returnValue)
    {
        ReturnValue = returnValue;
    }

    /// <summary>Gets the return value condition.</summary>
    public bool ReturnValue { get; }
}
#endif
