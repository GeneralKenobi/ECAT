using System;

namespace ECAT.Core
{
	/// <summary>
	/// Constructors with this attribute, when used by IoC, will have missing parameters auto-resolved by container
	/// </summary>
	[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited =false)]
    public class Autowire : Attribute { }
}